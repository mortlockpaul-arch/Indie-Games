using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using ZP2K9.ai;
using ZP2K9.characters;
using ZP2K9.map;
using ZP2K9.net;

namespace ZP2K9.particles.grenades;

public class ZapGren
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		p.netSprite = 32;
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
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		float num = p.frame - fTime;
		if (p.frame < 5f && (int)(p.frame * 5f) != (int)(num * 5f))
		{
			for (int i = 0; i < Game1.character.Length; i++)
			{
				if (Game1.character[i] == null || i == p.netOwner || !HitManager.GetHostile(p.netOwner, i))
				{
					continue;
				}
				Vector2 val = p.loc - Game1.character[i].loc;
				if (((Vector2)(ref val)).LengthSquared() < 250000f && AI.GetVis(p.loc, Game1.character[i].loc + new Vector2(0f, -40f), map))
				{
					Vector2 val2 = Game1.character[i].loc + new Vector2(0f, -40f) - p.loc;
					((Vector2)(ref val2)).Normalize();
					val2 *= 1000f;
					Game1.pMan.AddParticle(45, p.loc, val2, 0f, 10, p.netOwner);
					Vector2 val3 = p.loc - Scroll.scroll;
					if (((Vector2)(ref val3)).Length() < 900f)
					{
						Sound.PlayCue("plasma");
					}
					if (p.ground)
					{
						p.ground = false;
						p.traj = new Vector2(0f, -700f);
					}
				}
			}
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
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(576, 448, 64, 64), new Color(new Vector4(1f, 1f, 1f, p.frame)), p.angle, new Vector2(32f, 32f), Scroll.zoom * 0.55f, (SpriteEffects)0, 1f);
		if (p.frame < 5f)
		{
			Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 0.1f, 0.2f, 1f, Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.8f, 1f), 1f);
		}
	}
}
