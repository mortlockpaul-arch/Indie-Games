using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Shaders;
using Quasar.Textures;

namespace AvatarFarmOnline.Items.Game.HUD;

internal class ProgressCircle : Sized2DRectangleMesh
{
	public ProgressCircle(Layout2D.LayoutData layout, Vector3 color)
		: base(layout, TextureManager.Textures["HUD/ProgressCircle"])
	{
		shader = ShaderManager.Shaders["ProgressCircle"];
		FirstMaterial.SetFloatParameter(0, 0f);
		FirstMaterial.Diffuse = color;
		FirstMaterial.Ambient = Vector3.Zero;
	}

	public void SetProgress(float progress)
	{
		FirstMaterial.SetFloatParameter(0, progress);
	}

	public void SetFGColor(Vector3 color)
	{
		FirstMaterial.Diffuse = color;
	}

	public void SetBGColor(Vector3 color)
	{
		FirstMaterial.Ambient = color;
	}
}
