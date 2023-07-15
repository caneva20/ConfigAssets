using ConfigAssets.Metadata;
using UnityEngine;

namespace ConfigAssets.Infrastructure {
    public interface IConfigLoader {
        T Load<T>(AssetMetadata metadata) where T : ScriptableObject;
    }
}