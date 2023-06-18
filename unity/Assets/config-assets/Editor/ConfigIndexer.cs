using System.Linq;
using me.caneva20.ConfigAssets.Loading;
using me.caneva20.ConfigAssets.Logging;
using UnityEditor;
using UnityEditor.Callbacks;

namespace me.caneva20.ConfigAssets.Editor {
    public static class ConfigIndexer {
        private static ConfigurationDefinition[] Definitions => ConfigurationFinder.FindConfigurations();

        [DidReloadScripts]
        private static void OnScriptReload() {
            //UpdatePreloadList(); //TODO
        }

        private static void UpdatePreloadList() {
            var definitions = Definitions;

            foreach (var definition in definitions) {
                if (!definition.IsValid) {
                    continue;
                }

                ConfigLoader.Load(definition.Type);
            }

            foreach (var definition in definitions) {
                if (!definition.IsValid) {
                    continue;
                }

                var guids = AssetDatabase.FindAssets($"t:{definition.Type}").ToList();

                if (guids.Count == 0) {
                    ConfigAssetLogger.LogInformation($"No assets of type {definition.Type.FullName} was found");
                } else if (guids.Count > 1) {
                    ConfigAssetLogger.LogError(
                        $"Multiple assets of type {definition.Type.FullName} was found. Please delete all but 1. (Bellow is a list of all of them)");

                    foreach (var g in guids) {
                        var path = AssetDatabase.GUIDToAssetPath(g);
                        var invalidAsset = AssetDatabase.LoadAssetAtPath(path, definition.Type);

                        ConfigAssetLogger.LogError(
                            $"This asset may be invalid for {definition.Type.FullName}. <Click this message to highlight it>",
                            invalidAsset);
                    }
                }

                if (guids.Count != 1) {
                    continue;
                }

                var guid = guids.First();
                var asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), definition.Type);

                if (asset == null) {
                    ConfigAssetLogger.LogError($"No asset found with guid {guid}");
                    continue;
                }

                var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
                var isPreloaded = preloadedAssets.Contains(asset);

                if (isPreloaded) {
                    continue;
                }

                preloadedAssets.Add(asset);

                PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            }
        }
    }
}
