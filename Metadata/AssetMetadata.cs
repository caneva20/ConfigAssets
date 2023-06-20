using System;

namespace ConfigAssets.Metadata {
    public class AssetMetadata {
        public Type Type { get; }
        public string AssetName { get; }

        public AssetMetadata(Type type) {
            Type = type;
            AssetName = $"{Type.FullName}.asset";
        }
    }
}