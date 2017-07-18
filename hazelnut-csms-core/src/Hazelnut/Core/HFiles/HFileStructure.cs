namespace Hazelnut.Core.HFiles {
using System.Collections.Generic;

    public class HFileStructure {
        private readonly Dictionary<string, HFile> _fileStructure;
        public int CloudStorageId { get; set; }

        public HFileStructure() {
            _fileStructure = new Dictionary<string, HFile>();
        }

        public HFileStructure(Dictionary<string, HFile> fileStructure) {
            _fileStructure = fileStructure;
        }

        public void Add2FileStructure(HFile file) {
            _fileStructure.Add(file.FullFileName, file);
        }

        public bool RemoveFromFileStructure(string fullPath) {
            return _fileStructure.Remove(fullPath);
        }

        public bool Contains(string fullPath) {
            return _fileStructure.ContainsKey(fullPath);
        }
        
        public HFile GetFile(string path) {
            return _fileStructure[path];
        }

        public void SetFile(string path, HFile file) {
            _fileStructure[path] = file;
        }

        public IEnumerable<string> GetFilesFullPath() {
            return _fileStructure.Keys;
        }
    }
}