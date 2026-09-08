using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System;

internal sealed class Gen2GcCallback : CriticalFinalizerObject
{
	private readonly Func<bool> _callback0;

	private readonly Func<object, bool> _callback1;

	private WeakGCHandle<object> _weakTargetObj;

	private Gen2GcCallback(Func<bool> callback)
	{
		_callback0 = callback;
	}

	private Gen2GcCallback(Func<object, bool> callback, object targetObj)
	{
		_callback1 = callback;
		_weakTargetObj = new WeakGCHandle<object>(targetObj);
	}

	public static void Register(Func<bool> callback)
	{
		new Gen2GcCallback(callback);
	}

	public static void Register(Func<object, bool> callback, object targetObj)
	{
		new Gen2GcCallback(callback, targetObj);
	}

	~Gen2GcCallback()
	{
		if (_weakTargetObj.IsAllocated)
		{
			if (!_weakTargetObj.TryGetTarget(out object target))
			{
				_weakTargetObj.Dispose();
				return;
			}
			try
			{
				if (!_callback1(target))
				{
					_weakTargetObj.Dispose();
					return;
				}
			}
			catch
			{
			}
		}
		else
		{
			try
			{
				if (!_callback0())
				{
					return;
				}
			}
			catch
			{
			}
		}
		GC.ReRegisterForFinalize(this);
	}
}
