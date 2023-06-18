using System.Collections.Generic;
using System.Linq;
using ConfigAssets.Sourcegen.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConfigAssets.Sourcegen.Utils {
    public static class ConfigurationFilter {
        private const string TargetAttributeName = "me.caneva20.ConfigAssets.ConfigAttribute";

        public static IEnumerable<(ClassDeclarationSyntax @class, ISymbol classSymbol, ConfigAttributeData data)> FilterProviders(Compilation compilation,
            IEnumerable<ClassDeclarationSyntax> classes) {
            var targetAttribute = compilation.GetTypeByMetadataName(TargetAttributeName);

            foreach (var @class in classes) {
                var semanticModel = compilation.GetSemanticModel(@class.SyntaxTree);

                var classSymbol = semanticModel.GetDeclaredSymbol(@class);

                if (classSymbol is null || classSymbol.IsAbstract | classSymbol.IsStatic) {
                    continue;
                }

                var attribute = classSymbol.GetAttributes()
                    .FirstOrDefault(x => x.AttributeClass?.Equals(targetAttribute, SymbolEqualityComparer.Default) == true);

                if (attribute == null) {
                    continue;
                }

                yield return (@class, classSymbol, ReadAttributeData(attribute));
            }
        }

        private static ConfigAttributeData ReadAttributeData(AttributeData attribute) {
            var data = new ConfigAttributeData();

            foreach (var pair in attribute.NamedArguments.Where(x => !x.Value.IsNull)) {
                switch (pair.Key) {
                    case "FileName":
                        data.FileName = (string)pair.Value.Value;
                        break;
                    case "GenerateSingleton":
                        data.GenerateSingleton = (bool)pair.Value.Value!;
                        break;
                    case "DisplayName":
                        data.DisplayName = (string)pair.Value.Value;
                        break;
                    case "Scope":
                        data.Scope = (int)pair.Value.Value!;
                        break;
                    case "EnableProvider":
                        data.EnableProvider = (bool)pair.Value.Value!;
                        break;
                    case "Keywords":
                        data.Keywords = pair.Value.Values.Select(x => (string)x.Value).ToArray();
                        break;
                }
            }

            return data;
        }
    }
}