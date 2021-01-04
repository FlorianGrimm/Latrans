using System;

namespace Brimborium.Latrans.JSONCodeGenerator {
    public class CodeGeneratorResolveFailedException : Exception {
        public CodeGeneratorResolveFailedException(string message)
            : base(message) {
        }
    }
}