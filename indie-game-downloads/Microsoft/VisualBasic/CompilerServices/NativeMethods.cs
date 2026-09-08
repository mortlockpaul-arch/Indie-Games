using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VisualBasic.CompilerServices;

[ComVisible(false)]
internal sealed class NativeMethods
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
	internal struct SHFILEOPSTRUCT
	{
		internal nint hwnd;

		internal uint wFunc;

		[MarshalAs(UnmanagedType.LPTStr)]
		internal string pFrom;

		[MarshalAs(UnmanagedType.LPTStr)]
		internal string pTo;

		internal ushort fFlags;

		internal bool fAnyOperationsAborted;

		internal nint hNameMappings;

		[MarshalAs(UnmanagedType.LPTStr)]
		internal string lpszProgressTitle;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	private struct SHFILEOPSTRUCT64
	{
		internal nint hwnd;

		internal uint wFunc;

		[MarshalAs(UnmanagedType.LPTStr)]
		internal string pFrom;

		[MarshalAs(UnmanagedType.LPTStr)]
		internal string pTo;

		internal ushort fFlags;

		internal bool fAnyOperationsAborted;

		internal nint hNameMappings;

		[MarshalAs(UnmanagedType.LPTStr)]
		internal string lpszProgressTitle;
	}

	internal enum SHFileOperationType : uint
	{
		FO_MOVE = 1u,
		FO_COPY,
		FO_DELETE,
		FO_RENAME
	}

	[Flags]
	internal enum ShFileOperationFlags : ushort
	{
		FOF_MULTIDESTFILES = 1,
		FOF_CONFIRMMOUSE = 2,
		FOF_SILENT = 4,
		FOF_RENAMEONCOLLISION = 8,
		FOF_NOCONFIRMATION = 0x10,
		FOF_WANTMAPPINGHANDLE = 0x20,
		FOF_ALLOWUNDO = 0x40,
		FOF_FILESONLY = 0x80,
		FOF_SIMPLEPROGRESS = 0x100,
		FOF_NOCONFIRMMKDIR = 0x200,
		FOF_NOERRORUI = 0x400,
		FOF_NOCOPYSECURITYATTRIBS = 0x800,
		FOF_NORECURSION = 0x1000,
		FOF_NO_CONNECTED_ELEMENTS = 0x2000,
		FOF_WANTNUKEWARNING = 0x4000,
		FOF_NORECURSEREPARSE = 0x8000
	}

	[DllImport("kernel32", BestFitMapping = false, CharSet = CharSet.Auto, ThrowOnUnmappableChar = true)]
	internal static extern int GetVolumeInformation([MarshalAs(UnmanagedType.LPTStr)] string lpRootPathName, StringBuilder lpVolumeNameBuffer, int nVolumeNameSize, ref int lpVolumeSerialNumber, ref int lpMaximumComponentLength, ref int lpFileSystemFlags, nint lpFileSystemNameBuffer, int nFileSystemNameSize);

	internal static int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp)
	{
		if (IntPtr.Size == 4)
		{
			return SHFileOperation32(ref lpFileOp);
		}
		SHFILEOPSTRUCT64 lpFileOp2 = new SHFILEOPSTRUCT64
		{
			hwnd = lpFileOp.hwnd,
			wFunc = lpFileOp.wFunc,
			pFrom = lpFileOp.pFrom,
			pTo = lpFileOp.pTo,
			fFlags = lpFileOp.fFlags,
			fAnyOperationsAborted = lpFileOp.fAnyOperationsAborted,
			hNameMappings = lpFileOp.hNameMappings,
			lpszProgressTitle = lpFileOp.lpszProgressTitle
		};
		int result = SHFileOperation64(ref lpFileOp2);
		lpFileOp.fAnyOperationsAborted = lpFileOp2.fAnyOperationsAborted;
		return result;
	}

	[DllImport("shell32.dll", CharSet = CharSet.Auto, EntryPoint = "SHFileOperation", SetLastError = true, ThrowOnUnmappableChar = true)]
	private static extern int SHFileOperation32(ref SHFILEOPSTRUCT lpFileOp);

	[DllImport("shell32.dll", CharSet = CharSet.Auto, EntryPoint = "SHFileOperation", SetLastError = true, ThrowOnUnmappableChar = true)]
	private static extern int SHFileOperation64(ref SHFILEOPSTRUCT64 lpFileOp);

	[DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern void SHChangeNotify(uint wEventId, uint uFlags, nint dwItem1, nint dwItem2);

	[DllImport("kernel32", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);
}
