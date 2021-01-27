using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using Brimborium.Latrans.SourceGen;
using System.Linq;

namespace Brimborium.Latrans.SourceGen {

    public class CompilationHelper {
        public static Compilation CreateCompilation(string source, MetadataReference[]? additionalReferences = null) {
            // Bypass System.Runtime error.
            Assembly systemRuntimeAssembly = Assembly.Load("System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Assembly systemCollectionsAssembly = Assembly.Load("System.Collections, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            string systemRuntimeAssemblyPath = systemRuntimeAssembly.Location;
            string systemCollecitonsAssemblyPath = systemCollectionsAssembly.Location;

            HashSet<string> assemblyPaths = new HashSet<string>();
            assemblyPaths.Add(systemRuntimeAssemblyPath);
            assemblyPaths.Add(systemCollecitonsAssemblyPath);
            foreach (var type in new Type[] {
                typeof(object),
                typeof(Attribute),
                typeof(Type),
                typeof(KeyValuePair),
                typeof(ContractNamespaceAttribute)
            }) {
                var assemblyPath = type.Assembly.Location;
                assemblyPaths.Add(assemblyPath);
            }
            List<MetadataReference> references = new List<MetadataReference>();
            references.AddRange(
                assemblyPaths.Select(assemblyPath => MetadataReference.CreateFromFile(assemblyPath)));

            // Add additional references as needed.
            if (additionalReferences != null) {
                references.AddRange(additionalReferences);
            }
            //List<MetadataReference> references = new List<MetadataReference> {
            //    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            //    MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
            //    //MetadataReference.CreateFromFile(typeof(JsonSerializableAttribute).Assembly.Location),
            //    //MetadataReference.CreateFromFile(typeof(JsonSerializerOptions).Assembly.Location),
            //    MetadataReference.CreateFromFile(typeof(Type).Assembly.Location),
            //    MetadataReference.CreateFromFile(typeof(KeyValuePair).Assembly.Location),
            //    MetadataReference.CreateFromFile(typeof(ContractNamespaceAttribute).Assembly.Location),
            //    MetadataReference.CreateFromFile(systemRuntimeAssemblyPath),
            //    MetadataReference.CreateFromFile(systemCollecitonsAssemblyPath),
            //};
            // Add additional references as needed.
            //if (additionalReferences != null) {
            //    foreach (MetadataReference reference in additionalReferences) {
            //        references.Add(reference);
            //    }
            //}

            return CSharpCompilation.Create(
                "TestAssembly",
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(source) },
                references: references.ToArray(),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );
        }

        private static GeneratorDriver CreateDriver(Compilation compilation, params ISourceGenerator[] generators)
            => CSharpGeneratorDriver.Create(
                generators: ImmutableArray.Create(generators),
                parseOptions: new CSharpParseOptions(kind: SourceCodeKind.Regular, documentationMode: DocumentationMode.Parse));

        public static Compilation RunGenerators(Compilation compilation, out ImmutableArray<Diagnostic> diagnostics, params ISourceGenerator[] generators) {
            CreateDriver(compilation, generators).RunGeneratorsAndUpdateCompilation(compilation, out Compilation outCompilation, out diagnostics);
            return outCompilation;
        }

        public static byte[] CreateAssemblyImage(Compilation compilation) {
            MemoryStream ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);
            if (!emitResult.Success) {
                throw new InvalidOperationException();
            }
            return ms.ToArray();
        }
    }
}
