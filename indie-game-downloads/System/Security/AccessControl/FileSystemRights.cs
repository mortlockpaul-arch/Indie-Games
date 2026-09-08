namespace System.Security.AccessControl;

[Flags]
public enum FileSystemRights
{
	ReadData = 1,
	ListDirectory = ReadData,
	WriteData = 2,
	CreateFiles = WriteData,
	AppendData = 4,
	CreateDirectories = AppendData,
	ReadExtendedAttributes = 8,
	WriteExtendedAttributes = 0x10,
	ExecuteFile = 0x20,
	Traverse = ExecuteFile,
	DeleteSubdirectoriesAndFiles = 0x40,
	ReadAttributes = 0x80,
	WriteAttributes = 0x100,
	Delete = 0x10000,
	ReadPermissions = 0x20000,
	ChangePermissions = 0x40000,
	TakeOwnership = 0x80000,
	Synchronize = 0x100000,
	FullControl = ReadData | WriteData | AppendData | ReadExtendedAttributes | WriteExtendedAttributes | ExecuteFile | DeleteSubdirectoriesAndFiles | ReadAttributes | WriteAttributes | Delete | ReadPermissions | ChangePermissions | TakeOwnership | Synchronize,
	Read = 0x20089,
	ReadAndExecute = 0x200A9,
	Write = 0x116,
	Modify = 0x301BF
}
