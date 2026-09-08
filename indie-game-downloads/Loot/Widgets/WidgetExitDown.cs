using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Screens;

namespace Loot.Widgets;

public class WidgetExitDown : WidgetAction
{
	protected override Sprite Sprite => ((double)DungeonView.Camera.Zoom > 0.5) ? WidgetSprite.LadderDown : WidgetSprite.ArrowDown;

	public WidgetExitDown(Location loc)
		: base(loc)
	{
	}

	public WidgetExitDown(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		PlaySound.GoDown();
		SaveScreen.GoToDepth(DM.Player.Depth + 1, () => DM.Map.ExitUp);
	}
}
