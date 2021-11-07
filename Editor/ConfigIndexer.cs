using System;
using System.Collections.Generic;
using System.Linq;
using me.caneva20.ConfigAssets.Loading;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace me.caneva20.ConfigAssets.Editor {
    public static class ConfigIndexer {
        [DidReloadScripts]
        private static void OnScriptReload() {
            var types = FindConfigurations().ToList();

            foreach (var type in types) {
                ConfigLoader.Load(type);
            }

            UpdatePreloadList();
            SettingsProviderBuilder.Build(types);
            
            RefreshAssetDatabase();
        }

        private static IEnumerable<Type> FindConfigurations() {
            var configType = typeof(Config);

            return AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(x => x.GetTypes())
               .Where(x => !x.IsInterface && !x.IsAbstract && x != configType && configType.IsAssignableFrom(x));
        }

        private static void UpdatePreloadList() {
            var guids = AssetDatabase.FindAssets($"t:{typeof(Config)}");

            var configs = guids.Select(guid =>
                AssetDatabase.LoadAssetAtPath<Config>(AssetDatabase.GUIDToAssetPath(guid)));

            var preload = PlayerSettings.GetPreloadedAssets().Where(x => x && x is Config).ToList();

            var except = configs.Except(preload);

            preload.AddRange(except);

            PlayerSettings.SetPreloadedAssets(preload.ToArray());
        }

        private static void RefreshAssetDatabase() {
            try {
                AssetDatabase.Refresh();
            } catch (Exception e) {
                Debug.LogWarning("[ConfigAssets] Failed to refresh asset database");
            }
        }
    }
}
