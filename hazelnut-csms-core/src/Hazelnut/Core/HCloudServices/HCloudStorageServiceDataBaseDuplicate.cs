using Hazelnut.Core.HFiles;

namespace Hazelnut.Core.HCloudStorageServices {
    using Newtonsoft.Json;
    using System.Collections.Generic;
    
    public class HCloudStorageServiceDataBaseDuplicate : HCloudStorageServiceData {
        [JsonProperty("BaseFileStructure")] 
        public Dictionary<string, HFileBase> BaseFileStructure { get; set; }

        public Dictionary<string, HFile> GetBaseFSAsHFileStructure() {
            var hFileStructure = new Dictionary<string, HFile>();
            if (BaseFileStructure != null) {
                foreach (var filePath in BaseFileStructure.Keys) {
                    hFileStructure.Add(filePath, BaseFileStructure[filePath]);
                }
            }
            return hFileStructure;
        }

        public void SetBaseFSFromHFileStructure(HFileStructure fs) {
            BaseFileStructure = new Dictionary<string, HFileBase>();
            foreach (var path in fs.GetFilesFullPath()) {
                var file = fs.GetFile(path);
                var newHFileBase = new HFileBase() {
                    FileName = file.FileName,
                    Path = file.Path,
                    Size = file.Size,
                    LastEditDateTime = file.LastEditDateTime,
                    MimeType = file.MimeType
                };
                BaseFileStructure.Add(path, newHFileBase);
            }
            
        }
        
    }
}