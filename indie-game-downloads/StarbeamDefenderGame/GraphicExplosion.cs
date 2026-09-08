using Microsoft.Xna.Framework;

namespace StarbeamDefenderGame;

public class GraphicExplosion
{
	private bool active;

	private Vector2 position;

	private Vector2 size;

	private int frame;

	private int frametimer;

	public int CurrentFrame => frame;

	public Vector2 Position => position;

	public Vector2 Size => size;

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

	public GraphicExplosion()
	{
		position = Vector2.Zero;
		size = Vector2.Zero;
		active = false;
		frame = 0;
		frametimer = 0;
	}

	public void Create(Vector2 position, Vector2 size)
	{
		this.position = position;
		this.size = size;
		active = true;
		frametimer = 150;
		frame = 0;
	}

	public void Update(int timems)
	{
		if (!active)
		{
			return;
		}
		frametimer -= timems;
		if (frametimer < 0)
		{
			frametimer += 150;
			frame++;
			if (frame > 3)
			{
				active = false;
			}
		}
	}
}
