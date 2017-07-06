namespace Hazelnut.Core.DropboxApiV2 {
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System;
    using System.IO;

    public class RequestDropBoxApi {
        private HttpClient httpClientApiDxApi;
        private HttpClient httpClientContentDxApi;
        private readonly string apiDxApiBaseUrl = "https://api.dropboxapi.com/";
        private readonly string contentDxApiBaseUrl = "https://content.dropboxapi.com/";
        private string oauth2AccessToken = "";
        
        public RequestDropBoxApi(string oauth2AccessToken) {
            this.oauth2AccessToken = oauth2AccessToken;
        }

        public async Task<string> PostDropBoxApiAsync(string dropboxApiEndpoint, HttpContent httpContent) {
            string responseContentString = null;
            HttpClient _httpClientApiDropbox = getApiDxApiHttpClient();
            HttpContent responseContent = await PostAsync(_httpClientApiDropbox, dropboxApiEndpoint, httpContent);
            responseContentString = await responseContent.ReadAsStringAsync();
            return responseContentString;
        }

        public async Task<MemoryStream> PostContentApiAsync(string contentDxApiEndpoint, HttpContent httpContent) {
            MemoryStream memoryStream = new MemoryStream();
            HttpClient _httpContentDxApi = getContentDxApiHttpClient();
            HttpContent responseContent = await PostAsync(_httpContentDxApi, contentDxApiEndpoint, httpContent);
            await responseContent.CopyToAsync(memoryStream);
            return memoryStream;
        }

        public async Task<string> PostContentApiAsStringAsync(string contentDxApiEndpoint, HttpContent httpContent) {
            MemoryStream memoryStream = new MemoryStream();
            HttpClient _httpContentDxApi = getContentDxApiHttpClient();
            HttpContent responseContent = await PostAsync(_httpContentDxApi, contentDxApiEndpoint, httpContent);
            return await responseContent.ReadAsStringAsync();
        }

        private async Task<HttpContent> PostAsync(HttpClient client, string endpoint, 
            HttpContent httpContent) {
            HttpContent responseContent;
            try {
                HttpResponseMessage response = await client.PostAsync(endpoint, httpContent);
                responseContent = response.Content;
                if(!response.IsSuccessStatusCode) {
                    //I really don't want to handle this right now
                    Console.WriteLine("Call to DropBox API failed for: " + response.RequestMessage);
                    Console.WriteLine("Call to DropBox API failed for: " + response.RequestMessage + "\n"
                        + response.RequestMessage);
                    Console.WriteLine("Response msg: " + responseContent.ReadAsStringAsync());
                }
            } catch (Exception ex) {
                Console.Write(ex);
                responseContent = null;
            }
            return responseContent;
        }

        private HttpClient getApiDxApiHttpClient() {
            if(httpClientApiDxApi == null) {
                httpClientApiDxApi = new HttpClient();
                httpClientApiDxApi.BaseAddress = new Uri(apiDxApiBaseUrl);
                httpClientApiDxApi.DefaultRequestHeaders.Clear();
                httpClientApiDxApi.DefaultRequestHeaders.Add("Authorization","Bearer " + oauth2AccessToken);
            }
            return httpClientApiDxApi;
        }

        private HttpClient getContentDxApiHttpClient() {
            if(httpClientContentDxApi == null) {
                httpClientContentDxApi = new HttpClient();
                httpClientContentDxApi.BaseAddress = new Uri(contentDxApiBaseUrl);
                httpClientContentDxApi.DefaultRequestHeaders.Clear();
                httpClientContentDxApi.DefaultRequestHeaders.Add("Authorization","Bearer " + oauth2AccessToken);
            }
            return httpClientContentDxApi;
        }

    }
}