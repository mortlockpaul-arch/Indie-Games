using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Widgets;

public class WidgetFountainEmpty : Widget
{
	protected override Sprite Sprite => WidgetSprite.HPFountainEmpty;

	public WidgetFountainEmpty(Location loc)
		: base(loc)
	{
	}

	public WidgetFountainEmpty(BinaryReader reader)
		: base(reader)
	{
	}
}
