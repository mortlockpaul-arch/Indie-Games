using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Renderer;

namespace PlayerData;

public class ContractionManager
{
	private const float CONTRACTION_TIME = 6.5f;

	private const float TIME_ITER = 0.4f;

	private const float START_TIME = 0.6f;

	private const float END_TIME = 0.3f;

	private const float POW_DIST = 130f;

	private PushManager m_pushManager;

	private RenderSprite m_sprLeft;

	private RenderSprite m_sprRight;

	private RenderSprite m_sprMarker;

	private RenderSprite m_sprBar;

	private float m_fSpeedMove;

	private float m_fTimePassed;

	private int m_iDir;

	private SoundEffect[] m_breathSound;

	private SpriteImage m_particle;

	private RenderSprite m_breathing;

	private bool m_bActivatedRight;

	private bool m_bActivatedLeft;

	public bool IsComplete => m_fTimePassed > 6.5f;

	public ContractionManager(PushManager pushManager)
	{
		m_pushManager = pushManager;
		Initialize();
	}

	public void Initialize()
	{
		m_fTimePassed = 0f;
		m_breathing = SpriteManager.GetSprite("images/UI/keeprhythm", SceneRenderer.GetScreenDim() / 2f + new Vector2(0f, 100f), DepthConsts.LOGO_DEPTH);
		m_sprLeft = SpriteManager.GetSprite("images/UI/LeftTrigger", SceneRenderer.GetScreenDim() / 2f + new Vector2(-130f, -100f), DepthConsts.CONTR_SIDE_DEPTH);
		m_sprRight = SpriteManager.GetSprite("images/UI/RightTrigger", SceneRenderer.GetScreenDim() / 2f + new Vector2(130f, -100f), DepthConsts.CONTR_SIDE_DEPTH);
		m_sprMarker = SpriteManager.GetSprite("images/UI/contractionMarker", SceneRenderer.GetScreenDim() / 2f + new Vector2(0f, -100f), DepthConsts.CONTR_MARKER_DEPTH);
		m_sprBar = SpriteManager.GetSprite("images/UI/contractionBar", SceneRenderer.GetScreenDim() / 2f + new Vector2(0f, -100f), DepthConsts.CONTR_SIDE_DEPTH - 0.1f);
		m_sprBar.SurfaceScale = new Vector2(m_sprRight.Position.X - m_sprLeft.Position.X, m_sprBar.SurfaceScale.Y);
		m_particle = SpriteManager.GetImage("images/UI/contractionParticle");
		m_sprLeft.Alpha = 0f;
		m_sprRight.Alpha = 0f;
		m_sprMarker.Alpha = 0f;
		m_sprBar.Alpha = 0f;
		m_breathing.Alpha = 0f;
		m_iDir = 1;
		m_fSpeedMove = (m_sprRight.Position.X - m_sprLeft.Position.X) / 0.6f;
		m_breathSound = new SoundEffect[2];
		m_breathSound[0] = SoundManager.GetSoundEffect("sounds/freesound/breathin_7805__hanstimm__z1");
		m_breathSound[1] = SoundManager.GetSoundEffect("sounds/freesound/breathout_7805__hanstimm__z1");
		m_bActivatedRight = false;
		m_bActivatedLeft = false;
	}

	public void Draw(TimeTracker gameTime)
	{
		m_sprLeft.Draw(gameTime);
		m_sprRight.Draw(gameTime);
		m_sprMarker.Draw(gameTime);
		m_sprBar.Draw(gameTime);
		m_breathing.Draw(gameTime);
	}

	public void HandleInput(TimeTracker gameTime)
	{
		if (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.LeftTrigger) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.LeftShoulder))
		{
			float num = Math.Abs(m_sprLeft.Position.X - m_sprMarker.Position.X);
			float num2 = Math.Max(0f, 130f - num) / 130f;
			num2 = 1f - (1f - num2) * (1f - num2);
			m_pushManager.IterateTimer(num2 * 0.4f);
			if (num < 130f && !m_bActivatedLeft)
			{
				m_bActivatedLeft = true;
				for (int i = 0; (float)i < num2 * 30f; i++)
				{
					AnimateParticle particle = ParticleManager.GetParticle();
					particle.Initialize(m_particle, m_sprLeft.Position, m_sprLeft.Depth + 0.1f, (int)SceneRenderer.GetRand(400f, 1300f), new Vector2(SceneRenderer.GetRand(-100f, 100f), SceneRenderer.GetRand(-200f, 0f)), fadesOut: true, Color.White, Color.White, 10f, 100f, additive: false, new Vector2(0f, 200f), SceneRenderer.GetRand(0f, 3.14f), 0f, default(Vector2));
				}
			}
		}
		if (!ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.RightTrigger) && !ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.RightShoulder))
		{
			return;
		}
		float num3 = Math.Abs(m_sprRight.Position.X - m_sprMarker.Position.X);
		float num4 = Math.Max(0f, 130f - num3) / 130f;
		num4 = 1f - (1f - num4) * (1f - num4);
		m_pushManager.IterateTimer(num4 * 0.4f);
		if (num3 < 130f && !m_bActivatedRight)
		{
			m_bActivatedRight = true;
			for (int j = 0; (float)j < num4 * 30f; j++)
			{
				AnimateParticle particle2 = ParticleManager.GetParticle();
				particle2.Initialize(m_particle, m_sprRight.Position, m_sprRight.Depth + 0.1f, (int)SceneRenderer.GetRand(400f, 1300f), new Vector2(SceneRenderer.GetRand(-100f, 100f), SceneRenderer.GetRand(-200f, 0f)), fadesOut: true, Color.White, Color.White, 10f, 100f, additive: false, new Vector2(0f, 200f), SceneRenderer.GetRand(0f, 3.14f), 0f, default(Vector2));
			}
		}
	}

	public void Update(TimeTracker gameTime)
	{
		if (!IsComplete)
		{
			m_fTimePassed += gameTime.FractionOfSecond;
			m_sprLeft.Alpha = Math.Min(1f, m_fTimePassed / 0.2f);
			_ = m_sprRight.Position.X;
			_ = m_sprLeft.Position.X;
			m_fSpeedMove = (m_sprRight.Position.X - m_sprLeft.Position.X) / ((1f - m_fTimePassed / 6.5f) * 0.6f + m_fTimePassed / 6.5f * 0.3f);
			m_sprMarker.Position += new Vector2(gameTime.FractionOfSecond * m_fSpeedMove * (float)m_iDir, 0f);
			if (m_iDir > 0 && m_sprMarker.Position.X > m_sprRight.Position.X)
			{
				m_iDir = -1;
				m_bActivatedLeft = false;
				SoundManager.AddSoundToPlay(m_breathSound[0], 1f, 0f, 0);
			}
			else if (m_iDir < 0 && m_sprMarker.Position.X < m_sprLeft.Position.X)
			{
				m_iDir = 1;
				m_bActivatedRight = false;
				SoundManager.AddSoundToPlay(m_breathSound[1], 1f, 0f, 0);
			}
			float num = Math.Abs(m_sprRight.Position.X - m_sprMarker.Position.X);
			m_sprRight.SurfaceScale = new Vector2(m_sprRight.GetSpriteImage().Width, m_sprRight.GetSpriteImage().Height) + new Vector2(20f * Math.Max(0f, 1f - num / 50f));
			num = Math.Abs(m_sprLeft.Position.X - m_sprMarker.Position.X);
			m_sprLeft.SurfaceScale = new Vector2(m_sprLeft.GetSpriteImage().Width, m_sprLeft.GetSpriteImage().Height) + new Vector2(20f * Math.Max(0f, 1f - num / 50f));
			m_breathing.Position = SceneRenderer.GetScreenDim() / 2f + new Vector2(0f, 100f) + new Vector2(SceneRenderer.GetRand(-3f, 3f), SceneRenderer.GetRand(-3f, 3f));
			m_breathing.Rotation = SceneRenderer.GetRand(-0.05f, 0.05f);
		}
		else
		{
			m_sprLeft.Alpha -= gameTime.FractionOfSecond * 4f;
			if (m_sprLeft.Alpha < 0f)
			{
				m_sprLeft.Alpha = 0f;
			}
		}
		m_sprRight.Alpha = m_sprLeft.Alpha;
		m_sprMarker.Alpha = m_sprLeft.Alpha;
		m_sprBar.Alpha = m_sprLeft.Alpha;
		m_breathing.Alpha = m_sprLeft.Alpha;
	}
}
