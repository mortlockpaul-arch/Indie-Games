using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class SymbolLoader
{
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static AggregateSymbol GetPredefAgg(PredefinedType pt)
	{
		return TypeManager.GetPredefAgg(pt);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static AggregateType GetPredefindType(PredefinedType pt)
	{
		return GetPredefAgg(pt).getThisType();
	}

	public static Symbol LookupAggMember(Name name, AggregateSymbol agg, symbmask_t mask)
	{
		return SymbolStore.LookupSym(name, agg, mask);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool IsBaseInterface(AggregateType atsDer, AggregateType pBase)
	{
		if (pBase.IsInterfaceType)
		{
			while (atsDer != null)
			{
				CType[] items = atsDer.IfacesAll.Items;
				for (int i = 0; i < items.Length; i++)
				{
					if (AreTypesEqualForConversion(items[i], pBase))
					{
						return true;
					}
				}
				atsDer = atsDer.BaseClass;
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool IsBaseClassOfClass(CType pDerived, CType pBase)
	{
		if (pDerived.IsClassType)
		{
			return IsBaseClass(pDerived, pBase);
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool IsBaseClass(CType pDerived, CType pBase)
	{
		if (!(pBase is AggregateType { IsClassType: not false } aggregateType))
		{
			return false;
		}
		AggregateType aggregateType2 = pDerived as AggregateType;
		if (aggregateType2 == null)
		{
			if (!(pDerived is NullableType nullableType))
			{
				return false;
			}
			aggregateType2 = nullableType.GetAts();
		}
		for (AggregateType baseClass = aggregateType2.BaseClass; baseClass != null; baseClass = baseClass.BaseClass)
		{
			if (baseClass == aggregateType)
			{
				return true;
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool HasCovariantArrayConversion(ArrayType pSource, ArrayType pDest)
	{
		if (pSource.Rank == pDest.Rank && pSource.IsSZArray == pDest.IsSZArray)
		{
			return HasImplicitReferenceConversion(pSource.ElementType, pDest.ElementType);
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool HasIdentityOrImplicitReferenceConversion(CType pSource, CType pDest)
	{
		if (AreTypesEqualForConversion(pSource, pDest))
		{
			return true;
		}
		return HasImplicitReferenceConversion(pSource, pDest);
	}

	private static bool AreTypesEqualForConversion(CType pType1, CType pType2)
	{
		return pType1.Equals(pType2);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool HasArrayConversionToInterface(ArrayType pSource, CType pDest)
	{
		if (!pSource.IsSZArray || !pDest.IsInterfaceType)
		{
			return false;
		}
		if (pDest.IsPredefType(PredefinedType.PT_IENUMERABLE))
		{
			return true;
		}
		AggregateType aggregateType = (AggregateType)pDest;
		AggregateSymbol owningAggregate = aggregateType.OwningAggregate;
		if (!owningAggregate.isPredefAgg(PredefinedType.PT_G_ILIST) && !owningAggregate.isPredefAgg(PredefinedType.PT_G_ICOLLECTION) && !owningAggregate.isPredefAgg(PredefinedType.PT_G_IENUMERABLE) && !owningAggregate.isPredefAgg(PredefinedType.PT_G_IREADONLYCOLLECTION) && !owningAggregate.isPredefAgg(PredefinedType.PT_G_IREADONLYLIST))
		{
			return false;
		}
		return HasIdentityOrImplicitReferenceConversion(pSource.ElementType, aggregateType.TypeArgsAll[0]);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool HasImplicitReferenceConversion(CType pSource, CType pDest)
	{
		if (pSource.IsReferenceType && pDest.IsPredefType(PredefinedType.PT_OBJECT))
		{
			return true;
		}
		if (pSource is AggregateType aggregateType)
		{
			if (pDest is AggregateType aggregateType2)
			{
				switch (aggregateType.OwningAggregate.AggKind())
				{
				case AggKindEnum.Class:
					switch (aggregateType2.OwningAggregate.AggKind())
					{
					case AggKindEnum.Class:
						return IsBaseClass(aggregateType, aggregateType2);
					case AggKindEnum.Interface:
						return HasAnyBaseInterfaceConversion(aggregateType, aggregateType2);
					}
					break;
				case AggKindEnum.Interface:
					if (aggregateType2.IsInterfaceType)
					{
						if (!HasAnyBaseInterfaceConversion(aggregateType, aggregateType2))
						{
							return HasInterfaceConversion(aggregateType, aggregateType2);
						}
						return true;
					}
					break;
				case AggKindEnum.Delegate:
					if (aggregateType2.IsPredefType(PredefinedType.PT_MULTIDEL) || aggregateType2.IsPredefType(PredefinedType.PT_DELEGATE) || IsBaseInterface(GetPredefindType(PredefinedType.PT_MULTIDEL), aggregateType2))
					{
						return true;
					}
					if (pDest.IsDelegateType)
					{
						return HasDelegateConversion(aggregateType, aggregateType2);
					}
					return false;
				}
			}
		}
		else if (pSource is ArrayType pSource2)
		{
			if (pDest is ArrayType pDest2)
			{
				return HasCovariantArrayConversion(pSource2, pDest2);
			}
			if (pDest is AggregateType aggregateType3)
			{
				if (aggregateType3.IsPredefType(PredefinedType.PT_ARRAY) || IsBaseInterface(GetPredefindType(PredefinedType.PT_ARRAY), aggregateType3))
				{
					return true;
				}
				return HasArrayConversionToInterface(pSource2, pDest);
			}
		}
		else if (pSource is NullType)
		{
			if (!pDest.IsReferenceType)
			{
				return pDest is NullableType;
			}
			return true;
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool HasAnyBaseInterfaceConversion(CType pDerived, CType pBase)
	{
		if (pBase.IsInterfaceType)
		{
			AggregateType aggregateType = pDerived as AggregateType;
			if (aggregateType != null)
			{
				AggregateType pDest = (AggregateType)pBase;
				while (aggregateType != null)
				{
					CType[] items = aggregateType.IfacesAll.Items;
					for (int i = 0; i < items.Length; i++)
					{
						if (HasInterfaceConversion((AggregateType)items[i], pDest))
						{
							return true;
						}
					}
					aggregateType = aggregateType.BaseClass;
				}
				return false;
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool HasInterfaceConversion(AggregateType pSource, AggregateType pDest)
	{
		return HasVariantConversion(pSource, pDest);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool HasDelegateConversion(AggregateType pSource, AggregateType pDest)
	{
		return HasVariantConversion(pSource, pDest);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool HasVariantConversion(AggregateType pSource, AggregateType pDest)
	{
		if (pSource == pDest)
		{
			return true;
		}
		AggregateSymbol owningAggregate = pSource.OwningAggregate;
		if (owningAggregate != pDest.OwningAggregate)
		{
			return false;
		}
		TypeArray typeVarsAll = owningAggregate.GetTypeVarsAll();
		TypeArray typeArgsAll = pSource.TypeArgsAll;
		TypeArray typeArgsAll2 = pDest.TypeArgsAll;
		for (int i = 0; i < typeVarsAll.Count; i++)
		{
			CType cType = typeArgsAll[i];
			CType cType2 = typeArgsAll2[i];
			if (cType != cType2)
			{
				TypeParameterType typeParameterType = (TypeParameterType)typeVarsAll[i];
				if (typeParameterType.Invariant)
				{
					return false;
				}
				if (typeParameterType.Covariant && !HasImplicitReferenceConversion(cType, cType2))
				{
					return false;
				}
				if (typeParameterType.Contravariant && !HasImplicitReferenceConversion(cType2, cType))
				{
					return false;
				}
			}
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool HasImplicitBoxingConversion(CType pSource, CType pDest)
	{
		if (!pDest.IsReferenceType)
		{
			return false;
		}
		if (pSource is NullableType nullableType)
		{
			pSource = nullableType.UnderlyingType;
		}
		else if (!pSource.IsValueType)
		{
			return false;
		}
		if (!IsBaseClass(pSource, pDest))
		{
			return HasAnyBaseInterfaceConversion(pSource, pDest);
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool HasBaseConversion(CType pSource, CType pDest)
	{
		if (pSource is AggregateType && pDest.IsPredefType(PredefinedType.PT_OBJECT))
		{
			return true;
		}
		if (!HasIdentityOrImplicitReferenceConversion(pSource, pDest))
		{
			return HasImplicitBoxingConversion(pSource, pDest);
		}
		return true;
	}

	public static bool IsBaseAggregate(AggregateSymbol derived, AggregateSymbol @base)
	{
		if (derived == @base)
		{
			return true;
		}
		if (@base.IsInterface())
		{
			while (derived != null)
			{
				CType[] items = derived.GetIfacesAll().Items;
				for (int i = 0; i < items.Length; i++)
				{
					if (((AggregateType)items[i]).OwningAggregate == @base)
					{
						return true;
					}
				}
				derived = derived.GetBaseAgg();
			}
			return false;
		}
		while (derived.GetBaseClass() != null)
		{
			derived = derived.GetBaseClass().OwningAggregate;
			if (derived == @base)
			{
				return true;
			}
		}
		return false;
	}
}
