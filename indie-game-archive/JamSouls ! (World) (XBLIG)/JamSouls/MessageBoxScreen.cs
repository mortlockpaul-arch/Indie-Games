using System;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

internal class MessageBoxScreen : GameScreen
{
	private string message;

	private AnimatedSprite m_Atex;

	private AnimatedSprite m_Btex;

	private ButtonState Astate = ButtonState.Pressed;

	private ButtonState Bstate = ButtonState.Pressed;

	public event EventHandler<PlayerIndexEventArgs> Accepted;

	public event EventHandler<PlayerIndexEventArgs> Cancelled;

	public MessageBoxScreen(string message, AnimatedSprite aTex, AnimatedSprite bTex)
		: this(message, includeUsageText: true, aTex, bTex)
	{
	}

	public MessageBoxScreen(string message, bool includeUsageText, AnimatedSprite bTex, AnimatedSprite aTex)
	{
		m_Atex = aTex;
		m_Btex = bTex;
		if (includeUsageText)
		{
			this.message = message + "\nA button : ok\nB button : cancel";
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
		_ = base.ScreenManager.Game.Content;
	}

	public override void HandleInput()
	{
		if (InputManager.GetKeyState(base.ControllingPlayer.Value, 4) == ButtonState.Pressed && Astate == ButtonState.Released)
		{
			if (Accepted != null)
			{
				Accepted(this, new PlayerIndexEventArgs(base.ControllingPlayer.Value));
			}
			ExitScreen();
		}
		Astate = InputManager.GetKeyState(base.ControllingPlayer.Value, 4);
		if (InputManager.GetKeyState(base.ControllingPlayer.Value, 5) == ButtonState.Pressed && Bstate == ButtonState.Released)
		{
			if (Cancelled != null)
			{
				Cancelled(this, new PlayerIndexEventArgs(base.ControllingPlayer.Value));
			}
			ExitScreen();
		}
		Bstate = InputManager.GetKeyState(base.ControllingPlayer.Value, 5);
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		SpriteFont goBoomMiddle = base.ScreenManager.GoBoomMiddle;
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2 / 3);
		Viewport viewport = base.ScreenManager.GraphicsDevice.Viewport;
		Vector2 vector = new Vector2(viewport.Width, viewport.Height);
		Vector2 vector2 = goBoomMiddle.MeasureString(message);
		Vector2 vector3 = (vector - vector2) / 2f;
		Vector2 Position = vector3;
		Position.Y += 30f;
		Vector2 Position2 = Position;
		Position2.Y += 50f;
		Vector2 position = Position;
		position.Y += 20f;
		position.X += m_Atex.GetFrameWidth();
		Vector2 position2 = Position2;
		position2.Y += 20f;
		position2.X += m_Btex.GetFrameWidth();
		Color color = new Color(255, 255, 255, base.TransitionAlpha);
		spriteBatch.Begin();
		spriteBatch.DrawString(base.ScreenManager.GoBoomMiddle, message, vector3, color);
		m_Atex.Draw(ref Position, SpriteEffects.None, Color.White, 1f);
		spriteBatch.DrawString(base.ScreenManager.GoBoomMiddle, TextManager.GetText(TextID.VALID), position, Color.White);
		m_Btex.Draw(ref Position2, SpriteEffects.None, Color.White, 1f);
		spriteBatch.DrawString(base.ScreenManager.GoBoomMiddle, TextManager.GetText(TextID.UNDO), position2, Color.White);
		spriteBatch.End();
	}
}
