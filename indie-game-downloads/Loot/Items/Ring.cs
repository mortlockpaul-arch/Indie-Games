using System.IO;

namespace Loot.Items;

public abstract class Ring : Equipment
{
	public override string Name => (!Identified) ? "Ring (?)" : name;

	public Ring(string name)
		: base(Equipment.RandomTier(), name)
	{
		RandomStats();
	}

	public Ring(EquipmentTier tier, string name)
		: base(tier, name)
	{
		RandomStats();
	}

	public Ring(BinaryReader reader, string name)
		: base(reader, name)
	{
	}
}
