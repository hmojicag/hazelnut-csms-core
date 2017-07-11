namespace Hazelnut.Core.DropboxApiV2 {
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System;
    using System.IO;

    public class RequestDropBoxApi {
        private HttpClient httpClientApiDbxApi;
        private HttpClient httpClientContentDbxApi;
        private readonly string apiDbxApiBaseUrl = "https://api.dropboxapi.com/";
        private readonly string contentDbxApiBaseUrl = "https://content.dropboxapi.com/";
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
                    Console.WriteLine("Call to DropBox API failed for: {0} \n {1}", response.RequestMessage, responseContent);
                    Console.WriteLine("Response msg: " + await responseContent.ReadAsStringAsync());
                }
            } catch (Exception ex) {
                Console.Write(ex);
                responseContent = null;
            }
            return responseContent;
        }

        private HttpClient getApiDxApiHttpClient() {
            if(httpClientApiDbxApi == null) {
                httpClientApiDbxApi = new HttpClient();
                httpClientApiDbxApi.BaseAddress = new Uri(apiDbxApiBaseUrl);
                httpClientApiDbxApi.DefaultRequestHeaders.Clear();
                httpClientApiDbxApi.DefaultRequestHeaders.Add("Authorization","Bearer " + oauth2AccessToken);
            }
            return httpClientApiDbxApi;
        }

        private HttpClient getContentDxApiHttpClient() {
            if(httpClientContentDbxApi == null) {
                httpClientContentDbxApi = new HttpClient();
                httpClientContentDbxApi.BaseAddress = new Uri(contentDbxApiBaseUrl);
                httpClientContentDbxApi.DefaultRequestHeaders.Clear();
                httpClientContentDbxApi.DefaultRequestHeaders.Add("Authorization","Bearer " + oauth2AccessToken);
            }
            return httpClientContentDbxApi;
        }

    }
}