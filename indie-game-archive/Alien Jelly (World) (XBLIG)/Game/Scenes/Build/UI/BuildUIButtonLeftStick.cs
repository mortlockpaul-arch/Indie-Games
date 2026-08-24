using GKEngine.Entities;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Build.UI;

public class BuildUIButtonLeftStick : BuildUIButton
{
	public BuildUIButtonLeftStick(BuildUI oUI)
		: base(oUI, new Vector2(-206f, -65f), BuildUI.COLOR_TEXT_DEFAULT, "Content/UI/Build/Button_Stick_Left", oUI.fontKA_18, SpriteString.Align.Right, new Vector2(13f, 2f))
	{
	}

	protected override void Load()
	{
		base.Load();
		textTitle.scale = new Vector2(0.77f, 0.77f);
		textTitleShadow.scale = new Vector2(0.77f, 0.77f);
	}

	public override uint GetState()
	{
		uint result = 0u;
		if (ui.universe.mode == BuildUniverse.Modes.Edit)
		{
			result = (ui.universe.player.atomsMoving ? 2u : ((!ui.universe.player.atomsRotate) ? 1u : 3u));
		}
		else if (ui.universe.mode == BuildUniverse.Modes.Add)
		{
			result = 4u;
		}
		else if (ui.universe.mode == BuildUniverse.Modes.Camera)
		{
			result = 5u;
		}
		return result;
	}

	public override void SetState(uint xState)
	{
		base.SetState(xState);
		switch (state)
		{
		case 1u:
			base.text = "MOVE\nCURSOR";
			break;
		case 2u:
			base.text = "MOVE\nBLOCKS";
			break;
		case 3u:
			base.text = "ROTATE\nBLOCKS";
			break;
		case 4u:
			base.text = "MOVE\nBLOCK";
			break;
		case 5u:
			base.text = "MOVE\nCAMERA";
			break;
		}
	}
}
