using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using ZP2K9.characters;
using ZP2K9.map;
using ZP2K9.net;

namespace ZP2K9.particles.grenades;

public class Crate
{
	public static void Init(Particle p, Vector2 loc, Vector2 traj, int owner)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		p.netSprite = 24;
		p.loc = loc;
		p.netOwner = owner;
		p.frame = 120f;
		p.size = 1f;
	}

	public static void NetWrite(Particle p, PacketWriter writer)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		NetPacker.WriteByte(writer, p.netSprite);
		NetPacker.WriteByte(writer, p.netOwner);
		NetPacker.WriteVec2(writer, p.loc);
	}

	public static void NetInit(Particle p, PacketReader reader)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		p.netInduced = true;
		p.netOwner = NetPacker.ReadByte(reader);
		p.loc = NetPacker.ReadVec2(reader);
		p.frame = 120f;
		p.size = 1f;
	}

	public static void Update(Particle p, GameMap map, Character[] c, float fTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		Vector2 loc = p.loc;
		p.loc += fTime * p.traj;
		if (p.ground)
		{
			p.traj = default(Vector2);
			if (p.size > 0f)
			{
				p.size -= fTime;
			}
		}
		else
		{
			p.traj = new Vector2(0f, 100f);
			if (map.GetIsCol(p.loc))
			{
				p.loc = loc;
				p.ground = true;
				p.traj = default(Vector2);
			}
		}
		for (int i = 0; i < c.Length; i++)
		{
			if (c[i] == null || c[i].hp < 0)
			{
				continue;
			}
			bool flag = true;
			if (GameState.gameType == 4 && Game1.character[i].team == 1)
			{
				flag = false;
			}
			Vector2 val = c[i].loc - new Vector2(0f, 32f) - p.loc;
			if (!(((Vector2)(ref val)).LengthSquared() < 2400f) || !flag)
			{
				continue;
			}
			if (c[i].charKeys.KeyPickup())
			{
				p.frame = -1f;
				c[i].GiveGoodies();
				Vector2 val2 = p.loc - Scroll.scroll;
				if (((Vector2)(ref val2)).Length() < 700f)
				{
					Sound.PlayCue("suit");
				}
				for (int j = 0; j < 32; j++)
				{
					Game1.pMan.AddParticle(38, c[i].loc + Rand.GetRandomVec2(-32f, 32f, -90f, 0f), new Vector2(0f, -30f), Rand.GetRandomFloat(0.2f, 0.5f), 0, 0);
				}
			}
			else if (Game1.netSession.GetPlayerOne() == i)
			{
				Game1.hud.AddPickup(0, 4);
			}
		}
		p.frame -= fTime;
	}

	public static void Draw(Particle p, SpriteBatch sprite)
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		int playerOne = Game1.netSession.GetPlayerOne();
		if (GameState.gameType != 4 || playerOne <= -1 || Game1.character[playerOne] == null || Game1.character[playerOne].team != 1)
		{
			if (p.size > 0f)
			{
				sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc - new Vector2(0f, 36f)), (Rectangle?)new Rectangle(832, 0, 96, 96), new Color(new Vector4(1f, 1f, 1f, p.size)), (float)Math.Cos(p.frame * 4f) * 0.1f, new Vector2(48f, 62f), Scroll.zoom * new Vector2(1f, p.size) * 0.55f, (SpriteEffects)0, 1f);
			}
			sprite.Draw(Game1.spritesTex, Scroll.GetLoc(p.loc), (Rectangle?)new Rectangle(762, 32, 64, 64), new Color(new Vector4(1f, 1f, 1f, p.frame)), 0f, new Vector2(32f, 58f), Scroll.zoom * 0.55f, (SpriteEffects)0, 1f);
		}
	}
}
