using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using me.caneva20.ConfigAssets.Loading;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace me.caneva20.ConfigAssets.Editor {
    public static class ConfigIndexer {
        private static readonly Type _configType = typeof(Config);

        [DidReloadScripts]
        private static void OnScriptReload() {
            var types = FindConfigurations().ToList();

            foreach (var definition in types) {
                ConfigLoader.Load(definition.Type);
            }

            UpdatePreloadList();
            SettingsProviderBuilder.Build(types);
            
            RefreshAssetDatabase();
        }

        private static IEnumerable<ConfigurationDefinition> FindConfigurations() {
            var systemTypes = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(x => x.GetTypes())
               .Where(x => !x.IsInterface && !x.IsAbstract && x != _configType);

            return systemTypes.Select(GetDefinition).Where(x => x != null);
        }

        private static ConfigurationDefinition GetDefinition(Type type) {
            var usesInheritance = _configType.IsAssignableFrom(type);
            var configAttribute = type.GetCustomAttribute<ConfigAttribute>();

            if (!usesInheritance && configAttribute == null) {
                return null;
            }
            
            return new ConfigurationDefinition {
                Type = type,
                UsesInheritance = usesInheritance,
                Attribute = configAttribute,
            };
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
