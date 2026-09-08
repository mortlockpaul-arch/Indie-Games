using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text.Json.Serialization.Metadata;

internal readonly struct PropertyRef(ulong key, JsonPropertyInfo info, byte[] utf8PropertyName) : IEquatable<PropertyRef>
{
	public readonly ulong Key = key;

	public readonly JsonPropertyInfo Info = info;

	public readonly byte[] Utf8PropertyName = utf8PropertyName;

	public bool Equals(PropertyRef other)
	{
		return Equals(other.Utf8PropertyName, other.Key);
	}

	public override bool Equals(object obj)
	{
		if (obj is PropertyRef other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Key.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ReadOnlySpan<byte> propertyName, ulong key)
	{
		if (key == Key)
		{
			if (propertyName.Length > 7)
			{
				return propertyName.SequenceEqual(Utf8PropertyName);
			}
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ulong GetKey(ReadOnlySpan<byte> name)
	{
		int length = name.Length;
		ulong num = (ulong)(byte)length << 56;
		return num | (ulong)(length switch
		{
			0 => 0L, 
			1 => name[0], 
			2 => MemoryMarshal.Read<ushort>(name), 
			3 => (long)(MemoryMarshal.Read<ushort>(name) | ((ulong)name[2] << 16)), 
			4 => MemoryMarshal.Read<uint>(name), 
			5 => (long)(MemoryMarshal.Read<uint>(name) | ((ulong)name[4] << 32)), 
			6 => (long)(MemoryMarshal.Read<uint>(name) | ((ulong)MemoryMarshal.Read<ushort>(name.Slice(4, 2)) << 32)), 
			7 => (long)(MemoryMarshal.Read<uint>(name) | ((ulong)MemoryMarshal.Read<ushort>(name.Slice(4, 2)) << 32) | ((ulong)name[6] << 48)), 
			_ => (long)(MemoryMarshal.Read<ulong>(name) & 0xFFFFFFFFFFFFFFL), 
		});
	}
}
