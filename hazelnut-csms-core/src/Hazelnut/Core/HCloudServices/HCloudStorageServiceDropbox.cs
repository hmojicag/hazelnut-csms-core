namespace Hazelnut.Core.HCloudStorageServices {
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Hazelnut.Core.HFiles;
    using Hazelnut.Core.DropboxApiV2;
    using Hazelnut.Core.DropboxApiV2.Files;

    public sealed class HCloudStorageServiceDropbox : HCloudStorageService {

        private string oauth2AccessToken;
        private DropboxClient dropboxClient;

        public HCloudStorageServiceDropbox(HCloudStorageServiceData data)
            : base(data) { }

        public override void InitializeService() {
            if (data is HCloudStorageServiceDataDropbox) {
                HCloudStorageServiceDataDropbox dbxData = (HCloudStorageServiceDataDropbox)data;
                oauth2AccessToken = dbxData.Oauth2AccessToken;
                dropboxClient = new DropboxClient(oauth2AccessToken);
            } else {
                throw new ArgumentException(
                    "Data passed in is not of type HCloudStorageServiceDataDropbox"
                );
            }
        }

        public override async Task<bool> FetchFileStructure() {
            List<Metadata> dbxFilesMetadata = await dropboxClient.ListFullDropBoxAsync();
            FileStructure = new HFileStructure();
            foreach(Metadata dbxMetadata in dbxFilesMetadata) {
                if (dbxMetadata is FileMetadata) {
                    HFileDropbox dbxFile = new HFileDropbox((FileMetadata)dbxMetadata, this);
                    FileStructure.Add2FileStructure(dbxFile);
                }
            }
            Console.WriteLine("File structure correctly fetched from {0}", dropboxClient.ToString());
            FileStructure.CloudStorageId = CloudStorageServiceId;
            return IsFetched = true;
        }

        public override async Task<HFile> CreateFile(HFile file) {
            if (file != null && !string.IsNullOrEmpty(file.FullFileName)) {
                if (!file.isDownloaded) {
                    await file.DownloadContentAsync();
                }
                FileMetadata newFile = await
                    dropboxClient.UploadAsync(file.Content, file.FullFileName);
                Console.WriteLine("File {0} created for client {1}", file.FullFileName, dropboxClient.ToString());
                return new HFileDropbox(newFile, this);
            }
            return null;
        }
        
        public override async Task<bool> DeleteFile(HFile file) {
            if (file == null) {
                return false;
            }
            Metadata metadata = await dropboxClient.DeleteAsync(file.FullFileName);
            Console.WriteLine("File {0} deleted for client {1}", file.FullFileName, dropboxClient.ToString());
            return metadata != null && metadata.PathDisplay.Equals(file.FullFileName);
        }

        public override async Task<HFile> UpdateFile(HFile file) {
            if (await DeleteFile(file)) {
                return await CreateFile(file);
            }
            return null;
        }

        public override async Task<MemoryStream> DownloadFileContent(HFile file) {
            if (file == null || !string.IsNullOrEmpty(file.FullFileName)) {
                Task<MemoryStream> t = dropboxClient.DownloadAsync(file.FullFileName);
                Console.WriteLine("File {0} Downloaded for client {1}", file.FullFileName, dropboxClient.ToString());
                MemoryStream memoryStream = await t;
                return memoryStream;
            } else {
                throw new ArgumentException("Hfile file instance is null or FullFileName propertie is not compliant");
            }
        }

    }
}