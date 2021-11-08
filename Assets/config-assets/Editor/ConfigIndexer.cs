using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using me.caneva20.ConfigAssets.Editor.Builders;
using me.caneva20.ConfigAssets.Loading;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace me.caneva20.ConfigAssets.Editor {
    public static class ConfigIndexer {
        private static string GenMarkerFile => $"{Path.GetTempPath()}/config-assets.gen-marker";

        private static bool IsGenerating {
            get => File.Exists(GenMarkerFile);
            set {
                if (value) {
                    File.Create(GenMarkerFile);
                } else {
                    File.Delete(GenMarkerFile);
                }
            }
        }

        [DidReloadScripts]
        private static void OnScriptReload() {
            if (IsGenerating) {
                IsGenerating = false;
                return;
            }

            IsGenerating = true;

            var definitions = FindConfigurations().ToList();

            ConfigLoader.LoadDefaults();
            EnhanceConfigurations(definitions);

            foreach (var definition in definitions) {
                ConfigLoader.Load(definition.Type);
            }

            UpdatePreloadList(definitions);
            SettingsProviderBuilder.Build(definitions);

            RefreshAssetDatabase();
        }

        private static IEnumerable<ConfigurationDefinition> FindConfigurations() {
            var systemTypes = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(x => x.GetTypes())
               .Where(x => !x.IsInterface && !x.IsAbstract);

            return systemTypes.Select(GetDefinition).Where(x => x != null);
        }

        private static ConfigurationDefinition GetDefinition(Type type) {
            var configAttribute = type.GetCustomAttribute<ConfigAttribute>();

            if (configAttribute == null) {
                return null;
            }

            return new ConfigurationDefinition {
                Type = type,
                Attribute = configAttribute,
            };
        }

        private static void UpdatePreloadList(IEnumerable<ConfigurationDefinition> definitions) {
            foreach (var definition in definitions) {
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

        private static void EnhanceConfigurations(IEnumerable<ConfigurationDefinition> configurationTypes) {
            foreach (var configurationType in configurationTypes) {
                EnhancedConfigurationBuilder.Build(configurationType);
            }
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
