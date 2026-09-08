namespace System.Xml.Xsl.Xslt;

internal readonly struct Pattern(TemplateMatch match, int priority)
{
	public readonly TemplateMatch Match = match;

	public readonly int Priority = priority;
}
