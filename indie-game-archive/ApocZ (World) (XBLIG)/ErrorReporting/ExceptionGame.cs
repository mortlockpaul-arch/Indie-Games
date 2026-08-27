using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ErrorReporting;

public class ExceptionGame : Game
{
	private const string gamerTag = "SKreationDev2";

	private const string email = "Kevin.Kelley@ApocZ.com";

	private const string ActionExit = "Exit";

	private const string ActionSendMessage = "SendMessage";

	private GameInput input;

	private readonly Exception exception;

	private SpriteBatch batch;

	private SpriteFont font;

	private bool? isSendMessageAllowed = null;

	private PlayerIndex? controllingPlayer = null;

	public ExceptionGame(Exception e)
	{
		GraphicsDeviceManager graphicsDeviceManager = new GraphicsDeviceManager(this)
		{
			PreferredBackBufferWidth = 1280,
			PreferredBackBufferHeight = 720
		};
		exception = e;
		base.Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		if (!GamerServicesDispatcher.IsInitialized)
		{
			GamerServicesDispatcher.Initialize(base.Services);
		}
		GamerServicesDispatcher.WindowHandle = base.Window.Handle;
		base.Initialize();
	}

	protected override void LoadContent()
	{
		batch = new SpriteBatch(base.GraphicsDevice);
		font = base.Content.Load<SpriteFont>("Debug");
		input = new GameInput();
		input.AddGamePadInput("Exit", Buttons.B, isReleasedPreviously: true);
		input.AddKeyboardInput("Exit", Keys.B, isReleasedPreviously: true);
		input.AddGamePadInput("SendMessage", Buttons.A, isReleasedPreviously: true);
		input.AddKeyboardInput("SendMessage", Keys.A, isReleasedPreviously: true);
	}

	protected override void Update(GameTime gameTime)
	{
		GamerServicesDispatcher.Update();
		input.BeginUpdate();
		if (input.IsPressed("Exit", null))
		{
			Exit();
		}
		if (input.IsPressed("SendMessage", null, out var theControllingPlayer))
		{
			SendMessage(theControllingPlayer);
		}
		input.EndUpdate();
		base.Update(gameTime);
	}

	private void SendMessage(PlayerIndex theControllingPlayer)
	{
		if (!Guide.IsVisible && IsSendMessageAllowed(theControllingPlayer))
		{
			string text = "Error Report: " + exception.ToString();
			if (text.Length >= 255)
			{
				text = text.Substring(0, 255);
			}
			Guide.ShowComposeMessage(theControllingPlayer, text, null);
		}
	}

	private bool IsSendMessageAllowed(PlayerIndex theControllingPlayer)
	{
		if (!isSendMessageAllowed.HasValue || theControllingPlayer != controllingPlayer)
		{
			controllingPlayer = theControllingPlayer;
			SignedInGamer signedInGamer = Gamer.SignedInGamers[theControllingPlayer];
			if (signedInGamer == null || !signedInGamer.IsSignedInToLive || signedInGamer.Privileges.AllowCommunication != GamerPrivilegeSetting.Everyone)
			{
				isSendMessageAllowed = false;
			}
			else
			{
				isSendMessageAllowed = true;
			}
		}
		return isSendMessageAllowed.Value;
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.Black);
		Vector2 position = new Vector2(base.GraphicsDevice.Viewport.TitleSafeArea.X, base.GraphicsDevice.Viewport.TitleSafeArea.Y);
		batch.Begin();
		batch.DrawString(font, "ApocZ has encountered an unexpected error and had", position, Color.White);
		position.Y += 24f;
		batch.DrawString(font, "to shut down. We are sorry for the inconvenience.", position, Color.White);
		if (!isSendMessageAllowed.HasValue || isSendMessageAllowed == true)
		{
			position.Y += 48f;
			batch.DrawString(font, "Press A to send the error message to the developer.", position, Color.White);
			position.Y += 24f;
			batch.DrawString(font, "You must choose to manually enter the gamertag", position, Color.White);
			position.Y += 24f;
			batch.DrawString(font, "'SKreationDev2' to send the message to.", position, Color.White);
		}
		position.Y += 48f;
		batch.DrawString(font, "You can send information about this error message ", position, Color.White);
		position.Y += 24f;
		batch.DrawString(font, "to Kevin.Kelley@ApocZ.com", position, Color.White);
		position.Y += 24f;
		batch.DrawString(font, "Press B to Exit", position, Color.White);
		position.Y += 48f;
		batch.DrawString(font, exception.ToString(), position, Color.White);
		batch.End();
		base.Draw(gameTime);
	}
}
