using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConfigAssets.Sourcegen.Analyzers.Fixes {
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConfigAttributeAnalyzerFixProvider)), Shared]
    public class ConfigAttributeAnalyzerFixProvider : CodeFixProvider {
        private const string Title = "Make partial";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ConfigAttributeAnalyzer.DiagnosticId);

        public override Task RegisterCodeFixesAsync(CodeFixContext context) {
            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(title: Title, equivalenceKey: Title, createChangedDocument: token => MakePartial(context.Document, diagnostic, token)),
                diagnostic);
            
            return Task.CompletedTask;
        }

        private async Task<Document> MakePartial(Document document, Diagnostic diagnostic, CancellationToken token) {
            var root = await document.GetSyntaxRootAsync(token).ConfigureAwait(false);
            var cds = root.FindToken(diagnostic.Location.SourceSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();

            var partialToken = SyntaxFactory.Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(SyntaxFactory.Space);

            var newModifiers = cds.Modifiers.Add(partialToken);
            var newCds = cds.WithModifiers(newModifiers);

            var newRoot = root.ReplaceNode(cds, newCds);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}