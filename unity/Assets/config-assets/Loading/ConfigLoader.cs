using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

[assembly: InternalsVisibleTo("ConfigAssets.Editor")]

namespace me.caneva20.ConfigAssets.Loading {
    public static class ConfigLoader {
        private static readonly string BaseDirectory = Path.Join("configurations", "Resources", Path.DirectorySeparatorChar.ToString());

        public static object Load(Type type) {
            return Load(type, BaseDirectory, MakeAssetName(type));
        }

        private static object Load(Type type, string dirPath, string assetName) {
#if UNITY_EDITOR
            foreach (var assetGuid in AssetDatabase.FindAssets($"t:{type.Name}")) {
                AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGuid), type);
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

#if UNITY_EDITOR
            var absolutePath = Path.Combine(Application.dataPath, dirPath);

            if (!Directory.Exists(absolutePath)) {
                Directory.CreateDirectory(absolutePath);
            }

            var path = Path.Join("Assets", dirPath, assetName);

            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif

            return config;
        }
    }
}