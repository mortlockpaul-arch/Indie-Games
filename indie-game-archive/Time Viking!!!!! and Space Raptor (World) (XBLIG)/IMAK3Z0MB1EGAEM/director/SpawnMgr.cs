using IMAK3Z0MB1EGAEM.character;
using IMAK3Z0MB1EGAEM.map;
using IMAK3Z0MB1EGAEM.particles;
using Microsoft.Xna.Framework;
using Viking_x86;
using Viking_x86.director;

namespace IMAK3Z0MB1EGAEM.director;

public class SpawnMgr
{
	private static int toggle;

	public static void DoClick(int phase, int beats)
	{
		switch (TimeMgr.time)
		{
		case 0:
			ZombieClick(phase, beats);
			break;
		case 1:
			VikingClick(phase, beats);
			break;
		}
	}

	private static void VikingClick(int phase, int beats)
	{
		switch (phase)
		{
		case 0:
			if (beats >= 32)
			{
				VikingPop(2, 1);
			}
			if (beats >= 96)
			{
				VikingPop(2, 1);
			}
			if (beats >= 192)
			{
				if (beats % 2 == 0)
				{
					VikingPop(7, 1);
				}
				else
				{
					VikingPop(2, 1);
				}
			}
			break;
		case 1:
			if (beats / 8 < 30)
			{
				VikingPop(4, 1);
				if (beats % 2 == 0)
				{
					VikingPop(2, 1);
				}
				if (beats / 8 >= 20)
				{
					VikingPop(3, 2);
				}
				if (beats / 8 >= 16 && beats / 8 < 20 && beats % 2 == 0)
				{
					VikingPop(7, 1);
				}
			}
			break;
		case 2:
			if (beats < 16)
			{
				break;
			}
			if (beats % 4 == 0)
			{
				VikingPop(6, 1);
			}
			if (beats / 8 < 8)
			{
				break;
			}
			if (beats % 4 == 2)
			{
				VikingPop(6, 1);
			}
			VikingPop(2, 1);
			if (beats / 8 >= 28)
			{
				VikingPop(2, 1);
				if (beats % 16 == 0)
				{
					VikingPop(8, 1);
				}
				else
				{
					VikingPop(3, 1);
				}
			}
			break;
		case 3:
			if (beats / 8 >= 24 && beats % 16 == 0)
			{
				VikingPop(7, 1);
			}
			if (beats / 8 >= 4)
			{
				if (beats % 16 == 0)
				{
					VikingPop(5, 1);
				}
				VikingPop(2, 1);
				if (beats % 16 == 8)
				{
					VikingPop(8, 1);
				}
				VikingPop(3, 1);
				switch (beats / 8)
				{
				case 12:
				case 13:
				case 14:
				case 15:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
				case 33:
				case 34:
				case 35:
				case 36:
				case 37:
				case 38:
				case 39:
					VikingPop(4, 1);
					break;
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
					break;
				}
			}
			break;
		case 4:
			if (beats / 8 < 2)
			{
				break;
			}
			if (beats % 2 == 0)
			{
				VikingPop(7, 2);
			}
			if (beats / 8 >= 20)
			{
				if (beats % 2 == 0)
				{
					VikingPop(2, 2);
				}
				if (beats % 16 == 0)
				{
					VikingPop(8, 1);
				}
			}
			break;
		case 5:
			if (beats / 8 >= 60 || beats / 8 < 2)
			{
				break;
			}
			VikingPop(2, 2);
			if (Game1.vgame.charMgr.moon.active)
			{
				if (beats / 8 % 4 == 0)
				{
					VikingPop(3, 1);
				}
				if (Game1.vgame.charMgr.moon.GetDif() < 500f)
				{
					VikingPop(2, 2);
				}
				else if (beats / 4 % 4 == 0)
				{
					if (beats % 2 == 0)
					{
						VikingPop(2, 1);
					}
					else
					{
						VikingPop(7, 1);
					}
				}
			}
			else
			{
				VikingPop(3, 1);
				if (beats % 16 == 0)
				{
					VikingPop(5, 1);
				}
				if (beats % 2 == 0)
				{
					VikingPop(7, 2);
				}
				else
				{
					VikingPop(2, 2);
				}
			}
			break;
		case 6:
		{
			if (beats != 0)
			{
				break;
			}
			Game1.vgame.pMgr.Reset();
			for (int i = 0; i < 2; i++)
			{
				if (Game1.vgame.charMgr.character[i].exists)
				{
					Game1.vgame.charMgr.character[i].score += Game1.vgame.charMgr.character[i].lives * 10000;
					Game1.vgame.charMgr.character[i].lives = 0;
					Game1.vgame.charMgr.character[i].KillChar();
				}
			}
			Game1.vgame.charMgr.ClearMonsters();
			break;
		}
		}
	}

	private static void VikingPop(int type, int count)
	{
		if (Game1.vgame.charMgr.character[0].exists && Game1.vgame.charMgr.character[1].exists)
		{
			toggle = 1 - toggle;
			if (toggle == 0 && type == 2)
			{
				Game1.vgame.spawnMgr.Spawnemy(type);
			}
		}
		for (int i = 0; i < count; i++)
		{
			Game1.vgame.spawnMgr.Spawnemy(type);
		}
	}

	private static void ZombieClick(int phase, int beats)
	{
		switch (phase)
		{
		case 0:
			switch (beats)
			{
			case 4:
			case 32:
			case 64:
			case 128:
			case 192:
				MakeGoodies(4);
				break;
			case 224:
				MakeGoodies(8);
				break;
			}
			switch (beats)
			{
			case 0:
			case 4:
			case 8:
			case 16:
				Spawn(0, 16);
				Pop(0, 4);
				break;
			case 24:
			case 32:
			case 40:
			case 48:
				Spawn(0, 36);
				Spawn(1, 1);
				Pop(0, 2);
				break;
			case 64:
			case 68:
			case 72:
			case 74:
			case 76:
				Pop(0, 24);
				Pop(1, 1);
				break;
			case 80:
			case 88:
			case 96:
			case 104:
			case 112:
				Spawn(0, 24);
				Pop(1, 2);
				break;
			case 120:
			case 124:
			case 132:
			case 140:
				Spawn(0, 24);
				Spawn(1, 1);
				Pop(0, 2);
				break;
			case 144:
			case 148:
			case 152:
			case 154:
			case 156:
			case 160:
			case 164:
			case 168:
			case 170:
			case 172:
			case 176:
			case 180:
			case 184:
			case 186:
			case 188:
				Pop(0, 16);
				Pop(1, 1);
				break;
			case 192:
			case 193:
			case 194:
			case 195:
			case 196:
			case 197:
			case 198:
			case 199:
			case 200:
			case 201:
			case 202:
			case 203:
			case 204:
			case 205:
			case 206:
			case 207:
			case 208:
			case 209:
			case 210:
			case 211:
			case 212:
			case 213:
			case 214:
			case 215:
			case 216:
			case 217:
			case 218:
			case 219:
			case 220:
			case 221:
			case 222:
			case 223:
				Pop(0, 3);
				break;
			}
			break;
		case 1:
			switch (beats)
			{
			case 0:
				MakeGoodies(8);
				break;
			case 4:
			case 112:
				MakeGoodies(2);
				break;
			case 96:
			case 160:
				MakeGoodies(4);
				break;
			}
			switch (beats)
			{
			case 16:
			case 17:
			case 18:
			case 19:
			case 32:
			case 33:
			case 34:
			case 35:
			case 48:
			case 49:
			case 50:
			case 51:
			case 64:
			case 65:
			case 66:
			case 67:
				Pop(5, 1);
				Pop(7, 1);
				break;
			case 96:
			case 97:
			case 100:
			case 101:
			case 104:
			case 105:
			case 108:
			case 109:
				Pop(1, 2);
				Spawn(0, 10);
				Pop(0, 10);
				break;
			case 112:
			case 113:
			case 114:
			case 115:
			case 128:
			case 129:
			case 130:
			case 131:
			case 144:
			case 145:
			case 146:
			case 147:
			case 160:
			case 161:
			case 162:
			case 163:
				Pop(5, 1);
				Pop(7, 1);
				break;
			case 164:
			case 165:
			case 168:
			case 169:
			case 172:
			case 173:
				Pop(1, 2);
				Spawn(0, 10);
				Pop(0, 10);
				break;
			case 176:
				Pop(1, 2);
				Spawn(0, 10);
				Pop(0, 10);
				Pop(5, 4);
				Pop(7, 4);
				break;
			}
			break;
		case 2:
			switch (beats)
			{
			case 0:
				ClearMonsters();
				MakeGoodies(12);
				break;
			case 64:
			case 128:
			case 192:
			case 256:
				MakeGoodies(4);
				break;
			}
			switch (beats)
			{
			case 0:
			case 16:
			case 32:
			case 48:
			case 64:
			case 96:
			case 128:
				Spawn(2, 7);
				break;
			}
			break;
		case 3:
			switch (beats)
			{
			case 0:
				ClearMonsters();
				MakeGoodies(4);
				break;
			case 32:
			case 64:
			case 96:
			case 128:
			case 160:
			case 192:
			case 224:
			case 256:
				MakeGoodies(4);
				break;
			}
			if (beats % 4 == 0)
			{
				Pop(8, 4);
				Spawn(8, 4);
			}
			switch (beats)
			{
			case 0:
			case 32:
				Pop(1, 2);
				Spawn(0, 20);
				Pop(0, 20);
				Spawn(8, 20);
				Pop(8, 20);
				break;
			case 16:
				Pop(5, 6);
				break;
			case 64:
			case 128:
				Pop(1, 2);
				Spawn(0, 20);
				Pop(0, 20);
				Pop(7, 4);
				Spawn(8, 20);
				Pop(8, 20);
				break;
			case 96:
				Pop(5, 6);
				break;
			case 160:
			case 192:
				Pop(1, 2);
				Spawn(0, 20);
				Pop(0, 20);
				Spawn(8, 10);
				Pop(8, 10);
				break;
			case 176:
				Spawn(8, 10);
				Pop(8, 10);
				break;
			case 224:
			case 256:
				Pop(1, 2);
				Spawn(0, 20);
				Pop(0, 20);
				Pop(7, 4);
				Spawn(8, 10);
				Pop(8, 10);
				break;
			case 240:
				Spawn(8, 10);
				Pop(8, 10);
				break;
			case 288:
			case 320:
				Pop(1, 2);
				Spawn(0, 20);
				Pop(0, 20);
				Pop(7, 4);
				Spawn(8, 10);
				Pop(8, 10);
				break;
			case 304:
				Spawn(8, 10);
				Pop(8, 10);
				break;
			}
			break;
		case 4:
			switch (beats)
			{
			case 0:
			case 32:
			case 64:
			case 128:
			case 192:
				MakeGoodies(4);
				break;
			}
			switch (beats)
			{
			case 0:
			case 4:
			case 8:
			case 12:
			case 16:
			case 20:
			case 24:
			case 28:
				Pop(10, 3);
				Spawn(10, 3);
				break;
			case 32:
			case 36:
			case 40:
			case 44:
			case 48:
			case 52:
			case 56:
			case 60:
				Pop(10, 3);
				Spawn(10, 3);
				Pop(5, 1);
				break;
			case 64:
			case 68:
			case 72:
			case 76:
			case 80:
			case 84:
			case 88:
			case 92:
				Pop(10, 4);
				Spawn(10, 4);
				break;
			case 96:
			case 100:
			case 104:
			case 108:
			case 112:
			case 116:
			case 120:
			case 124:
				Pop(10, 5);
				Spawn(10, 5);
				Pop(5, 1);
				break;
			case 128:
			case 132:
			case 136:
			case 140:
			case 144:
			case 148:
			case 152:
			case 156:
				Pop(10, 3);
				Spawn(10, 3);
				Pop(5, 1);
				Pop(7, 1);
				break;
			case 160:
			case 164:
			case 168:
			case 172:
			case 176:
			case 180:
			case 184:
			case 188:
			case 192:
			case 196:
			case 200:
			case 204:
			case 208:
			case 212:
			case 216:
			case 220:
				Pop(10, 1);
				Spawn(10, 1);
				Pop(5, 1);
				Pop(7, 1);
				break;
			}
			break;
		case 5:
			switch (beats)
			{
			case 0:
			case 32:
			case 64:
			case 96:
				MakeGoodies(4);
				break;
			}
			switch (beats)
			{
			case 0:
			case 4:
			case 8:
			case 12:
			case 16:
			case 20:
			case 24:
			case 28:
				Pop(9, 8);
				Pop(7, 1);
				break;
			case 32:
			case 36:
			case 40:
			case 44:
			case 48:
			case 52:
			case 56:
			case 60:
				Pop(9, 16);
				Pop(7, 1);
				Pop(5, 1);
				break;
			case 64:
			case 68:
			case 72:
			case 76:
			case 80:
			case 84:
			case 88:
			case 92:
				Pop(9, 8);
				Pop(7, 1);
				break;
			case 96:
			case 100:
			case 104:
			case 108:
			case 112:
			case 116:
			case 120:
			case 124:
				Pop(9, 16);
				Pop(7, 1);
				Pop(5, 1);
				break;
			}
			break;
		case 6:
			switch (beats)
			{
			case 0:
				MakeGoodies(8);
				break;
			case 54:
				MakeGoodies(8);
				break;
			case 196:
			case 212:
			case 220:
			case 228:
				MakeGoodies(8);
				break;
			}
			switch (beats)
			{
			case 4:
			case 8:
			case 12:
			case 14:
			case 16:
			case 20:
			case 24:
			case 28:
			case 30:
			case 32:
				Pop(0, 24);
				Pop(1, 1);
				break;
			case 36:
			case 38:
			case 40:
			case 42:
			case 44:
				Pop(0, 20);
				Pop(1, 1);
				Pop(8, 4);
				break;
			case 54:
			case 62:
			case 70:
			case 78:
			case 84:
			case 92:
			case 100:
			case 108:
				Spawn(0, 24);
				Pop(8, 24);
				Pop(1, 2);
				Pop(5, 2);
				break;
			case 116:
			case 124:
			case 132:
			case 140:
			case 148:
			case 156:
			case 164:
			case 172:
			case 180:
			case 188:
			case 196:
				Spawn(0, 12);
				Pop(8, 4);
				Pop(1, 2);
				Pop(5, 2);
				Pop(9, 2);
				Pop(7, 1);
				break;
			case 200:
			case 204:
			case 208:
			case 212:
			case 214:
			case 218:
			case 222:
			case 226:
			case 230:
			case 234:
			case 238:
				Spawn(0, 12);
				Pop(8, 4);
				Pop(1, 2);
				Pop(5, 2);
				Pop(9, 4);
				Pop(7, 1);
				break;
			case 242:
			case 248:
			case 252:
			case 256:
				Spawn(0, 24);
				Pop(8, 4);
				Pop(1, 2);
				Pop(5, 2);
				Pop(9, 4);
				Pop(7, 1);
				break;
			case 260:
				ClearMonsters();
				break;
			}
			break;
		case 7:
		{
			if (beats != 0)
			{
				break;
			}
			ClearMonsters();
			for (int i = 0; i < ParticleMan.particle.Length; i++)
			{
				ParticleMan.particle[i].exists = false;
			}
			for (int j = 0; j < CharMan.hero.Length; j++)
			{
				if (CharMan.hero[j].exists)
				{
					CharMan.hero[j].score += CharMan.hero[j].lives * 10000;
					CharMan.hero[j].lives = 1;
					CharMan.hero[j].Kill();
				}
			}
			break;
		}
		}
	}

	private static void ClearMonsters()
	{
		for (int i = 0; i < CharMan.monster.Length; i++)
		{
			if (CharMan.monster[i].exists)
			{
				CharMan.monster[i].exists = false;
			}
		}
	}

	private static void MakeGoodies(int count)
	{
		for (int i = 0; i < count; i++)
		{
			if (Rand.CoinToss(0.3f))
			{
				ParticleMan.AddParticle(8, Rand.GetRandomVec2(400f, MapMan.mapSize.X - 400f, 400f, MapMan.mapSize.Y - 400f), default(Vector2), 0, 0f, Rand.GetRandomInt(0, 6));
			}
		}
	}

	public static void MakeGoodie(Vector2 loc)
	{
		ParticleMan.AddParticle(8, loc, default(Vector2), 0, 0f, Rand.GetRandomInt(0, 9));
	}

	private static int GetAdjustedCount(int count)
	{
		int num = 0;
		for (int i = 0; i < CharMan.hero.Length; i++)
		{
			if (CharMan.hero[i].exists && CharMan.hero[i].lives > 0)
			{
				num++;
			}
		}
		switch (num)
		{
		case 0:
		case 1:
			if (count < 3)
			{
				return count;
			}
			return FloatToInt((float)count * 0.75f);
		case 2:
			return count;
		case 3:
			return FloatToInt((float)count * 1.5f);
		case 4:
			return FloatToInt((float)count * 2f);
		default:
			return (int)((float)count * 0.7f);
		}
	}

	private static int FloatToInt(float v)
	{
		int num = (int)v;
		float chanceToSucceed = v - (float)num;
		return num + (Rand.CoinToss(chanceToSucceed) ? 1 : 0);
	}

	private static void Spawn(int type, int count)
	{
		count = GetAdjustedCount(count);
		for (int i = 0; i < count; i++)
		{
			CharMan.MakeMonster(Monster.GetCornerVec(), type);
		}
	}

	public static void Pop(int type, int count)
	{
		count = GetAdjustedCount(count);
		for (int i = 0; i < count; i++)
		{
			Vector2 randomVec = Rand.GetRandomVec2(0f, MapMan.mapSize.X, 0f, MapMan.mapSize.Y);
			if (GameState.state == GameState.State.EndlessZombiesPlaying)
			{
				int endlessRoomX = ZombieGame.GetEndlessRoomX();
				int endlessRoomY = ZombieGame.GetEndlessRoomY();
				float num = 100f;
				Vector2 vector = new Vector2((float)endlessRoomX * 64f * 16f * 1.2f, (float)endlessRoomY * 64f * 12f * 1.2f) + new Vector2(num, num);
				Vector2 vector2 = new Vector2((float)(endlessRoomX + 1) * 64f * 16f * 1.2f, (float)(endlessRoomY + 1) * 64f * 12f * 1.2f) - new Vector2(num, num);
				randomVec = Rand.GetRandomVec2(vector.X, vector2.X, vector.Y, vector2.Y);
			}
			CharMan.MakeMonster(randomVec, type, midSpawn: true);
		}
	}
}
