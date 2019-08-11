using UnityEngine;

namespace caneva20.ConfigAssets {
    public class Defaults : Config<Defaults> {
        [SerializeField] private string _baseDirectory = "Configurations\\Resources";
        [SerializeField] private bool _appendNamespaceToFile = true;
        [SerializeField] private int _nameSpaceLength = 2;

        public bool AppendNamespaceToFile => _appendNamespaceToFile;
        public string BaseDirectory => _baseDirectory;
        public int NameSpaceLength => _nameSpaceLength;
    }
}