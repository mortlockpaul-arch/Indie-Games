using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace JamSouls;

internal class BumperScreen : GameState
{
	private VideoPlayer m_IntroPlayer;

	private Video m_Intro;

	private Matrix m_ScaleMatrix;

	public BumperScreen()
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(1.0);
		SignedInGamer.SignedIn += base.SignedInGamer_SignedIn;
		SignedInGamer.SignedOut += base.SignedInGamer_SignedOut;
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		m_Intro = content.Load<Video>("Common/Intro");
		m_IntroPlayer = new VideoPlayer();
		m_IntroPlayer.Play(m_Intro);
		float x = (float)base.ScreenManager.GraphicsDevice.Viewport.Width / (float)m_Intro.Width;
		float y = (float)base.ScreenManager.GraphicsDevice.Viewport.Height / (float)m_Intro.Height;
		GameContext.TileSafeTop = base.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Top;
		GameContext.TileSafeBottom = base.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Bottom;
		GameContext.TileSafeRight = base.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Right;
		GameContext.TileSafeLeft = base.ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Left;
		m_ScaleMatrix = Matrix.CreateScale(new Vector3(x, y, 0f));
		base.ScreenManager.Game.ResetElapsedTime();
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (m_IntroPlayer.IsDisposed)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i <= 3; i++)
		{
			if (GamePad.GetState((PlayerIndex)i).IsConnected && GamePad.GetState((PlayerIndex)i).IsButtonDown(Buttons.Start))
			{
				flag = true;
			}
		}
		if (flag || m_IntroPlayer.State == MediaState.Stopped)
		{
			m_IntroPlayer.Stop();
			m_IntroPlayer.Dispose();
			LoadingScreen.Load(base.ScreenManager, false, PlayerIndex.One, new LogoScreen());
		}
	}

	public override void HandleInput()
	{
		base.HandleInput();
	}

	public override void Draw(GameTime gameTime)
	{
		if (!m_IntroPlayer.IsDisposed)
		{
			SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, m_ScaleMatrix);
			spriteBatch.Draw(m_IntroPlayer.GetTexture(), Vector2.Zero, Color.White);
			spriteBatch.End();
		}
		if (base.TransitionPosition > 0f)
		{
			base.ScreenManager.FadeBackBufferToBlack(255 - base.TransitionAlpha);
		}
	}
}
