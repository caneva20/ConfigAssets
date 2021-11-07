using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace me.caneva20.ConfigAssets.Editor {
    public static class SettingsProviderBuilder {
        public static void Build(IEnumerable<Type> configurationTypes) {
            var providers = GetProviders(configurationTypes);

            var generator = new SettingsProviderGenerator {
                Session = new Dictionary<string, object> {
                    { "_providers", providers.ToArray() }
                }
            };

            generator.Initialize();
            var csharpCode = generator.TransformText();

            var dir = $"Assets/{Defaults.Instance.CodeGenDirectory}";

            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText($@"{dir}/ConfigAssetsSettingsProvider.g.cs", csharpCode);
        }

        private static IEnumerable<ProviderDefinition> GetProviders(IEnumerable<Type> providerTypes) {
            foreach (var configType in providerTypes) {
                var attribute = ConfigAttribute.Find(configType);

                if (attribute?.EnableProvider == false) {
                    continue;
                }

                var name = configType?.FullName;
                var displayName = attribute?.DisplayName ?? configType?.Name;

                var keywords = (attribute?.Keywords ?? Array.Empty<string>()).Select(x => $"\"{x}\"").ToList();

                yield return new ProviderDefinition {
                    Name = name?.Replace(".", ""),
                    NamespacedName = configType?.FullName,
                    DisplayName = displayName,
                    Scope = (attribute?.Scope ?? SettingsScope.Project).ToString(),
                    Keywords = keywords.Count == 0 ? "null" : $"new string[] {{{string.Join(", ", keywords)}}}",
                };
            }
        }
    }
}
