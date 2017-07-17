using System;
using System.IO;
using System.Threading.Tasks;
using Hazelnut.Core.HCloudStorageServices;

namespace Hazelnut.Core.HFiles {
    public abstract class HFile {

        //TODO: Folders in Google Drive are Case Sensitive, In DropBox f = F. The same as Linux vs Windows
        //TODO: Save the path with Case letters and not just the one in Lower Case

        /**
        * Set a time offset of x minutes.
        * Between these offsset time two different files with the same name
        * on different drives will be treated the same
        */
        protected readonly long offsetMinutes = 5L;
        public string Path { get; set; }                    //Without file name, with trailing slash
        public string FileName { get; set; }                //Including File Extension
        public ulong Size { get; set; }
        public DateTime LastEditDateTime { get; set; }
        public HCloudStorageService SourceCloudStorageService { get; set; }
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