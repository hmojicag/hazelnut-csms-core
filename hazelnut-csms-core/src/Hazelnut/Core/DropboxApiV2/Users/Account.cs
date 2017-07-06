using Newtonsoft.Json;
using System;

namespace Hazelnut.Core.DropboxApiV2.Users {

    [JsonObject(MemberSerialization.OptIn)]
    public class Account {

        [JsonProperty("account_id")]
        public string AccountId { get; set; }
        
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        public Account() {

        }
    }
}