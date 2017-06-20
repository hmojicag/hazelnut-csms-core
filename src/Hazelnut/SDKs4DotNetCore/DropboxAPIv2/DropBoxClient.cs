using Dropbox.Api.Users;
using System;

namespace Dropbox.Api {
    public class DropboxClient {

        private string oauth2AccessToken;
        private Account account;

        public DropboxClient(string oauth2AccessToken) {
            this.oauth2AccessToken = oauth2AccessToken;
        }
    
        public Account GetCurrentAccount() {
            Account account = null;

            Console.WriteLine(oauth2AccessToken);


            return account;
        }

    }
}