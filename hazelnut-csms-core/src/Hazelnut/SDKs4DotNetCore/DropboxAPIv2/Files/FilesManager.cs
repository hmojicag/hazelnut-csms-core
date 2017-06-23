namespace Dropbox.Api.Files {

    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Newtonsoft.Json;

    public static class FilesManager {
        
        private static string endpoint_list_folder = "2/files/list_folder";

        public static List<Metadata> ListFullDropBoxAsync() {
            List<Metadata> fileList = null;

            ListFolderReponse folderReponse = ListFolderReponse.RequestListFolderAsync().Result;

            return fileList;
        }

        
        private class ListFolderReponse {
        
            public List<Metadata> Entries { get; set; }
            public string Cursor { get; set; }
            public bool HasMore { get; set; }

            public static async Task<ListFolderReponse> RequestListFolderAsync() {
                HttpClient client = HttpClientSingleton.getHttpClientInstance();
                ListFolderReponse listFolderResponse = null;
                List<Metadata> folderList;
                string requestBody = @"
                {
                    'path': '',
                    'recursive': true,
                    'include_media_info': false,
                    'include_deleted': false,
                    'include_has_explicit_shared_members': false
                }";
                StringContent stringContent = new StringContent(requestBody);
                stringContent.Headers.Clear();
                stringContent.Headers.Add("Content-Type", "application/json");

                string jsonResponse = await RequestDropBoxApi.PostAsync(endpoint_list_folder, stringContent);
                if(jsonResponse != null) {
                    listFolderResponse = new ListFolderReponse();
                    //Process json response
                }

                return listFolderResponse;
            }

            public static async Task<ListFolderReponse> RequestListFolderContinueAsync() {
                return null;
            }

        }   

    }

}