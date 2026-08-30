using IMAK3Z0MB1EGAEM.audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86.character;

namespace Viking_x86.particles.pickups;

public class Pickup : BaseParticle
{
	public const int FIRE_RAPID = 0;

	public const int FIRE_BOMB = 1;

	public const int FIRE_SPREAD = 2;

	public const int EXTRA_LIFE = 3;

	public const int INVULNERABILITY = 4;

	public override void Init(Particle p, Vector2 loc, Vector2 traj, float size, int flags, int owner)
	{
		p.loc = loc;
		p.traj = traj;
		p.size = size;
		p.owner = 1;
		p.frame = 15f;
		p.flags = flags;
		base.Init(p, loc, traj, size, flags, owner);
	}

	public override void Update(Particle p)
	{
		if (p.traj.Y < 50f)
		{
			p.traj.Y += Game1.frameTime * 50f;
			if (p.traj.Y > 50f)
			{
				p.traj.Y = 50f;
			}
		}
		for (int i = 0; i < 2; i++)
		{
			if (VikingGame.mainPlayerIdx[i] <= -1 || !p.exists)
			{
				continue;
			}
			Character character = Game1.vgame.charMgr.character[i];
			if (character.exists && character.nameIn <= 0 && p.loc.X > character.loc.X - 20f && p.loc.X < character.loc.X + 20f && p.loc.Y > character.loc.Y - 60f && p.loc.Y < character.loc.Y)
			{
				for (int j = 0; j < 32; j++)
				{
					Game1.vgame.pMgr.AddParticle(16, p.loc, Rand.GetRandomVec2(-50f, 50f, -150f, 150f), Rand.GetRandomFloat(0.5f, 25f), 0, 0);
				}
				Sound.Play("suit");
				switch (p.flags)
				{
				case 0:
					character.SetShot(1);
					break;
				case 1:
					character.SetShot(2);
					break;
				case 2:
					character.SetShot(3);
					break;
				case 4:
					character.SetShield(1);
					break;
				case 3:
					character.lives++;
					break;
				}
				p.exists = false;
			}
		}
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(p.loc, 1f), new Rectangle(256 + p.flags * 64, 704, 64, 64), Color.White, VScroll.angle, new Vector2(32f, 32f), VScroll.zoom * 0.5f, SpriteEffects.None, 1f);
		if ((int)(p.frame * 10f) % 3 == 0)
		{
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(p.loc, 1f), new Rectangle(960, 704, 64, 64), new Color(1f, 1f, 1f, 0.9f), VScroll.angle, new Vector2(32f, 32f), VScroll.zoom * 0.5f, SpriteEffects.None, 1f);
		}
	}
}
