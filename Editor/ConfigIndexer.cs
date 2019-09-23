using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;

namespace caneva20.ConfigAssets.Editor {
    public static class ConfigIndexer {
        [DidReloadScripts]
        private static void OnScriptReload() {
            var guids = AssetDatabase.FindAssets($"t:{typeof(Config)}");

            var configs = guids.Select(guid => AssetDatabase.LoadAssetAtPath<Config>(AssetDatabase.GUIDToAssetPath(guid)));
            
            SetPreloadList(configs); 
        }

        private static void SetPreloadList(IEnumerable<Config> configs) {
            var preload = PlayerSettings.GetPreloadedAssets().Where(_ => _ && _ is Config).ToList();

            var except = configs.Except(preload);
            
            preload.AddRange(except);

            PlayerSettings.SetPreloadedAssets(preload.ToArray());
        }
    }
}