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
internal sealed class CSharpSetMemberBinder : SetMemberBinder, ICSharpBinder
{
	private readonly CSharpArgumentInfo[] _argumentInfo;

	private readonly RuntimeBinder _binder;

	private readonly Type _callingContext;

	public BindingFlag BindingFlags => (BindingFlag)0;

	public bool IsBinderThatCanHaveRefReceiver => false;

	internal bool IsCompoundAssignment { get; }

	private bool IsChecked => _binder.IsChecked;

	public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		return runtimeBinder.BindAssignment(this, arguments, locals);
	}

	public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
	{
		SymbolTable.PopulateSymbolTableWithName(base.Name, null, arguments[0].Type);
	}

	CSharpArgumentInfo ICSharpBinder.GetArgumentInfo(int index)
	{
		return _argumentInfo[index];
	}

	public CSharpSetMemberBinder(string name, bool isCompoundAssignment, bool isChecked, Type callingContext, IEnumerable<CSharpArgumentInfo> argumentInfo)
		: base(name, ignoreCase: false)
	{
		IsCompoundAssignment = isCompoundAssignment;
		_argumentInfo = BinderHelper.ToArray(argumentInfo);
		_callingContext = callingContext;
		_binder = new RuntimeBinder(callingContext, isChecked);
	}

	public int GetGetBinderEquivalenceHash()
	{
		int h = _callingContext?.GetHashCode() ?? 0;
		if (IsChecked)
		{
			h = HashHelpers.Combine(h, 1);
		}
		if (IsCompoundAssignment)
		{
			h = HashHelpers.Combine(h, 1);
		}
		h = HashHelpers.Combine(h, base.Name.GetHashCode());
		return BinderHelper.AddArgHashes(h, _argumentInfo);
	}

	public bool IsEquivalentTo(ICSharpBinder other)
	{
		if (!(other is CSharpSetMemberBinder cSharpSetMemberBinder))
		{
			return false;
		}
		if (base.Name != cSharpSetMemberBinder.Name || _callingContext != cSharpSetMemberBinder._callingContext || IsChecked != cSharpSetMemberBinder.IsChecked || IsCompoundAssignment != cSharpSetMemberBinder.IsCompoundAssignment || _argumentInfo.Length != cSharpSetMemberBinder._argumentInfo.Length)
		{
			return false;
		}
		return BinderHelper.CompareArgInfos(_argumentInfo, cSharpSetMemberBinder._argumentInfo);
	}

	public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
	{
		if (ComBinder.TryBindSetMember(this, target, value, out var result))
		{
			return result;
		}
		BinderHelper.ValidateBindArgument(target, "target");
		BinderHelper.ValidateBindArgument(value, "value");
		return BinderHelper.Bind(this, _binder, new DynamicMetaObject[2] { target, value }, _argumentInfo, errorSuggestion);
	}
}
