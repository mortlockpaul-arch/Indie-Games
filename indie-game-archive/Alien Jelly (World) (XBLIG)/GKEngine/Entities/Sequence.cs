using System;
using System.Collections.Generic;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Entities;

public class Sequence : Entity2D
{
	public delegate void CallBack();

	private SpriteBatch spriteBatch;

	public new Scene scene;

	public string assetBase;

	public int frameStart;

	public int frameEnd;

	public int frameSequenceDigits;

	public List<Texture2D> frames = new List<Texture2D>();

	protected Rectangle bounds;

	public int playCurrentFrame;

	public int playTotalTime;

	public int playCurrentTime;

	public int playDir = 1;

	public bool isPlaying;

	public bool isLoaded;

	public CallBack Play_CallBack;

	public Sequence(Scene oScene, string xAssetBase, int xStart, int xEnd, int xDigits)
	{
		scene = oScene;
		assetBase = xAssetBase;
		frameStart = xStart;
		frameEnd = xEnd;
		frameSequenceDigits = xDigits;
	}

	public override void Load()
	{
		spriteBatch = new SpriteBatch(GameEngine.instance.GraphicsDevice);
		for (int i = frameStart; i <= frameEnd; i++)
		{
			string text = i.ToString();
			text = text.PadLeft(frameSequenceDigits, '0');
			frames.Add(GameEngine.Content.Load<Texture2D>(assetBase + text));
		}
		if (frames.Count > 0 && size.X == 0f)
		{
			size = new Vector2(frames[0].Width, frames[0].Height);
		}
		bounds = new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), Convert.ToInt32(size.X), Convert.ToInt32(size.Y));
		isLoaded = true;
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible && isLoaded)
		{
			bounds.X = (int)position.X;
			bounds.Y = (int)position.X;
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			spriteBatch.Draw(frames[playCurrentFrame], bounds, Color.White);
			spriteBatch.End();
		}
	}

	public void Play(int oPlayTime, int xDir, CallBack oCallBack)
	{
		if (xDir < 0)
		{
			playCurrentFrame = frames.Count - 1;
		}
		else
		{
			playCurrentFrame = 0;
		}
		playTotalTime = oPlayTime;
		playCurrentTime = 0;
		playDir = xDir;
		Play_CallBack = oCallBack;
		isPlaying = true;
		GameEngine.instance.updateStack.Add(Play_Step);
	}

	public bool Play_Step(GameTime oGameTime)
	{
		bool result = false;
		playCurrentTime += oGameTime.ElapsedGameTime.Milliseconds;
		decimal num = Convert.ToDecimal(playCurrentTime) / (decimal)playTotalTime;
		if (num >= 1m)
		{
			result = true;
			isPlaying = false;
			if (Play_CallBack != null)
			{
				Play_CallBack();
			}
		}
		else
		{
			playCurrentFrame = (int)Math.Floor((double)num * (double)frames.Count);
			if (playDir < 0)
			{
				playCurrentFrame = frames.Count - 1 - playCurrentFrame;
			}
		}
		return result;
	}

	public void Pause(int oPlayTime, CallBack oCallBack)
	{
		playTotalTime = oPlayTime;
		playCurrentTime = 0;
		Play_CallBack = oCallBack;
		isPlaying = true;
		GameEngine.instance.updateStack.Add(Pause_Step);
	}

	public bool Pause_Step(GameTime oGameTime)
	{
		bool result = false;
		playCurrentTime += oGameTime.ElapsedGameTime.Milliseconds;
		decimal num = Convert.ToDecimal(playCurrentTime) / (decimal)playTotalTime;
		if (num >= 1m)
		{
			result = true;
			isPlaying = false;
			Play_CallBack();
		}
		return result;
	}
}
