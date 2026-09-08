namespace System.Xml.Xsl.Xslt;

internal sealed class DecimalFormatDecl
{
	public readonly XmlQualifiedName Name;

	public readonly string InfinitySymbol;

	public readonly string NanSymbol;

	public readonly char[] Characters;

	public static readonly DecimalFormatDecl Default = new DecimalFormatDecl(new XmlQualifiedName(), "Infinity", "NaN", ".,%‰0#;-");

	public DecimalFormatDecl(XmlQualifiedName name, string infinitySymbol, string nanSymbol, string characters)
	{
		Name = name;
		InfinitySymbol = infinitySymbol;
		NanSymbol = nanSymbol;
		Characters = characters.ToCharArray();
	}
}
