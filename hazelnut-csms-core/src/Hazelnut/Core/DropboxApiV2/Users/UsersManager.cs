namespace Hazelnut.Core.DropboxApiV2.Users {
    using System;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Newtonsoft.Json;

    public class UsersManager {
        
        private RequestDropBoxApi dropboxHttpClient;
        public UsersManager(RequestDropBoxApi dropboxHttpClient) {
            this.dropboxHttpClient = dropboxHttpClient;
        }

        private string endpoint_get_current_account = "2/users/get_current_account";
        public async Task<Account> GetCurrentAccountAsync() {
            Account account = null;
            string responseContent = await dropboxHttpClient.PostDropBoxApiAsync(endpoint_get_current_account, null);
            account = JsonConvert.DeserializeObject<Account>(responseContent);  
            return account;
        }
    }
}