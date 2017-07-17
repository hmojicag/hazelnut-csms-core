using Hazelnut.Core.DropboxApiV2;
using Hazelnut.Core.DropboxApiV2.Files;
using System.Threading.Tasks;
using Hazelnut.Core.DropboxApiV2.Users;
using System;
using System.IO;
using System.Text;

using Hazelnut.Core.HCloudStorageServices;
using Hazelnut.Core.HUsers;
using Hazelnut.Core.HFiles;
using System.Collections.Generic;
using System.Linq;

namespace Hazelnut.Core {
    public class HCloudSync {

        public enum SyncType {
            DUPLICATED,
            DISTRIBUTED
        }

        private HUser user;
        private List<HCloudStorageService> hcssList;

        public HCloudSync(HUser _user, List<HCloudStorageServiceData> hcssData) {
            this.user = _user;
            hcssList = new List<HCloudStorageService>();
            foreach (HCloudStorageServiceData data in hcssData) {
                if(data is HCloudStorageServiceDataDropbox) {
                    hcssList.Add(new HCloudStorageServiceDropbox(data));
                } else if (data is HCloudStorageServiceDataGDrive) {
                    hcssList.Add(new HCloudStorageServiceGDrive(data));
                }
            }
        }

        public async Task<HFileStructure> ApplyDuplicationAsync() {
            //CONTINUE WORKING HERE WHEN YOU WAKE UP
            //Try to make this Async: https://docs.microsoft.com/en-us/dotnet/csharp/async
            //using Task.WhenAll
            foreach (HCloudStorageService hcss in hcssList) {
                hcss.InitializeService();
                await hcss.FetchFileStructure();
            }

            return null;
        }

        //DUMMY CODE THAT SHOULD BE DELETED
        //FROM HERE TO ALL BELOW
        public HCloudSync() {

            //This is just a test for the DropBox API
            //string oauth2AccessToken = "YkSN6i4mCBAAAAAAAAAAB7ElQjewrG1XmIw9W1tEWDZfofOBjMqWXKUabW76_Yb_";
            //DropboxClient dbx = new DropboxClient(oauth2AccessToken);
            //getAccount(dbx).Wait();
            //ListFullDropBox(dbx).Wait();
            //DownloadAsync(dbx).Wait();
            //UploadAsync(dbx).Wait();
            //GetMetadata(dbx).Wait();
            //DeleteAsync(dbx).Wait();
        }
        
        private async Task dummyFolderOpGDrive(HCloudStorageService hcss) {
            HFileDropbox dbxFile = new HFileDropbox();
            dbxFile.Path = "/autoCreatedFolder/anotherOne/OhhShitCommeOn/";
            dbxFile.FileName = "dummyFile.txt";
            dbxFile.Content = new MemoryStream(new byte[] {72, 65, 90, 65});
            HFile newTestFile = await hcss.CreateFile(dbxFile);
            newTestFile.Content =  new MemoryStream(new byte[] {72, 65, 90, 65, 46});
            HFile updatedFile = await hcss.UpdateFile(newTestFile);
            MemoryStream contentStream = await hcss.DownloadFileContent(updatedFile);
            contentStream.Position = 0;
            Console.WriteLine("HCloudSync test. File downloaded from GDrive: " + byteArray2String(contentStream.ToArray()));
            await hcss.DeleteFile(updatedFile);
        }

        private async Task dummyFileOpDbx(HCloudStorageService hcss) {
            HFileGDrive gDriveTestFile = new HFileGDrive();
            gDriveTestFile.FileName = "gDriveTestFile.txt";
            gDriveTestFile.Path = "/";
            gDriveTestFile.Content = new MemoryStream(new byte[] {72, 65, 90, 65});
            HFile newTestFile = await hcss.CreateFile(gDriveTestFile);
            newTestFile.Content =  new MemoryStream(new byte[] {72, 65, 90, 65, 46});
            HFile updateTestFile = await hcss.UpdateFile(newTestFile);
            await updateTestFile.DownloadContentAsync();
            Console.WriteLine("File Downloaded: {0}", updateTestFile.Content.ToString());
            await hcss.DeleteFile(updateTestFile);
        }

        private string byteArray2String(byte[] array) {
            return array.Aggregate("", (current, b) => current + Convert.ToString(Convert.ToChar(b)));
        }
        
        private async Task getAccount(DropboxClient dbx) {
            Account account = await dbx.GetCurrentAccountAsync();
        }

        private async Task ListFullDropBox(DropboxClient dbx) {
            await dbx.ListFullDropBoxAsync();
        }

        private async Task DownloadAsync(DropboxClient dbx) {
            MemoryStream memoryStream = await dbx.DownloadAsync("id:-sUUqp8epSAAAAAAAAAABA");
            using (System.IO.FileStream fs = System.IO.File.Create(@"C:\testfolder\lol.pdf"))
            {
                memoryStream.WriteTo(fs);
            }
        }

        private async Task UploadAsync(DropboxClient dbx) {
            MemoryStream memoryStream = new MemoryStream();
            UnicodeEncoding uniEnconding = new UnicodeEncoding();
            byte[] testFileContent = uniEnconding.GetBytes(
                "This is an auto generated test file"
            );
            memoryStream.Write(testFileContent, 0, testFileContent.Length);
            string fileName = "/autoGeneratedTestFile-" + DateTime.Now.ToString("dd-MM-yyyy_hh_mm") + ".txt";
            await dbx.UploadAsync(memoryStream, fileName);
        }

        private async Task GetMetadataAsync(DropboxClient dbx) {
            FileMetadata fileMetadata = await dbx.GetMetadataAsync("/test-lv0-0.txt");
            Console.WriteLine("FileMetadata:");
            Console.WriteLine("Path Lower: " + fileMetadata.PathLower);
            Console.WriteLine("Id: " + fileMetadata.Id);
            Console.WriteLine("Rev: " + fileMetadata.Rev);
        }

        private async Task DeleteAsync(DropboxClient dbx) {
            Metadata metadata = await dbx.DeleteAsync("/autoGeneratedTestFile-06-07-2017_02_46.txt");
            Console.WriteLine("Item deleted: ");
            if(metadata.IsFile) {
                FileMetadata fileMetadata = (FileMetadata)metadata;
                Console.Write("File");
                Console.WriteLine("Path Lower: " + fileMetadata.PathLower);
                Console.WriteLine("Id: " + fileMetadata.Id);
                Console.WriteLine("Rev: " + fileMetadata.Rev);
            } else {
                Console.Write("Folder");
                FolderMetadata folderMetadata = (FolderMetadata)metadata;
                Console.WriteLine("Path Lower: " + folderMetadata.PathLower);
            }
        }

    }
}