using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class Explosion
{
	private Vector2 position;

	private int size;

	private bool expanding;

	private float maxsize;

	private int speedms;

	private int speedtimer;

	private int score;

	private PlayerIndex playersetoff;

	public PlayerIndex PlayerToScore => playersetoff;

	public int Score => score;

	public Vector2 Position => position;

	public int Size => size;

	public bool Active
	{
		get
		{
			if (size > -1)
			{
				return true;
			}
			return false;
		}
		set
		{
			if (!value)
			{
				size = -1;
			}
		}
	}

	public Explosion()
	{
		position = Vector2.Zero;
		size = -1;
		expanding = false;
		maxsize = 0f;
	}

	public void Create(Vector2 position, float maxsize, int speed, int score, PlayerIndex scoringplayer)
	{
		this.position = position;
		this.maxsize = maxsize;
		speedms = speed;
		size = 0;
		expanding = true;
		this.score = score;
		playersetoff = scoringplayer;
	}

	public void Update(int timems)
	{
		speedtimer -= timems;
		if (speedtimer >= 0)
		{
			return;
		}
		speedtimer = speedms;
		if (expanding)
		{
			size++;
			if ((float)size >= maxsize)
			{
				expanding = false;
			}
		}
		else
		{
			size--;
		}
	}

	public bool Collision(Vector2 position)
	{
		if ((position - this.position).Length() <= (float)size)
		{
			return true;
		}
		return false;
	}
}
