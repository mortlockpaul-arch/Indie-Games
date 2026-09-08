using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Net.NetworkInformation;

internal static class InterfaceInfoPal
{
	public unsafe static uint InterfaceNameToIndex<TChar>(ReadOnlySpan<TChar> interfaceName) where TChar : unmanaged, IBinaryNumber<TChar>
	{
		int num = 0;
		char* ptr = null;
		ulong interfaceLuid = 0uL;
		uint ifIndex = 0u;
		if (typeof(TChar) == typeof(char))
		{
			num = interfaceName.Length + 1;
		}
		else if (typeof(TChar) == typeof(byte))
		{
			ReadOnlySpan<byte> bytes = MemoryMarshal.Cast<TChar, byte>(interfaceName);
			num = Encoding.UTF8.GetCharCount(bytes) + 1;
		}
		try
		{
			ptr = (char*)(((uint)num <= 256u) ? null : NativeMemory.Alloc((nuint)(num * 2)));
			Span<char> span = ((ptr != null) ? new Span<char>(ptr, num) : stackalloc char[256].Slice(0, num));
			Span<char> span2 = span;
			if (typeof(TChar) == typeof(char))
			{
				MemoryMarshal.Cast<TChar, char>(interfaceName).CopyTo(span2);
			}
			else if (typeof(TChar) == typeof(byte))
			{
				ReadOnlySpan<byte> bytes2 = MemoryMarshal.Cast<TChar, byte>(interfaceName);
				Encoding.UTF8.GetChars(bytes2, span2);
			}
			span2[span2.Length - 1] = '\0';
			if (global::Interop.IpHlpApi.ConvertInterfaceNameToLuid(span2, ref interfaceLuid) != 0)
			{
				return 0u;
			}
		}
		finally
		{
			if (ptr != null)
			{
				NativeMemory.Free(ptr);
			}
		}
		if (global::Interop.IpHlpApi.ConvertInterfaceLuidToIndex(in interfaceLuid, ref ifIndex) != 0)
		{
			return 0u;
		}
		return ifIndex;
	}
}
