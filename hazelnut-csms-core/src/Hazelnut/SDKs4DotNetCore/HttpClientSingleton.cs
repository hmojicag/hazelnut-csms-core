using System.Net.Http;
using System;

namespace Dropbox.Api {
    public class HttpClientSingleton {
        private static HttpClient httpClient;
        private static string dropBoxApiBaseUrl = "https://api.dropboxapi.com/";
        private static string oauth2AccessToken = "";

        private HttpClientSingleton() {
            //We don't want anybody creating an instance of this class
        }

        public static HttpClient getHttpClientInstance(string _oauth2AccessToken) {
            oauth2AccessToken = _oauth2AccessToken;
            return getHttpClientInstance();
        }

        public static HttpClient getHttpClientInstance() {
            if(httpClient == null) {
                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(dropBoxApiBaseUrl);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization","Bearer " + oauth2AccessToken);
            }
            return httpClient;
        }

    }
}