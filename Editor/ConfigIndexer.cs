using System;
using System.Collections.Generic;
using System.Linq;
using ConfigAssets.Infrastructure;
using ConfigAssets.Logging;

namespace ConfigAssets.Editor {
    public static class ConfigIndexer {
        public static void UpdatePreloadList(Type[] configurationTypes) {
#if UNITY_EDITOR
            var types = string.Join(", ", configurationTypes.Select(x => x.Name));
            ConfigAssetLogger.LogVerbose($"Update preload list with '{types}'");

            var assets = UnityEditor.PlayerSettings.GetPreloadedAssets().ToList();

            foreach (var type in configurationTypes) {
                ConfigLoader.Load(type);
                
                var guids = UnityEditor.AssetDatabase.FindAssets($"t:{type}").ToList();

                if (guids.Count > 1) {
                    PrintInvalidAssets(guids, type);

                    continue;
                }

                var guid = guids.Single();
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(guid), type);

                if (asset == null) {
                    ConfigAssetLogger.LogError($"No asset found with guid {guid}");
                    continue;
                }

                if (!assets.Contains(asset)) {
                    assets.Add(asset);
                }
            }

            UnityEditor.PlayerSettings.SetPreloadedAssets(assets.ToArray());
#endif
        }

        private static void PrintInvalidAssets(List<string> guids, Type type) {
#if UNITY_EDITOR
            ConfigAssetLogger.LogError($"Multiple assets of type {type} was found. Please delete all but 1. (Bellow is a list of all of them)");

            foreach (var g in guids) {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(g);
                var invalidAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(path, type);

                ConfigAssetLogger.LogError($"This asset may be invalid for {type}. <Click this message to highlight it>", invalidAsset);
            }
#endif
        }
    }
}