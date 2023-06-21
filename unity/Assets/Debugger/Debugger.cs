using System.IO;
using System.Linq;
using ConfigAssets.Editor;
using ConfigAssets.Metadata;
using ConfigAssets.Package.Models;
using ConfigAssets.Pipelines;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace ConfigAssets.Debugger {
    public static class Debugger {
        [MenuItem("ConfigAssets/Debug/Create asset")]
        public static void CreateAsset() {
            EditorServices.PipelineExecutor.Process(0, new [] {
                new PipelineStep(CreateGeneratedPackage),
                new PipelineStep(() => EditorServices.AssetCreator.CreateAsset(new AssetMetadata(typeof(DebuggerConfig))))
            });

            Debug.Log("[DEBUG] Asset created");
        }
        
        [MenuItem("ConfigAssets/Debug/Create generated package")]
        public static void CreateGeneratedPackage() {
            EditorServices.PackageProvider.CreatePackage(PackageType.GeneratedAssets);

            Debug.Log("[DEBUG] Package created");
        }
        
        [MenuItem("ConfigAssets/Debug/Delete generated package")]
        public static void DeleteGeneratedPackage() {
            var absolutePath = Path.Combine(Application.dataPath.Replace("Assets", ""), "Packages", "config-assets.generated");
            
            Directory.Delete(absolutePath, true);
            
            Client.Resolve();

            Debug.Log("[DEBUG] Package deleted");
        }

        [MenuItem("ConfigAssets/Debug/Load asset")]
        public static void LoadAsset() {
            var config = Services.ConfigLoader.Load<DebuggerConfig>(new AssetMetadata(typeof(DebuggerConfig)));

            Debug.Log(config);
        }

        [MenuItem("ConfigAssets/Debug/List types")]
        public static void ListTypes() {
            var types = TypeCache.GetTypesWithAttribute(typeof(ConfigAttribute));

            foreach (var s in types.Select(x => x.FullName)) {
                Debug.Log(s);
            }
        }
    }
}