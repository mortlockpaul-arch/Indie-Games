using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Errors;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class CSemanticChecker
{
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static void CheckForStaticClass(CType type)
	{
		if (type.IsStaticClass)
		{
			throw ErrorHandling.Error(ErrorCode.ERR_ConvertToStaticClass, type);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ACCESSERROR CheckAccess2(Symbol symCheck, AggregateType atsCheck, Symbol symWhere, CType typeThru)
	{
		ACCESSERROR aCCESSERROR = CheckAccessCore(symCheck, atsCheck, symWhere, typeThru);
		if (ACCESSERROR.ACCESSERROR_NOERROR != aCCESSERROR)
		{
			return aCCESSERROR;
		}
		CType cType = symCheck.getType();
		if (cType == null)
		{
			return ACCESSERROR.ACCESSERROR_NOERROR;
		}
		if (atsCheck.TypeArgsAll.Count > 0)
		{
			cType = TypeManager.SubstType(cType, atsCheck);
		}
		if (!CheckTypeAccess(cType, symWhere))
		{
			return ACCESSERROR.ACCESSERROR_NOACCESS;
		}
		return ACCESSERROR.ACCESSERROR_NOERROR;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool CheckTypeAccess(CType type, Symbol symWhere)
	{
		type = type.GetNakedType(fStripNub: true);
		AggregateType aggregateType = type as AggregateType;
		if (aggregateType == null)
		{
			return true;
		}
		do
		{
			if (ACCESSERROR.ACCESSERROR_NOERROR != CheckAccessCore(aggregateType.OwningAggregate, aggregateType.OuterType, symWhere, null))
			{
				return false;
			}
			aggregateType = aggregateType.OuterType;
		}
		while (aggregateType != null);
		TypeArray typeArgsAll = ((AggregateType)type).TypeArgsAll;
		for (int i = 0; i < typeArgsAll.Count; i++)
		{
			if (!CheckTypeAccess(typeArgsAll[i], symWhere))
			{
				return false;
			}
		}
		return true;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ACCESSERROR CheckAccessCore(Symbol symCheck, AggregateType atsCheck, Symbol symWhere, CType typeThru)
	{
		switch (symCheck.GetAccess())
		{
		default:
			throw Error.InternalCompilerError();
		case ACCESS.ACC_UNKNOWN:
			return ACCESSERROR.ACCESSERROR_NOACCESS;
		case ACCESS.ACC_PUBLIC:
			return ACCESSERROR.ACCESSERROR_NOERROR;
		case ACCESS.ACC_PRIVATE:
		case ACCESS.ACC_PROTECTED:
			if (symWhere == null)
			{
				return ACCESSERROR.ACCESSERROR_NOACCESS;
			}
			break;
		case ACCESS.ACC_INTERNAL:
		case ACCESS.ACC_INTERNALPROTECTED:
			if (symWhere == null)
			{
				return ACCESSERROR.ACCESSERROR_NOACCESS;
			}
			if (symWhere.SameAssemOrFriend(symCheck))
			{
				return ACCESSERROR.ACCESSERROR_NOERROR;
			}
			if (symCheck.GetAccess() == ACCESS.ACC_INTERNAL)
			{
				return ACCESSERROR.ACCESSERROR_NOACCESS;
			}
			break;
		case ACCESS.ACC_INTERNAL_AND_PROTECTED:
			if (symWhere == null || !symWhere.SameAssemOrFriend(symCheck))
			{
				return ACCESSERROR.ACCESSERROR_NOACCESS;
			}
			break;
		}
		AggregateSymbol aggregateSymbol = null;
		for (Symbol symbol = symWhere; symbol != null; symbol = symbol.parent)
		{
			if (symbol is AggregateSymbol aggregateSymbol2)
			{
				aggregateSymbol = aggregateSymbol2;
				break;
			}
		}
		if (aggregateSymbol == null)
		{
			return ACCESSERROR.ACCESSERROR_NOACCESS;
		}
		AggregateSymbol aggregateSymbol3 = symCheck.parent as AggregateSymbol;
		for (AggregateSymbol aggregateSymbol4 = aggregateSymbol; aggregateSymbol4 != null; aggregateSymbol4 = aggregateSymbol4.GetOuterAgg())
		{
			if (aggregateSymbol4 == aggregateSymbol3)
			{
				return ACCESSERROR.ACCESSERROR_NOERROR;
			}
		}
		if (symCheck.GetAccess() == ACCESS.ACC_PRIVATE)
		{
			return ACCESSERROR.ACCESSERROR_NOACCESS;
		}
		AggregateType aggregateType = null;
		if (typeThru != null && !symCheck.isStatic)
		{
			aggregateType = typeThru.GetAts();
		}
		bool flag = false;
		for (AggregateSymbol aggregateSymbol5 = aggregateSymbol; aggregateSymbol5 != null; aggregateSymbol5 = aggregateSymbol5.GetOuterAgg())
		{
			if (aggregateSymbol5.FindBaseAgg(aggregateSymbol3))
			{
				flag = true;
				if (aggregateType == null || aggregateType.OwningAggregate.FindBaseAgg(aggregateSymbol5))
				{
					return ACCESSERROR.ACCESSERROR_NOERROR;
				}
			}
		}
		if (!flag)
		{
			return ACCESSERROR.ACCESSERROR_NOACCESS;
		}
		return ACCESSERROR.ACCESSERROR_NOACCESSTHRU;
	}

	public static bool CheckBogus(Symbol sym)
	{
		return (sym as PropertySymbol)?.Bogus ?? false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static RuntimeBinderException ReportAccessError(SymWithType swtBad, Symbol symWhere, CType typeQual)
	{
		if (CheckAccess2(swtBad.Sym, swtBad.GetType(), symWhere, typeQual) != ACCESSERROR.ACCESSERROR_NOACCESSTHRU)
		{
			return ErrorHandling.Error(ErrorCode.ERR_BadAccess, swtBad);
		}
		return ErrorHandling.Error(ErrorCode.ERR_BadProtectedAccess, swtBad, typeQual, symWhere);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool CheckAccess(Symbol symCheck, AggregateType atsCheck, Symbol symWhere, CType typeThru)
	{
		return CheckAccess2(symCheck, atsCheck, symWhere, typeThru) == ACCESSERROR.ACCESSERROR_NOERROR;
	}
}
