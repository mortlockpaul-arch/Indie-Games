using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Items;
using Microsoft.Xna.Framework;

namespace Loot.Widgets;

public class WidgetChestClosed : WidgetAction
{
	protected override Sprite Sprite => WidgetSprite.ChestClosed;

	public WidgetChestClosed(Location loc)
		: base(loc)
	{
	}

	public WidgetChestClosed(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		PlaySound.Chest();
		if (DM.Player.IsLucky(33))
		{
			dropLoot();
		}
		else
		{
			fireTrap();
		}
	}

	private void fireTrap()
	{
		DM.AddEffect(FXText.GetFX(DM.Player.Location, "Fire Trap!", new Color(255, 204, 0), new Color(204, 0, 0)));
		addFire(Location.N);
		addFire(Location.S);
		addFire(Location.E);
		addFire(Location.W);
		DM.Map.SetWidget(new WidgetFire(Location));
	}

	private void addFire(Location loc)
	{
		if (DM.Map.IsValid(loc) && DM.Map.IsFloor(loc) && DM.Map.GetWidget(loc) == null)
		{
			DM.Map.SetItem(loc, null);
			DM.Map.SetWidget(new WidgetFire(loc));
		}
	}

	private void dropLoot()
	{
		addItem(Location.N);
		addItem(Location.S);
		addItem(Location.E);
		addItem(Location.W);
		DM.Map.SetWidget(new WidgetChestOpen(Location));
	}

	private void addItem(Location loc)
	{
		if (DM.Map.IsValid(loc) && DM.Map.IsFloor(loc) && !DM.Map.IsItem(loc))
		{
			DM.Map.SetItem(loc, ItemRegistry.Random(DM.Player.Depth));
			DM.AddPoof(loc);
		}
	}
}
