using Microsoft.Xna.Framework;
using Viking_x86.director;

namespace Viking_x86.vikinggame;

public class SpawnMgr
{
	private Vector2 galagaVec;

	private float galagaFrame;

	private int galagaFace;

	public void SpawnGalaga()
	{
		if (!(galagaFrame > 0f))
		{
			galagaVec = Game1.vgame.charMgr.character[0].loc;
			if (Game1.vgame.charMgr.character[1].exists && (!Game1.vgame.charMgr.character[0].exists || Rand.CoinToss(0.5f)))
			{
				galagaVec = Game1.vgame.charMgr.character[1].loc;
			}
			galagaVec.X = Game1.vgame.world.towerX + 160f;
			galagaVec.Y -= 300f;
			float num = 380f;
			if (Rand.CoinToss(0.5f))
			{
				galagaVec.X += num;
				galagaFace = 0;
			}
			else
			{
				galagaVec.X -= num;
				galagaFace = 1;
			}
			galagaFrame = 3f;
		}
	}

	public void Update()
	{
		if (galagaFrame > 0f)
		{
			float num = galagaFrame;
			galagaFrame -= Game1.frameTime;
			if ((int)(num * 5f) != (int)(galagaFrame * 5f))
			{
				Game1.vgame.charMgr.Init(6, galagaVec, default(Vector2), 0, galagaFace, 1);
			}
		}
	}

	public void Spawnemy(int type)
	{
		if (type == 5)
		{
			for (int i = 0; i < Game1.vgame.charMgr.character.Length; i++)
			{
				if (Game1.vgame.charMgr.character[i].exists && Game1.vgame.charMgr.character[i].defID == 5)
				{
					return;
				}
			}
		}
		if (type == 6)
		{
			SpawnGalaga();
			return;
		}
		Vector2 vector = default(Vector2);
		Vector2 vector2 = default(Vector2);
		float num = 0f;
		vector = Game1.vgame.charMgr.character[0].loc;
		if (Game1.vgame.charMgr.character[1].exists && Game1.vgame.charMgr.character[1].loc.Y < Game1.vgame.charMgr.character[0].loc.Y)
		{
			vector = Game1.vgame.charMgr.character[1].loc;
		}
		switch (type)
		{
		case 2:
			vector += Rand.GetRandomVec2(-30f, 30f, -200f, 0f);
			num = 400f;
			vector2 = Rand.GetRandomVec2(200f, 300f, -300f, -250f);
			if (Rand.CoinToss(0.5f))
			{
				Game1.vgame.charMgr.Init(type, vector + new Vector2(0f - num, 0f), new Vector2(vector2.X, vector2.Y), 0, 1, 1);
			}
			else
			{
				Game1.vgame.charMgr.Init(type, vector + new Vector2(num, 0f), new Vector2(0f - vector2.X, vector2.Y), 0, 0, 1);
			}
			break;
		case 3:
			vector += Rand.GetRandomVec2(-30f, 30f, -320f, -200f);
			num = 300f;
			vector2 = new Vector2(200f, 0f);
			if (Rand.CoinToss(0.5f))
			{
				Game1.vgame.charMgr.Init(type, vector + new Vector2(0f - num, 0f), new Vector2(vector2.X, vector2.Y), 0, 1, 1);
			}
			else
			{
				Game1.vgame.charMgr.Init(type, vector + new Vector2(num, 0f), new Vector2(0f - vector2.X, vector2.Y), 0, 0, 1);
			}
			break;
		case 7:
		case 8:
			vector += Rand.GetRandomVec2(-30f, 30f, -320f, -200f);
			num = 300f;
			vector2 = new Vector2(200f, 0f);
			if (Rand.CoinToss(0.5f))
			{
				Game1.vgame.charMgr.Init(type, vector + new Vector2(0f - num, 0f), new Vector2(vector2.X, vector2.Y), 0, 1, 1);
			}
			else
			{
				Game1.vgame.charMgr.Init(type, vector + new Vector2(num, 0f), new Vector2(0f - vector2.X, vector2.Y), 0, 0, 1);
			}
			break;
		case 4:
			vector.X = Rand.GetRandomFloat(Game1.vgame.world.towerX, Game1.vgame.world.towerX + 320f);
			vector.Y -= 400f;
			vector2.Y = 50f;
			Game1.vgame.charMgr.Init(type, vector, new Vector2(vector2.X, vector2.Y), 0, 1, 1);
			break;
		case 5:
			vector.X = Game1.vgame.world.towerX + 160f;
			vector.Y += 300f;
			num = 200f;
			vector2 = new Vector2(0f, 0f);
			if (Rand.CoinToss(0.5f))
			{
				Game1.vgame.charMgr.Init(type, vector + new Vector2(0f - num, 0f), new Vector2(vector2.X, vector2.Y), 0, 1, 1);
			}
			else
			{
				Game1.vgame.charMgr.Init(type, vector + new Vector2(num, 0f), new Vector2(0f - vector2.X, vector2.Y), 0, 0, 1);
			}
			break;
		case 6:
			vector += Rand.GetRandomVec2(-30f, 30f, -320f, -200f);
			num = 300f;
			vector2 = new Vector2(200f, 0f);
			if (Rand.CoinToss(0.5f))
			{
				Game1.vgame.charMgr.Init(type, vector + new Vector2(0f - num, 0f), new Vector2(vector2.X, vector2.Y), 0, 1, 1);
			}
			else
			{
				Game1.vgame.charMgr.Init(type, vector + new Vector2(num, 0f), new Vector2(0f - vector2.X, vector2.Y), 0, 0, 1);
			}
			break;
		}
	}

	internal void SpawnPickup()
	{
		switch (TimeMgr.VikingTMgr().phase)
		{
		case 5:
			if (!Game1.vgame.charMgr.moon.active)
			{
				return;
			}
			break;
		case 6:
			return;
		}
		Game1.vgame.pMgr.AddParticle(15, Game1.vgame.world.risingBaseVec + Rand.GetRandomVec2(-160f, 160f, -600f, -600f), new Vector2(0f, 50f), 0f, Rand.GetRandomInt(0, 5), 0);
	}
}
