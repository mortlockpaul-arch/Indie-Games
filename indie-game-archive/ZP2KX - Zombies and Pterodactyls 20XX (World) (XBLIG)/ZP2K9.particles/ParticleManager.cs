using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using ZP2K9.characters;
using ZP2K9.map;
using ZP2K9.net;

namespace ZP2K9.particles;

public class ParticleManager
{
	public Particle[] particle;

	private float frame;

	public Explode[] netExplode;

	private ChronoMgr chronoMgr;

	public ParticleManager()
	{
		particle = new Particle[2048];
		for (int i = 0; i < particle.Length; i++)
		{
			particle[i] = new Particle();
		}
		netExplode = new Explode[8];
		for (int j = 0; j < netExplode.Length; j++)
		{
			netExplode[j] = new Explode();
		}
		chronoMgr = new ChronoMgr();
	}

	public void Update(GameMap map, Character[] c)
	{
		frame += Game1.frameTime;
		if (frame > 6.28f)
		{
			frame -= 6.28f;
		}
		for (int i = 0; i < particle.Length; i++)
		{
			if (!particle[i].exists)
			{
				continue;
			}
			try
			{
				particle[i].Update(map, c, Game1.frameTime);
			}
			catch
			{
				particle[i].exists = false;
			}
			if (particle[i].frame <= 0f)
			{
				if (particle[i].netSprite > -1)
				{
					particle[i].impotent = true;
				}
				else
				{
					particle[i].exists = false;
				}
			}
		}
	}

	public void NetCleanup(int playerOne)
	{
		if (Game1.netSession.netType == 1 || Game1.netSession.netType == 0)
		{
			NetWriteCleanup();
			return;
		}
		for (int i = 0; i < particle.Length; i++)
		{
			if (particle[i].exists && particle[i].netOwner > -1 && !Game1.netSession.GetNetworkOwner(particle[i].netOwner) && !particle[i].netInduced)
			{
				int type = particle[i].type;
				if (type != 5 && type != 30)
				{
					particle[i].exists = false;
				}
			}
		}
	}

	public void NetWriteCleanup()
	{
		for (int i = 0; i < particle.Length; i++)
		{
			if (particle[i].exists && particle[i].netOwner > -1 && Game1.netSession.GetNetworkOwner(particle[i].netOwner) && particle[i].netSprite > -1)
			{
				particle[i].netSprite = -1;
			}
		}
	}

	public static bool CheckVisibleToPlayer(Vector2 loc, Vector2 traj, float frame, int dest)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (Game1.character[dest] == null)
		{
			return true;
		}
		Vector2 loc2 = Game1.character[dest].loc;
		if (loc.X > loc2.X - 800f && loc.X < loc2.X + 800f && loc.Y > loc2.Y - 500f && loc.Y < loc2.Y + 500f)
		{
			return true;
		}
		Vector2 val = loc + traj * frame;
		if (val.X > loc2.X - 800f && val.X < loc2.X + 800f && val.Y > loc2.Y - 500f && val.Y < loc2.Y + 500f)
		{
			return true;
		}
		return false;
	}

	public void WriteParticles(int playerOne, int dest, PacketWriter writer)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < particle.Length; i++)
		{
			if (!particle[i].exists || particle[i].netSprite <= -1 || particle[i].netOwner <= -1 || !Game1.netSession.GetNetworkOwner(particle[i].netOwner) || Game1.character[dest] == null)
			{
				continue;
			}
			bool flag = false;
			if (particle[i].netWeak)
			{
				flag = CheckVisibleToPlayer(particle[i].loc, particle[i].traj, particle[i].frame, dest);
				if (!flag && dest == 0)
				{
					for (int j = 10; j < Game1.character.Length; j++)
					{
						if (Game1.character[j] != null && CheckVisibleToPlayer(particle[i].loc, particle[i].traj, particle[i].frame, j))
						{
							flag = true;
							break;
						}
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				NetPacker.WriteMsg(writer, 4);
				particle[i].NetWrite(writer);
			}
		}
	}

	public void AddParticle(int type, Vector2 loc, Vector2 traj, float size, int flags, int netOwner)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < particle.Length; i++)
		{
			if (!particle[i].exists)
			{
				particle[i].Init(type, loc, traj, size, flags, netOwner);
				particle[i].exists = true;
				break;
			}
		}
	}

	public void AddParticle(int type, PacketReader reader)
	{
		for (int i = 0; i < particle.Length; i++)
		{
			if (!particle[i].exists)
			{
				particle[i].Init(type, reader);
				particle[i].exists = true;
				break;
			}
		}
	}

	public void Draw(SpriteBatch sprite, bool alpha)
	{
		for (int i = 0; i < particle.Length; i++)
		{
			if (particle[i].exists && particle[i].alpha == alpha)
			{
				particle[i].Draw(sprite);
			}
		}
	}

	internal void Explode(Vector2 loc, int owner, int damage, float range)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (Game1.netSession.netType == 3 || Game1.netSession.netType == 2)
		{
			if (!Game1.netSession.GetNetworkOwner(owner))
			{
				return;
			}
			for (int i = 0; i < netExplode.Length; i++)
			{
				if (!netExplode[i].exists)
				{
					netExplode[i].Init(loc, range, damage);
					break;
				}
			}
		}
		DoExplode(loc, owner, damage, range, net: false);
	}

	internal void Explode(PacketReader reader)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 loc = NetPacker.ReadVec2(reader);
		int damage = NetPacker.ReadByte(reader);
		float range = NetPacker.ReadByte(reader);
		int owner = NetPacker.ReadByte(reader);
		DoExplode(loc, owner, damage, range, net: true);
	}

	public void WriteExplodes(PacketWriter writer)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < netExplode.Length; i++)
		{
			if (netExplode[i].exists)
			{
				if (netExplode[i].splash > 250f)
				{
					NetPacker.WriteMsg(writer, 10);
					NetPacker.WriteVec2(writer, netExplode[i].loc);
					((BinaryWriter)(object)writer).Write(NetPacker.FloatToInt16(netExplode[i].damage));
					((BinaryWriter)(object)writer).Write(NetPacker.FloatToInt16(netExplode[i].splash));
					NetPacker.WriteByte(writer, Game1.netSession.GetPlayerOne());
				}
				else
				{
					NetPacker.WriteMsg(writer, 5);
					NetPacker.WriteVec2(writer, netExplode[i].loc);
					NetPacker.WriteByte(writer, netExplode[i].damage);
					NetPacker.WriteByte(writer, (int)netExplode[i].splash);
					NetPacker.WriteByte(writer, Game1.netSession.GetPlayerOne());
				}
			}
		}
	}

	public void CleanupNetExplodes()
	{
		for (int i = 0; i < netExplode.Length; i++)
		{
			if (netExplode[i].exists)
			{
				netExplode[i].exists = false;
			}
		}
	}

	private void DoExplode(Vector2 loc, int owner, int damage, float range, bool net)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		if (!net)
		{
			Game1.pMan.AddParticle(28, loc, default(Vector2), range, damage, owner);
		}
		Game1.pMan.AddParticle(1, loc, default(Vector2), 4f, 0, 0);
		for (int i = 0; i < 4; i++)
		{
			Game1.pMan.AddParticle(3, loc, Rand.GetRandomVec2(-1000f, 1000f, -1000f, 1000f), 0f, 0, 0);
		}
		for (int j = 0; j < 32; j++)
		{
			Game1.pMan.AddParticle(58, loc, Rand.GetRandomVec2(Rand.GetRandomFloat(500f, 1500f)), 0f, 0, 0);
		}
		Vector2 val = loc - Scroll.scroll;
		if (((Vector2)(ref val)).LengthSquared() < 640000f)
		{
			if (range > 500f)
			{
				Sound.PlayCue("nukesplode");
			}
			else
			{
				Sound.PlayCue("explode");
			}
			Vector2 val2 = loc - Scroll.scroll;
			Quake.SetQuake((800f - ((Vector2)(ref val2)).Length()) / 800f);
		}
		if (range > 500f)
		{
			for (int k = 0; k < 12; k++)
			{
				float randomFloat = Rand.GetRandomFloat(-100f, 100f);
				Game1.pMan.AddParticle(66, loc + Rand.GetRandomVec2(randomFloat / 4f, randomFloat / 4f, -100f, 60f), Rand.GetRandomVec2(randomFloat, randomFloat, -250f, -100f), Rand.GetRandomFloat(5f, 7f), 0, -1);
			}
		}
	}

	internal void AddChrono(Vector2 vector2)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		chronoMgr.AddChrono(vector2);
	}

	internal void ResetChronos()
	{
		chronoMgr.ResetChronos();
	}

	internal bool GetChronod(Vector2 loc)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return chronoMgr.GetChronod(loc);
	}

	internal void Bigsplode(PacketReader reader)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 loc = NetPacker.ReadVec2(reader);
		int damage = ((BinaryReader)(object)reader).ReadInt16();
		float range = ((BinaryReader)(object)reader).ReadInt16();
		int owner = NetPacker.ReadByte(reader);
		DoExplode(loc, owner, damage, range, net: true);
	}
}
