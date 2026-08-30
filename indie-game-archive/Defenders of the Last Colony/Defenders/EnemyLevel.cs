using System;
using Microsoft.Xna.Framework;

namespace Defenders;

internal class EnemyLevel
{
	public int type;

	public int maxAmmount;

	public int ammount;

	public uint frame;

	public int rate;

	public Vector2 position;

	public EnemyLevel(int type, int ammount, uint frame)
	{
		Random random = new Random();
		this.type = type;
		this.ammount = ammount;
		maxAmmount = ammount;
		this.frame = frame;
		rate = 30;
		position = new Vector2(random.Next(-600, 1880), random.Next(320, 1040));
	}

	public EnemyLevel(int type, int ammount, uint frame, Vector2 position)
	{
		this.type = type;
		this.ammount = ammount;
		maxAmmount = ammount;
		this.frame = frame;
		rate = 30;
		this.position = position;
	}

	public bool isMouseOver(Vector2 mouse)
	{
		int num = 40;
		Rectangle rectangle = new Rectangle((int)(position.X - (float)(num / 2)), (int)(position.Y - (float)(num / 2)), num, num);
		Rectangle value = new Rectangle((int)(mouse.X - 2f), (int)(mouse.Y - 2f), 4, 4);
		return rectangle.Intersects(value);
	}

	public void Reset()
	{
		ammount = maxAmmount;
	}
}
