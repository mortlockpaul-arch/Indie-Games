using System;
using System.IO;
using Loot.Dungeon;
using Loot.PC;
using Loot.Screens;
using Loot.Statuses;

namespace Loot.Items.Potions;

public class PotionDrinkMe : Potion
{
	protected override int RealValue => 50;

	public PotionDrinkMe()
		: base("\"Drink Me\"")
	{
	}

	public PotionDrinkMe(BinaryReader reader)
		: base(reader, "\"Drink Me\"")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		Player player = DM.Player;
		if (!player.IsLucky())
		{
			Message.Display("It's poison!");
			DM.Player.Status.Add(new StatusPoison(DM.Player.MaxHP / 80, 20, TimeSpan.FromMilliseconds(500.0)));
			return;
		}
		switch (DM.Random.Next(6))
		{
		case 0:
		{
			int num2 = player.MaxHP / 2;
			player.HP += num2;
			Message.Display("You feel better. +" + num2 + " HP");
			break;
		}
		case 1:
			Message.Display("You feel healthy.");
			DM.Player.Status.RemoveAll<StatusPoison>();
			break;
		case 2:
			Message.Display("You fade out of existence!");
			DM.Player.Status.Add(new StatusInvisible(TimeSpan.FromSeconds(10.0)));
			break;
		case 3:
			Message.Display("You float up into the air!");
			DM.Player.Status.Add(new StatusLevitate(TimeSpan.FromSeconds(40.0)));
			break;
		case 4:
		{
			Message.Display("You feel more experienced!");
			int xp = (player.NextXP - player.PrevXP) / 4;
			player.AddXP(xp);
			break;
		}
		case 5:
		{
			int num = DM.Random.Next(4);
			player.Base.Modify(num, 1);
			player.CalcStats();
			Message.Display("You gained 1 " + Stats.Name[num] + "!");
			break;
		}
		}
	}
}
