using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace me.caneva20.ConfigAssets.Editor.Builders {
    internal static class SettingsProviderBuilder {
        internal static void Build(IEnumerable<ConfigurationDefinition> definitions) {
            var providers = GetProviders(definitions);

            var generator = new SettingsProviderGenerator {
                Session = new Dictionary<string, object> {
                    { "_providers", providers.ToArray() }
                }
            };

            generator.Initialize();
            var csharpCode = generator.TransformText();

            var dir = $@"Assets\{Defaults.Instance.CodeGenDirectory}\Editor";

            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText($@"{dir}\ConfigAssetsSettingsProvider.g.cs", csharpCode);
        }

        private static IEnumerable<ProviderDefinition> GetProviders(IEnumerable<ConfigurationDefinition> definitions) {
            foreach (var definition in definitions) {
                if (!definition.IsValid || definition.Attribute?.EnableProvider == false) {
                    continue;
                }

                var name = definition.Type?.FullName;
                var displayName = definition.Attribute?.DisplayName ?? definition.Type?.Name;

                var keywords = (definition.Attribute?.Keywords ?? Array.Empty<string>()).Select(x => $"\"{x}\"").ToList();

                yield return new ProviderDefinition {
                    Name = name?.Replace(".", ""),
                    NamespacedName = definition.Type?.FullName,
                    DisplayName = displayName,
                    Scope = (definition.Attribute?.Scope ?? SettingsScope.Project).ToString(),
                    Keywords = keywords.Count == 0 ? "null" : $"new string[] {{{string.Join(", ", keywords)}}}",
                };
            }
        }
    }
}
