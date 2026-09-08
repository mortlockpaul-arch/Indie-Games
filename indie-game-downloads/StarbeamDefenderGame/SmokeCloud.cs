using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class SmokeCloud
{
	private Vector2 position;

	private float alpha;

	private float size;

	private double angle;

	private bool active;

	private int timerfade;

	private int frame;

	public float Alpha => alpha;

	public bool Active
	{
		get
		{
			return active;
		}
		set
		{
			active = value;
		}
	}

	public int Angle => (int)angle;

	public Vector2 Position => position;

	public float Size => size;

	public int Frame => frame;

	public SmokeCloud()
	{
		position = Vector2.Zero;
		size = 0f;
		alpha = 0f;
		angle = 0.0;
		active = false;
	}

	public void Create(Vector2 pos, float size, double angle, int frame)
	{
		this.frame = frame;
		position = pos;
		alpha = 1f;
		this.size = size;
		this.angle = angle;
		active = true;
	}

	public void Update(int timems)
	{
		timerfade -= timems;
		if (timerfade < 0)
		{
			timerfade = 100;
			alpha -= 0.1f;
			if (alpha < 0.1f)
			{
				active = false;
			}
		}
	}
}
