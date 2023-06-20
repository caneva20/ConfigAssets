#if UNITY_EDITOR
using System.IO;
using ConfigAssets.Package.Models;
using UnityEditor.PackageManager;
using UnityEngine;

namespace ConfigAssets.Package {
    public class EditorPackageCreator : IPackageCreator {
        public string GetLocation(PackageDefinition definition) {
            return Path.Combine("Packages", definition.Name);
        }

        public bool Exists(PackageDefinition definition) {
            var filePath = GetPackagePath(definition.Name);
            
            return File.Exists(filePath);
        }
        
        public void Create(PackageDefinition definition) {
            if (Exists(definition)) {
                return;
            }
            
            var filePath = GetPackagePath(definition.Name);

            var definitionJson = $@"{{
    ""name"": ""{definition.Name}"",
    ""version"": ""{definition.Version}"",
    ""displayName"": ""{definition.DisplayName}"",
    ""description"": ""{definition.Description}""
}}";

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            File.WriteAllText(filePath, definitionJson);

            Client.Resolve();
        }

        private static string GetPackagePath(string name) {
            return Path.Combine(Application.dataPath.Replace("Assets", ""), "Packages", name, "package.json");
        }
    }
}
#endif