using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xbox360Game1.Sprites;

public class Sprite : IGameObject, IAnimation, IProjectile, IExplosion
{
	public enum Direction
	{
		None,
		Up,
		Down
	}

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

	public List<Projectile> Projectiles { get; set; }

	public Rectangle SourceProjectile { get; set; }

	public Texture2D TextureOfExplosion { get; set; }

	public List<Rectangle> SourceOfExplosion { get; set; }

	public Vector2 PositionOfExplosion { get; set; }

	public bool ShowExplosion { get; set; }

	public int NextFrameIndexOfExplosion { get; set; }

	public DateTime ExplosionFrameTime { get; set; }

	public Sprite(int width, int height)
	{
		SourceRectangles = new List<Rectangle>();
		NextFrameIndex = 0;
		Width = width;
		Height = height;
	}
}
