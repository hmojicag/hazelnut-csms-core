namespace Hazelnut.Core.HCloudStorageServices {
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Hazelnut.Core.HFiles;

    public abstract class HCloudStorageService {

        public enum CloudStorageType {
            Base,
            DropBox,
            GDrive,
            OneDrive
        };

        protected HCloudStorageServiceData data;

        public HCloudStorageService(HCloudStorageServiceData data) {
            if(data == null || string.IsNullOrEmpty(data.HCloudStorageServiceId)) {
                throw new ArgumentException(@"HCloudStorageServiceData and 
                    HCloudStorageServiceId should not be null");
            }

            this.data = data;
            IsFetched = false;
        }

        protected HFileStructure fileStructure { get; set; }
        public string CloudStorageServiceId { 
            get { return data.HCloudStorageServiceId; }
        }
        public bool IsFetched { get; protected set; }
        public CloudStorageType StorageType { get; protected set; }
        public abstract void InitializeService();
        public abstract Task<bool> FetchFileStructure();
        public abstract Task<HFile> CreateFile(HFile file);
        public abstract Task<bool> DeleteFile(HFile file);
        public abstract Task<HFile> UpdateFile(HFile file);
        public abstract Task<MemoryStream> DownloadFileContent(HFile file);
    }
}