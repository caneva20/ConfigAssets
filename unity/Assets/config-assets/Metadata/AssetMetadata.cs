using System;

namespace ConfigAssets.Metadata {
    public class AssetMetadata {
        public Type Type { get; }
        public string AssetName { get; }
        public string AssetNameWithoutExtension { get; }

        public AssetMetadata(Type type) {
            Type = type;
            AssetNameWithoutExtension = Type.FullName;
            AssetName = $"{AssetNameWithoutExtension}.asset";
        }
    }
}