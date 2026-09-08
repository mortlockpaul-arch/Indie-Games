using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace System.Security;

internal static class IdentityHelper
{
	internal static string GetNormalizedUriHash(Uri uri)
	{
		return GetStrongHashSuitableForObjectName(uri.ToString());
	}

	internal static string GetNormalizedStrongNameHash(AssemblyName name)
	{
		byte[] publicKey = name.GetPublicKey();
		if (publicKey == null || publicKey.Length == 0)
		{
			return null;
		}
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(publicKey);
		binaryWriter.Write(name.Version.Major);
		binaryWriter.Write(name.Name);
		memoryStream.Position = 0L;
		return GetStrongHashSuitableForObjectName(memoryStream);
	}

	internal static string GetStrongHashSuitableForObjectName(string name)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(name.ToUpperInvariant());
		memoryStream.Position = 0L;
		return GetStrongHashSuitableForObjectName(memoryStream);
	}

	internal static string GetStrongHashSuitableForObjectName(Stream stream)
	{
		using SHA1 sHA = SHA1.Create();
		return ToBase32StringSuitableForDirName(sHA.ComputeHash(stream));
	}

	internal static string ToBase32StringSuitableForDirName(byte[] buff)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = buff.Length;
		int num2 = 0;
		ReadOnlySpan<byte> readOnlySpan = "abcdefghijklmnopqrstuvwxyz012345"u8;
		do
		{
			byte b = (byte)((num2 < num) ? buff[num2++] : 0);
			byte b2 = (byte)((num2 < num) ? buff[num2++] : 0);
			byte b3 = (byte)((num2 < num) ? buff[num2++] : 0);
			byte b4 = (byte)((num2 < num) ? buff[num2++] : 0);
			byte b5 = (byte)((num2 < num) ? buff[num2++] : 0);
			stringBuilder.Append((char)readOnlySpan[b & 0x1F]);
			stringBuilder.Append((char)readOnlySpan[b2 & 0x1F]);
			stringBuilder.Append((char)readOnlySpan[b3 & 0x1F]);
			stringBuilder.Append((char)readOnlySpan[b4 & 0x1F]);
			stringBuilder.Append((char)readOnlySpan[b5 & 0x1F]);
			stringBuilder.Append((char)readOnlySpan[((b & 0xE0) >> 5) | ((b4 & 0x60) >> 2)]);
			stringBuilder.Append((char)readOnlySpan[((b2 & 0xE0) >> 5) | ((b5 & 0x60) >> 2)]);
			b3 >>= 5;
			if ((b4 & 0x80) != 0)
			{
				b3 |= 8;
			}
			if ((b5 & 0x80) != 0)
			{
				b3 |= 0x10;
			}
			stringBuilder.Append((char)readOnlySpan[b3]);
		}
		while (num2 < num);
		return stringBuilder.ToString();
	}
}
