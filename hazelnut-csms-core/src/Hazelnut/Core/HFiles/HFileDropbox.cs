namespace Hazelnut.Core.HFiles {
    using Hazelnut.Core.DropboxApiV2.Files;
    using Hazelnut.Core.HCloudStorageServices;

    public class HFileDropbox : HFile {
        public HFileDropbox() : base() {

        }

        public HFileDropbox(FileMetadata dbxFileMetadata, HDropboxCloudStorageService dbxCSS) {
            this.FileName = dbxFileMetadata.Name;
            string fullPath = dbxFileMetadata.PathDisplay;
            this.Path = fullPath.Substring(0, fullPath.LastIndexOf('/')+1);
            this.Size = dbxFileMetadata.Size;
            this.LastEditDateTime = dbxFileMetadata.ServerModified;
            this.SourceCloudStorageService = dbxCSS;
        }

        public HFileDropbox(FileMetadata dbxFileMetadata) 
            : this(dbxFileMetadata, null){ }
            
    }
}