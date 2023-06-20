#if UNITY_EDITOR
using System;
using ConfigAssets.Package;
using ConfigAssets.Package.Models;
using UnityEditor;
using UnityEngine;

namespace ConfigAssets.Infrastructure {
    public class EditorAssetCreator : IAssetCreator {
        private readonly IPackageProvider _packageProvider;

        public EditorAssetCreator(IPackageProvider packageProvider) {
            _packageProvider = packageProvider;
        }

        public object CreateAsset<T>() where T : ScriptableObject {
            var type = typeof(T);
            var assetName = MakeAssetName(type);
            var assetPath = _packageProvider.GetResourceLocation(PackageType.GeneratedAssets, assetName);

            var asset = ScriptableObject.CreateInstance(type);

            AssetDatabase.CreateAsset(asset, assetPath);

            RefreshAssets();

            return asset;
        }

        private static string MakeAssetName(Type type) {
            return $"{type.FullName}.asset";
        }

        private static void RefreshAssets() {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif