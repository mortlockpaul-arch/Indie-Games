using System;
using System.Collections.ObjectModel;
using System.IO;
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

	private Random m_Random;

	private NetworkSession m_NetSession;

	private IAsyncResult m_AsyncGameSearch;

	private AvailableNetworkSessionCollection m_AvailableSessions;

	private int m_TicksSincePosUpdate;

	private bool m_ForcePositionUpdate;

	private PacketWriter m_PacketWriter;

	private PacketReader m_PacketReader;

	private TimeSyncServer m_TimeSyncServer;

	private TimeSyncClient m_TimeSyncClient;

	private int m_TimeSyncRequestSent;

	public bool IsXBoxLiveGame
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Invalid comparison between Unknown and I4
			if (!IsNetworkGame)
			{
				return false;
			}
			return (int)m_SessionType == 2;
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
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		m_Random = new Random();
		m_PacketWriter = new PacketWriter();
		m_PacketReader = new PacketReader();
		((GameComponent)this)._002Ector(game);
		NetworkSession.InviteAccepted += NetworkSession_InviteAccepted;
	}

	public override void Update(GameTime gameTime)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Invalid comparison between Unknown and I4
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
			if ((int)m_NetSession.SessionState == 1)
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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
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
				Guide.ShowSignIn(1, (int)type == 2);
			}
		}
	}

	public bool IsPlayerSignedIn()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				if (((int)m_SessionType == 2 && current.IsSignedInToLive) || (int)m_SessionType != 2)
				{
					return true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		return false;
	}

	public void CreateGame(bool publicgame)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (m_NetSession != null)
		{
			AbortGameSession();
		}
		NetworkSessionProperties val = new NetworkSessionProperties();
		val[0] = 0;
		val[1] = 1;
		val[2] = ((!publicgame) ? 1 : 0);
		m_NetSession = NetworkSession.Create(m_SessionType, 1, 8, 0, val);
		SetGameStatus(GameStatus.InProgress);
		m_NetSession.AllowJoinInProgress = true;
		m_NetSession.AllowHostMigration = true;
		m_TimeSyncServer = new TimeSyncServer(((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0], m_PacketReader, m_PacketWriter);
		LevelNumber = m_Random.Next(GameLevel.constLevelCount);
		m_NetSession.StartGame();
		SystemLinkGameOptions systemLinkGameOptions = new SystemLinkGameOptions();
		systemLinkGameOptions.m_Gamers.Add((Gamer)(object)((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0]);
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
			m_TimeSyncClient = new TimeSyncClient(((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0], m_NetSession.Host, m_PacketReader, m_PacketWriter);
			SystemLinkGameOptions systemLinkGameOptions = new SystemLinkGameOptions();
			systemLinkGameOptions.m_Gamers.Add((Gamer)(object)((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0]);
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
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
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
			m_TimeSyncClient = new TimeSyncClient(((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0], m_NetSession.Host, m_PacketReader, m_PacketWriter);
			SystemLinkGameOptions systemLinkGameOptions = new SystemLinkGameOptions();
			systemLinkGameOptions.m_Gamers.Add((Gamer)(object)((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0]);
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
			m_TimeSyncServer = new TimeSyncServer(((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0], m_PacketReader, m_PacketWriter);
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
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
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
		NetworkSessionProperties val = new NetworkSessionProperties();
		val[0] = 0;
		val[1] = 1;
		val[2] = 0;
		m_AsyncGameSearch = NetworkSession.BeginFind(m_SessionType, 1, val, (AsyncCallback)null, (object)null);
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
		MainGame.Instance.AddToMessageWindow(((Gamer)e.Gamer).Gamertag + " has left the game");
		MainGame.Players.DeletePlayer(e.Gamer.Id);
		GamerLeftEvent(sender, e);
	}

	private void NetworkSessionEndedEvent(object sender, NetworkSessionEndedEventArgs e)
	{
		SessionEndedEvent(sender, e);
	}

	private void ProcessIncomingPackets(GameTime gameTime)
	{
		NetworkGamer val = default(NetworkGamer);
		while (((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].IsDataAvailable)
		{
			try
			{
				((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].ReceiveData(m_PacketReader, ref val);
				byte b = ((BinaryReader)(object)m_PacketReader).ReadByte();
				if (b != 3 && object.ReferenceEquals(val, ((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0]))
				{
					break;
				}
				switch (b)
				{
				case 1:
					m_TimeSyncServer.ProcessTimeSyncRequest(val);
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
		((BinaryWriter)(object)m_PacketWriter).Write((byte)4);
		((BinaryWriter)(object)m_PacketWriter).Write((short)1);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)0);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)20);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)LevelNumber);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)0);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)0);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)0);
		((BinaryWriter)(object)m_PacketWriter).Write(0);
		if (recipient == null)
		{
			((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)3);
		}
		else
		{
			((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)3, recipient);
		}
	}

	private void ProcessGameInfoPacket()
	{
		((BinaryReader)(object)m_PacketReader).ReadInt16();
		((BinaryReader)(object)m_PacketReader).ReadByte();
		((BinaryReader)(object)m_PacketReader).ReadByte();
		LevelNumber = ((BinaryReader)(object)m_PacketReader).ReadByte();
		((BinaryReader)(object)m_PacketReader).ReadByte();
		((BinaryReader)(object)m_PacketReader).ReadByte();
		((BinaryReader)(object)m_PacketReader).ReadByte();
		((BinaryReader)(object)m_PacketReader).ReadInt32();
		MainGame.LevelData.LoadLevel(LevelNumber);
		SetupSessionEventHandlers();
	}

	private void SendPlayerJoinedPacket(NetworkGamer recipient, Player player)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		((BinaryWriter)(object)m_PacketWriter).Write((byte)15);
		NetworkGamer val = (NetworkGamer)player.GetGamer();
		((BinaryWriter)(object)m_PacketWriter).Write(val.Id);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)player.TheShip.Colour);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)player.Team);
		((BinaryWriter)(object)m_PacketWriter).Write(player.TheShip.Position.X);
		((BinaryWriter)(object)m_PacketWriter).Write(player.TheShip.Position.Y);
		((BinaryWriter)(object)m_PacketWriter).Write(player.TheShip.Rotation);
		if (recipient == null)
		{
			((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)3);
		}
		else
		{
			((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)3, recipient);
		}
	}

	private void ProcessPlayerJoinedPacket()
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		byte b = ((BinaryReader)(object)m_PacketReader).ReadByte();
		ShipColor shipColor = (ShipColor)((BinaryReader)(object)m_PacketReader).ReadByte();
		((BinaryReader)(object)m_PacketReader).ReadByte();
		float num = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		float num2 = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		float num3 = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		if (!MainGame.Players.DoesPlayerExist(b))
		{
			NetworkGamer val = m_NetSession.FindGamerById(b);
			MainGame.Players.AddRemotePlayer(val, shipColor, null, new Vector3(num, num2, 0f), num3);
			MainGame.Instance.AddToMessageWindow(((Gamer)val).Gamertag + " has joined the game");
		}
		else
		{
			Player player = MainGame.Players.GetPlayer(b);
			player.TheShip.Position = new Vector3(num, num2, 0f);
			player.TheShip.Rotation = num3;
			player.TheShip.Colour = shipColor;
		}
	}

	private void SendPositionPackets()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Player leftPlayer = MainGame.Instance.LeftPlayer;
		Vector3 predictedPos = leftPlayer.TheShip.GetPredictedPos(0.5f);
		byte value = (byte)(MathHelper.ToDegrees(leftPlayer.TheShip.Rotation) / 360f * 255f);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)20);
		((BinaryWriter)(object)m_PacketWriter).Write(leftPlayer.PlayerID);
		((BinaryWriter)(object)m_PacketWriter).Write((int)(TimeManager.TotalSeconds * 1000.0) + 500);
		((BinaryWriter)(object)m_PacketWriter).Write(predictedPos.X);
		((BinaryWriter)(object)m_PacketWriter).Write(predictedPos.Y);
		((BinaryWriter)(object)m_PacketWriter).Write(value);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)leftPlayer.TheShip.Shields);
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)2);
	}

	private void ProcessPositionUpdatePacket()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		byte id = ((BinaryReader)(object)m_PacketReader).ReadByte();
		double num = (double)((BinaryReader)(object)m_PacketReader).ReadInt32() / 1000.0;
		double val = num - TimeManager.TotalSeconds;
		val = Math.Max(val, 0.05);
		float num2 = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		float num3 = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		Vector3 pos = default(Vector3);
		((Vector3)(ref pos))._002Ector(num2, num3, 0f);
		float targetRotation = MathHelper.ToRadians((float)(int)((BinaryReader)(object)m_PacketReader).ReadByte() / 255f * 360f);
		Ship theShip = MainGame.Players.GetCheckedPlayer(id).TheShip;
		theShip.SetPredictedPos(pos, (float)val);
		theShip.TargetRotation = targetRotation;
		byte b = ((BinaryReader)(object)m_PacketReader).ReadByte();
		theShip.Shields = (int)b;
	}

	public void SendShowPowerupPacket(int powerupID, double when, Vector3 position)
	{
		int value = (int)(when * 1000.0);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)30);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)powerupID);
		((BinaryWriter)(object)m_PacketWriter).Write(value);
		((BinaryWriter)(object)m_PacketWriter).Write(position.X);
		((BinaryWriter)(object)m_PacketWriter).Write(position.Y);
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)3);
	}

	private void ProcessShowPowerupPacket()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		int index = ((BinaryReader)(object)m_PacketReader).ReadByte();
		int num = ((BinaryReader)(object)m_PacketReader).ReadInt32();
		float num2 = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		float num3 = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		double when = (double)num / 1000.0;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(num2, num3, 0f);
		MainGame.LevelData.PowerUps.GetPowerUp(index).SetNextAppearanceTime(when, val);
	}

	public void SendPowerupCollectedPacket(int powerupID, byte playerID)
	{
		((BinaryWriter)(object)m_PacketWriter).Write((byte)31);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)powerupID);
		((BinaryWriter)(object)m_PacketWriter).Write(playerID);
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)3);
	}

	private void ProcessPowerupCollectedPacket()
	{
		int index = ((BinaryReader)(object)m_PacketReader).ReadByte();
		byte id = ((BinaryReader)(object)m_PacketReader).ReadByte();
		PowerUp powerUp = MainGame.LevelData.PowerUps.GetPowerUp(index);
		powerUp.ApplyPowerup(MainGame.Players.GetCheckedPlayer(id));
	}

	public void SendShipDestroyedPacket(byte deadPlayerID, byte killedByPlayerID)
	{
		((BinaryWriter)(object)m_PacketWriter).Write((byte)40);
		((BinaryWriter)(object)m_PacketWriter).Write(deadPlayerID);
		((BinaryWriter)(object)m_PacketWriter).Write(killedByPlayerID);
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)2);
	}

	private void ProcessShipDestroyedPacket()
	{
		byte id = ((BinaryReader)(object)m_PacketReader).ReadByte();
		sbyte b = ((BinaryReader)(object)m_PacketReader).ReadSByte();
		Player killedBy = null;
		if (b >= 0)
		{
			killedBy = MainGame.Players.GetCheckedPlayer((byte)b);
		}
		MainGame.Players.GetCheckedPlayer(id).Die(killedBy);
	}

	public void SendPlayersCollidedPacket(byte localPlayerID, byte otherPlayerID, ref Vector3 velocity, ref float damage)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		((BinaryWriter)(object)m_PacketWriter).Write((byte)70);
		((BinaryWriter)(object)m_PacketWriter).Write(localPlayerID);
		((BinaryWriter)(object)m_PacketWriter).Write(otherPlayerID);
		((BinaryWriter)(object)m_PacketWriter).Write(velocity.X);
		((BinaryWriter)(object)m_PacketWriter).Write(velocity.Y);
		((BinaryWriter)(object)m_PacketWriter).Write((short)damage);
		RemotePlayer remotePlayer = (RemotePlayer)MainGame.Players.GetPlayer(otherPlayerID);
		NetworkGamer val = (NetworkGamer)remotePlayer.GetGamer();
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)2, val);
	}

	private void ProcessPlayersCollidedPacket()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		((BinaryReader)(object)m_PacketReader).ReadByte();
		byte id = ((BinaryReader)(object)m_PacketReader).ReadByte();
		Vector3 val = new Vector3
		{
			X = ((BinaryReader)(object)m_PacketReader).ReadSingle(),
			Y = ((BinaryReader)(object)m_PacketReader).ReadSingle()
		};
		((BinaryReader)(object)m_PacketReader).ReadInt16();
		Player checkedPlayer = MainGame.Players.GetCheckedPlayer(id);
		checkedPlayer.Die(null);
	}

	public void SendWeaponFiredPacket(byte playerid, double time, WeaponType weapon, int weaponcount, Vector3 endpos)
	{
		((BinaryWriter)(object)m_PacketWriter).Write((byte)50);
		((BinaryWriter)(object)m_PacketWriter).Write(playerid);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)weapon);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)weaponcount);
		((BinaryWriter)(object)m_PacketWriter).Write(endpos.X);
		((BinaryWriter)(object)m_PacketWriter).Write(endpos.Y);
		((BinaryWriter)(object)m_PacketWriter).Write((int)(time * 1000.0));
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)2);
		if (m_TicksSincePosUpdate >= 3)
		{
			m_ForcePositionUpdate = true;
		}
	}

	private void ProcessWeaponFiredPacket()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		byte id = ((BinaryReader)(object)m_PacketReader).ReadByte();
		WeaponType type = (WeaponType)((BinaryReader)(object)m_PacketReader).ReadByte();
		int guncount = ((BinaryReader)(object)m_PacketReader).ReadByte();
		float num = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		float num2 = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		int num3 = ((BinaryReader)(object)m_PacketReader).ReadInt32();
		RemotePlayer remotePlayer = (RemotePlayer)MainGame.Players.GetCheckedPlayer(id);
		Vector3 endPos = new Vector3(num, num2, 0f);
		remotePlayer.TheShip.Weapons.FireRemoteWeapon(type, guncount, ref endPos, (double)num3 / 1000.0, null);
	}

	public void SendVBlasterFiredPacket(byte playerid, double time, int weaponcount, ref Vector3 centre_endpos, ref Vector3 innerleft_endpos, ref Vector3 innerright_endpos, ref Vector3 outerleft_endpos, ref Vector3 outerright_endpos)
	{
		((BinaryWriter)(object)m_PacketWriter).Write((byte)52);
		((BinaryWriter)(object)m_PacketWriter).Write(playerid);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)weaponcount);
		((BinaryWriter)(object)m_PacketWriter).Write(centre_endpos.X);
		((BinaryWriter)(object)m_PacketWriter).Write(centre_endpos.Y);
		((BinaryWriter)(object)m_PacketWriter).Write(innerleft_endpos.X);
		((BinaryWriter)(object)m_PacketWriter).Write(innerleft_endpos.Y);
		((BinaryWriter)(object)m_PacketWriter).Write(innerright_endpos.X);
		((BinaryWriter)(object)m_PacketWriter).Write(innerright_endpos.Y);
		((BinaryWriter)(object)m_PacketWriter).Write(outerleft_endpos.X);
		((BinaryWriter)(object)m_PacketWriter).Write(outerleft_endpos.Y);
		((BinaryWriter)(object)m_PacketWriter).Write(outerright_endpos.X);
		((BinaryWriter)(object)m_PacketWriter).Write(outerright_endpos.Y);
		((BinaryWriter)(object)m_PacketWriter).Write((int)(time * 1000.0));
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)2);
		if (m_TicksSincePosUpdate >= 3)
		{
			m_ForcePositionUpdate = true;
		}
	}

	private void ProcessVBlasterFiredPacket()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		byte id = ((BinaryReader)(object)m_PacketReader).ReadByte();
		int guncount = ((BinaryReader)(object)m_PacketReader).ReadByte();
		Vector3 centre_endpos = new Vector3
		{
			X = ((BinaryReader)(object)m_PacketReader).ReadSingle(),
			Y = ((BinaryReader)(object)m_PacketReader).ReadSingle()
		};
		Vector3 innerleft_endpos = new Vector3
		{
			X = ((BinaryReader)(object)m_PacketReader).ReadSingle(),
			Y = ((BinaryReader)(object)m_PacketReader).ReadSingle()
		};
		Vector3 innerright_endpos = new Vector3
		{
			X = ((BinaryReader)(object)m_PacketReader).ReadSingle(),
			Y = ((BinaryReader)(object)m_PacketReader).ReadSingle()
		};
		Vector3 outerleft_endpos = new Vector3
		{
			X = ((BinaryReader)(object)m_PacketReader).ReadSingle(),
			Y = ((BinaryReader)(object)m_PacketReader).ReadSingle()
		};
		Vector3 outerright_endpos = new Vector3
		{
			X = ((BinaryReader)(object)m_PacketReader).ReadSingle(),
			Y = ((BinaryReader)(object)m_PacketReader).ReadSingle()
		};
		int num = ((BinaryReader)(object)m_PacketReader).ReadInt32();
		RemotePlayer remotePlayer = (RemotePlayer)MainGame.Players.GetCheckedPlayer(id);
		VBlaster vBlaster = (VBlaster)remotePlayer.TheShip.Weapons.GetWeapon(WeaponType.VBlaster);
		vBlaster.FireRemoteWeapon(guncount, (double)num / 1000.0, ref centre_endpos, ref innerleft_endpos, ref innerright_endpos, ref outerleft_endpos, ref outerright_endpos);
	}

	public void SendSpecialWeaponFiredPacket()
	{
	}

	public void SendSpecialWeaponFiredPacket(byte player_id, double endtime, SpecialWeaponType weapon, Vector3 startpos)
	{
		((BinaryWriter)(object)m_PacketWriter).Write((byte)60);
		((BinaryWriter)(object)m_PacketWriter).Write(player_id);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)weapon);
		((BinaryWriter)(object)m_PacketWriter).Write(startpos.X);
		((BinaryWriter)(object)m_PacketWriter).Write(startpos.Y);
		((BinaryWriter)(object)m_PacketWriter).Write((int)(endtime * 1000.0));
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)2);
		if (m_TicksSincePosUpdate >= 3)
		{
			m_ForcePositionUpdate = true;
		}
	}

	private void ProcessSpecialWeaponFiredPacket()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		byte id = ((BinaryReader)(object)m_PacketReader).ReadByte();
		SpecialWeaponType type = (SpecialWeaponType)((BinaryReader)(object)m_PacketReader).ReadByte();
		float num = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		float num2 = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		int num3 = ((BinaryReader)(object)m_PacketReader).ReadInt32();
		RemotePlayer remotePlayer = (RemotePlayer)MainGame.Players.GetCheckedPlayer(id);
		Vector3 startPos = new Vector3(num, num2, 0f);
		remotePlayer.TheShip.Weapons.FireRemoteWeapon(type, ref startPos, (double)num3 / 1000.0);
	}

	public void SendPlayerRespawnedPacket(byte playerID, RespawnLocation pos)
	{
		((BinaryWriter)(object)m_PacketWriter).Write((byte)80);
		((BinaryWriter)(object)m_PacketWriter).Write(playerID);
		((BinaryWriter)(object)m_PacketWriter).Write(pos.Position.X);
		((BinaryWriter)(object)m_PacketWriter).Write(pos.Position.Y);
		((BinaryWriter)(object)m_PacketWriter).Write(pos.Rotation);
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)2);
	}

	private void ProcessPlayerRespawnedPacket()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		byte id = ((BinaryReader)(object)m_PacketReader).ReadByte();
		float num = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		float num2 = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		float rotation = ((BinaryReader)(object)m_PacketReader).ReadSingle();
		RespawnLocation pos = new RespawnLocation
		{
			Position = new Vector3(num, num2, 0f),
			Rotation = rotation
		};
		RemotePlayer remotePlayer = (RemotePlayer)MainGame.Players.GetCheckedPlayer(id);
		remotePlayer.Respawn(pos);
	}

	public void SendGameOverPacket()
	{
		((BinaryWriter)(object)m_PacketWriter).Write((byte)7);
		foreach (Player value in MainGame.Players.PlayerMap.Values)
		{
			((BinaryWriter)(object)m_PacketWriter).Write(value.PlayerID);
			((BinaryWriter)(object)m_PacketWriter).Write((byte)value.Kills);
		}
		for (int i = MainGame.Players.Count; i < 8; i++)
		{
			((BinaryWriter)(object)m_PacketWriter).Write((sbyte)(-1));
			((BinaryWriter)(object)m_PacketWriter).Write((byte)0);
		}
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)3);
	}

	private void ProcessGameOverPacket()
	{
		for (int i = 0; i < 8; i++)
		{
			sbyte b = ((BinaryReader)(object)m_PacketReader).ReadSByte();
			byte kills = ((BinaryReader)(object)m_PacketReader).ReadByte();
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
		((BinaryWriter)(object)m_PacketWriter).Write((byte)90);
		((BinaryWriter)(object)m_PacketWriter).Write(playerid);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)score);
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)3, m_NetSession.Host);
	}

	private void ProcessPlayerScorePacket()
	{
		byte id = ((BinaryReader)(object)m_PacketReader).ReadByte();
		int kills = ((BinaryReader)(object)m_PacketReader).ReadByte();
		Player player = MainGame.Players.GetPlayer(id);
		player.Kills = kills;
	}

	public void SendNextLevelStartedPacket(int levelnumber)
	{
		((BinaryWriter)(object)m_PacketWriter).Write((byte)100);
		((BinaryWriter)(object)m_PacketWriter).Write((byte)levelnumber);
		((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0].SendData(m_PacketWriter, (SendDataOptions)3);
	}

	private void ProcessNextLevelStartedPacket()
	{
		LevelNumber = ((BinaryReader)(object)m_PacketReader).ReadByte();
		MainGame.LevelData.LoadLevel(LevelNumber);
		PrepareForNextLevel();
		if (MainGame.Instance.IsPaused)
		{
			MainGame.Instance.ResumeGame();
		}
	}

	private Gamer FindGamer(string gamertag)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		GamerCollectionEnumerator<NetworkGamer> enumerator = m_NetSession.AllGamers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Gamer current = (Gamer)(object)enumerator.Current;
				if (current.Gamertag == gamertag)
				{
					return current;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		return null;
	}

	public LocalNetworkGamer GetLocalGamer()
	{
		return ((ReadOnlyCollection<LocalNetworkGamer>)(object)m_NetSession.LocalGamers)[0];
	}
}
