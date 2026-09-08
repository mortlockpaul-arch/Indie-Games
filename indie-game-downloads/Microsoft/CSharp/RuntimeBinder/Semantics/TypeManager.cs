using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class TypeManager
{
	private sealed class StdTypeVarColl
	{
		private readonly List<TypeParameterType> prgptvs;

		public StdTypeVarColl()
		{
			prgptvs = new List<TypeParameterType>();
		}

		public TypeParameterType GetTypeVarSym(int iv, bool fMeth)
		{
			TypeParameterType typeParameterType;
			if (iv >= prgptvs.Count)
			{
				TypeParameterSymbol typeParameterSymbol = new TypeParameterSymbol();
				typeParameterSymbol.SetIsMethodTypeParameter(fMeth);
				typeParameterSymbol.SetIndexInOwnParameters(iv);
				typeParameterSymbol.SetIndexInTotalParameters(iv);
				typeParameterSymbol.SetAccess(ACCESS.ACC_PRIVATE);
				typeParameterType = GetTypeParameter(typeParameterSymbol);
				prgptvs.Add(typeParameterType);
			}
			else
			{
				typeParameterType = prgptvs[iv];
			}
			return typeParameterType;
		}
	}

	private static readonly Dictionary<(Assembly, Assembly), bool> s_internalsVisibleToCache = new Dictionary<(Assembly, Assembly), bool>();

	private static readonly StdTypeVarColl s_stvcMethod = new StdTypeVarColl();

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ArrayType GetArray(CType elementType, int args, bool isSZArray)
	{
		int rankNum = ((!isSZArray) ? args : 0);
		ArrayType arrayType = TypeTable.LookupArray(elementType, rankNum);
		if (arrayType == null)
		{
			arrayType = new ArrayType(elementType, args, isSZArray);
			TypeTable.InsertArray(elementType, rankNum, arrayType);
		}
		return arrayType;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static AggregateType GetAggregate(AggregateSymbol agg, AggregateType atsOuter, TypeArray typeArgs)
	{
		if (typeArgs == null)
		{
			typeArgs = TypeArray.Empty;
		}
		AggregateType aggregateType = TypeTable.LookupAggregate(agg, atsOuter, typeArgs);
		if (aggregateType == null)
		{
			aggregateType = new AggregateType(agg, typeArgs, atsOuter);
			TypeTable.InsertAggregate(agg, atsOuter, typeArgs, aggregateType);
		}
		return aggregateType;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static AggregateType GetAggregate(AggregateSymbol agg, TypeArray typeArgsAll)
	{
		if (typeArgsAll.Count == 0)
		{
			return agg.getThisType();
		}
		AggregateSymbol outerAgg = agg.GetOuterAgg();
		if (outerAgg == null)
		{
			return GetAggregate(agg, null, typeArgsAll);
		}
		int count = outerAgg.GetTypeVarsAll().Count;
		TypeArray typeArgsAll2 = TypeArray.Allocate(count, typeArgsAll, 0);
		TypeArray typeArgs = TypeArray.Allocate(agg.GetTypeVars().Count, typeArgsAll, count);
		AggregateType aggregate = GetAggregate(outerAgg, typeArgsAll2);
		return GetAggregate(agg, aggregate, typeArgs);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static PointerType GetPointer(CType baseType)
	{
		PointerType pointerType = TypeTable.LookupPointer(baseType);
		if (pointerType == null)
		{
			pointerType = new PointerType(baseType);
			TypeTable.InsertPointer(baseType, pointerType);
		}
		return pointerType;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static NullableType GetNullable(CType pUnderlyingType)
	{
		NullableType nullableType = TypeTable.LookupNullable(pUnderlyingType);
		if (nullableType == null)
		{
			nullableType = new NullableType(pUnderlyingType);
			TypeTable.InsertNullable(pUnderlyingType, nullableType);
		}
		return nullableType;
	}

	public static ParameterModifierType GetParameterModifier(CType paramType, bool isOut)
	{
		ParameterModifierType parameterModifierType = TypeTable.LookupParameterModifier(paramType, isOut);
		if (parameterModifierType == null)
		{
			parameterModifierType = new ParameterModifierType(paramType, isOut);
			TypeTable.InsertParameterModifier(paramType, isOut, parameterModifierType);
		}
		return parameterModifierType;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static AggregateSymbol GetNullable()
	{
		return GetPredefAgg(PredefinedType.PT_G_OPTIONAL);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static CType SubstType(CType typeSrc, TypeArray typeArgsCls, TypeArray typeArgsMeth, bool denormMeth)
	{
		SubstContext substContext = new SubstContext(typeArgsCls, typeArgsMeth, denormMeth);
		if (!substContext.IsNop)
		{
			return SubstTypeCore(typeSrc, substContext);
		}
		return typeSrc;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static AggregateType SubstType(AggregateType typeSrc, TypeArray typeArgsCls)
	{
		SubstContext substContext = new SubstContext(typeArgsCls, null, denormMeth: false);
		if (!substContext.IsNop)
		{
			return SubstTypeCore(typeSrc, substContext);
		}
		return typeSrc;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static CType SubstType(CType typeSrc, TypeArray typeArgsCls, TypeArray typeArgsMeth)
	{
		return SubstType(typeSrc, typeArgsCls, typeArgsMeth, denormMeth: false);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static TypeArray SubstTypeArray(TypeArray taSrc, SubstContext ctx)
	{
		if (taSrc != null && taSrc.Count != 0 && ctx != null && !ctx.IsNop)
		{
			CType[] items = taSrc.Items;
			for (int i = 0; i < items.Length; i++)
			{
				CType obj = items[i];
				CType cType = SubstTypeCore(obj, ctx);
				if (obj != cType)
				{
					CType[] array = new CType[items.Length];
					Array.Copy(items, array, i);
					array[i] = cType;
					while (++i < items.Length)
					{
						array[i] = SubstTypeCore(items[i], ctx);
					}
					return TypeArray.Allocate(array);
				}
			}
		}
		return taSrc;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static TypeArray SubstTypeArray(TypeArray taSrc, TypeArray typeArgsCls, TypeArray typeArgsMeth)
	{
		if (taSrc != null && taSrc.Count != 0)
		{
			return SubstTypeArray(taSrc, new SubstContext(typeArgsCls, typeArgsMeth, denormMeth: false));
		}
		return taSrc;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static TypeArray SubstTypeArray(TypeArray taSrc, TypeArray typeArgsCls)
	{
		return SubstTypeArray(taSrc, typeArgsCls, null);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static AggregateType SubstTypeCore(AggregateType type, SubstContext ctx)
	{
		TypeArray typeArgsAll = type.TypeArgsAll;
		if (typeArgsAll.Count > 0)
		{
			TypeArray typeArray = SubstTypeArray(typeArgsAll, ctx);
			if (typeArgsAll != typeArray)
			{
				return GetAggregate(type.OwningAggregate, typeArray);
			}
		}
		return type;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static CType SubstTypeCore(CType type, SubstContext pctx)
	{
		switch (type.TypeKind)
		{
		default:
			return type;
		case TypeKind.TK_VoidType:
		case TypeKind.TK_NullType:
		case TypeKind.TK_MethodGroupType:
		case TypeKind.TK_ArgumentListType:
			return type;
		case TypeKind.TK_ParameterModifierType:
		{
			ParameterModifierType parameterModifierType = (ParameterModifierType)type;
			CType underlyingType;
			CType cType = SubstTypeCore(underlyingType = parameterModifierType.ParameterType, pctx);
			if (cType != underlyingType)
			{
				return GetParameterModifier(cType, parameterModifierType.IsOut);
			}
			return type;
		}
		case TypeKind.TK_ArrayType:
		{
			ArrayType arrayType = (ArrayType)type;
			CType underlyingType;
			CType cType = SubstTypeCore(underlyingType = arrayType.ElementType, pctx);
			if (cType != underlyingType)
			{
				return GetArray(cType, arrayType.Rank, arrayType.IsSZArray);
			}
			return type;
		}
		case TypeKind.TK_PointerType:
		{
			CType underlyingType;
			CType cType = SubstTypeCore(underlyingType = ((PointerType)type).ReferentType, pctx);
			if (cType != underlyingType)
			{
				return GetPointer(cType);
			}
			return type;
		}
		case TypeKind.TK_NullableType:
		{
			CType underlyingType;
			CType cType = SubstTypeCore(underlyingType = ((NullableType)type).UnderlyingType, pctx);
			if (cType != underlyingType)
			{
				return GetNullable(cType);
			}
			return type;
		}
		case TypeKind.TK_AggregateType:
			return SubstTypeCore((AggregateType)type, pctx);
		case TypeKind.TK_TypeParameterType:
		{
			TypeParameterSymbol symbol = ((TypeParameterType)type).Symbol;
			int indexInTotalParameters = symbol.GetIndexInTotalParameters();
			if (symbol.IsMethodTypeParameter())
			{
				if (pctx.DenormMeth && symbol.parent != null)
				{
					return type;
				}
				if (indexInTotalParameters < pctx.MethodTypes.Length)
				{
					return pctx.MethodTypes[indexInTotalParameters];
				}
				return type;
			}
			if (indexInTotalParameters >= pctx.ClassTypes.Length)
			{
				return type;
			}
			return pctx.ClassTypes[indexInTotalParameters];
		}
		}
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool SubstEqualTypes(CType typeDst, CType typeSrc, TypeArray typeArgsCls, TypeArray typeArgsMeth, bool denormMeth)
	{
		if (typeDst.Equals(typeSrc))
		{
			return true;
		}
		SubstContext substContext = new SubstContext(typeArgsCls, typeArgsMeth, denormMeth);
		if (!substContext.IsNop)
		{
			return SubstEqualTypesCore(typeDst, typeSrc, substContext);
		}
		return false;
	}

	public static bool SubstEqualTypeArrays(TypeArray taDst, TypeArray taSrc, TypeArray typeArgsCls, TypeArray typeArgsMeth)
	{
		if (taDst == taSrc || (taDst != null && taDst.Equals(taSrc)))
		{
			return true;
		}
		if (taDst.Count != taSrc.Count)
		{
			return false;
		}
		if (taDst.Count == 0)
		{
			return true;
		}
		SubstContext substContext = new SubstContext(typeArgsCls, typeArgsMeth, denormMeth: true);
		if (substContext.IsNop)
		{
			return false;
		}
		for (int i = 0; i < taDst.Count; i++)
		{
			if (!SubstEqualTypesCore(taDst[i], taSrc[i], substContext))
			{
				return false;
			}
		}
		return true;
	}

	private static bool SubstEqualTypesCore(CType typeDst, CType typeSrc, SubstContext pctx)
	{
		while (typeDst != typeSrc && !typeDst.Equals(typeSrc))
		{
			switch (typeSrc.TypeKind)
			{
			default:
				return false;
			case TypeKind.TK_VoidType:
			case TypeKind.TK_NullType:
				return false;
			case TypeKind.TK_ArrayType:
			{
				ArrayType arrayType = (ArrayType)typeSrc;
				if (!(typeDst is ArrayType arrayType2) || arrayType2.Rank != arrayType.Rank || arrayType2.IsSZArray != arrayType.IsSZArray)
				{
					return false;
				}
				break;
			}
			case TypeKind.TK_ParameterModifierType:
				if (!(typeDst is ParameterModifierType parameterModifierType) || parameterModifierType.IsOut != ((ParameterModifierType)typeSrc).IsOut)
				{
					return false;
				}
				break;
			case TypeKind.TK_PointerType:
			case TypeKind.TK_NullableType:
				if (typeDst.TypeKind != typeSrc.TypeKind)
				{
					return false;
				}
				break;
			case TypeKind.TK_AggregateType:
			{
				if (!(typeDst is AggregateType aggregateType))
				{
					return false;
				}
				AggregateType aggregateType2 = (AggregateType)typeSrc;
				if (aggregateType2.OwningAggregate != aggregateType.OwningAggregate)
				{
					return false;
				}
				for (int i = 0; i < aggregateType2.TypeArgsAll.Count; i++)
				{
					if (!SubstEqualTypesCore(aggregateType.TypeArgsAll[i], aggregateType2.TypeArgsAll[i], pctx))
					{
						return false;
					}
				}
				return true;
			}
			case TypeKind.TK_TypeParameterType:
			{
				TypeParameterSymbol symbol = ((TypeParameterType)typeSrc).Symbol;
				int indexInTotalParameters = symbol.GetIndexInTotalParameters();
				if (symbol.IsMethodTypeParameter())
				{
					if (pctx.DenormMeth && symbol.parent != null)
					{
						return false;
					}
					if (indexInTotalParameters < pctx.MethodTypes.Length)
					{
						return typeDst == pctx.MethodTypes[indexInTotalParameters];
					}
				}
				else if (indexInTotalParameters < pctx.ClassTypes.Length)
				{
					return typeDst == pctx.ClassTypes[indexInTotalParameters];
				}
				return false;
			}
			}
			typeSrc = typeSrc.BaseOrParameterOrElementType;
			typeDst = typeDst.BaseOrParameterOrElementType;
		}
		return true;
	}

	public static bool TypeContainsType(CType type, CType typeFind)
	{
		while (type != typeFind && !type.Equals(typeFind))
		{
			switch (type.TypeKind)
			{
			default:
				return false;
			case TypeKind.TK_VoidType:
			case TypeKind.TK_NullType:
				return false;
			case TypeKind.TK_ArrayType:
			case TypeKind.TK_PointerType:
			case TypeKind.TK_ParameterModifierType:
			case TypeKind.TK_NullableType:
				break;
			case TypeKind.TK_AggregateType:
			{
				AggregateType aggregateType = (AggregateType)type;
				for (int i = 0; i < aggregateType.TypeArgsAll.Count; i++)
				{
					if (TypeContainsType(aggregateType.TypeArgsAll[i], typeFind))
					{
						return true;
					}
				}
				return false;
			}
			case TypeKind.TK_TypeParameterType:
				return false;
			}
			type = type.BaseOrParameterOrElementType;
		}
		return true;
	}

	public static bool TypeContainsTyVars(CType type, TypeArray typeVars)
	{
		while (true)
		{
			switch (type.TypeKind)
			{
			default:
				return false;
			case TypeKind.TK_VoidType:
			case TypeKind.TK_NullType:
			case TypeKind.TK_MethodGroupType:
				return false;
			case TypeKind.TK_ArrayType:
			case TypeKind.TK_PointerType:
			case TypeKind.TK_ParameterModifierType:
			case TypeKind.TK_NullableType:
				break;
			case TypeKind.TK_AggregateType:
			{
				AggregateType aggregateType = (AggregateType)type;
				for (int i = 0; i < aggregateType.TypeArgsAll.Count; i++)
				{
					if (TypeContainsTyVars(aggregateType.TypeArgsAll[i], typeVars))
					{
						return true;
					}
				}
				return false;
			}
			case TypeKind.TK_TypeParameterType:
				if (typeVars != null && typeVars.Count > 0)
				{
					int indexInTotalParameters = ((TypeParameterType)type).IndexInTotalParameters;
					if (indexInTotalParameters < typeVars.Count)
					{
						return type == typeVars[indexInTotalParameters];
					}
					return false;
				}
				return true;
			}
			type = type.BaseOrParameterOrElementType;
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static AggregateSymbol GetPredefAgg(PredefinedType pt)
	{
		return PredefinedTypes.GetPredefinedAggregate(pt);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static AggregateType SubstType(AggregateType typeSrc, SubstContext ctx)
	{
		if (ctx != null && !ctx.IsNop)
		{
			return SubstTypeCore(typeSrc, ctx);
		}
		return typeSrc;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CType SubstType(CType typeSrc, SubstContext pctx)
	{
		if (pctx != null && !pctx.IsNop)
		{
			return SubstTypeCore(typeSrc, pctx);
		}
		return typeSrc;
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CType SubstType(CType typeSrc, AggregateType atsCls)
	{
		return SubstType(typeSrc, atsCls, null);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CType SubstType(CType typeSrc, AggregateType atsCls, TypeArray typeArgsMeth)
	{
		return SubstType(typeSrc, atsCls?.TypeArgsAll, typeArgsMeth);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static CType SubstType(CType typeSrc, CType typeCls, TypeArray typeArgsMeth)
	{
		return SubstType(typeSrc, (typeCls as AggregateType)?.TypeArgsAll, typeArgsMeth);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static TypeArray SubstTypeArray(TypeArray taSrc, AggregateType atsCls, TypeArray typeArgsMeth)
	{
		return SubstTypeArray(taSrc, atsCls?.TypeArgsAll, typeArgsMeth);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static TypeArray SubstTypeArray(TypeArray taSrc, AggregateType atsCls)
	{
		return SubstTypeArray(taSrc, atsCls, null);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool SubstEqualTypes(CType typeDst, CType typeSrc, CType typeCls, TypeArray typeArgsMeth)
	{
		return SubstEqualTypes(typeDst, typeSrc, (typeCls as AggregateType)?.TypeArgsAll, typeArgsMeth, denormMeth: false);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool SubstEqualTypes(CType typeDst, CType typeSrc, CType typeCls)
	{
		return SubstEqualTypes(typeDst, typeSrc, typeCls, null);
	}

	public static TypeParameterType GetStdMethTypeVar(int iv)
	{
		return s_stvcMethod.GetTypeVarSym(iv, fMeth: true);
	}

	public static TypeParameterType GetTypeParameter(TypeParameterSymbol pSymbol)
	{
		return new TypeParameterType(pSymbol);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static CType GetBestAccessibleType(AggregateSymbol context, CType typeSrc)
	{
		if (CSemanticChecker.CheckTypeAccess(typeSrc, context))
		{
			return typeSrc;
		}
		AggregateType aggregateType = typeSrc as AggregateType;
		if (aggregateType != null)
		{
			AggregateType baseClass;
			while (true)
			{
				if ((aggregateType.IsInterfaceType || aggregateType.IsDelegateType) && TryVarianceAdjustmentToGetAccessibleType(context, aggregateType, out var typeDst))
				{
					return typeDst;
				}
				baseClass = aggregateType.BaseClass;
				if (baseClass == null)
				{
					return GetPredefAgg(PredefinedType.PT_OBJECT).getThisType();
				}
				if (CSemanticChecker.CheckTypeAccess(baseClass, context))
				{
					break;
				}
				aggregateType = baseClass;
			}
			return baseClass;
		}
		if (typeSrc is ArrayType typeSrc2)
		{
			if (TryArrayVarianceAdjustmentToGetAccessibleType(context, typeSrc2, out var typeDst2))
			{
				return typeDst2;
			}
			return GetPredefAgg(PredefinedType.PT_ARRAY).getThisType();
		}
		return GetPredefAgg(PredefinedType.PT_VALUE).getThisType();
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool TryVarianceAdjustmentToGetAccessibleType(AggregateSymbol context, AggregateType typeSrc, out CType typeDst)
	{
		typeDst = null;
		AggregateSymbol owningAggregate = typeSrc.OwningAggregate;
		AggregateType thisType = owningAggregate.getThisType();
		if (!CSemanticChecker.CheckTypeAccess(thisType, context))
		{
			return false;
		}
		TypeArray typeArgsThis = typeSrc.TypeArgsThis;
		TypeArray typeArgsThis2 = thisType.TypeArgsThis;
		CType[] array = new CType[typeArgsThis.Count];
		for (int i = 0; i < array.Length; i++)
		{
			CType cType = typeArgsThis[i];
			if (CSemanticChecker.CheckTypeAccess(cType, context))
			{
				array[i] = cType;
				continue;
			}
			if (!cType.IsReferenceType || !((TypeParameterType)typeArgsThis2[i]).Covariant)
			{
				return false;
			}
			array[i] = GetBestAccessibleType(context, cType);
		}
		TypeArray typeArgs = TypeArray.Allocate(array);
		CType aggregate = GetAggregate(owningAggregate, typeSrc.OuterType, typeArgs);
		if (!TypeBind.CheckConstraints(aggregate, CheckConstraintsFlags.NoErrors))
		{
			return false;
		}
		typeDst = aggregate;
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool TryArrayVarianceAdjustmentToGetAccessibleType(AggregateSymbol context, ArrayType typeSrc, out CType typeDst)
	{
		CType elementType = typeSrc.ElementType;
		if (elementType.IsReferenceType)
		{
			CType bestAccessibleType = GetBestAccessibleType(context, elementType);
			typeDst = GetArray(bestAccessibleType, typeSrc.Rank, typeSrc.IsSZArray);
			return true;
		}
		typeDst = null;
		return false;
	}

	internal static bool InternalsVisibleTo(Assembly assemblyThatDefinesAttribute, Assembly assemblyToCheck)
	{
		(Assembly, Assembly) key = (assemblyThatDefinesAttribute, assemblyToCheck);
		if (!s_internalsVisibleToCache.TryGetValue(key, out var value))
		{
			try
			{
				AssemblyName name = assemblyToCheck.GetName();
				foreach (Attribute customAttribute in assemblyThatDefinesAttribute.GetCustomAttributes())
				{
					if (customAttribute is InternalsVisibleToAttribute internalsVisibleToAttribute && AssemblyName.ReferenceMatchesDefinition(new AssemblyName(internalsVisibleToAttribute.AssemblyName), name))
					{
						value = true;
						break;
					}
				}
			}
			catch (SecurityException)
			{
				value = false;
			}
			s_internalsVisibleToCache[key] = value;
		}
		return value;
	}
}
