using Microsoft.Xna.Framework;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Textures;

namespace Quasar.GameUtils.Meshes;

public class HUDDetailRectangle : BorderedRectangle
{
	public HUDDetailRectangle(Vector2 size, Vector2 offset)
		: base(TextureManager.Textures["HUD/HUDDetail"], size, offset, GameTemplate.HUDDetailRectangleMargins)
	{
	}

	public HUDDetailRectangle(Vector2 size)
		: this(size, Vector2.Zero)
	{
	}

	public HUDDetailRectangle(Layout2D.LayoutData ld)
		: this(ld.size, ld.position)
	{
	}
}
