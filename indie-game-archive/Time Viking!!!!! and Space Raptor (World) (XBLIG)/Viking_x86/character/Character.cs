using System;
using IMAK3Z0MB1EGAEM.audio;
using IMAK3Z0MB1EGAEM.menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SheetEdit.TextureSheet;
using Viking_x86.director;
using Viking_x86.vikinggame;
using Yuki_Win;
using xCharEdit.Character;

namespace Viking_x86.character;

public class Character
{
	public const int STATE_AIR = 0;

	public const int STATE_GROUND = 1;

	public const int BODY_ALL = 0;

	public const int BODY_TOP = 1;

	public const int FACE_LEFT = 0;

	public const int FACE_RIGHT = 1;

	public const int TEAM_GOOD = 0;

	public const int TEAM_EVIL = 1;

	public const int SHOT_NORMAL = 0;

	public const int SHOT_RAPID = 1;

	public const int SHOT_BOMB = 2;

	public const int SHOT_SPREAD = 3;

	public const int SHIELD_RESPAWN = 0;

	public const int SHIELD_PICKUP = 1;

	public const int SHIELD_MOON = 2;

	public Vector2 loc;

	public Vector2 traj;

	public int state;

	public bool exists;

	public int face;

	public BodySec[] bodySec;

	public float delta;

	public float gamma;

	private float shootFrame;

	private int shootFace;

	public bool split;

	public int defID;

	public int ID;

	public int team;

	public CharKeys charKeys;

	public int hp;

	private float angle;

	private float walkFrame;

	private int shotType;

	private int ammo;

	private float shieldFrame;

	public int lives;

	public long score;

	public int nameIn;

	public char[] name = new char[3];

	public float respawnFrame;

	public void SetShield(int type)
	{
		switch (type)
		{
		case 0:
			shieldFrame = 5f;
			break;
		case 1:
			shieldFrame = 20f;
			break;
		case 2:
			shieldFrame = (float)TimeMgr.CurTMgr().trackLeft;
			break;
		}
	}

	public void SetShot(int type)
	{
		bool flag = type == shotType;
		shotType = type;
		int num = 0;
		switch (type)
		{
		case 1:
			num = 300;
			break;
		case 2:
			num = 40;
			break;
		case 3:
			num = 80;
			break;
		}
		if (flag)
		{
			ammo += num;
		}
		else
		{
			ammo = num;
		}
	}

	public Character(int ID)
	{
		this.ID = ID;
		bodySec = new BodySec[2];
		for (int i = 0; i < bodySec.Length; i++)
		{
			bodySec[i] = new BodySec();
		}
		state = 1;
		SetAnimation("idle", 0, overRide: true);
		charKeys = new CharKeys();
	}

	internal void Update()
	{
		if (ID < 2)
		{
			charKeys.Update(VikingGame.mainPlayerIdx[ID]);
			if (nameIn > 0)
			{
				if (nameIn < 4)
				{
					if (charKeys.keyUp)
					{
						name[nameIn - 1] += '\u0001';
						if (name[nameIn - 1] > 'Z')
						{
							name[nameIn - 1] = 'A';
						}
						Console.WriteLine(((byte)name[nameIn - 1]).ToString());
					}
					if (charKeys.keyDown)
					{
						name[nameIn - 1] -= '\u0001';
						if (name[nameIn - 1] < 'A')
						{
							name[nameIn - 1] = 'Z';
						}
						Console.WriteLine(((byte)name[nameIn - 1]).ToString());
					}
					if (nameIn == 1)
					{
						name[1] = '-';
						name[2] = '-';
					}
					else if (nameIn == 2)
					{
						name[2] = '-';
					}
					if (charKeys.keyAccept)
					{
						nameIn++;
						if (nameIn == 4)
						{
							string text = name[0].ToString() + name[1] + name[2];
							HighScores.AddScore(text, score);
						}
						else
						{
							name[nameIn - 1] = 'A';
						}
					}
					if (charKeys.keyCancel && nameIn > 1)
					{
						nameIn--;
					}
				}
				else
				{
					respawnFrame -= Game1.frameTime;
					if (respawnFrame <= 0f)
					{
						exists = false;
					}
				}
				return;
			}
			if (shieldFrame > 0f)
			{
				shieldFrame -= Game1.frameTime;
				for (int i = 0; i < Game1.vgame.charMgr.character.Length; i++)
				{
					Character character = Game1.vgame.charMgr.character[i];
					if (character.exists && character.team != team)
					{
						float num = (character.loc - loc).LengthSquared();
						if (num < 3600f)
						{
							character.hp = 0;
							character.Hit(ID);
						}
					}
				}
			}
		}
		switch (defID)
		{
		case 0:
		case 1:
		case 2:
		{
			float num7 = 200f;
			if (ID >= 2)
			{
				charKeys.Clear();
				if (state == 1)
				{
					int num8 = 0;
					if (Game1.vgame.charMgr.character[1].exists && (!Game1.vgame.charMgr.character[0].exists || Game1.vgame.charMgr.character[0].respawnFrame > 0f))
					{
						num8 = 1;
					}
					if (Game1.vgame.charMgr.character[0].exists && Game1.vgame.charMgr.character[0].respawnFrame <= 0f && Game1.vgame.charMgr.character[1].exists && Game1.vgame.charMgr.character[1].respawnFrame <= 0f)
					{
						float num9 = loc.X - Game1.vgame.charMgr.character[0].loc.X;
						float num10 = loc.X - Game1.vgame.charMgr.character[1].loc.X;
						if (num9 < 0f)
						{
							num9 = 0f - num9;
						}
						if (num10 < 0f)
						{
							num10 = 0f - num10;
						}
						if (num10 < num9)
						{
							num8 = 1;
						}
					}
					if (loc.X >= Game1.vgame.charMgr.character[num8].loc.X + 10f)
					{
						charKeys.runVec.X = -1f;
					}
					else if (loc.X <= Game1.vgame.charMgr.character[num8].loc.X - 10f)
					{
						charKeys.runVec.X = 1f;
					}
					else if (Game1.vgame.charMgr.character[num8].exists && Game1.vgame.charMgr.character[num8].respawnFrame <= 0f)
					{
						if (bodySec[0].animName != "attack")
						{
							Sound.Play("sword");
						}
						SetAnimation("attack", 0, overRide: false);
						Game1.vgame.charMgr.character[num8].Hit(ID);
					}
				}
			}
			delta += Game1.frameTime;
			float num11 = 10f;
			bool flag = loc.X >= Game1.vgame.world.towerX + num11 && loc.X <= Game1.vgame.world.towerX + 320f - num11;
			switch (state)
			{
			case 1:
			{
				loc.X += traj.X * Game1.frameTime;
				if (traj.X > 0f)
				{
					traj.X -= Game1.frameTime * 800f;
					if (traj.X < 0f)
					{
						traj.X = 0f;
					}
				}
				if (traj.X < 0f)
				{
					traj.X += Game1.frameTime * 800f;
					if (traj.X > 0f)
					{
						traj.X = 0f;
					}
				}
				switch (bodySec[0].animName)
				{
				case "idle":
				case "run":
				{
					float num12 = 0f;
					float num13 = 5f * Game1.frameTime;
					if (charKeys.runVec.X < -0.3f)
					{
						SetAnimation("run", 0, overRide: false);
						face = 0;
						num12 = charKeys.runVec.X * num7;
						walkFrame += num13;
					}
					else if (charKeys.runVec.X > 0.3f)
					{
						SetAnimation("run", 0, overRide: false);
						face = 1;
						num12 = charKeys.runVec.X * num7;
						walkFrame += num13;
					}
					else
					{
						SetAnimation("idle", 0, overRide: false);
						walkFrame = 0.9f;
					}
					if (walkFrame > 1f)
					{
						walkFrame--;
						if (defID == 0)
						{
							Sound.Play("boot");
						}
						if (defID == 1)
						{
							Sound.Play("foot");
						}
					}
					if (traj.X > num12)
					{
						traj.X -= Game1.frameTime * 2000f;
						if (traj.X < num12)
						{
							traj.X = num12;
						}
					}
					if (traj.X < num12)
					{
						traj.X += Game1.frameTime * 2000f;
						if (traj.X > num12)
						{
							traj.X = num12;
						}
					}
					if (shootFrame > 0f)
					{
						shootFrame -= Game1.frameTime;
					}
					if (charKeys.shootVec.LengthSquared() > 0.09f && shootFrame <= 0f)
					{
						switch (shotType)
						{
						case 0:
							shootFrame = 0.15f;
							break;
						case 1:
							shootFrame = 0.05f;
							break;
						case 3:
							shootFrame = 0.2f;
							break;
						case 2:
							shootFrame = 0.4f;
							break;
						}
						Vector2 shootVec = charKeys.shootVec;
						shootVec.Normalize();
						float num14 = Trig.GetAngle(default(Vector2), shootVec) + 3.14f;
						num14 -= VScroll.angle;
						if (Math.Cos(num14) > 0.0)
						{
							shootFace = 1;
						}
						else
						{
							shootFace = 0;
						}
						double num15 = 0.0 - Math.Sin(num14);
						Vector2 vector3 = default(Vector2);
						if (num15 > 0.8600000143051147)
						{
							SetAnimation("uzap", 1, overRide: true);
							vector3 = new Vector2(0f, -50f);
							if (defID == 1)
							{
								vector3 = new Vector2(22f, -50f);
							}
						}
						else if (num15 > 0.5)
						{
							SetAnimation("ulzap", 1, overRide: true);
							vector3 = new Vector2(10f, -46f);
							if (defID == 1)
							{
								vector3 = new Vector2(26f, -45f);
							}
						}
						else
						{
							SetAnimation("zap", 1, overRide: true);
							vector3 = new Vector2(20f, -40f);
							if (defID == 1)
							{
								vector3 = new Vector2(29f, -40f);
							}
						}
						Vector2 vector4 = loc + new Vector2(vector3.X * ((shootFace == 1) ? 1f : (-1f)), vector3.Y);
						switch (shotType)
						{
						case 0:
						{
							if (defID == 0)
							{
								Sound.Play("znormal");
							}
							if (defID == 1)
							{
								Sound.Play("spit");
							}
							for (int m = -1; m < 2; m++)
							{
								Game1.vgame.pMgr.AddParticle((defID == 0) ? 4 : 20, vector4, new Vector2((float)Math.Cos((float)m * 0.1f + num14), (float)Math.Sin((float)m * 0.1f + num14)) * 800f, 0f, 0, ID);
							}
							break;
						}
						case 3:
						{
							if (defID == 0)
							{
								Sound.Play("zspread");
							}
							if (defID == 1)
							{
								Sound.Play("spitspread");
							}
							for (int n = -4; n < 5; n++)
							{
								Game1.vgame.pMgr.AddParticle((defID == 0) ? 4 : 20, vector4, new Vector2((float)Math.Cos((float)n * 0.1f + num14), (float)Math.Sin((float)n * 0.1f + num14)) * 800f, 0f, 0, ID);
							}
							ammo--;
							if (ammo <= 0)
							{
								shotType = 0;
							}
							break;
						}
						case 1:
						{
							if (defID == 0)
							{
								Sound.Play("zrapid");
							}
							if (defID == 1)
							{
								Sound.Play("spitrapid");
							}
							for (int l = 0; l < 2; l++)
							{
								float num16 = num14 + Rand.GetRandomFloat(-0.1f, 0.1f);
								Game1.vgame.pMgr.AddParticle((defID == 0) ? 4 : 20, vector4, new Vector2((float)Math.Cos(num16), (float)Math.Sin(num16)) * 800f, 0f, 0, ID);
							}
							ammo--;
							if (ammo <= 0)
							{
								shotType = 0;
							}
							break;
						}
						case 2:
							if (defID == 0)
							{
								Sound.Play("zrocket");
							}
							if (defID == 1)
							{
								Sound.Play("spitbomb");
							}
							Game1.vgame.pMgr.AddParticle((defID == 0) ? 17 : 23, vector4, new Vector2((float)Math.Cos(num14), (float)Math.Sin(num14)) * 800f, 0f, 0, ID);
							ammo--;
							if (ammo <= 0)
							{
								shotType = 0;
							}
							break;
						}
						if (defID == 0)
						{
							Game1.vgame.pMgr.AddParticle(21, vector4, default(Vector2), 0.4f, 0, ID);
						}
						else
						{
							for (int num17 = -1; num17 < 2; num17++)
							{
								float num18 = Rand.GetRandomFloat(-1f, 1f) * 0.3f;
								Game1.vgame.pMgr.AddParticle(22, vector4, new Vector2((float)Math.Cos(num18 + num14), (float)Math.Sin(num18 + num14)) * Rand.GetRandomFloat(100f, 500f), 0f, 0, ID);
							}
						}
					}
					if (shootFrame > 0f)
					{
						face = shootFace;
					}
					break;
				}
				}
				float minY = Game1.vgame.world.GetMinY(loc.X);
				if (loc.Y < minY)
				{
					loc.Y += Game1.frameTime * 100f;
					if (loc.Y > minY)
					{
						loc.Y = minY;
					}
				}
				if (loc.Y > minY)
				{
					loc.Y -= Game1.frameTime * 100f;
					if (loc.Y < minY)
					{
						loc.Y = minY;
					}
				}
				break;
			}
			case 0:
				loc += traj * Game1.frameTime;
				traj.Y += 6f;
				if (Game1.vgame.world.TestCollision(loc) || loc.Y > Game1.vgame.world.height + 800f)
				{
					loc.Y = Game1.vgame.world.GetMinY(loc.X);
					state = 1;
					SetAnimation("land", 0, overRide: true);
				}
				break;
			}
			if (flag)
			{
				if (loc.X < Game1.vgame.world.towerX + num11)
				{
					loc.X = Game1.vgame.world.towerX + num11;
				}
				if (loc.X > Game1.vgame.world.towerX + 320f - num11)
				{
					loc.X = Game1.vgame.world.towerX + 320f - num11;
				}
			}
			if (loc.X < 0f)
			{
				loc.X = 0f;
			}
			if (defID != 2 || state != 0)
			{
				bodySec[0].Update(0, this);
				if (split)
				{
					bodySec[1].Update(1, this);
				}
			}
			if (team == 1 && loc.Y > Game1.vgame.charMgr.character[0].loc.Y + 500f)
			{
				exists = false;
			}
			break;
		}
		case 3:
			loc += traj * Game1.frameTime;
			if (traj.X < 0f && loc.X < Game1.vgame.world.towerX - 300f)
			{
				exists = false;
			}
			if (traj.X > 0f && loc.X > Game1.vgame.world.towerX + 320f + 300f)
			{
				exists = false;
			}
			delta += Game1.frameTime;
			if (delta > 1f)
			{
				delta--;
				if (Rand.CoinToss(0.5f))
				{
					Game1.vgame.pMgr.AddParticle(9, loc, new Vector2(0f, 100f), 0f, 0, ID);
				}
			}
			break;
		case 7:
		case 8:
		{
			loc += traj * Game1.frameTime;
			if (traj.X < 0f && loc.X < Game1.vgame.world.towerX - 300f)
			{
				exists = false;
			}
			if (traj.X > 0f && loc.X > Game1.vgame.world.towerX + 320f + 300f)
			{
				exists = false;
			}
			delta += Game1.frameTime;
			if (!(gamma > 0f))
			{
				break;
			}
			gamma -= Game1.frameTime;
			int num3 = 0;
			if (Game1.vgame.charMgr.character[1].exists && !Game1.vgame.charMgr.character[0].exists)
			{
				num3 = 1;
			}
			if (Game1.vgame.charMgr.character[0].exists && Game1.vgame.charMgr.character[1].exists)
			{
				float num4 = loc.X - Game1.vgame.charMgr.character[0].loc.X;
				float num5 = loc.X - Game1.vgame.charMgr.character[1].loc.X;
				if (num4 < 0f)
				{
					num4 = 0f - num4;
				}
				if (num5 < 0f)
				{
					num5 = 0f - num5;
				}
				if (num5 < num4)
				{
					num3 = 1;
				}
			}
			if (loc.X > Game1.vgame.charMgr.character[num3].loc.X - 16f && loc.X < Game1.vgame.charMgr.character[num3].loc.X + 16f)
			{
				gamma--;
			}
			if (loc.X > Game1.vgame.world.towerX + 30f && loc.X < Game1.vgame.world.towerX + 320f - 30f && gamma <= 0f)
			{
				traj.Y = ((traj.X < 0f) ? (0f - traj.X) : traj.X);
				traj.X = 0f;
				gamma--;
			}
			if (Game1.vgame.world.TestCollision(loc + new Vector2(0f, -32f)))
			{
				Kill(ID);
				break;
			}
			for (int k = 0; k < 2; k++)
			{
				if (Game1.vgame.charMgr.character[k].exists && Game1.vgame.charMgr.character[k].CheckHit(loc + new Vector2(0f, -40f)))
				{
					Game1.vgame.charMgr.character[k].Hit(ID);
					Kill(ID);
				}
			}
			break;
		}
		case 4:
			traj.Y += Game1.frameTime * 7f;
			loc += traj * Game1.frameTime;
			if (Game1.vgame.world.TestCollision(loc) || loc.Y > Game1.vgame.world.height + 800f)
			{
				Kill(ID);
			}
			break;
		case 5:
		{
			if (hp <= 0)
			{
				traj.Y += Game1.frameTime * 300f;
				loc += traj * Game1.frameTime;
				delta += Game1.frameTime;
				if (loc.Y > VScroll.scroll.Y + 500f)
				{
					exists = false;
				}
				break;
			}
			Vector2 vector = new Vector2
			{
				Y = VScroll.scroll.Y
			};
			float num6 = Game1.vgame.world.towerX + 160f;
			if (loc.X > num6)
			{
				vector.X = num6 + 250f;
			}
			else
			{
				vector.X = num6 - 250f;
			}
			delta += Game1.frameTime;
			vector += new Vector2((float)Math.Cos(delta), (float)Math.Sin(delta)) * new Vector2(5f, 90f);
			loc += (vector - loc) * Game1.frameTime * 0.5f;
			if (delta > 6.28f && Math.Sin(delta) > 0.0 && Math.Cos(delta) > 0.0)
			{
				Vector2 vector2 = loc + new Vector2(20f * ((face == 1) ? 1f : (-1f)), -69f);
				Game1.vgame.pMgr.AddParticle(12, vector2, new Vector2(500f * ((face == 1) ? 1f : (-1f)), 300f) * 0.1f, 0.2f, 0, ID);
				Game1.vgame.pMgr.AddParticle(10, vector2, new Vector2(500f * ((face == 1) ? 1f : (-1f)), 300f), 0f, 0, ID);
				VikingQuake.SetQuake(0.25f);
			}
			break;
		}
		case 6:
		{
			delta += Game1.frameTime;
			float num2 = ((face == 0) ? 1f : (-1f));
			if (delta < 4f)
			{
				angle += Game1.frameTime * 0.35f * num2;
			}
			else if (delta < 5f)
			{
				angle += Game1.frameTime * 5f * num2;
			}
			else
			{
				angle += Game1.frameTime * num2;
			}
			traj = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 140f;
			loc += traj * Game1.frameTime;
			if (Game1.vgame.world.TestCollision(loc))
			{
				Kill(ID);
			}
			else
			{
				for (int j = 0; j < 2; j++)
				{
					if (Game1.vgame.charMgr.character[j].exists && Game1.vgame.charMgr.character[j].CheckHit(loc))
					{
						Game1.vgame.charMgr.character[j].Hit(ID);
						Kill(ID);
					}
				}
			}
			if (delta > 8f)
			{
				exists = false;
			}
			break;
		}
		}
	}

	public void SetAnimation(string newAnim, int sec, bool overRide)
	{
		int animFromName = GetAnimFromName(newAnim);
		if (sec == 0)
		{
			bodySec[0].SetAnim(animFromName, newAnim, overRide);
			return;
		}
		bodySec[1].SetAnim(animFromName, newAnim, overRide);
		split = true;
	}

	private int GetAnimFromName(string newAnim)
	{
		CharDef charDef = CharDefMgr.charDef[defID];
		for (int i = 0; i < charDef.GetAnimationArray().Length; i++)
		{
			Animation animation = charDef.GetAnimation(i);
			if (animation != null && animation.name == newAnim)
			{
				return i;
			}
		}
		return 0;
	}

	internal void Draw()
	{
		float foreBright = Game1.vgame.world.GetForeBright();
		if (respawnFrame > 0f)
		{
			return;
		}
		if (shieldFrame > 0f)
		{
			SpriteTools.End();
			SpriteTools.BeginAdditive();
			for (int i = 0; i < 4; i++)
			{
				float num;
				for (num = (float)i * 0.25f + shieldFrame; num > 1f; num--)
				{
				}
				num = 1f - num;
				float num2 = 1f;
				if (num > 0.8f)
				{
					num2 = (1f - num) * 5f;
				}
				if (num < 0.5f)
				{
					num2 = num * 2f;
				}
				Color color = new Color(1f - num, 1f - num, 1f, num2);
				if (shieldFrame < 3f)
				{
					color = new Color(1f, 1f - num, 1f - num, ((int)(shieldFrame * 30f) % 2 == 0) ? num2 : 0f);
				}
				SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(loc + new Vector2(0f, -25f), 1f), new Rectangle(0, 832, 128, 128), color, 0f, new Vector2(64f, 64f), num * VScroll.zoom, SpriteEffects.None, 1f);
			}
			SpriteTools.End();
			SpriteTools.BeginAlpha();
		}
		switch (defID)
		{
		case 0:
		case 1:
		case 2:
		{
			Vector2 screenLoc = VScroll.GetScreenLoc(loc, 1f);
			float num6 = 0.15f * VScroll.zoom;
			if (defID == 2 && state == 0)
			{
				XTexture xTexture = VikingGame.textures[CharDefMgr.charDef[defID].texName];
				Rectangle spriteRect = xTexture.GetSpriteRect(16);
				Vector2 spriteOrigin = xTexture.GetSpriteOrigin(16);
				spriteOrigin.X -= spriteRect.X;
				spriteOrigin.Y -= spriteRect.Y;
				bool flag = face == 0;
				if (!flag)
				{
					spriteOrigin.X = (float)spriteRect.Width - spriteOrigin.X;
				}
				SpriteTools.sprite.Draw(xTexture.texture, screenLoc, spriteRect, new Color(foreBright, foreBright, foreBright, 1f), VScroll.angle, spriteOrigin, num6, (!flag) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
			}
			else if (split)
			{
				bodySec[0].Draw(screenLoc, num6, face, this, 0, 1);
				bodySec[1].Draw(screenLoc, num6, face, this, 1, 0);
				bodySec[0].Draw(screenLoc, num6, face, this, 0, 2);
			}
			else
			{
				bodySec[0].Draw(screenLoc, num6, face, this, -1, 0);
			}
			break;
		}
		case 3:
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(loc, 1f), new Rectangle(256, 64 + 128 * ((int)(delta * 4f) % 2), 192, 128), new Color(foreBright, foreBright, foreBright * 0.5f, 1f), VScroll.angle, new Vector2(96f, 168f), 0.4f * VScroll.zoom, SpriteEffects.None, 1f);
			break;
		case 4:
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(loc, 1f), new Rectangle(598, 65, 426, 190), new Color(foreBright, foreBright, foreBright, 1f), VScroll.angle - 1.57f, new Vector2(96f, 96f), 0.2f * VScroll.zoom, SpriteEffects.None, 1f);
			break;
		case 5:
		{
			Color color2 = new Color(1f, 1f, 1f, 1f);
			if (hp <= 0)
			{
				color2 = (((int)(delta * 30f) % 2 != 0) ? new Color(foreBright, foreBright, foreBright, 0.5f) : new Color(1f, 0f, 0f, 0.5f));
			}
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(loc, 1f), new Rectangle(0, 192, 192, 384), color2, VScroll.angle, new Vector2(96f, 192f), 0.6f * VScroll.zoom, (face != 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f);
			break;
		}
		case 6:
		{
			while (angle < 0f)
			{
				angle += 6.28f;
			}
			while (angle > 6.28f)
			{
				angle -= 6.28f;
			}
			float num3;
			for (num3 = angle; num3 > 1.57f; num3 -= 1.57f)
			{
			}
			num3 /= 1.57f;
			num3 *= 6f;
			int num4 = (int)(angle / 1.57f);
			float num5 = delta * 2f;
			if (num5 > 1f)
			{
				num5 = 1f;
			}
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(loc, 1f), new Rectangle((int)num3 * 128, 576, 128, 128), new Color(foreBright, foreBright, foreBright * (float)(ID % 2), num5), (float)num4 * 1.57f + VScroll.angle - 1.57f, new Vector2(64f, 64f), 0.6f * VScroll.zoom, SpriteEffects.None, 1f);
			break;
		}
		case 7:
			SpriteTools.sprite.Draw(Game1.vgame.grassTex, VScroll.GetScreenLoc(loc, 1f), new Rectangle(496, 272 + 64 * ((int)(delta * 16f) % 4), 64, 64), new Color(foreBright, foreBright, foreBright, 1f), VScroll.angle, new Vector2(32f, 64f), 0.7f * VScroll.zoom, SpriteEffects.None, 1f);
			break;
		case 8:
			SpriteTools.sprite.Draw(Game1.vgame.grassTex, VScroll.GetScreenLoc(loc, 1f), new Rectangle(496, 272 + 64 * ((int)(delta * 16f) % 4), 64, 64), new Color(foreBright, foreBright, foreBright, 1f), VScroll.angle, new Vector2(32f, 64f), 1.4f * VScroll.zoom, SpriteEffects.None, 1f);
			break;
		}
	}

	internal void Init(Vector2 loc, int defID, int face, int team)
	{
		this.loc = loc;
		this.defID = defID;
		this.face = face;
		this.team = team;
		exists = true;
		delta = 0f;
		hp = 1;
		respawnFrame = 0f;
		bool flag = Game1.vgame.charMgr.character[0].exists && Game1.vgame.charMgr.character[1].exists;
		switch (defID)
		{
		case 2:
		case 3:
			if (flag && Rand.CoinToss(0.5f))
			{
				hp = 2;
			}
			break;
		case 4:
			hp = 10;
			if (flag)
			{
				hp = 20;
			}
			break;
		case 5:
			hp = 20;
			if (flag)
			{
				hp = 40;
			}
			break;
		case 7:
			gamma = Rand.GetRandomFloat(2f, 3f);
			break;
		case 8:
			gamma = Rand.GetRandomFloat(2f, 3f);
			hp = 15;
			if (flag)
			{
				hp = 30;
			}
			break;
		case 6:
		{
			angle = 1.57f;
			float num = 0.5f;
			if (face == 0)
			{
				angle += num;
			}
			else
			{
				angle -= num;
			}
			break;
		}
		case 0:
		case 1:
			lives = 3;
			break;
		}
	}

	internal bool CheckHit(Vector2 v)
	{
		return CheckHit(v, 0f);
	}

	internal bool CheckHit(Vector2 v, float buf)
	{
		float num = 20f + buf;
		float num2 = 80f + buf;
		switch (defID)
		{
		case 0:
		case 1:
			if (Game1.vgame.charMgr.moon.active && Game1.vgame.charMgr.moon.hp < 0f)
			{
				return false;
			}
			if (shieldFrame > 0f)
			{
				return false;
			}
			num2 = 60f;
			if (v.X > loc.X - num && v.X < loc.X + num && v.Y < loc.Y && v.Y > loc.Y - num2)
			{
				return true;
			}
			break;
		case 6:
			num = 30f + buf;
			num2 = 30f + buf;
			if (v.X > loc.X - num && v.X < loc.X + num && v.Y < loc.Y + num2 && v.Y > loc.Y - num2)
			{
				return true;
			}
			break;
		case 5:
			if (hp <= 0)
			{
				return false;
			}
			num2 = 100f + buf;
			if (v.X > loc.X - num && v.X < loc.X + num && v.Y < loc.Y + num2 && v.Y > loc.Y - num2)
			{
				return true;
			}
			break;
		case 8:
			if (v.X > loc.X - num * 1.8f && v.X < loc.X + num * 1.8f && v.Y < loc.Y && v.Y > loc.Y - num2 * 1.8f)
			{
				return true;
			}
			break;
		default:
			if (v.X > loc.X - num && v.X < loc.X + num && v.Y < loc.Y && v.Y > loc.Y - num2)
			{
				return true;
			}
			break;
		}
		return false;
	}

	internal void Kill(int killer)
	{
		if (ID < 2)
		{
			if (bodySec[0].animName != "die")
			{
				hp = 0;
				SetAnimation("die", 0, overRide: true);
				split = false;
			}
			return;
		}
		exists = false;
		float num = 25f;
		int num2 = 100;
		switch (defID)
		{
		case 2:
			Game1.vgame.pMgr.MakeScrapBomb(loc + new Vector2(0f, -50f));
			Sound.Play("junk");
			break;
		case 7:
			Game1.vgame.pMgr.MakePixelBomb(loc + new Vector2(0f, -50f));
			num2 = 200;
			break;
		case 3:
			Sound.Play("mexplode");
			Game1.vgame.pMgr.MakePixelBomb(loc + new Vector2(0f, -50f));
			num2 = 400;
			break;
		case 6:
			Sound.Play("mexplode");
			Game1.vgame.pMgr.MakeGalagaBomb(loc + new Vector2(0f, 0f));
			num = 0f;
			break;
		case 4:
		case 8:
		{
			Sound.Play("explode");
			Game1.vgame.pMgr.MakeGiantBomb(loc + new Vector2(0f, -50f));
			num2 = 250;
			for (int i = 0; i < 2; i++)
			{
				if (Game1.vgame.charMgr.character[i].exists)
				{
					float num3 = (Game1.vgame.charMgr.character[i].loc - loc).LengthSquared();
					if (num3 < 6000f)
					{
						Game1.vgame.charMgr.character[i].Hit(ID);
					}
				}
			}
			break;
		}
		case 5:
			Sound.Play("nekodie");
			exists = true;
			num2 = 1000;
			break;
		}
		Game1.vgame.charMgr.character[killer].score += num2;
		Game1.vgame.pMgr.AddParticle(19, loc + new Vector2(0f, 0f - num), default(Vector2), 0f, num2, 0);
		if (Game1.vgame.world.lifeFrame <= 0f)
		{
			bool flag = false;
			for (int j = 0; j < 2; j++)
			{
				if (Game1.vgame.charMgr.character[j].exists && Game1.vgame.charMgr.character[j].lives < 3)
				{
					flag = true;
					break;
				}
			}
			if (flag && loc.X > Game1.vgame.world.towerX && loc.X < Game1.vgame.world.towerX + 320f)
			{
				Game1.vgame.pMgr.AddParticle(15, loc + new Vector2(0f, 0f - num), new Vector2(0f, -50f), 0f, 3, 0);
				Game1.vgame.world.lifeFrame = 10f;
			}
		}
		if (Game1.vgame.world.pickupFrame <= 0f && loc.X > Game1.vgame.world.towerX && loc.X < Game1.vgame.world.towerX + 320f)
		{
			int randomInt = Rand.GetRandomInt(0, 5);
			if (randomInt == 3)
			{
				randomInt = Rand.GetRandomInt(0, 5);
			}
			Game1.vgame.pMgr.AddParticle(15, loc + new Vector2(0f, 0f - num), new Vector2(0f, -50f), 0f, Rand.GetRandomInt(0, 5), 0);
			Game1.vgame.world.pickupFrame = Rand.GetRandomFloat(10f, 15f);
		}
	}

	internal void Hit(int hitter)
	{
		if (!(shieldFrame > 0f))
		{
			hp--;
			int num = defID;
			if (num == 4)
			{
				loc.Y -= 5f;
			}
			if (hp <= 0)
			{
				Kill(hitter);
			}
		}
	}

	internal int GetShot()
	{
		return shotType;
	}

	internal long GetAmmo()
	{
		return ammo;
	}

	internal void KillChar()
	{
		if (nameIn <= 0)
		{
			nameIn = 1;
			name[0] = 'A';
			name[1] = 'A';
			name[2] = 'A';
			respawnFrame = 3f;
		}
	}
}
