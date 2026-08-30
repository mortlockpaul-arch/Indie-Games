using Microsoft.Xna.Framework;

namespace Kobingo.Xna.Library;

public class Actor
{
	public bool IsActive { get; protected set; }

	public bool IsVisible { get; set; }

	public int Lifespan { get; set; }

	public int Lifetime { get; private set; }

	public float NormalizedLifetime { get; private set; }

	public Actor()
	{
		IsActive = true;
		IsVisible = true;
	}

	public void Update(GameTime gameTime)
	{
		if (!IsActive)
		{
			return;
		}
		DoUpdate(gameTime);
		Lifetime += gameTime.ElapsedGameTime.Milliseconds;
		if (Lifespan > 0)
		{
			if (Lifetime >= Lifespan)
			{
				Deactivate();
			}
			NormalizedLifetime = MathHelper.Clamp((float)Lifetime / (float)Lifespan, 0f, 1f);
		}
	}

	protected virtual void DoUpdate(GameTime gameTime)
	{
	}

	public void Draw(GameTime gameTime)
	{
		if (IsVisible && IsActive)
		{
			DoDraw(gameTime);
		}
	}

	protected virtual void DoDraw(GameTime gameTime)
	{
	}

	public virtual void Activate()
	{
		IsActive = true;
		Lifetime = 0;
	}

	public virtual void Deactivate()
	{
		IsActive = false;
	}
}
