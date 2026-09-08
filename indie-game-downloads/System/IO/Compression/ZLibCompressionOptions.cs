namespace System.IO.Compression;

public sealed class ZLibCompressionOptions
{
	private int _compressionLevel = -1;

	private ZLibCompressionStrategy _strategy;

	public int CompressionLevel
	{
		get
		{
			return _compressionLevel;
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, -1, "value");
			ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 9, "value");
			_compressionLevel = value;
		}
	}

	public ZLibCompressionStrategy CompressionStrategy
	{
		get
		{
			return _strategy;
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan((int)value, 0, "value");
			ArgumentOutOfRangeException.ThrowIfGreaterThan((int)value, 4, "value");
			_strategy = value;
		}
	}
}
