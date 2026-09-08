namespace System.Xml.Xsl;

[Flags]
internal enum XslFlags
{
	None = 0,
	String = 1,
	Number = 2,
	Boolean = 4,
	Node = 8,
	Nodeset = 0x10,
	Rtf = 0x20,
	TypeFilter = String | Number | Boolean | Node | Nodeset | Rtf,
	AnyType = TypeFilter,
	Current = 0x100,
	Position = 0x200,
	Last = 0x400,
	FocusFilter = Current | Position | Last,
	FullFocus = FocusFilter,
	HasCalls = 0x1000,
	MayBeDefault = 0x2000,
	SideEffects = 0x4000,
	Stop = 0x8000
}
