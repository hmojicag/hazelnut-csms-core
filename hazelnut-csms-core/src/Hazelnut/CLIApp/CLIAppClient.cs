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

    public class CLIAppClient {
        private int HUserId { get; }
        public CLIAppClient(int hUserId) {
            HUserId = hUserId;
        }

        public void ExecuteSync() {
            using(HazelnutCLIContext hCLIContext = new HazelnutCLIContext()) {

                //Most important variables
                Hazelnut.Core.HUsers.HUser coreUser;
                List<HCloudStorageServiceData> hcssdata;
                HCloudSync hCloudSync;
                HCloudSync.SyncType syncType;

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
                if (!Enum.TryParse<HCloudSync.SyncType>(modelUser.SyncType, out syncType)) {
                    throw new InvalidUserParamsException("User " + HUserId
                        + "Has No Valid Sync Type: " + modelUser.SyncType);
                }
                coreUser = new Hazelnut.Core.HUsers.HUser(modelUser.HUserId, modelUser.HUserName);
                hcssdata = new List<HCloudStorageServiceData>();
                foreach(HCloudStorageDrive drive in modelUser.HCloudStorageDrives) {
                    if (drive.Type.Equals("Dropbox")) {
                        HDropboxCloudStorageServiceData dbxData = 
                            JsonConvert.DeserializeObject<HDropboxCloudStorageServiceData>(drive.Config);
                        hcssdata.Add(dbxData);
                    } else if (drive.Type.Equals("GDrive")) {
                        //Will Do somethinghere soon
                    } else if (drive.Type.Equals("OneDrive")) {
                        //Will Do somethinghere soon
                    } else if (drive.Type.Equals("HazelnutBaseFileStructure")) {
                        //Restore here the Base File Structure
                    }
                }

                //Synchronize Drives
                hCloudSync = new HCloudSync(coreUser, hcssdata);
                if(syncType == HCloudSync.SyncType.DUPLICATED) {
                    HFileStructure baseFileStructure = hCloudSync.ApplyDuplicatedSync();
                    //Update DB
                }
                
            }
        }
    }
}