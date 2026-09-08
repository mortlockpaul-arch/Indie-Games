using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eyehook.Framework;

public class AnimatedSprite : Sprite
{
	public int Frame;

	public int SubSet;

	public bool HasLooped;

	public int Direction = 1;

	private readonly Rectangle frameRect;

	private readonly Vector2 origin;

	private readonly int frames;

	private readonly TimeSpan frameDuration;

	private TimeSpan frameWait;

	public AnimatedSprite(Texture2D texture, Rectangle frameRect, Vector2? origin, int frames, TimeSpan frameDuration)
		: base(texture)
	{
		this.frameRect = frameRect;
		this.origin = ((!origin.HasValue) ? new Vector2(frameRect.Width / 2, frameRect.Height / 2) : origin.Value);
		this.frames = frames;
		this.frameDuration = frameDuration;
		Frame = 0;
		frameWait = frameDuration;
	}

	public void Reset()
	{
		Frame = 0;
		HasLooped = false;
		frameWait = frameDuration;
	}

	public override void Update(GameTime gameTime)
	{
		if (!(frameDuration > TimeSpan.Zero))
		{
			return;
		}
		frameWait -= gameTime.ElapsedGameTime;
		if (frameWait <= TimeSpan.Zero)
		{
			Frame += Direction;
			if (Frame == frames)
			{
				Frame = 0;
				HasLooped = true;
			}
			else if (Frame < 0)
			{
				Frame = frames - 1;
				HasLooped = true;
			}
			frameWait += frameDuration;
		}
	}

	public override void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale)
	{
		spriteBatch.Draw(sourceRectangle: new Rectangle(frameRect.X + Frame * frameRect.Width, frameRect.Y + SubSet * frameRect.Height, frameRect.Width, frameRect.Height), texture: texture, position: position, color: color, rotation: Rotation, origin: origin, scale: scale, effects: SpriteEffects, layerDepth: 0f);
	}
}
