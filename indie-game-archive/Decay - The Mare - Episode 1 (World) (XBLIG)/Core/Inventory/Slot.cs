using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core.Inventory;

public class Slot
{
	public enum SLOT_TYPE
	{
		MEDIUM,
		LARGE
	}

	public Item m_item;

	public Texture2D m_bkg;

	public Vector2 m_pos = Vector2.Zero;

	public SLOT_TYPE m_type = SLOT_TYPE.LARGE;

	public Slot(Texture2D bkg, SLOT_TYPE type)
	{
		m_bkg = bkg;
		m_type = type;
	}

	public virtual void Clear()
	{
		m_bkg = null;
		m_item = null;
	}

	public void SetItem(Item item)
	{
		m_item = item;
	}

	public virtual void Update(TimeSpan elapsed)
	{
	}

	public virtual void Draw(SpriteBatch SB, Color color)
	{
		SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		SB.Draw(m_bkg, new Rectangle((int)m_pos.X, (int)m_pos.Y, m_bkg.Width, m_bkg.Height), null, color, 0f, Vector2.Zero, SpriteEffects.None, 1f);
		if (m_item == null)
		{
			SB.End();
			return;
		}
		Vector2 pos = m_pos;
		pos.X += m_bkg.Width / 2;
		pos.Y += m_bkg.Height / 2;
		switch (m_type)
		{
		case SLOT_TYPE.MEDIUM:
		{
			int num = (int)Math.Round((float)m_item.m_icon.Width * 0.57f);
			int num2 = (int)Math.Round((float)m_item.m_icon.Height * 0.57f);
			pos.X -= (float)Math.Round((float)num * 0.5f);
			pos.Y -= (float)Math.Round((float)num2 * 0.5f);
			SB.Draw(m_item.m_icon, new Rectangle((int)pos.X, (int)pos.Y, num, num2), color);
			break;
		}
		case SLOT_TYPE.LARGE:
			pos.X -= (float)Math.Round((float)m_item.m_icon.Width * 0.5f);
			pos.Y -= (float)Math.Round((float)m_item.m_icon.Height * 0.5f);
			SB.Draw(m_item.m_icon, new Rectangle((int)pos.X, (int)pos.Y, m_item.m_icon.Width, m_item.m_icon.Height), color);
			break;
		}
		SB.End();
	}
}
