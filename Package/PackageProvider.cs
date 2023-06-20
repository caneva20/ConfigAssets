using System;
using System.Collections.Generic;
using System.IO;
using ConfigAssets.Package.Models;

namespace ConfigAssets.Package {
    public class PackageProvider : IPackageProvider {
        private static readonly Dictionary<PackageType, PackageDefinition> Definitions = new() {
            {
                PackageType.GeneratedAssets, new PackageDefinition("config-assets.generated",
                    "0.0.0",
                    "Config Assets Generated Files",
                    "CAREFUL! DO NOT REMOVE!\\n\\nThis package is used to hold generated files from ConfigAssets package\\n\\nRemoving this will also delete your saved configurations!") {
                    Author = "ConfigAssets"
                }
            }
        };

        private readonly IPackageCreator _packageCreator;

        public PackageProvider(IPackageCreator packageCreator) {
            _packageCreator = packageCreator;
        }

        public void CreatePackage(PackageType package) {
            var definition = GetDefinition(package);

            _packageCreator.Create(definition);

            var packageLocation = _packageCreator.GetLocation(definition);

            Directory.CreateDirectory($"{packageLocation}/Resources");
        }

        public string GetResourceLocation(PackageType package, string resourceName) {
            var definition = GetDefinition(package);

            if (!_packageCreator.Exists(definition)) {
                _packageCreator.Create(definition);
            }

            var packageLocation = _packageCreator.GetLocation(definition);

            return $"{packageLocation}/Resources/{resourceName}";
        }

        private PackageDefinition GetDefinition(PackageType package) {
            if (!Definitions.TryGetValue(package, out var definition)) {
                throw new InvalidOperationException($"Definition for package of {package} was not found");
            }

            return definition;
        }
    }
}