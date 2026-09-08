using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Internal.NativeCrypto;

internal sealed class SafeAlgorithmHandle : SafeBCryptHandle
{
	protected sealed override bool ReleaseHandle()
	{
		return BCryptCloseAlgorithmProvider(handle, 0) == 0;
	}

	[DllImport("BCrypt.dll", ExactSpelling = true)]
	[LibraryImport("BCrypt.dll")]
	private static extern uint BCryptCloseAlgorithmProvider(nint hAlgorithm, int dwFlags);
}
