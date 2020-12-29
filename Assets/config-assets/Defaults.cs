using UnityEngine;

namespace me.caneva20.ConfigAssets {
    public class Defaults : Config<Defaults> {
        [SerializeField] private string _baseDirectory = "Configurations\\Resources";
        [SerializeField] private string _codeGenDirectory = "Configurations";
        [SerializeField] private bool _appendNamespaceToFile = true;
        [SerializeField] private int _nameSpaceLength = 2;

        public bool AppendNamespaceToFile => _appendNamespaceToFile;
        public string BaseDirectory => _baseDirectory;
        public string CodeGenDirectory => _codeGenDirectory;
        public int NameSpaceLength => _nameSpaceLength;
    }
}