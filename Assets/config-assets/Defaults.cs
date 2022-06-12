using System.IO;
using UnityEngine;

namespace me.caneva20.ConfigAssets {
    [Config(DisplayName = "ConfigAssets settings")]
    public class Defaults : Config<Defaults> {
        [SerializeField] private string _baseDirectory = "Configurations\\Resources";
        [SerializeField] private string _codeGenDirectory = Path.Join("Configurations", "Editor");
        [SerializeField] private bool _appendNamespaceToFile = true;
        [SerializeField] private int _nameSpaceLength = 2;

        public bool AppendNamespaceToFile => _appendNamespaceToFile;
        public string BaseDirectory => _baseDirectory.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        public string CodeGenDirectory => _codeGenDirectory.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        public int NameSpaceLength => _nameSpaceLength;
    }
}