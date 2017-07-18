namespace Hazelnut.Core.HCloudStorageServices {
    using Newtonsoft.Json;
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class HCloudStorageServiceData {
        [JsonProperty("HCloudStorageServiceId")]
        public int HCloudStorageServiceId { get; set; }
    }
}