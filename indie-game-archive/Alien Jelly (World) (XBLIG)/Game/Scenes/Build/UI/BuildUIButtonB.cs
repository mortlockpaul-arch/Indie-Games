using GKEngine.Entities;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Build.UI;

public class BuildUIButtonB : BuildUIButton
{
	public BuildUIButtonB(BuildUI oUI)
		: base(oUI, new Vector2(13f, -88f), BuildUI.COLOR_TEXT_DEFAULT, "Content/UI/Build/Button_B", oUI.fontKA_18, SpriteString.Align.Left, new Vector2(46f, 6f))
	{
	}

	public override uint GetState()
	{
		uint result = 0u;
		if (ui.universe.mode == BuildUniverse.Modes.Edit && (ui.universe.atoms.selected.Count > 0 || ui.universe.atoms.over != null))
		{
			result = 1u;
		}
		else if (ui.universe.mode == BuildUniverse.Modes.Add && ui.universe.painter.undo.Count > 0)
		{
			result = 2u;
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
			base.text = "DELETE";
			break;
		case 2u:
			base.text = "UNDO";
			break;
		case 3u:
			base.text = "DONE";
			break;
		}
	}
}
