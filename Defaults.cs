using me.caneva20.ConfigAssets.Loading;
using UnityEngine;

namespace me.caneva20.ConfigAssets {
    [Config(DisplayName = "ConfigAssets settings")]
    public class Defaults : ScriptableObject {
        internal static Defaults Instance => ConfigLoader.LoadDefaults();

        [SerializeField] private string _baseDirectory = "Configurations\\Resources";
        [SerializeField] private string _codeGenDirectory = "Configurations\\generated";
        [SerializeField] private bool _appendNamespaceToFile = true;
        [SerializeField] private int _nameSpaceLength = 2;

        public bool AppendNamespaceToFile => _appendNamespaceToFile;
        public string BaseDirectory => _baseDirectory;
        public string CodeGenDirectory => _codeGenDirectory;
        public int NameSpaceLength => _nameSpaceLength;
    }
}