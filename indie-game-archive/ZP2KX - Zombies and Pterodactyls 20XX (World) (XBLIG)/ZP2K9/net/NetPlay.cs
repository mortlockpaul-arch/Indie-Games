using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Yuki_Win.netplay;
using ZP2K9.characters;
using ZP2K9.characters.weapons;
using ZP2K9.hud.messageHud;
using ZP2K9.particles;

namespace ZP2K9.net;

public class NetPlay
{
	public const byte MSG_CHARACTER = 0;

	public const byte MSG_END = 1;

	public const byte MSG_INIT = 2;

	public const byte MSG_SERVER_STAT = 3;

	public const byte MSG_PARTICLE = 4;

	public const byte MSG_EXPLODE = 5;

	public const byte MSG_KILL = 6;

	public const byte MSG_SHORT_CHARACTER = 7;

	public const byte MSG_HITBY = 8;

	public const byte MSG_KICK = 9;

	public const byte MSG_BIGSPLODE = 10;

	public const short okKey = 1337;

	private const float STATUS_UPDATE_THRESHOLD = 0.8f;

	private float writeFrame;

	private NetBool alphaBool;

	private NetBool betaBool;

	private NetBool gammaBool;

	public int ID;

	public bool needsInit;

	public PacketWriter writer;

	public PacketReader reader;

	public int currentMap;

	public int currentMapListIdx;

	public int shortWrite;

	public BandwidthManager bandwidthManager;

	public NetPlay()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		base._002Ector();
		ID = -1;
		writer = new PacketWriter();
		reader = new PacketReader();
		bandwidthManager = new BandwidthManager();
		alphaBool = new NetBool();
		betaBool = new NetBool();
		gammaBool = new NetBool();
	}

	public void DrawHud(SpriteBatch sprite)
	{
	}

	internal void Update(NetworkSession netSession, Character[] c)
	{
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cc: Unknown result type (might be due to invalid IL or missing references)
		LocalNetworkGamer val = ((ReadOnlyCollection<LocalNetworkGamer>)(object)netSession.LocalGamers)[0];
		for (int i = 0; i < c.Length; i++)
		{
			if (c[i] != null && c[i].deltaSinceUpdate > 10f)
			{
				c[i] = null;
			}
		}
		int iD = ID;
		int num = 0;
		int num2 = 0;
		NetworkGamer val2 = default(NetworkGamer);
		while (val.IsDataAvailable)
		{
			val.ReceiveData(reader, ref val2);
			num += reader.Length;
			byte b = 0;
			byte b2 = 0;
			byte b3 = 0;
			byte b4 = byte.MaxValue;
			if (Game1.netSession.playerList.ContainsKey(val2.Id))
			{
				bandwidthManager.charHistory[Game1.netSession.playerList[val2.Id]].AddRead(reader.Length);
			}
			try
			{
				bool flag = false;
				while (!flag)
				{
					byte b5 = ((BinaryReader)(object)reader).ReadByte();
					if (((BinaryReader)(object)reader).ReadInt16() != 1337)
					{
						Console.WriteLine("Fatal packet error! " + b5 + ": " + b + ": " + ((b == 4) ? (b2 + "/" + b3) : "") + " pm: " + b4);
						while (reader.Position < reader.Length)
						{
							((BinaryReader)(object)reader).ReadBoolean();
						}
						flag = true;
						break;
					}
					b4 = b;
					switch (b5)
					{
					case 2:
						if (needsInit)
						{
							needsInit = false;
							ID = NetPacker.ReadByte(reader);
							c[ID] = new Character(ID, 0, default(Vector2));
							c[ID].SetNewClass();
							c[ID].Reset();
							currentMap = NetPacker.ReadByte(reader);
							GameState.gameType = NetPacker.ReadByte(reader);
							if (netSession.IsHost)
							{
								TryRebootBot();
							}
							else
							{
								try
								{
									Game1.gameMap.Read(new BinaryReader(File.Open("map/data/" + MapList.mapCatalog[currentMap].path + ".zkx", FileMode.Open, FileAccess.Read)));
									Game1.nodeMgr.Refresh(Game1.gameMap);
								}
								catch (Exception ex)
								{
									Console.WriteLine(ex.ToString());
								}
							}
							if (!Game1.netSession.playerList.ContainsKey(((NetworkGamer)val).Id))
							{
								Game1.netSession.playerList.Add(((NetworkGamer)val).Id, ID);
							}
							Game1.gameMap.GetSpawn(0, Game1.character[ID]);
							Game1.netSession.ResetGameStats();
						}
						else
						{
							((BinaryReader)(object)reader).ReadByte();
							((BinaryReader)(object)reader).ReadByte();
						}
						break;
					case 9:
						NetPacker.ReadByte(reader);
						GameState.mode = 2;
						Game1.netSession.Kill();
						Game1.menu.Close();
						Game1.menu.DoError("Kicked!", (Game1.netSession.netType == 2) ? 5 : 6);
						break;
					case 8:
						ReadHitBy(reader);
						break;
					case 6:
						KillManager.ReadKill(reader);
						break;
					case 0:
						ReadCharacter(c, reader, val2);
						break;
					case 7:
						ReadShortCharacter(c, reader, val2);
						break;
					case 5:
						Game1.pMan.Explode(reader);
						break;
					case 10:
						Game1.pMan.Bigsplode(reader);
						break;
					case 4:
					{
						byte b6 = ((BinaryReader)(object)reader).ReadByte();
						b3 = b2;
						b2 = b6;
						switch (b6)
						{
						case 0:
							Game1.pMan.AddParticle(8, reader);
							break;
						case 1:
							Game1.pMan.AddParticle(9, reader);
							break;
						case 3:
							Game1.pMan.AddParticle(11, reader);
							break;
						case 2:
							Game1.pMan.AddParticle(10, reader);
							break;
						case 4:
							Game1.pMan.AddParticle(12, reader);
							break;
						case 5:
							Game1.pMan.AddParticle(13, reader);
							break;
						case 6:
							Game1.pMan.AddParticle(14, reader);
							break;
						case 34:
							Game1.pMan.AddParticle(62, reader);
							break;
						case 32:
							Game1.pMan.AddParticle(60, reader);
							break;
						case 33:
							Game1.pMan.AddParticle(61, reader);
							break;
						case 31:
							Game1.pMan.AddParticle(59, reader);
							break;
						case 7:
							Game1.pMan.AddParticle(19, reader);
							break;
						case 8:
							Game1.pMan.AddParticle(20, reader);
							break;
						case 25:
							Game1.pMan.AddParticle(44, reader);
							break;
						case 9:
							Game1.pMan.AddParticle(21, reader);
							break;
						case 14:
							Game1.pMan.AddParticle(15, reader);
							break;
						case 26:
							Game1.pMan.AddParticle(45, reader);
							break;
						case 10:
							Game1.pMan.AddParticle(23, reader);
							break;
						case 15:
							Game1.pMan.AddParticle(4, reader);
							break;
						case 11:
							Game1.pMan.AddParticle(25, reader);
							break;
						case 12:
							Game1.pMan.AddParticle(27, reader);
							break;
						case 13:
							Game1.pMan.AddParticle(28, reader);
							break;
						case 27:
							Game1.pMan.AddParticle(47, reader);
							break;
						case 16:
							Game1.pMan.AddParticle(29, reader);
							break;
						case 17:
							Game1.pMan.AddParticle(37, reader);
							break;
						case 18:
							Game1.pMan.AddParticle(31, reader);
							break;
						case 19:
							Game1.pMan.AddParticle(35, reader);
							break;
						case 20:
							Game1.pMan.AddParticle(33, reader);
							break;
						case 21:
							Game1.pMan.AddParticle(34, reader);
							break;
						case 22:
							Game1.pMan.AddParticle(39, reader);
							break;
						case 23:
							Game1.pMan.AddParticle(40, reader);
							break;
						case 24:
							Game1.pMan.AddParticle(43, reader);
							break;
						case 29:
							Game1.pMan.AddParticle(54, reader);
							break;
						case 30:
							Game1.pMan.AddParticle(55, reader);
							break;
						case 28:
							Game1.pMan.AddParticle(52, reader);
							break;
						case 36:
							Game1.pMan.AddParticle(64, reader);
							break;
						case 37:
							Game1.pMan.AddParticle(65, reader);
							break;
						case 38:
							Game1.pMan.AddParticle(67, reader);
							break;
						case 44:
							Console.WriteLine("Livin' a lie.");
							break;
						default:
							Console.WriteLine("Fatal particle error: " + b6 + " not found.");
							break;
						}
						break;
					}
					case 3:
						ReadServerState(reader);
						break;
					case 1:
						flag = true;
						break;
					}
					b = b5;
				}
				if (Game1.netSession.netSession.IsHost)
				{
					ID = 0;
				}
			}
			catch (Exception ex2)
			{
				Console.WriteLine(ex2.ToString());
				while (reader.Position < reader.Length)
				{
					((BinaryReader)(object)reader).ReadBoolean();
				}
			}
		}
		if (Game1.netSession.netSession.IsHost)
		{
			ID = 0;
		}
		if (ID >= c.Length)
		{
			Console.WriteLine("Error, bad ID: " + ID);
			ID = iD;
		}
		if (iD > -1 && ID != iD)
		{
			Console.WriteLine("ID Change error: + " + ID);
			ID = iD;
		}
		if (ID <= -1)
		{
			return;
		}
		if (writeFrame <= 0f)
		{
			shortWrite = (shortWrite + 1) % 3;
			int num3 = 0;
			for (int j = 0; j < ((ReadOnlyCollection<NetworkGamer>)(object)netSession.RemoteGamers).Count; j++)
			{
				NetworkGamer val3 = ((ReadOnlyCollection<NetworkGamer>)(object)netSession.RemoteGamers)[j];
				byte id = val3.Id;
				int num4 = -1;
				bool flag2 = false;
				if (Game1.netSession.playerList.ContainsKey(id))
				{
					flag2 = true;
					num4 = Game1.netSession.playerList[id];
					if (c[num4] == null)
					{
						flag2 = false;
					}
					else if (ParticleManager.CheckVisibleToPlayer(c[num4].loc, c[num4].traj, 0.1f, ID))
					{
						flag2 = false;
					}
				}
				bool flag3 = false;
				if (val3.IsHost && GameState.mode == 2 && (Game1.netSession.redFlagState == ID || Game1.netSession.blueFlagState == ID))
				{
					flag3 = true;
				}
				try
				{
					if (flag2 && !flag3)
					{
						if (shortWrite == 0)
						{
							WriteShortCharacter(c[ID], writer, bot: false);
						}
					}
					else
					{
						WriteCharacter(c[ID], writer, bot: false);
					}
					if (num4 > -1 && c[ID].hitByChar[num4])
					{
						c[ID].hitByChar[num4] = false;
						WriteHitBy(c[ID], writer);
					}
				}
				catch (Exception)
				{
					Console.WriteLine("Character write failed: " + ID);
				}
				for (int k = 0; k < c.Length; k++)
				{
					if (k == ID || c[k] == null || c[k].ai == null || !Game1.netSession.GetNetworkOwner(k))
					{
						continue;
					}
					num4 = Game1.netSession.playerList[id];
					if (c[num4] != null)
					{
						if (ParticleManager.CheckVisibleToPlayer(c[num4].loc, c[num4].traj, 0.1f, k))
						{
							WriteCharacter(c[k], writer, bot: true);
						}
						else if (shortWrite == 0)
						{
							WriteShortCharacter(c[k], writer, bot: true);
						}
						if (num4 > -1 && c[k].hitByChar[num4])
						{
							c[k].hitByChar[num4] = false;
							WriteHitBy(c[k], writer);
						}
					}
				}
				if (netSession.IsHost)
				{
					WriteServerState(writer);
				}
				if (num4 > -1)
				{
					Game1.pMan.WriteParticles(Game1.netSession.GetPlayerOne(), num4, writer);
				}
				Game1.pMan.WriteExplodes(writer);
				bool flag4 = false;
				if (KillManager.WriteKills(writer))
				{
					flag4 = true;
				}
				NetPacker.WriteMsg(writer, 1);
				num3 += writer.Length;
				num2 += writer.Length;
				if (Game1.netSession.playerList.ContainsKey(id))
				{
					bandwidthManager.charHistory[Game1.netSession.playerList[id]].AddWrite(writer.Length);
				}
				flag4 = false;
				val.SendData(writer, (SendDataOptions)(flag4 ? 1 : 2), val3);
			}
			if (ID > -1 && c[ID] != null)
			{
				c[ID].deltaSinceUpdate = 0f;
			}
			for (int l = 0; l < c.Length; l++)
			{
				if (l != ID && c[l] != null && c[l].ai != null && Game1.netSession.GetNetworkOwner(l))
				{
					c[l].deltaSinceUpdate = 0f;
				}
			}
			KillManager.CleanKills();
			Game1.pMan.NetWriteCleanup();
			Game1.pMan.CleanupNetExplodes();
			float num5 = 5000f;
			if (Game1.netSession.netType == 2)
			{
				num5 = 8000f;
			}
			writeFrame = (float)num3 / num5;
			if (writeFrame < 0.08f)
			{
				writeFrame = 0.08f;
			}
		}
		else
		{
			writeFrame -= Game1.frameTime;
		}
		bandwidthManager.UpdateSentReceived(netSession.BytesPerSecondSent, netSession.BytesPerSecondReceived, num2, num);
	}

	private void TryRebootBot()
	{
		if (Game1.netSession.rebootBot)
		{
			Game1.netSession.rebootBot = false;
			Game1.menu.menuLevel[9].active = true;
			Game1.menu.menuLevel[9].selected = 5;
			Game1.menu.menuLevel[9].item[5].selX = 1;
		}
	}

	private void WriteHitBy(Character c, PacketWriter writer)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		NetPacker.WriteMsg(writer, 8);
		NetPacker.WriteByte(writer, c.ID);
		NetPacker.WriteVec2(writer, c.hitVec);
		NetPacker.WriteVec2(writer, c.hitTraj);
		NetPacker.WriteByte(writer, c.hitType);
	}

	private void ReadHitBy(PacketReader reader)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		int num = NetPacker.ReadByte(reader);
		Vector2 loc = NetPacker.ReadVec2(reader);
		Vector2 traj = NetPacker.ReadVec2(reader);
		int type = NetPacker.ReadByte(reader);
		HitManager.DoWound(type, loc, traj, Game1.character[num]);
		Sound.PlayConfirm();
	}

	private void ReadShortCharacter(Character[] character, PacketReader reader, NetworkGamer sender)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		int num = NetPacker.ReadByte(reader);
		bool flag = ((BinaryReader)(object)reader).ReadBoolean();
		float num2 = NetPacker.ByteToTinyFloat(((BinaryReader)(object)reader).ReadByte());
		byte id = sender.Id;
		if (!flag)
		{
			if (!Game1.netSession.playerList.ContainsKey(id))
			{
				Game1.netSession.playerList.Add(id, num);
			}
			else if (Game1.netSession.playerList[id] != num)
			{
				Game1.netSession.playerList[id] = num;
			}
		}
		Character character2 = character[num];
		if (character[num] == null)
		{
			character[num] = new Character(num, -9, default(Vector2));
			character2 = character[num];
		}
		else
		{
			character[num].keySrc = -9;
			if (character[num].ai != null)
			{
				character[num].ai = null;
			}
			character2 = character[num];
		}
		float deltaSinceUpdate = character2.deltaSinceUpdate;
		num2 -= character2.deltaSinceUpdate;
		character2.deltaSinceUpdate = 0f;
		character2.metaWriteMode = NetPacker.ReadByte(reader);
		switch (character2.metaWriteMode)
		{
		case 0:
			character2.headTex = NetPacker.ReadByte(reader);
			character2.hatTex = NetPacker.ReadByte(reader);
			character2.torsoTex = NetPacker.ReadByte(reader);
			character2.legsTex = NetPacker.ReadByte(reader);
			break;
		case 1:
			character2.bodyType = NetPacker.ReadByte(reader);
			character2.skinTex = NetPacker.ReadByte(reader);
			character2.jetpack = NetPacker.ReadByte(reader);
			break;
		case 2:
			character2.perk[0] = NetPacker.ReadSByte(reader);
			character2.perk[1] = NetPacker.ReadSByte(reader);
			character2.perk[2] = NetPacker.ReadSByte(reader);
			character2.team = NetPacker.ReadByte(reader);
			break;
		case 3:
		{
			char c = Convert.ToChar(((BinaryReader)(object)reader).ReadInt16());
			char c2 = Convert.ToChar(((BinaryReader)(object)reader).ReadInt16());
			char c3 = Convert.ToChar(((BinaryReader)(object)reader).ReadInt16());
			if (character2.clanChar[0] != c || character2.clanChar[1] != c2 || character2.clanChar[2] != c3)
			{
				character2.clanChar[0] = c;
				character2.clanChar[1] = c2;
				character2.clanChar[2] = c3;
				character2.needsClantagUpdate = true;
			}
			break;
		}
		case 4:
		{
			int level = NetPacker.ReadByte(reader);
			character2.level = level;
			character2.score = ((BinaryReader)(object)reader).ReadInt16();
			break;
		}
		}
		character2.hp = NetPacker.ReadSByte(reader);
		if (character2.hp < 0)
		{
			KillChar(character2);
		}
		Vector2 loc = NetPacker.ReadVec2(reader);
		character2.loc = loc;
		Vector2 val = character2.loc - character2.lastNetLoc;
		character2.lastNetLoc = character2.loc;
		character2.radarTraj = val / deltaSinceUpdate;
		character2.recentShortUpdate = true;
		if (Rand.CointToss(0.5f))
		{
			character2.charKeys.keyRight = true;
		}
		else
		{
			character2.charKeys.keyLeft = true;
		}
	}

	private void WriteShortCharacter(Character c, PacketWriter writer, bool bot)
	{
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		float deltaSinceUpdate = c.deltaSinceUpdate;
		c.metaWriteMode = (c.metaWriteMode + 1) % 5;
		NetPacker.WriteMsg(writer, 7);
		NetPacker.WriteByte(writer, c.ID);
		((BinaryWriter)(object)writer).Write(bot);
		((BinaryWriter)(object)writer).Write(NetPacker.TinyFloatToByte(deltaSinceUpdate));
		NetPacker.WriteByte(writer, c.metaWriteMode);
		switch (c.metaWriteMode)
		{
		case 0:
			NetPacker.WriteByte(writer, c.headTex);
			NetPacker.WriteByte(writer, c.hatTex);
			NetPacker.WriteByte(writer, c.torsoTex);
			NetPacker.WriteByte(writer, c.legsTex);
			break;
		case 1:
			NetPacker.WriteByte(writer, c.bodyType);
			NetPacker.WriteByte(writer, c.skinTex);
			NetPacker.WriteByte(writer, c.jetpack);
			break;
		case 2:
			NetPacker.WriteSByte(writer, c.perk[0]);
			NetPacker.WriteSByte(writer, c.perk[1]);
			NetPacker.WriteSByte(writer, c.perk[2]);
			NetPacker.WriteByte(writer, c.team);
			break;
		case 3:
			((BinaryWriter)(object)writer).Write(Convert.ToInt16(c.clanChar[0]));
			((BinaryWriter)(object)writer).Write(Convert.ToInt16(c.clanChar[1]));
			((BinaryWriter)(object)writer).Write(Convert.ToInt16(c.clanChar[2]));
			break;
		case 4:
			if (Game1.netSession.GetPlayerOne() == c.ID)
			{
				NetPacker.WriteByte(writer, Game1.zProfile.level);
			}
			else
			{
				NetPacker.WriteByte(writer, 0);
			}
			((BinaryWriter)(object)writer).Write((short)c.score);
			break;
		}
		NetPacker.WriteSByte(writer, c.hp);
		NetPacker.WriteVec2(writer, c.loc);
	}

	private void KillChar(Character c)
	{
		if (Game1.netSession.IsHost())
		{
			if (Game1.netSession.redFlagState == c.ID || Game1.netSession.blueFlagState == c.ID)
			{
				Game1.hud.AddMessage(KillManager.GetPlayerName(c.ID), Message.msgDroppedFlag, c.GetTeam(), 0, -1);
			}
			if (Game1.netSession.redFlagState == c.ID)
			{
				Game1.netSession.redFlagState = 200;
			}
			if (Game1.netSession.blueFlagState == c.ID)
			{
				Game1.netSession.blueFlagState = 200;
			}
		}
	}

	private void ReadCharacter(Character[] character, PacketReader reader, NetworkGamer sender)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		int num = NetPacker.ReadByte(reader);
		bool flag = ((BinaryReader)(object)reader).ReadBoolean();
		float num2 = NetPacker.ByteToTinyFloat(((BinaryReader)(object)reader).ReadByte());
		byte id = sender.Id;
		if (!flag)
		{
			if (!Game1.netSession.playerList.ContainsKey(id))
			{
				Game1.netSession.playerList.Add(id, num);
			}
			else if (Game1.netSession.playerList[id] != num)
			{
				Game1.netSession.playerList[id] = num;
			}
		}
		Character character2 = character[num];
		if (character[num] == null)
		{
			character[num] = new Character(num, -9, default(Vector2));
			character2 = character[num];
		}
		else
		{
			character[num].keySrc = -9;
			if (character[num].ai != null)
			{
				character[num].ai = null;
			}
		}
		num2 -= character2.deltaSinceUpdate;
		character2.deltaSinceUpdate = 0f;
		Vector2 val = NetPacker.ReadVec2(reader);
		if (character2 == null)
		{
			character2 = new Character(num, -9, val);
		}
		Vector2 val2 = NetPacker.ReadVec2(reader);
		_ = character2.state;
		character2.state = NetPacker.ReadByte(reader);
		character2.loc = val;
		character2.goalLoc = val + (float)sender.RoundtripTime.TotalSeconds * val2 * 0.5f;
		character2.traj = val2;
		character2.radarTraj = character2.traj;
		character2.latency = (float)sender.RoundtripTime.TotalSeconds * 0.5f;
		alphaBool.Read(reader);
		betaBool.Read(reader);
		gammaBool.Read(reader);
		character2.gibbed = alphaBool.val[0];
		if (alphaBool.val[1])
		{
			character2.freeze = 1.5f;
		}
		else if (character2.freeze > 0.8f)
		{
			character2.freeze = 0.8f;
		}
		if (alphaBool.val[2])
		{
			character2.fire = 1.5f;
		}
		else if (character2.fire > 0.8f)
		{
			character2.fire = 0.8f;
		}
		if (alphaBool.val[3])
		{
			character2.poison = 1.5f;
		}
		else if (character2.poison > 0.8f)
		{
			character2.poison = 0.8f;
		}
		if (alphaBool.val[4])
		{
			character2.shrink = 1.5f;
		}
		else if (character2.shrink > 0.8f)
		{
			character2.shrink = 0.8f;
		}
		if (gammaBool.val[0])
		{
			character2.rainbowed = 1.5f;
		}
		else if (character2.rainbowed > 0.8f)
		{
			character2.rainbowed = 0.8f;
		}
		character2.splitAnim = alphaBool.val[5];
		character2.face = (alphaBool.val[6] ? 1 : 0);
		character2.recentShortUpdate = false;
		for (int i = 0; i < 2; i++)
		{
			character2.bodySec[i].anim = NetPacker.ReadByte(reader);
			character2.bodySec[i].key = NetPacker.ReadByte(reader);
			character2.bodySec[i].SetAnimNameFromInt(character2);
			character2.bodySec[i].curFrame = NetPacker.ByteToSmallFloat(((BinaryReader)(object)reader).ReadByte());
		}
		character2.metaWriteMode = NetPacker.ReadByte(reader);
		switch (character2.metaWriteMode)
		{
		case 0:
			character2.headTex = NetPacker.ReadByte(reader);
			character2.hatTex = NetPacker.ReadByte(reader);
			character2.torsoTex = NetPacker.ReadByte(reader);
			character2.legsTex = NetPacker.ReadByte(reader);
			break;
		case 1:
			character2.bodyType = NetPacker.ReadByte(reader);
			character2.skinTex = NetPacker.ReadByte(reader);
			character2.jetpack = NetPacker.ReadByte(reader);
			break;
		case 2:
			character2.perk[0] = NetPacker.ReadSByte(reader);
			character2.perk[1] = NetPacker.ReadSByte(reader);
			character2.perk[2] = NetPacker.ReadSByte(reader);
			character2.team = NetPacker.ReadByte(reader);
			break;
		case 3:
		{
			char c = Convert.ToChar(((BinaryReader)(object)reader).ReadInt16());
			char c2 = Convert.ToChar(((BinaryReader)(object)reader).ReadInt16());
			char c3 = Convert.ToChar(((BinaryReader)(object)reader).ReadInt16());
			if (character2.clanChar[0] != c || character2.clanChar[1] != c2 || character2.clanChar[2] != c3)
			{
				character2.clanChar[0] = c;
				character2.clanChar[1] = c2;
				character2.clanChar[2] = c3;
				character2.needsClantagUpdate = true;
			}
			break;
		}
		case 4:
		{
			int level = NetPacker.ReadByte(reader);
			character2.level = level;
			character2.score = ((BinaryReader)(object)reader).ReadInt16();
			break;
		}
		}
		character2.weapon[0] = NetPacker.ReadByte(reader);
		character2.curWeap = 0;
		character2.angle = (NetPacker.ReadRadian(reader) - 3.14f) * 2f;
		character2.charKeys.ClearKeys();
		character2.suit = NetPacker.ReadByte(reader);
		character2.charKeys.keyRight = alphaBool.val[7];
		character2.charKeys.keyLeft = betaBool.val[0];
		character2.charKeys.keyUp = betaBool.val[1];
		character2.charKeys.keyDown = betaBool.val[2];
		character2.netJetpack = betaBool.val[3];
		character2.charKeys.SetKeyPickup();
		if (character2.charKeys.keyJetpack)
		{
			character2.jetGas = 1f;
			character2.jetRecover = 0f;
		}
		bool flag2 = betaBool.val[4];
		bool flag3 = betaBool.val[5];
		bool flag4 = betaBool.val[6];
		bool flag5 = betaBool.val[7];
		if (flag2)
		{
			character2.charKeys.shootVec = NetPacker.ReadNormalizedVec2(reader);
			character2.ammo[WeaponCatalog.weapons[character2.weapon[0]].ammoType] = 100;
			character2.magazine[0] = WeaponCatalog.weapons[character2.weapon[0]].maxClip;
		}
		if (flag3)
		{
			character2.charKeys.keyGrenade = true;
			character2.grenType[0] = NetPacker.ReadByte(reader);
			character2.grenAmmo[0] = 5;
		}
		if (flag5)
		{
			character2.spawnFrame = NetPacker.ByteToTinyFloat(((BinaryReader)(object)reader).ReadByte());
		}
		character2.hp = NetPacker.ReadSByte(reader);
		if (character2.dyingFrame > 0f && character2.hp >= 0)
		{
			character2.dyingFrame = 0f;
		}
		if (character2.hp < 0)
		{
			KillChar(character2);
		}
		if (flag4)
		{
			character2.rollFace = 1;
		}
		else
		{
			character2.rollFace = 0;
		}
		character2.charKeys.runVec = new Vector2(1f, 1f);
		character2.charKeys.runSpeed = 1f;
	}

	private void WriteCharacter(Character c, PacketWriter writer, bool bot)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		float deltaSinceUpdate = c.deltaSinceUpdate;
		c.metaWriteMode = (c.metaWriteMode + 1) % 5;
		NetPacker.WriteMsg(writer, 0);
		NetPacker.WriteByte(writer, c.ID);
		((BinaryWriter)(object)writer).Write(bot);
		((BinaryWriter)(object)writer).Write(NetPacker.TinyFloatToByte(deltaSinceUpdate));
		NetPacker.WriteVec2(writer, c.loc);
		NetPacker.WriteVec2(writer, c.traj);
		NetPacker.WriteByte(writer, c.state);
		alphaBool.val[0] = c.gibbed;
		alphaBool.val[1] = c.freeze > 0.8f;
		alphaBool.val[2] = c.fire > 0.8f;
		alphaBool.val[3] = c.poison > 0.8f;
		alphaBool.val[4] = c.shrink > 0.8f;
		alphaBool.val[5] = c.splitAnim;
		alphaBool.val[6] = c.face == 1;
		alphaBool.val[7] = c.charKeys.keyRight;
		betaBool.val[0] = c.charKeys.keyLeft;
		betaBool.val[1] = c.charKeys.keyUp;
		betaBool.val[2] = c.charKeys.keyDown;
		betaBool.val[3] = c.netJetpack;
		bool flag = ((Vector2)(ref c.charKeys.shootVec)).Length() > 0.6f && c.reloadFrame <= 0f;
		bool flag2 = (c.charKeys.keyGrenade && c.grenAmmo[0] > 0) || (c.charKeys.keyGren2 && c.grenAmmo[1] > 0);
		betaBool.val[4] = flag;
		betaBool.val[5] = flag2;
		betaBool.val[6] = c.rollFace == 1;
		bool flag3 = c.spawnFrame > 0f;
		betaBool.val[7] = flag3;
		gammaBool.val[0] = c.rainbowed > 0.8f;
		alphaBool.Write(writer);
		betaBool.Write(writer);
		gammaBool.Write(writer);
		for (int i = 0; i < 2; i++)
		{
			NetPacker.WriteByte(writer, c.bodySec[i].anim);
			NetPacker.WriteByte(writer, c.bodySec[i].key);
			((BinaryWriter)(object)writer).Write(NetPacker.SmallFloatToByte(c.bodySec[i].curFrame));
		}
		NetPacker.WriteByte(writer, c.metaWriteMode);
		switch (c.metaWriteMode)
		{
		case 0:
			NetPacker.WriteByte(writer, c.headTex);
			NetPacker.WriteByte(writer, c.hatTex);
			NetPacker.WriteByte(writer, c.torsoTex);
			NetPacker.WriteByte(writer, c.legsTex);
			break;
		case 1:
			NetPacker.WriteByte(writer, c.bodyType);
			NetPacker.WriteByte(writer, c.skinTex);
			NetPacker.WriteByte(writer, c.jetpack);
			break;
		case 2:
			NetPacker.WriteSByte(writer, c.perk[0]);
			NetPacker.WriteSByte(writer, c.perk[1]);
			NetPacker.WriteSByte(writer, c.perk[2]);
			NetPacker.WriteByte(writer, c.team);
			break;
		case 3:
			((BinaryWriter)(object)writer).Write(Convert.ToInt16(c.clanChar[0]));
			((BinaryWriter)(object)writer).Write(Convert.ToInt16(c.clanChar[1]));
			((BinaryWriter)(object)writer).Write(Convert.ToInt16(c.clanChar[2]));
			break;
		case 4:
			if (Game1.netSession.GetPlayerOne() == c.ID)
			{
				NetPacker.WriteByte(writer, Game1.zProfile.level);
			}
			else
			{
				NetPacker.WriteByte(writer, 0);
			}
			((BinaryWriter)(object)writer).Write((short)c.score);
			break;
		}
		NetPacker.WriteByte(writer, c.weapon[c.curWeap]);
		NetPacker.WriteRadian(writer, c.angle / 2f + 3.14f);
		NetPacker.WriteByte(writer, c.suit);
		if (flag)
		{
			NetPacker.WriteNormalizedVec2(writer, c.charKeys.shootVec);
		}
		if (flag2)
		{
			if (c.charKeys.keyGren2)
			{
				NetPacker.WriteByte(writer, c.grenType[1]);
			}
			else
			{
				NetPacker.WriteByte(writer, c.grenType[0]);
			}
		}
		if (flag3)
		{
			((BinaryWriter)(object)writer).Write(NetPacker.TinyFloatToByte(c.spawnFrame));
		}
		NetPacker.WriteSByte(writer, c.hp);
	}

	private void WriteServerState(PacketWriter writer)
	{
		NetPacker.WriteMsg(writer, 3);
		((BinaryWriter)(object)writer).Write(GameState.mode == 2);
		NetPacker.WriteByte(writer, MapList.maplist[currentMapListIdx]);
		((BinaryWriter)(object)writer).Write(Game1.netSession.postLobby);
		NetPacker.WriteByte(writer, GameState.gameType);
		NetPacker.WriteByte(writer, Game1.netSession.mutator);
		((BinaryWriter)(object)writer).Write(Game1.menu.menuLevel[15].active);
		switch (GameState.gameType)
		{
		case 2:
			NetPacker.WriteByte(writer, Game1.netSession.redFlagState);
			NetPacker.WriteByte(writer, Game1.netSession.blueFlagState);
			NetPacker.WriteByte(writer, Game1.netSession.redScore);
			NetPacker.WriteByte(writer, Game1.netSession.blueScore);
			break;
		case 3:
			((BinaryWriter)(object)writer).Write((short)Game1.netSession.redTime);
			((BinaryWriter)(object)writer).Write((short)Game1.netSession.blueTime);
			break;
		case 1:
		case 4:
			((BinaryWriter)(object)writer).Write((short)Game1.netSession.redScore);
			((BinaryWriter)(object)writer).Write((short)Game1.netSession.blueScore);
			break;
		}
		switch (GameState.gameType)
		{
		case 2:
			NetPacker.WriteByte(writer, Game1.netSession.CTFScoreIdx);
			break;
		case 3:
			NetPacker.WriteByte(writer, Game1.netSession.KOTHScoreIdx);
			break;
		case 4:
			NetPacker.WriteByte(writer, Game1.netSession.ZHScoreIdx);
			break;
		case 1:
			NetPacker.WriteByte(writer, Game1.netSession.TDMScoreIdx);
			break;
		case 0:
			NetPacker.WriteByte(writer, Game1.netSession.DMScoreIdx);
			break;
		}
	}

	private void ReadServerState(PacketReader reader)
	{
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		if (((BinaryReader)(object)reader).ReadBoolean())
		{
			GameState.mode = 2;
		}
		else if (GameState.mode != 1)
		{
			GameState.mode = 1;
			Game1.menu.Close();
		}
		int num = currentMap;
		currentMap = NetPacker.ReadByte(reader);
		bool postLobby = Game1.netSession.postLobby;
		Game1.netSession.postLobby = ((BinaryReader)(object)reader).ReadBoolean();
		if (currentMap != num || (!Game1.netSession.postLobby && postLobby))
		{
			try
			{
				Game1.store.Write(0);
				Game1.gameMap.Read(new BinaryReader(File.Open("map/data/" + MapList.mapCatalog[currentMap].path + ".zkx", FileMode.Open, FileAccess.Read)));
				Game1.nodeMgr.Refresh(Game1.gameMap);
				if (!needsInit)
				{
					int bodyType = Game1.character[ID].bodyType;
					int skinTex = Game1.character[ID].skinTex;
					int headTex = Game1.character[ID].headTex;
					int hatTex = Game1.character[ID].hatTex;
					int torsoTex = Game1.character[ID].torsoTex;
					int legsTex = Game1.character[ID].legsTex;
					int team = Game1.character[ID].team;
					int jetpack = Game1.character[ID].jetpack;
					Game1.character[ID] = new Character(ID, 0, default(Vector2));
					Game1.character[ID].headTex = headTex;
					Game1.character[ID].hatTex = hatTex;
					Game1.character[ID].torsoTex = torsoTex;
					Game1.character[ID].legsTex = legsTex;
					Game1.character[ID].skinTex = skinTex;
					Game1.character[ID].bodyType = bodyType;
					Game1.character[ID].team = team;
					Game1.character[ID].jetpack = jetpack;
					for (int i = 0; i < 3; i++)
					{
						Game1.character[ID].perk[i] = Game1.zProfile.ClassSet().perk[i];
					}
					Game1.character[ID].Reset();
					Game1.gameMap.GetSpawn(0, Game1.character[ID]);
				}
				Game1.netSession.ResetGameStats();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
		if (Game1.netSession.postLobby && !postLobby)
		{
			Music.Reset();
		}
		int gameType = GameState.gameType;
		GameState.gameType = NetPacker.ReadByte(reader);
		int mutator = Game1.netSession.mutator;
		Game1.netSession.mutator = NetPacker.ReadByte(reader);
		if (Game1.netSession.mutator != mutator || gameType != GameState.gameType)
		{
			Game1.netSession.ChangeMutator();
		}
		if (((BinaryReader)(object)reader).ReadBoolean())
		{
			Game1.hud.SetServerChangingSettings();
		}
		switch (GameState.gameType)
		{
		case 2:
			Game1.netSession.redFlagState = NetPacker.ReadByte(reader);
			Game1.netSession.blueFlagState = NetPacker.ReadByte(reader);
			Game1.netSession.redScore = NetPacker.ReadByte(reader);
			Game1.netSession.blueScore = NetPacker.ReadByte(reader);
			break;
		case 3:
			Game1.netSession.redTime = ((BinaryReader)(object)reader).ReadInt16();
			Game1.netSession.blueTime = ((BinaryReader)(object)reader).ReadInt16();
			break;
		case 1:
		case 4:
			Game1.netSession.redScore = ((BinaryReader)(object)reader).ReadInt16();
			Game1.netSession.blueScore = ((BinaryReader)(object)reader).ReadInt16();
			break;
		}
		switch (GameState.gameType)
		{
		case 2:
			Game1.netSession.CTFScoreIdx = NetPacker.ReadByte(reader);
			break;
		case 3:
			Game1.netSession.KOTHScoreIdx = NetPacker.ReadByte(reader);
			break;
		case 4:
			Game1.netSession.ZHScoreIdx = NetPacker.ReadByte(reader);
			break;
		case 1:
			Game1.netSession.TDMScoreIdx = NetPacker.ReadByte(reader);
			break;
		case 0:
			Game1.netSession.DMScoreIdx = NetPacker.ReadByte(reader);
			break;
		}
		for (int j = 0; j < 2; j++)
		{
			int num2 = ((j == 0) ? Game1.netSession.pRedFlagState : Game1.netSession.pBlueFlagState);
			int num3 = ((j == 0) ? Game1.netSession.redFlagState : Game1.netSession.blueFlagState);
			int num4 = -1;
			int num5 = -1;
			int num6 = -1;
			if (num2 != num3)
			{
				if (num2 != 200)
				{
					if (num2 >= 0 && num2 < Game1.character.Length && Game1.character[num2] != null)
					{
						if (Game1.character[num2].hp >= 0)
						{
							num5 = num2;
						}
						else
						{
							num4 = num2;
							if (num3 != 200)
							{
								num6 = num3;
							}
						}
					}
				}
				else
				{
					num6 = num3;
				}
			}
			if (num4 > -1 && num4 < Game1.character.Length && Game1.character[num4] != null)
			{
				Game1.hud.AddMessage(KillManager.GetPlayerName(num4), Message.msgDroppedFlag, Game1.character[num4].GetTeam(), 0, -1);
			}
			if (num6 > -1 && num6 < Game1.character.Length && Game1.character[num6] != null)
			{
				Game1.hud.AddMessage(KillManager.GetPlayerName(num6), Message.msgGotFlag, Game1.character[num6].GetTeam(), 0, -1);
			}
			if (num5 > -1 && num5 < Game1.character.Length && Game1.character[num5] != null)
			{
				Game1.hud.AddMessage(KillManager.GetPlayerName(num5), Message.msgCappedFlag, Game1.character[num5].GetTeam(), 0, -1);
			}
		}
		Game1.netSession.pRedFlagState = Game1.netSession.redFlagState;
		Game1.netSession.pBlueFlagState = Game1.netSession.blueFlagState;
	}
}
