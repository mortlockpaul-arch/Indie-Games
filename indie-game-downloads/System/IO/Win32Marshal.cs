using System.Runtime.InteropServices;

namespace System.IO;

internal static class Win32Marshal
{
	internal static Exception GetExceptionForLastWin32Error(string path = "")
	{
		return GetExceptionForWin32Error(Marshal.GetLastPInvokeError(), path);
	}

	internal static Exception GetExceptionForWin32Error(int errorCode, string path = "", string errorDetails = null)
	{
		switch (errorCode)
		{
		case 2:
			return new FileNotFoundException(string.IsNullOrEmpty(path) ? System.SR.IO_FileNotFound : System.SR.Format(System.SR.IO_FileNotFound_FileName, path), path);
		case 3:
			return new DirectoryNotFoundException(string.IsNullOrEmpty(path) ? System.SR.IO_PathNotFound_NoPathName : System.SR.Format(System.SR.IO_PathNotFound_Path, path));
		case 5:
			return new UnauthorizedAccessException(string.IsNullOrEmpty(path) ? System.SR.UnauthorizedAccess_IODenied_NoPathName : System.SR.Format(System.SR.UnauthorizedAccess_IODenied_Path, path));
		case 183:
			if (!string.IsNullOrEmpty(path))
			{
				return new IOException(System.SR.Format(System.SR.IO_AlreadyExists_Name, path), MakeHRFromErrorCode(errorCode));
			}
			break;
		case 206:
			return new PathTooLongException(string.IsNullOrEmpty(path) ? System.SR.IO_PathTooLong : System.SR.Format(System.SR.IO_PathTooLong_Path, path));
		case 32:
			return new IOException(string.IsNullOrEmpty(path) ? System.SR.IO_SharingViolation_NoFileName : System.SR.Format(System.SR.IO_SharingViolation_File, path), MakeHRFromErrorCode(errorCode));
		case 80:
			if (!string.IsNullOrEmpty(path))
			{
				return new IOException(System.SR.Format(System.SR.IO_FileExists_Name, path), MakeHRFromErrorCode(errorCode));
			}
			break;
		case 995:
			return new OperationCanceledException();
		}
		string text = GetPInvokeErrorMessage(errorCode);
		if (!string.IsNullOrEmpty(path))
		{
			text = text + " : '" + path + "'.";
		}
		if (!string.IsNullOrEmpty(errorDetails))
		{
			text = text + " " + errorDetails;
		}
		return new IOException(text, MakeHRFromErrorCode(errorCode));
		static string GetPInvokeErrorMessage(int error)
		{
			return Marshal.GetPInvokeErrorMessage(error);
		}
	}

	internal static int MakeHRFromErrorCode(int errorCode)
	{
		if ((0xFFFF0000u & errorCode) != 0L)
		{
			return errorCode;
		}
		return -2147024896 | errorCode;
	}
}
