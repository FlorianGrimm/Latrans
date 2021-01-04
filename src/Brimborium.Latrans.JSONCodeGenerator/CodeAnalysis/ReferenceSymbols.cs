#nullable disable

using Microsoft.CodeAnalysis;

namespace Brimborium.Latrans.JSONCodeGenerator {
    public class ReferenceSymbols {
        public readonly INamedTypeSymbol Task;
        public readonly INamedTypeSymbol TaskOfT;
        public readonly string DataMemberAttribute;
        public readonly string IgnoreDataMemberAttribute;
        public readonly string SerializationConstructorAttribute;

        public ReferenceSymbols(Compilation compilation) {
            TaskOfT = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
            Task = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");

            DataMemberAttribute = "DataMember";
            IgnoreDataMemberAttribute = "IgnoreDataMember";
            SerializationConstructorAttribute = "SerializationConstructor";
            //DataMemberAttribute = compilation.GetTypeByMetadataName("System.Runtime.Serialization.DataMemberAttribute");
            //IgnoreDataMemberAttribute = compilation.GetTypeByMetadataName("System.Runtime.Serialization.IgnoreDataMemberAttribute");
            //SerializationConstructorAttribute = compilation.GetTypeByMetadataName("Utf8Json.SerializationConstructorAttribute");
        }
    }
}