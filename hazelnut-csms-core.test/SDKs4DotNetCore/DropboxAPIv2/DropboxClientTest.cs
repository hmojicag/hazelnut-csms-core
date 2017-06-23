using Xunit;
using Dropbox.Api;
using Dropbox.Api.Users;
using System.Threading.Tasks;

namespace Hazelnut.UnitTests.Dropbox.Api {
    public class DropboxClientTest {
        private readonly DropboxClient _dropboxClient;
        private string ACCESS_TOKEN = "YkSN6i4mCBAAAAAAAAAAB7ElQjewrG1XmIw9W1tEWDZfofOBjMqWXKUabW76_Yb_";
        private string dropboxTestUser = "Hazael Mojica";
        private string dropboxTestEmail = "hazelnut.csms@gmail.com";

        public DropboxClientTest() {
            _dropboxClient = new DropboxClient(ACCESS_TOKEN);
        }

        [Fact]
        public void DropboxAuthenticationTest() {
            DropboxAuthenticationTestImpl().Wait();
        }

        private async Task DropboxAuthenticationTestImpl() {
            Account account = await _dropboxClient.GetCurrentAccountAsync();
            Assert.NotNull(account);
            Assert.True(account.Name.DisplayName.Equals(dropboxTestUser), "Dropbox account username should be " + dropboxTestUser);
            Assert.True(account.Email.Equals(dropboxTestEmail), "Dropbox account test email should be " + dropboxTestEmail);            
        }

    }
}