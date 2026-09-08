using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Effects;
using Loot.NPCs;

namespace Loot.Widgets;

public class WidgetTombstoneTrap : WidgetAction
{
	private bool active = true;

	protected override Sprite Sprite => WidgetSprite.Tombstone;

	public WidgetTombstoneTrap(Location loc)
		: base(loc)
	{
	}

	public WidgetTombstoneTrap(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnMove()
	{
		if (active)
		{
			base.OnMove();
		}
	}

	public override void OnClick()
	{
		if (active)
		{
			base.OnClick();
			addEnemies();
			active = false;
		}
	}

	private void addEnemies()
	{
		int depth = DM.Player.Depth;
		int num = ((depth < 5) ? 1 : ((depth < 10) ? 2 : ((depth >= 25) ? 4 : 3)));
		for (int i = 0; i < num; i++)
		{
			Location location = DM.Map.FindNearest(DM.Player.Location, DM.Map.CanAddNPC);
			if (location == Location.Zero)
			{
				break;
			}
			NPC enemy = getEnemy(depth, location);
			DM.AddNPC(enemy);
			DM.AddEffect(new FXPoof(enemy));
		}
	}

	private NPC getEnemy(int depth, Location loc)
	{
		return DM.Random.Next(4) switch
		{
			0 => new Bat(depth, loc), 
			1 => new Skeleton(depth, loc), 
			2 => new Zombie(depth, loc), 
			_ => new Vampire(depth, loc), 
		};
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		active = reader.ReadBoolean();
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write(active);
	}
}
