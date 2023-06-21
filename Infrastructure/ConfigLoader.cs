using ConfigAssets.Metadata;
using UnityEngine;

namespace ConfigAssets.Infrastructure {
    public class ConfigLoader : IConfigLoader {
        public T Load<T>(AssetMetadata metadata) where T : ScriptableObject {
            return (T)Resources.Load(metadata.AssetName);
        }
    }
}