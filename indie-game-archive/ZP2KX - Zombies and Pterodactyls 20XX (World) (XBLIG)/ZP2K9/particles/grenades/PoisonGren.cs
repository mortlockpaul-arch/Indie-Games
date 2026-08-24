using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using ZP2K9.characters;
using ZP2K9.map;
using ZP2K9.net;

namespace ZP2K9.particles.grenades;

public class PoisonGren
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		p.netSprite = 6;
		p.loc = loc;
		p.traj = traj;
		p.netOwner = owner;
		p.frame = 6f;
		p.bounce = true;
		p.dir = Rand.GetRandomFloat(0f, 6.28f);
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
		NetPacker.WriteVec2(writer, p.traj);
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
		p.frame = 6f;
		p.bounce = true;
		p.angle = Rand.GetRandomRadian();
		p.dir = Rand.GetRandomRadian();
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		float num = p.frame - fTime;
		if (p.frame > 2f && p.frame < 5f && (int)(p.frame * 20f) != (int)(num * 20f))
		{
			Game1.pMan.AddParticle(4, p.loc, Rand.GetRandomVec2(-30f, 30f, -300f, -290f), 0f, 0, p.netOwner);
		}
		if (p.frame < 5f && (int)(p.frame * 20f) != (int)(num * 20f))
		{
			Game1.pMan.AddParticle(5, p.loc, default(Vector2), 200f, 0, p.netOwner);
		}
		if (p.frame < 1f)
		{
			p.frame = -1f;
			int damage = 200;
			float range = 200f;
			if (Game1.character[p.netOwner] != null && Game1.character[p.netOwner].perk[1] == 7)
			{
				damage = 250;
				range = 250f;
			}
			Game1.pMan.Explode(p.loc, p.netOwner, damage, range);
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(128, 448, 64, 64), new Color(new Vector4(1f, 1f, 1f, p.frame)), p.angle, new Vector2(32f, 32f), Scroll.zoom * 0.55f, (SpriteEffects)0, 1f);
	}
}
