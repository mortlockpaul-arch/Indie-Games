using Microsoft.Xna.Framework.Graphics;
using Quasar.Meshes;
using Quasar.Render;

namespace Quasar.Items;

public class Skybox : RenderItem
{
	public Skybox(string texture)
	{
		addMesh(new SkyboxMesh(texture));
	}

	public Skybox(TextureCube texture)
	{
		addMesh(new SkyboxMesh(texture));
	}

	protected override void DoRender()
	{
		transform.Translation = SceneRenderData.CurrentRenderData.Camera.Transform.WorldTranslation;
		base.DoRender();
	}
}
