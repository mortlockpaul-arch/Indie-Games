using IMAK3Z0MB1EGAEM.audio;
using IMAK3Z0MB1EGAEM.character;
using IMAK3Z0MB1EGAEM.director;
using Microsoft.Xna.Framework;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.particles;

public class HitManager
{
	public static void HitMonster(Monster m, Vector2 traj, Vector2 loc, int owner)
	{
		switch (m.type)
		{
		case 0:
			MakeBloodChunks(m.loc, traj);
			CharMan.hero[owner].AddPoints(m.loc, 100L);
			m.exists = false;
			if (Rand.CoinToss(0.01f))
			{
				SpawnMgr.MakeGoodie(m.loc);
			}
			break;
		case 10:
		{
			for (int num2 = 0; num2 < 10; num2++)
			{
				ParticleMan.AddParticle(15, m.loc, Rand.GetRandomVec2(-200f, 200f, -200f, 200f), 0, Rand.GetRandomFloat(0.1f, 0.3f), 0);
			}
			m.exists = false;
			CharMan.hero[owner].AddPoints(m.loc, 120L);
			if (Rand.CoinToss(0.02f))
			{
				SpawnMgr.MakeGoodie(m.loc);
			}
			break;
		}
		case 9:
		{
			for (int num4 = 0; num4 < 10; num4++)
			{
				ParticleMan.AddParticle(7, m.loc, Rand.GetRandomVec2(-200f, 200f, -200f, 200f), 0, Rand.GetRandomFloat(0.3f, 0.5f), 0);
			}
			m.exists = false;
			CharMan.hero[owner].AddPoints(m.loc, 150L);
			if (Rand.CoinToss(0.01f))
			{
				SpawnMgr.MakeGoodie(m.loc);
			}
			break;
		}
		case 8:
			m.hp--;
			if (m.hp <= 0)
			{
				for (int num5 = 0; num5 < 10; num5++)
				{
					ParticleMan.AddParticle(10, m.loc, Rand.GetRandomVec2(-200f, 200f, -200f, 200f), 0, Rand.GetRandomFloat(1.3f, 2f), 0);
				}
				Sound.Play("explode");
				CharMan.hero[owner].AddPoints(m.loc, 500L);
				m.exists = false;
				if (Rand.CoinToss(0.1f))
				{
					SpawnMgr.MakeGoodie(m.loc);
				}
			}
			else
			{
				MakeBloodSplode(loc, 3, 0.2f, 150f);
				m.loc += traj * 0.002f;
			}
			break;
		case 1:
			m.hp--;
			if (m.hp <= 0)
			{
				MakeBloodChunks(m.loc, traj);
				MakeBloodSplode(loc, 10, Rand.GetRandomFloat(0.5f, 1f), 300f);
				CharMan.hero[owner].AddPoints(m.loc, 500L);
				m.exists = false;
				if (Rand.CoinToss(0.4f))
				{
					SpawnMgr.MakeGoodie(m.loc);
				}
			}
			else
			{
				MakeBloodSplode(loc, 3, 0.2f, 150f);
				m.loc += traj * 0.002f;
			}
			break;
		case 2:
		{
			m.exists = false;
			for (int num = 0; num < 4; num++)
			{
				CharMan.MakeMonster(m.loc, 3);
			}
			MakePixelSplode(loc, 10, Rand.GetRandomFloat(0.4f, 1f), 500f);
			CharMan.hero[owner].AddPoints(m.loc, 200L);
			if (Rand.CoinToss(0.1f))
			{
				SpawnMgr.MakeGoodie(m.loc);
			}
			break;
		}
		case 3:
		{
			m.exists = false;
			for (int num3 = 0; num3 < 3; num3++)
			{
				CharMan.MakeMonster(m.loc, 4);
			}
			MakePixelSplode(loc, 7, Rand.GetRandomFloat(0.3f, 0.7f), 300f);
			CharMan.hero[owner].AddPoints(m.loc, 150L);
			if (Rand.CoinToss(0.02f))
			{
				SpawnMgr.MakeGoodie(m.loc);
			}
			break;
		}
		case 4:
			m.exists = false;
			MakePixelSplode(loc, 5, Rand.GetRandomFloat(0.3f, 0.4f), 200f);
			CharMan.hero[owner].AddPoints(m.loc, 50L);
			if (Rand.CoinToss(0.02f))
			{
				SpawnMgr.MakeGoodie(m.loc);
			}
			break;
		case 5:
			m.hp--;
			if (m.hp <= 0)
			{
				MakeGoo(loc, 10, Rand.GetRandomFloat(0.5f, 1f), 300f);
				CharMan.hero[owner].AddPoints(m.loc, 800L);
				m.exists = false;
				if (Rand.CoinToss(0.4f))
				{
					SpawnMgr.MakeGoodie(m.loc);
				}
			}
			else
			{
				MakeGoo(loc, 3, 0.2f, 50f);
				m.loc += traj * 0.002f;
				for (int n = 0; n < 3; n++)
				{
					CharMan.MakeMonster(m.loc + Rand.GetRandomVec2(-20f, 20f, -20f, 20f), 6, midSpawn: true);
				}
			}
			break;
		case 6:
			m.exists = false;
			MakeGoo(loc, 5, Rand.GetRandomFloat(0.3f, 0.4f), 200f);
			CharMan.hero[owner].AddPoints(m.loc, 50L);
			break;
		case 7:
			m.hp--;
			if (m.hp <= 0)
			{
				m.exists = false;
				for (int i = 0; i < 10; i++)
				{
					ParticleMan.AddParticle(7, m.loc, Rand.GetRandomVec2(-500f, 500f, -500f, 500f), 0, 0f, 0);
				}
				for (int j = 0; j < ParticleMan.particle.Length; j++)
				{
					if (ParticleMan.particle[j].exists && ParticleMan.particle[j].type == 6 && ParticleMan.particle[j].owner == m.idx)
					{
						for (int k = 0; k < 3; k++)
						{
							ParticleMan.AddParticle(7, ParticleMan.particle[j].loc, Rand.GetRandomVec2(-200f, 200f, -200f, 200f), 0, 0f, 0);
						}
						ParticleMan.particle[j].exists = false;
					}
				}
				if (Rand.CoinToss(0.4f))
				{
					SpawnMgr.MakeGoodie(m.loc);
				}
				CharMan.hero[owner].AddPoints(m.loc, 600L);
			}
			else
			{
				for (int l = 0; l < 4; l++)
				{
					ParticleMan.AddParticle(7, m.loc, Rand.GetRandomVec2(-500f, 500f, -500f, 500f), 0, 0f, 0);
				}
			}
			break;
		}
	}

	public static void CheckHeroSmash(Monster m)
	{
		Vector2 loc = m.loc;
		for (int i = 0; i < CharMan.hero.Length; i++)
		{
			if (!CharMan.hero[i].exists || !(CharMan.hero[i].respawnFrame <= 0f))
			{
				continue;
			}
			Hero hero = CharMan.hero[i];
			float num = 20f;
			if (CharMan.hero[i].spawnFrame > 0f)
			{
				num = 50f;
			}
			if (hero.loc.X > loc.X - num && hero.loc.X < loc.X + num && hero.loc.Y > loc.Y - num && hero.loc.Y < loc.Y + num)
			{
				if (CharMan.hero[i].spawnFrame > 0f)
				{
					HitMonster(m, m.loc - CharMan.hero[i].loc, m.loc, i);
				}
				else
				{
					hero.Kill();
				}
			}
		}
	}

	private static void MakeBloodChunks(Vector2 loc, Vector2 traj)
	{
		for (int i = 0; i < 10; i++)
		{
			ParticleMan.AddParticle(0, loc, Rand.GetRandomVec2(-150f, 150f, -150f, 150f) + traj * Rand.GetRandomFloat(0f, 0.3f), 0, Rand.GetRandomFloat(0.2f, 0.5f), 0);
			ParticleMan.AddParticle(0, loc, Rand.GetRandomVec2(-150f, 150f, -150f, 150f), 0, Rand.GetRandomFloat(0.2f, 0.5f), 0);
		}
	}

	public static void MakeBloodSplode(Vector2 loc, int reps, float size, float traj)
	{
		for (int i = 0; i < reps; i++)
		{
			ParticleMan.AddParticle(0, loc, Rand.GetRandomVec2(0f - traj, traj, 0f - traj, traj), 0, size, 0);
		}
	}

	private static void MakePixelSplode(Vector2 loc, int reps, float size, float traj)
	{
		for (int i = 0; i < reps; i++)
		{
			ParticleMan.AddParticle(4, loc, Rand.GetRandomVec2(0f - traj, traj, 0f - traj, traj), 0, size, 0);
		}
	}

	private static void MakeGoo(Vector2 loc, int reps, float size, float traj)
	{
		for (int i = 0; i < reps; i++)
		{
			ParticleMan.AddParticle(5, loc, Rand.GetRandomVec2(0f - traj, traj, 0f - traj, traj), 0, size, 0);
		}
	}
}
