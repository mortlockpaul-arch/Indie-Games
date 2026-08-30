using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerFromHell;

internal struct AnimationPlayer
{
	private Animation animation;

	private int frameIndex;

	private float time;

	public Animation Animation => animation;

	public int FrameIndex => frameIndex;

	public Vector2 Origin => new Vector2((float)Animation.FrameWidth / 2f, Animation.FrameHeight);

	public void PlayAnimation(Animation animation)
	{
		if (Animation != animation)
		{
			this.animation = animation;
			frameIndex = 0;
			time = 0f;
		}
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
	{
		if (Animation == null)
		{
			throw new NotSupportedException("No animation is currently playing.");
		}
		time += (float)gameTime.ElapsedGameTime.TotalSeconds;
		while (time > Animation.FrameTime)
		{
			time -= Animation.FrameTime;
			if (Animation.IsLooping)
			{
				frameIndex = (frameIndex + 1) % Animation.FrameCount;
			}
			else
			{
				frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
			}
		}
		spriteBatch.Draw(sourceRectangle: new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height), texture: Animation.Texture, position: position, color: Color.White, rotation: 0f, origin: Origin, scale: 1f, effects: spriteEffects, layerDepth: 0f);
	}
}
