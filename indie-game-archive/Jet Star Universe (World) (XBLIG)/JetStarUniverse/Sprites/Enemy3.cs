using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace JetStarUniverse.Sprites;

public class Enemy3 : Sprite
{
	public bool Dead { get; set; }

	public int Life { get; set; }

	public Color TintColor { get; set; }

	public DateTime ChangePositionTime { get; set; }

	public bool ReversePosition { get; set; }

	public bool Reverse { get; set; }

	public int Score { get; set; }

	public Enemy3(int width, int height)
		: base(width, height)
	{
		Dead = false;
		Life = 2;
		base.Projectiles = new List<Projectile>();
		base.Projectiles.Add(new Projectile());
	}
}
