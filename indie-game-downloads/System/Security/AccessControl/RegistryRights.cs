namespace System.Security.AccessControl;

[Flags]
public enum RegistryRights
{
	QueryValues = 1,
	SetValue = 2,
	CreateSubKey = 4,
	EnumerateSubKeys = 8,
	Notify = 0x10,
	CreateLink = 0x20,
	ExecuteKey = 0x20019,
	ReadKey = ExecuteKey,
	WriteKey = 0x20006,
	Delete = 0x10000,
	ReadPermissions = 0x20000,
	ChangePermissions = 0x40000,
	TakeOwnership = 0x80000,
	FullControl = ExecuteKey | SetValue | CreateSubKey | CreateLink | Delete | ChangePermissions | TakeOwnership
}
