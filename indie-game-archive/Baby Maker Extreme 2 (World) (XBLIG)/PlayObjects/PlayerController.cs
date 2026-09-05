using Microsoft.Xna.Framework;
using Renderer;

namespace PlayObjects;

public abstract class PlayerController
{
	protected int m_ilastEffectMode;

	public virtual void Draw(TimeTracker gameTime)
	{
	}

	public virtual void Update(TimeTracker gameTime, float modPow, float modBoost, Vector2 averageVel)
	{
	}

	public virtual void HandleInput(TimeTracker gameTime)
	{
	}

	public virtual void Reset()
	{
	}

	public virtual void CollisionResponse()
	{
	}

	public virtual void RevertAction()
	{
	}

	public virtual void SwapOutfit()
	{
	}

	public void SwapVirtual(SpriteInstance spr)
	{
		if (m_ilastEffectMode == 0)
		{
			spr.GetSpriteImage().SetSpritePage(TextureContainer.GetPage("images/Launcher/boostDive3Virtual"));
		}
		else
		{
			spr.GetSpriteImage().SetSpritePage(TextureContainer.GetPage("images/Launcher/boostDive3"));
		}
	}
}
