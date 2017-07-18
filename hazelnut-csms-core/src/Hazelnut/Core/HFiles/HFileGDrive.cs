using System;
using Hazelnut.Core.DropboxApiV2.Users;
using Hazelnut.Core.HCloudStorageServices;

namespace Hazelnut.Core.HFiles {
    using GData = Google.Apis.Drive.v3.Data;
    public class HFileGDrive : HFile {
        public HFileGDrive() : base() {
            
        }

        public HFileGDrive(GData.File file, string fullPath, HCloudStorageServiceGDrive hcss) {
            GDriveId = file.Id;
            GDriveParentFolderId = file.Parents[0];
            MimeType = file.MimeType;
            Path = fullPath;
            FileName = file.Name;
            Size = Convert.ToUInt64(file.Size.Value);
            LastEditDateTime = file.ModifiedTime.Value;
            SourceCloudStorageService = hcss;
        }

        public string GDriveParentFolderId { get; set; }
        public string GDriveId { get; set; }
        
        public string FindGDriveParentFolderId() {
            //Find from HCSS filestructure
            return string.Empty;
        }

        public string FindGDriveId() {
            //Find from HCSS filestructure
            return string.Empty;
        }

    }
}