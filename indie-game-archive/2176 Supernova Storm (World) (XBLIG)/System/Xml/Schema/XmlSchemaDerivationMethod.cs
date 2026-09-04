namespace System.Xml.Schema;

[Flags]
public enum XmlSchemaDerivationMethod
{
	All = 0xFF,
	Empty = 0,
	Extension = 2,
	List = 8,
	None = 0x100,
	Restriction = 4,
	Substitution = 1,
	Union = 0x10
}
