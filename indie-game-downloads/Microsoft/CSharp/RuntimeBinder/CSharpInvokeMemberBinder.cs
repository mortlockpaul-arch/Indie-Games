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
internal sealed class CSharpInvokeMemberBinder : InvokeMemberBinder, ICSharpInvokeOrInvokeMemberBinder, ICSharpBinder
{
	private readonly CSharpArgumentInfo[] _argumentInfo;

	private readonly RuntimeBinder _binder;

	public BindingFlag BindingFlags => (BindingFlag)0;

	public bool IsBinderThatCanHaveRefReceiver => true;

	bool ICSharpInvokeOrInvokeMemberBinder.StaticCall => _argumentInfo[0]?.IsStaticType ?? false;

	public CSharpCallFlags Flags { get; }

	public Type CallingContext { get; }

	public Type[] TypeArguments { get; }

	bool ICSharpInvokeOrInvokeMemberBinder.ResultDiscarded => (Flags & CSharpCallFlags.ResultDiscarded) != 0;

	public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		return runtimeBinder.DispatchPayload(this, arguments, locals);
	}

	public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
	{
		RuntimeBinder.PopulateSymbolTableWithPayloadInformation(this, callingType, arguments);
	}

	public CSharpArgumentInfo GetArgumentInfo(int index)
	{
		return _argumentInfo[index];
	}

	public CSharpArgumentInfo[] ArgumentInfoArray()
	{
		CSharpArgumentInfo[] array = new CSharpArgumentInfo[_argumentInfo.Length];
		_argumentInfo.CopyTo(array, 0);
		return array;
	}

	public CSharpInvokeMemberBinder(CSharpCallFlags flags, string name, Type callingContext, IEnumerable<Type> typeArguments, IEnumerable<CSharpArgumentInfo> argumentInfo)
		: base(name, ignoreCase: false, BinderHelper.CreateCallInfo(ref argumentInfo, 1))
	{
		Flags = flags;
		CallingContext = callingContext;
		TypeArguments = BinderHelper.ToArray(typeArguments);
		_argumentInfo = BinderHelper.ToArray(argumentInfo);
		_binder = new RuntimeBinder(callingContext);
	}

	public int GetGetBinderEquivalenceHash()
	{
		return BinderHelper.AddArgHashes(HashHelpers.Combine(HashHelpers.Combine(CallingContext?.GetHashCode() ?? 0, (int)Flags), base.Name.GetHashCode()), TypeArguments, _argumentInfo);
	}

	public bool IsEquivalentTo(ICSharpBinder other)
	{
		if (!(other is CSharpInvokeMemberBinder cSharpInvokeMemberBinder))
		{
			return false;
		}
		if (Flags != cSharpInvokeMemberBinder.Flags || CallingContext != cSharpInvokeMemberBinder.CallingContext || base.Name != cSharpInvokeMemberBinder.Name || TypeArguments.Length != cSharpInvokeMemberBinder.TypeArguments.Length || _argumentInfo.Length != cSharpInvokeMemberBinder._argumentInfo.Length)
		{
			return false;
		}
		return BinderHelper.CompareArgInfos(TypeArguments, cSharpInvokeMemberBinder.TypeArguments, _argumentInfo, cSharpInvokeMemberBinder._argumentInfo);
	}

	public override DynamicMetaObject FallbackInvokeMember(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion)
	{
		if (ComBinder.TryBindInvokeMember(this, target, args, out var result))
		{
			return result;
		}
		BinderHelper.ValidateBindArgument(target, "target");
		BinderHelper.ValidateBindArgument(args, "args");
		return BinderHelper.Bind(this, _binder, BinderHelper.Cons(target, args), _argumentInfo, errorSuggestion);
	}

	public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion)
	{
		return new CSharpInvokeBinder(Flags, CallingContext, _argumentInfo).TryGetExisting().Defer(target, args);
	}
}
