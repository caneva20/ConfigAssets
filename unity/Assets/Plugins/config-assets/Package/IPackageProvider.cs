using ConfigAssets.Package.Models;

namespace ConfigAssets.Package {
    public interface IPackageProvider {
        void CreatePackage(PackageType package);
        string GetResourceLocation(PackageType package, string resourceName);
    }
}