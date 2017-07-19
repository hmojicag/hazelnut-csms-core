using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Google.Apis.Logging;
using Hazelnut.CLIApp;
using Hazelnut.Core.HCloudStorageServices;
using Hazelnut.Core.HFiles;
using Newtonsoft.Json;

namespace hazelnut_csms_core
{
    class Program
    {
        static void Main(string[] args)
        {
            int huserId = 1;
            if (args.Length > 0) {
                try {
                    huserId = Convert.ToInt32(args[0]);
                } catch(Exception ex) {
                    Console.WriteLine(@"Invalid argument. You must pass the user id to sync. Or 
                        leaving blank to sync user 1");
                    Console.WriteLine(ex);
                    return;
                }
            }
            Console.WriteLine("--Starting Hazelnut CSMS CLIAppClient for user {0}--", huserId);
            var cliAppClient = new CLIAppClient(huserId);
            try {
                ExecuteCLIOperations(cliAppClient);
                Console.WriteLine("--Hazelnut CSMS CLIAppClient Finished--");
            } catch (Exception ex) {
                Console.WriteLine(ex);
                Console.WriteLine("--ERROR, something went wrong and CSMS CLIAppClient had to exit--");
            }
        }

        private static void ExecuteCLIOperations(CLIAppClient cliAppClient) {
            cliAppClient.ExecuteOperationsAsync().Wait();
        }

        /*private static void serializeTest() {
            var FileStructure = new Dictionary<string, HFile>();
            HFile dbxFile = new HFileDropbox() {
                Path = "/",
                FileName = "lol.txt",
                Size = 12L,
                MimeType = "text/plain",
                LastEditDateTime = DateTime.Now,
                SourceCloudStorageService = new HCloudStorageServiceDropbox(new HCloudStorageServiceDataDropbox() {
                    HCloudStorageServiceId = "lol",
                    Oauth2AccessToken = "lmao"
                })
            };
            dbxFile.SourceCloudStorageService.InitializeService();
            FileStructure.Add("/lol.txt", dbxFile);
            
            string json = JsonConvert.SerializeObject(FileStructure);
            var dbxHCSS = JsonConvert.DeserializeObject<Dictionary<string, HFileBase>>(json, new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All
            });
            Console.WriteLine(json);
        }*/
        
    }
}
    