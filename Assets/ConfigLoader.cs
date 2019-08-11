using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace caneva20.ConfigAssets {
    public static class ConfigLoader {
        public static T Load<T>(Action<LoaderOptions<T>> optionsCreator = null) where T : ScriptableObject {
            var options = new LoaderOptions<T>();
            optionsCreator?.Invoke(options);
            
            var config = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
            
            return config ? config : CreateConfig(options);
        }

        private static T CreateConfig<T>(LoaderOptions<T> options) where T : ScriptableObject {
            var config = ScriptableObject.CreateInstance<T>();

        #if UNITY_EDITOR
            var absolutePath = Path.Combine(Application.dataPath, options.DirPath);

            if (!Directory.Exists(absolutePath)) {
                Directory.CreateDirectory(absolutePath);
            }

            var path = $"Assets/{options.DirPath}{options.AssetName}";
            
            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.Refresh();
        #endif

            return config;
        }
    }
}