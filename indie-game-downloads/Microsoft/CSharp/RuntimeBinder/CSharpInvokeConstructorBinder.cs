using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Numerics.Hashing;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class CSharpInvokeConstructorBinder : DynamicMetaObjectBinder, ICSharpInvokeOrInvokeMemberBinder, ICSharpBinder
{
	private readonly CSharpArgumentInfo[] _argumentInfo;

	private readonly RuntimeBinder _binder;

	private readonly Type _callingContext;

	public BindingFlag BindingFlags => (BindingFlag)0;

	public bool IsBinderThatCanHaveRefReceiver => true;

	public CSharpCallFlags Flags { get; }

	public bool StaticCall => true;

	public Type[] TypeArguments => Type.EmptyTypes;

	public string Name => ".ctor";

	bool ICSharpInvokeOrInvokeMemberBinder.ResultDiscarded => false;

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

	public CSharpInvokeConstructorBinder(CSharpCallFlags flags, Type callingContext, IEnumerable<CSharpArgumentInfo> argumentInfo)
	{
		Flags = flags;
		_callingContext = callingContext;
		_argumentInfo = BinderHelper.ToArray(argumentInfo);
		_binder = new RuntimeBinder(callingContext);
	}

	public int GetGetBinderEquivalenceHash()
	{
		return BinderHelper.AddArgHashes(HashHelpers.Combine(HashHelpers.Combine(_callingContext?.GetHashCode() ?? 0, (int)Flags), Name.GetHashCode()), TypeArguments, _argumentInfo);
	}

	public bool IsEquivalentTo(ICSharpBinder other)
	{
		if (!(other is CSharpInvokeConstructorBinder cSharpInvokeConstructorBinder))
		{
			return false;
		}
		if (Flags != cSharpInvokeConstructorBinder.Flags || _callingContext != cSharpInvokeConstructorBinder._callingContext || Name != cSharpInvokeConstructorBinder.Name || TypeArguments.Length != cSharpInvokeConstructorBinder.TypeArguments.Length || _argumentInfo.Length != cSharpInvokeConstructorBinder._argumentInfo.Length)
		{
			return false;
		}
		return BinderHelper.CompareArgInfos(TypeArguments, cSharpInvokeConstructorBinder.TypeArguments, _argumentInfo, cSharpInvokeConstructorBinder._argumentInfo);
	}

	public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
	{
		BinderHelper.ValidateBindArgument(target, "target");
		BinderHelper.ValidateBindArgument(args, "args");
		return BinderHelper.Bind(this, _binder, BinderHelper.Cons(target, args), _argumentInfo, null);
	}
}
