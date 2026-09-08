using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class NativeTypes
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class SystemTime
	{
		public short wYear;

		public short wMonth;

		public short wDayOfWeek;

		public short wDay;

		public short wHour;

		public short wMinute;

		public short wSecond;

		public short wMilliseconds;

		internal SystemTime()
		{
		}
	}
}
