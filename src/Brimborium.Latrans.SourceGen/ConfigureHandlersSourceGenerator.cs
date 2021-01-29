using Analyzer.Utilities;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Brimborium.Latrans.SourceGen {
    // ISyntaxContextReceiver
    public class ConfigureHandlersSourceGenerator : ISourceGenerator {
        private static CompilationUnitSyntax GetDefaultStartupMediator(string @namespace) {
            var namespaceSyntax = SyntaxFactory.ParseName(@namespace);
            return CompilationUnit()
                .WithUsings(
                    SingletonList<UsingDirectiveSyntax>(
                        UsingDirective(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("Microsoft"),
                                    IdentifierName("Extensions")),
                                IdentifierName("DependencyInjection")))))
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        NamespaceDeclaration(
                            //QualifiedName(IdentifierName("DemoWebApp"), IdentifierName("Logic"))
                            namespaceSyntax
                            )
                        .WithMembers(
                            SingletonList<MemberDeclarationSyntax>(
                                ClassDeclaration("StartupMediator")
                                .WithModifiers(
                                    TokenList(
                                        new[]{
                                            Token(SyntaxKind.PublicKeyword),
                                            Token(SyntaxKind.PartialKeyword)}))
                                .WithMembers(
                                    List<MemberDeclarationSyntax>(
                                        new MemberDeclarationSyntax[]{
                                            MethodDeclaration(
                                                PredefinedType(
                                                    Token(SyntaxKind.VoidKeyword)),
                                                Identifier("ConfigureServices"))
                                            .WithModifiers(
                                                TokenList(
                                                    Token(SyntaxKind.PublicKeyword)))
                                            .WithParameterList(
                                                ParameterList(
                                                    SingletonSeparatedList<ParameterSyntax>(
                                                        Parameter(
                                                            Identifier("services"))
                                                        .WithType(
                                                            IdentifierName("IServiceCollection")))))
                                            .WithBody(
                                                Block(
                                                    SingletonList<StatementSyntax>(
                                                        ExpressionStatement(
                                                            InvocationExpression(
                                                                IdentifierName("ConfigureHandlers"))
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                                        Argument(
                                                                            IdentifierName("services"))))))))),
                                            MethodDeclaration(
                                                PredefinedType(
                                                    Token(SyntaxKind.VoidKeyword)),
                                                Identifier("ConfigureHandlers"))
                                            .WithModifiers(
                                                TokenList(
                                                    Token(SyntaxKind.PartialKeyword)))
                                            .WithParameterList(
                                                ParameterList(
                                                    SingletonSeparatedList<ParameterSyntax>(
                                                        Parameter(
                                                            Identifier("services"))
                                                        .WithType(
                                                            IdentifierName("IServiceCollection")))))
                                            .WithSemicolonToken(
                                                Token(SyntaxKind.SemicolonToken))}))))))
                                    .NormalizeWhitespace();
        }

        public void Initialize(GeneratorInitializationContext context) {
            context.RegisterForSyntaxNotifications(() => new ConfigureHandlersSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context) {
            if (context.Compilation is CSharpCompilation compilation) {

                var lstCu = this.Generate(context);
                if (lstCu is object) {
                    for (int idx = 0; idx < lstCu.Count; idx++) {
                        var cu = lstCu[idx];
                        var sourceText = cu.GetText(System.Text.Encoding.UTF8);
                        context.AddSource($"ConfigureHandlers-{idx}.generated.cs", sourceText);
                    }
                }
            }
        }

        public List<CompilationUnitSyntax> Generate(GeneratorExecutionContext context) {
            var result = new List<CompilationUnitSyntax>();
            if (context.SyntaxReceiver is ConfigureHandlersSyntaxReceiver receiver) {
                MetadataLoadContext metadataLoadContext = new MetadataLoadContext(context.Compilation);
                var wellKnownTypeProvider = WellKnownTypeProvider.GetOrCreate(context.Compilation);
                var wkIServiceCollectionSymbol = wellKnownTypeProvider.GetOrCreateTypeByMetadataName("Microsoft.Extensions.DependencyInjection.IServiceCollection");

                var lstStartupMediators = new List<(SemanticModel semanticModel, INamedTypeSymbol cdStartupMediator)>();

                foreach (var cucdStartupMediator in receiver.StartupMediators) {
                    SemanticModel cuSemanticModel = context.Compilation.GetSemanticModel(cucdStartupMediator.CompilationUnit.SyntaxTree);
                    foreach (var classDeclaration in cucdStartupMediator.ClassDeclarations) {
                        INamedTypeSymbol? cdStartupMediator = cuSemanticModel.GetDeclaredSymbol(classDeclaration);
                        if (cdStartupMediator is object) {
                            foreach (var memberSymbol in cdStartupMediator.GetMembers()) {
                                if (memberSymbol is IMethodSymbol methodSymbol) {                                    
                                    if (string.Equals(methodSymbol.Name, "ConfigureServices", StringComparison.Ordinal)) {
                                        if (methodSymbol.Parameters.Length == 1) {
                                            var parameter = methodSymbol.Parameters[0];
                                            var parameterType = parameter.Type;
                                            if (SymbolEqualityComparer.Default.Equals(parameterType, wkIServiceCollectionSymbol)
                                                || parameterType.AllInterfaces.Any(
                                                    interfaceType => SymbolEqualityComparer.Default.Equals(interfaceType, wkIServiceCollectionSymbol))
                                                ) {
                                                lstStartupMediators.Add((cuSemanticModel, cdStartupMediator));
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            /*
                            var typeStartupMediator = classDeclaredSymbol.AsType(metadataLoadContext);
                            if (typeStartupMediator is object) {
                                var foundConfigureServices = typeStartupMediator.GetMethods()
                                    .Any(m => string.Equals(m.Name, "ConfigureServices", StringComparison.Ordinal));
                                if (foundConfigureServices) {
                                    lstStartupMediators.Add(classDeclaredSymbol);
                                }
                            }
                            */
                        }
                    }
                }

                if (lstStartupMediators.Count == 0) {
                    var an = context.Compilation.Assembly.Name;
                    var cuDefaultStartupMediator = GetDefaultStartupMediator(an);
                    result.Add(cuDefaultStartupMediator);
                } else {
                    foreach (var (cuSemanticModel, cdStartupMediator) in lstStartupMediators) {
                        foreach (var memberSymbol in cdStartupMediator.GetMembers()) {
                            if (memberSymbol is IMethodSymbol methodSymbol) {
                                if (string.Equals(methodSymbol.Name, "ConfigureServices", StringComparison.Ordinal)) {
                                    if (methodSymbol.Parameters.Length == 1) {
                                        var parameter = methodSymbol.Parameters[0];
                                        var parameterType = parameter.Type;
                                        if (SymbolEqualityComparer.Default.Equals(parameterType, wkIServiceCollectionSymbol)
                                            || parameterType.AllInterfaces.Any(
                                                interfaceType => SymbolEqualityComparer.Default.Equals(interfaceType, wkIServiceCollectionSymbol))
                                            ) {
                                            foreach (var syntaxReference in methodSymbol.DeclaringSyntaxReferences) {
                                                var syntax = syntaxReference.GetSyntax();
                                                "Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax"
                                                var r = cuSemanticModel.AnalyzeDataFlow(syntax);
                                            }
                                        }
                                        //var t = parameter.Type.AsType(metadataLoadContext);
                                    }
                                } else if (methodSymbol.MethodKind == MethodKind.DeclareMethod) {
                                    continue;
                                }
                                System.Console.Out.WriteLine($"methodSymbol:{methodSymbol.Name}");
                            }
                        }
                    }
                }

                foreach (var cucdHandler in receiver.Handlers) {
                    SemanticModel cuSemanticModel = context.Compilation.GetSemanticModel(cucdHandler.CompilationUnit.SyntaxTree);
                    // compilationSemanticModel.
                    // compilationUnit.sy
                    // cucdHandler.CompilationUnit
                    foreach (var cd in cucdHandler.ClassDeclarations) {
                        var classDeclaredSymbol = cuSemanticModel.GetDeclaredSymbol(cd);
                        if (classDeclaredSymbol is object) {
                            foreach (var interfaceSymbol in classDeclaredSymbol.AllInterfaces) {
                                var interfaceType = interfaceSymbol.AsType(metadataLoadContext);
                                if (interfaceType?.Name == "x") {
                                }

                            }

                            var typeHandler = classDeclaredSymbol.AsType(metadataLoadContext);
                            if (typeHandler is object) {
                                //foundConfigureServices = typeHandler.GetMethods()
                                //    .Any(m => string.Equals(m.Name, "ConfigureServices", StringComparison.Ordinal));
                                //if (foundConfigureServices) {
                                //    break;
                                //}
                            }
                        }
                    }
                }
            }
            return result;
        }
    }

    public class CuClassDeclaration {
        public CuClassDeclaration(CompilationUnitSyntax compilationUnit) {
            this.CompilationUnit = compilationUnit;
            this.ClassDeclarations = new List<ClassDeclarationSyntax>();
        }
        public readonly CompilationUnitSyntax CompilationUnit;
        public readonly List<ClassDeclarationSyntax> ClassDeclarations;
    }

    public class ConfigureHandlersSyntaxReceiver : ISyntaxReceiver {
        //public readonly List<CompilationUnitSyntax> CompilationUnits;
        public readonly List<CuClassDeclaration> Handlers;
        public readonly List<CuClassDeclaration> StartupMediators;

        private CuClassDeclaration? _CurrentHandler;

        public ConfigureHandlersSyntaxReceiver() {
            //  this.CompilationUnits = new List<CompilationUnitSyntax>();
            this.Handlers = new List<CuClassDeclaration>();
            this.StartupMediators = new List<CuClassDeclaration>();
        }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (syntaxNode is CompilationUnitSyntax compilationUnit) {
                //CompilationUnits.Add(compilationUnit);
                var cucd = new CuClassDeclaration(compilationUnit);
                this.Handlers.Add(this._CurrentHandler = cucd);
                return;
            }

            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax) {
                if (classDeclarationSyntax.Identifier.Text.EndsWith("Handler")
                    && classDeclarationSyntax.BaseList is BaseListSyntax baseList
                    && baseList.Types.Count > 0) {
                    if (this._CurrentHandler is object) {
                        this._CurrentHandler.ClassDeclarations.Add(classDeclarationSyntax);
                        return;
                    }
                }
                if (string.Equals(classDeclarationSyntax.Identifier.Text, "StartupMediator", StringComparison.Ordinal)) {
                    if (this._CurrentHandler is object) {
                        var cucdStartupMediator = new CuClassDeclaration(this._CurrentHandler.CompilationUnit);
                        cucdStartupMediator.ClassDeclarations.Add(classDeclarationSyntax);
                        this.StartupMediators.Add(cucdStartupMediator);

                        return;
                    }
                }
            }
        }
    }
}