using System.IO;

namespace Loot.Items;

public abstract class Amulet : Equipment
{
	public override string Name => (!Identified) ? "Amulet (?)" : name;

	public Amulet(string name)
		: base(Equipment.RandomTier(), name)
	{
		RandomStats();
	}

	public Amulet(EquipmentTier tier, string name)
		: base(tier, name)
	{
		RandomStats();
	}

	public Amulet(BinaryReader reader, string name)
		: base(reader, name)
	{
	}
}
