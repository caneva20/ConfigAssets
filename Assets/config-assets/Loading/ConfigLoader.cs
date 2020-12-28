using System;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace me.caneva20.ConfigAssets.Loading {
    public static class ConfigLoader {
        public static T Load<T>(Action<LoaderOptions<T>> optionsCreator = null) where T : ScriptableObject {
            var options = new LoaderOptions<T>();
            optionsCreator?.Invoke(options);
            
        #if UNITY_EDITOR
            foreach (var assetGuid in AssetDatabase.FindAssets($"t:{typeof(T).Name}")) {
                AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assetGuid));
            }
        #endif

            var config = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();

            return config ? config : CreateConfigAsset(options);
        }

        internal static T CreateConfigAsset<T>(LoaderOptions<T> options) where T : ScriptableObject {
            var config = ScriptableObject.CreateInstance<T>();

        #if UNITY_EDITOR
            var absolutePath = Path.Combine(Application.dataPath, options.DirPath);

            if (!Directory.Exists(absolutePath)) {
                Directory.CreateDirectory(absolutePath);
            }

            var path = $"Assets/{options.DirPath}{options.AssetName}";

            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        #endif

            return config;
        }
    }
}