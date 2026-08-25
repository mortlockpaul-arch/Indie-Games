using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using SpaceBlast.Weapons;

namespace SpaceBlast.Networking;

internal class NetworkManager : GameComponent
{
	private const int constVersion = 1;

	private const int constFuturePosMs = 500;

	private const float constFuturePosSecs = 0.5f;

	private const int constTimeSyncFreqMs = 10000;

	private NetworkSessionType m_SessionType;

	private bool m_SigningIn;

	public bool IsNetworkGame;

	public int LevelNumber;

	private Random m_Random = new Random();

	private NetworkSession m_NetSession;

	private IAsyncResult m_AsyncGameSearch;

	private AvailableNetworkSessionCollection m_AvailableSessions;

	private int m_TicksSincePosUpdate;

	private bool m_ForcePositionUpdate;

	private PacketWriter m_PacketWriter = new PacketWriter();

	private PacketReader m_PacketReader = new PacketReader();

	private TimeSyncServer m_TimeSyncServer;

	private TimeSyncClient m_TimeSyncClient;

	private int m_TimeSyncRequestSent;

	public bool IsXBoxLiveGame
	{
		get
		{
			if (!IsNetworkGame)
			{
				return false;
			}
			return m_SessionType == NetworkSessionType.PlayerMatch;
		}
	}

	public bool IsHost
	{
		get
		{
			if (m_NetSession == null)
			{
				return false;
			}
			return m_NetSession.IsHost;
		}
	}

	private event EventHandler<EventArgs> m_SignedInEvent;

	private event EventHandler<EventArgs> m_SignedInCancelledEvent;

	private event EventHandler<EventArgs> m_GameSearchCompleteEvent;

	public event EventHandler<GamerJoinedEventArgs> GamerJoinedEvent;

	public event EventHandler<GamerLeftEventArgs> GamerLeftEvent;

	public event EventHandler<GameStartedEventArgs> GameStartedEvent;

	public event EventHandler<GameEndedEventArgs> GameEndedEvent;

	public event EventHandler<NetworkSessionEndedEventArgs> SessionEndedEvent;

	public NetworkManager(Game game)
		: base(game)
	{
		NetworkSession.InviteAccepted += NetworkSession_InviteAccepted;
	}

	public override void Update(GameTime gameTime)
	{
		if (m_SigningIn)
		{
			if (IsPlayerSignedIn())
			{
				m_SigningIn = false;
				if (m_SignedInEvent != null)
				{
					m_SignedInEvent(this, EventArgs.Empty);
				}
			}
			else if (m_SigningIn && !Guide.IsVisible)
			{
				m_SigningIn = false;
				if (m_SignedInCancelledEvent != null)
				{
					m_SignedInCancelledEvent(this, EventArgs.Empty);
				}
			}
		}
		else if (m_NetSession != null)
		{
			ProcessIncomingPackets(gameTime);
			if (m_NetSession.SessionState == NetworkSessionState.Playing)
			{
				m_TicksSincePosUpdate++;
				if (m_TicksSincePosUpdate >= 6 || m_ForcePositionUpdate)
				{
					m_TicksSincePosUpdate = 0;
					SendPositionPackets();
					m_ForcePositionUpdate = false;
				}
			}
			if (m_TimeSyncClient != null && gameTime.TotalRealTime.TotalMilliseconds - (double)m_TimeSyncRequestSent > 10000.0)
			{
				m_TimeSyncClient.SendTimeSyncRequest((int)gameTime.TotalRealTime.TotalMilliseconds);
				m_TimeSyncRequestSent = (int)gameTime.TotalRealTime.TotalMilliseconds;
			}
			m_NetSession.Update();
		}
		if (m_AsyncGameSearch != null && m_AsyncGameSearch.IsCompleted)
		{
			m_AvailableSessions = NetworkSession.EndFind(m_AsyncGameSearch);
			m_AsyncGameSearch = null;
			m_GameSearchCompleteEvent(this, EventArgs.Empty);
		}
	}

	public void SignIn(NetworkSessionType type, EventHandler<EventArgs> SignInEvent, EventHandler<EventArgs> SignInCancelledEvent)
	{
		m_SignedInEvent = SignInEvent;
		m_SignedInCancelledEvent = SignInCancelledEvent;
		m_SessionType = type;
		if (IsPlayerSignedIn())
		{
			m_SigningIn = false;
			if (m_SignedInEvent != null)
			{
				m_SignedInEvent(this, EventArgs.Empty);
			}
		}
		else
		{
			m_SigningIn = true;
			if (!Guide.IsVisible)
			{
				Guide.ShowSignIn(1, type == NetworkSessionType.PlayerMatch);
			}
		}
	}

	public bool IsPlayerSignedIn()
	{
		foreach (SignedInGamer signedInGamer in Gamer.SignedInGamers)
		{
			if ((m_SessionType == NetworkSessionType.PlayerMatch && signedInGamer.IsSignedInToLive) || m_SessionType != NetworkSessionType.PlayerMatch)
			{
				return true;
			}
		}
		return false;
	}

	public void CreateGame(bool publicgame)
	{
		if (m_NetSession != null)
		{
			AbortGameSession();
		}
		NetworkSessionProperties networkSessionProperties = new NetworkSessionProperties();
		networkSessionProperties[0] = 0;
		networkSessionProperties[1] = 1;
		networkSessionProperties[2] = ((!publicgame) ? 1 : 0);
		m_NetSession = NetworkSession.Create(m_SessionType, 1, 8, 0, networkSessionProperties);
		SetGameStatus(GameStatus.InProgress);
		m_NetSession.AllowJoinInProgress = true;
		m_NetSession.AllowHostMigration = true;
		m_TimeSyncServer = new TimeSyncServer(m_NetSession.LocalGamers[0], m_PacketReader, m_PacketWriter);
		LevelNumber = m_Random.Next(GameLevel.constLevelCount);
		m_NetSession.StartGame();
		SystemLinkGameOptions systemLinkGameOptions = new SystemLinkGameOptions();
		systemLinkGameOptions.m_Gamers.Add(m_NetSession.LocalGamers[0]);
		MainGame.Instance.StartNewGame(systemLinkGameOptions);
		SetupSessionEventHandlers();
		MainGame.Instance.AddToMessageWindow("You are the first player. Other players will join shortly.");
	}

	public bool JoinGameSession(AvailableNetworkSession session)
	{
		try
		{
			if (m_NetSession != null)
			{
				m_NetSession.Dispose();
				m_NetSession = null;
			}
			m_NetSession = NetworkSession.Join(session);
			m_TimeSyncClient = new TimeSyncClient(m_NetSession.LocalGamers[0], m_NetSession.Host, m_PacketReader, m_PacketWriter);
			SystemLinkGameOptions systemLinkGameOptions = new SystemLinkGameOptions();
			systemLinkGameOptions.m_Gamers.Add(m_NetSession.LocalGamers[0]);
			MainGame.Instance.StartNewGame(systemLinkGameOptions);
		}
		catch (NetworkSessionJoinException)
		{
			return false;
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public void StartNextLevel()
	{
		int num;
		for (num = LevelNumber; num == LevelNumber; num = m_Random.Next(GameLevel.constLevelCount))
		{
		}
		LevelNumber = num;
		MainGame.LevelData.LoadLevel(LevelNumber);
		m_NetSession.StartGame();
		SetGameStatus(GameStatus.InProgress);
		SendNextLevelStartedPacket(LevelNumber);
		PrepareForNextLevel();
		if (MainGame.Instance.IsPaused)
		{
			MainGame.Instance.ResumeGame();
		}
	}

	private void PrepareForNextLevel()
	{
		int num = 0;
		foreach (Player value in MainGame.Players.PlayerMap.Values)
		{
			value.TheShip.Position = MainGame.LevelData.GetPlayerStartPosition(num++).Position;
			value.TheShip.Reset();
			value.Kills = 0;
		}
		MainGame.Players.ResetTeamScores();
	}

	private void NetworkSession_InviteAccepted(object sender, InviteAcceptedEventArgs e)
	{
		try
		{
			if (m_NetSession != null)
			{
				m_NetSession.Dispose();
				m_NetSession = null;
			}
			m_NetSession = NetworkSession.JoinInvited(1);
			m_TimeSyncClient = new TimeSyncClient(m_NetSession.LocalGamers[0], m_NetSession.Host, m_PacketReader, m_PacketWriter);
			SystemLinkGameOptions systemLinkGameOptions = new SystemLinkGameOptions();
			systemLinkGameOptions.m_Gamers.Add(m_NetSession.LocalGamers[0]);
			MainGame.Instance.StartNewGame(systemLinkGameOptions);
		}
		catch (NetworkSessionJoinException)
		{
		}
		catch (Exception)
		{
		}
	}

	private void SetupSessionEventHandlers()
	{
		m_NetSession.GameStarted += NetworkGameStartedEvent;
		m_NetSession.GameEnded += NetworkGameEndedEvent;
		m_NetSession.SessionEnded += NetworkSessionEndedEvent;
		m_NetSession.HostChanged += NetworkSessionHostChangedEvent;
		m_NetSession.GamerJoined += NetworkGamerJoinedEvent;
		m_NetSession.GamerLeft += NetworkGamerLeftEvent;
	}

	private void NetworkSessionHostChangedEvent(object sender, HostChangedEventArgs e)
	{
		if (IsHost)
		{
			m_TimeSyncClient = null;
			m_TimeSyncServer = new TimeSyncServer(m_NetSession.LocalGamers[0], m_PacketReader, m_PacketWriter);
		}
		else
		{
			m_TimeSyncClient.HostChanged(e.NewHost);
			SendPlayerScorePacket(MainGame.Instance.LeftPlayer.PlayerID, MainGame.Instance.LeftPlayer.Kills);
		}
	}

	public void AbortGameSession()
	{
		if (m_NetSession != null)
		{
			m_NetSession.Dispose();
		}
		m_NetSession = null;
	}

	public void FindNetworkSessions(EventHandler<EventArgs> searchCompleteEvent)
	{
		if (m_AsyncGameSearch != null)
		{
			NetworkSession.EndFind(m_AsyncGameSearch);
			m_AsyncGameSearch = null;
		}
		if (m_AvailableSessions != null)
		{
			m_AvailableSessions.Dispose();
			m_AvailableSessions = null;
		}
		if (m_NetSession != null)
		{
			m_NetSession.Dispose();
			m_NetSession = null;
		}
		m_GameSearchCompleteEvent = searchCompleteEvent;
		NetworkSessionProperties networkSessionProperties = new NetworkSessionProperties();
		networkSessionProperties[0] = 0;
		networkSessionProperties[1] = 1;
		networkSessionProperties[2] = 0;
		m_AsyncGameSearch = NetworkSession.BeginFind(m_SessionType, 1, networkSessionProperties, null, null);
	}

	public AvailableNetworkSessionCollection GetGameSearchList()
	{
		return m_AvailableSessions;
	}

	public void LeaveNetworkGame()
	{
		m_NetSession.Dispose();
		m_NetSession = null;
	}

	public void SetGameOver()
	{
		m_NetSession.EndGame();
	}

	public void SetGameStatus(GameStatus status)
	{
		m_NetSession.SessionProperties[0] = (int)status;
	}

	private GameStatus GetGameStatus()
	{
		return (GameStatus)m_NetSession.SessionProperties[0].Value;
	}

	private void NetworkGameStartedEvent(object sender, GameStartedEventArgs e)
	{
		GameStartedEvent(sender, e);
	}

	private void NetworkGameEndedEvent(object sender, GameEndedEventArgs e)
	{
		GameEndedEvent(sender, e);
	}

	private void NetworkGamerJoinedEvent(object sender, GamerJoinedEventArgs e)
	{
		if (m_NetSession.IsHost && !(e.Gamer is LocalNetworkGamer))
		{
			SendGameInfo(e.Gamer);
			foreach (Player value in MainGame.Players.PlayerMap.Values)
			{
				SendPlayerJoinedPacket(e.Gamer, value);
			}
			byte id = MainGame.Players.AddRemotePlayer(e.Gamer, null, null);
			SendPlayerJoinedPacket(null, MainGame.Players.GetPlayer(id));
		}
		GamerJoinedEvent(sender, e);
	}

	private void NetworkGamerLeftEvent(object sender, GamerLeftEventArgs e)
	{
		MainGame.Instance.AddToMessageWindow(e.Gamer.Gamertag + " has left the game");
		MainGame.Players.DeletePlayer(e.Gamer.Id);
		GamerLeftEvent(sender, e);
	}

	private void NetworkSessionEndedEvent(object sender, NetworkSessionEndedEventArgs e)
	{
		SessionEndedEvent(sender, e);
	}

	private void ProcessIncomingPackets(GameTime gameTime)
	{
		while (m_NetSession.LocalGamers[0].IsDataAvailable)
		{
			try
			{
				m_NetSession.LocalGamers[0].ReceiveData(m_PacketReader, out var sender);
				byte b = m_PacketReader.ReadByte();
				if (b != 3 && object.ReferenceEquals(sender, m_NetSession.LocalGamers[0]))
				{
					break;
				}
				switch (b)
				{
				case 1:
					m_TimeSyncServer.ProcessTimeSyncRequest(sender);
					break;
				case 2:
					m_TimeSyncClient.ProcessTimeSyncResponse((int)gameTime.TotalRealTime.TotalMilliseconds);
					break;
				case 4:
					ProcessGameInfoPacket();
					break;
				case 7:
					ProcessGameOverPacket();
					break;
				case 15:
					ProcessPlayerJoinedPacket();
					break;
				case 20:
					ProcessPositionUpdatePacket();
					break;
				case 30:
					ProcessShowPowerupPacket();
					break;
				case 31:
					ProcessPowerupCollectedPacket();
					break;
				case 40:
					ProcessShipDestroyedPacket();
					break;
				case 50:
					ProcessWeaponFiredPacket();
					break;
				case 52:
					ProcessVBlasterFiredPacket();
					break;
				case 60:
					ProcessSpecialWeaponFiredPacket();
					break;
				case 70:
					ProcessPlayersCollidedPacket();
					break;
				case 80:
					ProcessPlayerRespawnedPacket();
					break;
				case 90:
					ProcessPlayerScorePacket();
					break;
				case 100:
					ProcessNextLevelStartedPacket();
					break;
				}
			}
			catch (PlayerDoesntExistException)
			{
			}
			catch (Exception)
			{
			}
		}
	}

	private void SendGameInfo(NetworkGamer recipient)
	{
		m_PacketWriter.Write((byte)4);
		m_PacketWriter.Write((short)1);
		m_PacketWriter.Write((byte)0);
		m_PacketWriter.Write((byte)20);
		m_PacketWriter.Write((byte)LevelNumber);
		m_PacketWriter.Write((byte)0);
		m_PacketWriter.Write((byte)0);
		m_PacketWriter.Write((byte)0);
		m_PacketWriter.Write(0);
		if (recipient == null)
		{
			m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.ReliableInOrder);
		}
		else
		{
			m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.ReliableInOrder, recipient);
		}
	}

	private void ProcessGameInfoPacket()
	{
		m_PacketReader.ReadInt16();
		m_PacketReader.ReadByte();
		m_PacketReader.ReadByte();
		LevelNumber = m_PacketReader.ReadByte();
		m_PacketReader.ReadByte();
		m_PacketReader.ReadByte();
		m_PacketReader.ReadByte();
		m_PacketReader.ReadInt32();
		MainGame.LevelData.LoadLevel(LevelNumber);
		SetupSessionEventHandlers();
	}

	private void SendPlayerJoinedPacket(NetworkGamer recipient, Player player)
	{
		m_PacketWriter.Write((byte)15);
		NetworkGamer networkGamer = (NetworkGamer)player.GetGamer();
		m_PacketWriter.Write(networkGamer.Id);
		m_PacketWriter.Write((byte)player.TheShip.Colour);
		m_PacketWriter.Write((byte)player.Team);
		m_PacketWriter.Write(player.TheShip.Position.X);
		m_PacketWriter.Write(player.TheShip.Position.Y);
		m_PacketWriter.Write(player.TheShip.Rotation);
		if (recipient == null)
		{
			m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.ReliableInOrder);
		}
		else
		{
			m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.ReliableInOrder, recipient);
		}
	}

	private void ProcessPlayerJoinedPacket()
	{
		byte b = m_PacketReader.ReadByte();
		ShipColor shipColor = (ShipColor)m_PacketReader.ReadByte();
		m_PacketReader.ReadByte();
		float x = m_PacketReader.ReadSingle();
		float y = m_PacketReader.ReadSingle();
		float num = m_PacketReader.ReadSingle();
		if (!MainGame.Players.DoesPlayerExist(b))
		{
			NetworkGamer networkGamer = m_NetSession.FindGamerById(b);
			MainGame.Players.AddRemotePlayer(networkGamer, shipColor, null, new Vector3(x, y, 0f), num);
			MainGame.Instance.AddToMessageWindow(networkGamer.Gamertag + " has joined the game");
		}
		else
		{
			Player player = MainGame.Players.GetPlayer(b);
			player.TheShip.Position = new Vector3(x, y, 0f);
			player.TheShip.Rotation = num;
			player.TheShip.Colour = shipColor;
		}
	}

	private void SendPositionPackets()
	{
		Player leftPlayer = MainGame.Instance.LeftPlayer;
		Vector3 predictedPos = leftPlayer.TheShip.GetPredictedPos(0.5f);
		byte value = (byte)(MathHelper.ToDegrees(leftPlayer.TheShip.Rotation) / 360f * 255f);
		m_PacketWriter.Write((byte)20);
		m_PacketWriter.Write(leftPlayer.PlayerID);
		m_PacketWriter.Write((int)(TimeManager.TotalSeconds * 1000.0) + 500);
		m_PacketWriter.Write(predictedPos.X);
		m_PacketWriter.Write(predictedPos.Y);
		m_PacketWriter.Write(value);
		m_PacketWriter.Write((byte)leftPlayer.TheShip.Shields);
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.InOrder);
	}

	private void ProcessPositionUpdatePacket()
	{
		byte id = m_PacketReader.ReadByte();
		double num = (double)m_PacketReader.ReadInt32() / 1000.0;
		double val = num - TimeManager.TotalSeconds;
		val = Math.Max(val, 0.05);
		float x = m_PacketReader.ReadSingle();
		float y = m_PacketReader.ReadSingle();
		Vector3 pos = new Vector3(x, y, 0f);
		float targetRotation = MathHelper.ToRadians((float)(int)m_PacketReader.ReadByte() / 255f * 360f);
		Ship theShip = MainGame.Players.GetCheckedPlayer(id).TheShip;
		theShip.SetPredictedPos(pos, (float)val);
		theShip.TargetRotation = targetRotation;
		byte b = m_PacketReader.ReadByte();
		theShip.Shields = (int)b;
	}

	public void SendShowPowerupPacket(int powerupID, double when, Vector3 position)
	{
		int value = (int)(when * 1000.0);
		m_PacketWriter.Write((byte)30);
		m_PacketWriter.Write((byte)powerupID);
		m_PacketWriter.Write(value);
		m_PacketWriter.Write(position.X);
		m_PacketWriter.Write(position.Y);
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.ReliableInOrder);
	}

	private void ProcessShowPowerupPacket()
	{
		int index = m_PacketReader.ReadByte();
		int num = m_PacketReader.ReadInt32();
		float x = m_PacketReader.ReadSingle();
		float y = m_PacketReader.ReadSingle();
		double when = (double)num / 1000.0;
		Vector3 vector = new Vector3(x, y, 0f);
		MainGame.LevelData.PowerUps.GetPowerUp(index).SetNextAppearanceTime(when, vector);
	}

	public void SendPowerupCollectedPacket(int powerupID, byte playerID)
	{
		m_PacketWriter.Write((byte)31);
		m_PacketWriter.Write((byte)powerupID);
		m_PacketWriter.Write(playerID);
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.ReliableInOrder);
	}

	private void ProcessPowerupCollectedPacket()
	{
		int index = m_PacketReader.ReadByte();
		byte id = m_PacketReader.ReadByte();
		PowerUp powerUp = MainGame.LevelData.PowerUps.GetPowerUp(index);
		powerUp.ApplyPowerup(MainGame.Players.GetCheckedPlayer(id));
	}

	public void SendShipDestroyedPacket(byte deadPlayerID, byte killedByPlayerID)
	{
		m_PacketWriter.Write((byte)40);
		m_PacketWriter.Write(deadPlayerID);
		m_PacketWriter.Write(killedByPlayerID);
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.InOrder);
	}

	private void ProcessShipDestroyedPacket()
	{
		byte id = m_PacketReader.ReadByte();
		sbyte b = m_PacketReader.ReadSByte();
		Player killedBy = null;
		if (b >= 0)
		{
			killedBy = MainGame.Players.GetCheckedPlayer((byte)b);
		}
		MainGame.Players.GetCheckedPlayer(id).Die(killedBy);
	}

	public void SendPlayersCollidedPacket(byte localPlayerID, byte otherPlayerID, ref Vector3 velocity, ref float damage)
	{
		m_PacketWriter.Write((byte)70);
		m_PacketWriter.Write(localPlayerID);
		m_PacketWriter.Write(otherPlayerID);
		m_PacketWriter.Write(velocity.X);
		m_PacketWriter.Write(velocity.Y);
		m_PacketWriter.Write((short)damage);
		RemotePlayer remotePlayer = (RemotePlayer)MainGame.Players.GetPlayer(otherPlayerID);
		NetworkGamer recipient = (NetworkGamer)remotePlayer.GetGamer();
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.InOrder, recipient);
	}

	private void ProcessPlayersCollidedPacket()
	{
		m_PacketReader.ReadByte();
		byte id = m_PacketReader.ReadByte();
		Vector3 vector = new Vector3
		{
			X = m_PacketReader.ReadSingle(),
			Y = m_PacketReader.ReadSingle()
		};
		m_PacketReader.ReadInt16();
		Player checkedPlayer = MainGame.Players.GetCheckedPlayer(id);
		checkedPlayer.Die(null);
	}

	public void SendWeaponFiredPacket(byte playerid, double time, WeaponType weapon, int weaponcount, Vector3 endpos)
	{
		m_PacketWriter.Write((byte)50);
		m_PacketWriter.Write(playerid);
		m_PacketWriter.Write((byte)weapon);
		m_PacketWriter.Write((byte)weaponcount);
		m_PacketWriter.Write(endpos.X);
		m_PacketWriter.Write(endpos.Y);
		m_PacketWriter.Write((int)(time * 1000.0));
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.InOrder);
		if (m_TicksSincePosUpdate >= 3)
		{
			m_ForcePositionUpdate = true;
		}
	}

	private void ProcessWeaponFiredPacket()
	{
		byte id = m_PacketReader.ReadByte();
		WeaponType type = (WeaponType)m_PacketReader.ReadByte();
		int guncount = m_PacketReader.ReadByte();
		float x = m_PacketReader.ReadSingle();
		float y = m_PacketReader.ReadSingle();
		int num = m_PacketReader.ReadInt32();
		RemotePlayer remotePlayer = (RemotePlayer)MainGame.Players.GetCheckedPlayer(id);
		Vector3 endPos = new Vector3(x, y, 0f);
		remotePlayer.TheShip.Weapons.FireRemoteWeapon(type, guncount, ref endPos, (double)num / 1000.0, null);
	}

	public void SendVBlasterFiredPacket(byte playerid, double time, int weaponcount, ref Vector3 centre_endpos, ref Vector3 innerleft_endpos, ref Vector3 innerright_endpos, ref Vector3 outerleft_endpos, ref Vector3 outerright_endpos)
	{
		m_PacketWriter.Write((byte)52);
		m_PacketWriter.Write(playerid);
		m_PacketWriter.Write((byte)weaponcount);
		m_PacketWriter.Write(centre_endpos.X);
		m_PacketWriter.Write(centre_endpos.Y);
		m_PacketWriter.Write(innerleft_endpos.X);
		m_PacketWriter.Write(innerleft_endpos.Y);
		m_PacketWriter.Write(innerright_endpos.X);
		m_PacketWriter.Write(innerright_endpos.Y);
		m_PacketWriter.Write(outerleft_endpos.X);
		m_PacketWriter.Write(outerleft_endpos.Y);
		m_PacketWriter.Write(outerright_endpos.X);
		m_PacketWriter.Write(outerright_endpos.Y);
		m_PacketWriter.Write((int)(time * 1000.0));
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.InOrder);
		if (m_TicksSincePosUpdate >= 3)
		{
			m_ForcePositionUpdate = true;
		}
	}

	private void ProcessVBlasterFiredPacket()
	{
		byte id = m_PacketReader.ReadByte();
		int guncount = m_PacketReader.ReadByte();
		Vector3 centre_endpos = new Vector3
		{
			X = m_PacketReader.ReadSingle(),
			Y = m_PacketReader.ReadSingle()
		};
		Vector3 innerleft_endpos = new Vector3
		{
			X = m_PacketReader.ReadSingle(),
			Y = m_PacketReader.ReadSingle()
		};
		Vector3 innerright_endpos = new Vector3
		{
			X = m_PacketReader.ReadSingle(),
			Y = m_PacketReader.ReadSingle()
		};
		Vector3 outerleft_endpos = new Vector3
		{
			X = m_PacketReader.ReadSingle(),
			Y = m_PacketReader.ReadSingle()
		};
		Vector3 outerright_endpos = new Vector3
		{
			X = m_PacketReader.ReadSingle(),
			Y = m_PacketReader.ReadSingle()
		};
		int num = m_PacketReader.ReadInt32();
		RemotePlayer remotePlayer = (RemotePlayer)MainGame.Players.GetCheckedPlayer(id);
		VBlaster vBlaster = (VBlaster)remotePlayer.TheShip.Weapons.GetWeapon(WeaponType.VBlaster);
		vBlaster.FireRemoteWeapon(guncount, (double)num / 1000.0, ref centre_endpos, ref innerleft_endpos, ref innerright_endpos, ref outerleft_endpos, ref outerright_endpos);
	}

	public void SendSpecialWeaponFiredPacket()
	{
	}

	public void SendSpecialWeaponFiredPacket(byte player_id, double endtime, SpecialWeaponType weapon, Vector3 startpos)
	{
		m_PacketWriter.Write((byte)60);
		m_PacketWriter.Write(player_id);
		m_PacketWriter.Write((byte)weapon);
		m_PacketWriter.Write(startpos.X);
		m_PacketWriter.Write(startpos.Y);
		m_PacketWriter.Write((int)(endtime * 1000.0));
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.InOrder);
		if (m_TicksSincePosUpdate >= 3)
		{
			m_ForcePositionUpdate = true;
		}
	}

	private void ProcessSpecialWeaponFiredPacket()
	{
		byte id = m_PacketReader.ReadByte();
		SpecialWeaponType type = (SpecialWeaponType)m_PacketReader.ReadByte();
		float x = m_PacketReader.ReadSingle();
		float y = m_PacketReader.ReadSingle();
		int num = m_PacketReader.ReadInt32();
		RemotePlayer remotePlayer = (RemotePlayer)MainGame.Players.GetCheckedPlayer(id);
		Vector3 startPos = new Vector3(x, y, 0f);
		remotePlayer.TheShip.Weapons.FireRemoteWeapon(type, ref startPos, (double)num / 1000.0);
	}

	public void SendPlayerRespawnedPacket(byte playerID, RespawnLocation pos)
	{
		m_PacketWriter.Write((byte)80);
		m_PacketWriter.Write(playerID);
		m_PacketWriter.Write(pos.Position.X);
		m_PacketWriter.Write(pos.Position.Y);
		m_PacketWriter.Write(pos.Rotation);
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.InOrder);
	}

	private void ProcessPlayerRespawnedPacket()
	{
		byte id = m_PacketReader.ReadByte();
		float x = m_PacketReader.ReadSingle();
		float y = m_PacketReader.ReadSingle();
		float rotation = m_PacketReader.ReadSingle();
		RespawnLocation pos = new RespawnLocation
		{
			Position = new Vector3(x, y, 0f),
			Rotation = rotation
		};
		RemotePlayer remotePlayer = (RemotePlayer)MainGame.Players.GetCheckedPlayer(id);
		remotePlayer.Respawn(pos);
	}

	public void SendGameOverPacket()
	{
		m_PacketWriter.Write((byte)7);
		foreach (Player value in MainGame.Players.PlayerMap.Values)
		{
			m_PacketWriter.Write(value.PlayerID);
			m_PacketWriter.Write((byte)value.Kills);
		}
		for (int i = MainGame.Players.Count; i < 8; i++)
		{
			m_PacketWriter.Write((sbyte)(-1));
			m_PacketWriter.Write((byte)0);
		}
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.ReliableInOrder);
	}

	private void ProcessGameOverPacket()
	{
		for (int i = 0; i < 8; i++)
		{
			sbyte b = m_PacketReader.ReadSByte();
			byte kills = m_PacketReader.ReadByte();
			if (b > 0)
			{
				Player player = MainGame.Players.GetPlayer((byte)b);
				player.Kills = kills;
			}
		}
		MainGame.Instance.ShowGameOverPage();
	}

	public void SendPlayerScorePacket(byte playerid, int score)
	{
		m_PacketWriter.Write((byte)90);
		m_PacketWriter.Write(playerid);
		m_PacketWriter.Write((byte)score);
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.ReliableInOrder, m_NetSession.Host);
	}

	private void ProcessPlayerScorePacket()
	{
		byte id = m_PacketReader.ReadByte();
		int kills = m_PacketReader.ReadByte();
		Player player = MainGame.Players.GetPlayer(id);
		player.Kills = kills;
	}

	public void SendNextLevelStartedPacket(int levelnumber)
	{
		m_PacketWriter.Write((byte)100);
		m_PacketWriter.Write((byte)levelnumber);
		m_NetSession.LocalGamers[0].SendData(m_PacketWriter, SendDataOptions.ReliableInOrder);
	}

	private void ProcessNextLevelStartedPacket()
	{
		LevelNumber = m_PacketReader.ReadByte();
		MainGame.LevelData.LoadLevel(LevelNumber);
		PrepareForNextLevel();
		if (MainGame.Instance.IsPaused)
		{
			MainGame.Instance.ResumeGame();
		}
	}

	private Gamer FindGamer(string gamertag)
	{
		foreach (NetworkGamer allGamer in m_NetSession.AllGamers)
		{
			if (allGamer.Gamertag == gamertag)
			{
				return allGamer;
			}
		}
		return null;
	}

	public LocalNetworkGamer GetLocalGamer()
	{
		return m_NetSession.LocalGamers[0];
	}
}
