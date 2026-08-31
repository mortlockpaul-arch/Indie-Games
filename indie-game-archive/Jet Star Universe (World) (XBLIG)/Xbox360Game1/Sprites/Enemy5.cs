using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Xbox360Game1.Sprites;

public class Enemy5 : Sprite
{
	public bool Dead { get; set; }

	public int Life { get; set; }

	public Color TintColor { get; set; }

	public DateTime ChangePositionTime { get; set; }

	public bool ReversePosition { get; set; }

	public int Score { get; set; }

	public Enemy5(int width, int height)
		: base(width, height)
	{
		Dead = false;
		Life = 1;
		base.Projectiles = new List<Projectile>();
		base.Projectiles.Add(new Projectile());
	}
}
