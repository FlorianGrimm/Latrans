using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Brimborium.Latrans.SourceGen {
    public class PropertySourceGenerator : ISourceGenerator {
        public void Initialize(GeneratorInitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new PropertySyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context) {
            if (context.Compilation is CSharpCompilation compilation) {
                
                var cu = this.Generate(context);
                if (cu is object) {
                    Microsoft.CodeAnalysis.Text.SourceText sourceText = cu.GetText(System.Text.Encoding.UTF8);
                    context.AddSource("Property.generated.cs", sourceText);
                }
            }
        }

        public CompilationUnitSyntax? Generate(GeneratorExecutionContext executionContext) {
            if (executionContext.SyntaxReceiver is PropertySyntaxReceiver receiver) {
                MetadataLoadContext metadataLoadContext = new(executionContext.Compilation);
                foreach (CompilationUnitSyntax compilationUnit in receiver.CompilationUnits) {
                    SemanticModel compilationSemanticModel = executionContext.Compilation.GetSemanticModel(compilationUnit.SyntaxTree);
                    // compilationUnit.sy
                }
            }
            return default;
        }
    }
    public class PropertySyntaxReceiver : ISyntaxReceiver {
        public readonly List<CompilationUnitSyntax> CompilationUnits;
        public readonly List<ITypeSymbol> TypeSymbols;

        public PropertySyntaxReceiver() {
            this.CompilationUnits = new List<CompilationUnitSyntax>();
            this.TypeSymbols = new List<ITypeSymbol>();
        }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (syntaxNode is CompilationUnitSyntax compilationUnit) {
                CompilationUnits.Add(compilationUnit);
                return;
            }
            if (syntaxNode is ITypeSymbol typeSymbol) {
                if (typeSymbol.Name.EndsWith("Handler")) { 
                    this.TypeSymbols.Add(typeSymbol);
                }
                //var baseType = typeSymbol.BaseType;
                //typeSymbol.Interfaces.Select(i => i.ContainingAssembly);
            }
        }
    }
}