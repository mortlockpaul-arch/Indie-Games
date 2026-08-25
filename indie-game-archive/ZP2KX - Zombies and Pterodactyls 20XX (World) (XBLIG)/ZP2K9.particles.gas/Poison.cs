using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using ZP2K9.characters;
using ZP2K9.map;
using ZP2K9.net;

namespace ZP2K9.particles.gas;

public class Poison
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		p.netSprite = 15;
		p.netOwner = owner;
		p.loc = loc;
		p.traj = traj;
		p.frame = 2f;
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomFloat(-5f, 5f);
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
		NetPacker.WriteVec2(writer, p.traj);
		((BinaryWriter)(object)writer).Write(NetPacker.SmallFloatToByte(p.frame));
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
		p.frame = NetPacker.ByteToSmallFloat(((BinaryReader)(object)reader).ReadByte());
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomFloat(-5f, 5f);
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		p.frame -= fTime;
		Vector2 loc = p.loc;
		p.angle += fTime * p.dir;
		if (p.frame < 1.3f && p.traj.Y < -100f)
		{
			p.traj.Y = -100f;
		}
		if (p.frame < 0.9f && p.traj.Y < -50f)
		{
			p.traj.Y = -50f;
		}
		ref Vector2 loc2 = ref p.loc;
		loc2.Y += p.traj.Y * fTime;
		HitManager.CheckHit(c, p, map, p.netOwner);
		if (map.GetIsCol(p.loc))
		{
			float num = Math.Abs(p.traj.Y) * Rand.GetRandomFloat(0f, 0.5f);
			if (p.traj.X > 0f)
			{
				ref Vector2 traj = ref p.traj;
				traj.X += num;
			}
			else
			{
				ref Vector2 traj2 = ref p.traj;
				traj2.X -= num;
			}
			p.loc.Y = loc.Y;
			p.traj.Y = 0f;
		}
		ref Vector2 loc3 = ref p.loc;
		loc3.X += p.traj.X * Game1.frameTime;
		if (map.GetIsCol(p.loc))
		{
			p.loc.X = loc.X;
			p.traj.X = 0f;
		}
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		float num = p.frame;
		if (num > 1f)
		{
			num = 1f;
		}
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(64, 0, 64, 64), new Color(new Vector4(0f, 0.6f, 0f, num * 0.3f)), p.angle, new Vector2(32f, 32f), (2.1f - p.frame) * 1.2f * Scroll.zoom, (SpriteEffects)0, 1f);
	}
}
