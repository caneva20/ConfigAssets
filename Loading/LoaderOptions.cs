using System.Linq;

namespace caneva20.ConfigAssets.Loading {
    public struct LoaderOptions<T> {
        private static Defaults _config = Defaults.Instance;

        private string _dirPath;
        private string _assetName;

        public LoaderOptions(string dirPath, string assetName) {
            _dirPath = string.IsNullOrWhiteSpace(dirPath) ? null : dirPath;
            _assetName = string.IsNullOrWhiteSpace(assetName) ? null : assetName;
        }

        public string DirPath {
            get => GetDirPath();
            set => _dirPath = value;
        }

        public string AssetName {
            get => GetAssetName();
            set => _assetName = value;
        }

        private string GetDirPath() {
            var path = _dirPath ?? _config.BaseDirectory;

            return $"{path}{(path.EndsWith("/") || path.EndsWith(@"\") ? "" : @"\")}";
        }

        private string GetAssetName() {
            var assetName = $"{_assetName ?? typeof(T).Name}.asset";
            var ns = typeof(T).Namespace;

            if (!_config.AppendNamespaceToFile || ns == null) {
                return assetName;
            }

            string append;

            if (_config.NameSpaceLength == -1) {
                append = ns;
            } else {
                var split = ns.Split('.');

                append = string.Join(".", split.Take(_config.NameSpaceLength));
            }

            return $"{append}.{assetName}";
        }
    }
}