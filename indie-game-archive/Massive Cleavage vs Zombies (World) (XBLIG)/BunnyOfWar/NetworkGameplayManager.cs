using System;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace BunnyOfWar;

public static class NetworkGameplayManager
{
	public enum PacketType
	{
		HumanFighterPosition,
		HumanFighterPositionJumping,
		ObjectDamaged,
		AnimationChange,
		HumanHealth,
		ComputerHealthChange,
		RangedAttack,
		AddProjectile,
		Pause,
		TriggerTriggered,
		AssignRandomSeed,
		AssignPlayerID,
		PlayerReadyChange,
		SelectedALevel,
		SelectedAPvPLevel,
		SelectedABonusLevel,
		WorldMapPosition,
		Quit,
		ScreenManagerChange,
		FighterDeath,
		FighterStats,
		FighterStunned
	}

	public static PlayerIndex localPlayerIndex = PlayerIndex.Four;

	public static string localGamerTag = "";

	private static PacketWriter packetWriter = new PacketWriter();

	private static PacketReader packetReader = new PacketReader();

	private static int updatesSinceWorldDataSend = 0;

	private static int updatesSinceStatusPacket = 0;

	private static NetworkSession networkSession => Networking.networkSession;

	public static void Load()
	{
	}

	public static void SetAndSendPlayerIDs()
	{
		for (int i = 0; i < FighterManager.humanPlayers.Count; i++)
		{
			FighterManager.humanPlayers[i].ID = i;
			SendNetworkPlayerID(FighterManager.humanPlayers[i].PROPERTIES.GamerTag, (byte)FighterManager.humanPlayers[i].ID);
		}
	}

	public static void SetAndSendRandomSeed()
	{
		int millisecond = DateTime.Now.Millisecond;
		RandomStaticGlobals.RandomAI = new Random(millisecond);
		if (Networking.NullCheckSucceed())
		{
			Networking.packetWriter.Write((byte)10);
			Networking.packetWriter.Write(millisecond);
			Networking.networkSession.LocalGamers[0].SendData(Networking.packetWriter, SendDataOptions.ReliableInOrder);
		}
	}

	public static void SendNetworkPlayerID(string gamerTag, byte ID)
	{
		if (Networking.NullCheckSucceed())
		{
			Networking.packetWriter.Write((byte)11);
			Networking.packetWriter.Write(ID);
			Networking.packetWriter.Write(gamerTag.Length);
			Networking.packetWriter.Write(gamerTag.ToCharArray());
			Networking.networkSession.LocalGamers[0].SendData(Networking.packetWriter, SendDataOptions.ReliableInOrder);
		}
	}

	private static void ReadNetworkPlayerID(string gamerTag, byte ID)
	{
		while (ID >= FighterManager.humanPlayers.Count)
		{
			FighterManager.addNewHumanPlayer(null, isNetworkPlayer: false, "", 1f);
		}
		FighterManager.humanPlayers[ID].PROPERTIES.GamerTag = gamerTag;
		FighterManager.humanPlayers[ID].ID = ID;
		foreach (SignedInGamer signedInGamer in Gamer.SignedInGamers)
		{
			if (signedInGamer.Gamertag.ToLower().Trim() == gamerTag.ToLower().Trim())
			{
				FighterManager.humanPlayers[ID].PROPERTIES.PlayerIndexControllerNumber = signedInGamer.PlayerIndex;
				FighterManager.localXboxPlayerID = ID;
				FighterManager.humanPlayers[ID].PROPERTIES.isLocal = true;
			}
			else if (!Networking.isHost)
			{
				FighterManager.humanPlayers[ID].PROPERTIES.isLocal = false;
			}
		}
	}

	public static void SendPlayerStats()
	{
		if (!Networking.NullCheckSucceed())
		{
			return;
		}
		foreach (FighterObject humanPlayer in FighterManager.humanPlayers)
		{
			if (humanPlayer.PROPERTIES.isLocal && humanPlayer.ID >= 0)
			{
				byte value = (byte)humanPlayer.ID;
				HumanProfileObject humanProfile = humanPlayer.PROPERTIES.HumanProfile;
				packetWriter.Write((byte)20);
				packetWriter.Write(value);
				packetWriter.Write((ushort)humanProfile.blocks);
				packetWriter.Write((ushort)humanProfile.counters);
				packetWriter.Write((ushort)humanProfile.damageDealt);
				packetWriter.Write((ushort)humanProfile.damageTaken);
				packetWriter.Write((ushort)humanProfile.deaths);
				packetWriter.Write((ushort)humanProfile.kills);
				packetWriter.Write((ushort)humanProfile.parries);
				packetWriter.Write((ushort)humanProfile.shotsFired);
				packetWriter.Write((ushort)humanProfile.shotsMade);
				packetWriter.Write((ushort)humanProfile.shotsBlocked);
				packetWriter.Write((ulong)humanProfile.timeSpentBlocking);
				packetWriter.Write((ulong)humanProfile.timeSpentPlaying);
				networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
			}
		}
	}

	public static void ReadPlayerStats(PacketReader pr)
	{
		try
		{
			byte index = pr.ReadByte();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.blocks = pr.ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.counters = pr.ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.damageDealt = pr.ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.damageTaken = pr.ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.deaths = pr.ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.kills = pr.ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.parries = pr.ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.shotsFired = pr.ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.shotsMade = pr.ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.shotsBlocked = pr.ReadUInt16();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.timeSpentBlocking = pr.ReadUInt64();
			FighterManager.humanPlayers[index].PROPERTIES.HumanProfile.timeSpentPlaying = pr.ReadUInt64();
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
	}

	public static void SendFighterPosition(int id, int x, int y)
	{
		if (Networking.NullCheckSucceed())
		{
			packetWriter.Write((byte)0);
			packetWriter.Write((byte)id);
			packetWriter.Write((ushort)x);
			packetWriter.Write((ushort)y);
			networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.None);
		}
	}

	public static void ReadFighterPosition(PacketReader pr)
	{
		try
		{
			int index = packetReader.ReadByte();
			int x = packetReader.ReadUInt16();
			int y = packetReader.ReadUInt16();
			FighterManager.humanPlayers[index].moveRemotely(x, y, null);
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
	}

	public static void SendPackets(PacketType pt, int? a, int? b)
	{
		if (Networking.NullCheckSucceed())
		{
			packetWriter.Write((byte)pt);
			if (a.HasValue)
			{
				packetWriter.Write(a.Value);
			}
			if (b.HasValue)
			{
				packetWriter.Write(b.Value);
			}
			networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
		}
	}

	public static void SendPauseState()
	{
		if (Networking.NullCheckSucceed())
		{
			packetWriter.Write((byte)8);
			packetWriter.Write(RandomStaticGlobals.isGamePaused);
			networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
		}
	}

	private static void ReadPauseState(PacketReader pr)
	{
		bool flag = pr.ReadBoolean();
		if (flag != RandomStaticGlobals.isGamePaused)
		{
			if (!flag)
			{
				ScreenManager.ShowBlank();
			}
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: false);
		}
	}

	public static void SendJumping(int playerID, int x, int y, int jumpHeight, bool areWeHuman)
	{
		if (Networking.NullCheckSucceed())
		{
			if (jumpHeight > 2000 || jumpHeight < 0)
			{
				jumpHeight = 0;
			}
			packetWriter.Write((byte)1);
			packetWriter.Write((ushort)playerID);
			packetWriter.Write((ushort)x);
			packetWriter.Write((ushort)y);
			packetWriter.Write((ushort)jumpHeight);
			packetWriter.Write(areWeHuman);
			networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.None);
		}
	}

	public static void ReadJumping(PacketReader pr)
	{
		int index = pr.ReadUInt16();
		int x = pr.ReadUInt16();
		int y = pr.ReadUInt16();
		int value = pr.ReadUInt16();
		if (pr.ReadBoolean())
		{
			FighterManager.humanPlayers[index].moveRemotely(x, y, value);
		}
		else
		{
			FighterManager.computerPlayers[index].moveRemotely(x, y, value);
		}
	}

	public static void SendObjectDamage(int objectID, int damageAmount)
	{
		if (Networking.NullCheckSucceed())
		{
			packetWriter.Write((byte)2);
			packetWriter.Write((ushort)objectID);
			packetWriter.Write((ushort)damageAmount);
			networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.Reliable);
		}
	}

	public static void ReadPackets()
	{
		if (!Networking.NullCheckSucceed())
		{
			return;
		}
		foreach (LocalNetworkGamer localGamer in Networking.LocalGamers)
		{
			while (localGamer.IsDataAvailable)
			{
				localGamer.ReceiveData(packetReader, out var sender);
				if (sender.IsLocal)
				{
					continue;
				}
				switch ((PacketType)packetReader.ReadByte())
				{
				case PacketType.Pause:
					ReadPauseState(packetReader);
					break;
				case PacketType.HumanFighterPosition:
					ReadFighterPosition(packetReader);
					break;
				case PacketType.HumanFighterPositionJumping:
					ReadJumping(packetReader);
					break;
				case PacketType.AnimationChange:
					FighterManager.ReadAnimationChange(packetReader);
					break;
				case PacketType.HumanHealth:
					FighterManager.ReadHumanHealth(packetReader);
					break;
				case PacketType.ComputerHealthChange:
					FighterManager.ReadComputerDamage(packetReader);
					break;
				case PacketType.RangedAttack:
					FighterManager.ReadRangedAttack(packetReader);
					break;
				case PacketType.AssignRandomSeed:
					RandomStaticGlobals.RandomAI = new Random(packetReader.ReadInt32());
					break;
				case PacketType.AssignPlayerID:
				{
					byte iD = packetReader.ReadByte();
					int count = packetReader.ReadInt32();
					string gamerTag = new string(packetReader.ReadChars(count));
					ReadNetworkPlayerID(gamerTag, iD);
					break;
				}
				case PacketType.WorldMapPosition:
					ScreenManager.SetWorldMapPosition(packetReader.ReadInt32(), packetReader.ReadInt32());
					break;
				case PacketType.SelectedALevel:
					LevelManager.LoadLevel(packetReader.ReadInt32());
					break;
				case PacketType.SelectedAPvPLevel:
					LevelManager.LoadPvPLevel(packetReader.ReadInt32());
					break;
				case PacketType.SelectedABonusLevel:
					LevelManager.LoadLevel("bonus", isPvP: false);
					break;
				case PacketType.Quit:
					ScreenManager.ShowMainMenu();
					break;
				case PacketType.ScreenManagerChange:
					ScreenManager.ReadScreenChange(packetReader);
					break;
				case PacketType.TriggerTriggered:
					TriggerManager.ReadTriggerTriggered(packetReader);
					break;
				case PacketType.ObjectDamaged:
					try
					{
						int index = packetReader.ReadUInt16();
						int amount = packetReader.ReadUInt16();
						ObstacleManager.Obstacles[index].takeDamage(amount, broadcast: false);
					}
					catch (Exception)
					{
					}
					break;
				case PacketType.FighterDeath:
					FighterManager.ReadFighterDeath(packetReader);
					break;
				case PacketType.FighterStats:
					ReadPlayerStats(packetReader);
					break;
				case PacketType.FighterStunned:
					FighterManager.ReadFighterStunned(packetReader);
					break;
				case PacketType.AddProjectile:
					FighterManager.ReadAddProjectile(packetReader);
					break;
				}
			}
		}
	}
}
