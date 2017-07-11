namespace Hazelnut.Core.HCloudStorageServices {
    using Newtonsoft.Json;

    public sealed class HGDriveCloudStorageServiceData : HCloudStorageServiceData {
        [JsonProperty("ClientSecretsJsonFileContent")]
        public string ClientSecretsJsonFileContent { get; set; }
    }
}