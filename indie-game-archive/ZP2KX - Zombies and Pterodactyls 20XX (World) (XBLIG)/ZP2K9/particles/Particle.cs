using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using ZP2K9.characters;
using ZP2K9.map;
using ZP2K9.particles.debris;
using ZP2K9.particles.gas;
using ZP2K9.particles.gore;
using ZP2K9.particles.grenades;
using ZP2K9.particles.pyro;
using ZP2K9.particles.shot;

namespace ZP2K9.particles;

public class Particle
{
	public const int DIRT = 0;

	public const int EXPLOSION = 1;

	public const int SMOKE = 2;

	public const int SMOKEFARM = 3;

	public const int POISON = 4;

	public const int POISONSPLASH = 5;

	public const int BLOODCLOUD = 6;

	public const int EXITWOUND = 7;

	public const int BOMBLET = 8;

	public const int FLAMEGREN = 9;

	public const int GLAUNCH = 10;

	public const int GRENADE = 11;

	public const int MINE = 12;

	public const int MIRV = 13;

	public const int POISONGREN = 14;

	public const int NAPALM = 15;

	public const int PLASMABLAST = 16;

	public const int PLASMASMOKE = 17;

	public const int BRASS = 18;

	public const int BULLET = 19;

	public const int KICK = 20;

	public const int MINIBULLET = 21;

	public const int MUZZLEFLASH = 22;

	public const int PLASMA = 23;

	public const int PLASMAFLASH = 24;

	public const int ROCKET = 25;

	public const int SHELL = 26;

	public const int SHOT = 27;

	public const int SPLASH = 28;

	public const int FREEZEGREN = 29;

	public const int FREEZESPLASH = 30;

	public const int FLARE = 31;

	public const int FLARETRAIL = 32;

	public const int FREEZE = 33;

	public const int FLAME = 34;

	public const int SHRINK = 35;

	public const int SHRINKTRAIL = 36;

	public const int SHRINKSPLASH = 37;

	public const int FREEZESMOKE = 38;

	public const int GIB = 39;

	public const int ICE_GIB = 40;

	public const int SHRINKZAP = 41;

	public const int WATER_SPLASH = 42;

	public const int CRATE = 43;

	public const int SWORD = 44;

	public const int ELECTRICITY = 45;

	public const int ELECTIRICTYSPLASH = 46;

	public const int LASER = 47;

	public const int LASER_TRAIL = 48;

	public const int LASER_CHARGE = 49;

	public const int BUBBLE = 50;

	public const int DEDZ = 51;

	public const int BEE = 52;

	public const int BEE_BIT = 53;

	public const int SYRINGE = 54;

	public const int RAINBOW = 55;

	public const int RAINBOW_DUST = 56;

	public const int SYRINGE_DEAD = 57;

	public const int SPARK = 58;

	public const int TIMEGREN = 59;

	public const int ZAPGREN = 60;

	public const int NUKEGREN = 61;

	public const int VAMPIREGREN = 62;

	public const int AKFISH = 63;

	public const int FIRE_ROCKET = 64;

	public const int FLAMESWORD = 65;

	public const int NUKESPLODE = 66;

	public const int CAT = 67;

	public Vector2 loc;

	public Vector2 traj;

	public Vector2 orig;

	public float frame;

	public bool alpha;

	public float size;

	public float angle;

	public bool ground;

	public float dir;

	public bool bounce;

	public int flags;

	public int netID;

	public int netSprite = -1;

	public int netOwner = -1;

	public bool netInduced;

	public bool netWeak;

	public int type;

	public bool exists;

	public bool impotent;

	private void Clear()
	{
		alpha = false;
		exists = true;
		angle = 0f;
		ground = false;
		dir = 0f;
		bounce = false;
		netSprite = -1;
		netID = 0;
		netInduced = false;
		impotent = false;
		netWeak = false;
	}

	public void Init(int type, Vector2 loc, Vector2 traj, float size, int flags, int netOwner)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		Clear();
		this.type = type;
		this.loc = loc;
		this.traj = traj;
		this.size = size;
		this.flags = flags;
		this.netOwner = -1;
		switch (type)
		{
		case 67:
			Cat.Init(this, loc, traj, netOwner, flags);
			break;
		case 66:
			Nukesplode.Init(this, loc, traj, size);
			break;
		case 61:
			NukeGren.Init(this, loc, traj, netOwner);
			break;
		case 62:
			VampireGren.Init(this, loc, traj, netOwner);
			break;
		case 64:
			FireRocket.Init(this, loc, traj, netOwner, flags, size);
			break;
		case 60:
			ZapGren.Init(this, loc, traj, netOwner);
			break;
		case 59:
			TimeGren.Init(this, loc, traj, netOwner);
			break;
		case 58:
			Spark.Init(this, loc, traj);
			break;
		case 51:
			Dedz.Init(this, loc);
			break;
		case 50:
			Bubble.Init(this, loc, traj, size);
			break;
		case 49:
			LaserCharge.Init(this, loc, traj, size);
			break;
		case 45:
			Electricity.Init(this, loc, traj, netOwner, flags);
			break;
		case 46:
			ElectricitySplash.Init(this, loc, traj);
			break;
		case 0:
			Dirt.Init(this, loc, traj, size);
			break;
		case 42:
			WaterSplash.Init(this, loc, traj, size);
			break;
		case 1:
			Explosion.Init(this, loc, size);
			break;
		case 2:
			Smoke.Init(this, loc, traj, size);
			break;
		case 3:
			SmokeFarm.Init(this, loc, traj);
			break;
		case 4:
			Poison.Init(this, loc, traj, netOwner);
			break;
		case 5:
			PoisonSplash.Init(this, loc, netOwner, flags, size);
			break;
		case 6:
			BloodCloud.Init(this, loc, traj, size);
			break;
		case 7:
			ExitWound.Init(this, loc, traj, size);
			break;
		case 8:
			Bomblet.Init(this, loc, traj, netOwner);
			break;
		case 9:
			FlameGren.Init(this, loc, traj, netOwner);
			break;
		case 10:
			GLaunch.Init(this, loc, traj, netOwner, flags, size);
			break;
		case 11:
			Grenade.Init(this, loc, traj, netOwner);
			break;
		case 12:
			Mine.Init(this, loc, traj, netOwner);
			break;
		case 13:
			Mirv.Init(this, loc, traj, netOwner);
			break;
		case 14:
			PoisonGren.Init(this, loc, traj, netOwner);
			break;
		case 15:
			Napalm.Init(this, loc, traj, netOwner);
			break;
		case 16:
			PlasmaBlast.Init(this, loc, traj);
			break;
		case 17:
			PlasmaSmoke.Init(this, loc, traj);
			break;
		case 18:
			Brass.Init(this, loc, traj);
			break;
		case 19:
			Bullet.Init(this, loc, traj, netOwner, flags);
			break;
		case 20:
			Kick.Init(this, loc, traj, netOwner, flags);
			break;
		case 44:
			Sword.Init(this, loc, traj, netOwner, flags);
			break;
		case 65:
			FlameSword.Init(this, loc, traj, netOwner, flags);
			break;
		case 21:
			MiniBullet.Init(this, loc, traj, netOwner, flags);
			break;
		case 22:
			MuzzleFlash.Init(this, loc, traj, size);
			break;
		case 23:
			Plasma.Init(this, loc, traj, netOwner, flags);
			break;
		case 24:
			PlasmaFlash.Init(this, loc, traj, size);
			break;
		case 25:
			Rocket.Init(this, loc, traj, netOwner, flags, size);
			break;
		case 26:
			Shell.Init(this, loc, traj);
			break;
		case 27:
			Shot.Init(this, loc, traj, netOwner, flags);
			break;
		case 28:
			Splash.Init(this, loc, netOwner, flags, size);
			break;
		case 29:
			FreezeGren.Init(this, loc, traj, netOwner);
			break;
		case 30:
			FreezeSplash.Init(this, loc, netOwner, flags, size);
			break;
		case 31:
			Flare.Init(this, loc, traj, netOwner, flags);
			break;
		case 32:
			FlareTrail.Init(this, loc, traj, size);
			break;
		case 33:
			Freeze.Init(this, loc, traj, netOwner, flags);
			break;
		case 34:
			Flame.Init(this, loc, traj, netOwner);
			break;
		case 35:
			Shrink.Init(this, loc, traj, netOwner, flags, size);
			break;
		case 36:
			ShrinkTrail.Init(this, loc, traj, size);
			break;
		case 41:
			ShrinkZap.Init(this, loc, traj, size);
			break;
		case 47:
			Laser.Init(this, loc, traj, netOwner, flags, size);
			break;
		case 48:
			LaserTrail.Init(this, loc, traj, size);
			break;
		case 37:
			ShrinkSplash.Init(this, loc, netOwner, flags, size);
			break;
		case 38:
			FreezeSmoke.Init(this, loc, traj);
			break;
		case 39:
			Gib.Init(this, loc, traj, netOwner);
			break;
		case 40:
			IceGib.Init(this, loc, traj, netOwner);
			break;
		case 43:
			Crate.Init(this, loc, traj, netOwner);
			break;
		case 52:
			Bee.Init(this, loc, traj, netOwner, flags);
			break;
		case 53:
			BeeBit.Init(this, loc, traj, size);
			break;
		case 55:
			Rainbow.Init(this, loc, traj, netOwner, flags);
			break;
		case 56:
			RainbowDust.Init(this, loc, traj, size);
			break;
		case 54:
			Syringe.Init(this, loc, traj, netOwner, flags);
			break;
		case 57:
			SyringeDead.Init(this, loc, traj);
			break;
		default:
			Console.WriteLine("Can't init particle: " + type);
			break;
		}
	}

	public void Init(int type, PacketReader reader)
	{
		Clear();
		this.type = type;
		switch (type)
		{
		case 67:
			Cat.NetInit(this, reader);
			break;
		case 61:
			NukeGren.NetInit(this, reader);
			break;
		case 62:
			VampireGren.NetInit(this, reader);
			break;
		case 64:
			FireRocket.NetInit(this, reader);
			break;
		case 60:
			ZapGren.NetInit(this, reader);
			break;
		case 59:
			TimeGren.NetInit(this, reader);
			break;
		case 45:
			Electricity.NetInit(this, reader);
			break;
		case 4:
			Poison.NetInit(this, reader);
			break;
		case 8:
			Bomblet.NetInit(this, reader);
			break;
		case 9:
			FlameGren.NetInit(this, reader);
			break;
		case 10:
			GLaunch.NetInit(this, reader);
			break;
		case 11:
			Grenade.NetInit(this, reader);
			break;
		case 12:
			Mine.NetInit(this, reader);
			break;
		case 13:
			Mirv.NetInit(this, reader);
			break;
		case 14:
			PoisonGren.NetInit(this, reader);
			break;
		case 15:
			Napalm.NetInit(this, reader);
			break;
		case 19:
			Bullet.NetInit(this, reader);
			break;
		case 20:
			Kick.NetInit(this, reader);
			break;
		case 44:
			Sword.NetInit(this, reader);
			break;
		case 65:
			FlameSword.NetInit(this, reader);
			break;
		case 21:
			MiniBullet.NetInit(this, reader);
			break;
		case 23:
			Plasma.NetInit(this, reader);
			break;
		case 25:
			Rocket.NetInit(this, reader);
			break;
		case 27:
			Shot.NetInit(this, reader);
			break;
		case 28:
			Splash.NetInit(this, reader);
			break;
		case 29:
			FreezeGren.NetInit(this, reader);
			break;
		case 31:
			Flare.NetInit(this, reader);
			break;
		case 33:
			Freeze.NetInit(this, reader);
			break;
		case 34:
			Flame.NetInit(this, reader);
			break;
		case 35:
			Shrink.NetInit(this, reader);
			break;
		case 37:
			ShrinkSplash.NetInit(this, reader);
			break;
		case 39:
			Gib.NetInit(this, reader);
			break;
		case 40:
			IceGib.NetInit(this, reader);
			break;
		case 43:
			Crate.NetInit(this, reader);
			break;
		case 47:
			Laser.NetInit(this, reader);
			break;
		case 52:
			Bee.NetInit(this, reader);
			break;
		case 55:
			Rainbow.NetInit(this, reader);
			break;
		case 54:
			Syringe.NetInit(this, reader);
			break;
		case 5:
		case 6:
		case 7:
		case 16:
		case 17:
		case 18:
		case 22:
		case 24:
		case 26:
		case 30:
		case 32:
		case 36:
		case 38:
		case 41:
		case 42:
		case 46:
		case 48:
		case 49:
		case 50:
		case 51:
		case 53:
		case 56:
		case 57:
		case 58:
		case 63:
		case 66:
			break;
		}
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Scroll.GetLoc(loc);
		if (type == 66 && (val.X < -128f || val.Y < -364f || val.X > 1408f || val.Y > 848f))
		{
			return;
		}
		if (type == 59)
		{
			if (val.X < -256f || val.Y < -256f || val.X > 1536f || val.Y > 976f)
			{
				return;
			}
		}
		else if (val.X < -64f || val.Y < -64f || val.X > 1344f || val.Y > 784f)
		{
			return;
		}
		switch (type)
		{
		case 67:
			Cat.Draw(this, sprite);
			break;
		case 66:
			Nukesplode.Draw(this, sprite);
			break;
		case 62:
			VampireGren.Draw(this, sprite);
			break;
		case 61:
			NukeGren.Draw(this, sprite);
			break;
		case 64:
			FireRocket.Draw(this, sprite);
			break;
		case 60:
			ZapGren.Draw(this, sprite);
			break;
		case 59:
			TimeGren.Draw(this, sprite);
			break;
		case 58:
			Spark.Draw(this, sprite);
			break;
		case 51:
			Dedz.Draw(this, sprite);
			break;
		case 50:
			Bubble.Draw(this, sprite);
			break;
		case 49:
			LaserCharge.Draw(this, sprite);
			break;
		case 45:
			Electricity.Draw(this, sprite);
			break;
		case 46:
			ElectricitySplash.Draw(this, sprite);
			break;
		case 0:
			Dirt.Draw(this, sprite);
			break;
		case 42:
			WaterSplash.Draw(this, sprite);
			break;
		case 1:
			Explosion.Draw(this, sprite);
			break;
		case 2:
			Smoke.Draw(this, sprite);
			break;
		case 3:
			SmokeFarm.Draw(this, sprite);
			break;
		case 4:
			Poison.Draw(this, sprite);
			break;
		case 6:
			BloodCloud.Draw(this, sprite);
			break;
		case 7:
			ExitWound.Draw(this, sprite);
			break;
		case 8:
			Bomblet.Draw(this, sprite);
			break;
		case 9:
			FlameGren.Draw(this, sprite);
			break;
		case 10:
			GLaunch.Draw(this, sprite);
			break;
		case 11:
			Grenade.Draw(this, sprite);
			break;
		case 12:
			Mine.Draw(this, sprite);
			break;
		case 13:
			Mirv.Draw(this, sprite);
			break;
		case 14:
			PoisonGren.Draw(this, sprite);
			break;
		case 15:
			Napalm.Draw(this, sprite);
			break;
		case 16:
			PlasmaBlast.Draw(this, sprite);
			break;
		case 17:
			PlasmaSmoke.Draw(this, sprite);
			break;
		case 18:
			Brass.Draw(this, sprite);
			break;
		case 19:
			Bullet.Draw(this, sprite);
			break;
		case 20:
			Kick.Draw(this, sprite);
			break;
		case 44:
			Sword.Draw(this, sprite);
			break;
		case 65:
			FlameSword.Draw(this, sprite);
			break;
		case 21:
			MiniBullet.Draw(this, sprite);
			break;
		case 22:
			MuzzleFlash.Draw(this, sprite);
			break;
		case 23:
			Plasma.Draw(this, sprite);
			break;
		case 24:
			PlasmaFlash.Draw(this, sprite);
			break;
		case 25:
			Rocket.Draw(this, sprite);
			break;
		case 26:
			Shell.Draw(this, sprite);
			break;
		case 27:
			Shot.Draw(this, sprite);
			break;
		case 29:
			FreezeGren.Draw(this, sprite);
			break;
		case 31:
			Flare.Draw(this, sprite);
			break;
		case 32:
			FlareTrail.Draw(this, sprite);
			break;
		case 33:
			Freeze.Draw(this, sprite);
			break;
		case 34:
			Flame.Draw(this, sprite);
			break;
		case 35:
			Shrink.Draw(this, sprite);
			break;
		case 36:
			ShrinkTrail.Draw(this, sprite);
			break;
		case 41:
			ShrinkZap.Draw(this, sprite);
			break;
		case 38:
			FreezeSmoke.Draw(this, sprite);
			break;
		case 43:
			Crate.Draw(this, sprite);
			break;
		case 47:
			Laser.Draw(this, sprite);
			break;
		case 48:
			LaserTrail.Draw(this, sprite);
			break;
		case 52:
			Bee.Draw(this, sprite);
			break;
		case 53:
			BeeBit.Draw(this, sprite);
			break;
		case 56:
			RainbowDust.Draw(this, sprite);
			break;
		case 54:
			Syringe.Draw(this, sprite);
			break;
		case 57:
			SyringeDead.Draw(this, sprite);
			break;
		default:
			Console.WriteLine("Can't init particle: " + type);
			break;
		case 5:
		case 28:
		case 30:
		case 37:
		case 39:
		case 40:
		case 55:
			break;
		}
	}

	public void Update(GameMap map, Character[] c, float fTime)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (Game1.pMan.GetChronod(loc))
		{
			fTime /= 10f;
		}
		switch (type)
		{
		case 67:
			Cat.Update(this, map, c, fTime);
			break;
		case 66:
			Nukesplode.Update(this, map, c, fTime);
			break;
		case 61:
			NukeGren.Update(this, map, c, fTime);
			break;
		case 62:
			VampireGren.Update(this, map, c, fTime);
			break;
		case 64:
			FireRocket.Update(this, map, c, fTime);
			break;
		case 60:
			ZapGren.Update(this, map, c, fTime);
			break;
		case 59:
			TimeGren.Update(this, map, c, fTime);
			break;
		case 58:
			Spark.Update(this, map, c, fTime);
			break;
		case 51:
			Dedz.Update(this, map, c, fTime);
			break;
		case 50:
			Bubble.Update(this, map, c, fTime);
			break;
		case 49:
			LaserCharge.Update(this, map, c, fTime);
			break;
		case 45:
			Electricity.Update(this, map, c, fTime);
			break;
		case 47:
			Laser.Update(this, map, c, fTime);
			break;
		case 48:
			LaserTrail.Update(this, map, c, fTime);
			break;
		case 1:
			Explosion.Update(this, map, c, fTime);
			break;
		case 2:
			Smoke.Update(this, map, c, fTime);
			break;
		case 3:
			SmokeFarm.Update(this, map, c, fTime);
			break;
		case 42:
			WaterSplash.Update(this, fTime);
			BaseUpdate(map, c, fTime);
			break;
		case 4:
			Poison.Update(this, map, c, fTime);
			break;
		case 5:
			PoisonSplash.Update(this, map, c, fTime);
			break;
		case 6:
			BloodCloud.Update(this, map, c, fTime);
			break;
		case 7:
			ExitWound.Update(this, map, c, fTime);
			break;
		case 8:
			Bomblet.Update(this, map, c, fTime);
			break;
		case 9:
			FlameGren.Update(this, map, c, fTime);
			break;
		case 10:
			GLaunch.Update(this, map, c, fTime);
			break;
		case 11:
			Grenade.Update(this, map, c, fTime);
			break;
		case 12:
			Mine.Update(this, map, c, fTime);
			break;
		case 13:
			Mirv.Update(this, map, c, fTime);
			break;
		case 14:
			PoisonGren.Update(this, map, c, fTime);
			break;
		case 15:
			Napalm.Update(this, map, c, fTime);
			break;
		case 19:
			Bullet.Update(this, map, c, fTime);
			break;
		case 20:
			Kick.Update(this, map, c, fTime);
			break;
		case 44:
			Sword.Update(this, map, c, fTime);
			break;
		case 65:
			FlameSword.Update(this, map, c, fTime);
			break;
		case 21:
			MiniBullet.Update(this, map, c, fTime);
			break;
		case 23:
			Plasma.Update(this, map, c, fTime);
			break;
		case 25:
			Rocket.Update(this, map, c, fTime);
			break;
		case 27:
			Shot.Update(this, map, c, fTime);
			break;
		case 28:
			Splash.Update(this, map, c, fTime);
			break;
		case 29:
			FreezeGren.Update(this, map, c, fTime);
			break;
		case 30:
			FreezeSplash.Update(this, map, c, fTime);
			break;
		case 31:
			Flare.Update(this, map, c, fTime);
			break;
		case 32:
			FlareTrail.Update(this, map, c, fTime);
			break;
		case 33:
			Freeze.Update(this, map, c, fTime);
			break;
		case 34:
			Flame.Update(this, map, c, fTime);
			break;
		case 35:
			Shrink.Update(this, map, c, fTime);
			break;
		case 36:
			ShrinkTrail.Update(this, map, c, fTime);
			break;
		case 37:
			ShrinkSplash.Update(this, map, c, fTime);
			break;
		case 39:
			Gib.Update(this, map, c, fTime);
			break;
		case 40:
			IceGib.Update(this, map, c, fTime);
			break;
		case 43:
			Crate.Update(this, map, c, fTime);
			break;
		case 52:
			Bee.Update(this, map, c, fTime);
			break;
		case 55:
			Rainbow.Update(this, map, c, fTime);
			break;
		case 54:
			Syringe.Update(this, map, c, fTime);
			break;
		default:
			BaseUpdate(map, c, fTime);
			break;
		}
	}

	public void NetWrite(PacketWriter writer)
	{
		switch (type)
		{
		case 67:
			Cat.NetWrite(this, writer);
			break;
		case 62:
			VampireGren.NetWrite(this, writer);
			break;
		case 61:
			NukeGren.NetWrite(this, writer);
			break;
		case 64:
			FireRocket.NetWrite(this, writer);
			break;
		case 60:
			ZapGren.NetWrite(this, writer);
			break;
		case 59:
			TimeGren.NetWrite(this, writer);
			break;
		case 45:
			Electricity.NetWrite(this, writer);
			break;
		case 4:
			Poison.NetWrite(this, writer);
			break;
		case 8:
			Bomblet.NetWrite(this, writer);
			break;
		case 9:
			FlameGren.NetWrite(this, writer);
			break;
		case 10:
			GLaunch.NetWrite(this, writer);
			break;
		case 11:
			Grenade.NetWrite(this, writer);
			break;
		case 12:
			Mine.NetWrite(this, writer);
			break;
		case 13:
			Mirv.NetWrite(this, writer);
			break;
		case 14:
			PoisonGren.NetWrite(this, writer);
			break;
		case 15:
			Napalm.NetWrite(this, writer);
			break;
		case 19:
			Bullet.NetWrite(this, writer);
			break;
		case 20:
			Kick.NetWrite(this, writer);
			break;
		case 44:
			Sword.NetWrite(this, writer);
			break;
		case 65:
			FlameSword.NetWrite(this, writer);
			break;
		case 21:
			MiniBullet.NetWrite(this, writer);
			break;
		case 23:
			Plasma.NetWrite(this, writer);
			break;
		case 25:
			Rocket.NetWrite(this, writer);
			break;
		case 27:
			Shot.NetWrite(this, writer);
			break;
		case 28:
			Splash.NetWrite(this, writer);
			break;
		case 29:
			FreezeGren.NetWrite(this, writer);
			break;
		case 31:
			Flare.NetWrite(this, writer);
			break;
		case 33:
			Freeze.NetWrite(this, writer);
			break;
		case 34:
			Flame.NetWrite(this, writer);
			break;
		case 35:
			Shrink.NetWrite(this, writer);
			break;
		case 47:
			Laser.NetWrite(this, writer);
			break;
		case 37:
			ShrinkSplash.NetWrite(this, writer);
			break;
		case 43:
			Crate.NetWrite(this, writer);
			break;
		case 39:
			Gib.NetWrite(this, writer);
			break;
		case 40:
			IceGib.NetWrite(this, writer);
			break;
		case 52:
			Bee.NetWrite(this, writer);
			break;
		case 55:
			Rainbow.NetWrite(this, writer);
			break;
		case 54:
			Syringe.NetWrite(this, writer);
			break;
		case 5:
		case 6:
		case 7:
		case 16:
		case 17:
		case 18:
		case 22:
		case 24:
		case 26:
		case 30:
		case 32:
		case 36:
		case 38:
		case 41:
		case 42:
		case 46:
		case 48:
		case 49:
		case 50:
		case 51:
		case 53:
		case 56:
		case 57:
		case 58:
		case 63:
		case 66:
			break;
		}
	}

	public void BaseUpdate(GameMap map, Character[] c, float fTime)
	{
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		if (bounce)
		{
			Vector2 val = loc;
			if (!ground)
			{
				ref Vector2 reference = ref traj;
				reference.Y += Game1.gravity * fTime;
				ref Vector2 reference2 = ref loc;
				reference2.Y += traj.Y * fTime;
				bool flag = false;
				if (map.GetIsCol(loc))
				{
					loc.Y = val.Y;
					ref Vector2 reference3 = ref traj;
					reference3.Y *= -0.5f;
					ref Vector2 reference4 = ref traj;
					reference4.X *= 0.45f;
					dir = Rand.GetRandomFloat(-10f, 10f);
					if (traj.Y > -10f && traj.Y < 10f)
					{
						ground = true;
					}
					Vector2 val2 = loc - Scroll.scroll;
					if (((Vector2)(ref val2)).LengthSquared() < 90000f)
					{
						flag = true;
					}
				}
				ref Vector2 reference5 = ref loc;
				reference5.X += traj.X * fTime;
				int num = (int)(loc.X / 64f);
				int num2 = (int)(loc.Y / 32f);
				if (num > 0 && num > 0 && num2 < 256 && num2 < 256 && map.water.water[num, num2])
				{
					if (traj.X > 50f)
					{
						traj.X = 50f;
					}
					if (traj.X < -50f)
					{
						traj.X = -50f;
					}
					if (traj.Y > 50f)
					{
						traj.Y = 50f;
					}
				}
				if (map.GetIsCol(loc))
				{
					loc.X = val.X;
					ref Vector2 reference6 = ref traj;
					reference6.X *= -0.5f;
					dir = Rand.GetRandomFloat(-10f, 10f);
					Vector2 val3 = loc - Scroll.scroll;
					if (((Vector2)(ref val3)).LengthSquared() < 90000f)
					{
						flag = true;
					}
				}
				if (flag)
				{
					switch (type)
					{
					case 26:
						Sound.PlayCue("shell");
						break;
					case 18:
						Sound.PlayBrass();
						break;
					case 8:
					case 10:
					case 11:
					case 13:
					case 14:
					case 29:
					case 60:
					case 61:
					case 62:
						Sound.PlayCue("click3");
						break;
					case 39:
						switch (Rand.GetRandomInt(0, 16))
						{
						case 0:
							Sound.PlayCue("hit1");
							break;
						case 1:
							Sound.PlayCue("hit2");
							break;
						case 2:
							Sound.PlayCue("hit3");
							break;
						}
						break;
					}
				}
				angle += dir * fTime;
			}
		}
		else
		{
			loc += fTime * traj;
		}
		frame -= fTime;
	}

	public bool GetVis()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Scroll.GetLoc(loc);
		if (val.X < -100f)
		{
			return false;
		}
		if (type == 66)
		{
			if (val.Y < -900f)
			{
				return false;
			}
		}
		else if (val.Y < -100f)
		{
			return false;
		}
		if (val.X > 1380f)
		{
			return false;
		}
		if (val.Y > 820f)
		{
			return false;
		}
		return true;
	}
}
