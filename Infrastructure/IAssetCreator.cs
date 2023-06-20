using UnityEngine;

namespace ConfigAssets.Infrastructure {
    public interface IAssetCreator {
        object CreateAsset<T>() where T : ScriptableObject;
    }
}