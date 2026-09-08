using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Screens;

namespace Loot.Widgets;

public class WidgetHole : WidgetAction
{
	protected override Sprite Sprite => ((double)DungeonView.Camera.Zoom > 0.5) ? WidgetSprite.Hole : WidgetSprite.ArrowDown;

	public WidgetHole(Location loc)
		: base(loc)
	{
	}

	public WidgetHole(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		SaveScreen.GoToDepth(DM.Player.Depth + 1, () => DM.Map.FindNearest(Location, DM.Map.IsOpenFloor), delegate
		{
			DM.AddEffect(new FXDustCloud(DM.Player.Location));
		});
	}
}
