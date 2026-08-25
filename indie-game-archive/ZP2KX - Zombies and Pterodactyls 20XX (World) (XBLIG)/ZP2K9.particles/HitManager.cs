using Microsoft.Xna.Framework;
using ZP2K9.ai;
using ZP2K9.characters;
using ZP2K9.characters.weapons;
using ZP2K9.debug;
using ZP2K9.map;

namespace ZP2K9.particles;

public class HitManager
{
	public static void CheckNetFixHit(Particle p)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (p.netOwner <= -1 || p.netOwner >= Game1.character.Length || Game1.character[p.netOwner] == null)
		{
			return;
		}
		Vector2 val = Game1.character[p.netOwner].loc - new Vector2(0f, 48f);
		Vector2 val2 = p.loc - val;
		Vector2 loc = p.loc;
		if (((Vector2)(ref val2)).Length() > 20f)
		{
			int num = (int)(((Vector2)(ref val2)).Length() / 15f) + 1;
			for (int i = 0; i < num; i++)
			{
				float num2 = (float)(i + 1) / (float)num;
				Vector2 loc2 = val + val2 * num2;
				p.loc = loc2;
				p.Update(Game1.gameMap, Game1.character, Game1.frameTime);
				if (p.frame < 0f)
				{
					return;
				}
			}
		}
		p.loc = loc;
	}

	public static bool GetHostile(int i, int j)
	{
		if (i == j)
		{
			return true;
		}
		if (Game1.netSession.GetPlayerOne() == i && (DebugManager.botsIgnore || DebugManager.godMode))
		{
			return false;
		}
		if (GameState.gameType == 0)
		{
			return true;
		}
		if (Game1.character[i] == null || Game1.character[j] == null)
		{
			return false;
		}
		if (Game1.character[i].team != Game1.character[j].team)
		{
			return true;
		}
		return false;
	}

	public static bool CheckHit(Character[] c, Particle p, GameMap map, int owner)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_086c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0896: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0937: Unknown result type (might be due to invalid IL or missing references)
		//IL_093c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0941: Unknown result type (might be due to invalid IL or missing references)
		//IL_0948: Unknown result type (might be due to invalid IL or missing references)
		//IL_0955: Unknown result type (might be due to invalid IL or missing references)
		//IL_095a: Unknown result type (might be due to invalid IL or missing references)
		//IL_095f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0972: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (p.impotent)
		{
			return false;
		}
		for (int i = 0; i < c.Length; i++)
		{
			if ((i == owner && p.type != 5 && p.type != 15 && p.type != 28 && p.type != 37 && p.type != 4) || c[i] == null)
			{
				continue;
			}
			int num = p.flags;
			if (!GetHostile(owner, i) || !(c[i].dyingFrame <= 0f) || c[i].hp < 0 || !(c[i].respawnFrame <= 0f) || !(c[i].spawnFrame <= 0f))
			{
				continue;
			}
			bool flag = false;
			if (p.type == 28 || p.type == 5 || p.type == 30 || p.type == 37 || p.type == 44 || p.type == 65)
			{
				Vector2 val = c[i].loc - new Vector2(0f, 30f) - p.loc;
				float num2 = ((Vector2)(ref val)).Length();
				if (num2 < p.size)
				{
					if (AI.GetVis(p.loc, c[i].loc - new Vector2(0f, 30f), map))
					{
						flag = true;
						if (p.type != 44 && p.type != 65)
						{
							num = (int)((p.size - num2) / p.size * (float)p.flags);
						}
					}
					else if (p.type == 28 && p.size >= 500f && num2 < 250f)
					{
						if (c[i].fire < 5f)
						{
							c[i].fire = 5f;
						}
						c[i].fireOwner = p.netOwner;
						c[i].StartKill(Rand.GetRandomVec2(-100f, 100f, -100f, -100f));
					}
				}
			}
			else
			{
				Vector2 val2 = p.loc;
				if (owner > -1 && owner < c.Length)
				{
					Vector2 trailVec = c[i].GetTrailVec(c[owner].latency);
					val2 += c[i].loc - trailVec;
				}
				for (int j = 0; j < 2; j++)
				{
					if (((Rectangle)(ref c[i].hitRects[j])).Contains((int)val2.X, (int)val2.Y))
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				continue;
			}
			if (p.type == 25 || p.type == 10 || p.type == 37 || p.type == 35 || p.type == 55)
			{
				num = 0;
			}
			int hp = c[i].hp;
			if (c[i].shrink > 0f)
			{
				num *= 5;
			}
			switch (c[i].perk[2])
			{
			case 1:
				if (p.type == 28)
				{
					num /= 2;
				}
				break;
			case 3:
				if (p.type == 19 || p.type == 27 || p.type == 21)
				{
					num--;
					if (Rand.CointToss(0.5f))
					{
						num /= 2;
					}
				}
				break;
			}
			float num3 = 1f;
			int num4 = 0;
			if (c[owner] != null)
			{
				if (owner >= 20)
				{
					c[owner].perk[0] = (c[owner].perk[1] = (c[owner].perk[2] = -1));
				}
				int num5 = c[owner].perk[1];
				if (num5 == 4)
				{
					int type = p.type;
					if (type == 19 && num > 11)
					{
						num++;
					}
				}
				switch (c[owner].perk[0])
				{
				case 4:
					switch (p.type)
					{
					case 19:
					case 21:
					case 27:
						if (Rand.CointToss(0.5f))
						{
							num *= 2;
						}
						break;
					}
					break;
				case 2:
				{
					int type2 = p.type;
					if (type2 == 20)
					{
						num *= 8;
						num4 = 1;
					}
					break;
				}
				case 5:
					switch (p.type)
					{
					case 34:
						num++;
						break;
					case 33:
						num++;
						break;
					case 15:
						num++;
						break;
					}
					num3 *= 2f;
					break;
				}
			}
			if (GameState.gameType == 4 && c[owner].team == 0)
			{
				int type3 = p.type;
				if (type3 != 44 && type3 != 65)
				{
					num *= 2;
				}
			}
			if (c[i].suit == 3)
			{
				if (p.type == 28)
				{
					num /= 10;
				}
				else if (num > 1)
				{
					num--;
				}
			}
			else if (c[i].suit == 9 && p.type == 23)
			{
				num = 0;
			}
			if (owner >= 0 && owner < c.Length && c[owner] != null)
			{
				if (c[owner].suit == 6)
				{
					num *= 2;
				}
				if (c[owner].suit == 10)
				{
					c[owner].hp += num / 2;
					if (c[owner].hp > c[owner].GetMaxHP())
					{
						c[owner].hp = c[owner].GetMaxHP();
					}
				}
				if (c[owner].perk[0] == 8)
				{
					int num6 = num / 4;
					if (num6 < 1)
					{
						num6 = 1;
					}
					if (num6 > 10)
					{
						num6 = 10;
					}
					c[owner].hp += num6;
					if (c[owner].hp > c[owner].GetMaxHP())
					{
						c[owner].hp = c[owner].GetMaxHP();
					}
				}
			}
			float timeSinceHit = c[i].timeSinceHit;
			if (c[owner] != null)
			{
				float num7 = 1f;
				float num8 = 6f;
				float num9 = 5f;
				num7 = (float)(c[i].score + 200) / (float)(c[owner].score + 200);
				if (num7 > num8)
				{
					num7 = num8;
				}
				if (num7 < 1f / num8)
				{
					num7 = 1f / num8;
				}
				num7--;
				num7 /= num9;
				num7++;
				num = (int)((float)num * num7);
			}
			c[i].hp -= num;
			c[i].timeSinceHit = 0f;
			c[i].lastHitBy = owner;
			if (owner > -1 && c[owner] != null && c[owner].perk[1] == 9)
			{
				SapAmmo(c[i], num);
			}
			if (p.type == 27)
			{
				DoWound(p.type, p.loc, p.traj, c[i], owner);
			}
			else if (p.type == 23)
			{
				DoWound(p.type, p.loc, p.traj, c[i], owner);
			}
			else if (p.type == 30)
			{
				c[i].freeze = 3f * num3;
			}
			else if (p.type == 33)
			{
				c[i].freeze += 0.5f * num3;
				if (c[i].freeze > 10f)
				{
					c[i].freeze = 10f;
				}
			}
			else if (p.type == 55)
			{
				c[i].rainbowed = 7f;
			}
			else if (p.type == 54)
			{
				c[i].poison += 5f * num3;
				c[i].poisonOwner = owner;
				if (c[i].poison > 15f)
				{
					c[i].poison = 15f;
				}
			}
			else if (p.type == 37 || p.type == 35)
			{
				c[i].shrink = 3f;
			}
			else if (p.type == 5 || p.type == 4)
			{
				if (c[i].suit != 4)
				{
					c[i].poison = Rand.GetRandomFloat(9f, 10f) * num3;
					c[i].poisonOwner = owner;
				}
				else
				{
					c[i].timeSinceHit = timeSinceHit;
				}
			}
			else if (p.type == 34)
			{
				c[i].fire = 1.25f * num3;
				c[i].fireOwner = owner;
			}
			else if (p.type == 31)
			{
				c[i].fire = 2f * num3;
				c[i].fireOwner = owner;
			}
			else if (p.type == 15)
			{
				c[i].fire = 2.5f * num3;
				c[i].fireOwner = owner;
			}
			else if (p.type == 19)
			{
				DoWound(p.type, p.loc, p.traj, c[i], owner);
			}
			else if (p.type == 21)
			{
				DoWound(p.type, p.loc, p.traj, c[i], owner);
			}
			else if (p.type == 20 || p.type == 44 || p.type == 65)
			{
				DoWound(p.type, p.loc, p.traj, c[i], owner);
				if (p.type == 65)
				{
					float num10 = 2.5f * num3;
					if (c[i].fire < 10f)
					{
						c[i].fire += num10;
					}
					c[i].fireOwner = owner;
				}
				if (p.type == 44 || p.type == 65)
				{
					Vector2 traj = p.traj;
					c[i].StartKill(traj * 0.4f);
				}
				else
				{
					Vector2 traj2 = p.traj;
					c[i].StartKill(traj2 * ((num4 == 1) ? 0.6f : 0.5f));
				}
			}
			if (p.type == 28 && p.size >= 500f)
			{
				c[i].fire = 20f;
				c[i].fireOwner = p.netOwner;
				c[i].StartKill(Rand.GetRandomVec2(-100f, 100f, -100f, -100f));
			}
			c[i].killType = 0;
			switch (p.type)
			{
			case 10:
			case 25:
			case 28:
				c[i].killType = 5;
				break;
			case 19:
			case 21:
				c[i].killType = 1;
				break;
			case 27:
				c[i].killType = 10;
				break;
			case 4:
			case 5:
			case 14:
			case 54:
				c[i].killType = 4;
				break;
			case 47:
				c[i].killType = 15;
				break;
			case 52:
				c[i].killType = 12;
				break;
			case 16:
			case 23:
			case 45:
				c[i].killType = 9;
				break;
			case 67:
				c[i].killType = 13;
				break;
			case 29:
			case 33:
				c[i].killType = 3;
				break;
			case 20:
				c[i].killType = 8;
				break;
			case 44:
			case 65:
				c[i].killType = 7;
				break;
			case 15:
			case 31:
			case 34:
				c[i].killType = 2;
				break;
			}
			if (c[i].freeze > 0f)
			{
				c[i].killType = 3;
			}
			if (c[i].hp < 0)
			{
				if ((p.type == 44 || p.type == 65) && c[i].hp > -31)
				{
					c[i].hp = -31;
				}
				Vector2 val3 = p.traj;
				if (p.type == 28)
				{
					val3 = c[i].loc - p.loc;
					((Vector2)(ref val3)).Normalize();
					val3 *= 9000f;
				}
				if (hp >= 0)
				{
					c[i].killedBy = owner;
				}
				c[i].StartKill(val3 / 10f);
			}
			result = true;
			if (owner <= -1 || !Game1.netSession.GetNetworkOwner(i))
			{
				continue;
			}
			if (owner == Game1.netSession.GetPlayerOne())
			{
				if (i != owner)
				{
					Sound.PlayConfirm();
				}
			}
			else
			{
				c[i].SetHitBy(owner);
			}
		}
		return result;
	}

	private static void SapAmmo(Character victim, int damage)
	{
		if (victim.curWeap < 0 || victim.curWeap >= 4)
		{
			return;
		}
		Weapon weapon = WeaponCatalog.weapons[victim.weapon[victim.curWeap]];
		int maxClip = weapon.maxClip;
		float num = (float)damage / 100f;
		num *= 4f;
		int num2 = (int)((float)maxClip * num);
		if (num2 < 1)
		{
			num2 = 1;
		}
		victim.ammo[weapon.ammoType] -= num2;
		if (victim.ammo[weapon.ammoType] < 0)
		{
			victim.magazine[victim.curWeap] += victim.ammo[weapon.ammoType];
			if (victim.magazine[victim.curWeap] < 0)
			{
				victim.magazine[victim.curWeap] = 0;
			}
			victim.ammo[weapon.ammoType] = 0;
		}
	}

	public static void DoWound(int type, Vector2 loc, Vector2 traj, Character c, int owner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		c.hitVec = loc;
		c.hitTraj = traj;
		c.hitType = type;
		if (Game1.netSession.GetNetworkOwner(c.ID))
		{
			DoWound(type, loc, traj, c);
		}
	}

	public static void DoWound(int type, Vector2 loc, Vector2 traj, Character c)
	{
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Rand.CointToss(0.1f);
		switch (type)
		{
		case 27:
		{
			Game1.pMan.AddParticle(6, loc, traj * Rand.GetRandomFloat(-0.05f, 0.01f), Rand.GetRandomFloat(0.2f, 0.9f), 0, 0);
			if (Rand.CointToss(0.3f))
			{
				Game1.pMan.AddParticle(7, loc, Rand.GetRandomVec2(-1000f, 1000f, -1000f, 1000f), Rand.GetRandomFloat(0.1f, 0.4f), 0, 0);
			}
			Vector2 val3 = loc - Scroll.scroll;
			if (((Vector2)(ref val3)).LengthSquared() < 90000f)
			{
				Sound.PlayCue("bhit");
			}
			if (flag)
			{
				for (int l = 0; l < 2; l++)
				{
					Game1.pMan.AddParticle(58, loc, traj * Rand.GetRandomFloat(-0.5f, -0.1f) + Rand.GetRandomVec2(-650f, 650f, -650f, 650f), 0f, 0, 0);
				}
			}
			break;
		}
		case 19:
		{
			Game1.pMan.AddParticle(6, loc, traj * Rand.GetRandomFloat(-0.01f, 0f), Rand.GetRandomFloat(0.8f, 1.2f), 0, 0);
			if (Rand.CointToss(0.3f))
			{
				Game1.pMan.AddParticle(7, loc, traj * Rand.GetRandomFloat(0.2f, 0.5f), Rand.GetRandomFloat(0.2f, 1.2f), 0, 0);
			}
			else
			{
				Game1.pMan.AddParticle(6, loc, traj * Rand.GetRandomFloat(-0.03f, 0f), Rand.GetRandomFloat(0.4f, 0.9f), 0, 0);
			}
			if (Rand.CointToss(0.3f))
			{
				Game1.pMan.AddParticle(7, loc, Rand.GetRandomVec2(-1000f, 1000f, -1000f, 1000f), Rand.GetRandomFloat(0.1f, 0.4f), 0, 0);
			}
			Vector2 val4 = loc - Scroll.scroll;
			if (((Vector2)(ref val4)).LengthSquared() < 90000f)
			{
				Sound.PlayCue("bhit");
			}
			if (flag)
			{
				for (int m = 0; m < 6; m++)
				{
					Game1.pMan.AddParticle(58, loc, traj * Rand.GetRandomFloat(-0.5f, -0.1f) + Rand.GetRandomVec2(-650f, 650f, -650f, 650f), 0f, 0, 0);
				}
			}
			break;
		}
		case 21:
		{
			Game1.pMan.AddParticle(6, loc, traj * Rand.GetRandomFloat(-0.01f, 0f), Rand.GetRandomFloat(0.4f, 0.8f), 0, 0);
			if (Rand.CointToss(0.2f))
			{
				Game1.pMan.AddParticle(7, loc, traj * Rand.GetRandomFloat(0.2f, 0.5f), Rand.GetRandomFloat(0.2f, 0.8f), 0, 0);
			}
			else
			{
				Game1.pMan.AddParticle(6, loc, traj * Rand.GetRandomFloat(-0.03f, 0f), Rand.GetRandomFloat(0.3f, 0.8f), 0, 0);
			}
			if (Rand.CointToss(0.3f))
			{
				Game1.pMan.AddParticle(7, loc, Rand.GetRandomVec2(-1000f, 1000f, -1000f, 1000f), Rand.GetRandomFloat(0.1f, 0.4f), 0, 0);
			}
			Vector2 val5 = loc - Scroll.scroll;
			if (((Vector2)(ref val5)).LengthSquared() < 90000f)
			{
				Sound.PlayCue("bhit");
			}
			if (flag)
			{
				for (int num = 0; num < 3; num++)
				{
					Game1.pMan.AddParticle(58, loc, traj * Rand.GetRandomFloat(-0.5f, -0.1f) + Rand.GetRandomVec2(-650f, 650f, -650f, 650f), 0f, 0, 0);
				}
			}
			break;
		}
		case 23:
		{
			for (int n = 0; n < 3; n++)
			{
				Game1.pMan.AddParticle(58, loc, traj * Rand.GetRandomFloat(-0.5f, -0.1f) + Rand.GetRandomVec2(-650f, 650f, -650f, 650f), 0f, 0, 0);
			}
			break;
		}
		case 20:
		case 44:
		case 65:
			if (type == 44 || type == 65)
			{
				for (int i = 0; i < 4; i++)
				{
					Game1.pMan.AddParticle(6, c.loc - new Vector2(0f, 40f), Rand.GetRandomVec2(-100f, 100f, -100f, 100f), Rand.GetRandomFloat(0.4f, 0.8f), 0, 0);
				}
				for (int j = 0; j < 5; j++)
				{
					Game1.pMan.AddParticle(7, c.loc - new Vector2(0f, 40f), traj * Rand.GetRandomFloat(0.2f, 0.5f) + Rand.GetRandomVec2(-100f, 100f, -100f, 100f), Rand.GetRandomFloat(0.2f, 0.8f), 0, 0);
				}
				Vector2 val = loc - Scroll.scroll;
				if (((Vector2)(ref val)).LengthSquared() < 90000f)
				{
					Quake.SetQuake(0.25f);
					Sound.PlayCue("hit2");
				}
				for (int k = 0; k < 24; k++)
				{
					Game1.pMan.AddParticle(58, loc, traj * Rand.GetRandomFloat(-0.5f, 1f) + Rand.GetRandomVec2(-650f, 650f, -650f, 650f), 0f, 0, 0);
				}
			}
			else
			{
				Vector2 val2 = loc - Scroll.scroll;
				if (((Vector2)(ref val2)).LengthSquared() < 90000f)
				{
					Quake.SetQuake(0.25f);
					Sound.PlayCue("hit");
				}
			}
			break;
		}
	}

	public static void DoKill(Character c)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Vector2 traj = c.traj;
		if (traj.Y > -50f)
		{
			traj.Y = -50f;
		}
		c.StartKill(traj);
	}
}
