using System.IO;
using ConfigAssets.Logging;
using UnityEngine;

namespace ConfigAssets.Editor {
    public static class PackageCreator {
        public static string Create(string name, string version, string displayName, string description) {
#if !UNITY_EDITOR
            return null;
#endif

            var absolutePath = Path.Combine(Application.dataPath.Replace("Assets", ""), "Packages", name);
            var relativePath = Path.Combine("Packages", name);

            Directory.CreateDirectory(absolutePath);

            var filePath = Path.Combine(absolutePath, "package.json");

            if (File.Exists(filePath)) {
                return relativePath;
            }

            var definition = $@"{{
    ""name"": ""{name}"",
    ""version"": ""{version}"",
    ""displayName"": ""{displayName}"",
    ""description"": ""{description}""
}}";

            File.WriteAllText(filePath, definition);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
            UnityEditor.AssetDatabase.Refresh();
#endif

            ConfigAssetLogger.LogInformation($"Package {name} created");

            return relativePath;
        }
    }
}