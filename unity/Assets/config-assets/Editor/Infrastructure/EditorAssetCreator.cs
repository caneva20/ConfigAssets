using ConfigAssets.Infrastructure;
using ConfigAssets.Metadata;
using ConfigAssets.Package;
using ConfigAssets.Package.Models;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ConfigAssets.Editor.Infrastructure {
    public class EditorAssetCreator : IAssetCreator {
        private readonly IPackageProvider _packageProvider;
        private readonly IPreloadedAssetService _preloadedAssetService;

        public EditorAssetCreator(IPackageProvider packageProvider, IPreloadedAssetService preloadedAssetService) {
            _packageProvider = packageProvider;
            _preloadedAssetService = preloadedAssetService;
        }

        public object CreateAsset(AssetMetadata metadata, bool preload = false) {
            var assetPath = _packageProvider.GetResourceLocation(PackageType.GeneratedAssets, metadata.AssetName);

            var asset = ScriptableObject.CreateInstance(metadata.Type);

            AssetDatabase.CreateAsset(asset, assetPath);

            RefreshAssets();

            if (preload) {
                _preloadedAssetService.Add(asset);
            }

            return asset;
        }

        private static void RefreshAssets() {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.SaveOpenScenes();
        }
    }
}