using ConfigAssets.Package.Models;

namespace ConfigAssets.Package {
    public interface IPackageCreator {
        void Create(PackageDefinition definition);
        string GetLocation(PackageDefinition definition);
        bool Exists(PackageDefinition definition);
    }
}