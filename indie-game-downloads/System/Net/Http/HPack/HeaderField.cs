using System.Text;

namespace System.Net.Http.HPack;

internal readonly struct HeaderField(int? staticTableIndex, ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
{
	public int? StaticTableIndex { get; } = staticTableIndex;

	public byte[] Name { get; } = name.ToArray();

	public byte[] Value { get; } = value.ToArray();

	public int Length => GetLength(Name.Length, Value.Length);

	public static int GetLength(int nameLength, int valueLength)
	{
		return nameLength + valueLength + 32;
	}

	public override string ToString()
	{
		if (Name != null)
		{
			return Encoding.Latin1.GetString(Name) + ": " + Encoding.Latin1.GetString(Value);
		}
		return "<empty>";
	}
}
