using Microsoft.CodeAnalysis;

namespace ConfigAssets.Sourcegen.Utils {
    public static class CompilationExtensions {
        public static INamedTypeSymbol GetType(this Compilation compilation, string attributeName) {
            return compilation.GetTypeByMetadataName(attributeName);
        }
    }
}