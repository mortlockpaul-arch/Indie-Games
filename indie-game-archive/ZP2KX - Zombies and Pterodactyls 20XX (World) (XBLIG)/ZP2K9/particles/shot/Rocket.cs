using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;
using ZP2K9.net;

namespace ZP2K9.particles.shot;

public class Rocket
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage, float splash)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		p.netSprite = 11;
		p.loc = loc;
		p.orig = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.alpha = false;
		p.frame = 1f;
		p.flags = damage;
		p.size = splash;
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.orig);
		NetPacker.WriteVec2(writer, p.traj);
		NetPacker.WriteByte(writer, p.flags);
		NetPacker.WriteByte(writer, (int)p.size);
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.traj = NetPacker.ReadVec2(reader);
		p.flags = NetPacker.ReadByte(reader);
		p.size = NetPacker.ReadByte(reader);
		p.frame = 1f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		Vector2 loc = p.loc;
		ref Vector2 traj = ref p.traj;
		traj.Y += fTime * Game1.gravity * 0.6f;
		float frame = p.frame;
		p.BaseUpdate(map, c, fTime);
		if ((int)(frame * 60f) != (int)(p.frame * 60f))
		{
			Game1.pMan.AddParticle(2, p.loc - p.traj * 0.01f, p.traj * -0.3f, Rand.GetRandomFloat(0.5f, 0.7f), 0, 0);
			Game1.pMan.AddParticle(1, p.loc - p.traj * 0.01f, p.traj * -0.2f - Rand.GetRandomVec2(-10f, 10f, -10f, 40f), Rand.GetRandomFloat(0.25f, 0.5f), 0, 0);
			Game1.pMan.AddParticle(1, p.loc - p.traj * 0.02f, p.traj * -0.2f, Rand.GetRandomFloat(0.25f, 0.5f), 0, 0);
		}
		if (HitManager.CheckHit(c, p, map, p.netOwner))
		{
			p.frame = -1f;
		}
		else if (map.GetIsCol(p.loc))
		{
			p.frame = -1f;
		}
		if (p.frame == -1f)
		{
			p.loc = loc;
			Game1.pMan.Explode(p.loc, p.netOwner, p.flags, p.size);
		}
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		p.angle = Trig.GetAngle(default(Vector2), p.traj) + 3.14f;
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(128, 32, 64, 32), new Color(new Vector4(1f, 1f, 1f, 1f)), p.angle, new Vector2(32f, 16f), Scroll.zoom * 0.4f, (SpriteEffects)0, 1f);
		Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.5f, 0.2f, 0.1f, 1f);
		Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 1f, 0.2f, 0.1f, 0.5f);
	}
}
