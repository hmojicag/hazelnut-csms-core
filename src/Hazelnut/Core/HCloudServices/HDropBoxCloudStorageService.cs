using Newtonsoft.Json;
using Hazelnut.Core.HFiles;

namespace Hazelnut.Core.HCloudStorageServices {
    public sealed class HDropBoxCloudStorageService : HCloudStorageService {

        public HDropBoxCloudStorageService(string cloudStorageServiceId)
            : base(cloudStorageServiceId) { }

        public override void initializeService(string jsonData) {
            JsonTextReader reader;
        }

        public override bool fetchFileStructure() {
            return false;
        }

        public override bool createFile(HFile file) {
            return false;
        }
        
        public override bool deleteFile(HFile file) {
            return false;
        }

        public override bool updateFile(HFile file) {
            return false;
        }

    }
}