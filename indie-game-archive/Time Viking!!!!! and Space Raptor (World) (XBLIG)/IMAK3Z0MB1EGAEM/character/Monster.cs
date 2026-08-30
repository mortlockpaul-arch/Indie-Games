using System;
using IMAK3Z0MB1EGAEM.director;
using IMAK3Z0MB1EGAEM.map;
using IMAK3Z0MB1EGAEM.particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;
using Yuki_Win;

namespace IMAK3Z0MB1EGAEM.character;

public class Monster
{
	public const int MONSTERTYPE_ZOMBIE = 0;

	public const int MONSTERTYPE_FATTY = 1;

	public const int MONSTERTYPE_BIGASTEROID = 2;

	public const int MONSTERTYPE_MIDASTEROID = 3;

	public const int MONSTERTYPE_LITTLEASTEROID = 4;

	public const int MONSTERTYPE_BIGGOO = 5;

	public const int MONSTERTYPE_LITTLEGOO = 6;

	public const int MONSTERTYPE_FACETRAIL = 7;

	public const int MONSTERTYPE_BOMBER = 8;

	public const int MONSTERTYPE_BLARTARD = 9;

	public const int MONSTERTYPE_GEODE = 10;

	public int type;

	private Legs legs;

	public bool exists;

	private Vector2 traj;

	public Vector2 loc;

	private float angle;

	private Vector2 shoot;

	public int idx;

	public int targ;

	public int hp;

	private float atraj;

	private Vector2 vtarg;

	public float spawnFrame;

	private float frame;

	private float blarframe;

	private int iAngle;

	private float grace;

	public float age;

	public Monster(int idx)
	{
		legs = new Legs();
		this.idx = idx;
	}

	public void Init(Vector2 loc, int type)
	{
		Init(loc, type, midspawn: false);
	}

	public void Init(Vector2 loc, int type, bool midspawn)
	{
		age = 0f;
		exists = true;
		this.loc = loc;
		legs.frame = 0f;
		targ = Rand.GetRandomInt(0, 4);
		this.type = type;
		spawnFrame = (midspawn ? 1f : 0f);
		traj = default(Vector2);
		grace = 2f;
		switch (type)
		{
		case 7:
			frame = 0.1f;
			iAngle = Rand.GetRandomInt(0, 4);
			angle = (float)iAngle * 1.57f;
			hp = 5;
			break;
		case 0:
			if (spawnFrame > 0f)
			{
				spawnFrame = 0.5f;
			}
			break;
		case 8:
			hp = 3;
			if (spawnFrame > 0f)
			{
				spawnFrame = 0.5f;
			}
			break;
		case 10:
			traj = Rand.GetRandomVec2(-100f, 100f, -100f, 100f);
			atraj = Rand.GetRandomFloat(-2f, 2f);
			break;
		case 2:
		case 3:
		case 4:
		{
			float randomFloat = Rand.GetRandomFloat(0f, 6.28f);
			traj = new Vector2((float)Math.Cos(randomFloat), (float)Math.Sin(randomFloat));
			switch (type)
			{
			case 2:
				traj *= 100f;
				atraj = Rand.GetRandomFloat(-1f, 1f);
				break;
			case 3:
				traj *= 150f;
				atraj = Rand.GetRandomFloat(-2f, 2f);
				break;
			case 4:
				traj *= 200f;
				atraj = Rand.GetRandomFloat(-3f, 3f);
				break;
			}
			break;
		}
		case 6:
			vtarg = loc + Rand.GetRandomVec2(-100f, 100f, -100f, 100f);
			targ = -1;
			if (spawnFrame > 0f)
			{
				spawnFrame = 0.5f;
			}
			break;
		case 5:
			hp = 10;
			if (spawnFrame > 0f)
			{
				spawnFrame = 0.5f;
			}
			break;
		case 1:
			hp = 5;
			if (spawnFrame > 0f)
			{
				spawnFrame = 0.5f;
			}
			break;
		case 9:
			break;
		}
	}

	public static Vector2 GetCornerVec()
	{
		float num = 400f;
		if (Rand.CoinToss(0.5f))
		{
			if (Rand.CoinToss(0.5f))
			{
				return Rand.GetRandomVec2(0f, MapMan.mapSize.X, 0f, 0f) + new Vector2(0f, 0f - num);
			}
			return Rand.GetRandomVec2(0f, MapMan.mapSize.X, MapMan.mapSize.Y, MapMan.mapSize.Y) + new Vector2(0f, num);
		}
		if (Rand.CoinToss(0.5f))
		{
			return Rand.GetRandomVec2(0f, 0f, 0f, MapMan.mapSize.Y) + new Vector2(0f - num, 0f);
		}
		return Rand.GetRandomVec2(MapMan.mapSize.X, MapMan.mapSize.X, 0f, MapMan.mapSize.Y) + new Vector2(num, 0f);
	}

	public void Update()
	{
		if (!exists)
		{
			return;
		}
		if (spawnFrame > 0f)
		{
			spawnFrame -= FMan.frameTime;
		}
		if (float.IsNaN(loc.X) || float.IsNaN(loc.Y))
		{
			exists = false;
		}
		age += FMan.frameTime;
		Vector2 vector = default(Vector2);
		Vector2 vector2 = MapMan.mapSize;
		bool flag = GameState.state == GameState.State.EndlessZombiesPlaying;
		float num = 1f;
		if (flag)
		{
			vector = ZombieGame.GetEndlessRoomTL();
			vector2 = ZombieGame.GetEndlessRoomBR();
			int endlessRound = ZombieGame.GetEndlessRound();
			if (endlessRound > 1)
			{
				num += (float)(endlessRound - 1) * 0.1f;
			}
		}
		switch (type)
		{
		case 7:
		case 9:
		{
			if (spawnFrame > 0f)
			{
				break;
			}
			float num8 = frame;
			frame -= FMan.frameTime * num;
			if (type == 7)
			{
				if ((int)(frame * 10f) != (int)(num8 * 10f))
				{
					ParticleMan.AddParticle(6, loc, default(Vector2), idx, 0f, 0);
					loc += new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * new Vector2(25f, 33f);
				}
			}
			else if (type == 9)
			{
				blarframe += FMan.frameTime * 10f;
				if (blarframe >= 4f)
				{
					blarframe -= 4f;
				}
				loc += new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * FMan.frameTime * 80f;
			}
			if (frame < 0f)
			{
				frame += (float)Rand.GetRandomInt(5, 10) * 0.1f;
				if (Rand.CoinToss(0.5f))
				{
					iAngle = (iAngle + 1) % 4;
				}
				else
				{
					iAngle = (iAngle + 3) % 4;
				}
				float num9 = 200f;
				if (loc.X > vector2.X - num9 && iAngle == 0)
				{
					iAngle = 2;
				}
				if (loc.X < vector.X + num9 && iAngle == 2)
				{
					iAngle = 0;
				}
				if (loc.Y > vector2.Y - num9 && iAngle == 1)
				{
					iAngle = 3;
				}
				if (loc.Y < vector.Y + num9 && iAngle == 3)
				{
					iAngle = 1;
				}
				angle = (float)iAngle * 1.57f;
			}
			break;
		}
		case 10:
			if (!CharMan.hero[targ].exists)
			{
				if (Rand.CoinToss(0.5f))
				{
					targ = (targ + 1) % 4;
				}
				else
				{
					targ = (targ + 3) % 4;
				}
			}
			if (CharMan.hero[targ].exists)
			{
				float num7 = Trig.GetAngle(loc, CharMan.hero[targ].loc);
				Vector2 vector3 = new Vector2((float)Math.Cos(num7), (float)Math.Sin(num7)) * -300f;
				traj += (vector3 - traj) * FMan.frameTime * 0.3f;
			}
			angle += atraj * FMan.frameTime * num;
			while (angle > 6.28f)
			{
				angle -= 6.28f;
			}
			while (angle < 0f)
			{
				angle += 6.28f;
			}
			loc += traj * FMan.frameTime;
			if (loc.X < vector.X && traj.X < 0f)
			{
				traj.X = 0f - traj.X;
			}
			if (loc.X > vector2.X && traj.X > 0f)
			{
				traj.X = 0f - traj.X;
			}
			if (loc.Y < vector.Y && traj.Y < 0f)
			{
				traj.Y = 0f - traj.Y;
			}
			if (loc.Y > vector2.Y && traj.Y > 0f)
			{
				traj.Y = 0f - traj.Y;
			}
			break;
		case 2:
		case 3:
		case 4:
		{
			angle += atraj * FMan.frameTime;
			while (angle > 6.28f)
			{
				angle -= 6.28f;
			}
			while (angle < 0f)
			{
				angle += 6.28f;
			}
			float num6 = 10f;
			loc += traj * FMan.frameTime * num;
			if (loc.X < vector.X - num6)
			{
				loc.X = vector2.X + num6;
			}
			if (loc.Y < vector.Y - num6)
			{
				loc.Y = vector2.Y + num6;
			}
			if (loc.X > vector2.X + num6)
			{
				loc.X = vector.X - num6;
			}
			if (loc.Y > vector2.Y + num6)
			{
				loc.Y = vector.Y - num6;
			}
			break;
		}
		default:
		{
			if (targ < 0)
			{
				traj = vtarg - loc;
				if (traj.Length() < 20f)
				{
					targ = Rand.GetRandomInt(0, 4);
				}
			}
			else if (CharMan.hero[targ].exists)
			{
				traj = CharMan.hero[targ].loc - loc;
				if (CharMan.hero[targ].respawnFrame > 0f)
				{
					traj *= -1f;
					if (loc.X < vector.X)
					{
						loc.X = vector.X;
					}
					if (loc.Y < vector.Y)
					{
						loc.Y = vector.Y;
					}
					if (loc.X > vector2.X)
					{
						loc.X = vector2.X;
					}
					if (loc.Y > vector2.Y)
					{
						loc.Y = vector2.Y;
					}
				}
			}
			else if (Rand.CoinToss(0.5f))
			{
				targ = (targ + 1) % 4;
			}
			else
			{
				targ = (targ + 3) % 4;
			}
			traj.Normalize();
			if (spawnFrame > 0f)
			{
				int num2 = type;
				if (num2 == 0 || num2 == 8)
				{
					traj = default(Vector2);
					angle = spawnFrame * 9f;
				}
			}
			legs.traj = traj;
			switch (type)
			{
			case 5:
				legs.Update(0.8f);
				break;
			case 6:
				legs.Update(1.2f);
				break;
			case 8:
				legs.Update(1.2f);
				break;
			default:
				legs.Update();
				break;
			}
			float num3 = 80f;
			switch (type)
			{
			case 8:
				num3 = 90f;
				break;
			case 6:
				num3 = 75f;
				break;
			case 5:
				num3 = 35f;
				break;
			}
			loc += traj * FMan.frameTime * num3 * num;
			float num4 = angle;
			if (shoot.Length() > 0.2f)
			{
				num4 = Trig.GetAngle(default(Vector2), shoot);
			}
			else if (traj.Length() > 0f)
			{
				num4 = Trig.GetAngle(default(Vector2), traj);
				switch (type)
				{
				default:
					num4 += (float)Math.Sin(legs.frame) * 0.15f;
					break;
				case 5:
				case 6:
					break;
				}
			}
			float num5;
			for (num5 = num4 - angle; num5 < -3.14f; num5 += 6.28f)
			{
			}
			while (num5 > 3.14f)
			{
				num5 -= 6.28f;
			}
			angle += num5 * FMan.frameTime * 10f;
			break;
		}
		}
		if (grace > 0f)
		{
			grace -= FMan.frameTime;
		}
		else
		{
			HitManager.CheckHeroSmash(this);
		}
	}

	public void Draw()
	{
		if (!exists)
		{
			return;
		}
		switch (type)
		{
		case 1:
			legs.Draw(loc, 1.4f * ((spawnFrame > 0f) ? ((0.5f - spawnFrame) * 2f) : 1f));
			break;
		default:
			legs.Draw(loc, 1f * ((spawnFrame > 0f) ? ((0.5f - spawnFrame) * 2f) : 1f));
			break;
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 9:
		case 10:
			break;
		}
		float a = 1f;
		switch (type)
		{
		case 2:
		case 3:
		case 4:
			if (spawnFrame > 0f)
			{
				a = 1f - spawnFrame;
			}
			break;
		}
		switch (type)
		{
		case 9:
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(1152, 192 + (int)blarframe * 128, 128, 128), Color.White, 0f, new Vector2(64f, 64f), 0.4f * ScrollMan.zoom, SpriteEffects.None, 1f);
			break;
		case 10:
			if (spawnFrame > 0f)
			{
				SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(1024, 960, 192, 128), new Color(0.7f + 0.3f * (float)Math.Sin(angle), 0.7f + 0.3f * (float)Math.Sin(angle + 2f), 0.7f + 0.3f * (float)Math.Sin(angle + 4f), 0.5f), angle, new Vector2(96f, 64f), (0.3f + spawnFrame / 2f) * ScrollMan.zoom, SpriteEffects.None, 1f);
			}
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(1024, 960, 192, 128), new Color(0.7f + 0.3f * (float)Math.Sin(angle), 0.7f + 0.3f * (float)Math.Sin(angle + 2f), 0.7f + 0.3f * (float)Math.Sin(angle + 4f), 0.5f), angle, new Vector2(96f, 64f), 0.3f * ScrollMan.zoom, SpriteEffects.None, 1f);
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(1024, 960, 192, 128), new Color(1f, 1f, 1f, 0.5f), angle, new Vector2(96f, 64f), (1f - spawnFrame) * 0.3f * ScrollMan.zoom, SpriteEffects.None, 1f);
			break;
		case 7:
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(128, 1024, 128, 128), (spawnFrame > 0f) ? new Color(1f, (int)(spawnFrame * 8f) % 2, (int)(spawnFrame * 8f) % 2, 1f) : new Color(1f, 0f, 0f, 1f), 0f, new Vector2(64f, 64f), 0.25f * ScrollMan.zoom, SpriteEffects.None, 1f);
			break;
		case 5:
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(320, 768, 128, 128), Color.White, angle, new Vector2(64f, 64f), new Vector2((float)Math.Cos(legs.frame + Rand.GetRandomFloat(-0.5f, 0.5f)) * 0.08f + 0.7f, 0.6f) * 0.9f * ScrollMan.zoom * ((spawnFrame > 0f) ? ((0.5f - spawnFrame) * 2f) : 1f), SpriteEffects.None, 1f);
			break;
		case 6:
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(320, 768, 128, 128), Color.White, angle, new Vector2(64f, 64f), new Vector2((float)Math.Cos(legs.frame + Rand.GetRandomFloat(-0.5f, 0.5f)) * 0.25f + 0.7f, 0.6f) * 0.35f * ScrollMan.zoom * ((spawnFrame > 0f) ? ((0.5f - spawnFrame) * 2f) : 1f), SpriteEffects.None, 1f);
			break;
		case 8:
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(1152, 0, 128, 192), Color.White, angle, new Vector2(70f, 96f), 0.35f * ScrollMan.zoom * ((spawnFrame > 0f) ? ((0.5f - spawnFrame) * 2f) : 1f), SpriteEffects.None, 1f);
			break;
		case 0:
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(384, 128, 192, 192), Color.White, angle, new Vector2(142f, 96f), 0.3f * ScrollMan.zoom * ((spawnFrame > 0f) ? ((0.5f - spawnFrame) * 2f) : 1f), SpriteEffects.None, 1f);
			break;
		case 1:
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(832, 384, 192, 256), Color.White, angle, new Vector2(120f, 128f), 0.3f * ScrollMan.zoom * ((spawnFrame > 0f) ? ((0.5f - spawnFrame) * 2f) : 1f), SpriteEffects.None, 1f);
			break;
		case 2:
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(768, 768, 256, 256), new Color(1f, 1f, 1f, a), angle, new Vector2(128f, 128f), 0.3f * ScrollMan.zoom, SpriteEffects.None, 1f);
			break;
		case 3:
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(576, 768, 192, 192), new Color(1f, 1f, 1f, a), angle, new Vector2(96f, 96f), 0.3f * ScrollMan.zoom, SpriteEffects.None, 1f);
			break;
		case 4:
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(448, 768, 128, 128), new Color(1f, 1f, 1f, a), angle, new Vector2(64f, 64f), 0.3f * ScrollMan.zoom, SpriteEffects.None, 1f);
			break;
		}
	}
}
