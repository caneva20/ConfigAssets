using System;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace me.caneva20.ConfigAssets.Loading {
    public static class ConfigLoader {
        private static Defaults _defaults;

        private static void LoadDefaults() {
            if (_defaults != null) {
                return;
            }

            _defaults = Load<Defaults>(@"Configurations\Resources\", "Defaults.asset");
        }

        public static T Load<T>() where T : ScriptableObject {
            LoadDefaults();

            return Load<T>(MakeSaveDirectoryPath(), MakeAssetName(typeof(T)));
        }

        private static T Load<T>(string dirPath, string assetName) where T : ScriptableObject {
        #if UNITY_EDITOR
            foreach (var assetGuid in AssetDatabase.FindAssets($"t:{typeof(T).Name}")) {
                AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assetGuid));
            }
        #endif

            var config = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();

            return config ? config : (T)CreateConfigAsset(typeof(T), dirPath, assetName);
        }

        private static string MakeSaveDirectoryPath() {
            if (_defaults.BaseDirectory.EndsWith("/") || _defaults.BaseDirectory.EndsWith(@"\")) {
                return _defaults.BaseDirectory;
            }

            return $@"{_defaults.BaseDirectory}\";
        }
        
        private static string MakeAssetName(Type type) {
            var assetName = $"{type.Name}.asset";
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

            var path = $"Assets/{dirPath}{assetName}";

            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        #endif

            return config;
        }
    }
}