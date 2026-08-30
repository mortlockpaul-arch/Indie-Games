using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public struct Animation2DPlayer
{
	private Animation2D animation;

	private int frameIndex;

	private float time;

	private int totalFramesPlayed;

	private bool isFinished;

	public Animation2D Animation => animation;

	public int FrameIndex => frameIndex;

	public int TotalFramesPlayed => totalFramesPlayed;

	public Vector2 Origin => new Vector2((float)Animation.FrameWidth / 2f, Animation.FrameHeight / 2);

	public bool IsFinished => isFinished;

	public bool IsPlaying
	{
		get
		{
			if (animation != null)
			{
				return !IsFinished;
			}
			return false;
		}
	}

	public void PlayAnimation(Animation2D animation)
	{
		if (Animation != animation)
		{
			this.animation = animation;
			Reset();
		}
	}

	private void Reset()
	{
		frameIndex = 0;
		time = 0f;
		isFinished = false;
		totalFramesPlayed = 0;
	}

	public void ReplayAnimation(Animation2D animation)
	{
		if (Animation != animation)
		{
			PlayAnimation(animation);
		}
		else
		{
			Reset();
		}
	}

	public void DrawWithVerticalMirror(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float depth)
	{
		Draw(gameTime, spriteBatch, position, 1f, 0f, SpriteEffects.None, 0f, Color.White);
		Draw(gameTime, spriteBatch, position, 1f, 0f, SpriteEffects.FlipHorizontally, 0f, Color.White);
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, float depth)
	{
		Draw(gameTime, spriteBatch, position, 1f, 0f, spriteEffects, depth, Color.White);
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float speedRatio, float rotation, SpriteEffects spriteEffects, float depth, Color color)
	{
		if (Animation == null)
		{
			throw new NotSupportedException("No animation is currently playing.");
		}
		time += (float)gameTime.ElapsedGameTime.TotalSeconds * speedRatio;
		while (time > Animation.FrameTime)
		{
			totalFramesPlayed++;
			time -= Animation.FrameTime;
			if (Animation.IsLooping)
			{
				frameIndex = (frameIndex + 1) % Animation.FrameCount;
				continue;
			}
			isFinished = frameIndex == Animation.FrameCount - 1;
			frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
		}
		spriteBatch.Draw(Animation.Layers[FrameIndex].Tex, Drawing2D.RoundFloatPos(position), null, color, rotation, Origin, 1f, spriteEffects, depth);
	}
}
