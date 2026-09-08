using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal static class ComBinder
{
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal sealed class ComGetMemberBinder : GetMemberBinder
	{
		private readonly GetMemberBinder _originalBinder;

		internal bool _canReturnCallables;

		internal ComGetMemberBinder(GetMemberBinder originalBinder, bool canReturnCallables)
			: base(originalBinder.Name, originalBinder.IgnoreCase)
		{
			_originalBinder = originalBinder;
			_canReturnCallables = canReturnCallables;
		}

		public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
		{
			return _originalBinder.FallbackGetMember(target, errorSuggestion);
		}

		public override int GetHashCode()
		{
			return _originalBinder.GetHashCode() ^ (_canReturnCallables ? 1 : 0);
		}

		public override bool Equals(object obj)
		{
			if (obj is ComGetMemberBinder comGetMemberBinder && _canReturnCallables == comGetMemberBinder._canReturnCallables)
			{
				return _originalBinder.Equals(comGetMemberBinder._originalBinder);
			}
			return false;
		}
	}

	public static bool IsComObject(object value)
	{
		if (value != null)
		{
			return Marshal.IsComObject(value);
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool TryBindGetMember(GetMemberBinder binder, DynamicMetaObject instance, out DynamicMetaObject result, bool delayInvocation)
	{
		if (TryGetMetaObject(ref instance))
		{
			ComGetMemberBinder binder2 = new ComGetMemberBinder(binder, delayInvocation);
			result = instance.BindGetMember(binder2);
			if (result.Expression.Type.IsValueType)
			{
				result = new DynamicMetaObject(Expression.Convert(result.Expression, typeof(object)), result.Restrictions);
			}
			return true;
		}
		result = null;
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool TryBindSetMember(SetMemberBinder binder, DynamicMetaObject instance, DynamicMetaObject value, out DynamicMetaObject result)
	{
		if (TryGetMetaObject(ref instance))
		{
			result = instance.BindSetMember(binder, value);
			return true;
		}
		result = null;
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool TryBindInvoke(InvokeBinder binder, DynamicMetaObject instance, DynamicMetaObject[] args, out DynamicMetaObject result)
	{
		if (TryGetMetaObjectInvoke(ref instance))
		{
			result = instance.BindInvoke(binder, args);
			return true;
		}
		result = null;
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool TryBindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject instance, DynamicMetaObject[] args, out DynamicMetaObject result)
	{
		if (TryGetMetaObject(ref instance))
		{
			result = instance.BindInvokeMember(binder, args);
			return true;
		}
		result = null;
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool TryBindGetIndex(GetIndexBinder binder, DynamicMetaObject instance, DynamicMetaObject[] args, out DynamicMetaObject result)
	{
		if (TryGetMetaObjectInvoke(ref instance))
		{
			result = instance.BindGetIndex(binder, args);
			return true;
		}
		result = null;
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static bool TryBindSetIndex(SetIndexBinder binder, DynamicMetaObject instance, DynamicMetaObject[] args, DynamicMetaObject value, out DynamicMetaObject result)
	{
		if (TryGetMetaObjectInvoke(ref instance))
		{
			result = instance.BindSetIndex(binder, args, value);
			return true;
		}
		result = null;
		return false;
	}

	public static bool TryConvert(ConvertBinder binder, DynamicMetaObject instance, out DynamicMetaObject result)
	{
		if (IsComObject(instance.Value) && binder.Type.IsInterface)
		{
			result = new DynamicMetaObject(Expression.Convert(instance.Expression, binder.Type), BindingRestrictions.GetExpressionRestriction(Expression.Call(typeof(ComBinder).GetMethod("IsComObject", BindingFlags.Static | BindingFlags.Public), Helpers.Convert(instance.Expression, typeof(object)))));
			return true;
		}
		result = null;
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static IList<string> GetDynamicDataMemberNames(object value)
	{
		return ComObject.ObjectToComObject(value).GetMemberNames(dataOnly: true);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	internal static IList<KeyValuePair<string, object>> GetDynamicDataMembers(object value, IEnumerable<string> names)
	{
		return ComObject.ObjectToComObject(value).GetMembers(names);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool TryGetMetaObject(ref DynamicMetaObject instance)
	{
		if (instance is ComUnwrappedMetaObject)
		{
			return false;
		}
		if (IsComObject(instance.Value))
		{
			instance = new ComMetaObject(instance.Expression, instance.Restrictions, instance.Value);
			return true;
		}
		return false;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static bool TryGetMetaObjectInvoke(ref DynamicMetaObject instance)
	{
		if (TryGetMetaObject(ref instance))
		{
			return true;
		}
		if (instance.Value is IPseudoComObject pseudoComObject)
		{
			instance = pseudoComObject.GetMetaObject(instance.Expression);
			return true;
		}
		return false;
	}
}
