using System.Linq;
using ConfigAssets.Infrastructure;
using UnityEditor;
using UnityEngine;

namespace ConfigAssets.Editor.Infrastructure {
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
