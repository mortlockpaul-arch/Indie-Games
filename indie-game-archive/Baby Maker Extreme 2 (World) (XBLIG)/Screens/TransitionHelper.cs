using System;
using Microsoft.Xna.Framework;
using Renderer;

namespace Screens;

public class TransitionHelper
{
	private int m_iTransitionTimer;

	private SpriteInstance m_FadeSprite;

	private SpriteInstance m_FadeSprite2;

	private int m_iTotalTransitionTime;

	private bool m_bIsTransitioningFull;

	private bool m_bIsTransitioningOut;

	private bool m_bIsTransitioningIn;

	public int TransitionTime
	{
		get
		{
			return m_iTotalTransitionTime;
		}
		set
		{
			m_iTotalTransitionTime = value;
		}
	}

	public bool IsTransitionedIn => m_iTransitionTimer >= m_iTotalTransitionTime / 2;

	public bool IsTransitionedOut
	{
		get
		{
			if (!m_bIsTransitioningFull && !m_bIsTransitioningOut)
			{
				return !m_bIsTransitioningIn;
			}
			return false;
		}
	}

	public float Alpha => m_FadeSprite.Alpha;

	public TransitionHelper()
	{
		m_bIsTransitioningIn = false;
		m_bIsTransitioningOut = false;
		m_bIsTransitioningFull = false;
		m_iTransitionTimer = 0;
		m_iTotalTransitionTime = 1000;
		m_FadeSprite = TextureContainer.GetSprite("images/whitesquare", default(Vector2), DepthConsts.FADE_DEPTH);
		m_FadeSprite.Color = Color.White;
		m_FadeSprite.FlatColor = true;
		m_FadeSprite.Additive = true;
		m_FadeSprite2 = TextureContainer.GetSprite("images/particle", default(Vector2), DepthConsts.FADE_DEPTH + 1f);
		m_FadeSprite2.FlatColor = true;
		m_FadeSprite2.Additive = true;
	}

	public void Update(TimeTracker gameTime)
	{
		if (m_bIsTransitioningFull || m_bIsTransitioningOut)
		{
			m_iTransitionTimer += gameTime.ElapsedMilli;
			if (m_iTransitionTimer > m_iTotalTransitionTime)
			{
				m_iTransitionTimer = m_iTotalTransitionTime;
				m_bIsTransitioningFull = false;
				m_bIsTransitioningOut = false;
			}
		}
		if (m_bIsTransitioningIn)
		{
			m_iTransitionTimer += gameTime.ElapsedMilli;
			if (m_iTransitionTimer > m_iTotalTransitionTime / 2)
			{
				m_iTransitionTimer = m_iTotalTransitionTime / 2;
				m_bIsTransitioningIn = false;
			}
		}
		m_FadeSprite.Alpha = (float)Math.Sin(Math.PI * (double)(float)m_iTransitionTimer / (double)m_iTotalTransitionTime);
		m_FadeSprite2.Alpha = m_FadeSprite.Alpha;
	}

	public void Draw(TimeTracker gameTime)
	{
		m_FadeSprite.Position = SceneRenderer.GetCameraPosition();
		m_FadeSprite.SurfaceScale = SceneRenderer.GetScreenDim() * 2f;
		m_FadeSprite.Draw(gameTime);
		m_FadeSprite2.Position = SceneRenderer.GetCameraPosition();
		m_FadeSprite2.WidthScale = SceneRenderer.GetScreenDim().Y;
		m_FadeSprite2.Draw(gameTime);
	}

	public void StartTransition()
	{
		m_iTransitionTimer = 0;
		m_bIsTransitioningFull = true;
		m_bIsTransitioningIn = false;
		m_bIsTransitioningOut = false;
		m_FadeSprite.Alpha = (float)Math.Sin(Math.PI * (double)(float)m_iTransitionTimer / (double)m_iTotalTransitionTime);
		m_FadeSprite2.Alpha = m_FadeSprite.Alpha;
	}

	public void TransitionIn()
	{
		m_iTransitionTimer = 0;
		m_bIsTransitioningIn = true;
		m_bIsTransitioningOut = false;
		m_bIsTransitioningFull = false;
		m_FadeSprite.Alpha = (float)Math.Sin(Math.PI * (double)(float)m_iTransitionTimer / (double)m_iTotalTransitionTime);
		m_FadeSprite2.Alpha = m_FadeSprite.Alpha;
	}

	public void TransitionOut()
	{
		m_iTransitionTimer = m_iTotalTransitionTime / 2;
		m_bIsTransitioningIn = false;
		m_bIsTransitioningOut = true;
		m_bIsTransitioningFull = false;
		m_FadeSprite.Alpha = (float)Math.Sin(Math.PI * (double)(float)m_iTransitionTimer / (double)m_iTotalTransitionTime);
		m_FadeSprite2.Alpha = m_FadeSprite.Alpha;
	}
}
