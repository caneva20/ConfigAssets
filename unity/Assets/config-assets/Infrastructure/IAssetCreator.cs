using ConfigAssets.Metadata;

namespace ConfigAssets.Infrastructure {
    public interface IAssetCreator {
        object CreateAsset(AssetMetadata metadata, bool preload = false);
    }
}