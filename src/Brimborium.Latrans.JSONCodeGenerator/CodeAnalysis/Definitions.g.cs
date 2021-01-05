#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

namespace Brimborium.Latrans.JSON
{
    using System;

    public class GeneratedResolver : global::Brimborium.Latrans.JSON.IJsonFormatterResolver
    {
        public static readonly global::Brimborium.Latrans.JSON.IJsonFormatterResolver Instance = new GeneratedResolver();

        GeneratedResolver()
        {

        }

        public global::Brimborium.Latrans.JSON.IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly global::Brimborium.Latrans.JSON.IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                var f = GeneratedResolverGetFormatterHelper.GetFormatter(typeof(T));
                if (f != null)
                {
                    formatter = (global::Brimborium.Latrans.JSON.IJsonFormatter<T>)f;
                }
            }
        }
    }

    internal static class GeneratedResolverGetFormatterHelper
    {
        static readonly global::System.Collections.Generic.Dictionary<Type, int> lookup;

        static GeneratedResolverGetFormatterHelper()
        {
            lookup = new global::System.Collections.Generic.Dictionary<Type, int>(5)
            {
                {typeof(global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo[]), 0 },
                {typeof(global::Brimborium.Latrans.JSONCodeGenerator.IResolverRegisterInfo), 1 },
                {typeof(global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo), 2 },
                {typeof(global::Brimborium.Latrans.JSONCodeGenerator.ObjectSerializationInfo), 3 },
                {typeof(global::Brimborium.Latrans.JSONCodeGenerator.GenericSerializationInfo), 4 },
            };
        }

        internal static object GetFormatter(Type t)
        {
            int key;
            if (!lookup.TryGetValue(t, out key)) return null;

            switch (key)
            {
                case 0: return new global::Brimborium.Latrans.JSON.Formatters.ArrayFormatter<global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo>();
                case 1: return new IResolverRegisterInfoFormatter();
                case 2: return new MemberSerializationInfoFormatter();
                case 3: return new ObjectSerializationInfoFormatter();
                case 4: return new GenericSerializationInfoFormatter();
                default: return null;
            }
        }
    }
}

#pragma warning disable 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 219
#pragma warning disable 168

namespace Brimborium.Latrans.JSON
{
    using System;


    public sealed class IResolverRegisterInfoFormatter : global::Brimborium.Latrans.JSON.IJsonFormatter<global::Brimborium.Latrans.JSONCodeGenerator.IResolverRegisterInfo>
    {
        private readonly global::Brimborium.Latrans.JSON.JsonSerializationInfo ____JsonSerializationInfo;

        public IResolverRegisterInfoFormatter()
        {
            this.____JsonSerializationInfo = (new global::Brimborium.Latrans.JSON.JsonSerializationInfoBuilder())
                .Add("FullName", 0, true, false)
                .Add("FormatterName", 1, true, false)
                .Build();
        }

        public void Serialize(global::Brimborium.Latrans.JSON.JsonWriter writer, global::Brimborium.Latrans.JSONCodeGenerator.IResolverRegisterInfo value, global::Brimborium.Latrans.JSON.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteBeginObject();
            writer.WriteStartProperty(this.____JsonSerializationInfo,0);
            writer.WriteString(value.FullName);
            writer.WriteStartProperty(this.____JsonSerializationInfo,1);
            writer.WriteString(value.FormatterName);
            
            writer.WriteEndObject();
        }

        public global::Brimborium.Latrans.JSONCodeGenerator.IResolverRegisterInfo Deserialize(global::Brimborium.Latrans.JSON.JsonReader reader, global::Brimborium.Latrans.JSON.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            
            
            throw new InvalidOperationException("generated serializer for IInterface does not support deserialize.");
        }
    }

    public sealed class MemberSerializationInfoFormatter : global::Brimborium.Latrans.JSON.IJsonFormatter<global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo>
    {
        private readonly global::Brimborium.Latrans.JSON.JsonSerializationInfo ____JsonSerializationInfo;

        public MemberSerializationInfoFormatter()
        {
            this.____JsonSerializationInfo = (new global::Brimborium.Latrans.JSON.JsonSerializationInfoBuilder())
                .Add("Order", 0, true, true)
                .Add("IsIgnored", 1, true, true)
                .Add("IsProperty", 2, true, true)
                .Add("IsField", 3, true, true)
                .Add("IsWritable", 4, true, true)
                .Add("IsReadable", 5, true, true)
                .Add("IsConstructorParameter", 6, true, true)
                .Add("Type", 7, true, true)
                .Add("Name", 8, true, true)
                .Add("MemberName", 9, true, true)
                .Add("ShortTypeName", 10, true, true)
                .Build();
        }

        public void Serialize(global::Brimborium.Latrans.JSON.JsonWriter writer, global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo value, global::Brimborium.Latrans.JSON.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteBeginObject();
            writer.WriteStartProperty(this.____JsonSerializationInfo,0);
            writer.WriteInt32(value.Order);
            writer.WriteStartProperty(this.____JsonSerializationInfo,1);
            writer.WriteBoolean(value.IsIgnored);
            writer.WriteStartProperty(this.____JsonSerializationInfo,2);
            writer.WriteBoolean(value.IsProperty);
            writer.WriteStartProperty(this.____JsonSerializationInfo,3);
            writer.WriteBoolean(value.IsField);
            writer.WriteStartProperty(this.____JsonSerializationInfo,4);
            writer.WriteBoolean(value.IsWritable);
            writer.WriteStartProperty(this.____JsonSerializationInfo,5);
            writer.WriteBoolean(value.IsReadable);
            writer.WriteStartProperty(this.____JsonSerializationInfo,6);
            writer.WriteBoolean(value.IsConstructorParameter);
            writer.WriteStartProperty(this.____JsonSerializationInfo,7);
            writer.WriteString(value.Type);
            writer.WriteStartProperty(this.____JsonSerializationInfo,8);
            writer.WriteString(value.Name);
            writer.WriteStartProperty(this.____JsonSerializationInfo,9);
            writer.WriteString(value.MemberName);
            writer.WriteStartProperty(this.____JsonSerializationInfo,10);
            writer.WriteString(value.ShortTypeName);
            
            writer.WriteEndObject();
        }

        public global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo Deserialize(global::Brimborium.Latrans.JSON.JsonReader reader, global::Brimborium.Latrans.JSON.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__Order = default(int);
            var __s__Order = false;
            var __v__IsIgnored = default(bool);
            var __s__IsIgnored = false;
            var __v__IsProperty = default(bool);
            var __s__IsProperty = false;
            var __v__IsField = default(bool);
            var __s__IsField = false;
            var __v__IsWritable = default(bool);
            var __s__IsWritable = false;
            var __v__IsReadable = default(bool);
            var __s__IsReadable = false;
            var __v__IsConstructorParameter = default(bool);
            var __s__IsConstructorParameter = false;
            var __v__Type = default(string);
            var __s__Type = false;
            var __v__Name = default(string);
            var __s__Name = false;
            var __v__MemberName = default(string);
            var __s__MemberName = false;
            var __v__ShortTypeName = default(string);
            var __s__ShortTypeName = false;
            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            //
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                int key;
                if (reader.TryGetParameterValue(this.____JsonSerializationInfo, out key))
                {
                    reader.ReadNextBlock();
                    continue;
                } else {
                    switch (key)
                    {
                         case 0:
                             __v__Order = reader.ReadInt32();
                             __s__Order = true;
                             break;
                         case 1:
                             __v__IsIgnored = reader.ReadBoolean();
                             __s__IsIgnored = true;
                             break;
                         case 2:
                             __v__IsProperty = reader.ReadBoolean();
                             __s__IsProperty = true;
                             break;
                         case 3:
                             __v__IsField = reader.ReadBoolean();
                             __s__IsField = true;
                             break;
                         case 4:
                             __v__IsWritable = reader.ReadBoolean();
                             __s__IsWritable = true;
                             break;
                         case 5:
                             __v__IsReadable = reader.ReadBoolean();
                             __s__IsReadable = true;
                             break;
                         case 6:
                             __v__IsConstructorParameter = reader.ReadBoolean();
                             __s__IsConstructorParameter = true;
                             break;
                         case 7:
                             __v__Type = reader.ReadString();
                             __s__Type = true;
                             break;
                         case 8:
                             __v__Name = reader.ReadString();
                             __s__Name = true;
                             break;
                         case 9:
                             __v__MemberName = reader.ReadString();
                             __s__MemberName = true;
                             break;
                         case 10:
                             __v__ShortTypeName = reader.ReadString();
                             __s__ShortTypeName = true;
                             break;
                    default:
                        reader.ReadNextBlock();
                        break;
                    }
                }
            }

            var ____result = new global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo();
            if(__s__Order) { ____result.Order = __v__Order; }
            if(__s__IsIgnored) { ____result.IsIgnored = __v__IsIgnored; }
            if(__s__IsProperty) { ____result.IsProperty = __v__IsProperty; }
            if(__s__IsField) { ____result.IsField = __v__IsField; }
            if(__s__IsWritable) { ____result.IsWritable = __v__IsWritable; }
            if(__s__IsReadable) { ____result.IsReadable = __v__IsReadable; }
            if(__s__IsConstructorParameter) { ____result.IsConstructorParameter = __v__IsConstructorParameter; }
            if(__s__Type) { ____result.Type = __v__Type; }
            if(__s__Name) { ____result.Name = __v__Name; }
            if(__s__MemberName) { ____result.MemberName = __v__MemberName; }
            if(__s__ShortTypeName) { ____result.ShortTypeName = __v__ShortTypeName; }

            return ____result;
        }
    }

    public sealed class ObjectSerializationInfoFormatter : global::Brimborium.Latrans.JSON.IJsonFormatter<global::Brimborium.Latrans.JSONCodeGenerator.ObjectSerializationInfo>
    {
        private readonly global::Brimborium.Latrans.JSON.JsonSerializationInfo ____JsonSerializationInfo;

        public ObjectSerializationInfoFormatter()
        {
            this.____JsonSerializationInfo = (new global::Brimborium.Latrans.JSON.JsonSerializationInfoBuilder())
                .Add("Name", 0, true, true)
                .Add("Fullname", 1, true, true)
                .Add("Nspace", 2, true, true)
                .Add("IsClass", 3, true, true)
                .Add("IsStruct", 4, true, false)
                .Add("ConstructorParameters", 5, true, true)
                .Add("Members", 6, true, true)
                .Add("FormatterName", 7, true, false)
                .Add("HasConstructor", 8, true, true)
                .Add("WriteCount", 9, true, false)
                .Build();
        }

        public void Serialize(global::Brimborium.Latrans.JSON.JsonWriter writer, global::Brimborium.Latrans.JSONCodeGenerator.ObjectSerializationInfo value, global::Brimborium.Latrans.JSON.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteBeginObject();
            writer.WriteStartProperty(this.____JsonSerializationInfo,0);
            writer.WriteString(value.Name);
            writer.WriteStartProperty(this.____JsonSerializationInfo,1);
            writer.WriteString(value.FullName);
            writer.WriteStartProperty(this.____JsonSerializationInfo,2);
            writer.WriteString(value.Namespace);
            writer.WriteStartProperty(this.____JsonSerializationInfo,3);
            writer.WriteBoolean(value.IsClass);
            writer.WriteStartProperty(this.____JsonSerializationInfo,4);
            writer.WriteBoolean(value.IsStruct);
            writer.WriteStartProperty(this.____JsonSerializationInfo,5);
            formatterResolver.GetFormatterWithVerify<global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo[]>().Serialize(writer, value.ConstructorParameters, formatterResolver);
            writer.WriteStartProperty(this.____JsonSerializationInfo,6);
            formatterResolver.GetFormatterWithVerify<global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo[]>().Serialize(writer, value.Members, formatterResolver);
            writer.WriteStartProperty(this.____JsonSerializationInfo,7);
            writer.WriteString(value.FormatterName);
            writer.WriteStartProperty(this.____JsonSerializationInfo,8);
            writer.WriteBoolean(value.HasConstructor);
            writer.WriteStartProperty(this.____JsonSerializationInfo,9);
            writer.WriteInt32(value.WriteCount);
            
            writer.WriteEndObject();
        }

        public global::Brimborium.Latrans.JSONCodeGenerator.ObjectSerializationInfo Deserialize(global::Brimborium.Latrans.JSON.JsonReader reader, global::Brimborium.Latrans.JSON.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__Name = default(string);
            var __s__Name = false;
            var __v__FullName = default(string);
            var __s__FullName = false;
            var __v__Namespace = default(string);
            var __s__Namespace = false;
            var __v__IsClass = default(bool);
            var __s__IsClass = false;
            var __v__IsStruct = default(bool);
            var __s__IsStruct = false;
            var __v__ConstructorParameters = default(global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo[]);
            var __s__ConstructorParameters = false;
            var __v__Members = default(global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo[]);
            var __s__Members = false;
            var __v__FormatterName = default(string);
            var __s__FormatterName = false;
            var __v__HasConstructor = default(bool);
            var __s__HasConstructor = false;
            var __v__WriteCount = default(int);
            var __s__WriteCount = false;
            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            //
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                int key;
                if (reader.TryGetParameterValue(this.____JsonSerializationInfo, out key))
                {
                    reader.ReadNextBlock();
                    continue;
                } else {
                    switch (key)
                    {
                         case 0:
                             __v__Name = reader.ReadString();
                             __s__Name = true;
                             break;
                         case 1:
                             __v__FullName = reader.ReadString();
                             __s__FullName = true;
                             break;
                         case 2:
                             __v__Namespace = reader.ReadString();
                             __s__Namespace = true;
                             break;
                         case 3:
                             __v__IsClass = reader.ReadBoolean();
                             __s__IsClass = true;
                             break;
                         case 4:
                             __v__IsStruct = reader.ReadBoolean();
                             __s__IsStruct = true;
                             break;
                         case 5:
                             __v__ConstructorParameters = formatterResolver.GetFormatterWithVerify<global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo[]>().Deserialize(reader, formatterResolver);
                             __s__ConstructorParameters = true;
                             break;
                         case 6:
                             __v__Members = formatterResolver.GetFormatterWithVerify<global::Brimborium.Latrans.JSONCodeGenerator.MemberSerializationInfo[]>().Deserialize(reader, formatterResolver);
                             __s__Members = true;
                             break;
                         case 7:
                             __v__FormatterName = reader.ReadString();
                             __s__FormatterName = true;
                             break;
                         case 8:
                             __v__HasConstructor = reader.ReadBoolean();
                             __s__HasConstructor = true;
                             break;
                         case 9:
                             __v__WriteCount = reader.ReadInt32();
                             __s__WriteCount = true;
                             break;
                    default:
                        reader.ReadNextBlock();
                        break;
                    }
                }
            }

            var ____result = new global::Brimborium.Latrans.JSONCodeGenerator.ObjectSerializationInfo();
            if(__s__Name) { ____result.Name = __v__Name; }
            if(__s__FullName) { ____result.FullName = __v__FullName; }
            if(__s__Namespace) { ____result.Namespace = __v__Namespace; }
            if(__s__IsClass) { ____result.IsClass = __v__IsClass; }
            if(__s__ConstructorParameters) { ____result.ConstructorParameters = __v__ConstructorParameters; }
            if(__s__Members) { ____result.Members = __v__Members; }
            if(__s__HasConstructor) { ____result.HasConstructor = __v__HasConstructor; }

            return ____result;
        }
    }

    public sealed class GenericSerializationInfoFormatter : global::Brimborium.Latrans.JSON.IJsonFormatter<global::Brimborium.Latrans.JSONCodeGenerator.GenericSerializationInfo>
    {
        private readonly global::Brimborium.Latrans.JSON.JsonSerializationInfo ____JsonSerializationInfo;

        public GenericSerializationInfoFormatter()
        {
            this.____JsonSerializationInfo = (new global::Brimborium.Latrans.JSON.JsonSerializationInfoBuilder())
                .Add("FullName", 0, true, true)
                .Add("FormatterName", 1, true, true)
                .Build();
        }

        public void Serialize(global::Brimborium.Latrans.JSON.JsonWriter writer, global::Brimborium.Latrans.JSONCodeGenerator.GenericSerializationInfo value, global::Brimborium.Latrans.JSON.IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            

            writer.WriteBeginObject();
            writer.WriteStartProperty(this.____JsonSerializationInfo,0);
            writer.WriteString(value.FullName);
            writer.WriteStartProperty(this.____JsonSerializationInfo,1);
            writer.WriteString(value.FormatterName);
            
            writer.WriteEndObject();
        }

        public global::Brimborium.Latrans.JSONCodeGenerator.GenericSerializationInfo Deserialize(global::Brimborium.Latrans.JSON.JsonReader reader, global::Brimborium.Latrans.JSON.IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            

            var __v__FullName = default(string);
            var __s__FullName = false;
            var __v__FormatterName = default(string);
            var __s__FormatterName = false;
            var ____count = 0;
            reader.ReadIsBeginObjectWithVerify();
            //
            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))
            {
                int key;
                if (reader.TryGetParameterValue(this.____JsonSerializationInfo, out key))
                {
                    reader.ReadNextBlock();
                    continue;
                } else {
                    switch (key)
                    {
                         case 0:
                             __v__FullName = reader.ReadString();
                             __s__FullName = true;
                             break;
                         case 1:
                             __v__FormatterName = reader.ReadString();
                             __s__FormatterName = true;
                             break;
                    default:
                        reader.ReadNextBlock();
                        break;
                    }
                }
            }

            var ____result = new global::Brimborium.Latrans.JSONCodeGenerator.GenericSerializationInfo();
            if(__s__FullName) { ____result.FullName = __v__FullName; }
            if(__s__FormatterName) { ____result.FormatterName = __v__FormatterName; }

            return ____result;
        }
    }
}

#pragma warning disable 168
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612
