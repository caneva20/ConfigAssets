using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace me.caneva20.ConfigAssets.Editor.Builders {
    internal static class EnhancedConfigurationBuilder {
        internal static void Build(ConfigurationDefinition definition) {
            if (!definition.IsValid) {
                return;
            }

            var enhancedClass = GenerateEnhancedClass(definition);

            var dir = $@"Assets\{Defaults.Instance.CodeGenDirectory}";
            
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            
            File.WriteAllText($@"{dir}\{definition.Type.Name}.g.cs", enhancedClass);
        }

        private static PropertyDefinition[] GetProperties(Type type) {
            var attributeFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
               .Where(x => x.GetCustomAttribute<SerializeField>() != null);

            var publicFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            return attributeFields.Concat(publicFields)
               .Select(x => new PropertyDefinition {
                    Name = x.Name,
                    TargetName = ConvertToPropertyName(x.Name),
                    Type = x.FieldType.FullName,
                }).ToArray();
        }

        private static string GenerateEnhancedClass(ConfigurationDefinition definition) {
            var @namespace = definition.Type.Namespace;
            var name = definition.Type.Name;
            var fields = GetProperties(definition.Type);

            var generator = new EnhancedConfigurationGenerator {
                Session = new Dictionary<string, object> {
                    { "_className", name },
                    { "_namespace", @namespace },
                    { "_fields", fields },
                    { "_createSingleton", definition.Attribute?.GenerateSingleton ?? false }
                }
            };

            generator.Initialize();
            return generator.TransformText();
        }

        private static string ConvertToPropertyName(string name) {
            name = name.Replace("m_", "").Replace("_", "");
            return name.Substring(0, 1).ToUpper() + name.Substring(1);
        }
    }
}
