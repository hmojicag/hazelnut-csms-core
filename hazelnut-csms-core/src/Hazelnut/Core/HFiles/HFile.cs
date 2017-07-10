using System;
using System.IO;
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
        public string Path { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public long Size { get; set; }
        public DateTime LastEditDateTime { get; set; }
        public HCloudStorageService SourceCloudStorageService { get; set; }
        public string MimeType { get; set; }
        public MemoryStream Content { get; set; }
        public string FullFileName {
            get { return Path + FileName + "." + FileExtension; }
        }
        public bool isDownloaded {
            get { return Content != null; }
        }

        public HFile DownloadContent() {
            if (SourceCloudStorageService != null) {
                this.Content = SourceCloudStorageService.DownloadFileContent(this);
            } else {
                return null;
            }
            return this;
        }

    }
}