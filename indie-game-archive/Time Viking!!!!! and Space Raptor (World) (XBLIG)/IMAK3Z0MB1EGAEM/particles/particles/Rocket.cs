using IMAK3Z0MB1EGAEM.audio;
using IMAK3Z0MB1EGAEM.character;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;
using Yuki_Win;

namespace IMAK3Z0MB1EGAEM.particles.particles;

public class Rocket : BaseParticleDef
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		p.frame = 1f;
		p.loc = loc;
		p.traj = traj;
		p.owner = owner;
		p.angle = Trig.GetAngle(default(Vector2), traj);
		base.Init(p, loc, traj, owner, size, flags);
	}

	public override void Update(Particle p)
	{
		ParticleMan.AddParticle(10, p.loc, p.traj * -0.1f, 0, Rand.GetRandomFloat(0.2f, 0.3f), 0);
		ParticleMan.AddParticle(10, p.loc - p.traj * FMan.frameTime * 0.5f, p.traj * -0.1f, 0, Rand.GetRandomFloat(0.2f, 0.3f), 0);
		Monster[] monster = CharMan.monster;
		foreach (Monster monster2 in monster)
		{
			if (!monster2.exists || !(monster2.spawnFrame <= 1f) || !(monster2.loc.X > p.loc.X - 32f) || !(monster2.loc.Y > p.loc.Y - 32f) || !(monster2.loc.X < p.loc.X + 32f) || !(monster2.loc.Y < p.loc.Y + 32f) || !((monster2.loc - p.loc).Length() < 32f))
			{
				continue;
			}
			HitManager.HitMonster(monster2, p.traj, p.loc, p.owner);
			for (int j = 0; j < 10; j++)
			{
				ParticleMan.AddParticle(10, p.loc, Rand.GetRandomVec2(-200f, 200f, -200f, 200f), 0, Rand.GetRandomFloat(1.3f, 2f), 0);
			}
			Sound.Play("explode");
			for (int k = 0; k < CharMan.monster.Length; k++)
			{
				if (!CharMan.monster[k].exists)
				{
					continue;
				}
				Vector2 traj = CharMan.monster[k].loc - p.loc;
				if (traj.LengthSquared() < 10000f)
				{
					while (CharMan.monster[k].exists)
					{
						HitManager.HitMonster(CharMan.monster[k], traj, CharMan.monster[k].loc, p.owner);
					}
				}
			}
			p.exists = false;
			return;
		}
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(p.loc, 1f), new Rectangle(1088, 0, 64, 64), new Color(1f, 1f, 1f, 0.5f), p.angle, new Vector2(96f, 32f), 0.6f * ScrollMan.zoom, SpriteEffects.None, 1f);
		base.Draw(p);
	}
}
