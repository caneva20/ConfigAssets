#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ConfigAssets.Infrastructure {
    public class EditorPreloadedAssetService : IPreloadedAssetService {
        public void Add(Object asset) {
            var assets = PlayerSettings.GetPreloadedAssets().Where(x => x != null).ToList();

            if (assets.Contains(asset)) {
                return;
            }

            assets.Add(asset);

            PlayerSettings.SetPreloadedAssets(assets.ToArray());
        }
    }
}
#endif