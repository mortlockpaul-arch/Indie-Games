namespace System.Security.Cryptography;

[Flags]
public enum CngKeyCreationOptions
{
	None = 0,
	MachineKey = 0x20,
	OverwriteExistingKey = 0x80,
	PreferVbs = 0x10000,
	RequireVbs = 0x20000,
	UsePerBootKey = 0x40000
}
