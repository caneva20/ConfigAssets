using System.IO;
using ConfigAssets.Package;
using ConfigAssets.Package.Models;
using UnityEditor.PackageManager;

namespace ConfigAssets.Editor.Package {
    public class EditorPackageCreator : IPackageCreator {
        private static string CurrentDirectory => Directory.GetCurrentDirectory();
        
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
    ""description"": ""{definition.Description}"",
    ""author"": ""{definition.Author ?? ""}""
}}";

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            File.WriteAllText(filePath, definitionJson);

            Client.Resolve();
        }

        private static string GetPackagePath(string name) {
            return Path.Combine(CurrentDirectory, "Packages", name, "package.json");
        }
    }
}