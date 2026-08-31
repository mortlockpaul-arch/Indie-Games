using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JetStarUniverse.Sprites;

public class Bird : IGameObject, IAnimation, IExplosion
{
	private Rectangle _boxRectangle = default(Rectangle);

	public Vector2 Position { get; set; }

	public Vector2 CenterRight => new Vector2(Position.X + (float)SourceRectangles[NextFrameIndex].Width, Position.Y + (float)(SourceRectangles[NextFrameIndex].Height / 2));

	public Texture2D Texture2D { get; set; }

	public bool Hidden { get; set; }

	public Rectangle BoxRectangle
	{
		get
		{
			_boxRectangle.X = (int)Position.X;
			_boxRectangle.Y = (int)Position.Y;
			_boxRectangle.Width = Width;
			_boxRectangle.Height = Height;
			return _boxRectangle;
		}
	}

	public int Width { get; set; }

	public int Height { get; set; }

	public bool Hit { get; set; }

	public List<Rectangle> SourceRectangles { get; set; }

	public int NextFrameIndex { get; set; }

	public DateTime FrameTime { get; set; }

	public bool Reverse { get; set; }

	public bool Dead { get; set; }

	public Texture2D TextureOfExplosion { get; set; }

	public List<Rectangle> SourceOfExplosion { get; set; }

	public Vector2 PositionOfExplosion { get; set; }

	public bool ShowExplosion { get; set; }

	public int NextFrameIndexOfExplosion { get; set; }

	public DateTime ExplosionFrameTime { get; set; }

	public Bird(int width, int height)
	{
		SourceRectangles = new List<Rectangle>();
		NextFrameIndex = 0;
		Width = width;
		Height = height;
		SourceOfExplosion = new List<Rectangle>();
	}

	public Vector2 RandomLocation(float fixedX, float maxY)
	{
		Random random = new Random((int)DateTime.Now.Ticks);
		return new Vector2(fixedX, random.Next(10, (int)maxY - 10));
	}

	public static void ResetAllBirdPositions(List<Bird> birds, GraphicsDevice graphicsDevice, Rectangle screenSize)
	{
		for (int i = 0; i < birds.Count; i++)
		{
			Random random = new Random((int)DateTime.Now.Ticks);
			birds[i].Position = new Vector2((float)screenSize.Right + 250f, (float)screenSize.Top + 130f + (float)(i * 30));
			birds[i].Dead = false;
		}
	}
}
