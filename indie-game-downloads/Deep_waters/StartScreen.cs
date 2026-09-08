using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Deep_waters;

internal class StartScreen : GameScreen
{
	private bool changescreen;

	private InputState inputstate;

	private int pindex = -1;

	private bool pushstart;

	private SignedInGamer signedin;

	private bool guideshowonce;

	private Texture2D Title;

	private Texture2D Start;

	private ContentManager content;

	private float timer;

	private bool isvisible;

	public StartScreen()
	{
		inputstate = new InputState();
	}

	public override void LoadContent()
	{
		base.LoadContent();
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		Title = content.Load<Texture2D>("Sprites/Title");
		Start = content.Load<Texture2D>("Sprites/PressStart");
	}

	public override void UnloadContent()
	{
		if (base.ScreenManager.contentRepository != null)
		{
			base.ScreenManager.contentRepository.Dispose();
		}
		base.UnloadContent();
	}

	private void init()
	{
		changescreen = false;
		pindex = -1;
		pushstart = false;
		base.ControllingPlayer = null;
		signedin = null;
		guideshowonce = false;
		base.ScreenManager.Game.Components.Remove(base.ScreenManager.storageManager);
		base.ScreenManager.storageManager = new StorageManager(base.ScreenManager.Game);
		base.ScreenManager.Game.Components.Add(base.ScreenManager.storageManager);
	}

	public override void HandleInput(InputState input)
	{
		base.HandleInput(input);
		if (pindex != -1)
		{
			return;
		}
		for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
		{
			if ((!input.CurrentGamePadStates[(int)playerIndex].IsButtonDown(Buttons.Start) || !input.LastGamePadStates[(int)playerIndex].IsButtonUp(Buttons.Start)) && (!input.CurrentKeyboardStates[(int)playerIndex].IsKeyDown(Keys.Space) || !input.LastKeyboardStates[(int)playerIndex].IsKeyUp(Keys.Space)))
			{
				continue;
			}
			if (Gamer.SignedInGamers[playerIndex] == null)
			{
				if (!Guide.IsVisible)
				{
					Guide.ShowSignIn(1, onlineOnly: false);
				}
			}
			else if (Gamer.SignedInGamers[playerIndex].IsGuest)
			{
				if (!Guide.IsVisible)
				{
					Guide.ShowSignIn(1, onlineOnly: false);
				}
			}
			else
			{
				base.ControllingPlayer = playerIndex;
			}
		}
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		if (timer <= 600f)
		{
			timer += gameTime.ElapsedGameTime.Milliseconds;
		}
		else
		{
			timer = 0f;
			if (isvisible)
			{
				isvisible = false;
			}
			else
			{
				isvisible = true;
			}
		}
		if (base.ScreenManager.audioManager != null)
		{
			if (base.ScreenManager.audioManager.BGMCue == null)
			{
				base.ScreenManager.audioManager.PlayMusic("menu");
			}
			else if (base.ScreenManager.audioManager.BGMCue.IsStopped)
			{
				base.ScreenManager.audioManager.PlayMusic("menu");
			}
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (!base.IsActive)
		{
			return;
		}
		if (changescreen && pindex > -1 && !Guide.IsVisible)
		{
			base.ScreenManager.AddScreen(new MainMenuScreen("", new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height / 2 - 96)), null);
			UnloadContent();
			changescreen = false;
			return;
		}
		HandleInput(inputstate);
		if (base.ControllingPlayer.HasValue)
		{
			signedin = Gamer.SignedInGamers[base.ControllingPlayer.Value];
		}
		if (signedin != null && !signedin.IsGuest && !pushstart)
		{
			pushstart = true;
		}
		if (pushstart)
		{
			signedin = Gamer.SignedInGamers[base.ControllingPlayer.Value];
			if (signedin != null)
			{
				pindex = (int)base.ControllingPlayer.Value;
				changescreen = true;
			}
			pushstart = false;
		}
	}

	public void confirmwarning(IAsyncResult result)
	{
		int? num = Guide.EndShowMessageBox(result);
		int? num2 = num;
		if (num2.GetValueOrDefault() == 0 && num2.HasValue)
		{
			base.ScreenManager.RemoveScreen(this);
			base.ScreenManager.AddScreen(new MainMenuScreen("", new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height / 2 - 96)), null);
			UnloadContent();
			guideshowonce = false;
			changescreen = false;
			base.ScreenManager.settings.enablesaving = false;
		}
		else
		{
			init();
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
		SpriteBatch spriteBatch = new SpriteBatch(base.ScreenManager.GraphicsDevice);
		spriteBatch.Begin();
		spriteBatch.Draw(Title, new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2 - Title.Width / 2, 10f), Color.White);
		if (isvisible)
		{
			spriteBatch.Draw(Start, new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2 - Start.Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height - Start.Height - 60), Color.White);
		}
		spriteBatch.End();
	}
}
