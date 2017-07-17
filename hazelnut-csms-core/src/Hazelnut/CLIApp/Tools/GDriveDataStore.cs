using System.Threading.Tasks;

namespace Hazelnut.CLIApp.Tools {
    using CLIApp.Model;
    using Google.Apis.Util.Store;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    
    public class GDriveDataStore : IDataStore {
        
        private readonly string jsonName = "GDriveCredentials";
        private HazelnutCLIContext _hazelnutCliContext;
        private int _hCloudStorageServiceId;

        // Implement an IDataStore to class to save and retreieve data from the sqlite
        // db inside the HCloudStorageDrive model
        public GDriveDataStore(int hCloudStorageServiceId) {
            _hCloudStorageServiceId = hCloudStorageServiceId;
        }
        
        public Task StoreAsync<T>(string key, T value) {
            using (_hazelnutCliContext = new HazelnutCLIContext()) {
                //Get 
                HCloudStorageDrive gDrive = _hazelnutCliContext.HCloudStorageDrives
                    .FirstOrDefault(drive => drive.HCloudStorageDriveId == _hCloudStorageServiceId);
                
            }
            
            
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync<T>(string key) {
            throw new System.NotImplementedException();
        }

        public Task<T> GetAsync<T>(string key) {
            throw new System.NotImplementedException();
        }

        public Task ClearAsync() {
            throw new System.NotImplementedException();
        }
    }
}