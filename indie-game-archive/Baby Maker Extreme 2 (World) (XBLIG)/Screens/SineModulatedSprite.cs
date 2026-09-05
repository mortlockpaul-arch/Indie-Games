using System;
using Microsoft.Xna.Framework;
using Renderer;

namespace Screens;

public class SineModulatedSprite
{
	private SpriteInstance m_spr;

	private float m_fMinWidth;

	private float m_fMaxWidth;

	private float m_fMinHeight;

	private float m_fMaxHeight;

	private int m_iTransitionTime;

	private float m_fMinWidthModifier;

	private float m_fMaxWidthModifier;

	private float m_fMinHeightModifier;

	private float m_fMaxHeightModifier;

	private int m_iTimer;

	private bool m_bInvertWidthHeight;

	public float Percent
	{
		get
		{
			float num = (float)(m_iTimer % m_iTransitionTime) / (float)m_iTransitionTime;
			return (1f + (float)Math.Sin(Math.PI * 2.0 * (double)num)) / 2f;
		}
		set
		{
			m_iTimer = (int)(value * (float)m_iTransitionTime);
		}
	}

	public float Width
	{
		get
		{
			float percent = Percent;
			return m_fMinWidth * percent + m_fMaxWidth * (1f - percent);
		}
	}

	public SpriteInstance Sprite
	{
		get
		{
			return m_spr;
		}
		set
		{
			m_spr = value;
		}
	}

	public SineModulatedSprite(SpriteInstance sprite, int transitionTime, float minWidth, float maxWidth, bool invertWidthHeight)
	{
		m_spr = sprite;
		m_iTransitionTime = transitionTime;
		m_iTimer = 0;
		m_fMinWidth = minWidth;
		m_fMaxWidth = maxWidth;
		m_fMaxWidthModifier = maxWidth;
		m_fMinWidthModifier = minWidth;
		m_fMinHeight = minWidth * (sprite.SurfaceScale.Y / sprite.SurfaceScale.X);
		m_fMaxHeight = maxWidth * (sprite.SurfaceScale.Y / sprite.SurfaceScale.X);
		m_fMinHeightModifier = m_fMinHeight;
		m_fMaxHeightModifier = m_fMaxHeight;
		m_bInvertWidthHeight = invertWidthHeight;
		UpdateSprite();
	}

	public void Update(TimeTracker gameTime)
	{
		m_iTimer += gameTime.ElapsedMilli;
		if (m_fMinHeight != m_fMinHeightModifier)
		{
			float fMinHeight = m_fMinHeight;
			m_fMinHeight += 400f * gameTime.FractionOfSecond * (float)Math.Sign(m_fMinHeightModifier - m_fMinHeight);
			if (Math.Sign(m_fMinHeightModifier - m_fMinHeight) != Math.Sign(m_fMinHeightModifier - fMinHeight))
			{
				m_fMinHeight = m_fMinHeightModifier;
			}
		}
		if (m_fMaxHeight != m_fMaxHeightModifier)
		{
			float fMaxHeight = m_fMaxHeight;
			m_fMaxHeight += 400f * gameTime.FractionOfSecond * (float)Math.Sign(m_fMaxHeightModifier - m_fMaxHeight);
			if (Math.Sign(m_fMaxHeightModifier - m_fMaxHeight) != Math.Sign(m_fMaxHeightModifier - fMaxHeight))
			{
				m_fMaxHeight = m_fMaxHeightModifier;
			}
		}
		if (m_fMinWidth != m_fMinWidthModifier)
		{
			float fMinWidth = m_fMinWidth;
			m_fMinWidth += 400f * gameTime.FractionOfSecond * (float)Math.Sign(m_fMinWidthModifier - m_fMinWidth);
			if (Math.Sign(m_fMinWidthModifier - m_fMinWidth) != Math.Sign(m_fMinWidthModifier - fMinWidth))
			{
				m_fMinWidth = m_fMinWidthModifier;
			}
		}
		if (m_fMaxWidth != m_fMaxWidthModifier)
		{
			float fMaxWidth = m_fMaxWidth;
			m_fMaxWidth += 400f * gameTime.FractionOfSecond * (float)Math.Sign(m_fMaxWidthModifier - m_fMaxWidth);
			if (Math.Sign(m_fMaxWidthModifier - m_fMaxWidth) != Math.Sign(m_fMaxWidthModifier - fMaxWidth))
			{
				m_fMaxWidth = m_fMaxWidthModifier;
			}
		}
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		float percent = Percent;
		m_spr.WidthScale = m_fMinWidth * percent + m_fMaxWidth * (1f - percent);
		if (m_bInvertWidthHeight)
		{
			float y = m_fMaxHeight * percent + m_fMinHeight * (1f - percent);
			m_spr.SurfaceScale = new Vector2(m_spr.SurfaceScale.X, y);
		}
	}

	public void Draw(TimeTracker gameTime)
	{
		m_spr.Draw(gameTime);
	}

	public void SetNewWidths(float minWidth, float maxWidth)
	{
		m_fMinWidthModifier = minWidth;
		m_fMaxWidthModifier = maxWidth;
		m_fMinHeightModifier = minWidth * (m_fMinHeight / m_fMinWidth);
		m_fMaxHeightModifier = maxWidth * (m_fMaxHeight / m_fMaxWidth);
	}
}
