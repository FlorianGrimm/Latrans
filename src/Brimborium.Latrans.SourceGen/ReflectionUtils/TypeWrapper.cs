// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace System.Reflection {
    internal class TypeWrapper : Type {
        private readonly ITypeSymbol _TypeSymbol;
        private readonly MetadataLoadContext _MetadataLoadContext;
        private INamedTypeSymbol? _NamedTypeSymbol;
        private IArrayTypeSymbol? _ArrayTypeSymbol;
        private Type? _ElementType;
        private Type? _ValueType;
        private Type? _EnumType;

        public TypeWrapper(ITypeSymbol namedTypeSymbol, MetadataLoadContext metadataLoadContext) {
            _TypeSymbol = namedTypeSymbol;
            _MetadataLoadContext = metadataLoadContext;
            _NamedTypeSymbol = _TypeSymbol as INamedTypeSymbol;
            _ArrayTypeSymbol = _TypeSymbol as IArrayTypeSymbol;
        }

        public override Assembly Assembly => new AssemblyWrapper(_TypeSymbol.ContainingAssembly, _MetadataLoadContext);

        public override string AssemblyQualifiedName => throw new NotImplementedException();

        public override Type BaseType => _TypeSymbol.BaseType?.AsType(_MetadataLoadContext) ?? throw new ArgumentException("No BaseType??");

        public override string FullName => Namespace + "." + Name;

        public override Guid GUID => Guid.Empty;

        public override Module Module => throw new NotImplementedException();

        public override string Namespace =>
            IsArray ?
            GetElementType().Namespace :
            _TypeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining))!;

        public override Type UnderlyingSystemType => this;

        public override string Name {
            get {
                if (_ArrayTypeSymbol == null) {
                    return _TypeSymbol.MetadataName;
                }

                Type elementType = GetElementType();
                return elementType.Name + "[]";
            }
        }

        public override bool IsEnum {
            get {

                _EnumType ??= (
                    _MetadataLoadContext.Resolve(typeof(Enum))
                    ?? throw new ArgumentException("typeof(Enum) is not found."));
                return IsSubclassOf(_EnumType);
            }
        }

        public override bool IsGenericType => _NamedTypeSymbol?.IsGenericType == true;

        public override bool ContainsGenericParameters => _NamedTypeSymbol?.IsUnboundGenericType == true;

        public override bool IsGenericTypeDefinition => base.IsGenericTypeDefinition;

        public INamespaceSymbol GetNamespaceSymbol => _TypeSymbol.ContainingNamespace;

        public override Type[] GetGenericArguments() {
            if (_NamedTypeSymbol is null) {
                return new Type[0];
            } else {
                var args = new List<Type>();
                foreach (ITypeSymbol item in _NamedTypeSymbol.TypeArguments) {
                    args.Add(item.AsType(_MetadataLoadContext));
                }
                return args.ToArray();
            }
        }

        public override Type GetGenericTypeDefinition() {
            if (_NamedTypeSymbol is null) {
                return null!;
            } else {
                return _NamedTypeSymbol.ConstructedFrom.AsType(_MetadataLoadContext);
            }
        }

        public override IList<CustomAttributeData> GetCustomAttributesData() {
            var attributes = new List<CustomAttributeData>();
            foreach (AttributeData a in _TypeSymbol.GetAttributes()) {
                attributes.Add(new CustomAttributeDataWrapper(a, _MetadataLoadContext));
            }
            return attributes;
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) {
            var ctors = new List<ConstructorInfo>();
            if (_NamedTypeSymbol is object) {
                foreach (IMethodSymbol c in _NamedTypeSymbol.Constructors) {
                    ctors.Add(new ConstructorInfoWrapper(c, _MetadataLoadContext));
                }
            }
            return ctors.ToArray();
        }

        public override object[] GetCustomAttributes(bool inherit) {
            throw new NotSupportedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            throw new NotSupportedException();
        }

        public override Type GetElementType() {
            _ElementType ??= _ArrayTypeSymbol?.ElementType.AsType(_MetadataLoadContext)!;
            return _ElementType;
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr) {
            var fields = new List<FieldInfo>();
            foreach (ISymbol item in _TypeSymbol.GetMembers()) {
                // Associated Symbol checks the field is not a backingfield.
                if (item is IFieldSymbol field && field.AssociatedSymbol == null && !field.IsReadOnly) {
                    if ((item.DeclaredAccessibility & Accessibility.Public) == Accessibility.Public) {
                        fields.Add(new FieldInfoWrapper(field, _MetadataLoadContext));
                    }
                }
            }
            return fields.ToArray();
        }

        public override Type GetInterface(string name, bool ignoreCase) {
            throw new NotImplementedException();
        }

        public override Type[] GetInterfaces() {
            var interfaces = new List<Type>();
            foreach (INamedTypeSymbol i in _TypeSymbol.Interfaces) {
                interfaces.Add(i.AsType(_MetadataLoadContext));
            }
            return interfaces.ToArray();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr) {
            var members = new List<MemberInfo>();
            foreach (ISymbol m in _TypeSymbol.GetMembers()) {
                members.Add(new MemberInfoWrapper(m, _MetadataLoadContext));
            }
            return members.ToArray();
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr) {
            var methods = new List<MethodInfo>();
            if (_NamedTypeSymbol is object) {
                foreach (ISymbol m in _TypeSymbol.GetMembers()) {
                    // TODO: Efficiency
                    if (m is IMethodSymbol method && !_NamedTypeSymbol.Constructors.Contains(method)) {
                        methods.Add(method.AsMethodInfo(_MetadataLoadContext));
                    }
                }
            }
            return methods.ToArray();
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr) {
            throw new NotImplementedException();
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr) {
            var nestedTypes = new List<Type>();
            foreach (INamedTypeSymbol type in _TypeSymbol.GetTypeMembers()) {
                nestedTypes.Add(type.AsType(_MetadataLoadContext));
            }
            return nestedTypes.ToArray();
        }

        // TODO: make sure to use bindingAttr for correctness. Current implementation assumes public and non-static.
        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) {
            var properties = new List<PropertyInfo>();

            foreach (ISymbol item in _TypeSymbol.GetMembers()) {
                if (item is IPropertySymbol property && !property.IsReadOnly) {
                    if ((item.DeclaredAccessibility & Accessibility.Public) == Accessibility.Public) {
                        properties.Add(new PropertyWrapper(property, _MetadataLoadContext));
                    }
                }
            }

            return properties.ToArray();
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) {
            throw new NotSupportedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            throw new NotImplementedException();
        }

        protected override TypeAttributes GetAttributeFlagsImpl() {
            throw new NotImplementedException();
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) {
            throw new NotImplementedException();
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) {
            throw new NotImplementedException();
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) {
            throw new NotImplementedException();
        }

        protected override bool HasElementTypeImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsArrayImpl() {
            return _ArrayTypeSymbol != null;
        }

        protected override bool IsValueTypeImpl() {
            _ValueType ??= (_MetadataLoadContext.Resolve(typeof(ValueType))
                ?? throw new ArgumentException("typeof(ValueType) not found."));
            return IsSubclassOf(_ValueType);
        }

        protected override bool IsByRefImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsCOMObjectImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsPointerImpl() {
            throw new NotImplementedException();
        }

        protected override bool IsPrimitiveImpl() {
            throw new NotImplementedException();
        }

        public override bool IsAssignableFrom(Type c) {
            if (c is TypeWrapper tr) {
                return tr._TypeSymbol.AllInterfaces.Contains(_TypeSymbol) || (tr._NamedTypeSymbol != null && tr._NamedTypeSymbol.BaseTypes().Contains(_TypeSymbol));
            } else if (_MetadataLoadContext.Resolve(c) is TypeWrapper trr) {
                return trr._TypeSymbol.AllInterfaces.Contains(_TypeSymbol) || (trr._NamedTypeSymbol != null && trr._NamedTypeSymbol.BaseTypes().Contains(_TypeSymbol));
            }
            return false;
        }

        public override int GetHashCode() {
            return _TypeSymbol.GetHashCode();
        }

        public override bool Equals(object o) {
            if (o is TypeWrapper tw) {
                return _TypeSymbol.Equals(tw._TypeSymbol, SymbolEqualityComparer.Default);
            } else if (o is Type t && _MetadataLoadContext.Resolve(t) is TypeWrapper tww) {
                return _TypeSymbol.Equals(tww._TypeSymbol, SymbolEqualityComparer.Default);
            }

            return base.Equals(o);
        }

        public override bool Equals(Type o) {
            if (o is TypeWrapper tw) {
                return _TypeSymbol.Equals(tw._TypeSymbol, SymbolEqualityComparer.Default);
            } else if (_MetadataLoadContext.Resolve(o) is TypeWrapper tww) {
                return _TypeSymbol.Equals(tww._TypeSymbol, SymbolEqualityComparer.Default);
            }
            return base.Equals(o);
        }
    }
}
