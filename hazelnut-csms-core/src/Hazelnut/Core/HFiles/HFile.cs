using System;
using System.IO;
using System.Threading.Tasks;
using Hazelnut.Core.HCloudStorageServices;
using Newtonsoft.Json;

namespace Hazelnut.Core.HFiles {
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class HFile {

        //TODO: Folders in Google Drive are Case Sensitive, In DropBox f = F. The same as Linux vs Windows
        //TODO: Save the path with Case letters and not just the one in Lower Case
        public int CloudStorageServiceId {
            get {
                if (SourceCloudStorageService == null) {
                    return 0;
                } else {
                    return SourceCloudStorageService.CloudStorageServiceId;
                }
            }
        }
        [JsonProperty("Path")]
        public string Path { get; set; }                    //Without file name, with trailing slash
        [JsonProperty("FileName")]
        public string FileName { get; set; }                //Including File Extension
        [JsonProperty("Size")]
        public ulong Size { get; set; }
        [JsonProperty("LastEditDateTime")]
        public DateTime LastEditDateTime { get; set; }
        public HCloudStorageService SourceCloudStorageService { get; set; }
        [JsonProperty("MimeType")]
        public string MimeType { get; set; }
        public MemoryStream Content { get; set; }
        public string FullFileName {
            get { return Path + FileName; }
        }
        public string FileExtension {
            get {
                var dotIndex = FileName.LastIndexOf('.');
                var ext = (dotIndex < 0) ? string.Empty : FileName.Substring(dotIndex);
                return ext;
            }
        }
        public bool isDownloaded {
            get { return Content != null; }
        }

        public async Task<HFile> DownloadContentAsync() {
            if (SourceCloudStorageService != null) {
                this.Content = await SourceCloudStorageService.DownloadFileContent(this);
            } else {
                string msg = "Cloud Storage Service for file " + this.Path + "is Null";
                throw new NullReferenceException(msg);
            }
            return this;
        }

    }
}