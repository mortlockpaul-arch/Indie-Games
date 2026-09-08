using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Numerics.Hashing;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class CSharpIsEventBinder : DynamicMetaObjectBinder, ICSharpBinder
{
	private readonly RuntimeBinder _binder;

	private readonly Type _callingContext;

	public BindingFlag BindingFlags => (BindingFlag)0;

	public bool IsBinderThatCanHaveRefReceiver => false;

	public string Name { get; }

	public override Type ReturnType => typeof(bool);

	public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		return runtimeBinder.BindIsEvent(this, arguments, locals);
	}

	public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
	{
		SymbolTable.PopulateSymbolTableWithName(Name, null, arguments[0].Info.IsStaticType ? (arguments[0].Value as Type) : arguments[0].Type);
	}

	CSharpArgumentInfo ICSharpBinder.GetArgumentInfo(int index)
	{
		return CSharpArgumentInfo.None;
	}

	public CSharpIsEventBinder(string name, Type callingContext)
	{
		Name = name;
		_callingContext = callingContext;
		_binder = new RuntimeBinder(callingContext);
	}

	public int GetGetBinderEquivalenceHash()
	{
		return HashHelpers.Combine(_callingContext?.GetHashCode() ?? 0, Name.GetHashCode());
	}

	public bool IsEquivalentTo(ICSharpBinder other)
	{
		if (!(other is CSharpIsEventBinder cSharpIsEventBinder))
		{
			return false;
		}
		if (_callingContext != cSharpIsEventBinder._callingContext || Name != cSharpIsEventBinder.Name)
		{
			return false;
		}
		return true;
	}

	public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
	{
		BinderHelper.ValidateBindArgument(target, "target");
		return BinderHelper.Bind(this, _binder, new DynamicMetaObject[1] { target }, null, null);
	}
}
