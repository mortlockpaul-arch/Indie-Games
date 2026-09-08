using System;

namespace AvatarFarmOnline.Logic;

[Flags]
internal enum PlayerPermissions
{
	Full = 0,
	Build = 1,
	Harvest = 2,
	None = Build | Harvest
}
