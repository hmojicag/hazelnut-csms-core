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


        public DropboxClient(string oauth2AccessToken) {
            dropboxHttpClient = new RequestDropBoxApi(oauth2AccessToken);
        }
    
        public async Task<Account> GetCurrentAccountAsync() {
            UsersManager userManager = new UsersManager(dropboxHttpClient);
            return await userManager.GetCurrentAccountAsync();
        }

        public async Task<List<Metadata>> ListFullDropBoxAsync() {
            FilesManager filesManager = new FilesManager(dropboxHttpClient);
            return await filesManager.ListFullDropBoxAsync();
        }

        public async Task<MemoryStream> DownloadAsync(string path) {
            FilesManager filesManager = new FilesManager(dropboxHttpClient);
            return await filesManager.DownloadAsync(path);
        }

        public async Task<FileMetadata> UploadAsync(MemoryStream fileStream, string fullNamePath) {
            FilesManager filesManager = new FilesManager(dropboxHttpClient);
            return await filesManager.UploadAsync(fileStream, fullNamePath);
        }

        public void Dispose() {
            //Dispose something here
        }

    }
}