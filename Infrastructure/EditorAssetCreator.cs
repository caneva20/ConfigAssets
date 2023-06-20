#if UNITY_EDITOR
using System;
using ConfigAssets.Package;
using ConfigAssets.Package.Models;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ConfigAssets.Infrastructure {
    public class EditorAssetCreator : IAssetCreator {
        private readonly IPackageProvider _packageProvider;
        private readonly IPreloadedAssetService _preloadedAssetService;

        public EditorAssetCreator(IPackageProvider packageProvider, IPreloadedAssetService preloadedAssetService) {
            _packageProvider = packageProvider;
            _preloadedAssetService = preloadedAssetService;
        }

        public object CreateAsset<T>(bool preload = false) where T : ScriptableObject {
            var type = typeof(T);
            var assetName = MakeAssetName(type);
            var assetPath = _packageProvider.GetResourceLocation(PackageType.GeneratedAssets, assetName);

            var asset = ScriptableObject.CreateInstance(type);

            AssetDatabase.CreateAsset(asset, assetPath);

            RefreshAssets();
            
            if (preload) {
                _preloadedAssetService.Add(asset);
            }

            return asset;
        }

        private static string MakeAssetName(Type type) {
            return $"{type.FullName}.asset";
        }

        private static void RefreshAssets() {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.SaveOpenScenes();
        }
    }
}
#endif