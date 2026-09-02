using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Inventory;

public class Slot
{
	public enum SLOT_TYPE
	{
		MEDIUM,
		LARGE
	}

	public Item m_item;

	public Texture2D m_bkg;

	public Vector2 m_pos;

	public SLOT_TYPE m_type;

	public Slot(Texture2D bkg, SLOT_TYPE type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_pos = Vector2.Zero;
		m_type = SLOT_TYPE.LARGE;
		base._002Ector();
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
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		SB.Begin((SpriteBlendMode)1);
		SB.Draw(m_bkg, new Rectangle((int)m_pos.X, (int)m_pos.Y, m_bkg.Width, m_bkg.Height), (Rectangle?)null, color, 0f, Vector2.Zero, (SpriteEffects)0, 1f);
		if (m_item == null)
		{
			SB.End();
			return;
		}
		Vector2 pos = m_pos;
		pos.X += (float)(m_bkg.Width / 2);
		pos.Y += (float)(m_bkg.Height / 2);
		switch (m_type)
		{
		case SLOT_TYPE.MEDIUM:
			pos.X -= (float)(m_item.m_icon_medium.Width / 2);
			pos.Y -= (float)(m_item.m_icon_medium.Height / 2);
			SB.Draw(m_item.m_icon_medium, new Rectangle((int)pos.X, (int)pos.Y, m_item.m_icon_medium.Width, m_item.m_icon_medium.Height), color);
			break;
		case SLOT_TYPE.LARGE:
			pos.X -= (float)(m_item.m_icon_large.Width / 2);
			pos.Y -= (float)(m_item.m_icon_large.Height / 2);
			SB.Draw(m_item.m_icon_large, new Rectangle((int)pos.X, (int)pos.Y, m_item.m_icon_large.Width, m_item.m_icon_large.Height), color);
			break;
		}
		SB.End();
	}
}
