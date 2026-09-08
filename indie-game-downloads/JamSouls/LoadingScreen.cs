using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

internal class LoadingScreen : GameScreen
{
	private const int LOADING_SCREEN_NUM = 2;

	private const float FADE_LATENCY = 0.8f;

	private bool loadingIsSlow;

	private bool otherScreensAreGone;

	private bool bStopLoading;

	private bool m_bStopInput;

	private GameScreen[] screensToLoad;

	private Vector2 MESSAGE_POS = new Vector2(800f, GameContext.TileSafeBottom);

	private Texture2D m_CurrentLoadingScreen;

	private string m_Message;

	private Color m_TextColor;

	private float m_fadeTimer;

	private byte FADE_SPEED = 4;

	private bool m_bFadeIn = true;

	private float m_KeyLatency = 1500f;

	private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow, GameScreen[] screensToLoad)
	{
		this.loadingIsSlow = loadingIsSlow;
		this.screensToLoad = screensToLoad;
		base.TransitionOnTime = TimeSpan.FromSeconds(1.0);
		m_TextColor = Color.Black;
		m_TextColor.A = 0;
		m_Message = TextManager.GetText(TextID.LOADING);
		base.IsPopup = false;
	}

	public static void Load(ScreenManager screenManager, bool loadingIsSlow, PlayerIndex? controllingPlayer, params GameScreen[] screensToLoad)
	{
		GameScreen[] screens = screenManager.GetScreens();
		foreach (GameScreen gameScreen in screens)
		{
			gameScreen.ExitScreen();
		}
		LoadingScreen screen = new LoadingScreen(screenManager, loadingIsSlow, screensToLoad);
		screenManager.AddScreen(screen, controllingPlayer);
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		m_KeyLatency -= gameTime.ElapsedGameTime.Milliseconds;
		MESSAGE_POS.X = GameContext.TileSafeRight;
		if (m_CurrentLoadingScreen == null)
		{
			m_CurrentLoadingScreen = base.ScreenManager.Game.Content.Load<Texture2D>("Common/LoadingScreen/Loading1");
		}
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
		if (!otherScreensAreGone)
		{
			return;
		}
		if (!bStopLoading)
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
		else
		{
			if (m_bStopInput)
			{
				return;
			}
			for (int k = 0; k < InputManager.GamerIndex.Length; k++)
			{
				if (InputManager.GamerIndex[k] == -1 || m_bStopInput || InputManager.GetKeyState((PlayerIndex)InputManager.GamerIndex[k], 4) != ButtonState.Pressed || !(m_KeyLatency < 0f))
				{
					continue;
				}
				base.ScreenManager.RemoveScreen(this);
				m_bStopInput = true;
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
	}

	public override void Draw(GameTime gameTime)
	{
		if (base.ScreenState == ScreenState.Active && base.ScreenManager.GetScreens().Length == 1)
		{
			otherScreensAreGone = true;
		}
		if (loadingIsSlow && base.ScreenState == ScreenState.Active)
		{
			base.ScreenManager.GraphicsDevice.Clear(Color.Black);
			SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
			_ = base.ScreenManager.BubbleFontVeryBig;
			new Color(255, 255, 255, base.TransitionAlpha);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			if (m_CurrentLoadingScreen != null)
			{
				spriteBatch.Draw(m_CurrentLoadingScreen, Vector2.Zero, Color.White);
			}
			base.ScreenManager.DrawText(base.ScreenManager.GoBoomBig, ref MESSAGE_POS, m_Message, ScreenManager.TextOrigin.bottom_right, m_TextColor);
			spriteBatch.End();
		}
	}
}
