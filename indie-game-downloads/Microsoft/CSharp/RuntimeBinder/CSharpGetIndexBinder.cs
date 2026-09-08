using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder.ComInterop;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class CSharpGetIndexBinder : GetIndexBinder, ICSharpBinder
{
	private readonly CSharpArgumentInfo[] _argumentInfo;

	private readonly RuntimeBinder _binder;

	private readonly Type _callingContext;

	public string Name => "$Item$";

	public BindingFlag BindingFlags => BindingFlag.BIND_RVALUEREQUIRED;

	public bool IsBinderThatCanHaveRefReceiver => true;

	public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		Expr optionalIndexerArguments = runtimeBinder.CreateArgumentListEXPR(arguments, locals, 1, arguments.Length);
		return runtimeBinder.BindProperty(this, arguments[0], locals[0], optionalIndexerArguments);
	}

	public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
	{
		SymbolTable.PopulateSymbolTableWithName("$Item$", null, arguments[0].Type);
	}

	CSharpArgumentInfo ICSharpBinder.GetArgumentInfo(int index)
	{
		return _argumentInfo[index];
	}

	public CSharpGetIndexBinder(Type callingContext, IEnumerable<CSharpArgumentInfo> argumentInfo)
		: base(BinderHelper.CreateCallInfo(ref argumentInfo, 1))
	{
		_argumentInfo = argumentInfo as CSharpArgumentInfo[];
		_callingContext = callingContext;
		_binder = new RuntimeBinder(callingContext);
	}

	public int GetGetBinderEquivalenceHash()
	{
		return BinderHelper.AddArgHashes(_callingContext?.GetHashCode() ?? 0, _argumentInfo);
	}

	public bool IsEquivalentTo(ICSharpBinder other)
	{
		if (!(other is CSharpGetIndexBinder cSharpGetIndexBinder))
		{
			return false;
		}
		if (_callingContext != cSharpGetIndexBinder._callingContext || _argumentInfo.Length != cSharpGetIndexBinder._argumentInfo.Length)
		{
			return false;
		}
		return BinderHelper.CompareArgInfos(_argumentInfo, cSharpGetIndexBinder._argumentInfo);
	}

	public override DynamicMetaObject FallbackGetIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject errorSuggestion)
	{
		if (ComBinder.TryBindGetIndex(this, target, indexes, out var result))
		{
			return result;
		}
		BinderHelper.ValidateBindArgument(target, "target");
		BinderHelper.ValidateBindArgument(indexes, "indexes");
		return BinderHelper.Bind(this, _binder, BinderHelper.Cons(target, indexes), _argumentInfo, errorSuggestion);
	}
}
