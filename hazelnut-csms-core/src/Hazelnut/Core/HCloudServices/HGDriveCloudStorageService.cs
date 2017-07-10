namespace Hazelnut.Core.HCloudStorageServices {
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Drive.v3;
    using Google.Apis.Drive.v3.Data;
    using Google.Apis.Services;
    using Google.Apis.Util.Store;

    using Hazelnut.Core.HFiles;

    public sealed class HGDriveCloudStorageService : HCloudStorageService {

        public HGDriveCloudStorageService(HCloudStorageServiceData data)
            : base(data) { }

        public IDataStore DataStore { get; set; }

        public override void InitializeService() {
            if (data is HGDriveCloudStorageServiceData && DataStore != null) {

            } else {
                throw new ArgumentException(
                    @"Data passed in is not of type HGDriveCloudStorageServiceData or 
                    DataStore is null"
                );
            }
        }

        public override async Task<bool> FetchFileStructure() {
            
            return false;
        }

        public override async Task<bool> CreateFile(HFile file) {
            return false;
        }
        
        public override async Task<bool> DeleteFile(HFile file) {
            return false;
        }

        public override async Task<bool> UpdateFile(HFile file) {
            return false;
        }

        public override async Task<MemoryStream> DownloadFileContent(HFile file) {
            return null;
        }
    }
}