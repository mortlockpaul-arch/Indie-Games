using Microsoft.Xna.Framework;

namespace PlayObjects.Props;

public abstract class PropEffector
{
	public virtual void Draw(TimeTracker gameTime)
	{
	}

	public virtual void Update(TimeTracker gameTime)
	{
	}

	public virtual void CollisionResponse(Player p, Vector2 pos)
	{
	}

	public virtual void Reset()
	{
	}
}
