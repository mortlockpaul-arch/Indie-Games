using System;
using IMAK3Z0MB1EGAEM.audio;
using IMAK3Z0MB1EGAEM.director;
using IMAK3Z0MB1EGAEM.map;
using IMAK3Z0MB1EGAEM.menu;
using IMAK3Z0MB1EGAEM.particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Viking_x86;
using Viking_x86.director;
using Yuki_Win;

namespace IMAK3Z0MB1EGAEM.character;

public class Hero
{
	public enum Weapon
	{
		Rifle,
		MachineGun,
		Rockets,
		Flamethrower,
		Shotty,
		Beam,
		Neutron
	}

	public Vector2 loc;

	public Vector2 shoot;

	public Vector2 traj;

	public int idx;

	public float angle;

	public bool exists;

	public bool keyStart;

	public bool keyUp;

	public bool keyDown;

	public bool keyAccept;

	public bool keyCancel;

	private GamePadState pgs;

	private Legs legs;

	private float shootFrame;

	public long score;

	public float respawnFrame;

	public char[] name = new char[3];

	public int nameIn;

	public int lives;

	public Weapon weapon;

	public int specialAmmo;

	public float spawnFrame;

	public float speedFrame;

	public void Kill()
	{
		lives--;
		weapon = Weapon.Rifle;
		speedFrame = 0f;
		HitManager.MakeBloodSplode(loc, 10, Rand.GetRandomFloat(0.5f, 1f), 300f);
		ParticleMan.AddParticle(16, loc, default(Vector2), idx, 0f, 0);
		if (lives <= 0)
		{
			nameIn = 1;
			name[0] = 'A';
			name[1] = 'A';
			name[2] = 'A';
			respawnFrame = 3f;
		}
		else
		{
			respawnFrame = 3f;
		}
	}

	public void SetWeapon(Weapon weapon, int ammo)
	{
		if (weapon == this.weapon)
		{
			specialAmmo += ammo;
			return;
		}
		this.weapon = weapon;
		specialAmmo = ammo;
	}

	public Hero(int idx)
	{
		this.idx = idx;
		legs = new Legs();
		exists = false;
	}

	public void Init(Vector2 loc)
	{
		lives = 5;
		nameIn = 0;
		this.loc = loc;
		exists = true;
		weapon = Weapon.Rifle;
		score = 0L;
	}

	public void AddPoints(Vector2 mloc, long points)
	{
		ParticleMan.AddParticle(3, mloc, default(Vector2), 0, 0f, (int)points);
		score += points;
	}

	public void Update()
	{
		bool flag = GameState.state == GameState.State.EndlessZombiesPlaying;
		if (nameIn > 0)
		{
			UpdateKeys();
			if (nameIn < 4)
			{
				if (keyUp)
				{
					name[nameIn - 1] += '\u0001';
					if (name[nameIn - 1] > 'Z')
					{
						name[nameIn - 1] = 'A';
					}
					Console.WriteLine(((byte)name[nameIn - 1]).ToString());
				}
				if (keyDown)
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
				if (keyAccept)
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
				if (keyCancel && nameIn > 1)
				{
					nameIn--;
				}
			}
			else
			{
				respawnFrame -= FMan.frameTime;
				if (respawnFrame <= 0f)
				{
					exists = false;
				}
			}
			return;
		}
		if (respawnFrame > 0f)
		{
			respawnFrame -= FMan.frameTime;
			if (!(respawnFrame <= 0f))
			{
				return;
			}
			spawnFrame = 5f;
			loc = Rand.GetRandomVec2(-200f, 200f, -200f, 200f) + MapMan.mapSize / 2f;
			if (GameState.state == GameState.State.EndlessZombiesPlaying)
			{
				loc = Rand.GetRandomVec2(-200f, 200f, -200f, 200f) + (ZombieGame.GetEndlessRoomTL() + ZombieGame.GetEndlessRoomBR()) / 2f;
			}
		}
		if (spawnFrame > 0f)
		{
			spawnFrame -= FMan.frameTime;
		}
		if (!exists)
		{
			return;
		}
		bool flag2 = false;
		if (GameState.state == GameState.State.EndlessZombiesPlaying && CamMan.endlessTransFrame > 0f)
		{
			flag2 = true;
		}
		if (!flag2)
		{
			UpdateKeys();
		}
		legs.traj = traj;
		legs.Update();
		if (!exists)
		{
			return;
		}
		float num = 140f;
		if (speedFrame > 0f)
		{
			speedFrame -= FMan.frameTime;
			num = 240f;
		}
		Vector2 vector = loc;
		if (flag)
		{
			loc.X += traj.X * FMan.frameTime * num;
			if (MapMan.CheckHeroCol(loc))
			{
				loc.X = vector.X;
			}
			loc.Y += traj.Y * FMan.frameTime * num;
			if (MapMan.CheckHeroCol(loc))
			{
				loc.Y = vector.Y;
			}
		}
		else
		{
			loc += traj * FMan.frameTime * num;
		}
		float num2 = angle;
		if (shoot.Length() > 0.1f)
		{
			num2 = Trig.GetAngle(default(Vector2), shoot);
			if (shootFrame <= 0f && shoot.Length() > 0.9f)
			{
				angle = num2;
				Vector2 vector2 = shoot;
				vector2.Normalize();
				switch (weapon)
				{
				case Weapon.Rifle:
				{
					Sound.Play("auto");
					shootFrame = 0.1f;
					ParticleMan.AddParticle(1, loc, new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * -2000f, idx, 0f, 0);
					for (int m = 0; m < 10; m++)
					{
						ParticleMan.AddParticle(2, loc + (float)(m + 20) * -2f * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)), new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 5f, idx, Rand.GetRandomFloat(0f, 0.5f), 0);
					}
					break;
				}
				case Weapon.MachineGun:
				{
					Sound.Play("shotgun");
					shootFrame = 0.04f;
					specialAmmo--;
					float num4 = angle;
					num4 += Rand.GetRandomFloat(-0.12f, 0.12f);
					ParticleMan.AddParticle(1, loc, new Vector2((float)Math.Cos(num4), (float)Math.Sin(num4)) * -2000f, idx, 0f, 0);
					for (int j = 0; j < 10; j++)
					{
						ParticleMan.AddParticle(2, loc + (float)(j + 20) * -2f * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)), new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 5f, idx, Rand.GetRandomFloat(0f, 0.5f), 0);
					}
					break;
				}
				case Weapon.Shotty:
				{
					Sound.Play("shotty");
					shootFrame = 0.4f;
					specialAmmo--;
					for (int k = 0; k < 10; k++)
					{
						float num5 = angle;
						num5 += Rand.GetRandomFloat(-0.2f, 0.2f);
						ParticleMan.AddParticle(1, loc, new Vector2((float)Math.Cos(num5), (float)Math.Sin(num5)) * -2000f, idx, 0f, 0);
					}
					for (int l = 0; l < 10; l++)
					{
						ParticleMan.AddParticle(2, loc + (float)(l + 20) * -2f * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)), new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 5f, idx, Rand.GetRandomFloat(0f, 0.5f), 0);
					}
					break;
				}
				case Weapon.Flamethrower:
					Sound.Play("flame");
					shootFrame = 0.03f;
					specialAmmo--;
					ParticleMan.AddParticle(9, loc + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * -26f, new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * -500f, idx, 0f, 0);
					break;
				case Weapon.Rockets:
					Sound.Play("launch");
					shootFrame = 0.3f;
					specialAmmo--;
					ParticleMan.AddParticle(11, loc + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * -26f, new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * -1100f, idx, 0f, 0);
					break;
				case Weapon.Neutron:
				{
					Sound.Play("plas2");
					shootFrame = 0.2f;
					specialAmmo--;
					for (int i = -1; i < 2; i++)
					{
						float num3 = angle + (float)i * 0.3f;
						ParticleMan.AddParticle(12, loc + new Vector2((float)Math.Cos(num3), (float)Math.Sin(num3)) * -26f, new Vector2((float)Math.Cos(num3), (float)Math.Sin(num3)) * -1300f, idx, 0f, 0);
					}
					break;
				}
				case Weapon.Beam:
					Sound.Play("shrink");
					shootFrame = 0.2f;
					specialAmmo--;
					ParticleMan.AddParticle(14, loc + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * -26f, new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * -2000f, idx, 0f, 0);
					break;
				}
				if (weapon != Weapon.Rifle && specialAmmo <= 0)
				{
					weapon = Weapon.Rifle;
				}
			}
		}
		else if (traj.Length() > 0f)
		{
			num2 = Trig.GetAngle(default(Vector2), traj);
			num2 += (float)Math.Sin(legs.frame) * 0.1f;
		}
		if (shootFrame > 0f)
		{
			shootFrame -= FMan.frameTime;
		}
		float num6;
		for (num6 = num2 - angle; num6 < -3.14f; num6 += 6.28f)
		{
		}
		while (num6 > 3.14f)
		{
			num6 -= 6.28f;
		}
		angle += num6 * FMan.frameTime * 20f;
		float num7 = 100f;
		if (!flag)
		{
			if (loc.X < num7)
			{
				loc.X = num7;
			}
			if (loc.Y < num7)
			{
				loc.Y = num7;
			}
			if (loc.X > MapMan.mapSize.X - num7)
			{
				loc.X = MapMan.mapSize.X - num7;
			}
			if (loc.Y > MapMan.mapSize.Y - num7)
			{
				loc.Y = MapMan.mapSize.Y - num7;
			}
		}
	}

	public void Draw()
	{
		if (nameIn <= 0 && !(respawnFrame > 0f) && exists)
		{
			legs.Draw(loc, 1f);
			SpriteTools.sprite.Draw(ZombieGame.spritesTex, ScrollMan.GetScreenLoc(loc, 1f), new Rectangle(256 * idx, 0, 256, 128), Color.White, angle, new Vector2(138f, 64f), 0.3f * ScrollMan.zoom, SpriteEffects.None, 1f);
		}
	}

	private void UpdateKeys()
	{
		GamePadState state = GamePad.GetState((PlayerIndex)idx, GamePadDeadZone.Circular);
		if (state.Buttons.Start == ButtonState.Pressed && pgs.Buttons.Start == ButtonState.Released)
		{
			TimeMgr.CurTMgr().Pause(idx);
		}
		if (Guide.IsVisible)
		{
			TimeMgr.CurTMgr().Pause(idx);
		}
		if (!state.IsConnected)
		{
			TimeMgr.CurTMgr().Pause(idx);
		}
		traj = state.ThumbSticks.Left;
		traj.Y = 0f - traj.Y;
		shoot = state.ThumbSticks.Right;
		shoot.Y = 0f - shoot.Y;
		keyUp = false;
		keyDown = false;
		keyAccept = false;
		keyCancel = false;
		if ((state.Buttons.Start == ButtonState.Pressed && pgs.Buttons.Start == ButtonState.Released) || (state.Buttons.A == ButtonState.Pressed && pgs.Buttons.A == ButtonState.Released))
		{
			keyAccept = true;
		}
		if (state.Buttons.B == ButtonState.Pressed && pgs.Buttons.B == ButtonState.Released)
		{
			keyCancel = true;
		}
		if ((state.ThumbSticks.Left.Y > 0.3f && pgs.ThumbSticks.Left.Y <= 0.3f) || (state.DPad.Up == ButtonState.Pressed && pgs.DPad.Up == ButtonState.Released))
		{
			keyUp = true;
		}
		if ((state.ThumbSticks.Left.Y < -0.3f && pgs.ThumbSticks.Left.Y >= -0.3f) || (state.DPad.Down == ButtonState.Pressed && pgs.DPad.Down == ButtonState.Released))
		{
			keyDown = true;
		}
		pgs = state;
	}
}
