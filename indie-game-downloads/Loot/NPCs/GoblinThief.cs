using System.IO;
using Loot.Dungeon;
using Loot.Items;
using Loot.Screens;

namespace Loot.NPCs;

public class GoblinThief : Goblin
{
	private Item item;

	public GoblinThief(BinaryReader reader)
		: base(reader)
	{
	}

	public GoblinThief(int level, Location loc, Item item)
		: base(level, loc)
	{
		this.item = item;
	}

	public override void OnDeath()
	{
		base.OnDeath();
		if (item != null && DM.Player.Armor == null && item is Armor && !item.Cursed)
		{
			DM.Player.SetArmor((Armor)item);
		}
		else
		{
			DM.Player.AddItemOrDrop(item);
		}
		Message.Display("You caught the thief!");
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		item = ItemRegistry.Load(reader);
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		ItemRegistry.Save(writer, item);
	}
}
