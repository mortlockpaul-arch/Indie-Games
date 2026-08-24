using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.map;
using ZP2K9.net;

namespace ZP2K9.particles.pyro;

public class Flame
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		p.netSprite = 21;
		p.loc = loc;
		p.traj = traj;
		p.frame = 0.45f;
		p.netOwner = owner;
		p.size = Rand.GetRandomFloat(6f, 8f);
		p.dir = 0.6f;
		p.netWeak = true;
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
		p.size = Rand.GetRandomFloat(6f, 8f);
		p.dir = 0.6f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (p.frame > 0.1f)
		{
			if (HitManager.CheckHit(c, p, map, p.netOwner))
			{
				p.frame = -1f;
			}
			else if (map.GetIsCol(p.loc))
			{
				p.frame = -1f;
				Game1.pMan.AddParticle(15, p.loc - p.traj * 0.02f, default(Vector2), 1f, 0, p.netOwner);
			}
		}
		ref Vector2 traj = ref p.traj;
		traj.Y += Game1.frameTime * 900f;
		int num = (int)(p.loc.X / 64f);
		int num2 = (int)(p.loc.Y / 32f);
		if (num > 0 && num > 0 && num2 < 256 && num2 < 256 && map.water.water[num, num2])
		{
			p.frame -= fTime * 10f;
		}
		p.BaseUpdate(map, c, fTime);
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)((0.9f - p.frame * 2f) * 10f);
		if (num <= 8)
		{
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(num * 64, 160, 64, 64), new Color(new Vector4(1f, 1f, 1f, 1f)), Trig.GetAngle(default(Vector2), p.traj), new Vector2(32f, 32f), (0.6f - p.frame) * new Vector2(1f, 0.5f) * Scroll.zoom * p.size, (SpriteEffects)0, 1f);
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(num * 64, 160, 64, 64), new Color(new Vector4(1f, 1f, 1f, 0.2f)), Trig.GetAngle(default(Vector2), p.traj) + Rand.GetRandomFloat(-0.5f, 0.5f), new Vector2(32f, 32f), (0.5f - p.frame) * new Vector2(1f + p.frame, 0.7f) * Scroll.zoom * p.size * 2f, (SpriteEffects)0, 1f);
			float num2 = p.frame * 0.2f;
			if (num2 > 0.1f)
			{
				num2 = 0.1f;
			}
			Game1.postGlowMgr.Add(Scroll.GetLoc(p.loc), 1f, 0.5f, 0.2f, num2, 2f);
		}
	}
}
