using IMAK3Z0MB1EGAEM.audio;
using IMAK3Z0MB1EGAEM.character;
using IMAK3Z0MB1EGAEM.director;
using IMAK3Z0MB1EGAEM.map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;
using Viking_x86.director;

namespace IMAK3Z0MB1EGAEM.particles.particles;

public class PowerUp : BaseParticleDef
{
	public enum PowerUpType
	{
		WeaponMachine,
		WeaponRocket,
		WeaponFlame,
		WeaponShotty,
		WeaponBeam,
		WeaponNeutron,
		HelpSpeedup,
		HelpLife,
		FunShield,
		FunKill
	}

	public override void Init(Particle p, Vector2 loc, Vector2 traj, int owner, float size, int flags)
	{
		p.frame = 20f;
		p.loc = loc;
		p.traj = default(Vector2);
		p.owner = -1;
		p.size = 0f;
		p.flags = flags;
		if (p.flags == 7)
		{
			if (Rand.CoinToss(0.7f))
			{
				p.flags = Rand.GetRandomInt(0, 9);
			}
			else
			{
				if (TimeMgr.ZombieTMgr().playNum > 1 && Rand.CoinToss(0.95f))
				{
					p.flags = Rand.GetRandomInt(0, 9);
				}
				if (GameState.state == GameState.State.EndlessZombiesPlaying && ZombieGame.GetEndlessRound() > 3)
				{
					p.flags = Rand.GetRandomInt(0, 9);
				}
			}
		}
		p.angle = 1f;
		base.Init(p, loc, traj, owner, size, flags);
	}

	public override void Update(Particle p)
	{
		float num = 100f;
		float num2 = 150f;
		Vector2 vector = default(Vector2);
		Vector2 vector2 = MapMan.mapSize;
		if (GameState.state == GameState.State.EndlessZombiesPlaying)
		{
			vector = ZombieGame.GetEndlessRoomTL();
			vector2 = ZombieGame.GetEndlessRoomBR();
		}
		if (p.loc.X < vector.X + num2)
		{
			p.loc.X += FMan.frameTime * num;
		}
		if (p.loc.Y < vector.Y + num2)
		{
			p.loc.Y += FMan.frameTime * num;
		}
		if (p.loc.X > vector2.X - num2)
		{
			p.loc.X -= FMan.frameTime * num;
		}
		if (p.loc.Y > vector2.Y - num2)
		{
			p.loc.Y -= FMan.frameTime * num;
		}
		p.angle += FMan.frameTime * 1.5f;
		if (p.angle > 1.4f)
		{
			p.angle = 1f;
		}
		for (int i = 0; i < CharMan.hero.Length; i++)
		{
			if (!CharMan.hero[i].exists)
			{
				continue;
			}
			Hero hero = CharMan.hero[i];
			if (hero.loc.X > p.loc.X - 32f && hero.loc.X < p.loc.X + 32f && hero.loc.Y > p.loc.Y - 32f && hero.loc.Y < p.loc.Y + 32f)
			{
				p.exists = false;
				switch (p.flags)
				{
				case 0:
					hero.SetWeapon(Hero.Weapon.MachineGun, 400);
					break;
				case 1:
					hero.SetWeapon(Hero.Weapon.Rockets, 50);
					break;
				case 2:
					hero.SetWeapon(Hero.Weapon.Flamethrower, 250);
					break;
				case 3:
					hero.SetWeapon(Hero.Weapon.Shotty, 50);
					break;
				case 4:
					hero.SetWeapon(Hero.Weapon.Beam, 50);
					break;
				case 5:
					hero.SetWeapon(Hero.Weapon.Neutron, 60);
					break;
				case 7:
					hero.lives++;
					break;
				case 6:
					hero.speedFrame = 30f;
					break;
				case 8:
					hero.spawnFrame = 10f;
					break;
				}
				Sound.Play("suit");
			}
		}
		base.Update(p);
	}

	public override void Draw(Particle p)
	{
		float num = p.frame * 5f;
		if (num > 0.15f)
		{
			num = 0.15f;
		}
		float num2 = 1f;
		if (p.frame > 29.5f)
		{
			num2 = (30f - p.frame) * 2f;
		}
		if (!(p.frame < 5f) || (int)(p.frame * 6f) % 2 != 0)
		{
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(p.loc, 1f), new Rectangle(p.flags * 128, 1152, 128, 128), new Color(1f, 1f, 1f, num), 0f, new Vector2(64f, 64f), p.angle * ScrollMan.zoom * 0.4f * num2, SpriteEffects.None, 1f);
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(p.loc, 1f), new Rectangle(p.flags * 128, 1152, 128, 128), new Color(1f, 1f, 1f, p.frame * 5f), 0f, new Vector2(64f, 64f), 1f * ScrollMan.zoom * 0.4f * num2, SpriteEffects.None, 1f);
			base.Draw(p);
		}
	}
}
