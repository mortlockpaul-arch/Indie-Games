using System;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
internal sealed class BoundDispEvent : DynamicObject
{
	private readonly object _rcw;

	private readonly Guid _sourceIid;

	private readonly int _dispid;

	internal BoundDispEvent(object rcw, Guid sourceIid, int dispid)
	{
		_rcw = rcw;
		_sourceIid = sourceIid;
		_dispid = dispid;
	}

	public override bool TryBinaryOperation(BinaryOperationBinder binder, object handler, out object result)
	{
		if (binder.Operation == ExpressionType.AddAssign)
		{
			result = InPlaceAdd(handler);
			return true;
		}
		if (binder.Operation == ExpressionType.SubtractAssign)
		{
			result = InPlaceSubtract(handler);
			return true;
		}
		result = null;
		return false;
	}

	private static void VerifyHandler(object handler)
	{
		if ((handler is Delegate && handler.GetType() != typeof(Delegate)) || handler is IDynamicMetaObjectProvider || handler is DispCallable)
		{
			return;
		}
		throw Error.UnsupportedHandlerType();
	}

	private object InPlaceAdd(object handler)
	{
		VerifyHandler(handler);
		System.Runtime.InteropServices.ComEventsSink.FromRuntimeCallableWrapper(_rcw, _sourceIid, createIfNotFound: true).AddHandler(_dispid, handler);
		return this;
	}

	private object InPlaceSubtract(object handler)
	{
		VerifyHandler(handler);
		System.Runtime.InteropServices.ComEventsSink.FromRuntimeCallableWrapper(_rcw, _sourceIid, createIfNotFound: false)?.RemoveHandler(_dispid, handler);
		return this;
	}
}
