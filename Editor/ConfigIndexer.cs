using System;
using System.IO;
using System.Linq;
using me.caneva20.ConfigAssets.Editor.Builders;
using me.caneva20.ConfigAssets.Loading;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace me.caneva20.ConfigAssets.Editor {
    public static class ConfigIndexer {
        private static string GenMarkerFile => $"{Path.GetTempPath()}{Path.DirectorySeparatorChar}config-assets.gen-marker";

        private static GenerationStep GenerationStep {
            get {
                if (!File.Exists(GenMarkerFile)) {
                    File.Create(GenMarkerFile);
                }
                
                var fileText = File.ReadAllText(GenMarkerFile);
                return Enum.TryParse<GenerationStep>(fileText, out var step)
                    ? step
                    : GenerationStep.Finished;
            }
            set => File.WriteAllText(GenMarkerFile, value.ToString());
        }

        private static ConfigurationDefinition[] Definitions => ConfigurationFinder.FindConfigurations();

        [DidReloadScripts]
        private static void OnScriptReload() {
            switch (GenerationStep) {
                case GenerationStep.Enhancement:
                    DoEnhancement();
                    break;
                case GenerationStep.PostEnhancement:
                    DoPostEnhancement();
                    break;
                case GenerationStep.Finished:
                    GenerationStep = GenerationStep.Enhancement;
                    break;
            }

            RefreshAssetDatabase();
        }

        private static void DoEnhancement() {
            EnhanceConfigurations(Definitions);
            SettingsProviderBuilder.Build(Definitions);

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
                    Debug.Log($"No assets of type {definition.Type.FullName} was found");
                } else if (guids.Count > 1) {
                    Debug.LogError($"Multiple assets of type {definition.Type.FullName} was found. Please delete all but 1. (Bellow is a list of all of them)");

                    foreach (var g in guids) {
                        var path = AssetDatabase.GUIDToAssetPath(g);
                        var invalidAsset = AssetDatabase.LoadAssetAtPath(path, definition.Type);

                        Debug.LogError($"This asset may be invalid for {definition.Type.FullName}. <Click this message to highlight it>", invalidAsset);
                    }
                }
                
                if (guids.Count != 1) {
                    continue;
                }

                var guid = guids.First();
                var asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), definition.Type);
                
                if (asset == null) {
                    Debug.LogError($"No asset found with guid {guid}");
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
            foreach (var configurationType in configurationTypes) {
                EnhancedConfigurationBuilder.Build(configurationType);
            }

            RefreshAssetDatabase();
        }

        private static void RefreshAssetDatabase() {
            try {
                AssetDatabase.Refresh();
            } catch (Exception) {
                Debug.LogWarning("[ConfigAssets] Failed to refresh asset database");
            }
        }
    }
}
