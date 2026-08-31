using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Xbox360Game1.Sprites;

public class Player : Sprite
{
	public bool Dead { get; set; }

	public int Life { get; set; }

	public DateTime KeyPressTime { get; set; }

	public DateTime HitFrameTime { get; set; }

	public DateTime HitTime { get; set; }

	public Color TintColor { get; set; }

	public int Power { get; set; }

	public int Inventory { get; set; }

	public int Speed { get; set; }

	public bool Invincibility { get; set; }

	public DateTime InvincibilityTime { get; set; }

	public DateTime InvincibilityTimeFlash { get; set; }

	public Color InvisibilityColor { get; set; }

	public int NextPowerupBonus { get; set; }

	public Player(int width, int height)
		: base(width, height)
	{
		Dead = false;
		Life = 10;
		base.Projectiles = new List<Projectile>();
		base.Projectiles.Add(new Projectile());
		TintColor = Color.White;
		Power = 1;
		Inventory = 0;
		Speed = 1;
		NextPowerupBonus = 2500;
	}
}
