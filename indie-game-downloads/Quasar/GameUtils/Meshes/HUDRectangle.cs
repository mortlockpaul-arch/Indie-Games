using Microsoft.Xna.Framework;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Shaders;
using Quasar.Textures;

namespace Quasar.GameUtils.Meshes;

public class HUDRectangle : BorderedRectangle
{
	public HUDRectangle(Layout2D.LayoutData layout)
		: this(layout.size, layout.position)
	{
	}

	public HUDRectangle(Vector2 size)
		: this(size, Vector2.Zero)
	{
	}

	public HUDRectangle(Vector2 size, Vector2 offset)
		: base(TextureManager.Textures["HUD/HUD"], size, offset, GameTemplate.HUDRectangleMargins)
	{
		FirstMaterial.Textures.Add(TextureManager.Textures["HUD/HUDPattern"]);
		Shader = ShaderManager.Shaders["HUDRectangle"];
		FirstMaterial.AddFloatParameter(GameTemplate.HUDRectanglePatternSize[0]);
		FirstMaterial.AddFloatParameter(GameTemplate.HUDRectanglePatternSize[1]);
	}
}
