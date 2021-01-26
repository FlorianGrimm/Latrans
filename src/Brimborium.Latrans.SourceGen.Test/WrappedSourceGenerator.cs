using Microsoft.CodeAnalysis;

using System;
namespace Brimborium.Latrans.SourceGen {
    public class WrappedSourceGenerator : ISourceGenerator {
        public WrappedSourceGenerator() {
        }

        public void Execute(GeneratorExecutionContext context) {
            throw new NotImplementedException();
        }

        public void Initialize(GeneratorInitializationContext context) {
            throw new NotImplementedException();
        }
    }
}