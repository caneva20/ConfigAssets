﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfigAssets.Sourcegen.Receivers;
using ConfigAssets.Sourcegen.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ConfigAssets.Sourcegen.Generators {
    [Generator]
    public class EnhancedConfigurationGenerator : ISourceGenerator {
        public void Initialize(GeneratorInitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver("Config"));
        }

        public void Execute(GeneratorExecutionContext context) {
            if (!(context.SyntaxReceiver is AttributeSyntaxReceiver receiver)) {
                return;
            }

            var targetAttribute = context.Compilation.GetType("UnityEngine.SerializeField");

            foreach (var (_, symbol, data) in ConfigurationFilter.FilterProviders(context.Compilation, receiver.Classes)) {
                var fields = GetFields(targetAttribute, symbol);
                var source = GenerateSource(data.Metadata.Namespace, data.Metadata.ClassName, data.GenerateSingleton, fields);

                var sourceText = SourceText.From(source, Encoding.UTF8);

                context.AddSource($"{data.Metadata.ClassName}.g.cs", sourceText);
            }
        }

        private static string GenerateSource(string @namespace, string className, bool generateSingleton, IEnumerable<MemberInfo> fields) {
            var sb = new StringBuilder($@"// <auto-generated />
using UnityEngine;
using ConfigAssets.Infrastructure;

namespace {@namespace} {{
    public partial class {className} : ScriptableObject {{
        private static {className} _instance;
        {(generateSingleton ? "public" : "private")} static {className} Instance {{
            get {{
                if (_instance == null) {{
                    _instance = ({className})ConfigLoader.Load(typeof({className}));
                }}
            
                return _instance;
            }}
        }}

");

            foreach (var field in fields) {
                sb.AppendLine($"        public static {field.Type} {field.PropertyName} => Instance.{field.Name};");
            }

            sb.Append(@$"
    }}
}}
");

            return sb.ToString();
        }

        private static IEnumerable<MemberInfo> GetFields(INamedTypeSymbol attributeType, ISymbol symbol) {
            if (!(symbol is INamedTypeSymbol ns)) {
                return Enumerable.Empty<MemberInfo>();
            }

            var members = ns.GetMembers().OfType<IFieldSymbol>().Where(x =>
                x.DeclaredAccessibility == Accessibility.Public || ConfigurationFilter.HasAttribute(attributeType, x));

            return members.Select(x => new MemberInfo() {
                Type = ConfigurationFilter.GetFullyQualifiedName(x.Type),
                Name = x.Name,
                PropertyName = ConvertToPropertyName(x.Name)
            });
        }
        
        private static string ConvertToPropertyName(string name) {
            name = name.Replace("m_", "").Replace("_", "");
            return name[..1].ToUpper() + name[1..];
        }

        private struct MemberInfo {
            public string Type { get; set; }
            public string Name { get; set; }
            public string PropertyName { get; set; }
        }
    }
}