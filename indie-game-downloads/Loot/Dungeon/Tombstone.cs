using System.IO;
using Loot.Items;
using Loot.PC;

namespace Loot.Dungeon;

public class Tombstone
{
	public readonly string Name;

	public readonly int Depth;

	public readonly Item Item;

	public Tombstone(Player p)
	{
		Name = p.Name;
		Depth = p.Depth;
		int num = DM.Random.Next(4);
		for (int i = 0; i < 4; i++)
		{
			Item item = DM.Player.Equipment[(num + i) % 4];
			if (item != null && !item.Cursed)
			{
				Item = item;
			}
		}
	}

	public Tombstone(BinaryReader reader)
	{
		Name = reader.ReadString();
		Depth = reader.ReadInt32();
		Item = ItemRegistry.Load(reader);
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write(Name);
		writer.Write(Depth);
		ItemRegistry.Save(writer, Item);
	}
}
