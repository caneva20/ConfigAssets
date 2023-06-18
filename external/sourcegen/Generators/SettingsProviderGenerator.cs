﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ConfigAssets.Sourcegen.Models;
using ConfigAssets.Sourcegen.Receivers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ConfigAssets.Sourcegen.Generators {
    [Generator]
    public class SettingsProviderGenerator : ISourceGenerator {
        private readonly SymbolDisplayFormat _symbolDisplayFormat =
            new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        private const string TargetAttributeName = "me.caneva20.ConfigAssets.ConfigAttribute";

        public void Initialize(GeneratorInitializationContext context) {
            Debugger.Launch();

            context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver("Config"));
        }

        public void Execute(GeneratorExecutionContext context) {
            if (!(context.SyntaxReceiver is AttributeSyntaxReceiver receiver)) {
                return;
            }

            try {
                var sb = new StringBuilder(@"// <auto-generated />

using me.caneva20.ConfigAssets;
using me.caneva20.ConfigAssets.Loading;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SettingsScope = UnityEditor.SettingsScope;

public static class ConfigAssetsSettingsProvider {
    private static SettingsProvider CreateProvider<T>(string name, SettingsScope scope, IEnumerable<string> keywords) where T : ScriptableObject {
        return new SettingsProvider($""Config assets/{name}"", scope, keywords) {
            guiHandler = _ => Editor.CreateEditor((T)ConfigLoader.Load(typeof(T))).OnInspectorGUI()
        };
    }
");

                var providers = GetDefinitions(context.Compilation, receiver.Classes);

                foreach (var provider in providers) {
                    sb.Append($@"
    [SettingsProvider]
    public static SettingsProvider Create{provider.Name}Provider() {{
        return CreateProvider<{provider.FullyQualifiedName}>(""{provider.DisplayName}"", (SettingsScope){provider.Scope}, {provider.Keywords});
    }}
");
                }

                sb.Append("}");

                var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
                context.AddSource("ConfigAssetsSettingsProvider.g.cs", sourceText);
            }
            catch (Exception e) {
                Console.WriteLine(e);

                context.AddSource("Fuck.g.cs", $"/*\n{e}\n*/");
            }
        }

        private IEnumerable<ProviderDefinition> GetDefinitions(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classes) {
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

                var displayName = classSymbol.Name;
                var scope = 1;
                var keywords = Array.Empty<string>();
                
                foreach (var pair in attribute.NamedArguments) {
                    if (pair.Value.IsNull) {
                        continue;
                    }

                    switch (pair.Key) {
                        case "DisplayName":
                            displayName = (string)pair.Value.Value;
                            break;
                        case "Scope":
                            scope = (int)pair.Value.Value;
                            break;
                        case "Keywords":
                            keywords = pair.Value.Values.Select(x => (string)x.Value).ToArray();
                            break;
                    }
                }
                
                var fullyQualifiedName = classSymbol.ToDisplayString(_symbolDisplayFormat);
                yield return new ProviderDefinition {
                    Name = fullyQualifiedName.Replace(".", ""),
                    FullyQualifiedName = fullyQualifiedName,
                    DisplayName = displayName,
                    Keywords = keywords.Length == 0 ? "null" : $"new string[] {{{string.Join(", ", keywords.Select(x => @$"""{x}"""))}}}",
                    Scope = scope
                };
            }
        }
    }
}