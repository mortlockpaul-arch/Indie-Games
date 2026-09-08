using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal struct ExcepInfo
{
	private short wCode;

	private short wReserved;

	private nint bstrSource;

	private nint bstrDescription;

	private nint bstrHelpFile;

	private int dwHelpContext;

	private nint pvReserved;

	private nint pfnDeferredFillIn;

	private int scode;

	private static string ConvertAndFreeBstr(ref nint bstr)
	{
		if (bstr == IntPtr.Zero)
		{
			return null;
		}
		string result = Marshal.PtrToStringBSTR(bstr);
		Marshal.FreeBSTR(bstr);
		bstr = IntPtr.Zero;
		return result;
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	internal Exception GetException()
	{
		int errorCode = ((scode != 0) ? scode : wCode);
		Exception ex = Marshal.GetExceptionForHR(errorCode) ?? new COMException(null, errorCode);
		string text = ConvertAndFreeBstr(ref bstrDescription);
		if (text != null)
		{
			if (ex is COMException)
			{
				ex = new COMException(text, errorCode);
			}
			else
			{
				ConstructorInfo constructor = ex.GetType().GetConstructor(new Type[1] { typeof(string) });
				if ((object)constructor != null)
				{
					ex = (Exception)constructor.Invoke(new object[1] { text });
				}
			}
		}
		ex.Source = ConvertAndFreeBstr(ref bstrSource);
		string text2 = ConvertAndFreeBstr(ref bstrHelpFile);
		if (text2 != null && dwHelpContext != 0)
		{
			text2 = text2 + "#" + dwHelpContext;
		}
		ex.HelpLink = text2;
		return ex;
	}
}
