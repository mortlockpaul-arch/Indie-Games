using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Numerics.Hashing;
using Microsoft.CSharp.RuntimeBinder.ComInterop;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder;

[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
internal sealed class CSharpInvokeBinder : InvokeBinder, ICSharpInvokeOrInvokeMemberBinder, ICSharpBinder
{
	private readonly CSharpCallFlags _flags;

	private readonly CSharpArgumentInfo[] _argumentInfo;

	private readonly RuntimeBinder _binder;

	private readonly Type _callingContext;

	public BindingFlag BindingFlags => (BindingFlag)0;

	public bool IsBinderThatCanHaveRefReceiver => true;

	bool ICSharpInvokeOrInvokeMemberBinder.StaticCall
	{
		get
		{
			if (_argumentInfo[0] != null)
			{
				return _argumentInfo[0].IsStaticType;
			}
			return false;
		}
	}

	string ICSharpBinder.Name => "Invoke";

	Type[] ICSharpInvokeOrInvokeMemberBinder.TypeArguments => Type.EmptyTypes;

	CSharpCallFlags ICSharpInvokeOrInvokeMemberBinder.Flags => _flags;

	bool ICSharpInvokeOrInvokeMemberBinder.ResultDiscarded => (_flags & CSharpCallFlags.ResultDiscarded) != 0;

	public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		return runtimeBinder.DispatchPayload(this, arguments, locals);
	}

	public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
	{
		RuntimeBinder.PopulateSymbolTableWithPayloadInformation(this, callingType, arguments);
	}

	CSharpArgumentInfo ICSharpBinder.GetArgumentInfo(int index)
	{
		return _argumentInfo[index];
	}

	public CSharpInvokeBinder(CSharpCallFlags flags, Type callingContext, IEnumerable<CSharpArgumentInfo> argumentInfo)
		: base(BinderHelper.CreateCallInfo(ref argumentInfo, 1))
	{
		_flags = flags;
		_callingContext = callingContext;
		_argumentInfo = argumentInfo as CSharpArgumentInfo[];
		_binder = new RuntimeBinder(callingContext);
	}

	public int GetGetBinderEquivalenceHash()
	{
		return BinderHelper.AddArgHashes(HashHelpers.Combine(_callingContext?.GetHashCode() ?? 0, (int)_flags), _argumentInfo);
	}

	public bool IsEquivalentTo(ICSharpBinder other)
	{
		if (!(other is CSharpInvokeBinder cSharpInvokeBinder))
		{
			return false;
		}
		if (_flags != cSharpInvokeBinder._flags || _callingContext != cSharpInvokeBinder._callingContext || _argumentInfo.Length != cSharpInvokeBinder._argumentInfo.Length)
		{
			return false;
		}
		return BinderHelper.CompareArgInfos(_argumentInfo, cSharpInvokeBinder._argumentInfo);
	}

	public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion)
	{
		if (ComBinder.TryBindInvoke(this, target, args, out var result))
		{
			return result;
		}
		BinderHelper.ValidateBindArgument(target, "target");
		BinderHelper.ValidateBindArgument(args, "args");
		return BinderHelper.Bind(this, _binder, BinderHelper.Cons(target, args), _argumentInfo, errorSuggestion);
	}
}
