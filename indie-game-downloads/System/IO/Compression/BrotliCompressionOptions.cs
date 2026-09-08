namespace System.IO.Compression;

public sealed class BrotliCompressionOptions
{
	private int _quality = 4;

	public int Quality
	{
		get
		{
			return _quality;
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, "value");
			ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 11, "value");
			_quality = value;
		}
	}
}
