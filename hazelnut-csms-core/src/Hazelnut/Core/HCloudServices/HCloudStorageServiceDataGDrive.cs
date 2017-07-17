namespace Hazelnut.Core.HCloudStorageServices {
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Google.Apis.Util.Store;

    public sealed class HCloudStorageServiceDataGDrive : HCloudStorageServiceData {
        [JsonProperty("ClientSecretsJsonFileContent")]
        public JToken ClientSecretsJsonFileContent { get; set; }
        [JsonProperty("GDriveCredentials")]
        public JToken GDriveCredentials { get; set; }

        public IDataStore DataStore { get; set; }
        
        public string GetClientSecrets() {
            if (ClientSecretsJsonFileContent != null) {
                return JsonConvert.SerializeObject(ClientSecretsJsonFileContent);
            }
            return string.Empty;
        }
    }
}