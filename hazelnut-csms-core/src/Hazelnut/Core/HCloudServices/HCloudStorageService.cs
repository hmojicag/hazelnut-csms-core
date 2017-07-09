using Hazelnut.Core.HFiles;


namespace Hazelnut.Core.HCloudStorageServices {
    public abstract class HCloudStorageService {

        public enum CloudStorageType {
            Base,
            DropBox,
            GDrive,
            OneDrive
        };

        protected string cloudStorageServiceId;
        protected HFileStructure fileStructure;
        protected bool isFetched;


        public HCloudStorageService(string cloudStorageServiceId) {
            this.cloudStorageServiceId = cloudStorageServiceId;
            retrieveCloudStorageData();
        }

        private void retrieveCloudStorageData() {
            //Go to the DB and retrieve the data
            //Pass this data to the custom method for each service
            initializeService("");
        }



        public string CloudStorageServiceId { get; }
        public bool IsFetched { get; }
        public abstract void initializeService(string jsonData);
        public abstract bool fetchFileStructure();
        public abstract bool createFile(HFile file);
        public abstract bool deleteFile(HFile file);
        public abstract bool updateFile(HFile file);

    }
}