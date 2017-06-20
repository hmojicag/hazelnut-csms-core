
namespace Dropbox.Api.Users {
    public class Account {

        private string authorizeAccountEndPoint = "https://www.dropbox.com/oauth2/authorize";

        private string accountId;
        private bool disabled;
        private string email;
        private bool emailVerified;
        private string name;
        private string photoUrl;

        public Account() {

        }

        public Account GetCurrentAccount(string oauth2AccessToken) {
            Account account = null;

            

            return account;
        }

        public string AccountId {
            get { return accountId; }
            set { accountId = value; }
        }

        public bool Disabled {
            get { return disabled; }
            set {  disabled = value; }
        }
        
        public string Email {
            get { return email; }
            set { email = value; }
        }

        public bool EMailVerified {
            get { return emailVerified; }
            set {  emailVerified = value; }
        }

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public string PhotoUrl {
            get { return photoUrl; }
            set { photoUrl = value; }
        }

    }
}