using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static class SafeLsaMemoryHandleExtensions
{
	public unsafe static void InitializeReferencedDomainsList(this SafeLsaMemoryHandle referencedDomains)
	{
		referencedDomains.Initialize((uint)Marshal.SizeOf<global::Interop.LSA_REFERENCED_DOMAIN_LIST>());
		global::Interop.LSA_REFERENCED_DOMAIN_LIST lSA_REFERENCED_DOMAIN_LIST = referencedDomains.Read<global::Interop.LSA_REFERENCED_DOMAIN_LIST>(0uL);
		byte* pointer = null;
		try
		{
			referencedDomains.AcquirePointer(ref pointer);
			if (lSA_REFERENCED_DOMAIN_LIST.Domains != IntPtr.Zero)
			{
				global::Interop.LSA_TRUST_INFORMATION* domains = (global::Interop.LSA_TRUST_INFORMATION*)lSA_REFERENCED_DOMAIN_LIST.Domains;
				domains += lSA_REFERENCED_DOMAIN_LIST.Entries;
				long numBytes = (byte*)domains - pointer;
				referencedDomains.Initialize((ulong)numBytes);
			}
		}
		finally
		{
			if (pointer != null)
			{
				referencedDomains.ReleasePointer();
			}
		}
	}
}
