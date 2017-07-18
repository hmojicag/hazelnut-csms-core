using System.Threading.Tasks;
using Hazelnut.CLIApp.Tools;

namespace Hazelnut.CLIApp {
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    using Hazelnut.Core.HCloudStorageServices;
    using Hazelnut.CLIApp.Model;
    using Hazelnut.CLIApp.Exceptions;
    using Hazelnut.Core;
    using Hazelnut.Core.HFiles;
    using Google.Apis.Util.Store;

    public class CLIAppClient {
        private int HUserId { get; }
        public CLIAppClient(int hUserId) {
            HUserId = hUserId;
        }

        public async Task ExecuteOperationsAsync() {
            using(HazelnutCLIContext hCLIContext = new HazelnutCLIContext()) {

                //Most important variables
                Hazelnut.Core.HUsers.HUser coreUser;
                List<HCloudStorageServiceData> hcssdata;
                HCloudOperations hCloudOperations;
                HCloudOperations.OpType opType;

                //Get the data from SQLite DB
                var modelUsers = hCLIContext.HUsers
                    .Include(user => user.HCloudStorageDrives)
                    .Where(user => user.HUserId == HUserId)
                    .ToList();
                if(modelUsers == null
                    || modelUsers.Count < 1
                    || modelUsers[0].HCloudStorageDrives == null) {
                    throw new NoValidUserFoundException("User " + HUserId
                        + "Not registered in the Hazelnut CSMS CLIApp");
                }
                HUser modelUser = modelUsers[0];
                if (!Enum.TryParse<HCloudOperations.OpType>(modelUser.SyncType, out opType)) {
                    throw new InvalidUserParamsException("User " + HUserId
                        + "Has No Valid Sync Type: " + modelUser.SyncType);
                }
                coreUser = new Hazelnut.Core.HUsers.HUser(modelUser.HUserId, modelUser.HUserName);
                hcssdata = new List<HCloudStorageServiceData>();
                var baseFileStructures = new List<HFileStructure>();
                foreach(HCloudStorageDrive drive in modelUser.HCloudStorageDrives) {
                    if (drive.Type.Equals("Base-DUPLICATE")) {
                        var baseData = JsonConvert.DeserializeObject<HCloudStorageServiceDataBaseDuplicate>(drive.Config);
                        var baseFs = new HFileStructure(baseData.GetBaseFSAsHFileStructure());
                        baseFs.CloudStorageId = drive.HCloudStorageDriveId;
                        baseFileStructures.Add(baseFs);
                    } else if (drive.Type.Equals("Dropbox")) {
                        var dbxData = JsonConvert.DeserializeObject<HCloudStorageServiceDataDropbox>(drive.Config);
                        hcssdata.Add(dbxData);
                    } else if (drive.Type.Equals("GDrive")) {
                        var gDriveData = JsonConvert.DeserializeObject<HCloudStorageServiceDataGDrive>(drive.Config);
                        //TODO: Change the FileDataStore obj for a GDriveDataStore one you implement it
                        gDriveData.DataStore = new FileDataStore("/Users/hmojica/HazelnutCSMS/.credentials/drive-dotnet-quickstart.json", true);
                        hcssdata.Add(gDriveData);
                    } else if (drive.Type.Equals("OneDrive")) {
                        //Will Do somethinghere soon
                    } else if (drive.Type.Equals("HazelnutBaseFileStructure")) {
                        //Restore here the Base File Structure
                    }
                }

                //Synchronize Drives
                hCloudOperations = new HCloudOperations(coreUser, hcssdata);
                if(opType == HCloudOperations.OpType.DUPLICATED) {
                    var baseFileStructure = await hCloudOperations.ApplyDuplicationAsync(baseFileStructures);
                    //Update DB
                }
                
            }
        }
    }
}