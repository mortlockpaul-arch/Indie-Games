using GKEngine;
using GKEngine.Cameras;
using Game.Environment;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes.Play;

public class PlayUniverseDepth : UniverseDepth
{
	public const string PATH_EFFECT_QBIT = "Content/Effects/Pre/Depth_QBit";

	public PlayUniverse universe;

	private Effect effectQBit;

	private EffectParameter effectQBit_View;

	private EffectParameter effectQBit_Proj;

	private EffectParameter effectQBit_FocalLength;

	public PlayUniverseDepth(PlayUniverse oUniverse)
		: base(oUniverse.scene)
	{
		universe = oUniverse;
	}

	public override void Load()
	{
		base.Load();
		effectQBit = GameEngine.SceneContent.Load<Effect>("Content/Effects/Pre/Depth_QBit").Clone();
		effectQBit.Parameters["near"].SetValue(100f);
		effectQBit.Parameters["far"].SetValue(1000f);
		effectQBit_View = effectQBit.Parameters["View"];
		effectQBit_Proj = effectQBit.Parameters["Proj"];
		effectQBit_FocalLength = effectQBit.Parameters["focalLength"];
	}

	protected override void Render_SetParams()
	{
		base.Render_SetParams();
		Camera camera = scene.cameras.camera;
		effectQBit_View.SetValue(camera.view);
		effectQBit_Proj.SetValue(camera.projection);
		effectQBit_FocalLength.SetValue(camera.focalLength);
	}

	protected override void Render_DrawItems()
	{
		base.Render_DrawItems();
		universe.atoms.RenderDepthEffect(ref effectInstanced, ref effectSingle);
	}

	public override void Dispose()
	{
		effectQBit.Dispose();
		base.Dispose();
	}
}
