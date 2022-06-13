using System.IO;
using me.caneva20.ConfigAssets.Loading;
using UnityEngine;

namespace me.caneva20.ConfigAssets {
    [Config(DisplayName = "ConfigAssets settings")]
    public class Defaults : ScriptableObject {
        internal static Defaults Instance => ConfigLoader.LoadDefaults();

        [SerializeField] private string _baseDirectory = Path.Join("Configurations", "Resources");
        [SerializeField] private string _codeGenDirectory = Path.Join("Configurations", "generated");
        [SerializeField] private bool _appendNamespaceToFile = true;
        [SerializeField] private int _nameSpaceLength = 2;

        public bool AppendNamespaceToFile => _appendNamespaceToFile;

        public string BaseDirectory => _baseDirectory.Replace('\\', Path.DirectorySeparatorChar)
           .Replace('/', Path.DirectorySeparatorChar);

        public string CodeGenDirectory => _codeGenDirectory.Replace('\\', Path.DirectorySeparatorChar)
           .Replace('/', Path.DirectorySeparatorChar);

        public int NameSpaceLength => _nameSpaceLength;
    }
}
