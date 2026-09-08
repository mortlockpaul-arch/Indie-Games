using System;
using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class Building
{
	private float shield;

	private Vector2 position;

	private Vector2 size;

	private float shieldflicker;

	private int maxframes;

	private int currentframe;

	private int animtimer;

	private int animspeed;

	public float Shields
	{
		get
		{
			return shield;
		}
		set
		{
			shield = value;
		}
	}

	public float ShieldTransparency => shieldflicker;

	public Vector2 Position => position;

	public Vector2 Size => size;

	public int AnimationFrame => currentframe;

	public Building(Vector2 startpos, Vector2 size, int maxframes, int animspeed)
	{
		this.maxframes = maxframes;
		shield = 100f;
		position = startpos;
		this.size = size;
		this.animspeed = animspeed;
		if (this.maxframes > 0)
		{
			Random random = new Random();
			currentframe = random.Next(this.maxframes);
			animtimer = this.animspeed;
		}
		else
		{
			currentframe = 0;
		}
	}

	public Building()
	{
		shield = -1f;
		position = Vector2.Zero;
		size = Vector2.Zero;
	}

	public void Update(int timems, Turret playerone)
	{
		if (!(shield > 0f))
		{
			return;
		}
		shield += 0.002f * (float)timems;
		if (Shields > 100f)
		{
			shield = 100f;
		}
		shieldflicker = 0.01f * (shield / 2f);
		animtimer -= timems;
		if (animtimer < 0)
		{
			currentframe++;
			if (currentframe > maxframes)
			{
				currentframe = 0;
			}
			animtimer = animspeed;
		}
	}

	public bool CheckCollision(Vector2 point)
	{
		int num = ((!(shield > 0f)) ? 25 : 128);
		if ((position + new Vector2(size.X / 2f, size.Y) - point).Length() < (float)num)
		{
			return true;
		}
		return false;
	}
}
