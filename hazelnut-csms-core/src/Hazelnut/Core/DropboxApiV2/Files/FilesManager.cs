namespace Hazelnut.Core.DropboxApiV2.Files {
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;

    public class FilesManager {
        
        //Consider file size limit for single download/upload connections
        //up to 150MB

        //TODO: CHECK FOR NULL PARAMETERS ON EACH METHOD

        private RequestDropBoxApi dropboxHttpClient;
        private readonly string ENDPOINT_LIST_FOLDER = "2/files/list_folder";
        private readonly string ENDPOINT_FILES_DOWNLOAD = "2/files/download";
        private readonly string ENDPOINT_FILES_UPLOAD = "2/files/upload";
        private readonly string ENDPOINT_GET_METADATA = "2/files/get_metadata";
        private readonly string ENDPOINT_DELETE_V2 = "2/files/delete_v2";

        public FilesManager(RequestDropBoxApi dropboxHttpClient) {
            this.dropboxHttpClient = dropboxHttpClient;
        }

        public async Task<List<Metadata>> ListFullDropBoxAsync() {
            List<Metadata> fileList = new List<Metadata>();
            ListFolderReponse folderReponse = await RequestListFolderAsync("");
            fileList.AddRange(folderReponse.Entries);
            while(folderReponse.HasMore) {
                //folderReponse = await RequestListFolderContinueAsync();
                folderReponse.HasMore = false; //Take away this and uncomment above
            }

            return fileList;
        }

        public async Task<ListFolderReponse> RequestListFolderAsync(string path) {
            ListFolderReponse listFolderResponse = null;
            ListFilesRequestBody requestBody = new ListFilesRequestBody();
            requestBody.Path = path;
            requestBody.Recursive = true;
            requestBody.InclueMediaInfo = false;
            requestBody.IncludeDeleted = false;
            requestBody.IncludeHasExplicitSharedMembers = false;
            /*{
                "path": "",
                "recursive": true,
                "include_media_info": false,
                "include_deleted": false,
                "include_has_explicit_shared_members": false
            }*/
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(requestBody));
            stringContent.Headers.Clear();
            stringContent.Headers.Add("Content-Type", "application/json");
            string jsonResponse = await dropboxHttpClient.PostDropBoxApiAsync(ENDPOINT_LIST_FOLDER, stringContent);
            listFolderResponse = decodeListFolderResponse(jsonResponse);
            return listFolderResponse;
        }

        public async Task<FileMetadata> GetMetadataAsync(string identifier) {
            FilesRequestBodyIncludes requestBody = new FilesRequestBodyIncludes();
            requestBody.Path = identifier;
            requestBody.InclueMediaInfo = false;
            requestBody.IncludeDeleted = false;
            requestBody.IncludeHasExplicitSharedMembers = false;
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(requestBody));
            stringContent.Headers.Clear();
            stringContent.Headers.Add("Content-Type", "application/json");
            string jsonFileMetadata = await dropboxHttpClient.PostDropBoxApiAsync(ENDPOINT_GET_METADATA, stringContent);
            JObject jObjFileMeta = JObject.Parse(jsonFileMetadata);
            return decodeFile(jObjFileMeta.Root);
        }

        //Deletes a file or folder on the given path
        //This method doesn't accept an identifier (path, id or rev)
        //It only accepts a PATH
        //It returns Folder of File metadata NOT a deleted metadata
        public async Task<Metadata> DeleteAsync(string path) {
            FilesRequestBody requestBody = new FilesRequestBody();
            requestBody.Path = path;
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(requestBody));
            stringContent.Headers.Clear();
            stringContent.Headers.Add("Content-Type", "application/json");
            string jsonMetadata = await dropboxHttpClient.PostDropBoxApiAsync(ENDPOINT_DELETE_V2, stringContent);
            JObject jObjFileMeta = JObject.Parse(jsonMetadata);
            return decodeMetadata(jObjFileMeta["metadata"]);
        }

        /*public async Task<ListFolderReponse> RequestListFolderContinueAsync() {
            //TODO IMPLEMENT IT
            return null;
        }*/

        // identifier could be the path, id or rev as per
        // https://www.dropbox.com/developers/documentation/http/documentation#files-download
        public async Task<MemoryStream> DownloadAsync(string identifier) {
            StringContent stringContent = new StringContent("");
            DbxAPIArgHeaderDownload dropboxApiArg = new DbxAPIArgHeaderDownload();
            dropboxApiArg.Path = identifier;
            stringContent.Headers.Clear();
            stringContent.Headers.Add("Dropbox-API-Arg", JsonConvert.SerializeObject(dropboxApiArg));
            MemoryStream memoryStream = await dropboxHttpClient.PostContentApiAsync(ENDPOINT_FILES_DOWNLOAD, stringContent);
            return memoryStream;
        }
 
        public async Task<FileMetadata> UploadAsync(MemoryStream fileStream, string fullNamePath) {
            fileStream.Position = 0; //Put stream position to the begining
            StreamContent streamContent = new StreamContent(fileStream);
            DbxAPIArgHeaderUpload dropboxApiArg = new DbxAPIArgHeaderUpload();
            streamContent.Headers.Clear();
            dropboxApiArg.Path = fullNamePath;
            dropboxApiArg.Mode = "add";
            dropboxApiArg.Autorename = false;
            dropboxApiArg.Mute = false;
            string jsonContent = JsonConvert.SerializeObject(dropboxApiArg);
            streamContent.Headers.Add("Dropbox-API-Arg", jsonContent);
            streamContent.Headers.Add("Content-Type","application/octet-stream");
            string jsonFileMetadata = await dropboxHttpClient.PostContentApiAsStringAsync(ENDPOINT_FILES_UPLOAD, streamContent);
            JObject jObjFileMeta = JObject.Parse(jsonFileMetadata);
            return decodeFile(jObjFileMeta.Root);
        }

        private ListFolderReponse decodeListFolderResponse(string json) {
            ListFolderReponse listFolderResponse = new ListFolderReponse();
            JObject jsonResponse = JObject.Parse(json);
            listFolderResponse.HasMore = (bool)jsonResponse["has_more"];
            listFolderResponse.Cursor = (string)jsonResponse["cursor"];
            foreach(JToken entry in jsonResponse["entries"].Children()) {
                listFolderResponse.Entries.Add(decodeMetadata(entry));
            }
            return listFolderResponse;
        }

        private Metadata decodeMetadata(JToken jsonNode) {
            string tag = (string)jsonNode[".tag"];
            Metadata metadata = null;
            if(tag.Equals("folder")) {
                metadata = decodeFolder(jsonNode);
            } else if(tag.Equals("file")) {
                metadata = decodeFile(jsonNode);
            }
            return metadata;
        }

        private FolderMetadata decodeFolder(JToken jsonFolderNode) {
            FolderMetadata folderMetadata = new FolderMetadata(
                (string)jsonFolderNode["name"],
                (string)jsonFolderNode["id"],
                (string)jsonFolderNode["path_lower"],
                (string)jsonFolderNode["path_display"]
            );
            return folderMetadata;
        }

        private FileMetadata decodeFile(JToken jsonFileNode) {
            FileMetadata fileMetadata = new FileMetadata(
                (string)jsonFileNode["name"],
                (string)jsonFileNode["id"],
                (DateTime)jsonFileNode["client_modified"],
                (DateTime)jsonFileNode["server_modified"],
                (string)jsonFileNode["rev"],
                (ulong)jsonFileNode["size"],
                (string)jsonFileNode["path_lower"],
                (string)jsonFileNode["path_display"],
                (string)jsonFileNode["content_hash"]
            );
            return fileMetadata;
        }
        
        public class ListFolderReponse {
        
            public List<Metadata> Entries { get; set; }
            public string Cursor { get; set; }
            public bool HasMore { get; set; }

            public ListFolderReponse() {
                Entries = new List<Metadata>();
                Cursor = "";
                HasMore = false;
            }

        }

        [JsonObject(MemberSerialization.OptIn)]
        public class FilesRequestBody {
            [JsonProperty("path")]
            public string Path { get; set;}
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class FilesRequestBodyIncludes: FilesRequestBody {
            [JsonProperty("include_media_info")]
            public bool InclueMediaInfo { get; set;}
            [JsonProperty("include_deleted")]
            public bool IncludeDeleted { get; set;}
            [JsonProperty("include_has_explicit_shared_members")]
            public bool IncludeHasExplicitSharedMembers { get; set;}
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class ListFilesRequestBody : FilesRequestBodyIncludes {
            [JsonProperty("recursive")]
            public bool Recursive { get; set;}
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class DbxAPIArgHeaderDownload {
            //This is the value of the Dropbox-API-Arg
            [JsonProperty("path")]
            public string Path { get; set; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class DbxAPIArgHeaderUpload : DbxAPIArgHeaderDownload {
            [JsonProperty("mode")]
            public string Mode { get; set; }
            [JsonProperty("autorename")]
            public bool Autorename { get; set; }
            [JsonProperty("mute")]
            public bool Mute { get; set; }
        }

    }

}