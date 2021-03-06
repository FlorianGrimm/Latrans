﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace System.Reflection {
    internal class AssemblyWrapper : Assembly {
        private readonly MetadataLoadContext _metadataLoadContext;

        public AssemblyWrapper(IAssemblySymbol assembly, MetadataLoadContext metadataLoadContext) {
            Symbol = assembly;
            _metadataLoadContext = metadataLoadContext;
        }

        internal IAssemblySymbol Symbol { get; }

        public override string FullName => Symbol.Identity.Name;

        public override Type[] GetExportedTypes() {
            return GetTypes();
        }

        public override Type[] GetTypes() {
            var types = new List<Type>();
            var stack = new Stack<INamespaceSymbol>();
            stack.Push(Symbol.GlobalNamespace);
            while (stack.Count > 0) {
                INamespaceSymbol current = stack.Pop();

                foreach (INamedTypeSymbol type in current.GetTypeMembers()) {
                    var t = type.AsType(_metadataLoadContext);
                    if (t is object) {
                        types.Add(t);
                    }
                }

                foreach (INamespaceSymbol ns in current.GetNamespaceMembers()) {
                    stack.Push(ns);
                }
            }
            return types.ToArray();
        }

        public override Type GetType(string name) {
            return Symbol.GetTypeByMetadataName(name)!.AsType(_metadataLoadContext) ?? throw new ArgumentException();
        }
    }
}
