using GKEngine.Entities;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Build.UI;

public class BuildUIButtonY : BuildUIButton
{
	public BuildUIButtonY(BuildUI oUI)
		: base(oUI, new Vector2(-33f, -88f), BuildUI.COLOR_TEXT_DEFAULT, "Content/UI/Build/Button_Y", oUI.fontKA_18, SpriteString.Align.Right, new Vector2(0f, 6f))
	{
	}

	public override uint GetState()
	{
		uint result = 0u;
		if (ui.universe.mode == BuildUniverse.Modes.Edit)
		{
			result = 1u;
		}
		else if (ui.universe.mode == BuildUniverse.Modes.Add)
		{
			result = 2u;
		}
		return result;
	}

	public override void SetState(uint xState)
	{
		base.SetState(xState);
		switch (state)
		{
		case 1u:
			base.text = "ADD MODE";
			break;
		case 2u:
			base.text = "EDIT MODE";
			break;
		}
	}
}
