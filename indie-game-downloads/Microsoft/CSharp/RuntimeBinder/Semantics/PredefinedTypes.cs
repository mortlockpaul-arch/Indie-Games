using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp.RuntimeBinder.Syntax;

namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal static class PredefinedTypes
{
	private static readonly AggregateSymbol[] s_predefSymbols = new AggregateSymbol[49];

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static AggregateSymbol DelayLoadPredefSym(PredefinedType pt)
	{
		return InitializePredefinedType(((AggregateType)SymbolTable.GetCTypeFromType(PredefinedTypeFacts.GetAssociatedSystemType(pt))).OwningAggregate, pt);
	}

	internal static AggregateSymbol InitializePredefinedType(AggregateSymbol sym, PredefinedType pt)
	{
		sym.SetPredefined(predefined: true);
		sym.SetPredefType(pt);
		sym.SetSkipUDOps(pt <= PredefinedType.PT_ENUM && pt != PredefinedType.FirstNonSimpleType && pt != PredefinedType.PT_UINTPTR && pt != PredefinedType.PT_TYPE);
		return sym;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static AggregateSymbol GetPredefinedAggregate(PredefinedType pt)
	{
		ref AggregateSymbol reference = ref s_predefSymbols[(uint)pt];
		return reference ?? (reference = DelayLoadPredefSym(pt));
	}

	private static string GetNiceName(PredefinedType pt)
	{
		return PredefinedTypeFacts.GetNiceName(pt);
	}

	public static string GetNiceName(AggregateSymbol type)
	{
		if (!type.IsPredefined())
		{
			return null;
		}
		return GetNiceName(type.GetPredefType());
	}
}
