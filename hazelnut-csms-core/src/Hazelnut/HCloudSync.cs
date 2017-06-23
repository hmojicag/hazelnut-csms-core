using Dropbox.Api;
using System.Threading.Tasks;
using Dropbox.Api.Users;

namespace Hazelnut {
    public class HCloudSync {

        public HCloudSync() {

            //This is just a test for the DropBox API
            string oauth2AccessToken = "YkSN6i4mCBAAAAAAAAAAB7ElQjewrG1XmIw9W1tEWDZfofOBjMqWXKUabW76_Yb_";
            DropboxClient dbx = new DropboxClient(oauth2AccessToken);
            getAccount(dbx).Wait();
        }

        private async Task getAccount(DropboxClient dbx) {
            Account account = await dbx.GetCurrentAccountAsync();
        }

    }
}