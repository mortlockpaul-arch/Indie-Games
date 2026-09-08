using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

internal class VersusLoadingScreen : GameScreen
{
	private const float FADE_LATENCY = 0.8f;

	private Vector2[] PORTRAIT_START = new Vector2[4]
	{
		new Vector2(0f, 0f),
		new Vector2(190f, 50f),
		new Vector2(212f, 50f),
		new Vector2(120f, 50f)
	};

	private Vector2[] PORTRAIT_OFFSET = new Vector2[4]
	{
		new Vector2(0f, 0f),
		new Vector2(640f, 0f),
		new Vector2(300f, 0f),
		new Vector2(260f, 0f)
	};

	private Vector2 LOAD_TEXT_POSITION = new Vector2(GameContext.TileSafeRight, GameContext.TileSafeBottom);

	private List<Sprite> m_CharPortrait = new List<Sprite>();

	private Sprite m_CharBlink;

	private Sprite m_UpLayer;

	private Sprite m_DownLayer;

	private Texture2D m_Background;

	private Effect m_GradiantShader;

	private List<AudioClip> m_BipOne = new List<AudioClip>();

	private AudioClip m_BipTwo;

	private Color m_AnimColor;

	private static bool m_VersusReady;

	private int m_CurrentPlayer;

	private float m_AnimfadeTimer;

	private int m_ScreenConfig;

	private float FADE_SPEED_ANIM = 6f;

	private float BLINK_DURATION = 300f;

	private float m_BlinkDuration;

	private float SCREEN_FADE = 500f;

	private float m_ScreenFade;

	private Color m_BackgroundColor;

	private Vector2 MessagePosition = new Vector2(640f, 650f);

	private string m_Message = "";

	private Color m_TextColor;

	private float m_fadeTimer;

	private byte FADE_SPEED = 4;

	private bool m_bFadeIn = true;

	private GameScreen[] screensToLoad;

	private bool loadingIsSlow;

	private bool otherScreensAreGone;

	private bool bStopLoading;

	private bool bStopInput;

	private VersusLoadingScreen(ScreenManager screenManager, bool loadingIsSlow, GameScreen[] screensToLoad)
	{
		this.loadingIsSlow = loadingIsSlow;
		this.screensToLoad = screensToLoad;
		base.TransitionOnTime = TimeSpan.FromSeconds(1.0);
		base.TransitionOffTime = TimeSpan.FromSeconds(1.0);
		m_TextColor = Color.White;
		base.IsPopup = false;
		m_Message = TextManager.GetText(TextID.LOADING);
	}

	public static void Load(ScreenManager screenManager, bool loadingIsSlow, PlayerIndex? controllingPlayer, params GameScreen[] screensToLoad)
	{
		GameScreen[] screens = screenManager.GetScreens();
		foreach (GameScreen gameScreen in screens)
		{
			gameScreen.ExitScreen();
		}
		VersusLoadingScreen screen = new VersusLoadingScreen(screenManager, loadingIsSlow, screensToLoad);
		screenManager.AddScreen(screen, controllingPlayer);
		m_VersusReady = !loadingIsSlow;
	}

	private void InitVersusScreen()
	{
		m_ScreenConfig = 0;
		for (int i = 0; i < GameContext.Pinfo.Length; i++)
		{
			if (GameContext.Pinfo[i].Controller != PlayerController.NONE && GameContext.Pinfo[i].SbireDef == PlayerConfig.SBIRE_DEF.NONE)
			{
				m_CharPortrait.Add(LoadSprite("MVS_" + PlayerConfig.CHARACTER_NAME[GameContext.Pinfo[i].CharacterIdx], GameState.GameAtlas.GAME));
				m_ScreenConfig++;
			}
		}
		m_UpLayer = LoadSprite("MVS_Up", GameState.GameAtlas.GAME);
		m_DownLayer = LoadSprite("MVS_Down", GameState.GameAtlas.GAME);
		m_CharBlink = LoadSprite("MVS_White", GameState.GameAtlas.GAME);
		for (int j = 0; j < m_ScreenConfig; j++)
		{
			m_BipOne.Add(new AudioClip("Versus_Screen_Bip_1"));
		}
		m_BipTwo = new AudioClip("Versus_Screen_Bip_2");
		m_CurrentPlayer = 0;
		m_Background = new Texture2D(base.ScreenManager.GraphicsDevice, 1280, 720);
		m_GradiantShader = base.ScreenManager.Game.Content.Load<Effect>("FX/PostProcess/Gradiant");
		Color color = PlayerConfig.CHARACTER_COLOR[GameContext.Pinfo[0].CharacterIdx];
		Color black = Color.Black;
		m_GradiantShader.Parameters["TopColor"].SetValue(new Vector4((float)(int)color.R / 255f, (float)(int)color.G / 255f, (float)(int)color.B / 255f, 1f));
		m_GradiantShader.Parameters["DownColor"].SetValue(new Vector4((float)(int)black.R / 255f, (float)(int)black.G / 255f, (float)(int)black.B / 255f, 1f));
		m_AnimColor = new Color(1f, 1f, 1f, 0f);
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (!otherScreensAreGone)
		{
			return;
		}
		if (!bStopLoading && m_VersusReady)
		{
			GameScreen[] array = screensToLoad;
			foreach (GameScreen gameScreen in array)
			{
				if (gameScreen != null)
				{
					gameScreen.ScreenManager = base.ScreenManager;
					gameScreen.LoadContent();
				}
			}
			bStopLoading = true;
			m_Message = TextManager.GetText(TextID.A_CONTINUE);
		}
		if (!loadingIsSlow)
		{
			GameScreen[] array2 = screensToLoad;
			foreach (GameScreen gameScreen2 in array2)
			{
				if (gameScreen2 != null)
				{
					base.ScreenManager.AddScreenWithoutLoad(gameScreen2, base.ControllingPlayer);
				}
			}
			base.ScreenManager.RemoveScreen(this);
			bStopLoading = true;
			base.ScreenManager.Game.ResetElapsedTime();
		}
		else if (!bStopInput && m_VersusReady)
		{
			for (int k = 0; k < InputManager.GamerIndex.Length; k++)
			{
				if (InputManager.GamerIndex[k] == -1 || InputManager.GetKeyState((PlayerIndex)InputManager.GamerIndex[k], 4) != ButtonState.Pressed)
				{
					continue;
				}
				base.ScreenManager.RemoveScreen(this);
				bStopInput = true;
				GameScreen[] array3 = screensToLoad;
				foreach (GameScreen gameScreen3 in array3)
				{
					if (gameScreen3 != null)
					{
						base.ScreenManager.AddScreenWithoutLoad(gameScreen3, base.ControllingPlayer);
					}
				}
				base.ScreenManager.Game.ResetElapsedTime();
			}
		}
		if (m_ScreenFade >= SCREEN_FADE)
		{
			if (otherScreensAreGone && m_BlinkDuration <= 0f)
			{
				if (m_CurrentPlayer < m_ScreenConfig)
				{
					float num = m_AnimfadeTimer / 1000f * FADE_SPEED_ANIM;
					if (num >= 1f)
					{
						m_BlinkDuration = BLINK_DURATION;
						m_AnimfadeTimer = 0f;
						num = 0f;
						m_CurrentPlayer++;
						if (m_CurrentPlayer >= m_ScreenConfig)
						{
							m_BipTwo.Play();
						}
						else
						{
							m_BipOne[m_CurrentPlayer - 1].Play();
						}
					}
					else
					{
						m_AnimfadeTimer += gameTime.ElapsedGameTime.Milliseconds;
					}
					m_AnimColor.A = (byte)MathHelper.Lerp(0f, 255f, num);
				}
				else
				{
					m_VersusReady = true;
					if (m_fadeTimer > 0.8f)
					{
						if (m_bFadeIn)
						{
							m_TextColor.A += FADE_SPEED;
						}
						else
						{
							m_TextColor.A -= FADE_SPEED;
						}
						if (m_TextColor.A >= 200)
						{
							m_TextColor.A = 200;
							m_bFadeIn = false;
						}
						if (m_TextColor.A <= 25)
						{
							m_TextColor.A = 25;
							m_bFadeIn = true;
						}
						m_fadeTimer = 0f;
					}
					m_fadeTimer += gameTime.ElapsedGameTime.Milliseconds;
				}
			}
			m_BlinkDuration -= gameTime.ElapsedGameTime.Milliseconds;
		}
		else
		{
			if (m_UpLayer == null)
			{
				InitVersusScreen();
			}
			m_BackgroundColor = Color.White;
			m_BackgroundColor.A = (byte)MathHelper.Lerp(0f, 255f, m_ScreenFade / SCREEN_FADE);
			m_ScreenFade += gameTime.ElapsedGameTime.Milliseconds;
			if (m_ScreenFade >= SCREEN_FADE)
			{
				m_BackgroundColor.A = byte.MaxValue;
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		if (base.ScreenState == ScreenState.Active && base.ScreenManager.GetScreens().Length == 1)
		{
			otherScreensAreGone = true;
		}
		if (!loadingIsSlow || base.ScreenState != ScreenState.Active || m_DownLayer == null)
		{
			return;
		}
		base.ScreenManager.GraphicsDevice.Clear(Color.Black);
		base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, m_GradiantShader);
		base.ScreenManager.SpriteBatch.Draw(m_Background, Vector2.Zero, Color.White);
		base.ScreenManager.SpriteBatch.End();
		base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		Vector2 position = Vector2.Zero;
		for (int i = 0; i < m_ScreenConfig; i++)
		{
			if (i <= m_CurrentPlayer)
			{
				position = ((i != 0) ? new Vector2(PORTRAIT_START[m_ScreenConfig - 1].X + PORTRAIT_OFFSET[m_ScreenConfig - 1].X * (float)i, PORTRAIT_START[m_ScreenConfig - 1].Y) : PORTRAIT_START[m_ScreenConfig - 1]);
			}
			if (i < m_CurrentPlayer)
			{
				m_CharPortrait[i].Draw(new Vector2(position.X, position.Y - 50f), Color.White);
			}
		}
		if (m_CurrentPlayer < m_ScreenConfig)
		{
			m_CharBlink.Draw(position, m_AnimColor);
		}
		m_UpLayer.Draw(Vector2.Zero, m_BackgroundColor);
		m_DownLayer.Draw(new Vector2(0f, 720 - m_DownLayer.Height), m_BackgroundColor);
		for (int j = 0; j < m_ScreenConfig; j++)
		{
			position = ((j != 0) ? new Vector2(PORTRAIT_START[m_ScreenConfig - 1].X + PORTRAIT_OFFSET[m_ScreenConfig - 1].X * (float)j, PORTRAIT_START[m_ScreenConfig - 1].Y) : PORTRAIT_START[m_ScreenConfig - 1]);
			position.X += m_CharPortrait[j].Width / 2;
			position.Y = GameContext.TileSafeTop;
			if (j < m_CurrentPlayer)
			{
				int characterIdx = GameContext.Pinfo[j].CharacterIdx;
				position.Y = GameContext.TileSafeTop;
				base.ScreenManager.DrawText(base.ScreenManager.GoBoomBig, ref position, TextManager.GetText((TextID)(48 + characterIdx)), ScreenManager.TextOrigin.top_center, PlayerConfig.CHARACTER_COLOR[characterIdx]);
				position.Y += 40f;
				base.ScreenManager.DrawText(base.ScreenManager.GoBoom, ref position, GameContext.Pinfo[j].Name, ScreenManager.TextOrigin.top_center, Color.White);
			}
		}
		if (m_ScreenFade >= SCREEN_FADE)
		{
			base.ScreenManager.DrawText(base.ScreenManager.GoBoomBig, ref LOAD_TEXT_POSITION, m_Message, ScreenManager.TextOrigin.bottom_right, m_TextColor);
		}
		base.ScreenManager.SpriteBatch.End();
	}
}
