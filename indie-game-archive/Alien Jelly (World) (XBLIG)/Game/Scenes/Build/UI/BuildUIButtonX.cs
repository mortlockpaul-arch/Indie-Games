using GKEngine.Entities;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Build.UI;

public class BuildUIButtonX : BuildUIButton
{
	public BuildUIButtonX(BuildUI oUI)
		: base(oUI, new Vector2(-54f, -48f), BuildUI.COLOR_TEXT_DEFAULT, "Content/UI/Build/Button_X", oUI.fontKA_18, SpriteString.Align.Right, new Vector2(0f, 6f))
	{
	}

	public override uint GetState()
	{
		uint result = 0u;
		if (ui.universe.mode == BuildUniverse.Modes.Edit && (ui.universe.atoms.selected.Count > 0 || ui.universe.atoms.over != null))
		{
			result = 1u;
		}
		else if (ui.universe.mode == BuildUniverse.Modes.Add && !ui.universe.painter.selected.autoRotate)
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
			base.text = "ROTATE";
			break;
		case 2u:
			base.text = "ROTATE";
			break;
		}
	}
}
