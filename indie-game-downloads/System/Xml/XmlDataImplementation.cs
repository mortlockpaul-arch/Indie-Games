using System.Diagnostics.CodeAnalysis;

namespace System.Xml;

[RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly.")]
[RequiresDynamicCode("Members from serialized types may use dynamic code generation.")]
internal sealed class XmlDataImplementation : XmlImplementation
{
	public override XmlDocument CreateDocument()
	{
		return new XmlDataDocument(this);
	}
}
