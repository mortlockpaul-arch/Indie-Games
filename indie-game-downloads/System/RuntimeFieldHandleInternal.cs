namespace System;

internal struct RuntimeFieldHandleInternal
{
	internal nint m_handle;

	internal nint Value => m_handle;

	internal bool IsNullHandle()
	{
		return m_handle == IntPtr.Zero;
	}

	internal RuntimeFieldHandleInternal(nint value)
	{
		m_handle = value;
	}
}
