using Microsoft.Xna.Framework;

namespace Quasar.Elements.Cameras;

public class StaticCamera2D : Camera2D
{
	protected override Vector2 Position2D => new Vector2(0.5f, -0.5f);

	public StaticCamera2D()
	{
	}

	public StaticCamera2D(Vector2 screenSize)
		: base(screenSize)
	{
	}
}
