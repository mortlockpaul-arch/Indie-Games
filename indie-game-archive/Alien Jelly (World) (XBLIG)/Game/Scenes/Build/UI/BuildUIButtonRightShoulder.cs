using GKEngine.Entities;
using Game.Atoms;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Build.UI;

public class BuildUIButtonRightShoulder : BuildUIButton
{
	public BuildUIButtonRightShoulder(BuildUI oUI)
		: base(oUI, new Vector2(292f, -65f), BuildUI.COLOR_TEXT_DEFAULT, "Content/UI/Build/Button_Shoulder_Right", oUI.fontKA_18, SpriteString.Align.Left, new Vector2(0f, 26f))
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
		if (ui.universe.mode == BuildUniverse.Modes.Edit && ui.universe.atoms.over != null && ((ui.universe.atoms.over.properties != null && ui.universe.atoms.over.properties.Length > 0) || ui.universe.atoms.over is AtomSwitch))
		{
			result = 1u;
		}
		return result;
	}

	public override void SetState(uint xState)
	{
		base.SetState(xState);
		uint num = state;
		if (num == 1)
		{
			base.text = "PROPERTIES";
		}
	}
}
