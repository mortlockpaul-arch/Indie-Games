using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class CConversions
{
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool FImpRefConv(CType typeSrc, CType typeDst)
	{
		if (typeSrc.IsReferenceType)
		{
			return SymbolLoader.HasIdentityOrImplicitReferenceConversion(typeSrc, typeDst);
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool FExpRefConv(CType typeSrc, CType typeDst)
	{
		if (typeSrc.IsReferenceType && typeDst.IsReferenceType)
		{
			if (SymbolLoader.HasIdentityOrImplicitReferenceConversion(typeSrc, typeDst) || SymbolLoader.HasIdentityOrImplicitReferenceConversion(typeDst, typeSrc))
			{
				return true;
			}
			if ((typeSrc.IsInterfaceType && typeDst is TypeParameterType) || (typeSrc is TypeParameterType && typeDst.IsInterfaceType))
			{
				return true;
			}
			if (typeSrc is AggregateType aggregateType && typeDst is AggregateType aggregateType2)
			{
				AggregateSymbol owningAggregate = aggregateType.OwningAggregate;
				AggregateSymbol owningAggregate2 = aggregateType2.OwningAggregate;
				if ((owningAggregate.IsClass() && !owningAggregate.IsSealed() && owningAggregate2.IsInterface()) || (owningAggregate.IsInterface() && owningAggregate2.IsClass() && !owningAggregate2.IsSealed()) || (owningAggregate.IsInterface() && owningAggregate2.IsInterface()))
				{
					return true;
				}
			}
			if (typeSrc is ArrayType arrayType)
			{
				if (typeDst is ArrayType arrayType2)
				{
					if (arrayType.Rank == arrayType2.Rank && arrayType.IsSZArray == arrayType2.IsSZArray)
					{
						return FExpRefConv(arrayType.ElementType, arrayType2.ElementType);
					}
					return false;
				}
				if (!arrayType.IsSZArray || !typeDst.IsInterfaceType)
				{
					return false;
				}
				AggregateType aggregateType3 = (AggregateType)typeDst;
				TypeArray typeArgsAll = aggregateType3.TypeArgsAll;
				if (typeArgsAll.Count != 1)
				{
					return false;
				}
				AggregateSymbol predefAgg = SymbolLoader.GetPredefAgg(PredefinedType.PT_G_ILIST);
				AggregateSymbol predefAgg2 = SymbolLoader.GetPredefAgg(PredefinedType.PT_G_IREADONLYLIST);
				if ((predefAgg == null || !SymbolLoader.IsBaseAggregate(predefAgg, aggregateType3.OwningAggregate)) && (predefAgg2 == null || !SymbolLoader.IsBaseAggregate(predefAgg2, aggregateType3.OwningAggregate)))
				{
					return false;
				}
				return FExpRefConv(arrayType.ElementType, typeArgsAll[0]);
			}
			if (typeDst is ArrayType arrayType3 && typeSrc is AggregateType aggregateType4)
			{
				if (SymbolLoader.HasIdentityOrImplicitReferenceConversion(SymbolLoader.GetPredefindType(PredefinedType.PT_ARRAY), typeSrc))
				{
					return true;
				}
				if (!arrayType3.IsSZArray || !typeSrc.IsInterfaceType || aggregateType4.TypeArgsAll.Count != 1)
				{
					return false;
				}
				AggregateSymbol predefAgg3 = SymbolLoader.GetPredefAgg(PredefinedType.PT_G_ILIST);
				AggregateSymbol predefAgg4 = SymbolLoader.GetPredefAgg(PredefinedType.PT_G_IREADONLYLIST);
				if ((predefAgg3 == null || !SymbolLoader.IsBaseAggregate(predefAgg3, aggregateType4.OwningAggregate)) && (predefAgg4 == null || !SymbolLoader.IsBaseAggregate(predefAgg4, aggregateType4.OwningAggregate)))
				{
					return false;
				}
				CType elementType = arrayType3.ElementType;
				CType cType = aggregateType4.TypeArgsAll[0];
				if (elementType != cType)
				{
					return FExpRefConv(elementType, cType);
				}
				return true;
			}
			if (HasGenericDelegateExplicitReferenceConversion(typeSrc, typeDst))
			{
				return true;
			}
		}
		else
		{
			if (typeSrc.IsReferenceType)
			{
				return SymbolLoader.HasIdentityOrImplicitReferenceConversion(typeSrc, typeDst);
			}
			if (typeDst.IsReferenceType)
			{
				return SymbolLoader.HasIdentityOrImplicitReferenceConversion(typeDst, typeSrc);
			}
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool HasGenericDelegateExplicitReferenceConversion(CType source, CType target)
	{
		if (target is AggregateType pTarget)
		{
			return HasGenericDelegateExplicitReferenceConversion(source, pTarget);
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool HasGenericDelegateExplicitReferenceConversion(CType pSource, AggregateType pTarget)
	{
		if (!(pSource is AggregateType { IsDelegateType: not false } aggregateType) || !pTarget.IsDelegateType || aggregateType.OwningAggregate != pTarget.OwningAggregate || SymbolLoader.HasIdentityOrImplicitReferenceConversion(aggregateType, pTarget))
		{
			return false;
		}
		TypeArray typeVarsAll = aggregateType.OwningAggregate.GetTypeVarsAll();
		TypeArray typeArgsAll = aggregateType.TypeArgsAll;
		TypeArray typeArgsAll2 = pTarget.TypeArgsAll;
		for (int i = 0; i < typeVarsAll.Count; i++)
		{
			CType cType = typeArgsAll[i];
			CType cType2 = typeArgsAll2[i];
			if (cType == cType2)
			{
				continue;
			}
			TypeParameterType typeParameterType = (TypeParameterType)typeVarsAll[i];
			if (typeParameterType.Invariant)
			{
				return false;
			}
			if (typeParameterType.Covariant)
			{
				if (!FExpRefConv(cType, cType2))
				{
					return false;
				}
			}
			else if (typeParameterType.Contravariant && (!cType.IsReferenceType || !cType2.IsReferenceType))
			{
				return false;
			}
		}
		return true;
	}

	public static bool FWrappingConv(CType typeSrc, CType typeDst)
	{
		if (typeDst is NullableType nullableType)
		{
			return typeSrc == nullableType.UnderlyingType;
		}
		return false;
	}

	public static bool FUnwrappingConv(CType typeSrc, CType typeDst)
	{
		return FWrappingConv(typeDst, typeSrc);
	}
}
