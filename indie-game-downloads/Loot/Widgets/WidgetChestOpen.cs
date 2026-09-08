using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Widgets;

public class WidgetChestOpen : Widget
{
	protected override Sprite Sprite => WidgetSprite.ChestOpened;

	public WidgetChestOpen(Location loc)
		: base(loc)
	{
	}

	public WidgetChestOpen(BinaryReader reader)
		: base(reader)
	{
	}
}
