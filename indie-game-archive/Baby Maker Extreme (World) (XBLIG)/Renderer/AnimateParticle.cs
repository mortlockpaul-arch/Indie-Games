using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public class AnimateParticle
{
	private RenderSprite m_spr;

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

	public Vector2 Gravity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vGravity;
		}
	}

	public bool Active => m_bActive;

	public AnimateParticle()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_spr = new RenderSprite(null, default(Vector2), 0f);
		m_totalLifeTime = 0;
		m_timeAlive = 0;
		m_speed = default(Vector2);
		m_bActive = false;
		m_fFinalWidth = 1f;
		m_fStartWidth = 1f;
	}

	public void Initialize(SpriteImage img, Vector2 startPos, float depth, int totalTime, Vector2 speed, bool fadesOut, Color startColor, Color fadeColor, float startSize, float endSize, bool additive, Vector2 gravity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Initialize(img, startPos, depth, totalTime, speed, fadesOut, startColor, fadeColor, startSize, endSize, additive, gravity, 0f, 0f, default(Vector2));
	}

	public void Initialize(SpriteImage img, Vector2 startPos, float depth, int totalTime, Vector2 speed, bool fadesOut, Color startColor, Color fadeColor, float startSize, float endSize, bool additive, Vector2 gravity, float angle, float depthMod, Vector2 origin)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		m_spr.Initialize(img, startPos, depth);
		m_spr.BlendAdditive = additive;
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		m_timeAlive += gameTime.ElapsedMilli;
		Vector2 val = m_vGravity * (float)m_timeAlive / 1000f;
		RenderSprite spr = m_spr;
		spr.Position += (val + m_speed) * gameTime.FractionOfSecond;
		float num = (float)m_timeAlive / (float)m_totalLifeTime;
		m_spr.Color = new Color(new Color((1f - num) * ((Color)(ref m_cStartColor)).ToVector3() + num * ((Color)(ref m_cFadeColor)).ToVector3()), m_spr.Alpha);
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
