namespace System.Text.Json.Schema;

[Flags]
internal enum JsonSchemaType
{
	Any = 0,
	Null = 1,
	Boolean = 2,
	Integer = 4,
	Number = 8,
	String = 0x10,
	Object = 0x20,
	Array = 0x40
}
