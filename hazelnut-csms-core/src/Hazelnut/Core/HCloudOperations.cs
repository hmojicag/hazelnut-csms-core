using Hazelnut.Core.DropboxApiV2;
using Hazelnut.Core.DropboxApiV2.Files;
using System.Threading.Tasks;
using Hazelnut.Core.DropboxApiV2.Users;
using System;
using System.IO;
using System.Text;

using Hazelnut.Core.HCloudStorageServices;
using Hazelnut.Core.HUsers;
using Hazelnut.Core.HFiles;
using System.Collections.Generic;
using System.Linq;

namespace Hazelnut.Core {
    public class HCloudOperations {

        public enum OpType {
            DUPLICATED,
            DISTRIBUTED
        }
        
        /**
        * Set a time offset of x milliseconds.
        * Between these offsset time two different files with the same name
        * on different drives will be treated the same
        */
        private const int OffsetMilliseconds = 300000;//5min Time span

        private HUser user;
        private List<HCloudStorageService> hcssList;

        public HCloudOperations(HUser _user, List<HCloudStorageServiceData> hcssData) {
            this.user = _user;
            hcssList = new List<HCloudStorageService>();
            foreach (HCloudStorageServiceData data in hcssData) {
                if(data is HCloudStorageServiceDataDropbox) {
                    hcssList.Add(new HCloudStorageServiceDropbox(data));
                } else if (data is HCloudStorageServiceDataGDrive) {
                    hcssList.Add(new HCloudStorageServiceGDrive(data));
                }
            }
        }

        public async Task<HFileStructure> ApplyDuplicationAsync(List<HFileStructure> baseFileStructures) {

            if (baseFileStructures == null || baseFileStructures.Count < 1) {
                Console.WriteLine("Cannot apply Duplication without a Base file structure");
                return null;
            }
            
            //Asynchronously Init and Fetch file structures from each cloud service
            var fetchServices = new List<Task<bool>>();
            foreach (HCloudStorageService hcss in hcssList) {
                hcss.InitializeService();
                fetchServices.Add(hcss.FetchFileStructure());
            }
            //Wait for all services to fetch their FileStructure
            await Task.WhenAll(fetchServices);

            var baseFileStructure = baseFileStructures[0];
            var actionList = new List<HCloudAction>();
            GetActionList4Duplication(baseFileStructure, actionList);
            await ApplyActionList(actionList);

            return baseFileStructure;
        }

        private async Task ApplyActionList(List<HCloudAction> actionList) {
            var actionListTasks = new List<Task>();
            foreach (var action in actionList) {
                Task actionTask;
                switch (action.Action2Apply) {
                    case HCloudAction.ActionType.CREATE:
                        actionTask = action.CloudStorageService.CreateFile(action.ActionFile);
                    break;
                            
                    case HCloudAction.ActionType.UPDATE:
                        actionTask = action.CloudStorageService.UpdateFile(action.ActionFile);
                    break;

                    case HCloudAction.ActionType.REMOVE:
                        actionTask = action.CloudStorageService.DeleteFile(action.ActionFile);
                    break;
                        
                    default:
                        Console.WriteLine("Unknow HCloudAction.ActionType: {0}", action.Action2Apply);
                        actionTask = Task.CompletedTask;
                    break;
                }
                actionListTasks.Add(actionTask);
            }
            //Wait for all the threads/tasks to complete
            await Task.WhenAll(actionListTasks);
        }

        //Both arguments will be updated inside this method
        private void GetActionList4Duplication(HFileStructure baseFileStructure, 
            List<HCloudAction> actionList)  {

            //FIRST ITERATION.
            //UPDATE THE BASE FILE STRUCTURE IN ORDER TO FIND NEW FILES AND UPDATES
            foreach (var cloudStorageService in hcssList) {//LOOP-1ITER
                //Iterate through all the Cloud Storages looking for updates
                foreach (var fileFullPath in cloudStorageService.FileStructure.GetFilesFullPath()) {
                    //Compare every file in the Cloud Storage against the Base Metadata
                    HFile fileInCloudStorageService = cloudStorageService.FileStructure.GetFile(fileFullPath);
                    if (baseFileStructure.Contains(fileFullPath)) {
                        if (AreDifferent(baseFileStructure.GetFile(fileFullPath), fileInCloudStorageService)) {
                            //The file was updated in this Cloud Storage drive.
                            //Update the BaseFS accordingly
                            var mostRecentFile = GetMostRecentFile(baseFileStructure.GetFile(fileFullPath),
                                fileInCloudStorageService);
                            baseFileStructure.SetFile(mostRecentFile);
                        } else {
                            //The files are identical in terms of the offset time span and size
                            //Do NOTHING
                        }
                    } else {
                        //Base Metadata does not exist for this file (New File added).
                        //Or an even newer file was added via other drive
                        //Ex. A new file was added to Dropbox at 18/July/2017 23:00:00
                        //And the same file was added to Google Drive
                        //Add a reference to the Base Metadata.
                        baseFileStructure.Add2FileStructure(fileInCloudStorageService);
                    }
                }
            }//END LOOP-1ITER
            
            //SECOND ITERATION
            //FIND THE FILES REMOVED FROM CLOUD STORAGES DRIVES AND
            //CREATE THE ACTIONS TO APPLY
            foreach (var filePath in baseFileStructure.GetFilesFullPath()) {//LOOP-2ITER
                var fileInBase = baseFileStructure.GetFile(filePath);
                var deleteFlag = false;
                
                foreach (var cloudStorageService in hcssList)  {//LOOP-HCSS
                    //Compare the File in the Base Structure against each file everywhere
                    var hcssFileStructure = cloudStorageService.FileStructure;
                    if (fileInBase is HFileBase) {
                        //If this is an instance of HFileBase, it means we're on the path where the file
                        //has not been updated or was removed from one of the drives
                        if (!hcssFileStructure.Contains(fileInBase.FullFileName)) {
                            //This file was removed at least from one hcssFileStructure
                            //Delete it everywhere
                            deleteFlag = true;
                            break;
                        }
                    } else {
                        //This file is an instance of a Google Drive, Dropbox or something else. 
                        //It means we're on the path where the file was updated or it's new
                        if (hcssFileStructure.Contains(fileInBase.FullFileName)) {
                            if (hcssFileStructure.GetFile(fileInBase.FullFileName).CloudStorageServiceId
                                != fileInBase.CloudStorageServiceId) {
                                //Add an Update actionType
                                actionList.Add(new HCloudAction(
                                    HCloudAction.ActionType.UPDATE, fileInBase, cloudStorageService));
                            }
                        } else {
                            //Add a Creation actionType
                            actionList.Add(new HCloudAction(
                                HCloudAction.ActionType.CREATE, fileInBase, cloudStorageService));
                        }
                        
                    }
                }//END LOOP-HCSS
                
                if (deleteFlag) {
                    //If Delete flag was triggered, create remove actions for the HCSSDrives as needed
                    foreach (var cloudStorageService in hcssList) {
                        if (cloudStorageService.FileStructure.Contains(fileInBase.FullFileName)) {
                            var file2Remove = cloudStorageService.FileStructure.GetFile(fileInBase.FullFileName);
                            //Read this actionType as REMOVE file2Remove from cloudStorageService
                            actionList.Add(new HCloudAction(
                                HCloudAction.ActionType.REMOVE, file2Remove, cloudStorageService));
                        }
                    }
                    //Mark to remove from base file structure
                    actionList.Add(new HCloudAction(HCloudAction.ActionType.REMOVE, fileInBase, null));
                }
                
            }//END LOOP-2ITER
            
            //THIRD ITERATION
            //Clean up the BaseFS
            var action2BaseFS = actionList.Where(action => action.CloudStorageService == null);
            foreach (var action in action2BaseFS) {
                if (action.Action2Apply == HCloudAction.ActionType.REMOVE) {
                    //Remove from BaseFS
                    baseFileStructure.RemoveFromFileStructure(action.ActionFile.FullFileName);
                }
                //Add here more Actions as needed
            }
            //Remove all actions for BaseFS from the actionList
            actionList.RemoveAll(action => action.CloudStorageService == null);
        }

        //Returns true if the two files are different, false otherwise
        //If one or both are null, true is returned
        private bool AreDifferent(HFile file1, HFile file2) {
            if (file1 == null || file2 == null) {
                return true;
            }
            bool areEqualName = file1.FullFileName.Equals(file2.FullFileName);
            bool areEqualDate = Math.Abs((file1.LastEditDateTime - file2.LastEditDateTime).Milliseconds)
                                <= OffsetMilliseconds;
            bool areEqualSize = file1.Size == file2.Size;//Size in bytes
            return !(areEqualName && areEqualDate && areEqualSize);
        }

        //If both files are equal in terms of Time Span offset it will return file1
        private HFile GetMostRecentFile(HFile file1, HFile file2) {
            if (file1 == null || file2 == null) {
                return null;
            }

            if (Math.Abs((file1.LastEditDateTime - file2.LastEditDateTime).Milliseconds)
                <= OffsetMilliseconds) {
                //Both files are equal in terms of the TimeSpan Offste
                return file1;
            }

            if (file2.LastEditDateTime > file1.LastEditDateTime) {
                //File 2 seems to have a Later date
                return file2;
            } else {
                return file1;
            }
        }

        private class HCloudAction {
            public enum ActionType {
                CREATE,REMOVE,UPDATE
            }

            //You can read an Action as:
            //"APPLY THIS actionType USING THIS actionFile TO THIS cloudStorageService"
            //ex. CREATE dropboxFile in googledrive
            public HCloudAction(ActionType actionType, HFile actionFile, HCloudStorageService cloudStorageService) {
                Action2Apply = actionType;
                ActionFile = actionFile;
                CloudStorageService = cloudStorageService;
            }
            
            public ActionType Action2Apply { get; set; }
            public HFile ActionFile { get; set; }
            public HCloudStorageService CloudStorageService { get; set; }//If null then it points to BaseFS
        }

        //DUMMY CODE THAT SHOULD BE DELETED
        //FROM HERE TO ALL BELOW
        public HCloudOperations() {

            //This is just a test for the DropBox API
            //string oauth2AccessToken = "YkSN6i4mCBAAAAAAAAAAB7ElQjewrG1XmIw9W1tEWDZfofOBjMqWXKUabW76_Yb_";
            //DropboxClient dbx = new DropboxClient(oauth2AccessToken);
            //getAccount(dbx).Wait();
            //ListFullDropBox(dbx).Wait();
            //DownloadAsync(dbx).Wait();
            //UploadAsync(dbx).Wait();
            //GetMetadata(dbx).Wait();
            //DeleteAsync(dbx).Wait();
        }
        
        private async Task dummyFolderOpGDrive(HCloudStorageService hcss) {
            HFileDropbox dbxFile = new HFileDropbox();
            dbxFile.Path = "/autoCreatedFolder/anotherOne/OhhShitCommeOn/";
            dbxFile.FileName = "dummyFile.txt";
            dbxFile.Content = new MemoryStream(new byte[] {72, 65, 90, 65});
            HFile newTestFile = await hcss.CreateFile(dbxFile);
            newTestFile.Content =  new MemoryStream(new byte[] {72, 65, 90, 65, 46});
            HFile updatedFile = await hcss.UpdateFile(newTestFile);
            MemoryStream contentStream = await hcss.DownloadFileContent(updatedFile);
            contentStream.Position = 0;
            Console.WriteLine("HCloudOperations test. File downloaded from GDrive: " + byteArray2String(contentStream.ToArray()));
            await hcss.DeleteFile(updatedFile);
        }

        private async Task dummyFileOpDbx(HCloudStorageService hcss) {
            HFileGDrive gDriveTestFile = new HFileGDrive();
            gDriveTestFile.FileName = "gDriveTestFile.txt";
            gDriveTestFile.Path = "/";
            gDriveTestFile.Content = new MemoryStream(new byte[] {72, 65, 90, 65});
            HFile newTestFile = await hcss.CreateFile(gDriveTestFile);
            newTestFile.Content =  new MemoryStream(new byte[] {72, 65, 90, 65, 46});
            HFile updateTestFile = await hcss.UpdateFile(newTestFile);
            await updateTestFile.DownloadContentAsync();
            Console.WriteLine("File Downloaded: {0}", updateTestFile.Content.ToString());
            await hcss.DeleteFile(updateTestFile);
        }

        private string byteArray2String(byte[] array) {
            return array.Aggregate("", (current, b) => current + Convert.ToString(Convert.ToChar(b)));
        }
        
        private async Task getAccount(DropboxClient dbx) {
            Account account = await dbx.GetCurrentAccountAsync();
        }

        private async Task ListFullDropBox(DropboxClient dbx) {
            await dbx.ListFullDropBoxAsync();
        }

        private async Task DownloadAsync(DropboxClient dbx) {
            MemoryStream memoryStream = await dbx.DownloadAsync("id:-sUUqp8epSAAAAAAAAAABA");
            using (System.IO.FileStream fs = System.IO.File.Create(@"C:\testfolder\lol.pdf"))
            {
                memoryStream.WriteTo(fs);
            }
        }

        private async Task UploadAsync(DropboxClient dbx) {
            MemoryStream memoryStream = new MemoryStream();
            UnicodeEncoding uniEnconding = new UnicodeEncoding();
            byte[] testFileContent = uniEnconding.GetBytes(
                "This is an auto generated test file"
            );
            memoryStream.Write(testFileContent, 0, testFileContent.Length);
            string fileName = "/autoGeneratedTestFile-" + DateTime.Now.ToString("dd-MM-yyyy_hh_mm") + ".txt";
            await dbx.UploadAsync(memoryStream, fileName);
        }

        private async Task GetMetadataAsync(DropboxClient dbx) {
            FileMetadata fileMetadata = await dbx.GetMetadataAsync("/test-lv0-0.txt");
            Console.WriteLine("FileMetadata:");
            Console.WriteLine("Path Lower: " + fileMetadata.PathLower);
            Console.WriteLine("Id: " + fileMetadata.Id);
            Console.WriteLine("Rev: " + fileMetadata.Rev);
        }

        private async Task DeleteAsync(DropboxClient dbx) {
            Metadata metadata = await dbx.DeleteAsync("/autoGeneratedTestFile-06-07-2017_02_46.txt");
            Console.WriteLine("Item deleted: ");
            if(metadata.IsFile) {
                FileMetadata fileMetadata = (FileMetadata)metadata;
                Console.Write("File");
                Console.WriteLine("Path Lower: " + fileMetadata.PathLower);
                Console.WriteLine("Id: " + fileMetadata.Id);
                Console.WriteLine("Rev: " + fileMetadata.Rev);
            } else {
                Console.Write("Folder");
                FolderMetadata folderMetadata = (FolderMetadata)metadata;
                Console.WriteLine("Path Lower: " + folderMetadata.PathLower);
            }
        }

    }
}