using Microsoft.Xna.Framework;

namespace JamSouls;

public class Target
{
	public virtual Vector2 GetPosition()
	{
		return Vector2.Zero;
	}

	public virtual Vector2 GetTopLeftPosition()
	{
		return Vector2.Zero;
	}

	public virtual Vector2 GetBottomRightPosition()
	{
		return Vector2.Zero;
	}

	public virtual Vector2 GetBottomLeftPosition()
	{
		return Vector2.Zero;
	}
}
