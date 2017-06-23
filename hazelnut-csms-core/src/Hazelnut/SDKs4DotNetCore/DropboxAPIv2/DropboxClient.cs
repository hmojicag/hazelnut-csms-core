using Dropbox.Api.Users;
using Dropbox.Api.Files;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Dropbox.Api {
    public class DropboxClient:IDisposable {

        public DropboxClient(string oauth2AccessToken) {
            //Call HttpClient singleton in order to leave the singleton
            //already configured
            HttpClientSingleton.getHttpClientInstance(oauth2AccessToken);
        }
    
        public async Task<Account> GetCurrentAccountAsync() {
            return await UsersManager.GetCurrentAccountAsync();
        }

        public void Dispose() {
            //Dispose something here
        }

    }
}