namespace Hazelnut.Core.HFiles {
    public class HFileGDrive : HFile {
        public HFileGDrive() : base() {

        }

        public string GDriveParentFolderId { get; set; }
        public string GDriveId { get; set; }
        public string FindGDriveParentFolderId() {
            //Create a method in HGDriveCloudStorageService to do this and call it
            return string.Empty;
        }

        public string FindGDriveId() {
            //Create a method in HGDriveCloudStorageService to do this and call it
            return string.Empty;
        }

    }
}