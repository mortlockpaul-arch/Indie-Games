using System;
using Microsoft.Xna.Framework;
using Renderer;

namespace Screens;

public class ParticleEmitter
{
	private SpriteImage m_img;

	private Vector2 m_pos;

	private float m_startDepth;

	private bool m_bFadesOut;

	private bool m_bAdditive;

	private Color m_startColor1;

	private Color m_startColor2;

	private Color m_endColor1;

	private Color m_endColor2;

	private Vector2 m_gravity;

	private float m_depthMod;

	private Vector2 m_origin;

	private int m_iLifeStart;

	private int m_iLifeEnd;

	private float m_fSpeedStart;

	private float m_fSpeedEnd;

	private float m_fDirection;

	private float m_fDirectionRange;

	private Vector2 m_vMotionModifier;

	private float m_fStartWidth;

	private float m_fEndWidth1;

	private float m_fEndWidth2;

	private float m_angleRange;

	private int m_iSpawnRate;

	private int m_iTimer;

	public float Angle
	{
		get
		{
			return m_fDirection;
		}
		set
		{
			m_fDirection = value;
		}
	}

	public Vector2 Position
	{
		get
		{
			return m_pos;
		}
		set
		{
			m_pos = value;
		}
	}

	public Vector2 Modifier
	{
		get
		{
			return m_vMotionModifier;
		}
		set
		{
			m_vMotionModifier = value;
		}
	}

	public ParticleEmitter(SpriteImage img, Vector2 pos, float startDepth, bool fades, bool additive, Color cStart1, Color cStart2, Color cEnd1, Color cEnd2, Vector2 grav, float depthMod, Vector2 origin, int life1, int life2, float speed1, float speed2, float dir, float dirRange, Vector2 motionMod, float startWidth, float endWidth1, float endWidth2, float angleRange, int spawnRate)
	{
		m_img = img;
		m_pos = pos;
		m_startDepth = startDepth;
		m_bFadesOut = fades;
		m_bAdditive = additive;
		m_startColor1 = cStart1;
		m_startColor2 = cStart2;
		m_endColor1 = cEnd1;
		m_endColor2 = cEnd2;
		m_gravity = grav;
		m_depthMod = depthMod;
		m_origin = origin;
		m_iLifeStart = life1;
		m_iLifeEnd = life2;
		m_fSpeedStart = speed1;
		m_fSpeedEnd = speed2;
		m_fDirection = dir;
		m_fDirectionRange = dirRange;
		m_vMotionModifier = motionMod;
		m_fStartWidth = startWidth;
		m_fEndWidth1 = endWidth1;
		m_fEndWidth2 = endWidth2;
		m_angleRange = angleRange;
		m_iSpawnRate = spawnRate;
		m_iTimer = 0;
	}

	public void SpawnParticle()
	{
		int totalTime = (int)SceneRenderer.GetRand(m_iLifeStart, m_iLifeEnd);
		float num = m_fDirection + SceneRenderer.GetRand((0f - m_fDirectionRange) / 2f, m_fDirectionRange / 2f);
		Vector2 speed = m_vMotionModifier + SceneRenderer.GetRand(m_fSpeedStart, m_fSpeedEnd) * new Vector2(0f - (float)Math.Cos(num), (float)Math.Sin(num));
		float rand = SceneRenderer.GetRand(m_fEndWidth1, m_fEndWidth2);
		float rand2 = SceneRenderer.GetRand((0f - m_angleRange) / 2f, m_angleRange / 2f);
		float rand3 = SceneRenderer.GetRand(0f, 1f);
		Color startColor = new Color(m_startColor1.ToVector4() * rand3 + m_startColor2.ToVector4() * (1f - rand3));
		Color fadeColor = new Color(m_endColor1.ToVector4() * rand3 + m_endColor2.ToVector4() * (1f - rand3));
		ParticleManager.GetParticle().Initialize(m_img, m_pos, m_startDepth, totalTime, speed, m_bFadesOut, startColor, fadeColor, m_fStartWidth, rand, m_bAdditive, m_gravity, rand2, m_depthMod, m_origin, isFlat: true);
	}

	public void Update(TimeTracker gameTime)
	{
		m_iTimer += gameTime.ElapsedMilli;
		while (m_iTimer > m_iSpawnRate)
		{
			m_iTimer -= m_iSpawnRate;
			SpawnParticle();
		}
	}

	public void CreateBurst(int numParticles)
	{
		for (int i = 0; i < numParticles; i++)
		{
			SpawnParticle();
		}
	}

	public void SetColors(Color startColor1, Color startColor2, Color endColor1, Color endColor2)
	{
		m_startColor1 = startColor1;
		m_startColor2 = startColor2;
		m_endColor1 = endColor1;
		m_endColor2 = endColor2;
	}
}
