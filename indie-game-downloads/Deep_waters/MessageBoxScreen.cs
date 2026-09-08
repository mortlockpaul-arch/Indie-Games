using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

internal class MessageBoxScreen : GameScreen
{
	private string message;

	private Texture2D gradientTexture;

	private Texture2D borderTexture;

	private Texture2D bordervTexture;

	public event EventHandler<PlayerIndexEventArgs> Accepted;

	public event EventHandler<PlayerIndexEventArgs> Cancelled;

	public MessageBoxScreen(string message)
		: this(message, includeUsageText: true)
	{
	}

	public MessageBoxScreen(string message, bool includeUsageText)
	{
		if (includeUsageText)
		{
			this.message = message + "\nA button = ok\nB button = cancel";
		}
		else
		{
			this.message = message;
		}
		base.IsPopup = true;
		base.TransitionOnTime = TimeSpan.FromSeconds(0.2);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.2);
	}

	public override void LoadContent()
	{
		ContentManager content = base.ScreenManager.Game.Content;
		gradientTexture = content.Load<Texture2D>("Sprites/gradient");
		borderTexture = content.Load<Texture2D>("Sprites/border");
		bordervTexture = content.Load<Texture2D>("Sprites/borderv");
	}

	public override void HandleInput(InputState input)
	{
		if (input.IsMenuSelect(base.ControllingPlayer, out var playerIndex))
		{
			if (Accepted != null)
			{
				Accepted(this, new PlayerIndexEventArgs(playerIndex));
			}
			ExitScreen();
		}
		else if (input.IsMenuCancel(base.ControllingPlayer, out playerIndex))
		{
			if (Cancelled != null)
			{
				Cancelled(this, new PlayerIndexEventArgs(playerIndex));
			}
			ExitScreen();
		}
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		SpriteFont font = base.ScreenManager.Font;
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2f / 3f);
		Viewport viewport = base.ScreenManager.GraphicsDevice.Viewport;
		Vector2 vector = new Vector2(viewport.Width, viewport.Height);
		Vector2 vector2 = font.MeasureString(message) * 0.5f;
		Vector2 position = (vector - vector2) / 2f;
		Rectangle destinationRectangle = new Rectangle((int)position.X - 32, (int)position.Y - 16, (int)vector2.X + 64, (int)vector2.Y + 32);
		Rectangle destinationRectangle2 = new Rectangle((int)position.X - 32, (int)position.Y - 16, (int)vector2.X + 64, borderTexture.Height);
		Rectangle destinationRectangle3 = new Rectangle((int)position.X - 32, (int)position.Y + 16 + (int)vector2.Y - borderTexture.Height, (int)vector2.X + 64, borderTexture.Height);
		Rectangle destinationRectangle4 = new Rectangle((int)position.X - 32, (int)position.Y - 16, bordervTexture.Width, (int)vector2.Y + 32);
		Rectangle destinationRectangle5 = new Rectangle((int)position.X + 32 + (int)vector2.X - bordervTexture.Width, (int)position.Y - 16, bordervTexture.Width, (int)vector2.Y + 32);
		Color color = Color.White * base.TransitionAlpha;
		spriteBatch.Begin();
		spriteBatch.Draw(gradientTexture, destinationRectangle, color);
		spriteBatch.Draw(bordervTexture, destinationRectangle4, color);
		spriteBatch.Draw(bordervTexture, destinationRectangle5, color);
		spriteBatch.Draw(borderTexture, destinationRectangle2, color);
		spriteBatch.Draw(borderTexture, destinationRectangle3, color);
		spriteBatch.DrawString(base.ScreenManager.unselectedFont, message, position, color, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
		spriteBatch.End();
	}
}
