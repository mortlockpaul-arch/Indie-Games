using GKEngine.Entities;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Build.UI;

public class BuildUIButtonLeftShoulder : BuildUIButton
{
	public BuildUIButtonLeftShoulder(BuildUI oUI)
		: base(oUI, new Vector2(-362f, -65f), BuildUI.COLOR_TEXT_DEFAULT, "Content/UI/Build/Button_Shoulder_Left", oUI.fontKA_18, SpriteString.Align.Right, new Vector2(101f, 26f))
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
			base.text = "CHANGE AXIS";
		}
	}
}
