namespace Dropbox.Api {
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System;
    public class RequestDropBoxApi {
        public static async Task<string> PostAsync(string dropboxApiEndpoint, HttpContent httpContent) {
            HttpClient client = HttpClientSingleton.getHttpClientInstance();
            string responseContent = null;
            try {
                HttpResponseMessage response = await client.PostAsync(dropboxApiEndpoint, httpContent);
                responseContent = await response.Content.ReadAsStringAsync();
                if(!response.IsSuccessStatusCode) {
                    //I really don't want to handle this right now
                    Console.WriteLine("Call to DropBox API failed for: " + response.RequestMessage);
                    Console.WriteLine("Response msg: " + responseContent);
                    response = null;
                }
            } catch (Exception ex) {
                Console.Write(ex);
                responseContent = null;
            }
            return responseContent;
        }
    }
}