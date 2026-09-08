using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Slots;

namespace Loot.Widgets;

public class WidgetSlots : WidgetAction
{
	protected override Sprite Sprite => WidgetSprite.Slots;

	public WidgetSlots(Location loc)
		: base(loc)
	{
	}

	public WidgetSlots(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(new SlotScreen());
	}
}
