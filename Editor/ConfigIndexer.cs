using System;
using System.Linq;
using me.caneva20.ConfigAssets.Loading;
using me.caneva20.ConfigAssets.Logging;
using UnityEditor;
using UnityEditor.Callbacks;

namespace me.caneva20.ConfigAssets.Editor {
    public static class ConfigIndexer {
        private const string GENERATION_STEP_KEY = "me.caneva20.config-assets.generation-step";

        private static GenerationStep GenerationStep {
            get => (GenerationStep)EditorPrefs.GetInt(GENERATION_STEP_KEY, (int)GenerationStep.Enhancement);
            set => EditorPrefs.SetInt(GENERATION_STEP_KEY, (int)value);
        }

        private static ConfigurationDefinition[] Definitions => ConfigurationFinder.FindConfigurations();

        [DidReloadScripts]
        private static void OnScriptReload() {
            return;
            switch (GenerationStep) {
                case GenerationStep.Enhancement:
                    ConfigAssetLogger.LogVerbose("Enhancing configuration files");
                    DoEnhancement();
                    break;
                case GenerationStep.PostEnhancement:
                    ConfigAssetLogger.LogVerbose("Finishing enhancements to configuration files");
                    DoPostEnhancement();
                    break;
                case GenerationStep.Finished:
                    ConfigAssetLogger.LogVerbose("Enhancements to configuration files finished");
                    GenerationStep = GenerationStep.Enhancement;
                    break;
            }

            RefreshAssetDatabase();
        }

        private static void DoEnhancement() {
            EnhanceConfigurations(Definitions);

            GenerationStep = GenerationStep.PostEnhancement;
        }

        private static void DoPostEnhancement() {
            UpdatePreloadList();

            GenerationStep = GenerationStep.Finished;
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

        private static void EnhanceConfigurations(ConfigurationDefinition[] configurationTypes) {
            RefreshAssetDatabase();
        }

        private static void RefreshAssetDatabase() {
            ConfigAssetLogger.LogVerbose("Refreshing the asset database");

            try {
                AssetDatabase.Refresh();
            } catch (Exception) {
                ConfigAssetLogger.LogWarning("Failed to refresh asset database");
            }
        }
    }
}
