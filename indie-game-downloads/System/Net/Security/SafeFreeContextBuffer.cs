using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security;

internal abstract class SafeFreeContextBuffer : SafeHandleZeroOrMinusOneIsInvalid
{
	protected SafeFreeContextBuffer()
		: base(ownsHandle: true)
	{
	}

	internal void Set(nint value)
	{
		handle = value;
	}

	internal static int EnumeratePackages(out int pkgnum, out SafeFreeContextBuffer pkgArray)
	{
		int num = global::Interop.SspiCli.EnumerateSecurityPackagesW(out pkgnum, out var safeFreeContextBuffer_SECURITY);
		pkgArray = safeFreeContextBuffer_SECURITY;
		if (num != 0)
		{
			SafeFreeContextBuffer obj = pkgArray;
			if (obj == null)
			{
				return num;
			}
			obj.SetHandleAsInvalid();
		}
		return num;
	}

	internal static SafeFreeContextBuffer CreateEmptyHandle()
	{
		return new SafeFreeContextBuffer_SECURITY();
	}

	public unsafe static int QueryContextAttributes(SafeDeleteContext phContext, global::Interop.SspiCli.ContextAttribute contextAttribute, nint* handle)
	{
		bool success = false;
		try
		{
			phContext.DangerousAddRef(ref success);
			return global::Interop.SspiCli.QueryContextAttributesW(ref phContext._handle, contextAttribute, handle);
		}
		finally
		{
			if (success)
			{
				phContext.DangerousRelease();
			}
		}
	}

	public unsafe static int QueryContextAttributes(SafeDeleteContext phContext, global::Interop.SspiCli.ContextAttribute contextAttribute, byte* buffer, SafeHandle refHandle)
	{
		int num = -2146893055;
		bool success = false;
		try
		{
			phContext.DangerousAddRef(ref success);
			num = global::Interop.SspiCli.QueryContextAttributesW(ref phContext._handle, contextAttribute, buffer);
		}
		finally
		{
			if (success)
			{
				phContext.DangerousRelease();
			}
		}
		if (num == 0 && refHandle != null && refHandle is SafeFreeContextBuffer)
		{
			((SafeFreeContextBuffer)refHandle).Set(*(nint*)buffer);
		}
		if (num != 0)
		{
			refHandle?.SetHandleAsInvalid();
		}
		return num;
	}
}
