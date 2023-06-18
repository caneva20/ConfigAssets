using System.Collections.Generic;
using System.Linq;
using ConfigAssets.Sourcegen.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConfigAssets.Sourcegen.Utils {
    public static class ConfigurationFilter {
        public const string ConfigAttributeName = "me.caneva20.ConfigAssets.ConfigAttribute";

        private static readonly SymbolDisplayFormat SymbolDisplayFormat =
            new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        public static IEnumerable<(ClassDeclarationSyntax @class, ISymbol classSymbol, ConfigAttributeData data)> FilterProviders(Compilation compilation,
            IEnumerable<ClassDeclarationSyntax> classes) {
            var targetAttribute = compilation.GetType(ConfigAttributeName);

            foreach (var @class in classes) {
                var semanticModel = compilation.GetSemanticModel(@class.SyntaxTree);

                var classSymbol = semanticModel.GetDeclaredSymbol(@class);

                if (classSymbol is null || classSymbol.IsAbstract | classSymbol.IsStatic) {
                    continue;
                }

                var attribute = classSymbol.GetAttributes().FirstOrDefault(x => IsValidAttribute(targetAttribute, x));

                if (attribute == null) {
                    continue;
                }

                var data = new ConfigAttributeData();
                ReadAttributeData(data, attribute);
                ReadMetadata(data, classSymbol);

                yield return (@class, classSymbol, data);
            }
        }

        public static bool IsValidAttribute(INamedTypeSymbol targetAttribute, AttributeData attributeData) {
            return attributeData.AttributeClass?.Equals(targetAttribute, SymbolEqualityComparer.Default) == true;
        }

        public static bool HasAttribute(INamedTypeSymbol targetAttribute, ISymbol symbol) {
            return symbol.GetAttributes().Any(x => IsValidAttribute(targetAttribute, x));
        }

        public static string GetFullyQualifiedName(ISymbol symbol) {
            return symbol.ToDisplayString(SymbolDisplayFormat);
        }

        private static void ReadAttributeData(ConfigAttributeData data, AttributeData attribute) {
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
        }

        private static void ReadMetadata(ConfigAttributeData data, ISymbol symbol) {
            data.Metadata.ClassName = symbol.Name;
            data.Metadata.FullyQualifiedName = GetFullyQualifiedName(symbol);
            data.Metadata.Namespace = data.Metadata.FullyQualifiedName[..^(symbol.Name.Length + 1)];
        }
    }
}