#nullable disable

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;

namespace Brimborium.Latrans.JSONCodeGenerator {

    public class TypeCollector {
        const string CodegeneratorOnlyPreprocessorSymbol = "INCLUDE_ONLY_CODE_GENERATION";

        static readonly SymbolDisplayFormat binaryWriteFormat = new SymbolDisplayFormat(
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.ExpandNullable,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly);

        static readonly SymbolDisplayFormat shortTypeNameFormat = new SymbolDisplayFormat(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes);
        static readonly HashSet<string> embeddedTypes = new HashSet<string>(new string[]
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
            "decimal",
            "char",
            "string",
            "object",
            "System.Guid",
            "System.TimeSpan",
            "System.DateTime",
            "System.DateTimeOffset",

            // "MessagePack.Nil",

            // and arrays
            
            "short[]",
            "int[]",
            "long[]",
            "ushort[]",
            "uint[]",
            "ulong[]",
            "float[]",
            "double[]",
            "bool[]",
            "byte[]",
            "sbyte[]",
            "decimal[]",
            "char[]",
            "string[]",
            "System.DateTime[]",
            "System.ArraySegment<byte>",
            "System.ArraySegment<byte>?",

            // extensions
            /*
            "UnityEngine.Vector2",
            "UnityEngine.Vector3",
            "UnityEngine.Vector4",
            "UnityEngine.Quaternion",
            "UnityEngine.Color",
            "UnityEngine.Bounds",
            "UnityEngine.Rect",
            */
            // "System.Reactive.Unit",
        });
        static readonly Dictionary<string, string> knownGenericTypes = new Dictionary<string, string>
        {
            {"System.Collections.Generic.List<>", "global::Brimborium.Latrans.JSON.Formatters.ListFormatter<TREPLACE>" },
            {"System.Collections.Generic.LinkedList<>", "global::Brimborium.Latrans.JSON.Formatters.LinkedListFormatter<TREPLACE>"},
            {"System.Collections.Generic.Queue<>", "global::Brimborium.Latrans.JSON.Formatters.QeueueFormatter<TREPLACE>"},
            {"System.Collections.Generic.Stack<>", "global::Brimborium.Latrans.JSON.Formatters.StackFormatter<TREPLACE>"},
            {"System.Collections.Generic.HashSet<>", "global::Brimborium.Latrans.JSON.Formatters.HashSetFormatter<TREPLACE>"},
            {"System.Collections.ObjectModel.ReadOnlyCollection<>", "global::Brimborium.Latrans.JSON.Formatters.ReadOnlyCollectionFormatter<TREPLACE>"},
            {"System.Collections.Generic.IList<>", "global::Brimborium.Latrans.JSON.Formatters.InterfaceListFormatter<TREPLACE>"},
            {"System.Collections.Generic.ICollection<>", "global::Brimborium.Latrans.JSON.Formatters.InterfaceCollectionFormatter<TREPLACE>"},
            {"System.Collections.Generic.IEnumerable<>", "global::Brimborium.Latrans.JSON.Formatters.InterfaceEnumerableFormatter<TREPLACE>"},
            {"System.Collections.Generic.Dictionary<,>", "global::Brimborium.Latrans.JSON.Formatters.DictionaryFormatter<TREPLACE>"},
            {"System.Collections.Generic.IDictionary<,>", "global::Brimborium.Latrans.JSON.Formatters.InterfaceDictionaryFormatter<TREPLACE>"},
            {"System.Collections.Generic.SortedDictionary<,>", "global::Brimborium.Latrans.JSON.Formatters.SortedDictionaryFormatter<TREPLACE>"},
            {"System.Collections.Generic.SortedList<,>", "global::Brimborium.Latrans.JSON.Formatters.SortedListFormatter<TREPLACE>"},
            {"System.Linq.ILookup<,>", "global::Brimborium.Latrans.JSON.Formatters.InterfaceLookupFormatter<TREPLACE>"},
            {"System.Linq.IGrouping<,>", "global::Brimborium.Latrans.JSON.Formatters.InterfaceGroupingFormatter<TREPLACE>"},
            {"System.Collections.ObjectModel.ObservableCollection<>", "global::Brimborium.Latrans.JSON.Formatters.ObservableCollectionFormatter<TREPLACE>"},
            {"System.Collections.ObjectModel.ReadOnlyObservableCollection<>", "global::Brimborium.Latrans.JSON.Formatters.ReadOnlyObservableCollectionFormatter<TREPLACE>" },
            {"System.Collections.Generic.IReadOnlyList<>", "global::Brimborium.Latrans.JSON.Formatters.InterfaceReadOnlyListFormatter<TREPLACE>"},
            {"System.Collections.Generic.IReadOnlyCollection<>", "global::Brimborium.Latrans.JSON.Formatters.InterfaceReadOnlyCollectionFormatter<TREPLACE>"},
            {"System.Collections.Generic.ISet<>", "global::Brimborium.Latrans.JSON.Formatters.InterfaceSetFormatter<TREPLACE>"},
            {"System.Collections.Concurrent.ConcurrentBag<>", "global::Brimborium.Latrans.JSON.Formatters.ConcurrentBagFormatter<TREPLACE>"},
            {"System.Collections.Concurrent.ConcurrentQueue<>", "global::Brimborium.Latrans.JSON.Formatters.ConcurrentQueueFormatter<TREPLACE>"},
            {"System.Collections.Concurrent.ConcurrentStack<>", "global::Brimborium.Latrans.JSON.Formatters.ConcurrentStackFormatter<TREPLACE>"},
            {"System.Collections.ObjectModel.ReadOnlyDictionary<,>", "global::Brimborium.Latrans.JSON.Formatters.ReadOnlyDictionaryFormatter<TREPLACE>"},
            {"System.Collections.Generic.IReadOnlyDictionary<,>", "global::Brimborium.Latrans.JSON.Formatters.InterfaceReadOnlyDictionaryFormatter<TREPLACE>"},
            {"System.Collections.Concurrent.ConcurrentDictionary<,>", "global::Brimborium.Latrans.JSON.Formatters.ConcurrentDictionaryFormatter<TREPLACE>"},
            {"System.Lazy<>", "global::Brimborium.Latrans.JSON.Formatters.LazyFormatter<TREPLACE>"},
            {"System.Threading.Tasks<>", "global::Brimborium.Latrans.JSON.Formatters.TaskValueFormatter<TREPLACE>"},

            {"System.Tuple<>", "global::Brimborium.Latrans.JSON.Formatters.TupleFormatter<TREPLACE>"},
            {"System.Tuple<,>", "global::Brimborium.Latrans.JSON.Formatters.TupleFormatter<TREPLACE>"},
            {"System.Tuple<,,>", "global::Brimborium.Latrans.JSON.Formatters.TupleFormatter<TREPLACE>"},
            {"System.Tuple<,,,>", "global::Brimborium.Latrans.JSON.Formatters.TupleFormatter<TREPLACE>"},
            {"System.Tuple<,,,,>", "global::Brimborium.Latrans.JSON.Formatters.TupleFormatter<TREPLACE>"},
            {"System.Tuple<,,,,,>", "global::Brimborium.Latrans.JSON.Formatters.TupleFormatter<TREPLACE>"},
            {"System.Tuple<,,,,,,>", "global::Brimborium.Latrans.JSON.Formatters.TupleFormatter<TREPLACE>"},
            {"System.Tuple<,,,,,,,>", "global::Brimborium.Latrans.JSON.Formatters.TupleFormatter<TREPLACE>"},

            {"System.ValueTuple<>", "global::Brimborium.Latrans.JSON.Formatters.ValueTupleFormatter<TREPLACE>"},
            {"System.ValueTuple<,>", "global::Brimborium.Latrans.JSON.Formatters.ValueTupleFormatter<TREPLACE>"},
            {"System.ValueTuple<,,>", "global::Brimborium.Latrans.JSON.Formatters.ValueTupleFormatter<TREPLACE>"},
            {"System.ValueTuple<,,,>", "global::Brimborium.Latrans.JSON.Formatters.ValueTupleFormatter<TREPLACE>"},
            {"System.ValueTuple<,,,,>", "global::Brimborium.Latrans.JSON.Formatters.ValueTupleFormatter<TREPLACE>"},
            {"System.ValueTuple<,,,,,>", "global::Brimborium.Latrans.JSON.Formatters.ValueTupleFormatter<TREPLACE>"},
            {"System.ValueTuple<,,,,,,>", "global::Brimborium.Latrans.JSON.Formatters.ValueTupleFormatter<TREPLACE>"},
            {"System.ValueTuple<,,,,,,,>", "global::Brimborium.Latrans.JSON.Formatters.ValueTupleFormatter<TREPLACE>"},

            {"System.Collections.Generic.KeyValuePair<,>", "global::Brimborium.Latrans.JSON.Formatters.KeyValuePairFormatter<TREPLACE>"},
            {"System.Threading.Tasks.ValueTask<>", "global::Brimborium.Latrans.JSON.Formatters.KeyValuePairFormatter<TREPLACE>"},
            {"System.ArraySegment<>", "global::Brimborium.Latrans.JSON.Formatters.ArraySegmentFormatter<TREPLACE>"},

            // extensions

            {"System.Collections.Immutable.ImmutableArray<>", "global::Brimborium.Latrans.JSON.ImmutableCollection.ImmutableArrayFormatter<TREPLACE>"},
            {"System.Collections.Immutable.ImmutableList<>", "global::Brimborium.Latrans.JSON.ImmutableCollection.ImmutableListFormatter<TREPLACE>"},
            {"System.Collections.Immutable.ImmutableDictionary<,>", "global::Brimborium.Latrans.JSON.ImmutableCollection.ImmutableDictionaryFormatter<TREPLACE>"},
            {"System.Collections.Immutable.ImmutableHashSet<>", "global::Brimborium.Latrans.JSON.ImmutableCollection.ImmutableHashSetFormatter<TREPLACE>"},
            {"System.Collections.Immutable.ImmutableSortedDictionary<,>", "global::Brimborium.Latrans.JSON.ImmutableCollection.ImmutableSortedDictionaryFormatter<TREPLACE>"},
            {"System.Collections.Immutable.ImmutableSortedSet<>", "global::Brimborium.Latrans.JSON.ImmutableCollection.ImmutableSortedSetFormatter<TREPLACE>"},
            {"System.Collections.Immutable.ImmutableQueue<>", "global::Brimborium.Latrans.JSON.ImmutableCollection.ImmutableQueueFormatter<TREPLACE>"},
            {"System.Collections.Immutable.ImmutableStack<>", "global::Brimborium.Latrans.JSON.ImmutableCollection.ImmutableStackFormatter<TREPLACE>"},
            {"System.Collections.Immutable.IImmutableList<>", "global::Brimborium.Latrans.JSON.ImmutableCollection.InterfaceImmutableListFormatter<TREPLACE>"},
            {"System.Collections.Immutable.IImmutableDictionary<,>", "global::Brimborium.Latrans.JSON.ImmutableCollection.InterfaceImmutableDictionaryFormatter<TREPLACE>"},
            {"System.Collections.Immutable.IImmutableQueue<>", "global::Brimborium.Latrans.JSON.ImmutableCollection.InterfaceImmutableQueueFormatter<TREPLACE>"},
            {"System.Collections.Immutable.IImmutableSet<>", "global::Brimborium.Latrans.JSON.ImmutableCollection.InterfaceImmutableSetFormatter<TREPLACE>"},
            {"System.Collections.Immutable.IImmutableStack<>", "global::Brimborium.Latrans.JSON.ImmutableCollection.InterfaceImmutableStackFormatter<TREPLACE>"},
        };


        readonly ReferenceSymbols typeReferences;
        readonly INamedTypeSymbol[] targetTypes;
        readonly bool disallowInternal;

        // visitor workspace:
        HashSet<ITypeSymbol> alreadyCollected;
        List<ObjectSerializationInfo> collectedObjectInfo;
        List<GenericSerializationInfo> collectedGenericInfo;

        // --- 

        public TypeCollector(
            IEnumerable<string> inputFiles,
            IEnumerable<string> inputDirs,
            IEnumerable<string> conditinalSymbols,
            bool disallowInternal) {
            var compilation = RoslynExtensions.GetCompilationFromProject(inputFiles, inputDirs,
                conditinalSymbols.Concat(new[] { CodegeneratorOnlyPreprocessorSymbol }).ToArray()
            );
            
            this.typeReferences = new ReferenceSymbols(compilation);
            this.disallowInternal = disallowInternal;

            targetTypes = compilation.GetNamedTypeSymbols()
                .Where(x => {
                    if (x.DeclaredAccessibility == Accessibility.Public) {
                        return true;
                    }

                    if (!disallowInternal) {
                        return (x.DeclaredAccessibility == Accessibility.Friend);
                    }

                    return false;
                })
                .Where(x => (x.TypeKind == TypeKind.Interface) || (x.TypeKind == TypeKind.Class) || (x.TypeKind == TypeKind.Struct))
                .ToArray();
        }

        void ResetWorkspace() {
            alreadyCollected = new HashSet<ITypeSymbol>();
            collectedObjectInfo = new List<ObjectSerializationInfo>();
            collectedGenericInfo = new List<GenericSerializationInfo>();
        }

        // EntryPoint
        public (ObjectSerializationInfo[] objectInfo, GenericSerializationInfo[] genericInfo) Collect() {
            ResetWorkspace();

            foreach (var item in targetTypes) {
                CollectCore(item);
            }

            return (collectedObjectInfo.ToArray(), collectedGenericInfo.Distinct().ToArray());
        }

        // Gate of recursive collect
        void CollectCore(ITypeSymbol typeSymbol) {
            if (!alreadyCollected.Add(typeSymbol)) {
                return;
            }

            if (embeddedTypes.Contains(typeSymbol.ToString())) {
                return;
            }

            if (typeSymbol.TypeKind == TypeKind.Array) {
                CollectArray(typeSymbol as IArrayTypeSymbol);
                return;
            }

            if (!IsAllowAccessibility(typeSymbol)) {
                return;
            }

            var type = typeSymbol as INamedTypeSymbol;

            if (typeSymbol.TypeKind == TypeKind.Enum) {
                return;
            }

            if (type.IsGenericType) {
                CollectGeneric(type);
                return;
            }

            if (type.Locations[0].IsInMetadata) {
                return;
            }

            CollectObject(type);
            return;
        }

        void CollectArray(IArrayTypeSymbol array) {
            var elemType = array.ElementType;
            CollectCore(elemType);

            var info = new GenericSerializationInfo {
                FullName = array.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            };

            if (array.IsSZArray) {
                info.FormatterName = $"global::Brimborium.Latrans.JSON.Formatters.ArrayFormatter<{elemType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>";
            } else if (array.Rank == 2) {
                info.FormatterName = $"global::Brimborium.Latrans.JSON.Formatters.TwoDimentionalArrayFormatter<{elemType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>";
            } else if (array.Rank == 3) {
                info.FormatterName = $"global::Brimborium.Latrans.JSON.Formatters.ThreeDimentionalArrayFormatter<{elemType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>";
            } else if (array.Rank == 4) {
                info.FormatterName = $"global::Brimborium.Latrans.JSON.Formatters.FourDimentionalArrayFormatter<{elemType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>";
            } else {
                throw new InvalidOperationException("does not supports array dimention, " + info.FullName);
            }

            collectedGenericInfo.Add(info);

            return;
        }

        void CollectGeneric(INamedTypeSymbol type) {
            var genericType = type.ConstructUnboundGenericType();
            var genericTypeString = genericType.ToDisplayString();
            var fullName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            // special case
            if (fullName == "global::System.ArraySegment<byte>" || fullName == "global::System.ArraySegment<byte>?") {
                return;
            }

            // nullable
            if (genericTypeString == "T?") {
                CollectCore(type.TypeArguments[0]);

                if (!embeddedTypes.Contains(type.TypeArguments[0].ToString())) {
                    var info = new GenericSerializationInfo {
                        FormatterName = $"global::Brimborium.Latrans.JSON.Formatters.NullableFormatter<{type.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>",
                        FullName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    };

                    collectedGenericInfo.Add(info);
                }
                return;
            }

            // collection
            if (knownGenericTypes.TryGetValue(genericTypeString, out var formatter)) {
                foreach (var item in type.TypeArguments) {
                    CollectCore(item);
                }

                var typeArgs = string.Join(", ", type.TypeArguments.Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
                var f = formatter.Replace("TREPLACE", typeArgs);

                var info = new GenericSerializationInfo {
                    FormatterName = f,
                    FullName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                };

                collectedGenericInfo.Add(info);

                if (genericTypeString == "System.Linq.ILookup<,>") {
                    formatter = knownGenericTypes["System.Linq.IGrouping<,>"];
                    f = formatter.Replace("TREPLACE", typeArgs);

                    var groupingInfo = new GenericSerializationInfo {
                        FormatterName = f,
                        FullName = $"global::System.Linq.IGrouping<{typeArgs}>",
                    };

                    collectedGenericInfo.Add(groupingInfo);

                    formatter = knownGenericTypes["System.Collections.Generic.IEnumerable<>"];
                    typeArgs = type.TypeArguments[1].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    f = formatter.Replace("TREPLACE", typeArgs);

                    var enumerableInfo = new GenericSerializationInfo {
                        FormatterName = f,
                        FullName = $"global::System.Collections.Generic.IEnumerable<{typeArgs}>",
                    };

                    collectedGenericInfo.Add(enumerableInfo);
                }
            }
        }

        void CollectObject(INamedTypeSymbol type) {
            var isClass = !type.IsValueType;

            var dictMembers = new Dictionary<string, MemberSerializationInfo>();

            int position = 0;
            foreach (var item in type.GetAllMembers().OfType<IPropertySymbol>().Where(x => !x.IsOverride)) {
                var isIgnored = (item.GetAttributes().FindAttributeShortName(typeReferences.IgnoreDataMemberAttribute) != null);
                var dm = item.GetAttributes().FindAttributeShortName(typeReferences.DataMemberAttribute);

                var name = (dm.GetSingleNamedArgumentValueFromSyntaxTree("Name") as string) ?? item.Name;
                if (dm.GetSingleNamedArgumentValueFromSyntaxTree("Order") is int order) {
                } else {
                    order = int.MaxValue;
                }

                var member = new MemberSerializationInfo {
                    Order = order,
                    Position = position++,
                    IsIgnored = isIgnored,
                    IsReadable = (item.GetMethod != null) && item.GetMethod.DeclaredAccessibility == Accessibility.Public && !item.IsStatic,
                    IsWritable = (item.SetMethod != null) && item.SetMethod.DeclaredAccessibility == Accessibility.Public && !item.IsStatic,
                    MemberName = item.Name,
                    IsProperty = true,
                    IsField = false,
                    Name = name,
                    Type = item.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    ShortTypeName = item.Type.ToDisplayString(binaryWriteFormat)
                };
                if (!member.IsReadable && !member.IsWritable) {
                    continue;
                }

                dictMembers.Add(name, member);
                if (!member.IsIgnored) {
                    CollectCore(item.Type); // recursive collect
                }
            }
            foreach (var item in type.GetAllMembers().OfType<IFieldSymbol>()) {
                if (item.IsImplicitlyDeclared) {
                    continue;
                }

                var isIgnored = (item.GetAttributes().FindAttributeShortName(typeReferences.IgnoreDataMemberAttribute) != null);
                var dm = item.GetAttributes().FindAttributeShortName(typeReferences.DataMemberAttribute);
                var name = (dm.GetSingleNamedArgumentValueFromSyntaxTree("Name") as string) ?? item.Name;
                if (dm.GetSingleNamedArgumentValueFromSyntaxTree("Order") is int order) {
                } else {
                    order = int.MaxValue;
                }

                var member = new MemberSerializationInfo {
                    Order = order,
                    Position = position++,
                    IsIgnored = isIgnored,
                    IsReadable = item.DeclaredAccessibility == Accessibility.Public && !item.IsStatic,
                    IsWritable = item.DeclaredAccessibility == Accessibility.Public && !item.IsReadOnly && !item.IsStatic,
                    MemberName = item.Name,
                    IsProperty = false,
                    IsField = true,
                    Name = name,
                    Type = item.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    ShortTypeName = item.Type.ToDisplayString(binaryWriteFormat)
                };
                if (!member.IsReadable && !member.IsWritable) {
                    continue;
                }

                dictMembers.Add(name, member);
                if (!member.IsIgnored) {
                    CollectCore(item.Type); // recursive collect
                }
            }

            var arrValue = dictMembers.Values.OrderBy(m => m.Order).ThenBy(m => m.Position).ToArray();
            {
                int idxOrder = 0;
                int idxPosition = 0;
                foreach (var m in arrValue) {
                    if (m.IsIgnored) {
                        //
                        m.Order = -1;
                    } else {
                        m.Order = idxOrder++;
                    }
                    m.Position = idxPosition++;
                }
            }

            // GetConstructor
            IEnumerator<IMethodSymbol> ctorEnumerator = null;
            var ctor = type.Constructors.Where(x => x.DeclaredAccessibility == Accessibility.Public)
                .SingleOrDefault(x => x.GetAttributes().FindAttributeShortName(typeReferences.SerializationConstructorAttribute) != null);
            if (ctor == null) {
                ctorEnumerator =
                    type.Constructors.Where(x => x.DeclaredAccessibility == Accessibility.Public && !x.IsImplicitlyDeclared).OrderBy(x => x.Parameters.Length)
                    .Concat(type.Constructors.Where(x => x.DeclaredAccessibility == Accessibility.Public).OrderByDescending(x => x.Parameters.Length).Take(1))
                    .GetEnumerator();

                if (ctorEnumerator.MoveNext()) {
                    ctor = ctorEnumerator.Current;
                }
            }

            var constructorParameters = new List<MemberSerializationInfo>();
            if (ctor != null) {
                var constructorLookupDictionary = dictMembers.ToLookup(x => x.Key, x => x, StringComparer.OrdinalIgnoreCase);
                do {
                    constructorParameters.Clear();
                    var ctorParamIndex = 0;
                    foreach (var item in ctor.Parameters) {
                        MemberSerializationInfo paramMember;

                        var hasKey = constructorLookupDictionary[item.Name];
                        var len = hasKey.Count();
                        if (len != 0) {
                            if (len != 1) {
                                if (ctorEnumerator != null) {
                                    ctor = null;
                                    continue;
                                } else {
                                    throw new CodeGeneratorResolveFailedException("duplicate matched constructor parameter name:" + type.Name + " parameterName:" + item.Name + " paramterType:" + item.Type.Name);
                                }
                            }

                            paramMember = hasKey.First().Value;
                            if (item.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == paramMember.Type && paramMember.IsReadable) {
                                constructorParameters.Add(paramMember);
                            } else {
                                if (ctorEnumerator != null) {
                                    ctor = null;
                                    continue;
                                } else {
                                    throw new CodeGeneratorResolveFailedException("can't find matched constructor parameter, parameterType mismatch. type:" + type.Name + " parameterName:" + item.Name + " paramterType:" + item.Type.Name);
                                }
                            }
                        } else {
                            if (ctorEnumerator != null) {
                                ctor = null;
                                continue;
                            } else {
                                throw new CodeGeneratorResolveFailedException("can't find matched constructor parameter, index not found. type:" + type.Name + " parameterName:" + item.Name);
                            }
                        }
                        ctorParamIndex++;
                    }
                } while (TryGetNextConstructor(ctorEnumerator, ref ctor));

                if (ctor == null) {
                    return; // ignore instead of exception
                    // throw new CodeGeneratorResolveFailedException("can't find matched constructor. type:" + type.Name);
                }
            }

            if (dictMembers.Count == 0) {
                return;
            }
            foreach (var x in constructorParameters) {
                x.IsConstructorParameter = true;
            }
            var info = new ObjectSerializationInfo {
                IsClass = isClass,
                ConstructorParameters = constructorParameters.ToArray(),
                Members = dictMembers.Values.ToArray(),
                Name = type.ToDisplayString(shortTypeNameFormat).Replace(".", "_"),
                FullName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                Namespace = type.ContainingNamespace.IsGlobalNamespace ? null : type.ContainingNamespace.ToDisplayString(),
                HasConstructor = ctor != null
            };
            collectedObjectInfo.Add(info);
        }

        static bool TryGetNextConstructor(IEnumerator<IMethodSymbol> ctorEnumerator, ref IMethodSymbol ctor) {
            if (ctorEnumerator == null || ctor != null) {
                return false;
            }

            if (ctorEnumerator.MoveNext()) {
                ctor = ctorEnumerator.Current;
                return true;
            } else {
                ctor = null;
                return false;
            }
        }

        bool IsAllowAccessibility(ITypeSymbol symbol) {
            do {
                if (symbol.DeclaredAccessibility != Accessibility.Public) {
                    if (disallowInternal) {
                        return false;
                    }

                    if (symbol.DeclaredAccessibility != Accessibility.Internal) {
                        return true;
                    }
                }

                symbol = symbol.ContainingType;
            } while (symbol != null);

            return true;
        }
    }
}