using Microsoft.Xna.Framework;

namespace PropModel;

public class SpawnPositionData
{
	public const byte SpawnType_Undefined = 0;

	public const byte SpawnType_Consumable = 1;

	public const byte SpawnType_Equipment = 2;

	public const byte SpawnType_Weapon = 3;

	public const byte SpawnType_Vehicle = 4;

	public const byte ItemRange_Undefined = 0;

	public const byte ItemRange_Medical = 1;

	public const byte ItemRange_Nutrition = 2;

	public const byte ItemRange_Mechanical = 3;

	public const byte ItemRange_Civilian = 4;

	public const byte ItemRange_Military = 5;

	public byte itemRange;

	public ushort spawnType;

	public Vector3 spawmPosition;
}
