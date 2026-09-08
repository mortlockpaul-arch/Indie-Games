using System.Diagnostics;

namespace System;

internal sealed class RuntimeMethodInfoStub : IRuntimeMethodInfo
{
	private readonly object m_keepalive;

	private object m_a;

	private object m_b;

	private object m_c;

	private object m_d;

	private object m_e;

	private object m_f;

	private object m_g;

	private object m_h;

	public RuntimeMethodHandleInternal m_value;

	RuntimeMethodHandleInternal IRuntimeMethodInfo.Value => m_value;

	public RuntimeMethodInfoStub(RuntimeMethodHandleInternal methodHandleValue, object keepalive)
	{
		m_keepalive = keepalive;
		m_value = methodHandleValue;
	}

	[StackTraceHidden]
	[DebuggerStepThrough]
	[DebuggerHidden]
	internal static object FromPtr(nint pMD)
	{
		RuntimeMethodHandleInternal runtimeMethodHandleInternal = new RuntimeMethodHandleInternal(pMD);
		return new RuntimeMethodInfoStub(runtimeMethodHandleInternal, RuntimeMethodHandle.GetLoaderAllocator(runtimeMethodHandleInternal));
	}
}
