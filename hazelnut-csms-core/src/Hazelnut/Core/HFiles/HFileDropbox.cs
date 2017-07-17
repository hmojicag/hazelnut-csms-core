namespace Hazelnut.Core.HFiles {
    using Hazelnut.Core.DropboxApiV2.Files;
    using Hazelnut.Core.HCloudStorageServices;

    public class HFileDropbox : HFile {
        public HFileDropbox() : base() {

        }

        public HFileDropbox(FileMetadata dbxFileMetadata, HCloudStorageServiceDropbox dbxCSS) {
            FileName = dbxFileMetadata.Name;
            var fullPath = dbxFileMetadata.PathDisplay;
            Path = fullPath.Substring(0, fullPath.LastIndexOf('/')+1);
            Size = dbxFileMetadata.Size;
            LastEditDateTime = dbxFileMetadata.ServerModified;
            SourceCloudStorageService = dbxCSS;
        }

        public HFileDropbox(FileMetadata dbxFileMetadata) 
            : this(dbxFileMetadata, null){ }
            
    }
}