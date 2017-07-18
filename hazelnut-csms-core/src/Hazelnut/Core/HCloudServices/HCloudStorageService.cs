using Microsoft.EntityFrameworkCore;

namespace Hazelnut.Core.HCloudStorageServices {
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Hazelnut.Core.HFiles;
    using Newtonsoft.Json;
    
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class HCloudStorageService {

        protected HCloudStorageServiceData data;

        public HCloudStorageService(HCloudStorageServiceData data) {
            if(data == null || data.HCloudStorageServiceId < 1) {
                throw new ArgumentException(@"HCloudStorageServiceData and 
                    HCloudStorageServiceId should be a valid id");
            }

            this.data = data;
            IsFetched = false;
        }

        public HFileStructure FileStructure { get; set; }
        public int CloudStorageServiceId { 
            get { return data.HCloudStorageServiceId; }
        }
        public bool IsFetched { get; protected set; }
        public abstract void InitializeService();
        public abstract Task<bool> FetchFileStructure();
        public abstract Task<HFile> CreateFile(HFile file);
        public abstract Task<bool> DeleteFile(HFile file);
        public abstract Task<HFile> UpdateFile(HFile file);
        public abstract Task<MemoryStream> DownloadFileContent(HFile file);
    }
}