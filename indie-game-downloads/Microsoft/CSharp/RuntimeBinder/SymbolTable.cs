using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.CSharp.RuntimeBinder.Semantics;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder;

internal static class SymbolTable
{
	private readonly struct NameHashKey : IEquatable<NameHashKey>
	{
		internal Type Type { get; }

		internal string Name { get; }

		public NameHashKey(Type type, string name)
		{
			Type = type;
			Name = name;
		}

		public bool Equals(NameHashKey other)
		{
			if (Type.Equals(other.Type))
			{
				return Name.Equals(other.Name);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is NameHashKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode() ^ Name.GetHashCode();
		}
	}

	private static readonly HashSet<Type> s_typesWithConversionsLoaded = new HashSet<Type>();

	private static readonly HashSet<NameHashKey> s_namesLoadedForEachType = new HashSet<NameHashKey>();

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static void PopulateSymbolTableWithName(string name, IEnumerable<Type> typeArguments, Type callingType)
	{
		if (callingType.IsGenericType)
		{
			callingType = callingType.GetGenericTypeDefinition();
		}
		if (name == "$Item$")
		{
			name = callingType.GetIndexerName() ?? "$Item$";
		}
		NameHashKey nameHashKey = new NameHashKey(callingType, name);
		if (s_namesLoadedForEachType.Contains(nameHashKey))
		{
			return;
		}
		AddNamesOnType(nameHashKey);
		if (typeArguments == null)
		{
			return;
		}
		foreach (Type typeArgument in typeArguments)
		{
			AddConversionsForType(typeArgument);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static SymWithType LookupMember(string name, Expr callingObject, ParentSymbol context, int arity, MemberLookup mem, bool allowSpecialNames, bool requireInvocable)
	{
		CType cType = callingObject.Type;
		if (cType is ArrayType)
		{
			cType = SymbolLoader.GetPredefindType(PredefinedType.PT_ARRAY);
		}
		if (cType is NullableType nullableType)
		{
			cType = nullableType.GetAts();
		}
		if (!mem.Lookup(cType, callingObject, context, GetName(name), arity, (MemLookFlags)(((!allowSpecialNames) ? 256 : 0) | ((name == "$Item$") ? 4 : 0) | ((name == ".ctor") ? 2 : 0) | (requireInvocable ? 536870912 : 0))))
		{
			return null;
		}
		return mem.SwtFirst();
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void AddParameterConversions(MethodBase method)
	{
		ParameterInfo[] parameters = method.GetParameters();
		for (int i = 0; i < parameters.Length; i++)
		{
			AddConversionsForType(parameters[i].ParameterType);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void AddNamesOnType(NameHashKey key)
	{
		List<Type> inheritance = CreateInheritanceHierarchyList(key.Type);
		AddNamesInInheritanceHierarchy(key.Name, inheritance);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void AddNamesInInheritanceHierarchy(string name, List<Type> inheritance)
	{
		for (int num = inheritance.Count - 1; num >= 0; num--)
		{
			Type type = inheritance[num];
			if (type.IsGenericType)
			{
				type = type.GetGenericTypeDefinition();
			}
			if (s_namesLoadedForEachType.Add(new NameHashKey(type, name)))
			{
				IEnumerator<MemberInfo> enumerator = (from member in type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					where member.DeclaringType == type && member.Name == name
					select member).GetEnumerator();
				if (enumerator.MoveNext())
				{
					List<EventInfo> list = null;
					if (GetCTypeFromType(type) is AggregateType { OwningAggregate: var owningAggregate })
					{
						FieldSymbol addedField = null;
						do
						{
							MemberInfo current = enumerator.Current;
							if (current is MethodInfo methodInfo)
							{
								AddMethodToSymbolTable(methodInfo, owningAggregate, current.Name switch
								{
									"Invoke" => MethodKindEnum.Invoke, 
									"op_Implicit" => MethodKindEnum.ImplicitConv, 
									"op_Explicit" => MethodKindEnum.ExplicitConv, 
									_ => MethodKindEnum.Actual, 
								});
								AddParameterConversions(methodInfo);
							}
							else if (current is ConstructorInfo constructorInfo)
							{
								AddMethodToSymbolTable(constructorInfo, owningAggregate, MethodKindEnum.Constructor);
								AddParameterConversions(constructorInfo);
							}
							else if (current is PropertyInfo property)
							{
								AddPropertyToSymbolTable(property, owningAggregate);
							}
							else if (current is FieldInfo fieldInfo)
							{
								addedField = AddFieldToSymbolTable(fieldInfo, owningAggregate);
							}
							else if (current is EventInfo item)
							{
								(list ?? (list = new List<EventInfo>())).Add(item);
							}
						}
						while (enumerator.MoveNext());
						if (list != null)
						{
							foreach (EventInfo item2 in list)
							{
								AddEventToSymbolTable(item2, owningAggregate, addedField);
							}
						}
					}
				}
			}
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static List<Type> CreateInheritanceHierarchyList(Type type)
	{
		List<Type> list;
		if (type.IsInterface)
		{
			list = new List<Type>(type.GetInterfaces().Length + 2) { type };
			Type[] interfaces = type.GetInterfaces();
			foreach (Type type2 in interfaces)
			{
				LoadSymbolsFromType(type2);
				list.Add(type2);
			}
			Type typeFromHandle = typeof(object);
			LoadSymbolsFromType(typeFromHandle);
			list.Add(typeFromHandle);
		}
		else
		{
			list = new List<Type> { type };
			Type baseType = type.BaseType;
			while (baseType != null)
			{
				LoadSymbolsFromType(baseType);
				list.Add(baseType);
				baseType = baseType.BaseType;
			}
		}
		return list;
	}

	private static Name GetName(string p)
	{
		return NameManager.Add(p ?? "");
	}

	private static Name GetName(Type type)
	{
		string name = type.Name;
		if (type.IsGenericType)
		{
			int num = name.IndexOf('`');
			if (num >= 0)
			{
				return NameManager.Add(name, num);
			}
		}
		return NameManager.Add(name);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static TypeArray GetMethodTypeParameters(MethodInfo method, MethodSymbol parent)
	{
		if (method.IsGenericMethod)
		{
			Type[] genericArguments = method.GetGenericArguments();
			CType[] array = new CType[genericArguments.Length];
			for (int i = 0; i < genericArguments.Length; i++)
			{
				Type t = genericArguments[i];
				array[i] = LoadMethodTypeParameter(parent, t);
			}
			for (int j = 0; j < genericArguments.Length; j++)
			{
				Type type = genericArguments[j];
				((TypeParameterType)array[j]).Symbol.SetBounds(TypeArray.Allocate(GetCTypeArrayFromTypes(type.GetGenericParameterConstraints())));
			}
			return TypeArray.Allocate(array);
		}
		return TypeArray.Empty;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static TypeArray GetAggregateTypeParameters(Type type, AggregateSymbol agg)
	{
		if (type.IsGenericType)
		{
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			Type[] genericArguments = genericTypeDefinition.GetGenericArguments();
			List<CType> list = new List<CType>();
			int num = (agg.isNested() ? agg.GetOuterAgg().GetTypeVarsAll().Count : 0);
			foreach (Type type2 in genericArguments)
			{
				if (type2.GenericParameterPosition >= num)
				{
					CType cType = ((!type2.IsGenericParameter || !(type2.DeclaringType == genericTypeDefinition)) ? GetCTypeFromType(type2) : LoadClassTypeParameter(agg, type2));
					if (((TypeParameterType)cType).OwningSymbol == agg)
					{
						list.Add(cType);
					}
				}
			}
			return TypeArray.Allocate(list.ToArray());
		}
		return TypeArray.Empty;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static TypeParameterType LoadClassTypeParameter(AggregateSymbol parent, Type t)
	{
		for (AggregateSymbol aggregateSymbol = parent; aggregateSymbol != null; aggregateSymbol = aggregateSymbol.parent as AggregateSymbol)
		{
			for (TypeParameterSymbol typeParameterSymbol = SymbolStore.LookupSym(GetName(t), aggregateSymbol, symbmask_t.MASK_TypeParameterSymbol) as TypeParameterSymbol; typeParameterSymbol != null; typeParameterSymbol = typeParameterSymbol.LookupNext(symbmask_t.MASK_TypeParameterSymbol) as TypeParameterSymbol)
			{
				if (AreTypeParametersEquivalent(typeParameterSymbol.GetTypeParameterType().AssociatedSystemType, t))
				{
					return typeParameterSymbol.GetTypeParameterType();
				}
			}
		}
		return AddTypeParameterToSymbolTable(parent, null, t, bIsAggregate: true);
	}

	private static bool AreTypeParametersEquivalent(Type t1, Type t2)
	{
		if (t1 == t2)
		{
			return true;
		}
		Type originalTypeParameterType = GetOriginalTypeParameterType(t1);
		Type originalTypeParameterType2 = GetOriginalTypeParameterType(t2);
		return originalTypeParameterType == originalTypeParameterType2;
	}

	private static Type GetOriginalTypeParameterType(Type t)
	{
		int genericParameterPosition = t.GenericParameterPosition;
		Type type = t.DeclaringType;
		if (type != null && type.IsGenericType)
		{
			type = type.GetGenericTypeDefinition();
		}
		if (t.DeclaringMethod != null && (type.GetGenericArguments() == null || genericParameterPosition >= type.GetGenericArguments().Length))
		{
			return t;
		}
		while (type.GetGenericArguments().Length > genericParameterPosition)
		{
			Type type2 = type.DeclaringType;
			if (type2 != null && type2.IsGenericType)
			{
				type2 = type2.GetGenericTypeDefinition();
			}
			if ((object)type2 == null || !(type2.GetGenericArguments()?.Length > genericParameterPosition))
			{
				break;
			}
			type = type2;
		}
		return type.GetGenericArguments()[genericParameterPosition];
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static TypeParameterType LoadMethodTypeParameter(MethodSymbol parent, Type t)
	{
		for (Symbol symbol = parent.firstChild; symbol != null; symbol = symbol.nextChild)
		{
			if (symbol is TypeParameterSymbol typeParameterSymbol)
			{
				TypeParameterType typeParameterType = typeParameterSymbol.GetTypeParameterType();
				if (AreTypeParametersEquivalent(typeParameterType.AssociatedSystemType, t))
				{
					return typeParameterType;
				}
			}
		}
		return AddTypeParameterToSymbolTable(null, parent, t, bIsAggregate: false);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static TypeParameterType AddTypeParameterToSymbolTable(AggregateSymbol agg, MethodSymbol meth, Type t, bool bIsAggregate)
	{
		TypeParameterSymbol typeParameterSymbol = ((!bIsAggregate) ? SymFactory.CreateMethodTypeParameter(GetName(t), meth, t.GenericParameterPosition, t.GenericParameterPosition) : SymFactory.CreateClassTypeParameter(GetName(t), agg, t.GenericParameterPosition, t.GenericParameterPosition));
		if ((t.GenericParameterAttributes & GenericParameterAttributes.Covariant) != GenericParameterAttributes.None)
		{
			typeParameterSymbol.Covariant = true;
		}
		if ((t.GenericParameterAttributes & GenericParameterAttributes.Contravariant) != GenericParameterAttributes.None)
		{
			typeParameterSymbol.Contravariant = true;
		}
		SpecCons specCons = SpecCons.None;
		if ((t.GenericParameterAttributes & GenericParameterAttributes.DefaultConstructorConstraint) != GenericParameterAttributes.None)
		{
			specCons |= SpecCons.New;
		}
		if ((t.GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) != GenericParameterAttributes.None)
		{
			specCons |= SpecCons.Ref;
		}
		if ((t.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) != GenericParameterAttributes.None)
		{
			specCons |= SpecCons.Val;
		}
		typeParameterSymbol.SetConstraints(specCons);
		typeParameterSymbol.SetAccess(ACCESS.ACC_PUBLIC);
		return TypeManager.GetTypeParameter(typeParameterSymbol);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static CType LoadSymbolsFromType(Type type)
	{
		List<object> list = BuildDeclarationChain(type);
		NamespaceOrAggregateSymbol namespaceOrAggregateSymbol = NamespaceSymbol.Root;
		for (int i = 0; i < list.Count; i++)
		{
			object obj = list[i];
			if (obj is Type type2)
			{
				if (type2.IsNullableType())
				{
					return TypeManager.GetNullable(GetCTypeFromType(type2.GetGenericArguments()[0]));
				}
				AggregateSymbol aggregateSymbol = FindSymForType(SymbolStore.LookupSym(GetName(type2), namespaceOrAggregateSymbol, symbmask_t.MASK_AggregateSymbol), type2);
				if (aggregateSymbol == null)
				{
					CType cType = ProcessSpecialTypeInChain(namespaceOrAggregateSymbol, type2);
					if (cType != null)
					{
						return cType;
					}
					aggregateSymbol = AddAggregateToSymbolTable(namespaceOrAggregateSymbol, type2);
				}
				if (type2 == type)
				{
					return GetConstructedType(type, aggregateSymbol);
				}
				namespaceOrAggregateSymbol = aggregateSymbol;
			}
			else
			{
				if (obj is MethodInfo methinfo)
				{
					return ProcessMethodTypeParameter(methinfo, list[++i] as Type, namespaceOrAggregateSymbol as AggregateSymbol);
				}
				namespaceOrAggregateSymbol = AddNamespaceToSymbolTable(namespaceOrAggregateSymbol, obj as string);
			}
		}
		return null;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static TypeParameterType ProcessMethodTypeParameter(MethodInfo methinfo, Type t, AggregateSymbol parent)
	{
		MethodSymbol methodSymbol = FindMatchingMethod(methinfo, parent);
		if (methodSymbol == null)
		{
			methodSymbol = AddMethodToSymbolTable(methinfo, parent, MethodKindEnum.Actual);
		}
		return LoadMethodTypeParameter(methodSymbol, t);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static CType GetConstructedType(Type type, AggregateSymbol agg)
	{
		if (type.IsGenericType)
		{
			List<CType> list = new List<CType>();
			Type[] genericArguments = type.GetGenericArguments();
			foreach (Type type2 in genericArguments)
			{
				list.Add(GetCTypeFromType(type2));
			}
			TypeArray typeArgsAll = TypeArray.Allocate(list.ToArray());
			return TypeManager.GetAggregate(agg, typeArgsAll);
		}
		return agg.getThisType();
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static CType ProcessSpecialTypeInChain(NamespaceOrAggregateSymbol parent, Type t)
	{
		if (t.IsGenericParameter)
		{
			return LoadClassTypeParameter(parent as AggregateSymbol, t);
		}
		if (t.IsArray)
		{
			return TypeManager.GetArray(GetCTypeFromType(t.GetElementType()), t.GetArrayRank(), t.GetElementType().MakeArrayType() == t);
		}
		if (t.IsPointer)
		{
			return TypeManager.GetPointer(GetCTypeFromType(t.GetElementType()));
		}
		return null;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static List<object> BuildDeclarationChain(Type callingType)
	{
		if (callingType.IsByRef)
		{
			callingType = callingType.GetElementType();
		}
		List<object> list = new List<object>();
		Type type = callingType;
		while (type != null)
		{
			list.Add(type);
			if (type.IsGenericParameter && type.DeclaringMethod != null)
			{
				MethodBase declaringMethod = type.DeclaringMethod;
				MethodInfo[] methods = type.DeclaringType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo in methods)
				{
					if (methodInfo.HasSameMetadataDefinitionAs(declaringMethod) && methodInfo.IsGenericMethod)
					{
						list.Add(methodInfo);
					}
				}
			}
			type = type.DeclaringType;
		}
		list.Reverse();
		if (callingType.Namespace != null)
		{
			list.InsertRange(0, callingType.Namespace.Split('.'));
		}
		return list;
	}

	private static AggregateSymbol FindSymForType(Symbol sym, Type t)
	{
		while (sym != null)
		{
			if (sym is AggregateSymbol aggregateSymbol && aggregateSymbol.AssociatedSystemType.IsEquivalentTo(t.IsGenericType ? t.GetGenericTypeDefinition() : t))
			{
				return aggregateSymbol;
			}
			sym = sym.nextSameName;
		}
		return null;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static NamespaceSymbol AddNamespaceToSymbolTable(NamespaceOrAggregateSymbol parent, string sz)
	{
		Name name = GetName(sz);
		return (SymbolStore.LookupSym(name, parent, symbmask_t.MASK_NamespaceSymbol) as NamespaceSymbol) ?? SymFactory.CreateNamespace(name, parent as NamespaceSymbol);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static CType[] GetCTypeArrayFromTypes(Type[] types)
	{
		int num = types.Length;
		if (num == 0)
		{
			return Array.Empty<CType>();
		}
		CType[] array = new CType[num];
		for (int i = 0; i < types.Length; i++)
		{
			Type type = types[i];
			array[i] = GetCTypeFromType(type);
		}
		return array;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static CType GetCTypeFromType(Type type)
	{
		if (!type.IsByRef)
		{
			return LoadSymbolsFromType(type);
		}
		return TypeManager.GetParameterModifier(LoadSymbolsFromType(type.GetElementType()), isOut: false);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static AggregateSymbol AddAggregateToSymbolTable(NamespaceOrAggregateSymbol parent, Type type)
	{
		AggregateSymbol aggregateSymbol = SymFactory.CreateAggregate(GetName(type), parent);
		aggregateSymbol.AssociatedSystemType = (type.IsGenericType ? type.GetGenericTypeDefinition() : type);
		aggregateSymbol.AssociatedAssembly = type.Assembly;
		AggKindEnum aggKind;
		if (type.IsInterface)
		{
			aggKind = AggKindEnum.Interface;
		}
		else if (!type.IsEnum)
		{
			aggKind = (type.IsValueType ? AggKindEnum.Struct : ((!(type.BaseType != null) || (!(type.BaseType.FullName == "System.MulticastDelegate") && !(type.BaseType.FullName == "System.Delegate")) || !(type.FullName != "System.MulticastDelegate")) ? AggKindEnum.Class : AggKindEnum.Delegate));
		}
		else
		{
			aggKind = AggKindEnum.Enum;
			aggregateSymbol.SetUnderlyingType((AggregateType)GetCTypeFromType(Enum.GetUnderlyingType(type)));
		}
		aggregateSymbol.SetAggKind(aggKind);
		aggregateSymbol.SetTypeVars(TypeArray.Empty);
		ACCESS access = (type.IsPublic ? ACCESS.ACC_PUBLIC : ((!type.IsNested) ? ACCESS.ACC_INTERNAL : (type.IsNestedAssembly ? ACCESS.ACC_INTERNAL : (type.IsNestedFamORAssem ? ACCESS.ACC_INTERNALPROTECTED : (type.IsNestedPrivate ? ACCESS.ACC_PRIVATE : (type.IsNestedFamily ? ACCESS.ACC_PROTECTED : ((!type.IsNestedFamANDAssem) ? ACCESS.ACC_PUBLIC : ACCESS.ACC_INTERNAL_AND_PROTECTED)))))));
		aggregateSymbol.SetAccess(access);
		if (!type.IsGenericParameter)
		{
			aggregateSymbol.SetTypeVars(GetAggregateTypeParameters(type, aggregateSymbol));
		}
		if (type.IsGenericType)
		{
			Type[] genericArguments = type.GetGenericTypeDefinition().GetGenericArguments();
			for (int i = 0; i < aggregateSymbol.GetTypeVars().Count; i++)
			{
				Type type2 = genericArguments[i];
				if (aggregateSymbol.GetTypeVars()[i] is TypeParameterType typeParameterType)
				{
					typeParameterType.Symbol.SetBounds(TypeArray.Allocate(GetCTypeArrayFromTypes(type2.GetGenericParameterConstraints())));
				}
			}
		}
		aggregateSymbol.SetAbstract(type.IsAbstract);
		string fullName = type.FullName;
		if (type.IsGenericType)
		{
			fullName = type.GetGenericTypeDefinition().FullName;
		}
		if (fullName != null)
		{
			PredefinedType predefinedType = PredefinedTypeFacts.TryGetPredefTypeIndex(fullName);
			if (predefinedType != PredefinedType.PT_UNDEFINEDINDEX)
			{
				PredefinedTypes.InitializePredefinedType(aggregateSymbol, predefinedType);
			}
		}
		aggregateSymbol.SetSealed(type.IsSealed);
		if (type.BaseType != null)
		{
			Type type3 = type.BaseType;
			if (type3.IsGenericType)
			{
				type3 = type3.GetGenericTypeDefinition();
			}
			aggregateSymbol.SetBaseClass((AggregateType)GetCTypeFromType(type3));
		}
		aggregateSymbol.SetFirstUDConversion(null);
		SetInterfacesOnAggregate(aggregateSymbol, type);
		aggregateSymbol.SetHasPubNoArgCtor(type.GetConstructor(Type.EmptyTypes) != null);
		if (aggregateSymbol.IsDelegate())
		{
			PopulateSymbolTableWithName(".ctor", null, type);
			PopulateSymbolTableWithName("Invoke", null, type);
		}
		return aggregateSymbol;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void SetInterfacesOnAggregate(AggregateSymbol aggregate, Type type)
	{
		if (type.IsGenericType)
		{
			type = type.GetGenericTypeDefinition();
		}
		Type[] interfaces = type.GetInterfaces();
		aggregate.SetIfaces(TypeArray.Allocate(GetCTypeArrayFromTypes(interfaces)));
		aggregate.SetIfacesAll(aggregate.GetIfaces());
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static FieldSymbol AddFieldToSymbolTable(FieldInfo fieldInfo, AggregateSymbol aggregate)
	{
		if (SymbolStore.LookupSym(GetName(fieldInfo.Name), aggregate, symbmask_t.MASK_FieldSymbol) is FieldSymbol result)
		{
			return result;
		}
		FieldSymbol fieldSymbol = SymFactory.CreateMemberVar(GetName(fieldInfo.Name), aggregate);
		fieldSymbol.AssociatedFieldInfo = fieldInfo;
		fieldSymbol.isStatic = fieldInfo.IsStatic;
		ACCESS access = (fieldInfo.IsPublic ? ACCESS.ACC_PUBLIC : (fieldInfo.IsPrivate ? ACCESS.ACC_PRIVATE : (fieldInfo.IsFamily ? ACCESS.ACC_PROTECTED : (fieldInfo.IsAssembly ? ACCESS.ACC_INTERNAL : ((!fieldInfo.IsFamilyOrAssembly) ? ACCESS.ACC_INTERNAL_AND_PROTECTED : ACCESS.ACC_INTERNALPROTECTED)))));
		fieldSymbol.SetAccess(access);
		fieldSymbol.isReadOnly = fieldInfo.IsInitOnly;
		fieldSymbol.isEvent = false;
		fieldSymbol.SetType(GetCTypeFromType(fieldInfo.FieldType));
		return fieldSymbol;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void AddEventToSymbolTable(EventInfo eventInfo, AggregateSymbol aggregate, FieldSymbol addedField)
	{
		EventSymbol eventSymbol = SymbolStore.LookupSym(GetName(eventInfo.Name), aggregate, symbmask_t.MASK_EventSymbol) as EventSymbol;
		if (eventSymbol == null)
		{
			eventSymbol = SymFactory.CreateEvent(GetName(eventInfo.Name), aggregate);
			eventSymbol.AssociatedEventInfo = eventInfo;
			ACCESS access = ACCESS.ACC_PRIVATE;
			if (eventInfo.AddMethod != null)
			{
				eventSymbol.methAdd = AddMethodToSymbolTable(eventInfo.AddMethod, aggregate, MethodKindEnum.EventAccessor);
				eventSymbol.methAdd.SetEvent(eventSymbol);
				eventSymbol.isOverride = eventSymbol.methAdd.IsOverride();
				access = eventSymbol.methAdd.GetAccess();
			}
			if (eventInfo.RemoveMethod != null)
			{
				eventSymbol.methRemove = AddMethodToSymbolTable(eventInfo.RemoveMethod, aggregate, MethodKindEnum.EventAccessor);
				eventSymbol.methRemove.SetEvent(eventSymbol);
				eventSymbol.isOverride = eventSymbol.methRemove.IsOverride();
				access = eventSymbol.methRemove.GetAccess();
			}
			eventSymbol.isStatic = false;
			eventSymbol.type = GetCTypeFromType(eventInfo.EventHandlerType);
			eventSymbol.SetAccess(access);
			CType cType = addedField?.GetType();
			if (cType != null && cType == eventSymbol.type)
			{
				addedField.isEvent = true;
			}
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static void AddPredefinedPropertyToSymbolTable(AggregateSymbol type, Name property)
	{
		PropertyInfo[] properties = type.getThisType().AssociatedSystemType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.Name == property.Text)
			{
				AddPropertyToSymbolTable(propertyInfo, type);
			}
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void AddPropertyToSymbolTable(PropertyInfo property, AggregateSymbol aggregate)
	{
		bool flag = property.GetIndexParameters().Length != 0 && property.DeclaringType?.GetCustomAttribute<DefaultMemberAttribute>()?.MemberName == property.Name;
		Name name = ((!flag) ? GetName(property.Name) : GetName("$Item$"));
		PropertySymbol propertySymbol = SymbolStore.LookupSym(name, aggregate, symbmask_t.MASK_PropertySymbol) as PropertySymbol;
		if (propertySymbol != null)
		{
			PropertySymbol propertySymbol2 = null;
			while (propertySymbol != null)
			{
				if (propertySymbol.AssociatedPropertyInfo.IsEquivalentTo(property))
				{
					return;
				}
				propertySymbol2 = propertySymbol;
				propertySymbol = propertySymbol.LookupNext(symbmask_t.MASK_PropertySymbol) as PropertySymbol;
			}
			propertySymbol = propertySymbol2;
			if (flag)
			{
				propertySymbol = null;
			}
		}
		if (propertySymbol == null)
		{
			if (flag)
			{
				propertySymbol = SymFactory.CreateIndexer(name, aggregate);
				propertySymbol.Params = CreateParameterArray(null, property.GetIndexParameters());
			}
			else
			{
				propertySymbol = SymFactory.CreateProperty(GetName(property.Name), aggregate);
				propertySymbol.Params = TypeArray.Empty;
			}
		}
		propertySymbol.AssociatedPropertyInfo = property;
		propertySymbol.isStatic = ((property.GetGetMethod(nonPublic: true) != null) ? property.GetGetMethod(nonPublic: true).IsStatic : property.GetSetMethod(nonPublic: true).IsStatic);
		propertySymbol.isParamArray = DoesMethodHaveParameterArray(property.GetIndexParameters());
		propertySymbol.swtSlot = null;
		propertySymbol.RetType = GetCTypeFromType(property.PropertyType);
		propertySymbol.isOperator = flag;
		if (property.GetMethod != null || property.SetMethod != null)
		{
			MethodInfo methodInfo = property.GetMethod ?? property.SetMethod;
			propertySymbol.isOverride = methodInfo.IsVirtual && methodInfo.IsHideBySig && methodInfo.GetBaseDefinition() != methodInfo;
			propertySymbol.isHideByName = !methodInfo.IsHideBySig;
		}
		SetParameterDataForMethProp(propertySymbol, property.GetIndexParameters());
		MethodInfo getMethod = property.GetMethod;
		MethodInfo setMethod = property.SetMethod;
		ACCESS aCCESS = ACCESS.ACC_PRIVATE;
		if (getMethod != null)
		{
			propertySymbol.GetterMethod = AddMethodToSymbolTable(getMethod, aggregate, MethodKindEnum.PropAccessor);
			if (flag || propertySymbol.GetterMethod.Params.Count == 0)
			{
				propertySymbol.GetterMethod.SetProperty(propertySymbol);
			}
			else
			{
				propertySymbol.Bogus = true;
				propertySymbol.GetterMethod.SetMethKind(MethodKindEnum.Actual);
			}
			if (propertySymbol.GetterMethod.GetAccess() > aCCESS)
			{
				aCCESS = propertySymbol.GetterMethod.GetAccess();
			}
		}
		if (setMethod != null)
		{
			propertySymbol.SetterMethod = AddMethodToSymbolTable(setMethod, aggregate, MethodKindEnum.PropAccessor);
			if (flag || propertySymbol.SetterMethod.Params.Count == 1)
			{
				propertySymbol.SetterMethod.SetProperty(propertySymbol);
			}
			else
			{
				propertySymbol.Bogus = true;
				propertySymbol.SetterMethod.SetMethKind(MethodKindEnum.Actual);
			}
			if (propertySymbol.SetterMethod.GetAccess() > aCCESS)
			{
				aCCESS = propertySymbol.SetterMethod.GetAccess();
			}
		}
		propertySymbol.SetAccess(aCCESS);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static void AddPredefinedMethodToSymbolTable(AggregateSymbol type, Name methodName)
	{
		Type associatedSystemType = type.getThisType().AssociatedSystemType;
		if (methodName == NameManager.GetPredefinedName(PredefinedName.PN_CTOR))
		{
			ConstructorInfo[] constructors = associatedSystemType.GetConstructors();
			for (int i = 0; i < constructors.Length; i++)
			{
				AddMethodToSymbolTable(constructors[i], type, MethodKindEnum.Constructor);
			}
			return;
		}
		MethodInfo[] methods = associatedSystemType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.Name == methodName.Text && methodInfo.DeclaringType == associatedSystemType)
			{
				AddMethodToSymbolTable(methodInfo, type, (methodInfo.Name == "Invoke") ? MethodKindEnum.Invoke : MethodKindEnum.Actual);
			}
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static MethodSymbol AddMethodToSymbolTable(MethodBase member, AggregateSymbol callingAggregate, MethodKindEnum kind)
	{
		MethodInfo methodInfo = member as MethodInfo;
		if (kind == MethodKindEnum.Actual && (methodInfo == null || (!methodInfo.IsStatic && methodInfo.IsSpecialName)))
		{
			return null;
		}
		MethodSymbol methodSymbol = FindMatchingMethod(member, callingAggregate);
		if (methodSymbol != null)
		{
			return methodSymbol;
		}
		ParameterInfo[] parameters = member.GetParameters();
		methodSymbol = SymFactory.CreateMethod(GetName(member.Name), callingAggregate);
		methodSymbol.AssociatedMemberInfo = member;
		methodSymbol.SetMethKind(kind);
		if (kind == MethodKindEnum.ExplicitConv || kind == MethodKindEnum.ImplicitConv)
		{
			callingAggregate.SetHasConversion();
			methodSymbol.SetConvNext(callingAggregate.GetFirstUDConversion());
			callingAggregate.SetFirstUDConversion(methodSymbol);
		}
		ACCESS access = (member.IsPublic ? ACCESS.ACC_PUBLIC : (member.IsPrivate ? ACCESS.ACC_PRIVATE : (member.IsFamily ? ACCESS.ACC_PROTECTED : (member.IsFamilyOrAssembly ? ACCESS.ACC_INTERNALPROTECTED : ((!member.IsAssembly) ? ACCESS.ACC_INTERNAL_AND_PROTECTED : ACCESS.ACC_INTERNAL)))));
		methodSymbol.SetAccess(access);
		methodSymbol.isVirtual = member.IsVirtual;
		methodSymbol.isStatic = member.IsStatic;
		if (methodInfo != null)
		{
			methodSymbol.typeVars = GetMethodTypeParameters(methodInfo, methodSymbol);
			methodSymbol.isOverride = methodInfo.IsVirtual && methodInfo.IsHideBySig && methodInfo.GetBaseDefinition() != methodInfo;
			methodSymbol.isOperator = IsOperator(methodInfo);
			methodSymbol.swtSlot = GetSlotForOverride(methodInfo);
			methodSymbol.RetType = GetCTypeFromType(methodInfo.ReturnType);
		}
		else
		{
			methodSymbol.typeVars = TypeArray.Empty;
			methodSymbol.isOverride = false;
			methodSymbol.isOperator = false;
			methodSymbol.swtSlot = null;
			methodSymbol.RetType = VoidType.Instance;
		}
		methodSymbol.modOptCount = 0u;
		methodSymbol.isParamArray = DoesMethodHaveParameterArray(parameters);
		methodSymbol.isHideByName = false;
		methodSymbol.Params = CreateParameterArray(methodSymbol.AssociatedMemberInfo, parameters);
		SetParameterDataForMethProp(methodSymbol, parameters);
		return methodSymbol;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void SetParameterDataForMethProp(MethodOrPropertySymbol methProp, ParameterInfo[] parameters)
	{
		if (parameters.Length != 0)
		{
			if (parameters[^1].GetCustomAttribute<ParamArrayAttribute>(inherit: false) != null)
			{
				methProp.isParamArray = true;
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				SetParameterAttributes(methProp, parameters, i);
				methProp.ParameterNames.Add(GetName(parameters[i].Name));
			}
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void SetParameterAttributes(MethodOrPropertySymbol methProp, ParameterInfo[] parameters, int i)
	{
		ParameterInfo parameterInfo = parameters[i];
		if ((parameterInfo.Attributes & ParameterAttributes.Optional) != ParameterAttributes.None && !parameterInfo.ParameterType.IsByRef)
		{
			methProp.SetOptionalParameter(i);
			PopulateSymbolTableWithName("Value", new Type[1] { typeof(Missing) }, typeof(Missing));
		}
		if ((parameterInfo.Attributes & ParameterAttributes.HasFieldMarshal) != ParameterAttributes.None)
		{
			MarshalAsAttribute customAttribute = parameterInfo.GetCustomAttribute<MarshalAsAttribute>(inherit: false);
			if (customAttribute != null)
			{
				methProp.SetMarshalAsParameter(i, customAttribute.Value);
			}
		}
		DateTimeConstantAttribute customAttribute2 = parameterInfo.GetCustomAttribute<DateTimeConstantAttribute>(inherit: false);
		if (customAttribute2 != null)
		{
			ConstVal cv = ConstVal.Get(((DateTime)customAttribute2.Value).Ticks);
			CType predefindType = SymbolLoader.GetPredefindType(PredefinedType.PT_DATETIME);
			methProp.SetDefaultParameterValue(i, predefindType, cv);
			return;
		}
		DecimalConstantAttribute customAttribute3 = parameterInfo.GetCustomAttribute<DecimalConstantAttribute>();
		if (customAttribute3 != null)
		{
			ConstVal cv2 = ConstVal.Get(customAttribute3.Value);
			CType predefindType2 = SymbolLoader.GetPredefindType(PredefinedType.PT_DECIMAL);
			methProp.SetDefaultParameterValue(i, predefindType2, cv2);
		}
		else
		{
			if ((parameterInfo.Attributes & ParameterAttributes.HasDefault) == 0 || parameterInfo.ParameterType.IsByRef)
			{
				return;
			}
			ConstVal cv3 = default(ConstVal);
			CType predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_OBJECT);
			if (parameterInfo.DefaultValue != null)
			{
				object defaultValue = parameterInfo.DefaultValue;
				switch (Type.GetTypeCode(defaultValue.GetType()))
				{
				case TypeCode.Byte:
					cv3 = ConstVal.Get((byte)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_BYTE);
					break;
				case TypeCode.Int16:
					cv3 = ConstVal.Get((short)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_SHORT);
					break;
				case TypeCode.Int32:
					cv3 = ConstVal.Get((int)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_INT);
					break;
				case TypeCode.Int64:
					cv3 = ConstVal.Get((long)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_LONG);
					break;
				case TypeCode.Single:
					cv3 = ConstVal.Get((float)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_FLOAT);
					break;
				case TypeCode.Double:
					cv3 = ConstVal.Get((double)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_DOUBLE);
					break;
				case TypeCode.Char:
					cv3 = ConstVal.Get((char)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_CHAR);
					break;
				case TypeCode.Boolean:
					cv3 = ConstVal.Get((bool)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_BOOL);
					break;
				case TypeCode.SByte:
					cv3 = ConstVal.Get((sbyte)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_SBYTE);
					break;
				case TypeCode.UInt16:
					cv3 = ConstVal.Get((ushort)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_USHORT);
					break;
				case TypeCode.UInt32:
					cv3 = ConstVal.Get((uint)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_UINT);
					break;
				case TypeCode.UInt64:
					cv3 = ConstVal.Get((ulong)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_ULONG);
					break;
				case TypeCode.String:
					cv3 = ConstVal.Get((string)defaultValue);
					predefindType3 = SymbolLoader.GetPredefindType(PredefinedType.PT_STRING);
					break;
				}
			}
			methProp.SetDefaultParameterValue(i, predefindType3, cv3);
		}
	}

	private static MethodSymbol FindMatchingMethod(MemberInfo method, AggregateSymbol callingAggregate)
	{
		for (MethodSymbol methodSymbol = SymbolStore.LookupSym(GetName(method.Name), callingAggregate, symbmask_t.MASK_MethodSymbol) as MethodSymbol; methodSymbol != null; methodSymbol = methodSymbol.LookupNext(symbmask_t.MASK_MethodSymbol) as MethodSymbol)
		{
			if (methodSymbol.AssociatedMemberInfo.IsEquivalentTo(method))
			{
				return methodSymbol;
			}
		}
		return null;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static TypeArray CreateParameterArray(MemberInfo associatedInfo, ParameterInfo[] parameters)
	{
		bool flag = associatedInfo is MethodBase methodBase && (methodBase.CallingConvention & CallingConventions.VarArgs) != 0;
		CType[] array = new CType[flag ? (parameters.Length + 1) : parameters.Length];
		for (int i = 0; i < parameters.Length; i++)
		{
			array[i] = GetTypeOfParameter(parameters[i], associatedInfo);
		}
		if (flag)
		{
			array[^1] = ArgumentListType.Instance;
		}
		return TypeArray.Allocate(array);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static CType GetTypeOfParameter(ParameterInfo p, MemberInfo m)
	{
		Type parameterType = p.ParameterType;
		CType cType = ((!parameterType.IsGenericParameter || !(parameterType.DeclaringMethod != null) || !(parameterType.DeclaringMethod == m)) ? GetCTypeFromType(parameterType) : LoadMethodTypeParameter(FindMethodFromMemberInfo(m), parameterType));
		if (cType is ParameterModifierType parameterModifierType && p.IsOut && !p.IsIn)
		{
			cType = TypeManager.GetParameterModifier(parameterModifierType.ParameterType, isOut: true);
		}
		return cType;
	}

	private static bool DoesMethodHaveParameterArray(ParameterInfo[] parameters)
	{
		if (parameters.Length == 0)
		{
			return false;
		}
		object[] customAttributes = parameters[^1].GetCustomAttributes(inherit: false);
		for (int i = 0; i < customAttributes.Length; i++)
		{
			if (customAttributes[i] is ParamArrayAttribute)
			{
				return true;
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static SymWithType GetSlotForOverride(MethodInfo method)
	{
		if (method.IsVirtual && method.IsHideBySig)
		{
			MethodInfo baseDefinition = method.GetBaseDefinition();
			if (baseDefinition == method)
			{
				return null;
			}
			AggregateSymbol owningAggregate = ((AggregateType)GetCTypeFromType(baseDefinition.DeclaringType)).OwningAggregate;
			return new SymWithType(FindMethodFromMemberInfo(baseDefinition), owningAggregate.getThisType());
		}
		return null;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static MethodSymbol FindMethodFromMemberInfo(MemberInfo baseMemberInfo)
	{
		AggregateSymbol owningAggregate = ((AggregateType)GetCTypeFromType(baseMemberInfo.DeclaringType)).OwningAggregate;
		MethodSymbol methodSymbol = SymbolLoader.LookupAggMember(GetName(baseMemberInfo.Name), owningAggregate, symbmask_t.MASK_MethodSymbol) as MethodSymbol;
		while (methodSymbol != null && !methodSymbol.AssociatedMemberInfo.IsEquivalentTo(baseMemberInfo))
		{
			methodSymbol = methodSymbol.LookupNext(symbmask_t.MASK_MethodSymbol) as MethodSymbol;
		}
		return methodSymbol;
	}

	internal static bool AggregateContainsMethod(AggregateSymbol agg, string szName, symbmask_t mask)
	{
		return SymbolLoader.LookupAggMember(GetName(szName), agg, mask) != null;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static void AddConversionsForType(Type type)
	{
		if (type.IsInterface)
		{
			AddConversionsForOneType(type);
		}
		Type type2 = type;
		while (type2.BaseType != null)
		{
			AddConversionsForOneType(type2);
			type2 = type2.BaseType;
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static void AddConversionsForOneType(Type type)
	{
		if (type.IsGenericType)
		{
			type = type.GetGenericTypeDefinition();
		}
		if (!s_typesWithConversionsLoaded.Add(type))
		{
			return;
		}
		CType cType = GetCTypeFromType(type);
		if (!(cType is AggregateType))
		{
			CType baseOrParameterOrElementType;
			while ((baseOrParameterOrElementType = cType.BaseOrParameterOrElementType) != null)
			{
				cType = baseOrParameterOrElementType;
			}
		}
		if (cType is TypeParameterType typeParameterType)
		{
			CType[] items = typeParameterType.Bounds.Items;
			for (int i = 0; i < items.Length; i++)
			{
				AddConversionsForType(items[i].AssociatedSystemType);
			}
			return;
		}
		AggregateSymbol owningAggregate = ((AggregateType)cType).OwningAggregate;
		MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
		foreach (MethodInfo methodInfo in methods)
		{
			if (!(methodInfo.DeclaringType == type) || !methodInfo.IsSpecialName || methodInfo.IsGenericMethod)
			{
				continue;
			}
			string name = methodInfo.Name;
			MethodKindEnum kind;
			if (!(name == "op_Implicit"))
			{
				if (!(name == "op_Explicit"))
				{
					continue;
				}
				kind = MethodKindEnum.ExplicitConv;
			}
			else
			{
				kind = MethodKindEnum.ImplicitConv;
			}
			AddMethodToSymbolTable(methodInfo, owningAggregate, kind);
		}
	}

	private static bool IsOperator(MethodInfo method)
	{
		if (method.IsSpecialName && method.IsStatic)
		{
			switch (method.Name)
			{
			case "op_Multiply":
			case "op_Explicit":
			case "op_Implicit":
			case "op_Equality":
			case "op_Addition":
			case "op_LessThan":
			case "op_Division":
			case "op_ExclusiveOr":
			case "op_GreaterThan":
			case "op_Subtraction":
			case "op_Increment":
			case "op_LeftShift":
			case "op_UnaryPlus":
			case "op_BitwiseOr":
			case "op_Decrement":
			case "op_LogicalNot":
			case "op_RightShift":
			case "op_BitwiseAnd":
			case "op_Inequality":
			case "op_Modulus":
			case "op_LessThanOrEqual":
			case "op_GreaterThanOrEqual":
			case "op_UnaryNegation":
			case "op_OnesComplement":
			case "op_True":
			case "op_False":
				return true;
			}
		}
		return false;
	}
}
