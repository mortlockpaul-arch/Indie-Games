using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Numerics.Hashing;
using Microsoft.CSharp.RuntimeBinder.ComInterop;
using Microsoft.CSharp.RuntimeBinder.Semantics;

namespace Microsoft.CSharp.RuntimeBinder;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class CSharpConvertBinder : ConvertBinder, ICSharpBinder
{
	private readonly RuntimeBinder _binder;

	private readonly Type _callingContext;

	[ExcludeFromCodeCoverage(Justification = "Name should not be called for this binder")]
	public string Name => null;

	public BindingFlag BindingFlags => (BindingFlag)0;

	public bool IsBinderThatCanHaveRefReceiver => false;

	private CSharpConversionKind ConversionKind { get; }

	private bool IsChecked => _binder.IsChecked;

	public Expr DispatchPayload(RuntimeBinder runtimeBinder, ArgumentObject[] arguments, LocalVariableSymbol[] locals)
	{
		if (!base.Explicit)
		{
			return runtimeBinder.BindImplicitConversion(arguments, base.Type, locals, ConversionKind == CSharpConversionKind.ArrayCreationConversion);
		}
		return runtimeBinder.BindExplicitConversion(arguments, base.Type, locals);
	}

	public void PopulateSymbolTableWithName(Type callingType, ArgumentObject[] arguments)
	{
	}

	CSharpArgumentInfo ICSharpBinder.GetArgumentInfo(int index)
	{
		return CSharpArgumentInfo.None;
	}

	public CSharpConvertBinder(Type type, CSharpConversionKind conversionKind, bool isChecked, Type callingContext)
		: base(type, conversionKind == CSharpConversionKind.ExplicitConversion)
	{
		ConversionKind = conversionKind;
		_callingContext = callingContext;
		_binder = new RuntimeBinder(callingContext, isChecked);
	}

	public int GetGetBinderEquivalenceHash()
	{
		int h = _callingContext?.GetHashCode() ?? 0;
		h = HashHelpers.Combine(h, (int)ConversionKind);
		if (IsChecked)
		{
			h = HashHelpers.Combine(h, 1);
		}
		return HashHelpers.Combine(h, base.Type.GetHashCode());
	}

	public bool IsEquivalentTo(ICSharpBinder other)
	{
		if (!(other is CSharpConvertBinder cSharpConvertBinder))
		{
			return false;
		}
		if (ConversionKind != cSharpConvertBinder.ConversionKind || IsChecked != cSharpConvertBinder.IsChecked || _callingContext != cSharpConvertBinder._callingContext || base.Type != cSharpConvertBinder.Type)
		{
			return false;
		}
		return true;
	}

	public override DynamicMetaObject FallbackConvert(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
	{
		if (ComBinder.TryConvert(this, target, out var result))
		{
			return result;
		}
		BinderHelper.ValidateBindArgument(target, "target");
		return BinderHelper.Bind(this, _binder, new DynamicMetaObject[1] { target }, null, errorSuggestion);
	}
}
