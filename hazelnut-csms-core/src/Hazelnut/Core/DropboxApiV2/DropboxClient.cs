using Hazelnut.Core.DropboxApiV2.Users;
using Hazelnut.Core.DropboxApiV2.Files;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Hazelnut.Core.DropboxApiV2 {
    public class DropboxClient:IDisposable {

        private RequestDropBoxApi dropboxHttpClient;
        private FilesManager filesManager;
        private UsersManager usersManager;

        public DropboxClient(string oauth2AccessToken) {
            dropboxHttpClient = new RequestDropBoxApi(oauth2AccessToken);
        }
    
        public async Task<Account> GetCurrentAccountAsync() {
            return await getUsersManager().GetCurrentAccountAsync();
        }

        public async Task<List<Metadata>> ListFullDropBoxAsync() {
            return await getFileManager().ListFullDropBoxAsync();
        }

        public async Task<MemoryStream> DownloadAsync(string path) {
            return await getFileManager().DownloadAsync(path);
        }

        public async Task<FileMetadata> UploadAsync(MemoryStream fileStream, string fullNamePath) {
            return await getFileManager().UploadAsync(fileStream, fullNamePath);
        }

        public async Task<FileMetadata> GetMetadataAsync(string identifier) {
            return await getFileManager().GetMetadataAsync(identifier);
        }

        public async Task<Metadata> DeleteAsync(string path) {
            return await getFileManager().DeleteAsync(path);
        }

        public void Dispose() {
            //Dispose something here
        }

        private FilesManager getFileManager() {
            if(filesManager == null) {
                filesManager = new FilesManager(dropboxHttpClient);
            }
            return filesManager;
        }

        private UsersManager getUsersManager() {
            if(usersManager == null) {
                usersManager = new UsersManager(dropboxHttpClient);
            }
            return usersManager;
        }

    }
}