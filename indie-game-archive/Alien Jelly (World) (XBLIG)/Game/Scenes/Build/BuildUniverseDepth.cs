using Game.Environment;

namespace Game.Scenes.Build;

public class BuildUniverseDepth : UniverseDepth
{
	public BuildUniverse universe;

	public BuildUniverseDepth(BuildUniverse oUniverse)
		: base(oUniverse.scene)
	{
		universe = oUniverse;
	}

	protected override void Render_SetParams()
	{
		base.Render_SetParams();
		_ = scene.cameras.camera;
	}

	protected override void Render_DrawItems()
	{
		base.Render_DrawItems();
		universe.atoms.RenderDepthEffect(ref effectInstanced, ref effectSingle);
	}
}
