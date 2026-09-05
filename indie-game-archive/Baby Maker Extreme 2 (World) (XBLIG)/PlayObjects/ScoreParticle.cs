using System;
using Microsoft.Xna.Framework;
using Renderer;

namespace PlayObjects;

public class ScoreParticle
{
	private string m_text;

	private Vector2 m_pos;

	private Vector2 m_vel;

	private Vector2 m_grav;

	private Color m_color;

	private float m_depth;

	private float m_fadeSpeed;

	private float m_alpha;

	public bool Enabled => m_alpha > 0f;

	public ScoreParticle()
	{
		m_text = "+" + 30 + " points";
		m_pos = default(Vector2);
		m_vel = default(Vector2);
		m_color = Color.Transparent;
		m_depth = DepthConsts.LOGO_DEPTH + 1f;
		m_grav = new Vector2(0f, 1200f);
		m_fadeSpeed = 500f;
		m_alpha = 0f;
	}

	public void ResetTo(Vector2 pos, float dirMod, float pow, float fadeTime)
	{
		m_pos = pos;
		float num = -(float)Math.PI / 2f + dirMod;
		m_vel = new Vector2(0f - (float)Math.Cos(num), (float)Math.Sin(num)) * pow;
		m_alpha = 1f;
		m_fadeSpeed = fadeTime;
	}

	public void Update(TimeTracker gameTime)
	{
		m_vel += m_grav * gameTime.FractionOfSecond;
		m_pos += m_vel * gameTime.FractionOfSecond;
		m_alpha -= gameTime.FractionOfSecond / m_fadeSpeed;
		if (m_alpha < 0f)
		{
			m_color.A = 0;
		}
		else
		{
			m_color.A = (byte)(255f * m_alpha);
		}
	}

	public void Draw(TimeTracker gameTime)
	{
		SceneRenderer.DrawString(fonts.GRUNGE_FONT, m_text, SceneRenderer.GetCameraPosition() + m_pos, m_color, m_depth);
	}
}
