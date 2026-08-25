using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using ZP2K9.characters;
using ZP2K9.map;
using ZP2K9.net;

namespace ZP2K9.particles.shot;

public class Splash
{
	public static void Init(Particle p, Vector2 loc, int owner, int damage, float range)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		p.netSprite = 13;
		p.loc = loc;
		p.frame = 10f;
		p.flags = damage;
		p.size = range;
		p.netOwner = owner;
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
		((BinaryWriter)(object)writer).Write(NetPacker.FloatToInt16(p.flags));
		((BinaryWriter)(object)writer).Write(NetPacker.FloatToInt16(p.size));
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.flags = ((BinaryReader)(object)reader).ReadInt16();
		p.size = ((BinaryReader)(object)reader).ReadInt16();
		p.frame = 10f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		if (!p.ground)
		{
			HitManager.CheckHit(c, p, map, p.netOwner);
			p.ground = true;
		}
		p.frame -= fTime;
	}
}
