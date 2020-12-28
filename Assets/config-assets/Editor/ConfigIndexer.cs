using System;
using System.Collections.Generic;
using System.Linq;
using me.caneva20.ConfigAssets.Loading;
using UnityEditor;
using UnityEditor.Callbacks;

namespace me.caneva20.ConfigAssets.Editor {
    public static class ConfigIndexer {
        [DidReloadScripts]
        private static void OnScriptReload() {
            var configType = typeof(Config);
            
            var types = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(x => x.GetTypes())
               .Where(x =>
                    !x.IsInterface && !x.IsAbstract && x != configType && configType.IsAssignableFrom(x)); 
            
            foreach (var type in types) {
                ConfigLoader.Load(type);
            }

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