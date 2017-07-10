namespace Hazelnut.Core.HCloudStorageServices {
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Hazelnut.Core.HFiles;
    using Hazelnut.Core.DropboxApiV2;
    using Hazelnut.Core.DropboxApiV2.Files;

    public sealed class HDropboxCloudStorageService : HCloudStorageService {

        private string oauth2AccessToken;
        private DropboxClient dropboxClient;
        private List<Metadata> dbxFilesMetadata;

        public HDropboxCloudStorageService(HCloudStorageServiceData data)
            : base(data) { }

        public override void InitializeService() {
            if (data is HDropboxCloudStorageServiceData) {
                HDropboxCloudStorageServiceData dbxData = (HDropboxCloudStorageServiceData)data;
                oauth2AccessToken = dbxData.Oauth2AccessToken;
                dropboxClient = new DropboxClient(oauth2AccessToken);
            } else {
                throw new ArgumentException(
                    "Data passed in is not of type HDropboxCloudStorageServiceData"
                );
            }
        }

        public override bool FetchFileStructure() {
            FetchFileStructureImpl().Wait();
            return false;
        }

        public override bool CreateFile(HFile file) {
            return false;
        }
        
        public override bool DeleteFile(HFile file) {
            return false;
        }

        public override bool UpdateFile(HFile file) {
            return false;
        }

        public override MemoryStream DownloadFileContent(HFile file) {
            return null;
        }

        private async Task FetchFileStructureImpl() {
            dbxFilesMetadata = await dropboxClient.ListFullDropBoxAsync();
        }

    }
}