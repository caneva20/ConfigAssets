using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ConfigAssets.Sourcegen.Analyzers {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConfigAttributeAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "CSConfigAssetsMakePartial";
        private const string Title = "Classes marked with [Config] attribute must be partial";
        private const string Description = "Make partial";
        private const string Category = "Usage";

        private const string TargetAttributeName = "me.caneva20.ConfigAssets.ConfigAttribute";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Title, Category, DiagnosticSeverity.Error,
            isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) {

            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context) {
            var targetAttribute = context.Compilation.GetTypeByMetadataName(TargetAttributeName);
            var cds = (ClassDeclarationSyntax)context.Node;

            var semanticModel = context.SemanticModel;

            var classSymbol = semanticModel.GetDeclaredSymbol(cds);

            if (classSymbol is null || classSymbol.IsAbstract | classSymbol.IsStatic) {
                return;
            }

            var hasAttribute = classSymbol.GetAttributes()
                .Any(attr => attr.AttributeClass?.Equals(targetAttribute, SymbolEqualityComparer.Default) == true);

            if (!hasAttribute) {
                return;
            }

            var isPartial = cds.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword));

            if (isPartial) {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, cds.Identifier.GetLocation()));
        }
    }
}