using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.PC;
using Loot.Screens;

namespace Loot.Items.Potions;

public class PotionHealth : Potion
{
	protected override int RealValue => 100;

	public override Sprite Sprite => PotionSprite.Health;

	public PotionHealth()
		: base("Health Potion")
	{
	}

	public PotionHealth(BinaryReader reader)
		: base(reader, "Health Potion")
	{
	}

	public override void onActivate(int id)
	{
		if (DM.Player.HP == DM.Player.MaxHP)
		{
			Message.Display("You are already healthy.");
		}
		else
		{
			Activate(id);
		}
	}

	public void Activate(int id)
	{
		Player player = DM.Player;
		if (player.HP != player.MaxHP)
		{
			base.onActivate(id);
			int num = player.MaxHP / 2;
			player.HP += num;
			DM.AddHPEffect(DM.Player.Location, num);
		}
	}
}
