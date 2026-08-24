using GKEngine.Entities;
using Game.Scenes.Build.Players;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Build.UI;

public class BuildUIButtonRightStick : BuildUIButton
{
	public BuildUIButtonRightStick(BuildUI oUI)
		: base(oUI, new Vector2(142f, -65f), BuildUI.COLOR_TEXT_DEFAULT, "Content/UI/Build/Button_Stick_Right", oUI.fontKA_18, SpriteString.Align.Left, new Vector2(53f, 2f))
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
		if (ui.universe.mode == BuildUniverse.Modes.Edit || ui.universe.mode == BuildUniverse.Modes.Add)
		{
			result = ((ui.universe.player.camera.mode != PlayerCamera.Mode.Zoom) ? 1u : 2u);
		}
		else if (ui.universe.mode == BuildUniverse.Modes.Camera)
		{
			result = 3u;
		}
		return result;
	}

	public override void SetState(uint xState)
	{
		base.SetState(xState);
		switch (state)
		{
		case 1u:
			base.text = "ROTATE\nCAMERA";
			break;
		case 2u:
			base.text = "ZOOM\nCAMERA";
			break;
		case 3u:
			base.text = "ROTATE\nCAMERA";
			break;
		}
	}
}
