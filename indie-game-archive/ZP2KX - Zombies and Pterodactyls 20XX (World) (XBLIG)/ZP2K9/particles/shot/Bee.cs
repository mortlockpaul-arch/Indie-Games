using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using ZP2K9.characters;
using ZP2K9.map;
using ZP2K9.net;

namespace ZP2K9.particles.shot;

public class Bee
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner, int damage)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		p.netSprite = 28;
		p.loc = loc;
		p.orig = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.alpha = false;
		p.frame = 2f;
		p.flags = damage;
		p.dir = Rand.GetRandomFloat(-30f, 30f);
		p.angle = Rand.GetRandomRadian();
		p.netWeak = true;
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
		p.frame = 1.2f;
		p.dir = Rand.GetRandomFloat(-30f, 30f);
		p.angle = Rand.GetRandomRadian();
		p.alpha = true;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		_ = p.frame;
		Vector2 loc = p.loc;
		p.frame -= fTime;
		p.angle += p.dir * fTime;
		if (HitManager.CheckHit(c, p, map, p.netOwner))
		{
			p.frame = -1f;
		}
		else
		{
			bool flag = false;
			ref Vector2 loc2 = ref p.loc;
			loc2.X += p.traj.X * fTime;
			if (map.GetIsCol(p.loc))
			{
				p.loc.X = loc.X;
				p.traj.X = 0f - p.traj.X;
				flag = true;
			}
			ref Vector2 loc3 = ref p.loc;
			loc3.Y += p.traj.Y * fTime;
			if (map.GetIsCol(p.loc))
			{
				p.loc.Y = loc.Y;
				p.traj.Y = 0f - p.traj.Y;
				flag = true;
			}
			if (flag && Rand.CointToss(0.1f))
			{
				Vector2 val = p.loc - Scroll.scroll;
				if (((Vector2)(ref val)).LengthSquared() < 810000f)
				{
					Sound.PlayCue("swarm");
				}
			}
		}
		int num = (int)(p.loc.X / 64f);
		int num2 = (int)(p.loc.Y / 32f);
		if (num > 0 && num > 0 && num2 < 256 && num2 < 256 && map.water.water[num, num2])
		{
			p.frame -= fTime * 20f;
			if (p.frame < 0f)
			{
				p.frame = -1f;
			}
		}
		if (p.frame == -1f)
		{
			Vector2 val2 = p.loc - Scroll.scroll;
			if (((Vector2)(ref val2)).LengthSquared() < 810000f)
			{
				Sound.PlayCue("swarm");
			}
		}
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		float num = 2f - p.frame;
		if (num > 0.5f)
		{
			num = 0.5f;
		}
		Vector2 loc = Scroll.GetLoc(p.loc + new Vector2((float)Math.Cos(p.angle), (float)Math.Sin(p.angle)) * num * 50f);
		sprite.Draw(Game1.spritesTex, loc, (Rectangle?)new Rectangle(832 + (int)(p.frame * 30f) % 2 * 16, 224, 16, 16), new Color(1f, 1f, 1f, p.frame * 3f), 0f, new Vector2(8f, 8f), Scroll.zoom * 0.8f, (SpriteEffects)(!(p.traj.X < 0f)), 1f);
	}
}
