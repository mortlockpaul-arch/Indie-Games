using IMAK3Z0MB1EGAEM.character;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;
using Yuki_Win;

namespace IMAK3Z0MB1EGAEM.particles.particles;

public class Flame : BaseParticleDef
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		p.frame = 0.5f;
		p.loc = loc;
		p.traj = traj;
		p.owner = owner;
		p.size = Rand.GetRandomFloat(0f, 0.5f);
		p.angle = Trig.GetAngle(default(Vector2), traj);
		p.alpha = true;
		base.Init(p, loc, traj, owner, size, flags);
	}

	public override void Update(Particle p)
	{
		Monster[] monster = CharMan.monster;
		foreach (Monster monster2 in monster)
		{
			if (monster2.exists && monster2.spawnFrame <= 1f && monster2.loc.X > p.loc.X - 32f && monster2.loc.Y > p.loc.Y - 32f && monster2.loc.X < p.loc.X + 32f && monster2.loc.Y < p.loc.Y + 32f && (monster2.loc - p.loc).Length() < 32f)
			{
				HitManager.HitMonster(monster2, p.traj, p.loc, p.owner);
				if (Rand.CoinToss(0.25f))
				{
					p.exists = false;
				}
				return;
			}
		}
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		int num = (int)((0.5f - p.frame) * 2f * 9f);
		SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(p.loc, 1f), new Rectangle(1024, num * 64, 64, 64), new Color(1f, 1f, 1f, 0.7f), p.angle, new Vector2(32f, 32f), new Vector2(1f, 0.9f) * ScrollMan.zoom * (1.2f - p.frame * 1.5f + p.size), SpriteEffects.None, 1f);
		base.Draw(p);
	}
}
