using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Renderer;

namespace PlayerData;

public class PushManager
{
	private const float POW_ITER = 80f;

	private const float TIME_MIN_WARN = 1f;

	private const float POW_MIN_WARN = 400f;

	private const float HPOW_MAX = 5f;

	private RenderSprite m_sprStrength;

	private RenderSprite m_sprStrengthBg;

	private RenderSprite m_sprTime;

	private RenderSprite m_sprTimeBg;

	private RenderSprite m_sprTimeFg;

	private RenderSprite m_sprPushPrepare;

	private SpriteImage m_pushParticle;

	private SoundEffect m_eActivateSound;

	private AnimatedRenderSprite m_pushImg;

	private float m_fPow;

	private Vector2 m_vAngle;

	private float m_fTime;

	private RenderSprite m_sprPush;

	private Vector2 pushPos;

	private Player m_player;

	private int m_iPushPrequel;

	public bool IsComplete => m_fTime < 0f;

	public Vector2 Angle => m_vAngle;

	public float Power
	{
		get
		{
			float num = 1f - m_fPow / ((float)m_sprStrength.Texture.Height * 5f);
			return m_fPow * (1f - num * num);
		}
	}

	public PushManager(Player p)
	{
		m_player = p;
		Initialize();
	}

	public void Initialize()
	{
		m_iPushPrequel = 2500;
		pushPos = SceneRenderer.GetScreenDim() / 2f;
		m_sprStrength = SpriteManager.GetSprite("images/UI/pressureData", SceneRenderer.GetScreenDim() / 2f + new Vector2(370f, -100f), DepthConsts.CONTR_SIDE_DEPTH);
		m_sprStrengthBg = SpriteManager.GetSprite("images/UI/pressureBack", SceneRenderer.GetScreenDim() / 2f + new Vector2(370f, -100f), DepthConsts.CONTR_SIDE_DEPTH - 0.1f);
		m_sprTime = SpriteManager.GetSprite("images/UI/timerGoo", SceneRenderer.GetScreenDim() / 2f + new Vector2(250f, -60f), DepthConsts.CONTR_SIDE_DEPTH);
		m_sprTimeBg = SpriteManager.GetSprite("images/UI/timerBack", SceneRenderer.GetScreenDim() / 2f + new Vector2(250f, -60f), DepthConsts.CONTR_SIDE_DEPTH - 0.1f);
		m_sprTimeFg = SpriteManager.GetSprite("images/UI/timerFront", SceneRenderer.GetScreenDim() / 2f + new Vector2(250f, -60f), DepthConsts.CONTR_SIDE_DEPTH + 0.1f);
		m_pushParticle = SpriteManager.GetImage("images/whitesquare");
		m_fPow = 10f;
		m_fTime = 0f;
		m_sprTime.Alpha = 0f;
		m_sprTimeBg.Alpha = 0f;
		m_sprTimeFg.Alpha = 0f;
		m_sprStrength.Alpha = 0f;
		m_sprStrengthBg.Alpha = 0f;
		m_pushImg = new AnimatedRenderSprite("images/buttons2", 0.1f, repeats: true, new Rectangle(0, 0, 200, 100), 1, 2, DepthConsts.LOGO_DEPTH);
		m_pushImg.Position = pushPos;
		m_sprPushPrepare = SpriteManager.GetSprite("images/UI/preparemash", pushPos + new Vector2(0f, 150f), DepthConsts.LOGO_DEPTH);
		m_sprPushPrepare.Alpha = 0f;
		m_sprPush = SpriteManager.GetSprite("images/UI/mash", pushPos + new Vector2(0f, 200f), DepthConsts.LOGO_DEPTH);
		m_sprPush.Alpha = 0f;
		m_pushImg.Alpha = 0f;
		m_eActivateSound = SoundManager.GetSoundEffect("sounds/freesound/grunt_15921__pitx__Grito_07");
		m_player.BadBreathing = true;
		m_player.BadPushing = true;
	}

	public void Draw(TimeTracker gameTime, bool isComplete)
	{
		m_sprStrength.Draw(gameTime);
		m_sprTime.Draw(gameTime);
		m_sprTimeBg.Draw(gameTime);
		m_sprTimeFg.Draw(gameTime);
		m_sprStrengthBg.Draw(gameTime);
		m_sprPushPrepare.Draw(gameTime);
		m_pushImg.Draw(gameTime);
		m_sprPush.Draw(gameTime);
	}

	public void HandleInput(TimeTracker gameTime)
	{
		if (m_iPushPrequel <= 0 && ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B))
		{
			IteratePow();
		}
		m_vAngle = new Vector2((float)Math.Cos(-0.30000001192092896), (float)Math.Sin(-0.30000001192092896));
	}

	public void IteratePow()
	{
		float num = 0.5f;
		if (num <= 0.1f)
		{
			num = 0.1f;
		}
		m_fPow += 3f * num * 80f;
		AnimateParticle particle = ParticleManager.GetParticle();
		particle.Initialize(m_pushParticle, m_sprStrength.Position - new Vector2(SceneRenderer.GetRand(-20f, 20f), m_sprStrength.SurfaceScale.Y / 2f), m_sprStrength.Depth + 0.1f, (int)SceneRenderer.GetRand(400f, 1300f), new Vector2(SceneRenderer.GetRand(-100f, 100f), SceneRenderer.GetRand(-200f, 0f)), fadesOut: true, Color.Lime, Color.Lime, 10f, 10f, additive: false, new Vector2(0f, 200f), SceneRenderer.GetRand(0f, 3.14f), 0f, default(Vector2));
		if (m_fPow > 400f)
		{
			m_player.BadPushing = false;
		}
	}

	public void LowerPow(TimeTracker gameTime)
	{
		float num = Math.Min(1f, m_fPow / ((float)m_sprStrength.Texture.Height * 5f));
		m_fPow -= 800f * num * gameTime.FractionOfSecond;
		if (m_fPow < 10f)
		{
			m_fPow = 10f;
		}
	}

	public void IterateTimer(float time)
	{
		m_fTime += time;
		if (m_fTime > 1f)
		{
			m_player.BadBreathing = false;
		}
	}

	public void Update(TimeTracker gameTime, bool isCompleteContractions)
	{
		if (isCompleteContractions)
		{
			if (m_iPushPrequel > 0)
			{
				m_iPushPrequel -= gameTime.ElapsedMilli;
				m_pushImg.Position = pushPos;
				m_pushImg.Rotation = 0f;
				m_sprPushPrepare.Alpha += gameTime.FractionOfSecond * 5f;
				if (m_sprPushPrepare.Alpha > 1f)
				{
					m_sprPushPrepare.Alpha = 1f;
				}
				if (m_iPushPrequel < 300)
				{
					m_sprPushPrepare.Alpha = (float)m_iPushPrequel / 300f;
				}
				m_pushImg.Alpha = m_sprPushPrepare.Alpha;
			}
			else
			{
				m_sprPushPrepare.Alpha -= gameTime.FractionOfSecond * 5f;
				if (m_sprPushPrepare.Alpha < 0f)
				{
					m_sprPushPrepare.Alpha = 0f;
				}
				m_pushImg.Alpha = 1f;
				m_sprPush.Alpha = 1f;
				float fTime = m_fTime;
				m_fTime -= gameTime.FractionOfSecond;
				m_pushImg.Position = pushPos + new Vector2(SceneRenderer.GetRand(-5f, 5f), SceneRenderer.GetRand(-5f, 5f));
				m_pushImg.Rotation = SceneRenderer.GetRand(-0.1f, 0.1f);
				m_sprPush.Position = pushPos + new Vector2(SceneRenderer.GetRand(-5f, 5f), SceneRenderer.GetRand(-5f, 5f) + 150f);
				m_sprPush.Rotation = SceneRenderer.GetRand(-0.1f, 0.1f);
				if (m_fTime < 0.9f && fTime >= 0.9f)
				{
					SoundManager.AddSoundToPlay(m_eActivateSound, 1f, 0f, 0);
				}
				LowerPow(gameTime);
				ControlManager.SetFlatVibration(ControlManager.ActiveMenuIndex, m_fPow / 100f);
			}
		}
		m_sprTime.Alpha += gameTime.FractionOfSecond * 3f;
		if (m_sprTime.Alpha > 1f)
		{
			m_sprTime.Alpha = 1f;
		}
		m_sprTimeBg.Alpha = m_sprTime.Alpha;
		m_sprTimeFg.Alpha = m_sprTime.Alpha;
		m_sprStrengthBg.Alpha = m_sprTime.Alpha;
		m_sprStrength.Alpha = m_sprTime.Alpha;
		m_pushImg.Update(gameTime);
		UpdateMeterScales(gameTime);
		UpdateMeterPositions();
	}

	public void ClearUpdate(TimeTracker gameTime)
	{
		m_sprTime.Alpha -= gameTime.FractionOfSecond * 5f;
		if (m_sprTime.Alpha < 0f)
		{
			m_sprTime.Alpha = 0f;
		}
		m_sprTimeBg.Alpha = m_sprTime.Alpha;
		m_sprTimeFg.Alpha = m_sprTime.Alpha;
		m_sprStrengthBg.Alpha = m_sprTime.Alpha;
		m_sprStrength.Alpha = m_sprTime.Alpha;
		m_sprPush.Alpha = m_sprTime.Alpha;
		m_pushImg.Alpha = m_sprTime.Alpha;
	}

	public void UpdateMeterScales(TimeTracker gameTime)
	{
		float num = m_sprTime.GetSpriteImage().GetSpritePage().Texture.Height;
		if (m_sprTime.SurfaceScale.Y < m_fTime * 50f)
		{
			m_sprTime.GetSpriteImage().Height += gameTime.FractionOfSecond * 250f;
			m_sprTime.SurfaceScale += new Vector2(0f, gameTime.FractionOfSecond * 250f);
		}
		if (m_sprTime.SurfaceScale.Y >= m_fTime * 50f)
		{
			m_sprTime.GetSpriteImage().Height = m_fTime * 50f;
			m_sprTime.SurfaceScale = new Vector2(m_sprTime.SurfaceScale.X, m_fTime * 50f);
		}
		if (m_sprTime.SurfaceScale.Y >= num)
		{
			m_sprTime.GetSpriteImage().Height = num;
			m_sprTime.SurfaceScale = new Vector2(m_sprTime.SurfaceScale.X, num);
		}
		m_fPow = Math.Min((float)m_sprStrength.Texture.Height * 5f, m_fPow);
		m_sprStrength.GetSpriteImage().Height = m_fPow / 5f;
		m_sprStrength.GetSpriteImage().Y = (int)((float)m_sprStrength.GetSpriteImage().GetSpritePage().Texture.Height - m_sprStrength.GetSpriteImage().Height);
		m_sprStrength.SurfaceScale = new Vector2(m_sprStrength.SurfaceScale.X, m_fPow / 5f);
	}

	public void UpdateMeterPositions()
	{
		m_sprTime.Position = m_sprTimeBg.Position + new Vector2(0f, m_sprTimeBg.SurfaceScale.Y / 2f - 143f - m_sprTime.SurfaceScale.Y / 2f);
		m_sprStrength.Position = m_sprStrengthBg.Position + new Vector2(0f, (float)(m_sprStrength.GetSpriteImage().GetSpritePage().Texture.Height / 2) - m_sprStrength.SurfaceScale.Y / 2f);
	}
}
