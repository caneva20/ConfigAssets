using System;
using System.IO;
using System.Linq;
using ConfigAssets.Editor;
using UnityEngine;

namespace ConfigAssets.Loading {
    public static class ConfigLoader {
        private static string PackageDirectory => CreateStoragePackage();

        public static object Load(Type type) {
            return Load(type, PackageDirectory + Path.DirectorySeparatorChar, MakeAssetName(type));
        }

        private static object Load(Type type, string dirPath, string assetName) {
#if UNITY_EDITOR
            foreach (var assetGuid in UnityEditor.AssetDatabase.FindAssets($"t:{type.Name}")) {
                UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(assetGuid), type);
            }
#endif

            var config = Resources.FindObjectsOfTypeAll(type).FirstOrDefault();

            return config ? config : CreateConfigAsset(type, dirPath, assetName);
        }

        private static string MakeAssetName(Type type) {
            var attribute = ConfigAttribute.Find(type);
 
            var assetName = $"{attribute?.FileName ?? type.Name}.asset";
            var ns = type.Namespace;

            return $"{ns}.{assetName}";
        }

        internal static object CreateConfigAsset(Type type, string dirPath, string assetName) {
            var config = ScriptableObject.CreateInstance(type);

            var path = Path.Join(dirPath, assetName);

#if UNITY_EDITOR
            try {
                UnityEditor.AssetDatabase.CreateAsset(config, path);
            } catch (UnityException e) {
                Debug.Log(e);
            }
#endif
            RefreshAssets();

            return config;
        }

        private static void RefreshAssets() {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        private static string CreateStoragePackage() {
            var path = PackageCreator.Create("config-assets.generated",
                "0.0.0",
                "Config Assets Generated Files",
                "Package used to hold generated files from ConfigAssets package");

            var resourcesPath = Path.Combine(path, "Resources");

            Directory.CreateDirectory(resourcesPath);

            RefreshAssets();

            return resourcesPath;
        }
    }
}