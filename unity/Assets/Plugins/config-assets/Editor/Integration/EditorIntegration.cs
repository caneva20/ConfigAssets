using System;
using System.Collections.Generic;
using System.Linq;
using ConfigAssets.Metadata;
using ConfigAssets.Package.Models;
using ConfigAssets.Pipelines;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace ConfigAssets.Editor.Integration {
    public static class EditorIntegration {
        [DidReloadScripts(-1)]
        public static void OnScriptReload() {
            EditorServices.PipelineExecutor.Process(0,
                new[] {
                    new PipelineStep(() => EditorServices.PackageProvider.CreatePackage(PackageType.GeneratedAssets)),
                    new PipelineStep(CreateAssets)
                });
        }

        private static void CreateAssets() {
            foreach (var asset in GetAssets()) {
                EditorServices.AssetCreator.CreateAsset(asset, true);
            }
        }

        private static IEnumerable<AssetMetadata> GetAssets() {
            return TypeCache.GetTypesWithAttribute(typeof(ConfigAttribute)).Select(CreateMetadata).Where(NeedsCreation);
        }

        private static AssetMetadata CreateMetadata(Type field) {
            return new AssetMetadata(field);
        }

        private static bool NeedsCreation(AssetMetadata asset) {
            return Services.ConfigLoader.Load<ScriptableObject>(asset) == null;
        }
    }
}