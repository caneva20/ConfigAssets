using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace me.caneva20.ConfigAssets.Editor {
    internal static class ConfigurationFinder {
        public static ConfigurationDefinition[] FindConfigurations() {
            var systemTypes = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(x => x.GetTypes())
               .Where(x => !x.IsInterface && !x.IsAbstract);

            return systemTypes.Select(GetDefinition).Where(x => x != null).ToArray();
        }

        private static ConfigurationDefinition GetDefinition(Type type) {
            var configAttribute = type.GetCustomAttribute<ConfigAttribute>();

            if (configAttribute == null) {
                return null;
            }

            var cSharpFile = FindCSharpFile(type);

            return new ConfigurationDefinition {
                Type = type,
                Attribute = configAttribute,
                HasNamespace = HasNamespaceMatch(type, cSharpFile),
                IsPartial = IsPartialClass(type, cSharpFile)
            };
        }

        private static string FindCSharpFile(Type type) {
            var csFiles = Directory.GetFiles(@"Assets" + Path.DirectorySeparatorChar, "*.cs", SearchOption.AllDirectories)
               .Where(x => !x.EndsWith(".g.cs"))
               .Select(File.ReadAllText);

            return csFiles.FirstOrDefault(x => IsSourceFile(type, x));
        }

        private static bool IsSourceFile(Type type, string fileText) {
            return HasNamespaceMatch(type, fileText) && HasClassMatch(type, fileText);
        }

        private static bool HasMatch(string value, string regex) {
            return new Regex(regex).IsMatch(value);
        }

        private static bool HasNamespaceMatch(Type type, string fileText) {
            if (type.Namespace == null) {
                Debug.LogWarning($"Type {type.FullName} must contain a namespace");
                return false;
            }

            if (new Regex("namespace").Matches(fileText).Count != 1) {
                Debug.LogWarning("Target file contains none or multiple namespaces");
                return false;
            }

            return HasMatch(fileText, $"namespace( |\n)+{type.Namespace}");
        }

        private static bool HasClassMatch(MemberInfo type, string fileText) {
            return HasMatch(fileText, $"class( |\n)+{type.Name}");
        }

        private static bool IsPartialClass(MemberInfo type, string fileText) {
            return HasMatch(fileText, $"partial( |\n)+class( |\n)+{type.Name}");
        }
    }
}
