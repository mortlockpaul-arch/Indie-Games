using Microsoft.Xna.Framework;

namespace Renderer;

public class AnimateParticle
{
	private SpriteInstance m_spr;

	private int m_timeAlive;

	private int m_totalLifeTime;

	private Vector2 m_speed;

	private bool m_bActive;

	private bool m_bFadesOut;

	private Color m_cFadeColor;

	private Color m_cStartColor;

	private float m_fFinalWidth;

	private Vector2 m_vGravity;

	private float m_fDepthModSpeed;

	private float m_fStartWidth;

	public Vector2 Gravity => m_vGravity;

	public bool Active => m_bActive;

	public AnimateParticle()
	{
		m_spr = new SpriteInstance(null, default(Vector2), 0f);
		m_totalLifeTime = 0;
		m_timeAlive = 0;
		m_speed = default(Vector2);
		m_bActive = false;
		m_fFinalWidth = 1f;
		m_fStartWidth = 1f;
	}

	public void Initialize(SpriteImage img, Vector2 startPos, float depth, int totalTime, Vector2 speed, bool fadesOut, Color startColor, Color fadeColor, float startSize, float endSize, bool additive, Vector2 gravity)
	{
		Initialize(img, startPos, depth, totalTime, speed, fadesOut, startColor, fadeColor, startSize, endSize, additive, gravity, 0f, 0f, default(Vector2), isFlat: true);
	}

	public void Initialize(SpriteImage img, Vector2 startPos, float depth, int totalTime, Vector2 speed, bool fadesOut, Color startColor, Color fadeColor, float startSize, float endSize, bool additive, Vector2 gravity, float angle, float depthMod, Vector2 origin, bool isFlat)
	{
		m_spr.Initialize(img, startPos, depth);
		m_spr.FlatColor = isFlat;
		m_spr.Shadowed = false;
		m_spr.Additive = additive;
		m_spr.Color = startColor;
		m_spr.Rotation = angle;
		m_spr.Origin = origin;
		m_timeAlive = 0;
		m_speed = speed;
		m_totalLifeTime = totalTime;
		m_bActive = true;
		m_cStartColor = startColor;
		m_bFadesOut = fadesOut;
		m_cFadeColor = fadeColor;
		m_vGravity = gravity;
		m_fDepthModSpeed = depthMod;
		if (startSize >= 0f)
		{
			m_spr.WidthScale = startSize;
		}
		if (endSize >= 0f)
		{
			m_fFinalWidth = endSize;
		}
		m_fStartWidth = startSize;
	}

	public void Update(TimeTracker gameTime)
	{
		m_timeAlive += gameTime.ElapsedMilli;
		Vector2 vector = m_vGravity * m_timeAlive / 1000f;
		m_spr.Position += (vector + m_speed) * gameTime.FractionOfSecond;
		float num = (float)m_timeAlive / (float)m_totalLifeTime;
		Vector3 vector2 = (1f - num) * m_cStartColor.ToVector3() + num * m_cFadeColor.ToVector3();
		m_spr.Color = new Color(vector2.X, vector2.Y, vector2.Z, m_spr.Alpha);
		if (m_fFinalWidth >= 0f)
		{
			m_spr.WidthScale = (1f - num) * m_fStartWidth + num * m_fFinalWidth;
		}
		if (m_bFadesOut)
		{
			m_spr.Alpha = 1f - num;
		}
		m_spr.Depth += m_fDepthModSpeed * gameTime.FractionOfSecond;
		if (m_timeAlive > m_totalLifeTime)
		{
			m_bActive = false;
		}
	}

	public void Draw(TimeTracker gameTime)
	{
		m_spr.Draw(gameTime);
	}
}
