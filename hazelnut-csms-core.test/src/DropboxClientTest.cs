using Xunit;
using Hazelnut.Core.DropboxApiV2;
using Hazelnut.Core.DropboxApiV2.Users;
using Hazelnut.Core.DropboxApiV2.Files;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Hazelnut.IntegrationTests {
    public class DropboxClientTest {
        private readonly DropboxClient dbx;
        private string ACCESS_TOKEN = "YkSN6i4mCBAAAAAAAAAAB7ElQjewrG1XmIw9W1tEWDZfofOBjMqWXKUabW76_Yb_";
        private readonly string DROPBOX_TEST_USER = "Hazael Mojica";
        private readonly string DROPBOX_TEST_EMAIL = "hazelnut.csms@gmail.com";
        private readonly int NUMBER_OF_TEST_ENTRIES = 6;

        public DropboxClientTest() {
            dbx = new DropboxClient(ACCESS_TOKEN);
        }

        [Fact]
        public void DropboxAuthenticationTest() {
            DropboxAuthenticationTestImpl().Wait();
        }

        private async Task DropboxAuthenticationTestImpl() {
            Account account = await dbx.GetCurrentAccountAsync();
            Assert.NotNull(account);
            Assert.True(account.Name.DisplayName.Equals(DROPBOX_TEST_USER), "Dropbox account username should be " + DROPBOX_TEST_USER);
            Assert.True(account.Email.Equals(DROPBOX_TEST_EMAIL), "Dropbox account test email should be " + DROPBOX_TEST_EMAIL);            
        }

        [Fact]
        public void ListFullDropBoxTest() {
            ListFullDropBoxTestImpl().Wait();
        }

        private async Task ListFullDropBoxTestImpl() {
            List<Metadata> entries = await dbx.ListFullDropBoxAsync();
            Assert.NotNull(entries);
            Assert.True(entries.Count == NUMBER_OF_TEST_ENTRIES, 
                "Dropbox number of entries should be " + NUMBER_OF_TEST_ENTRIES + " and were "
                + entries.Count);
        }


    }
}