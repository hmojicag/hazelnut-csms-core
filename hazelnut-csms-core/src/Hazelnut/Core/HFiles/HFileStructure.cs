namespace Hazelnut.Core.HFiles {
using System.Collections.Generic;

    public class HFileStructure {
        private Dictionary<string, HFile> fileStructure;

        public HFileStructure() {
            fileStructure = new Dictionary<string, HFile>();
        }

        public void Add2FileStructure(HFile file) {
            fileStructure.Add(file.FullFileName, file);
        }

        public bool RemoveFromFileStructure(string fullPath) {
            return fileStructure.Remove(fullPath);
        }

        public bool Contains(string fullPath) {
            return fileStructure.ContainsKey(fullPath);
        }
        
        public HFile getFile(string path) {
            return fileStructure[path];
        }
    }
}