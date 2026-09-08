using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Deep_waters;

internal class BuyScreen : GameScreen
{
	private ContentManager content;

	private bool changescreen;

	private InputState inputstate;

	private int pindex = -1;

	private SignedInGamer signedin;

	private bool Back;

	private bool somthingispressed;

	private Texture2D bg;

	private bool comefromgameplay;

	private SpriteBatch spriteBatch;

	public BuyScreen()
	{
		inputstate = new InputState();
	}

	public BuyScreen(bool comfgp)
	{
		comefromgameplay = comfgp;
		inputstate = new InputState();
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		bg = content.Load<Texture2D>("Background/buyscreen");
		base.LoadContent();
		spriteBatch = base.ScreenManager.SpriteBatch;
		spriteBatch.GraphicsDevice.BlendState = BlendState.AlphaBlend;
		spriteBatch.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		spriteBatch.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
	}

	public override void UnloadContent()
	{
		content.Unload();
		base.UnloadContent();
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
			if (input.CurrentGamePadStates[(int)playerIndex].IsButtonDown(Buttons.X) && input.LastGamePadStates[(int)playerIndex].IsButtonUp(Buttons.X))
			{
				if (Gamer.SignedInGamers[playerIndex] == null)
				{
					Guide.ShowSignIn(1, onlineOnly: false);
				}
				if (Gamer.SignedInGamers[playerIndex] != null)
				{
					base.ControllingPlayer = playerIndex;
					signedin = Gamer.SignedInGamers[base.ControllingPlayer.Value];
					if (signedin != null && Guide.IsTrialMode)
					{
						try
						{
							Guide.ShowMarketplace(signedin.PlayerIndex);
						}
						catch (GamerPrivilegeException)
						{
							Guide.ShowSignIn(1, onlineOnly: true);
						}
					}
				}
			}
			if (!somthingispressed && input.CurrentGamePadStates[(int)playerIndex].IsButtonDown(Buttons.B) && input.LastGamePadStates[(int)playerIndex].IsButtonUp(Buttons.B))
			{
				somthingispressed = true;
				Back = true;
			}
		}
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		if (Guide.IsTrialMode)
		{
			changescreen = true;
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (base.IsActive)
		{
			HandleInput(inputstate);
		}
		if (!Guide.IsTrialMode && changescreen)
		{
			if (!comefromgameplay)
			{
				LoadingScreen.Load(base.ScreenManager, true, null, new BackgroundMenu3D("Menu", ext: true), new MainMenuScreen("", new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height / 2)));
			}
			else
			{
				LoadingScreen.Load(base.ScreenManager, true, base.ControllingPlayer, new GameplayScreen(2, bossalreadyreach: false));
			}
			changescreen = false;
		}
		if (Back && somthingispressed)
		{
			LoadingScreen.Load(base.ScreenManager, true, null, new BackgroundMenu3D("Menu", ext: true), new MainMenuScreen("", new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height / 2)));
			Back = false;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0f, 0);
		spriteBatch.Begin();
		spriteBatch.Draw(bg, new Rectangle(0, 0, base.ScreenManager.graphics.PreferredBackBufferWidth, base.ScreenManager.graphics.PreferredBackBufferHeight), Color.White);
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
