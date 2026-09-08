using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Deep_waters;

public class CreditsScreen : GameScreen
{
	private ContentManager content;

	private SpriteFont gameFont;

	private Vector2 position = new Vector2(360f, 800f);

	private float creditspeed = 0.1f;

	private bool menureturn;

	private bool menure;

	private List<string> credits;

	private Texture2D bg;

	private Video vid;

	private VideoPlayer vp;

	private float pauseAlpha;

	public CreditsScreen()
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
		credits = new List<string>();
		credits.Add("Dead Sea");
		credits.Add("from an idea of");
		credits.Add("Claudio Battiato\n");
		credits.Add("Concept Artist");
		credits.Add("Claudio Battiato\n");
		credits.Add("Produced by");
		credits.Add("Brave men games\n");
		credits.Add("Developed by");
		credits.Add("Brave men games\n");
		credits.Add("Programmer");
		credits.Add("Vittorio Messina\n");
		credits.Add("Graphic Artist");
		credits.Add("Vittorio Messina\n");
		credits.Add("Audio editing");
		credits.Add("Vittorio Messina\n");
		credits.Add("Original Soundtrack");
		credits.Add("Vittorio Messina\n");
		credits.Add("Special Thanks\n");
		credits.Add("Teresa");
		credits.Add("Giulia");
		credits.Add("and all peoples that made");
		credits.Add("possible the making of");
		credits.Add("this Game");
		credits.Add("Follow us at");
		credits.Add("bmgames.altervista.org");
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		gameFont = content.Load<SpriteFont>("Font/MenuSelected");
		vp = new VideoPlayer();
		vid = content.Load<Video>("Video/credits");
		Thread.Sleep(1000);
		base.ScreenManager.Game.ResetElapsedTime();
		base.ScreenManager.audioManager.PlayMusic("credits");
		vp.IsLooped = true;
		vp.Play(vid);
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen: false);
		if (coveredByOtherScreen)
		{
			pauseAlpha = Math.Min(pauseAlpha + 1f / 32f, 1f);
		}
		else
		{
			pauseAlpha = Math.Max(pauseAlpha - 1f / 32f, 0f);
		}
		if (base.IsActive)
		{
			new Vector2((float)(base.ScreenManager.GraphicsDevice.Viewport.Width / 2) - gameFont.MeasureString("Insert Gameplay Here").X / 2f, 200f);
			position.Y -= creditspeed * (float)gameTime.ElapsedGameTime.Milliseconds;
			if (menureturn && !menure)
			{
				base.ScreenManager.audioManager.stopMusic();
				LoadingScreen.Load(base.ScreenManager, true, null, new BackgroundMenu3D("Menu", ext: true), new MainMenuScreen("", new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height / 2)));
				menure = true;
			}
		}
	}

	public override void HandleInput(InputState input)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		int value = (int)base.ControllingPlayer.Value;
		_ = input.CurrentKeyboardStates[value];
		GamePadState gamePadState = input.CurrentGamePadStates[value];
		if (!gamePadState.IsConnected)
		{
			_ = input.GamePadWasConnected[value];
		}
		if (input.IsPauseGame(base.ControllingPlayer))
		{
			base.ScreenManager.audioManager.stopMusic();
			LoadingScreen.Load(base.ScreenManager, true, null, new BackgroundMenu3D("Menu", ext: true), new MainMenuScreen("", new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height / 2)));
		}
		else if (gamePadState.IsButtonDown(Buttons.A))
		{
			creditspeed = 0.2f;
		}
		else
		{
			creditspeed = 0.05f;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		if (base.ScreenState == ScreenState.Hidden)
		{
			return;
		}
		base.ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0f, 0);
		bg = vp.GetTexture();
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		spriteBatch.Begin();
		if (bg != null)
		{
			spriteBatch.Draw(bg, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), Color.White);
		}
		Vector2 vector = position;
		for (int i = 0; i < credits.Count; i++)
		{
			spriteBatch.DrawString(gameFont, credits[i], new Vector2(spriteBatch.GraphicsDevice.Viewport.Width / 2, vector.Y), Color.White, 0f, new Vector2(gameFont.MeasureString(credits[i]).X / 2f, gameFont.MeasureString(credits[i]).Y / 2f), 0.75f, SpriteEffects.None, 1f);
			if (i == credits.Count - 1 && vector.Y < -50f)
			{
				menureturn = true;
			}
			vector.Y += gameFont.MeasureString(credits[i]).Y * 1.3f;
		}
		spriteBatch.End();
		if (base.TransitionPosition > 0f)
		{
			base.ScreenManager.FadeBackBufferToBlack(255f - base.TransitionAlpha);
		}
	}
}
