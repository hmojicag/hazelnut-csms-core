namespace Hazelnut.Core.HCloudStorageServices {
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Drive.v3;
    using GData = Google.Apis.Drive.v3.Data;
    using Google.Apis.Services;
    using Hazelnut.Core.HFiles;
    using System.Text;
    using System.Threading;
    using System.Net.Http;
    using Google.Apis.Download;
    using Hazelnut.CLIApp.Exceptions;

    public sealed class HCloudStorageServiceGDrive : HCloudStorageService {
        public HCloudStorageServiceGDrive(HCloudStorageServiceData data)
            : base(data) { }

        private readonly string[] Scopes = {  DriveService.Scope.Drive };            //Access to all drive
        private readonly string ApplicationName = "Hazelnut CSMS";
        private Dictionary<string, string> folderStructure;
        private DriveService gDriveService;
        private const string REQUEST_FILE_FIELDS = "id, name, mimeType, modifiedTime, trashed, " +
                                                 "parents, fileExtension, size";        

        public override void InitializeService() {
            if (data is HCloudStorageServiceDataGDrive) {
                gDriveAuthenticationFlow();
            }
            else {
                throw new ArgumentException(
                    @"Data passed in is not of type HGDriveCloudStorageServiceData or 
                    DataStore is null"
                );
            }
        }

        public override async Task<bool> FetchFileStructure() {
            string pageToken = null;
            var hierarchyDictionary = new Dictionary<string, List<GData.File>>();
            var folderList = new Dictionary<string, GData.File>();
            var hierarchyFolderDictionary = new Dictionary<string, List<GData.File>>();
            do
            {
                var request = gDriveService.Files.List();
                request.Q = "trashed = false";                //Exclude trashed files and folders
                request.Spaces = "drive";
                request.Fields = "nextPageToken, files(" + REQUEST_FILE_FIELDS + ")";
                request.PageToken = pageToken;
                var result = await request.ExecuteAsync();
                foreach (var file in result.Files) {
                    //Remember that in Google Drive, folders are files with a different mime type
                    if (file.MimeType.Equals(AcceptedContentTypes["folder"])) {
                        //store id/file (folder) pair
                        folderList.Add(file.Id, file);
                    }
                    
                    //Store (parent folder id)/(list of files in that folder) pair
                    var parentFolderId = file.Parents[0];
                    var fileList = hierarchyDictionary.ContainsKey(parentFolderId)
                        ? hierarchyDictionary[parentFolderId]
                        : new List<GData.File>();
                    fileList.Add(file);
                    hierarchyDictionary[parentFolderId] = fileList;
                }
                pageToken = result.NextPageToken;
            } while (pageToken != null);

            var ok = CreateFileStructure(folderList, hierarchyDictionary);

            if (ok) {
                FileStructure.CloudStorageId = CloudStorageServiceId;
                Console.WriteLine("File structure fetched correctly from Google Drive");
            } else {
                Console.WriteLine("An error happened trying to fetch the file structure from Google Drive");
            }
            
            return ok;
        }

        public override async Task<HFile> CreateFile(HFile file) {

            if (file == null) {
                throw new ArgumentException("HFile is null");
            }

            if (FileStructure.Contains(file.FullFileName)) {
                Console.WriteLine("The file already exists in this Google Drive account: {0}", file.FullFileName);
                return FileStructure.GetFile(file.FullFileName);
            }
            
            if (!IsFetched) {
                await FetchFileStructure();
            }

            string fullPath;
            var fullFileName = file.FullFileName;
            try {
                fullPath = fullFileName.Substring(0, fullFileName.LastIndexOf('/')+1);
            } catch (Exception ex) {
                Console.WriteLine("The field: {0} is not correctly formatted. Error:\n {1}", fullFileName, ex);
                return null;
            }
            
            var parentFolderId = "";
            if (file is HFileGDrive) {
                parentFolderId = ((HFileGDrive) file).GDriveParentFolderId;
            }
            
            if (string.IsNullOrEmpty(parentFolderId)) {
                parentFolderId = RecursiveFolderCreation(fullPath);
            }

            MemoryStream streamContent = null;
            if (!file.isDownloaded) {
                await file.DownloadContentAsync();
            }
            streamContent = file.Content;

            var contentType = file.MimeType;
            if (string.IsNullOrEmpty(contentType)) {
                contentType = GuessContentTypeByExt(file.FileExtension);
            }
            
            //ACTUALLY CREATES THE FILE IN GOOGLE DRIVE
            
            var gDriveFileMetadata = new GData.File() {
                Name = file.FileName,
                Parents = new List<string> {
                    parentFolderId
                }
            };
            
            var request = gDriveService.Files.Create(
                gDriveFileMetadata, streamContent, contentType);
            request.Fields = REQUEST_FILE_FIELDS;
            await request.UploadAsync();
            var newFile = request.ResponseBody;
            Console.WriteLine("File {0} created in Google Drive", file.FullFileName);
            var newGDriveFile = new HFileGDrive(newFile, fullPath, this);
            FileStructure.Add2FileStructure(newGDriveFile);
            return newGDriveFile;
        }

        private string RecursiveFolderCreation(string folderPath) {
            if (folderStructure.ContainsKey(folderPath)) {
                return folderStructure[folderPath];
            }
            
            //Get last folder in the folderPath and create it
            string currentFolderName;
            string remainingPath;
            if (!folderPath.EndsWith("/")) {
                Console.WriteLine("Folder path incorrectly formatted. It should end with a '/' but found: {0}", folderPath);
                return null;
            }
            var noTrailingSlashPath = folderPath.TrimEnd('/');//Get rid of last slash
            try {
                currentFolderName = noTrailingSlashPath.Substring(noTrailingSlashPath.LastIndexOf('/') + 1);
                remainingPath = noTrailingSlashPath.Substring(0, noTrailingSlashPath.LastIndexOf('/') + 1);
            } catch (Exception ex) {
                Console.WriteLine("Something went wrong parsing folder path:\n{0}", ex);
                return null;
            }

            //Call recursively in case we also need to create the parent for this one
            var parentFolderId = RecursiveFolderCreation(remainingPath);
            var newFolderId = CreateFolder(parentFolderId, currentFolderName);
            folderStructure.Add(folderPath, newFolderId);

            return newFolderId;
        }

        private string CreateFolder(string parentFolderId, string folderName) {
            var fileMetadata = new GData.File()
            {
                Name = folderName,
                MimeType = AcceptedContentTypes["folder"],
                Parents = new List<string> {
                    parentFolderId
                }
            };
            var request = gDriveService.Files.Create(fileMetadata);
            request.Fields = "id";
            GData.File file;
            try {
                file = request.Execute();
                Console.WriteLine("Folder created: {0} with id {1}", folderName, file.Id);
            } catch (Exception ex) {
                Console.WriteLine("Unexpected error trying to create folder {0}.\n {1}", folderName, ex);
                return null;
            }
            return file.Id;
        }

        private string GuessContentTypeByExt(string extension) {
            string contentType;
            var extensionNoDot = extension.TrimStart('.');
            if (AcceptedContentTypes.ContainsKey(extensionNoDot)) {
                contentType = AcceptedContentTypes[extensionNoDot];
            } else {
                contentType = AcceptedContentTypes["default"];
            }
            return contentType;
        }

        public override async Task<bool> DeleteFile(HFile file) {

            if (file == null) {
                Console.WriteLine("Cannot delete null Google Drive file.");
                return false;
            }

            if (string.IsNullOrEmpty(file.FullFileName)) {
                Console.WriteLine("ERROR trying to dele file from Google Drive. Something is wrong with the full path name");
                return false;
            }

            string gDriveFileId;
            if (file is HFileGDrive) {
                gDriveFileId = (file as HFileGDrive).GDriveId;
            } else {
                if (!IsFetched) {
                    await FetchFileStructure();
                }
                if (FileStructure.Contains(file.FullFileName)) {
                    var file2Delete = FileStructure.GetFile(file.FullFileName) as HFileGDrive;
                    gDriveFileId = file2Delete.GDriveId;
                } else {
                    Console.WriteLine("The file to delete does not exist in Google Drive: {0}", file.FullFileName);
                    return false;
                }
            }

            var request = gDriveService.Files.Delete(gDriveFileId);
            string responseBody = request.Execute();
            
            //If successful, this method returns an empty response body.
            if (string.IsNullOrEmpty(responseBody)) {
                Console.WriteLine("File {0} deleted from Google Drive", file.FullFileName);
                FileStructure.RemoveFromFileStructure(file.FullFileName);
                return true;
            }
            
            Console.WriteLine("Something went wrong trying to delete file {0} from Google Drive", file.FullFileName);
            return false;
        }

        public override async Task<HFile> UpdateFile(HFile file) {
            HFile updatedFile = null;
            if (await DeleteFile(file)) {
                updatedFile = await CreateFile(file);
                Console.WriteLine("File Updated with a delete and create combo");
            } else {
                Console.WriteLine("Error trying to update file: {0}", file.FullFileName);
            }
            return updatedFile;
        }

        public override async Task<MemoryStream> DownloadFileContent(HFile file) {

            if (file == null) {
                Console.WriteLine("HFile is null. Can't download.");
                return null;
            }

            if (!IsFetched) {
                await FetchFileStructure();
            }
            
            HFileGDrive file2Download;
            if (FileStructure.Contains(file.FullFileName)) {
                file2Download = FileStructure.GetFile(file.FullFileName) as HFileGDrive;
            } else {
                Console.WriteLine("Can't download the file {0} cuz it does not exists in the Google Drive account",
                    file.FullFileName);
                return null;
            }

            if (file2Download.isDownloaded) {
                return file2Download.Content;
            }

            var contentStream = new MemoryStream();
            var request = gDriveService.Files.Get(file2Download.GDriveId);
            
            request.MediaDownloader.ProgressChanged+=
                (IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete for file: {0}", file.FullFileName);
                            break;
                        }
                        case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed for file: {0}", file.FullFileName);
                            break;
                        }
                    }
                };
            Console.WriteLine("Downloading file: {0}", file.FullFileName);
            await request.DownloadAsync(contentStream);
            
            return contentStream;
        }

        private void gDriveAuthenticationFlow() {
            var gDriveData = (HCloudStorageServiceDataGDrive) data;
            var clientSecret = gDriveData.GetClientSecrets();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(clientSecret));
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                gDriveData.DataStore).Result;
            
            // Create Drive API service.
            gDriveService = new DriveService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        private bool CreateFileStructure(Dictionary<string, GData.File> folderlDic,
            Dictionary<string,List<GData.File>> hierarchyDictionary) {
            bool rootFolderConflictFlag = (folderlDic.Count != (hierarchyDictionary.Count - 1));
            //folderlDic size must be hierarchyDictionary size - 1 always
            //if not, there's an error.
            //I decided no to throw an exception here because I wanted to find the
            //ids of the conflicting folders.
            
            //Find the root folder
            //This folder must be the one having an id which should not be listed in the folderList.
            var rootFolderid = string.Empty;
            foreach (var parentFolderid in hierarchyDictionary.Keys) {
                if (!folderlDic.ContainsKey(parentFolderid)) {
                    if (string.IsNullOrEmpty(rootFolderid)) {
                        //This is the first and only root folder found
                        rootFolderid = parentFolderid;
                        //If counts are ok, no need to keep iterating
                        if (!rootFolderConflictFlag) break;
                    } else if (!rootFolderid.Equals(parentFolderid)) {
                        //Found a second root folder, something's wrong
                        throw new InvalidDataException("Found more than one root folder id. At least two listed: "
                            + rootFolderid + ", " + parentFolderid);
                    }
                }
            }

            if (string.IsNullOrEmpty(rootFolderid)) {
                throw new InvalidDataException("No root folder found");
            }
            
            //Build the File Structure and the Folder Structure
            var rootPath = "/";
            folderStructure = new Dictionary<string, string>();
            folderStructure.Add(rootPath, rootFolderid);
            var fileStructureDict =
                RecursiveFileStructureBuildImpl(rootPath, hierarchyDictionary[rootFolderid], hierarchyDictionary);
            
            //Set FileStructure
            FileStructure = new HFileStructure(fileStructureDict);
            IsFetched = true;
            return true;
        }

        private Dictionary<string, HFile> RecursiveFileStructureBuildImpl(string currentPath, List<GData.File> files,
            Dictionary<string,List<GData.File>> hierarchyDictionary) {
            var filesInFolder = new Dictionary<string, HFile>();
            try {
                foreach (var file in files) {
                    if (file.MimeType.Equals(AcceptedContentTypes["folder"])) {
                        var subFolderPath = currentPath + file.Name + "/";
                        folderStructure.Add(subFolderPath, file.Id);
                        if (hierarchyDictionary.ContainsKey(file.Id)) {
                            var subFileList = hierarchyDictionary[file.Id];
                            var subFilesInFolder =
                                RecursiveFileStructureBuildImpl(subFolderPath, subFileList, hierarchyDictionary);
                            foreach (var filePath in subFilesInFolder.Keys) {
                                filesInFolder.Add(filePath, subFilesInFolder[filePath]);
                            }
                        }
                    } else {
                        var newGDriveFile = new HFileGDrive(file, currentPath, this);
                        filesInFolder.Add(newGDriveFile.FullFileName, newGDriveFile);
                    }
                }
            } catch (Exception ex) {
                string msg = "Something went wrong building a Google Drive File Structure at path: "
                             + currentPath + "\n" + ex;
                Console.WriteLine(msg);
                throw new InvalidFileStructureException(msg);
            }
            
            return filesInFolder;
        }
        
        
        private readonly Dictionary<string, string> AcceptedContentTypes = new Dictionary<string, string>() {
            {"xls","application/vnd.ms-excel"},
            {"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
            {"xml","text/xml"},
            {"ods","application/vnd.oasis.opendocument.spreadsheet"},
            {"csv","text/plain"},
            {"tmpl","text/plain"},
            {"pdf","application/pdf"},
            {"jpg","image/jpeg"},
            {"png","image/png"},
            {"gif","image/gif"},
            {"bmp","image/bmp"},
            {"txt","text/plain"},
            {"doc","application/msword"},
            {"js","text/js"},
            {"swf","application/x-shockwave-flash"},
            {"mp3","audio/mpeg"},
            {"zip","application/zip"},
            {"rar","application/rar"},
            {"tar","application/tar"},
            {"arj","application/arj"},
            {"cab","application/cab"},
            {"html","text/html"},
            {"htm","text/html"},
            {"default","application/octet-stream"},
            {"folder","application/vnd.google-apps.folder"}
        };
    }
}