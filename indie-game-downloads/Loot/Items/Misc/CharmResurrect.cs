using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Screens;

namespace Loot.Items.Misc;

public class CharmResurrect : Item
{
	private const int nukeRadius = 7;

	public override string Name => "Charm of Resurrection";

	public override Sprite Sprite => ItemSprite.CharmResurrect;

	public override int Value => 500;

	public override bool Identified => true;

	public CharmResurrect()
	{
	}

	public CharmResurrect(BinaryReader reader)
		: base(reader)
	{
	}

	public override void onActivate(int id)
	{
		Message.Display("It will save your life.");
	}

	public void Resurrect()
	{
		DM.Player.HP = DM.Player.MaxHP;
		for (int i = -7; i <= 7; i++)
		{
			for (int j = -7; j <= 7; j++)
			{
				Location loc = new Location(DM.Player.Location.Row + i, DM.Player.Location.Col + j);
				if (DM.Map.IsValid(loc) && (double)DM.Player.Location.Distance(loc) <= 7.0)
				{
					DM.Map.GetNPC(loc)?.OnHit(999999, crit: true, DM.Player);
				}
			}
		}
		DM.Player.Status.Clear();
		DM.AddEffect(new FXResurrect(DM.Player.Location));
	}
}
