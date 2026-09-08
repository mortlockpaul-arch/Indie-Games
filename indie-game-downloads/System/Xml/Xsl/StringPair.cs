namespace System.Xml.Xsl;

internal readonly struct StringPair(string left, string right)
{
	public string Left { get; } = left;

	public string Right { get; } = right;
}
