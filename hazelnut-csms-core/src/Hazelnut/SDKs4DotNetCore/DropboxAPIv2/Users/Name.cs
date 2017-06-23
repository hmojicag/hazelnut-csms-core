using Newtonsoft.Json;

namespace Dropbox.Api.Users {

    [JsonObject(MemberSerialization.OptIn)]
    public class Name {

        [JsonProperty("given_name")]
        public string GivenName {get; set;}
        
        [JsonProperty("surname")]
        public string SurName {get; set;}

        [JsonProperty("familiar_name")]
        public string FamiliarName {get; set;}

        [JsonProperty("display_name")]
        public string DisplayName {get; set;}

        [JsonProperty("abbreviated_name")]
        public string AbbreviatedName {get; set;}

    }
}