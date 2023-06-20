using System;
using System.Linq;
using UnityEngine;

namespace ConfigAssets.Infrastructure {
    public static class ConfigLoader {
        public static object Load(Type type) {
#if UNITY_EDITOR
            foreach (var assetGuid in UnityEditor.AssetDatabase.FindAssets($"t:{type.Name}")) {
                UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(assetGuid), type);
            }
#endif

            return Resources.FindObjectsOfTypeAll(type).FirstOrDefault();
        }
    }
}