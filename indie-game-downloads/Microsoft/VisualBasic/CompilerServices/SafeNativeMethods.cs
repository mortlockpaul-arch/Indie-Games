using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
[ComVisible(false)]
internal sealed class SafeNativeMethods
{
	[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	internal static extern void GetLocalTime(NativeTypes.SystemTime systime);
}
