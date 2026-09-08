using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Screens;

namespace Loot.Widgets;

public class WidgetFountainHP : WidgetAction
{
	protected override Sprite Sprite => WidgetSprite.HPFountainFull;

	public WidgetFountainHP(Location loc)
		: base(loc)
	{
	}

	public WidgetFountainHP(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		if (DM.Player.HP < DM.Player.MaxHP)
		{
			PlaySound.Potion();
			DM.Player.HP = DM.Player.MaxHP;
			DM.AddHPEffect(DM.Player.Location, DM.Player.MaxHP);
			DM.Map.SetWidget(new WidgetFountainEmpty(Location));
		}
		else
		{
			Message.Display("You are at full health!");
		}
	}
}
