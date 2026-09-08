using System;

namespace AvatarFarmOnline.Logic;

[Flags]
public enum ToolTypes
{
	Plow = 1,
	Harvest = 2,
	Plant = 4,
	Water = 8,
	Tree = 0x10,
	None = 0
}
