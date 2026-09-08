using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Storage;

internal static class UnsafeNativeMethods
{
	[DllImport("kernel32.dll", EntryPoint = "GetDiskFreeSpaceExA")]
	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	public static extern long GetDiskFreeSpaceEx(string lpDirectoryName, out long lpFreeBytesAvailableToCaller, out long lpTotalNumberOfBytes, out long lpTotalNumberOfFreeBytes);
}
