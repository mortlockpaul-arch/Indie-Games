using Microsoft.Xna.Framework;

namespace EGEngine;

public class DynamicObject
{
	public bool Active;

	public Vector3 Posiiton;

	public Vector3 Direction;

	public Matrix Transform;

	public virtual void LoadContent()
	{
	}

	public virtual void Update(GameTime gameTime)
	{
	}
}
