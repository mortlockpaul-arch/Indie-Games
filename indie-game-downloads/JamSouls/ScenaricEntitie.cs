using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public abstract class ScenaricEntitie : Target
{
	public SCENARIC TypeId;

	public string Name;

	public SpriteEffects m_SpriteEffect;

	public float m_zOrder;

	public bool m_bVisible;

	public abstract void Update(GameTime gameTime);

	public abstract void Draw();

	public void InitEntity()
	{
		m_zOrder = 1f;
		m_SpriteEffect = SpriteEffects.None;
		m_bVisible = true;
	}

	public void SetZ(float z)
	{
		m_zOrder = z;
	}

	public float GetZ()
	{
		return m_zOrder;
	}

	public virtual void SetPosition(Vector2 pos)
	{
	}

	public void SetSpriteEffect(SpriteEffects spe)
	{
		m_SpriteEffect = spe;
	}

	public SpriteEffects GetSpriteEffect()
	{
		return m_SpriteEffect;
	}

	public bool IsVisible()
	{
		return m_bVisible;
	}

	public void SetVisible(bool bVisible)
	{
		m_bVisible = bVisible;
	}
}
