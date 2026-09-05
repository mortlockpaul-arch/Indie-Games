using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Renderer;
using Screens;

namespace Scene;

public class LaunchHelper
{
	private const int PRE_TIME = 2000;

	private const int COMPLETE_TIME = 6000;

	private int m_iTimer;

	private SpriteInstance m_sprBack;

	private SpriteInstance m_sprMeter;

	private ParticleEmitter m_meterParticle;

	private RenderLight m_light;

	private float m_fPow;

	private SpriteInstance m_Button;

	private SpriteInstance m_getReady;

	private SpriteInstance m_timerBar;

	private SpriteInstance m_mash;

	private SoundEffect m_popSound;

	private SoundEffect m_gruntSound;

	private SoundEffect m_breathIn;

	private SoundEffect m_breathOut;

	public float Pow
	{
		get
		{
			return m_fPow;
		}
		set
		{
			m_fPow = value;
		}
	}

	public LaunchHelper()
	{
		m_fPow = 4f;
		SpriteImage image = TextureContainer.GetImage("images/whitesquare");
		m_meterParticle = new ParticleEmitter(image, default(Vector2), 1001f, fades: true, additive: true, Color.Lime, Color.Green, Color.Lime, Color.Green, new Vector2(0f, 600f), -0.1f, default(Vector2), 1000, 1300, 200f, 400f, 4.712389f, 1.6f, default(Vector2), 1f, 20f, 40f, 0f, 1000);
		m_sprBack = TextureContainer.GetSprite("images/pressureBack", new Vector2(1050f, 200f), 900f);
		m_sprBack.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/pressureBackNorm");
		m_sprMeter = TextureContainer.GetSprite("images/pressureData", new Vector2(1050f, 200f), 1000f);
		m_sprMeter.Additive = true;
		m_sprMeter.FlatColor = true;
		m_sprMeter.GetSpriteImage().Height = 10f;
		m_sprMeter.SurfaceScale = new Vector2(m_sprMeter.SurfaceScale.X, m_sprMeter.GetSpriteImage().Height);
		m_sprMeter.GetSpriteImage().Y = (int)((float)m_sprMeter.GetSpriteImage().GetSpritePage().DiffuseTex.Height - m_sprMeter.GetSpriteImage().Height);
		m_sprMeter.RecalcTexCoordinates();
		m_sprMeter.Position = new Vector2(1050f, 200f + (float)m_sprMeter.GetSpriteImage().GetSpritePage().DiffuseTex.Height / 2f - m_sprMeter.SurfaceScale.Y / 2f);
		m_light = new RenderLight(new Vector3(m_sprMeter.Position - new Vector2(0f, m_sprMeter.SurfaceScale.Y / 2f), 200f), 1f, 400, Color.Transparent);
		if (SceneRenderer.GetEffectMode() == 0)
		{
			m_Button = TextureContainer.GetSprite("images/Launcher/giantB", SceneRenderer.GetCameraPosition(), 1000f);
			m_getReady = TextureContainer.GetSprite("images/Launcher/getReady", SceneRenderer.GetCameraPosition(), 1001f);
			m_mash = TextureContainer.GetSprite("images/Launcher/mash", SceneRenderer.GetCameraPosition(), 1001f);
		}
		else
		{
			m_Button = TextureContainer.GetSprite("images/Launcher/giantBVirtual", SceneRenderer.GetCameraPosition(), 1000f);
			m_getReady = TextureContainer.GetSprite("images/Launcher/getReadyVirtual", SceneRenderer.GetCameraPosition(), 1001f);
			m_mash = TextureContainer.GetSprite("images/Launcher/mashVirtual", SceneRenderer.GetCameraPosition(), 1001f);
		}
		m_mash.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/Launcher/mashNorm");
		m_mash.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_getReady.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/Launcher/getReadyNorm");
		m_getReady.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_Button.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/Launcher/giantBnorm");
		m_Button.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_timerBar = TextureContainer.GetSprite("images/Launcher/timer", SceneRenderer.GetCameraPosition() - new Vector2(300f, 0f), 1000f);
		m_timerBar.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/Launcher/timerNorm");
		m_timerBar.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_timerBar.SurfaceScale = new Vector2(25f, 428.57144f);
		m_Button.SurfaceScale = new Vector2(300f);
		m_timerBar.Color = Color.Lime;
		m_mash.Alpha = 0f;
		m_popSound = SoundManager.GetSoundEffect("sounds/pop");
		m_gruntSound = SoundManager.GetSoundEffect("sounds/freesound/grunt_15921__pitx__Grito_07");
		m_breathIn = SoundManager.GetSoundEffect("sounds/freesound/breathin_7805__hanstimm__z1");
		m_breathOut = SoundManager.GetSoundEffect("sounds/freesound/breathout_7805__hanstimm__z1");
	}

	public void Draw(TimeTracker gameTime)
	{
		m_timerBar.Draw(gameTime);
		m_getReady.Draw(gameTime);
		m_Button.Draw(gameTime);
		m_mash.Draw(gameTime);
		m_sprBack.Draw(gameTime);
		m_sprMeter.Draw(gameTime);
		m_light.Draw(gameTime);
	}

	public void Update(TimeTracker gameTime)
	{
		m_timerBar.Position = SceneRenderer.GetCameraPosition() + new Vector2(240f, 0f);
		m_timerBar.SurfaceScale = new Vector2(m_timerBar.SurfaceScale.X, (float)(6000 - Math.Max(0, m_iTimer - 2000)) / 14f);
		m_getReady.Position = SceneRenderer.GetCameraPosition() + new Vector2(0f, 150f);
		m_Button.Position = SceneRenderer.GetCameraPosition() - new Vector2(0f, 100f);
		int iTimer = m_iTimer;
		m_iTimer += gameTime.ElapsedMilli;
		if (iTimer <= 6700 && m_iTimer > 6700)
		{
			SoundManager.AddSoundToPlay(m_gruntSound, 1f, 0f, 0);
		}
		if (iTimer <= 8000 && m_iTimer > 8000)
		{
			SoundManager.AddSoundToPlay(m_popSound, 1f, 0f, 0);
		}
		if (m_iTimer > 1000 && m_iTimer < 6000 && m_iTimer / 1000 != iTimer / 1000)
		{
			SoundManager.AddSoundToPlay(m_breathIn, 1f, 0f, 0);
		}
		if (m_iTimer > 1000 && m_iTimer < 6000 && (m_iTimer - 500) / 1000 != (iTimer - 500) / 1000)
		{
			SoundManager.AddSoundToPlay(m_breathOut, 1f, 0f, 0);
		}
		if (m_iTimer > 2000)
		{
			m_mash.Alpha += gameTime.FractionOfSecond * 10f;
			if (m_mash.Alpha > 1f)
			{
				m_mash.Alpha = 1f;
			}
			m_Button.Position = SceneRenderer.GetCameraPosition() - new Vector2(0f, 100f) + new Vector2(SceneRenderer.GetRand(-10f, 10f), SceneRenderer.GetRand(-10f, 10f));
			m_mash.Position = new Vector2(0f, 150f) + SceneRenderer.GetCameraPosition() + new Vector2(SceneRenderer.GetRand(-5f, 5f), SceneRenderer.GetRand(-5f, 5f));
			m_mash.Rotation = SceneRenderer.GetRand(-0.03f, 0.03f);
			m_Button.Rotation = SceneRenderer.GetRand(-0.3f, 0.3f);
			m_getReady.Alpha -= gameTime.FractionOfSecond * 10f;
			if (m_getReady.Alpha < 0f)
			{
				m_getReady.Alpha = 0f;
			}
		}
		m_light.color.A = (byte)Math.Max(0f, (float)(int)m_light.color.A - gameTime.FractionOfSecond * 64f);
		if (Pow > 4f)
		{
			Pow -= gameTime.FractionOfSecond * 50f;
		}
		m_sprMeter.GetSpriteImage().Height = m_fPow / 3f;
		m_sprMeter.SurfaceScale = new Vector2(m_sprMeter.SurfaceScale.X, m_sprMeter.GetSpriteImage().Height);
		m_sprMeter.GetSpriteImage().Y = (int)((float)m_sprMeter.GetSpriteImage().GetSpritePage().DiffuseTex.Height - m_sprMeter.GetSpriteImage().Height);
		m_sprMeter.RecalcTexCoordinates();
		m_sprMeter.Position = new Vector2(1050f, 200f + (float)m_sprMeter.GetSpriteImage().GetSpritePage().DiffuseTex.Height / 2f - m_sprMeter.SurfaceScale.Y / 2f);
	}

	public void HandleInput(TimeTracker gameTime)
	{
		if (m_iTimer <= 2000)
		{
			return;
		}
		if (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B))
		{
			m_fPow += 30f;
			if (m_fPow / 3f > (float)m_sprMeter.GetSpriteImage().GetSpritePage().DiffuseTex.Height)
			{
				m_fPow = m_sprMeter.GetSpriteImage().GetSpritePage().DiffuseTex.Height * 3;
			}
			m_meterParticle.Position = m_sprMeter.Position - new Vector2(0f, m_sprMeter.SurfaceScale.Y / 2f);
			float num = m_sprMeter.SurfaceScale.Y / (float)m_sprMeter.GetSpriteImage().GetSpritePage().DiffuseTex.Height;
			Color color = Color.White;
			Color color2 = Color.White;
			if (num < 0.5f)
			{
				num /= 0.5f;
				color = new Color(Color.Lime.ToVector3() * (1f - num) + Color.Yellow.ToVector3() * num);
				color2 = new Color(Color.Green.ToVector3() * (1f - num) + Color.Goldenrod.ToVector3() * num);
			}
			else
			{
				num = (num - 0.5f) / 0.5f;
				color = new Color(Color.Yellow.ToVector3() * (1f - num) + Color.Red.ToVector3() * num);
				color2 = new Color(Color.Goldenrod.ToVector3() * (1f - num) + Color.Maroon.ToVector3() * num);
			}
			Color color3 = color;
			color3.A = (byte)Math.Min(255, m_light.color.A + 60);
			m_light.color = color3;
			m_light.pos = new Vector3(m_meterParticle.Position.X, 0f - m_meterParticle.Position.Y, 200f);
			m_meterParticle.SetColors(color, color2, color, color2);
			m_meterParticle.CreateBurst(5);
		}
		ControlManager.SetFlatVibration(ControlManager.ActiveMenuIndex, (float)(m_iTimer - 2000) / 6000f);
	}

	public bool IsCompleted()
	{
		return m_iTimer > 8000;
	}
}
