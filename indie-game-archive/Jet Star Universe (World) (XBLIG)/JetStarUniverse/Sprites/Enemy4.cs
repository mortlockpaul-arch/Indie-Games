using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace JetStarUniverse.Sprites;

public class Enemy4 : Sprite
{
	public bool Dead { get; set; }

	public int Life { get; set; }

	public Color TintColor { get; set; }

	public DateTime ChangePositionTime { get; set; }

	public bool ReversePosition { get; set; }

	public bool Reverse { get; set; }

	public double GameTime { get; set; }

	public int Speed { get; set; }

	public int Score { get; set; }

	public Enemy4(int width, int height)
		: base(width, height)
	{
		Dead = false;
		Life = 5;
		base.Projectiles = new List<Projectile>();
		base.Projectiles.Add(new Projectile());
	}
}
