using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;

namespace Brimborium.Latrans.SourceGen {
    public class PropertySourceGenerator : ISourceGenerator {
        public void Initialize(GeneratorInitializationContext context) {
        }

        public void Execute(GeneratorExecutionContext context) {
            if (context.Compilation is CSharpCompilation compilation) {
                // context.SyntaxReceiver
                var cu = this.Generate(context);
                if (cu is object) {
                    Microsoft.CodeAnalysis.Text.SourceText sourceText = cu.GetText(System.Text.Encoding.UTF8);
                    context.AddSource("Property.generated.cs", sourceText);
                }
            }
        }

        public CompilationUnitSyntax? Generate(GeneratorExecutionContext context) {

            return default;
        }
    }
    public class PropertySyntaxReceiver : ISyntaxReceiver {
        public readonly List<CompilationUnitSyntax> CompilationUnits { get; }
        public PropertySyntaxReceiver() {
            this.CompilationUnits = new List<CompilationUnitSyntax>();
        }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (syntaxNode is CompilationUnitSyntax compilationUnit) {
                CompilationUnits.Add(compilationUnit);
            }
        }
    }
}