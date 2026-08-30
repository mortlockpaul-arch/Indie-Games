using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;
using Yuki_Win;

namespace IMAK3Z0MB1EGAEM.character;

public class Legs
{
	public float frame;

	private float angle;

	public Vector2 traj;

	public void Draw(Vector2 loc, float scale)
	{
		for (int i = 0; i < 2; i++)
		{
			float num = angle + ((i == 1) ? 3.14f : 0f);
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc + new Vector2((float)Math.Cos(num) * (float)Math.Sin(frame) * 8f, (float)Math.Sin(num) * (float)Math.Sin(frame) * 8f), 1f), new Rectangle(0, 256, 128, 64), Color.White, num, new Vector2(64f, 22f), ScrollMan.zoom * 0.3f * scale, SpriteEffects.None, 1f);
		}
	}

	public void Update()
	{
		Update(1f);
	}

	public void Update(float speed)
	{
		float num = angle;
		float num2 = traj.Length();
		if (num2 > 0f)
		{
			num = Trig.GetAngle(default(Vector2), traj);
			frame += num2 * FMan.frameTime * 11f * speed;
			if (frame > 6.28f)
			{
				frame -= 6.28f;
			}
		}
		else if (frame > 0f)
		{
			float num3 = 15f;
			if (frame < 1.57f)
			{
				frame -= FMan.frameTime * num3;
				if (frame < 0f)
				{
					frame = 0f;
				}
			}
			else if (frame < 3.14f)
			{
				frame += FMan.frameTime * num3;
				if (frame >= 3.14f)
				{
					frame = 0f;
				}
			}
			else if (frame < 4.71f)
			{
				frame -= FMan.frameTime * num3;
				if (frame <= 3.14f)
				{
					frame = 0f;
				}
			}
			else
			{
				frame += FMan.frameTime * num3;
				if (frame >= 6.28f)
				{
					frame = 0f;
				}
			}
		}
		float num4;
		for (num4 = num - angle; num4 < -3.14f; num4 += 6.28f)
		{
		}
		while (num4 > 3.14f)
		{
			num4 -= 6.28f;
		}
		angle += num4 * FMan.frameTime * 10f;
	}
}
