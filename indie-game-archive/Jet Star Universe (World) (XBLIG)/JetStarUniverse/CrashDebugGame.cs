using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JetStarUniverse;

public class CrashDebugGame : Game
{
	private SpriteBatch spriteBatch;

	private SpriteFont font;

	private readonly Exception exception;

	private float _xScroll = 100f;

	private float _yScroll = 100f;

	public CrashDebugGame(Exception exception)
	{
		this.exception = exception;
		new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		if (Game1.GamerServices == null)
		{
			base.Components.Add(new GamerServicesComponent(this));
		}
		base.Initialize();
		if (Game1.GamerServices != null)
		{
			base.Components.Add(Game1.GamerServices);
		}
	}

	protected override void LoadContent()
	{
		font = base.Content.Load<SpriteFont>("MyFontSm");
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
	}

	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Two).Buttons.Back == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Three).Buttons.Back == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Four).Buttons.Back == ButtonState.Pressed)
		{
			Exit();
		}
		if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X <= -0.25f || GamePad.GetState(PlayerIndex.Two).DPad.Left == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X <= -0.25f || GamePad.GetState(PlayerIndex.Three).DPad.Left == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Three).ThumbSticks.Left.X <= -0.25f || GamePad.GetState(PlayerIndex.Four).DPad.Left == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Four).ThumbSticks.Left.X <= -0.25f)
		{
			_xScroll += 5f;
		}
		if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X >= 0.25f || GamePad.GetState(PlayerIndex.Two).DPad.Right == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X >= 0.25f || GamePad.GetState(PlayerIndex.Three).DPad.Right == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Three).ThumbSticks.Left.X >= 0.25f || GamePad.GetState(PlayerIndex.Four).DPad.Right == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Four).ThumbSticks.Left.X >= 0.25f)
		{
			_xScroll -= 5f;
		}
		if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y >= 0.25f || GamePad.GetState(PlayerIndex.Two).DPad.Up == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.Y >= 0.25f || GamePad.GetState(PlayerIndex.Three).DPad.Up == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Three).ThumbSticks.Left.Y >= 0.25f || GamePad.GetState(PlayerIndex.Four).DPad.Up == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Four).ThumbSticks.Left.Y >= 0.25f)
		{
			_yScroll += 5f;
		}
		if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y <= -0.25f || GamePad.GetState(PlayerIndex.Two).DPad.Down == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.Y <= -0.25f || GamePad.GetState(PlayerIndex.Three).DPad.Down == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Three).ThumbSticks.Left.Y <= -0.25f || GamePad.GetState(PlayerIndex.Four).DPad.Down == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Four).ThumbSticks.Left.Y <= -0.25f)
		{
			_yScroll -= 5f;
		}
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin();
		spriteBatch.DrawString(font, "**** CRASH LOG ****", new Vector2(_xScroll, _yScroll), Color.White);
		spriteBatch.DrawString(font, "Press Back to Exit", new Vector2(_xScroll, _yScroll + 20f), Color.White);
		spriteBatch.DrawString(font, string.Format(CultureInfo.InvariantCulture, "Exception: {0}", new object[1] { exception.Message }), new Vector2(_xScroll, _yScroll + 40f), Color.White);
		spriteBatch.DrawString(font, string.Format(CultureInfo.InvariantCulture, "Stack Trace:\n{0}", new object[1] { exception.StackTrace }), new Vector2(_xScroll, _yScroll + 60f), Color.White);
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
