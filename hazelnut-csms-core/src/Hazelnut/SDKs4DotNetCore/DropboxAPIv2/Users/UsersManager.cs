namespace Dropbox.Api.Users {
    using System;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Newtonsoft.Json;

    public static class UsersManager {

        private static string endpoint_get_current_account = "2/users/get_current_account";
        public static async Task<Account> GetCurrentAccountAsync() {
            Account account = null;
            HttpClient client = HttpClientSingleton.getHttpClientInstance();
            try {
                HttpResponseMessage response = await client.PostAsync(endpoint_get_current_account, null);
                string responseContent = await response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode) {
                    account = JsonConvert.DeserializeObject<Account>(responseContent);
                } else {
                    //I really don't want to handle this right now
                    Console.WriteLine("Call to DropBox API failed for: " + response.RequestMessage);
                    Console.WriteLine("Response msg: " + responseContent);
                }
            } catch (Exception ex) {
                Console.Write(ex);
            }
            
            return account;
        }
    }
}