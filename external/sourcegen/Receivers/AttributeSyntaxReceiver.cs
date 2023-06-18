using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConfigAssets.Sourcegen.Receivers {
    public class AttributeSyntaxReceiver : ISyntaxReceiver {
        private readonly string _attributeName;

        public List<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

        public AttributeSyntaxReceiver(string attributeName) {
            _attributeName = attributeName;
        }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (!(syntaxNode is ClassDeclarationSyntax cds)) {
                return;
            }

            var attributes = cds.AttributeLists.SelectMany(x => x.Attributes).ToList();
            
            if (!attributes.Any()) {
                return;
            }
            
            if (attributes.Any(IsAttribute)) {
                Classes.Add(cds);
            }
        }
        
        private bool IsAttribute(AttributeSyntax attributeSyntax) {
            return attributeSyntax.Name.ToString() == _attributeName;
        }
    }
}