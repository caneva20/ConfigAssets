using ConfigAssets.Editor.Infrastructure;
using ConfigAssets.Editor.Package;
using ConfigAssets.Editor.Pipelines;
using ConfigAssets.Infrastructure;
using ConfigAssets.Package;
using ConfigAssets.Pipelines;

namespace ConfigAssets.Editor {
    public static class EditorServices {
        public static readonly IPipelineExecutor PipelineExecutor = new EditorPipelineExecutor();
        public static readonly IPackageCreator PackageCreator = new EditorPackageCreator();
        public static readonly IPackageProvider PackageProvider = new EditorPackageProvider(PackageCreator);
        
        public static readonly IPreloadedAssetService PreloadedAssetService = new EditorPreloadedAssetService();
        public static readonly IAssetCreator AssetCreator = new EditorAssetCreator(PackageProvider, PreloadedAssetService);
    }
}