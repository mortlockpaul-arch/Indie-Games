using System;
using System.Collections.Generic;
using System.Linq;
using BabyMakerExtreme2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MusicPlayer;
using PlayObjects;
using Renderer;

namespace Screens;

public class TitleScreen : Screen
{
	private const int EXTREME_GROW_TIME = 2000;

	private const int TWO_GROW_TIME = 1000;

	private const int EXTREME_LINGER_TIME = 1500;

	private SpriteInstance m_bg;

	private List<SineModulatedSprite> m_spr;

	private RenderLight m_light;

	private RenderLight m_light2;

	private int timer;

	private SineModulatedSprite m_extreme;

	private SpriteInstance m_extremeGlow;

	private SineModulatedSprite m_two;

	private SpriteInstance m_twoGlow;

	private List<SpriteInstance> m_explode;

	private bool m_bShowLogos;

	private Player m_player;

	private SoundEffect m_explodeSound;

	private string m_pressA;

	private SpriteInstance m_AButton;

	public TitleScreen(bool showLogos, Player player)
		: base(updateParent: false, drawParent: false, inputParent: false)
	{
		m_player = player;
		m_bShowLogos = showLogos;
		m_bg = TextureContainer.GetSprite("images/mainMenu/bg", default(Vector2), -100f);
		m_bg.SurfaceScale = SceneRenderer.GetScreenDim();
		m_bg.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/mainMenu/bgNorm");
		m_bg.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		SpriteInstance sprite = TextureContainer.GetSprite("images/logo", default(Vector2), DepthConsts.LOGO_DEPTH);
		sprite.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/logoNorm");
		sprite.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		_ = sprite.SurfaceScale;
		m_spr = new List<SineModulatedSprite>();
		sprite = TextureContainer.GetSprite("images/logo", new Rectangle(76, 101, 207, 226), new Vector2(-800f, -1000f), DepthConsts.LOGO_DEPTH);
		m_spr.Add(new SineModulatedSprite(sprite, 1000, sprite.WidthScale * 0.8f * 1.2f, sprite.WidthScale * 1.2f, invertWidthHeight: true));
		sprite = TextureContainer.GetSprite("images/logo", new Rectangle(283, 101, 207, 226), new Vector2(-400f, -1000f), DepthConsts.LOGO_DEPTH + 0.1f);
		m_spr.Add(new SineModulatedSprite(sprite, 1000, sprite.WidthScale * 0.8f * 1.2f, sprite.WidthScale * 1.2f, invertWidthHeight: true));
		sprite = TextureContainer.GetSprite("images/logo", new Rectangle(490, 101, 207, 226), new Vector2(0f, -1000f), DepthConsts.LOGO_DEPTH + 0.2f);
		m_spr.Add(new SineModulatedSprite(sprite, 1000, sprite.WidthScale * 0.8f * 1.2f, sprite.WidthScale * 1.2f, invertWidthHeight: true));
		sprite = TextureContainer.GetSprite("images/logo", new Rectangle(697, 101, 207, 226), new Vector2(400f, -1000f), DepthConsts.LOGO_DEPTH);
		m_spr.Add(new SineModulatedSprite(sprite, 1000, sprite.WidthScale * 0.8f * 1.2f, sprite.WidthScale * 1.2f, invertWidthHeight: true));
		sprite = TextureContainer.GetSprite("images/logo", new Rectangle(4, 566, 219, 217), new Vector2(-800f, -600f), DepthConsts.LOGO_DEPTH - 0.5f);
		m_spr.Add(new SineModulatedSprite(sprite, 1000, sprite.WidthScale * 0.8f * 1.2f, sprite.WidthScale * 1.2f, invertWidthHeight: true));
		sprite = TextureContainer.GetSprite("images/logo", new Rectangle(223, 567, 189, 215), new Vector2(-400f, -600f), DepthConsts.LOGO_DEPTH + 0.1f - 0.5f);
		m_spr.Add(new SineModulatedSprite(sprite, 1000, sprite.WidthScale * 0.8f * 1.2f, sprite.WidthScale * 1.2f, invertWidthHeight: true));
		sprite = TextureContainer.GetSprite("images/logo", new Rectangle(413, 567, 200, 214), new Vector2(0f, -600f), DepthConsts.LOGO_DEPTH + 0.2f - 0.5f);
		m_spr.Add(new SineModulatedSprite(sprite, 1000, sprite.WidthScale * 0.8f * 1.2f, sprite.WidthScale * 1.2f, invertWidthHeight: true));
		sprite = TextureContainer.GetSprite("images/logo", new Rectangle(627, 571, 181, 213), new Vector2(400f, -600f), DepthConsts.LOGO_DEPTH + 0.1f - 0.5f);
		m_spr.Add(new SineModulatedSprite(sprite, 1000, sprite.WidthScale * 0.8f * 1.2f, sprite.WidthScale * 1.2f, invertWidthHeight: true));
		sprite = TextureContainer.GetSprite("images/logo", new Rectangle(814, 569, 206, 212), new Vector2(800f, -600f), DepthConsts.LOGO_DEPTH - 0.5f);
		m_spr.Add(new SineModulatedSprite(sprite, 1000, sprite.WidthScale * 0.8f * 1.2f, sprite.WidthScale * 1.2f, invertWidthHeight: true));
		m_light = new RenderLight(new Vector3(300f, -100f, 130f), 1f, 300, Color.White);
		m_light.pos = new Vector3(SceneRenderer.GetCameraPosition().X - 200f, 0f - SceneRenderer.GetCameraPosition().Y + 250f, 600f);
		m_light.falloff = 0.1f;
		m_light.range = 1700;
		m_light.color = new Color(0.3f, 0.3f, 0.15f);
		m_light2 = new RenderLight(new Vector3(300f, -100f, 130f), 1f, 300, Color.White);
		m_light2.pos = new Vector3(SceneRenderer.GetCameraPosition().X - 200f, 0f - SceneRenderer.GetCameraPosition().Y + 250f, 600f);
		m_light2.falloff = 0.2f;
		m_light2.range = 1200;
		m_light2.color = new Color(0.3f, 0.3f, 0.15f);
		m_extreme = new SineModulatedSprite(TextureContainer.GetSprite("images/extremeText", default(Vector2), DepthConsts.LOGO_DEPTH - 1f), 4000, 1f, 1024f, invertWidthHeight: false);
		m_extreme.Sprite.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/extremeNorm");
		m_extreme.Sprite.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_extreme.Sprite.FlatColor = false;
		m_extreme.Percent = 0.25f;
		m_extremeGlow = TextureContainer.GetSprite("images/extremeTextGlow", default(Vector2), DepthConsts.LOGO_DEPTH - 1f + 0.1f);
		m_extremeGlow.Position = m_extreme.Sprite.Position;
		m_extremeGlow.SurfaceScale = m_extreme.Sprite.SurfaceScale;
		m_extremeGlow.Rotation = m_extreme.Sprite.Rotation;
		m_extremeGlow.FlatColor = true;
		m_extremeGlow.Additive = true;
		m_two = new SineModulatedSprite(TextureContainer.GetSprite("images/two", default(Vector2), DepthConsts.LOGO_DEPTH - 2f), 2000, 300f, 600f, invertWidthHeight: false);
		m_two.Sprite.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/twoNorm");
		m_two.Sprite.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_two.Percent = 0.25f;
		m_twoGlow = TextureContainer.GetSprite("images/twoGlow", default(Vector2), DepthConsts.LOGO_DEPTH - 2f + 0.1f);
		m_twoGlow.Position = m_two.Sprite.Position;
		m_twoGlow.SurfaceScale = m_two.Sprite.SurfaceScale;
		m_twoGlow.Rotation = m_two.Sprite.Rotation;
		m_twoGlow.FlatColor = true;
		m_twoGlow.Additive = true;
		timer = 0;
		m_explode = new List<SpriteInstance>();
		for (int i = 0; i < 5; i++)
		{
			m_explode.Add(TextureContainer.GetSprite("images/explode1", default(Vector2), DepthConsts.LOGO_DEPTH + 1f));
			m_explode.Last().FlatColor = true;
			m_explode.Last().Additive = true;
			m_explode.Last().Alpha = 0f;
			m_explode.Last().WidthScale = 1f;
			m_explode.Last().Rotation = SceneRenderer.GetRand(0f, (float)Math.PI * 2f);
		}
		m_explodeSound = SoundManager.GetSoundEffect("sounds/imphenzia/imphenzia_soundtrack_explosion15");
		m_pressA = "Press         to begin";
		m_AButton = TextureContainer.GetSprite("images/buttons/abxy", new Rectangle(50, 47, 50, 47), default(Vector2), DepthConsts.LOGO_DEPTH + 100f);
		m_AButton.Alpha = 0f;
		m_AButton.FlatColor = true;
	}

	public override void Update(TimeTracker gameTime)
	{
		if (ControlManager.ActiveMenuIndex >= 0)
		{
			if (m_bShowLogos)
			{
				for (int i = 0; i < m_spr.Count; i++)
				{
					m_spr[i].Sprite.Position += gameTime.FractionOfSecond * 800f * new Vector2((float)(i % 5) - 2.5f, -1 + 2 * (i / 5));
					m_spr[i].Sprite.Rotation += gameTime.FractionOfSecond * 6f * (float)Math.Pow(-1.0, i);
					m_spr[i].Sprite.WidthScale *= 1f + gameTime.FractionOfSecond * 5f;
					m_spr[i].Sprite.Alpha -= gameTime.FractionOfSecond * 2f;
				}
				if (timer < 3000)
				{
					if (timer < 2400)
					{
						m_extreme.Update(gameTime);
					}
				}
				else
				{
					if (timer > 3500)
					{
						m_extreme.Sprite.Rotation += gameTime.FractionOfSecond * 6f;
						m_extreme.Sprite.WidthScale += gameTime.FractionOfSecond * 1000f;
						m_extreme.Sprite.Alpha -= gameTime.FractionOfSecond * 2f;
					}
					if (timer < 4500)
					{
						m_two.Update(gameTime);
					}
				}
				m_extremeGlow.Position = m_extreme.Sprite.Position;
				m_extremeGlow.SurfaceScale = m_extreme.Sprite.SurfaceScale;
				m_extremeGlow.Rotation = m_extreme.Sprite.Rotation;
				m_extremeGlow.Alpha = m_extreme.Sprite.Alpha;
				m_twoGlow.Position = m_two.Sprite.Position;
				m_twoGlow.SurfaceScale = m_two.Sprite.SurfaceScale;
				m_twoGlow.Rotation = m_two.Sprite.Rotation;
				m_twoGlow.Alpha = m_two.Sprite.Alpha;
				for (int j = 0; j < m_explode.Count; j++)
				{
					m_explode[j].WidthScale += gameTime.FractionOfSecond * 2000f;
					m_explode[j].Alpha -= gameTime.FractionOfSecond * (0.3f * (float)(j + 1));
				}
				if (timer >= 5000 && ScreenStorage.PeekScreen() == this)
				{
					new MenuScreen(m_player);
				}
			}
			else if (ScreenStorage.PeekScreen() == this)
			{
				new MenuScreen(m_player);
			}
		}
		else
		{
			for (int k = 0; k < m_spr.Count; k++)
			{
				m_spr[k].Update(gameTime);
				if (k < 4)
				{
					if (k * 200 < timer)
					{
						m_spr[k].Sprite.Position = new Vector2(((float)(k % 4) - 2.5f) * 180f + 200f, k / 4 * 400 - 780) + new Vector2(0f, 700f * (float)Math.Sin(Math.Min(1.970796332755361, (float)(timer - k * 200) / 500f)));
					}
				}
				else if (k * 200 < timer)
				{
					m_spr[k].Sprite.Position = new Vector2(((float)((k + 1) % 5) - 2.5f) * 180f + 100f, (k + 1) / 5 * 400 - 920) + new Vector2(0f, 700f * (float)Math.Sin(Math.Min(1.970796332755361, (float)(timer - (k + 1) * 200) / 500f)));
				}
			}
		}
		m_light.pos = new Vector3(SceneRenderer.GetCameraPosition().X, 0f - SceneRenderer.GetCameraPosition().Y, 700f);
		m_light2.pos = new Vector3(SceneRenderer.GetCameraPosition().X, 0f - SceneRenderer.GetCameraPosition().Y, 500f);
		timer += gameTime.ElapsedMilli;
		m_light.pos += 400f * new Vector3((float)Math.Sin((float)timer / 300f), (float)Math.Cos((float)timer / 300f), 0f);
		m_light2.pos += 400f * new Vector3((float)Math.Sin(Math.PI + (double)((float)timer / 300f)), (float)Math.Cos(Math.PI + (double)((float)timer / 300f)), 0f);
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_bg.Draw(gameTime);
		if (m_bShowLogos)
		{
			if (ControlManager.ActiveMenuIndex < 0)
			{
				m_AButton.Alpha += gameTime.FractionOfSecond * 0.3f;
				if (m_AButton.Alpha > 1f)
				{
					m_AButton.Alpha = 1f;
				}
				Color white = Color.White;
				white *= m_AButton.Alpha;
				SceneRenderer.DrawString(fonts.BUTTON_FONT, m_pressA, SceneRenderer.GetCameraPosition() + new Vector2(-180f, 230f), white, DepthConsts.LOGO_DEPTH);
				m_AButton.Position = SceneRenderer.GetCameraPosition() + new Vector2(-45f, 255f);
				m_AButton.Draw(gameTime);
			}
			for (int i = 0; i < m_spr.Count; i++)
			{
				m_spr[i].Draw(gameTime);
			}
			if (ControlManager.ActiveMenuIndex >= 0)
			{
				m_extreme.Draw(gameTime);
				m_extremeGlow.Draw(gameTime);
				if (timer > 3100)
				{
					m_two.Draw(gameTime);
					m_twoGlow.Draw(gameTime);
				}
				for (int j = 0; j < m_explode.Count; j++)
				{
					m_explode[j].Draw(gameTime);
				}
			}
		}
		m_light.Draw(gameTime);
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (ControlManager.ActiveMenuIndex < 0 && ControlManager.DetectInput() >= 0)
		{
			ControlManager.ActiveMenuIndex = ControlManager.DetectInput();
			SaveManager.LoadGlobalOptions();
			timer = 0;
			for (int i = 0; i < m_explode.Count; i++)
			{
				m_explode[i].Alpha = 0.5f;
			}
			SceneRenderer.Avatar = new AvatarHandler(ControlManager.ActiveMenuIndex);
			SoundManager.AddSoundToPlay(m_explodeSound, 1f, 0f, 0);
			Mp3MusicPlayer.Pause();
			Mp3MusicPlayer.PreLoadSong("sounds/incompetech/Big Rock");
			ControlManager.SetVibration(ControlManager.ActiveMenuIndex, 2f);
		}
	}

	public override void OnRegainFocus(string applicatorInfo)
	{
		if (applicatorInfo.Equals("Start"))
		{
			ScreenStorage.PopScreen("");
		}
	}
}
