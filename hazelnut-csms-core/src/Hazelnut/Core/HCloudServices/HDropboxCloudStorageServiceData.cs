namespace Hazelnut.Core.HCloudStorageServices {
    using Newtonsoft.Json;

    public class HDropboxCloudStorageServiceData : HCloudStorageServiceData {
        [JsonProperty("Oauth2AccessToken")]
        public string Oauth2AccessToken { get; set; }
    }
}