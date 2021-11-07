using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using me.caneva20.ConfigAssets.Loading;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace me.caneva20.ConfigAssets.Editor {
    public static class ConfigIndexer {
        [DidReloadScripts]
        private static void OnScriptReload() {
            var configType = typeof(Config);

            var types = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(x => x.GetTypes())
               .Where(x =>
                    !x.IsInterface && !x.IsAbstract && x != configType &&
                    configType.IsAssignableFrom(x)).ToList();

            foreach (var type in types) {
                ConfigLoader.Load(type);
            }

            var guids = AssetDatabase.FindAssets($"t:{typeof(Config)}");

            var configs = guids.Select(guid =>
                AssetDatabase.LoadAssetAtPath<Config>(AssetDatabase.GUIDToAssetPath(guid)));

            SetPreloadList(configs);

            var providers = GetProviders(types);
            GenerateCodeFile(providers);
        }

        private static void SetPreloadList(IEnumerable<Config> configs) {
            var preload = PlayerSettings.GetPreloadedAssets()
               .Where(_ => _ && _ is Config)
               .ToList();

            var except = configs.Except(preload);

            preload.AddRange(except);

            PlayerSettings.SetPreloadedAssets(preload.ToArray());
        }

        private static IEnumerable<ProviderDefinition> GetProviders(IEnumerable<Type> providerTypes) {
            foreach (var configType in providerTypes) {
                var attribute = ConfigAttribute.Find(configType);

                if (attribute?.EnableProvider == false) {
                    continue;
                } 
 
                var name = configType?.FullName;
                var displayName = attribute?.DisplayName ?? configType?.Name;

                yield return new ProviderDefinition {
                    Name = name?.Replace(".", ""),
                    NamespacedName = configType?.FullName,
                    DisplayName = displayName,
                    Scope = attribute?.Scope ?? SettingsScope.Project,
                    Keywords = attribute?.Keywords ?? Array.Empty<string>(),
                };
            }
        }

        private static void GenerateCodeFile(IEnumerable<ProviderDefinition> providers) {
            var csharpCode = new SettingsProviderGenerator(providers).TransformText();

            var dir = $"Assets/{Defaults.Instance.CodeGenDirectory}";
            
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            
            File.WriteAllText($@"{dir}/ConfigAssetsSettingsProvider.g.cs", csharpCode);

            try {
                AssetDatabase.Refresh();
            } catch (Exception e) {
                Debug.LogWarning("[ConfigAssets] Failed to refresh asset database");
            }
        }
    }
}
