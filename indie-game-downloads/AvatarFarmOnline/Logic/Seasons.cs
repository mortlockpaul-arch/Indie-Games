using System;

namespace AvatarFarmOnline.Logic;

[Flags]
internal enum Seasons
{
	None = 0,
	Winter = 1,
	Spring = 2,
	Summer = 4,
	Fall = 8,
	Any = Winter | Spring | Summer | Fall
}
