using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Screens;

namespace Loot.Widgets;

public class WidgetExitOut : WidgetAction
{
	private const string downCue = "goDown";

	protected override Sprite Sprite => WidgetSprite.CaveExit;

	public WidgetExitOut(Location loc)
		: base(loc)
	{
	}

	public WidgetExitOut(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnMove()
	{
		DM.Player.Thought = ThoughtBubble.LTrigger;
	}

	public override void OnClick()
	{
		base.OnClick();
		DM.Player.SetDepth(51);
		SaveScreen.GameOver();
	}
}
