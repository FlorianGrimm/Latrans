using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Brimborium.Latrans.JSONCodeGenerator {
    public interface IResolverRegisterInfo {
        string? FullName { get; }
        string? FormatterName { get; }
    }

    [DataContract(Name = "Brimborium.JSON.ObjectSerializationInfo")]
    public class ObjectSerializationInfo : IResolverRegisterInfo {
        [DataMember(Name="Name", Order = 0)]
        public string? Name { get; set; }

        [DataMember(Name = "Fullname", IsRequired = false, Order = 1)]
        public string? FullName { get; set; }

        [DataMember(Name = "Nspace",  Order = 2)]
        public string? Namespace { get; set; }
        
        public bool IsClass { get; set; }
        
        public bool IsStruct { get { return !this.IsClass; } }
        
        public MemberSerializationInfo[] ConstructorParameters { get; set; }
        
        public MemberSerializationInfo[] Members { get; set; }

        public MemberSerializationInfo[] GetMembers() {
            return this.Members.Where(x => !x.IsIgnored).OrderBy(x => x.Order).ToArray();
        }

#warning odd & buggy
        public string FormatterName =>
            $"{this.Name}Formatter";

        public bool HasConstructor { get; set; }

        public ObjectSerializationInfo() {
            this.ConstructorParameters = System.Array.Empty<MemberSerializationInfo>();
            this.Members = System.Array.Empty<MemberSerializationInfo>();
        }

        public int WriteCount => this.Members.Count(x => x.IsReadable);

        public string GetConstructorString() {
            var args = string.Join(", ", this.ConstructorParameters.Select(x => "__" + x.Name + "__"));
            return $"{this.FullName}({args})";
        }
    }

    [DataContract(Name = "Brimborium.JSON.MemberSerializationInfo")]
    public class MemberSerializationInfo {
        [IgnoreDataMember]
        public int Position { get; set; }
        
        public int Order { get; set; }
        public bool IsIgnored { get; set; }
        public bool IsProperty { get; set; }
        public bool IsField { get; set; }
        public bool IsWritable { get; set; }
        public bool IsReadable { get; set; }
        public bool IsConstructorParameter { get; set; }

        public string Type { get; set; }
        public string Name { get; set; }
        public string MemberName { get; set; }
        public string ShortTypeName { get; set; }

        public MemberSerializationInfo() {
            this.Type = string.Empty;
            this.Name = string.Empty;
            this.MemberName = string.Empty;
            this.ShortTypeName   = string.Empty;
        }

        static HashSet<string>? _primitiveTypes;
        static HashSet<string> getPrimitiveTypes()
            => _primitiveTypes ??= new HashSet<string>(new string[]
        {
            "short",
            "int",
            "long",
            "ushort",
            "uint",
            "ulong",
            "float",
            "double",
            "bool",
            "byte",
            "sbyte",
            //"char",
            //"global::System.DateTime",
            //"byte[]",
            "string",
        });

        public string GetSerializeMethodString() {
            if (this.Type is object && getPrimitiveTypes().Contains(this.Type)) {
                return $"writer.Write{this.ShortTypeName.Replace("[]", "s")}(value.{this.MemberName})";
            } else {
                return $"formatterResolver.GetFormatterWithVerify<{this.Type}>().Serialize(writer, value.{this.MemberName}, formatterResolver)";
            }
        }

        public string GetDeserializeMethodString() {
            if (getPrimitiveTypes().Contains(this.Type)) {
                return $"reader.Read{this.ShortTypeName.Replace("[]", "s")}()";
            } else {
                return $"formatterResolver.GetFormatterWithVerify<{this.Type}>().Deserialize(reader, formatterResolver)";
            }
        }
    }

    public class GenericSerializationInfo : IResolverRegisterInfo, IEquatable<GenericSerializationInfo> {
        public string FullName { get; set; }

        public string FormatterName { get; set; }

        public GenericSerializationInfo() {
            this.FullName = string.Empty;
            this.FormatterName = string.Empty;
        }

        public bool Equals(GenericSerializationInfo? other)
            => (other is null)
                ? false
                : this.FullName.Equals(other.FullName);

        public override int GetHashCode() => this.FullName.GetHashCode();
    }
}