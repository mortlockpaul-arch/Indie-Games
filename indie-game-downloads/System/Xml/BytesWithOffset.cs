namespace System.Xml;

internal readonly struct BytesWithOffset(byte[] bytes, int offset)
{
	private readonly byte[] _bytes = bytes;

	private readonly int _offset = offset;

	public byte[] Bytes => _bytes;

	public int Offset => _offset;
}
