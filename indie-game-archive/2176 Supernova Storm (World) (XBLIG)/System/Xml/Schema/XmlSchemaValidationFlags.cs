namespace System.Xml.Schema;

[Flags]
public enum XmlSchemaValidationFlags
{
	AllowXmlAttributes = 0x10,
	None = 0,
	ProcessIdentityConstraints = 8,
	ProcessInlineSchema = 1,
	ProcessSchemaLocation = 2,
	ReportValidationWarnings = 4
}
