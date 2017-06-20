using System;
using Hazelnut.Core.HCloudStorageServices;

namespace Hazelnut.Core.HFiles {
    public class HFile {

        //TODO: Folders in Google Drive are Case Sensitive, In DropBox f = F. The same as Linux vs Windows
        //TODO: Save the path with Case letters and not just the one in Lower Case
        private string path;
        private string fileName;
        private string fileExtension;
        private string mimeType;
        private long size;
        private DateTime lastEditDateTime;
        private HCloudStorageService sourceCloudStorageService;
        /**
        * Set a time offset of x minutes.
        * Between these offsset time two different files with the same name
        * on different drives will be treated the same
        */
        private long offsetMinutes = 5L;

        public string FullFileName {
            get { return path + fileName + "." + fileExtension; }
        }

        public long Size {
            get { return size; }
            set { size = value; }
        }

        public DateTime LastEditDateTime {
            get { return lastEditDateTime; }
            set { lastEditDateTime = value; }
        }

        public HCloudStorageService SourceCloudStorageService {
            get { return sourceCloudStorageService; }
            set { sourceCloudStorageService = value; }
        }

    }
}