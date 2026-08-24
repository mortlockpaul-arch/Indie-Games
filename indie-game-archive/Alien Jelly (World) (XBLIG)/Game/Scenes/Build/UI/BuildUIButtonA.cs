using GKEngine.Entities;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Build.UI;

public class BuildUIButtonA : BuildUIButton
{
	public BuildUIButtonA(BuildUI oUI)
		: base(oUI, new Vector2(-9f, -49f), BuildUI.COLOR_TEXT_DEFAULT, "Content/UI/Build/Button_A", oUI.fontKA_18, SpriteString.Align.Left, new Vector2(46f, 6f))
	{
	}

	public override uint GetState()
	{
		uint result = 0u;
		if (ui.universe.mode == BuildUniverse.Modes.Edit && (ui.universe.atoms.selected.Count > 0 || ui.universe.atoms.over != null))
		{
			result = 1u;
		}
		else if (ui.universe.mode == BuildUniverse.Modes.Add)
		{
			result = 2u;
		}
		else if (ui.universe.mode == BuildUniverse.Modes.Camera)
		{
			result = 3u;
		}
		else if (ui.universe.mode == BuildUniverse.Modes.Focus)
		{
			result = 4u;
		}
		return result;
	}

	public override void SetState(uint xState)
	{
		base.SetState(xState);
		switch (state)
		{
		case 1u:
			base.text = "MOVE";
			break;
		case 2u:
			base.text = "PLACE";
			break;
		case 3u:
			base.text = "SET";
			break;
		case 4u:
			base.text = "SET";
			break;
		}
	}
}
