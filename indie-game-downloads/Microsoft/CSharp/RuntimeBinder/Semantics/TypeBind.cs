using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Errors;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class TypeBind
{
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool CheckConstraints(CType type, CheckConstraintsFlags flags)
	{
		type = type.GetNakedType(fStripNub: false);
		AggregateType aggregateType = type as AggregateType;
		if (aggregateType == null)
		{
			if (!(type is NullableType nullableType))
			{
				return true;
			}
			aggregateType = nullableType.GetAts();
		}
		if (aggregateType.TypeArgsAll.Count == 0)
		{
			aggregateType.ConstraintError = false;
			return true;
		}
		if (aggregateType.ConstraintError.HasValue)
		{
			if (aggregateType.ConstraintError != true)
			{
				return true;
			}
			if ((flags & CheckConstraintsFlags.NoErrors) != CheckConstraintsFlags.None)
			{
				return false;
			}
		}
		TypeArray typeVars = aggregateType.OwningAggregate.GetTypeVars();
		TypeArray typeArgsThis = aggregateType.TypeArgsThis;
		TypeArray typeArgsAll = aggregateType.TypeArgsAll;
		if (aggregateType.OuterType != null && ((flags & CheckConstraintsFlags.Outer) != CheckConstraintsFlags.None || !aggregateType.OuterType.ConstraintError.HasValue) && !CheckConstraints(aggregateType.OuterType, flags))
		{
			aggregateType.ConstraintError = true;
			return false;
		}
		if (typeVars.Count > 0 && !CheckConstraintsCore(aggregateType.OwningAggregate, typeVars, typeArgsThis, typeArgsAll, null, flags & CheckConstraintsFlags.NoErrors))
		{
			aggregateType.ConstraintError = true;
			return false;
		}
		for (int i = 0; i < typeArgsThis.Count; i++)
		{
			if (typeArgsThis[i].GetNakedType(fStripNub: true) is AggregateType aggregateType2 && !aggregateType2.ConstraintError.HasValue)
			{
				CheckConstraints(aggregateType2, flags | CheckConstraintsFlags.Outer);
				if (aggregateType2.ConstraintError == true)
				{
					aggregateType.ConstraintError = true;
					return false;
				}
			}
		}
		aggregateType.ConstraintError = false;
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static void CheckMethConstraints(MethWithInst mwi)
	{
		if (mwi.TypeArgs.Count > 0)
		{
			CheckConstraintsCore(mwi.Meth(), mwi.Meth().typeVars, mwi.TypeArgs, mwi.GetType().TypeArgsAll, mwi.TypeArgs, CheckConstraintsFlags.None);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool CheckConstraintsCore(Symbol symErr, TypeArray typeVars, TypeArray typeArgs, TypeArray typeArgsCls, TypeArray typeArgsMeth, CheckConstraintsFlags flags)
	{
		for (int i = 0; i < typeVars.Count; i++)
		{
			TypeParameterType var = (TypeParameterType)typeVars[i];
			CType arg = typeArgs[i];
			if (!CheckSingleConstraint(symErr, var, arg, typeArgsCls, typeArgsMeth, flags))
			{
				return false;
			}
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool CheckSingleConstraint(Symbol symErr, TypeParameterType var, CType arg, TypeArray typeArgsCls, TypeArray typeArgsMeth, CheckConstraintsFlags flags)
	{
		bool flag = (flags & CheckConstraintsFlags.NoErrors) == 0;
		if (var.HasRefConstraint && !arg.IsReferenceType)
		{
			if (flag)
			{
				throw ErrorHandling.Error(ErrorCode.ERR_RefConstraintNotSatisfied, symErr, new ErrArgNoRef(var), arg);
			}
			return false;
		}
		TypeArray typeArray = TypeManager.SubstTypeArray(var.Bounds, typeArgsCls, typeArgsMeth);
		int num = 0;
		if (var.HasValConstraint)
		{
			if (!arg.IsNonNullableValueType)
			{
				if (flag)
				{
					throw ErrorHandling.Error(ErrorCode.ERR_ValConstraintNotSatisfied, symErr, new ErrArgNoRef(var), arg);
				}
				return false;
			}
			if (typeArray.Count != 0 && typeArray[0].IsPredefType(PredefinedType.PT_VALUE))
			{
				num = 1;
			}
		}
		for (int i = num; i < typeArray.Count; i++)
		{
			CType cType = typeArray[i];
			if (!SatisfiesBound(arg, cType))
			{
				if (flag)
				{
					ErrorCode id = (arg.IsReferenceType ? ErrorCode.ERR_GenericConstraintNotSatisfiedRefType : ((!(arg is NullableType nullableType) || !SymbolLoader.HasBaseConversion(nullableType.UnderlyingType, cType)) ? ErrorCode.ERR_GenericConstraintNotSatisfiedValType : ((!cType.IsPredefType(PredefinedType.PT_ENUM) && nullableType.UnderlyingType != cType) ? ErrorCode.ERR_GenericConstraintNotSatisfiedNullableInterface : ErrorCode.ERR_GenericConstraintNotSatisfiedNullableEnum)));
					throw ErrorHandling.Error(id, new ErrArg(symErr), new ErrArg(cType, ErrArgFlags.Unique), var, new ErrArg(arg, ErrArgFlags.Unique));
				}
				return false;
			}
		}
		if (!var.HasNewConstraint || arg.IsValueType)
		{
			return true;
		}
		if (arg.IsClassType)
		{
			AggregateSymbol owningAggregate = ((AggregateType)arg).OwningAggregate;
			SymbolLoader.LookupAggMember(NameManager.GetPredefinedName(PredefinedName.PN_CTOR), owningAggregate, symbmask_t.MASK_ALL);
			if (owningAggregate.HasPubNoArgCtor() && !owningAggregate.IsAbstract())
			{
				return true;
			}
		}
		if (flag)
		{
			throw ErrorHandling.Error(ErrorCode.ERR_NewConstraintNotSatisfied, symErr, new ErrArgNoRef(var), arg);
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool SatisfiesBound(CType arg, CType typeBnd)
	{
		if (typeBnd == arg)
		{
			return true;
		}
		switch (typeBnd.TypeKind)
		{
		default:
			return false;
		case TypeKind.TK_VoidType:
		case TypeKind.TK_PointerType:
			return false;
		case TypeKind.TK_NullableType:
			typeBnd = ((NullableType)typeBnd).GetAts();
			break;
		case TypeKind.TK_AggregateType:
		case TypeKind.TK_ArrayType:
		case TypeKind.TK_TypeParameterType:
			break;
		}
		switch (arg.TypeKind)
		{
		default:
			return false;
		case TypeKind.TK_PointerType:
			return false;
		case TypeKind.TK_NullableType:
			arg = ((NullableType)arg).GetAts();
			break;
		case TypeKind.TK_AggregateType:
		case TypeKind.TK_ArrayType:
		case TypeKind.TK_TypeParameterType:
			break;
		}
		return SymbolLoader.HasBaseConversion(arg, typeBnd);
	}
}
