using IMAK3Z0MB1EGAEM.character;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;
using Yuki_Win;

namespace IMAK3Z0MB1EGAEM.particles.particles;

public class Shot : BaseParticleDef
{
	public override void Init(Particle p, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		p.frame = 0.3f;
		p.loc = loc;
		p.traj = traj;
		p.owner = owner;
		p.angle = Trig.GetAngle(default(Vector2), traj);
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
				p.exists = false;
				return;
			}
		}
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(p.loc, 1f), new Rectangle(256, 320, 192, 64), new Color(1f, 1f, 1f, 0.5f), p.angle, new Vector2(96f, 32f), 0.2f * ScrollMan.zoom, SpriteEffects.None, 1f);
		base.Draw(p);
	}
}
