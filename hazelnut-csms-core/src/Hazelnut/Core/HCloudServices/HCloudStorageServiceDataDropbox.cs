namespace Hazelnut.Core.HCloudStorageServices {
    using Newtonsoft.Json;

    public class HCloudStorageServiceDataDropbox : HCloudStorageServiceData {
        [JsonProperty("Oauth2AccessToken")]
        public string Oauth2AccessToken { get; set; }
    }
}