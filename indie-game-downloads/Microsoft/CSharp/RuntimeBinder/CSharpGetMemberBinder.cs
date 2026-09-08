using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Numerics.Hashing;
using Microsoft.CSharp.RuntimeBinder.ComInterop;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class CSharpGetMemberBinder : GetMemberBinder, IInvokeOnGetBinder, ICSharpBinder
{
	private readonly CSharpArgumentInfo[] _argumentInfo;

	private readonly RuntimeBinder _binder;

	private readonly Type _callingContext;

	public BindingFlag BindingFlags => BindingFlag.BIND_RVALUEREQUIRED;

	public bool IsBinderThatCanHaveRefReceiver => false;

	bool IInvokeOnGetBinder.InvokeOnGet => !ResultIndexed;

	private bool ResultIndexed { get; }

	public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		return runtimeBinder.BindProperty(this, arguments[0], locals[0], null);
	}

	public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
	{
		SymbolTable.PopulateSymbolTableWithName(base.Name, null, arguments[0].Type);
	}

	CSharpArgumentInfo ICSharpBinder.GetArgumentInfo(int index)
	{
		return _argumentInfo[index];
	}

	public CSharpGetMemberBinder(string name, bool resultIndexed, Type callingContext, IEnumerable<CSharpArgumentInfo> argumentInfo)
		: base(name, ignoreCase: false)
	{
		ResultIndexed = resultIndexed;
		_argumentInfo = BinderHelper.ToArray(argumentInfo);
		_callingContext = callingContext;
		_binder = new RuntimeBinder(callingContext);
	}

	public int GetGetBinderEquivalenceHash()
	{
		int h = _callingContext?.GetHashCode() ?? 0;
		if (ResultIndexed)
		{
			h = HashHelpers.Combine(h, 1);
		}
		h = HashHelpers.Combine(h, base.Name.GetHashCode());
		return BinderHelper.AddArgHashes(h, _argumentInfo);
	}

	public bool IsEquivalentTo(ICSharpBinder other)
	{
		if (!(other is CSharpGetMemberBinder cSharpGetMemberBinder))
		{
			return false;
		}
		if (base.Name != cSharpGetMemberBinder.Name || ResultIndexed != cSharpGetMemberBinder.ResultIndexed || _callingContext != cSharpGetMemberBinder._callingContext || _argumentInfo.Length != cSharpGetMemberBinder._argumentInfo.Length)
		{
			return false;
		}
		return BinderHelper.CompareArgInfos(_argumentInfo, cSharpGetMemberBinder._argumentInfo);
	}

	public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
	{
		if (ComBinder.TryBindGetMember(this, target, out var result, ResultIndexed))
		{
			return result;
		}
		BinderHelper.ValidateBindArgument(target, "target");
		return BinderHelper.Bind(this, _binder, new DynamicMetaObject[1] { target }, _argumentInfo, errorSuggestion);
	}
}
