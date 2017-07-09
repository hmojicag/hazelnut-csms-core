namespace Hazelnut.Core.HCloudStorageServices {
    using Newtonsoft.Json;
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class HCloudStorageServiceData {
        [JsonProperty("HCloudStorageServiceId")]
        public string HCloudStorageServiceId { get; set; }
    }
}