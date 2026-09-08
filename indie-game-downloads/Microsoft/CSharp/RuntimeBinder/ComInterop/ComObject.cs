using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal class ComObject : IDynamicMetaObjectProvider
{
	private static readonly object s_comObjectInfoKey = new object();

	internal object RuntimeCallableWrapper { get; }

	internal ComObject(object rcw)
	{
		RuntimeCallableWrapper = rcw;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static ComObject ObjectToComObject(object rcw)
	{
		object comObjectData = Marshal.GetComObjectData(rcw, s_comObjectInfoKey);
		if (comObjectData != null)
		{
			return (ComObject)comObjectData;
		}
		lock (s_comObjectInfoKey)
		{
			comObjectData = Marshal.GetComObjectData(rcw, s_comObjectInfoKey);
			if (comObjectData != null)
			{
				return (ComObject)comObjectData;
			}
			ComObject comObject = CreateComObject(rcw);
			if (!Marshal.SetComObjectData(rcw, s_comObjectInfoKey, comObject))
			{
				throw Error.SetComObjectDataFailed();
			}
			return comObject;
		}
	}

	internal static MemberExpression RcwFromComObject(Expression comObject)
	{
		return Expression.Property(Helpers.Convert(comObject, typeof(ComObject)), typeof(ComObject).GetProperty("RuntimeCallableWrapper", BindingFlags.Instance | BindingFlags.NonPublic));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal static MethodCallExpression RcwToComObject(Expression rcw)
	{
		return Expression.Call(typeof(ComObject).GetMethod("ObjectToComObject"), Helpers.Convert(rcw, typeof(object)));
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	private static ComObject CreateComObject(object rcw)
	{
		if (rcw is IDispatch rcw2)
		{
			return new IDispatchComObject(rcw2);
		}
		return new ComObject(rcw);
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal virtual IList<string> GetMemberNames(bool dataOnly)
	{
		return Array.Empty<string>();
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal virtual IList<KeyValuePair<string, object>> GetMembers(IEnumerable<string> names)
	{
		return Array.Empty<KeyValuePair<string, object>>();
	}

	DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
	{
		return new ComFallbackMetaObject(parameter, BindingRestrictions.Empty, this);
	}
}
