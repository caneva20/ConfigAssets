using UnityEngine;

namespace ConfigAssets.Infrastructure {
    public interface IAssetCreator {
        object CreateAsset<T>(bool preload = false) where T : ScriptableObject;
    }
}