using System.Runtime.InteropServices;

namespace System.StubHelpers;

internal static class BSTRMarshaler
{
	internal unsafe static nint ConvertToNative(string strManaged, nint pNativeBuffer)
	{
		if (strManaged == null)
		{
			return IntPtr.Zero;
		}
		bool num = StubHelpers.TryGetStringTrailByte(strManaged, out var data);
		uint num2 = (uint)(strManaged.Length * 2);
		if (num)
		{
			num2++;
		}
		byte* ptr;
		if (pNativeBuffer != IntPtr.Zero)
		{
			*(uint*)pNativeBuffer = num2;
			ptr = (byte*)(pNativeBuffer + 4);
		}
		else
		{
			ptr = (byte*)Marshal.AllocBSTRByteLen(num2);
		}
		Buffer.Memmove(ref *(char*)ptr, ref strManaged.GetRawStringData(), (nuint)strManaged.Length + (nuint)1u);
		if (num)
		{
			ptr[num2 - 1] = data;
		}
		return (nint)ptr;
	}

	internal unsafe static string ConvertToManaged(nint bstr)
	{
		if (IntPtr.Zero == bstr)
		{
			return null;
		}
		uint num = Marshal.SysStringByteLen(bstr);
		StubHelpers.CheckStringLength(num);
		string text = ((num != 1) ? new string((char*)bstr, 0, (int)(num / 2)) : string.FastAllocateString(0));
		if ((num & 1) == 1)
		{
			StubHelpers.SetStringTrailByte(text, *(byte*)((nuint)bstr + (nuint)(num - 1)));
		}
		return text;
	}

	internal static void ClearNative(nint pNative)
	{
		Marshal.FreeBSTR(pNative);
	}
}
