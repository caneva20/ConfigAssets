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
        private static Defaults _defaults;

        internal static Defaults LoadDefaults() {
            if (_defaults == null) {
                _defaults = (Defaults)Load(typeof(Defaults), Path.Join("Configurations", "Resources"), "Defaults.asset");
            }

            return _defaults;
        }

        public static object Load(Type type) {
            LoadDefaults();

            return Load(type, MakeSaveDirectoryPath(), MakeAssetName(type));
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

        private static string MakeSaveDirectoryPath() {
            if (_defaults.BaseDirectory.EndsWith("/") || _defaults.BaseDirectory.EndsWith(@"\")) {
                return _defaults.BaseDirectory;
            }

            return _defaults.BaseDirectory + Path.DirectorySeparatorChar;
        }

        private static string MakeAssetName(Type type) {
            var attribute = ConfigAttribute.Find(type);

            var assetName = $"{attribute?.FileName ?? type.Name}.asset";
            var ns = type.Namespace;

            if (!_defaults.AppendNamespaceToFile || ns == null) {
                return assetName;
            }

            string append;

            if (_defaults.NameSpaceLength == -1) {
                append = ns;
            } else {
                var split = ns.Split('.');

                append = string.Join(".", split.Take(_defaults.NameSpaceLength));
            }

            return $"{append}.{assetName}";
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
