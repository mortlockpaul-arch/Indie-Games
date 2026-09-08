namespace System.Security.Cryptography;

public sealed class KeySizes
{
	public int MinSize { get; }

	public int MaxSize { get; }

	public int SkipSize { get; }

	public KeySizes(int minSize, int maxSize, int skipSize)
	{
		MinSize = minSize;
		MaxSize = maxSize;
		SkipSize = skipSize;
	}
}
