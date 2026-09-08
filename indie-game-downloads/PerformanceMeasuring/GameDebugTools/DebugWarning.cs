using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PerformanceMeasuring.GameDebugTools;

public class DebugWarning : DrawableGameComponent
{
	private DebugManager debugManager;

	public DebugWarning(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		debugManager = base.Game.Services.GetService(typeof(DebugManager)) as DebugManager;
		if (debugManager == null)
		{
			throw new InvalidOperationException("DebugManager is not registered.");
		}
		base.Initialize();
	}

	public override void Update(GameTime gameTime)
	{
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = debugManager.SpriteBatch;
		SpriteFont debugFont = debugManager.DebugFont;
		Vector2 vector = debugFont.MeasureString("X");
		Rectangle destinationRectangle = new Rectangle(0, 0, (int)(vector.X * 14f), (int)(vector.Y * 1.3f));
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
		spriteBatch.Draw(debugManager.WhiteTexture, destinationRectangle, new Color(0, 0, 0, 128));
		spriteBatch.DrawString(debugFont, "DEBUG MODE", new Vector2(5f, 5f), Color.White);
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
