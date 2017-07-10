namespace Hazelnut.Core.HFiles {
using System.Collections.Generic;

    public class HFileStructure {
        private Dictionary<string, HFile> fileStructure;

        public HFileStructure() {
            fileStructure = new Dictionary<string, HFile>();
        }

        public void Add2FileStructure(string fullPath, HFile file) {

        }

    }
}