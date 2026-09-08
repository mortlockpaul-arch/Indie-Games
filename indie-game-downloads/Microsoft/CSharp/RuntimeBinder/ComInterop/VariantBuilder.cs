using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal sealed class VariantBuilder
{
	private MemberExpression _variant;

	private readonly ArgBuilder _argBuilder;

	private readonly VarEnum _targetComType;

	internal ParameterExpression TempVariable { get; private set; }

	internal bool IsByRef => (_targetComType & VarEnum.VT_BYREF) != 0;

	internal VariantBuilder(VarEnum targetComType, ArgBuilder builder)
	{
		_targetComType = targetComType;
		_argBuilder = builder;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal Expression InitializeArgumentVariant(MemberExpression variant, Expression parameter)
	{
		_variant = variant;
		if (IsByRef)
		{
			Expression expression = _argBuilder.MarshalToRef(parameter);
			TempVariable = Expression.Variable(expression.Type, null);
			return Expression.Block(Expression.Assign(TempVariable, expression), Expression.Call(DynamicVariantExtensions.GetByrefSetter(_targetComType & (VarEnum)(-16385)), variant, TempVariable));
		}
		Expression arg = _argBuilder.Marshal(parameter);
		if (_argBuilder is ConvertibleArgBuilder)
		{
			return Expression.Call(typeof(DynamicVariantExtensions).GetMethod("SetAsIConvertible"), variant, arg);
		}
		if (_targetComType.IsPrimitiveType() || _targetComType == VarEnum.VT_DISPATCH || _targetComType == VarEnum.VT_UNKNOWN || _targetComType == VarEnum.VT_VARIANT || _targetComType == VarEnum.VT_RECORD || _targetComType == VarEnum.VT_ARRAY)
		{
			return Expression.Call(null, DynamicVariantExtensions.GetSetter(_targetComType), variant, arg);
		}
		return _targetComType switch
		{
			VarEnum.VT_EMPTY => null, 
			VarEnum.VT_NULL => Expression.Assign(variant, Expression.Property(null, typeof(ComVariant).GetProperty("Null", BindingFlags.Static | BindingFlags.Public))), 
			_ => null, 
		};
	}

	private static Expression Release(Expression pUnk)
	{
		return Expression.Call(typeof(UnsafeMethods).GetMethod("IUnknownReleaseNotZero"), pUnk);
	}

	internal Expression Clear()
	{
		if (IsByRef)
		{
			if (_argBuilder is StringArgBuilder)
			{
				return Expression.Call(typeof(Marshal).GetMethod("FreeBSTR"), TempVariable);
			}
			if (_argBuilder is DispatchArgBuilder)
			{
				return Release(TempVariable);
			}
			if (_argBuilder is UnknownArgBuilder)
			{
				return Release(TempVariable);
			}
			if (_argBuilder is VariantArgBuilder)
			{
				return Expression.Call(TempVariable, typeof(ComVariant).GetMethod("Dispose"));
			}
			return null;
		}
		switch (_targetComType)
		{
		case VarEnum.VT_EMPTY:
		case VarEnum.VT_NULL:
			return null;
		case VarEnum.VT_BSTR:
		case VarEnum.VT_DISPATCH:
		case VarEnum.VT_VARIANT:
		case VarEnum.VT_UNKNOWN:
		case VarEnum.VT_RECORD:
		case VarEnum.VT_ARRAY:
			return Expression.Call(_variant, typeof(ComVariant).GetMethod("Dispose"));
		default:
			return null;
		}
	}

	internal Expression UpdateFromReturn(Expression parameter)
	{
		if (TempVariable == null)
		{
			return null;
		}
		return Expression.Assign(parameter, Helpers.Convert(_argBuilder.UnmarshalFromRef(TempVariable), parameter.Type));
	}
}
