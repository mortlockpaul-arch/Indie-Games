using System.IO;
using Loot.Dungeon;

namespace Loot.Widgets;

public abstract class WidgetAction : Widget
{
	public WidgetAction(Location loc)
		: base(loc)
	{
	}

	public WidgetAction(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnMove()
	{
		DM.Player.Thought = ThoughtBubble.LTrigger;
	}

	public override void OnClick()
	{
		DM.Player.Thought = null;
	}
}
