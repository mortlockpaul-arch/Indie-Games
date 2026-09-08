using System;
using System.Runtime.CompilerServices;

namespace Internal;

public static class Console
{
	public static class Error
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void WriteLine()
		{
			Write("\r\n");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Write(string s)
		{
			WriteCore(Interop.Kernel32.GetStdHandle(-12), s);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void WriteLine(string? s)
	{
		Write(s + "\r\n");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void WriteLine()
	{
		Write("\r\n");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void Write(string s)
	{
		WriteCore(Interop.Kernel32.GetStdHandle(-11), s);
	}

	private unsafe static void WriteCore(nint handle, string s)
	{
		int num = checked(s.Length * 4);
		int numBytesWritten;
		Span<byte> span;
		if ((uint)num < 1024u)
		{
			numBytesWritten = num;
			span = stackalloc byte[numBytesWritten];
		}
		else
		{
			span = new byte[num];
		}
		Span<byte> span2 = span;
		int numBytesToWrite;
		fixed (char* lpWideCharStr = s)
		{
			fixed (byte* lpMultiByteStr = span2)
			{
				numBytesToWrite = Interop.Kernel32.WideCharToMultiByte(Interop.Kernel32.GetConsoleOutputCP(), 0u, lpWideCharStr, s.Length, lpMultiByteStr, span2.Length, null, null);
			}
		}
		fixed (byte* bytes = span2)
		{
			Interop.Kernel32.WriteFile(handle, bytes, numBytesToWrite, out numBytesWritten, IntPtr.Zero);
		}
	}
}
