using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using ZP2K9.characters;
using ZP2K9.hud.messageHud;
using ZP2K9.menu;
using ZP2K9.menu.levels;

namespace ZP2K9.net;

public class NetSession
{
	public const int VERSION = 206;

	public const int NET_LOCAL = 0;

	public const int NET_EDITOR_TEST = 1;

	public const int NET_SYSTEMLINK = 2;

	public const int NET_LIVE = 3;

	public const int BOT_OFF = 0;

	public const int BOT_REPLACEMENT = 1;

	public const int BOT_MAX = 2;

	public const int DIFF_EASY = 0;

	public const int DIFF_NORMAL = 1;

	public const int DIFF_HARD = 2;

	public const int DIFF_EXPERT = 3;

	public const int FLAG_HOME = 200;

	public const int HILL_OPEN = 0;

	public const int HILL_BLUE = 1;

	public const int HILL_RED = 2;

	public StringBuilder version = new StringBuilder("Version: 2.0.6");

	public bool newVersAvailable;

	public StringBuilder[] newAvail = new StringBuilder[4]
	{
		new StringBuilder("New Version Available!"),
		new StringBuilder("-"),
		new StringBuilder("Download from"),
		new StringBuilder("Games Marketplace!")
	};

	public NetworkSession netSession;

	private IAsyncResult createResult;

	private IAsyncResult findResult;

	private IAsyncResult joinResult;

	private IAsyncResult joinInviteResult;

	public bool pendingCreate;

	public bool pendingFind;

	public bool pendingJoin;

	public bool pendingJoinInvite;

	public bool createFailed;

	public bool findFailed;

	public bool joinFailed;

	public bool joinInviteFailed;

	public string failMessage;

	public AvailableNetworkSessionCollection sessions;

	public NetPlay netPlay;

	private int freeSlot;

	public float redTime;

	public float blueTime;

	public int redScore;

	public int blueScore;

	public int netType;

	public Dictionary<byte, int> playerList;

	public int botCount;

	public int botDifficulty = 1;

	public bool rebootBot;

	public int mutator;

	public int scoreLimit = 100;

	private float postLobbyFrame;

	public bool postLobby;

	public float gameLength;

	public bool privateMatch;

	public int pRedFlagState = 200;

	public int pBlueFlagState = 200;

	public int redFlagState = 200;

	public int blueFlagState = 200;

	public int hillState;

	private float nullHostFixFrame;

	public int[] DMScores = new int[4] { 500, 1000, 1500, 2500 };

	public int[] TDMScores = new int[4] { 1000, 2500, 5000, 10000 };

	public int[] ZHScores = new int[4] { 800, 2000, 4500, 9000 };

	public int[] CTFScores = new int[4] { 3, 5, 7, 10 };

	public float[] KOTHScores = new float[4] { 180f, 300f, 420f, 600f };

	public int DMScoreIdx;

	public int TDMScoreIdx;

	public int ZHScoreIdx;

	public int CTFScoreIdx;

	public int KOTHScoreIdx;

	public NetSession()
	{
		playerList = new Dictionary<byte, int>();
		netPlay = new NetPlay();
	}

	public int BotCount()
	{
		int num = 0;
		switch (botCount)
		{
		case 1:
			if (netSession != null)
			{
				num = 7 - ((ReadOnlyCollection<NetworkGamer>)(object)netSession.AllGamers).Count;
			}
			break;
		case 2:
			num = 6;
			break;
		}
		if (num > 6)
		{
			num = 6;
		}
		return num;
	}

	public void ResetGameStats()
	{
		gameLength = 0f;
		redScore = 0;
		blueScore = 0;
		redTime = 0f;
		blueTime = 0f;
		redFlagState = 200;
		blueFlagState = 200;
		pRedFlagState = 200;
		pBlueFlagState = 200;
		hillState = 0;
	}

	public void JoinInvite(InviteAcceptedEventArgs ie)
	{
		if (netSession != null)
		{
			netSession.Dispose();
			while (!netSession.IsDisposed)
			{
			}
		}
		netType = 3;
		netPlay.needsInit = true;
		netPlay.ID = -1;
		Game1.hud.scoreBoard.Reset();
		Game1.character = new Character[32];
		playerList = new Dictionary<byte, int>();
		List<SignedInGamer> list = new List<SignedInGamer>();
		list.Add(ie.Gamer);
		joinInviteResult = NetworkSession.BeginJoinInvited((IEnumerable<SignedInGamer>)list, (AsyncCallback)null, (object)null);
		pendingJoinInvite = true;
	}

	public int GetPlayerOne()
	{
		if (netPlay != null)
		{
			if (netType == 1 || netType == 0)
			{
				return 0;
			}
			if (netPlay.ID > -1)
			{
				return netPlay.ID;
			}
		}
		return 0;
	}

	public bool IsHost()
	{
		if (netType == 0 || netType == 1)
		{
			return true;
		}
		if (netSession != null && netSession.IsHost)
		{
			return true;
		}
		return false;
	}

	public bool GetNetworkOwner(int i)
	{
		int playerOne = GetPlayerOne();
		if (netSession == null)
		{
			return true;
		}
		if (netType == 0 || netType == 1)
		{
			return true;
		}
		if (netPlay != null)
		{
			if (playerOne == i)
			{
				return true;
			}
			if (Game1.character[i] == null)
			{
				return false;
			}
			if (netSession.IsHost)
			{
				for (int j = 0; j < ((ReadOnlyCollection<NetworkGamer>)(object)netSession.RemoteGamers).Count; j++)
				{
					if (playerList.ContainsKey(((ReadOnlyCollection<NetworkGamer>)(object)netSession.RemoteGamers)[j].Id) && playerList[((ReadOnlyCollection<NetworkGamer>)(object)netSession.RemoteGamers)[j].Id] == i)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public void Kill()
	{
		Game1.store.Write(0);
		postLobbyFrame = 0f;
		postLobby = false;
		if (netSession == null || netSession.IsDisposed)
		{
			return;
		}
		try
		{
			netSession.Dispose();
			while (!netSession.IsDisposed)
			{
			}
			netSession = null;
		}
		catch (Exception ex)
		{
			failMessage = ex.Message;
		}
	}

	public bool GetHasGold()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		if (netType == 0 || netType == 2 || netType == 1)
		{
			return true;
		}
		if (Game1.mainPlayerIndex < 0)
		{
			return false;
		}
		for (int i = 0; i < ((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count; i++)
		{
			SignedInGamer val = ((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers)[i];
			if ((int)val.PlayerIndex == Game1.mainPlayerIndex && val.Privileges.AllowOnlineSessions)
			{
				return true;
			}
		}
		return false;
	}

	public void CreateSession(int type)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		Kill();
		netType = type;
		List<SignedInGamer> list = new List<SignedInGamer>();
		for (int i = 0; i < ((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count; i++)
		{
			SignedInGamer val = ((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers)[i];
			if ((int)val.PlayerIndex == Game1.mainPlayerIndex)
			{
				list.Add(val);
			}
		}
		NetworkSessionProperties val2 = new NetworkSessionProperties();
		val2[0] = 206;
		val2[1] = GameState.gameType;
		if (list.Count < 1)
		{
			createResult = NetworkSession.BeginCreate((NetworkSessionType)1, 1, 10, 0, val2, (AsyncCallback)null, (object)null);
		}
		else if (netType == 0)
		{
			createResult = NetworkSession.BeginCreate((NetworkSessionType)1, (IEnumerable<SignedInGamer>)list, 10, 0, val2, (AsyncCallback)null, (object)null);
		}
		else
		{
			createResult = NetworkSession.BeginCreate((NetworkSessionType)((netType == 2) ? 1 : 2), (IEnumerable<SignedInGamer>)list, 10, privateMatch ? 9 : 0, val2, (AsyncCallback)null, (object)null);
		}
		pendingCreate = true;
	}

	public void GetSessions(int type)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Invalid comparison between Unknown and I4
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		Kill();
		if (sessions != null)
		{
			sessions.Dispose();
			sessions = null;
		}
		netType = type;
		List<SignedInGamer> list = new List<SignedInGamer>();
		for (int i = 0; i < ((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count; i++)
		{
			SignedInGamer val = ((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers)[i];
			if ((int)val.PlayerIndex == Game1.mainPlayerIndex)
			{
				list.Add(val);
			}
		}
		NetworkSessionProperties val2 = new NetworkSessionProperties();
		if (list.Count < 1)
		{
			findResult = NetworkSession.BeginFind((NetworkSessionType)((netType == 2) ? 1 : 2), 1, val2, (AsyncCallback)null, (object)null);
		}
		else
		{
			findResult = NetworkSession.BeginFind((NetworkSessionType)((netType == 2) ? 1 : 2), (IEnumerable<SignedInGamer>)list, val2, (AsyncCallback)null, (object)null);
		}
		pendingFind = true;
	}

	public void JoinSession(AvailableNetworkSession s)
	{
		Kill();
		joinResult = NetworkSession.BeginJoin(s, (AsyncCallback)null, (object)null);
		pendingJoin = true;
	}

	private void ManageLobby(Character[] c)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between Unknown and I4
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		_ = c[GetPlayerOne()];
		if (postLobbyFrame > 0f)
		{
			Music.Reset();
			postLobbyFrame -= Game1.frameTime;
			if (postLobbyFrame <= 0f)
			{
				if ((int)netSession.SessionState != 1)
				{
					netSession.StartGame();
				}
				netPlay.currentMapListIdx = (netPlay.currentMapListIdx + 1) % MapList.total;
				netPlay.currentMap = MapList.maplist[netPlay.currentMapListIdx];
				Game1.store.Write(0);
				Game1.gameMap.Read(new BinaryReader(File.Open("map/data/" + MapList.mapCatalog[netPlay.currentMap].path + ".zkx", FileMode.Open, FileAccess.Read)));
				Game1.nodeMgr.Refresh(Game1.gameMap);
				c[0] = new Character(netPlay.ID, 0, default(Vector2));
				c[0].SetNewClass();
				c[0].Reset();
				Game1.gameMap.GetSpawn(0, Game1.character[0]);
				for (int i = 0; i < Game1.netSession.BotCount(); i++)
				{
					Game1.character[i + 20] = new Character(i + 20, -1, default(Vector2));
					Game1.character[i + 20].headTex = (Game1.character[i + 20].hatTex = (Game1.character[i + 20].torsoTex = (Game1.character[i + 20].legsTex = 7)));
					Game1.character[i + 20].team = i % 2;
					Game1.character[i + 20].jetpack = 0;
					Game1.gameMap.GetSpawn(0, Game1.character[i + 20]);
				}
				ResetGameStats();
			}
		}
		postLobby = postLobbyFrame > 0f;
		if (postLobby || !(gameLength > 10f))
		{
			return;
		}
		if (GameState.gameType == 2)
		{
			if (redFlagState != 200 && c[redFlagState] == null)
			{
				redFlagState = 200;
			}
			if (blueFlagState != 200 && c[blueFlagState] == null)
			{
				blueFlagState = 200;
			}
		}
		CheckWinner(c);
	}

	private void CheckWinner(Character[] c)
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Invalid comparison between Unknown and I4
		bool flag = false;
		switch (GameState.gameType)
		{
		case 0:
		{
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i] != null && c[i].score >= DMScores[DMScoreIdx])
				{
					flag = true;
				}
			}
			break;
		}
		case 1:
			if (blueScore >= TDMScores[TDMScoreIdx] || redScore >= TDMScores[TDMScoreIdx])
			{
				flag = true;
			}
			break;
		case 4:
			if (blueScore >= ZHScores[ZHScoreIdx] || redScore >= ZHScores[ZHScoreIdx])
			{
				flag = true;
			}
			break;
		case 2:
			if (blueScore >= CTFScores[CTFScoreIdx] || redScore >= CTFScores[CTFScoreIdx])
			{
				flag = true;
			}
			break;
		case 3:
			if (blueTime >= KOTHScores[KOTHScoreIdx] || redTime >= KOTHScores[KOTHScoreIdx])
			{
				flag = true;
			}
			break;
		}
		if (!flag)
		{
			return;
		}
		if ((int)netSession.SessionState == 1)
		{
			try
			{
				netSession.EndGame();
			}
			catch
			{
			}
		}
		postLobbyFrame = 10f;
		redFlagState = 200;
		blueFlagState = 200;
	}

	public void Update(Character[] c)
	{
		gameLength += Game1.frameTime;
		if (netSession != null)
		{
			if (!netSession.IsDisposed)
			{
				for (int i = 0; i < c.Length; i++)
				{
					if (c[i] == null)
					{
						freeSlot = i;
						break;
					}
				}
				try
				{
					netSession.Update();
				}
				catch (Exception e)
				{
					ServerCrashRehost(e);
					return;
				}
				if (netSession.IsHost)
				{
					ManageLobby(c);
				}
				if (((ReadOnlyCollection<NetworkGamer>)(object)netSession.AllGamers).Count >= 1)
				{
					netPlay.Update(netSession, c);
				}
				else
				{
					for (int j = 0; j < c.Length; j++)
					{
						if (c[j] != null)
						{
							c[j].deltaSinceUpdate = 0f;
						}
					}
					Game1.pMan.NetWriteCleanup();
				}
				if (((ReadOnlyCollection<NetworkGamer>)(object)netSession.AllGamers).Count == 0 && GameState.mode == 1 && (netType == 2 || netType == 3))
				{
					GameState.mode = 2;
					Kill();
					Game1.menu.Close();
					Game1.menu.DoError("Game ended!", (netType == 2) ? 5 : 6);
				}
			}
		}
		else if (GameState.mode == 1 && (netType == 2 || netType == 3))
		{
			GameState.mode = 2;
			Game1.menu.Close();
			Game1.menu.DoError("Game ended!", (netType == 2) ? 5 : 6);
		}
		if (pendingCreate && createResult.IsCompleted)
		{
			try
			{
				playerList = new Dictionary<byte, int>();
				Game1.character = new Character[32];
				c = Game1.character;
				netSession = NetworkSession.EndCreate(createResult);
				netSession.AllowJoinInProgress = true;
				netSession.StartGame();
			}
			catch (Exception ex)
			{
				createFailed = true;
				failMessage = ex.Message;
			}
			try
			{
				netSession.GamerJoined += netSession_GamerJoined;
				netSession.GamerLeft += netSession_GamerLeft;
			}
			catch (Exception)
			{
			}
			pendingCreate = false;
		}
		if (pendingFind && findResult.IsCompleted)
		{
			try
			{
				sessions = NetworkSession.EndFind(findResult);
			}
			catch (Exception ex3)
			{
				findFailed = true;
				failMessage = ex3.Message;
			}
			pendingFind = false;
		}
		if (pendingJoin && joinResult.IsCompleted)
		{
			try
			{
				netSession = NetworkSession.EndJoin(joinResult);
				playerList = new Dictionary<byte, int>();
			}
			catch (Exception ex4)
			{
				joinFailed = true;
				failMessage = ex4.Message;
			}
			try
			{
				netSession.GamerJoined += netSession_ClientGamerJoined;
				netSession.GamerLeft += netSession_ClientGamerLeft;
			}
			catch (Exception)
			{
			}
			pendingJoin = false;
		}
		if (!pendingJoinInvite || !joinInviteResult.IsCompleted)
		{
			return;
		}
		try
		{
			netSession = NetworkSession.EndJoinInvited(joinInviteResult);
			playerList = new Dictionary<byte, int>();
			int num = 100;
			if (netSession.SessionProperties[0].HasValue)
			{
				num = netSession.SessionProperties[0].Value;
			}
			if (num != 206)
			{
				netSession.Dispose();
				joinInviteFailed = true;
				failMessage = "Server has different version.";
				if (num > 206)
				{
					newVersAvailable = true;
				}
				GameState.mode = 2;
			}
		}
		catch (Exception ex6)
		{
			joinInviteFailed = true;
			failMessage = ex6.Message;
		}
		try
		{
			netSession.GamerJoined += netSession_ClientGamerJoined;
			netSession.GamerLeft += netSession_ClientGamerLeft;
		}
		catch (Exception)
		{
		}
		pendingJoinInvite = false;
	}

	internal void ServerCrashRehost(Exception e)
	{
		rebootBot = Game1.menu.menuLevel[9].item[5].selX == 1;
		GameState.mode = 2;
		Kill();
		Game1.menu.Close();
		if (e == null)
		{
			Game1.menu.DoError("Game Ended! Unexpected error.", (netType == 2) ? 5 : 6, 1);
		}
		else
		{
			Game1.menu.DoError("Game Ended! Error: " + e.Message, (netType == 2) ? 5 : 6, 1);
		}
	}

	private void netSession_ClientGamerJoined(object sender, GamerJoinedEventArgs e)
	{
		Game1.hud.AddMessage(new StringBuilder(((Gamer)e.Gamer).Gamertag), Message.msgJoined, 0, 0, -1);
	}

	private void netSession_ClientGamerLeft(object sender, GamerLeftEventArgs e)
	{
		Game1.hud.AddMessage(new StringBuilder(((Gamer)e.Gamer).Gamertag), Message.msgQuit, 0, 0, -1);
	}

	private void netSession_GamerJoined(object sender, GamerJoinedEventArgs e)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		freeSlot = -1;
		for (int i = 0; i < 20; i++)
		{
			bool flag = true;
			for (int j = 0; j < ((ReadOnlyCollection<NetworkGamer>)(object)netSession.AllGamers).Count; j++)
			{
				NetworkGamer val = ((ReadOnlyCollection<NetworkGamer>)(object)netSession.AllGamers)[j];
				if (val.Id != e.Gamer.Id && playerList.ContainsKey(val.Id) && playerList[val.Id] == i)
				{
					flag = false;
				}
			}
			if (flag)
			{
				freeSlot = i;
				break;
			}
		}
		NetworkGamer gamer = e.Gamer;
		PacketWriter val2 = new PacketWriter();
		LocalNetworkGamer val3 = ((ReadOnlyCollection<LocalNetworkGamer>)(object)netSession.LocalGamers)[0];
		if (freeSlot == -1)
		{
			NetPacker.WriteMsg(val2, 9);
			NetPacker.WriteByte(val2, 0);
			NetPacker.WriteMsg(val2, 1);
			val3.SendData(val2, (SendDataOptions)1, gamer);
			return;
		}
		NetPacker.WriteMsg(val2, 2);
		NetPacker.WriteByte(val2, freeSlot);
		NetPacker.WriteByte(val2, MapList.maplist[netPlay.currentMapListIdx]);
		NetPacker.WriteByte(val2, GameState.gameType);
		NetPacker.WriteMsg(val2, 1);
		val3.SendData(val2, (SendDataOptions)1, gamer);
		if (playerList.ContainsKey(gamer.Id))
		{
			if (playerList[gamer.Id] != freeSlot)
			{
				playerList[gamer.Id] = freeSlot;
			}
		}
		else
		{
			playerList.Add(gamer.Id, freeSlot);
		}
		Game1.hud.AddMessage(new StringBuilder(((Gamer)e.Gamer).Gamertag), Message.msgJoined, 0, 0, -1);
	}

	private void netSession_GamerLeft(object sender, GamerLeftEventArgs e)
	{
		NetworkGamer gamer = e.Gamer;
		Game1.DestroyChar(playerList[gamer.Id]);
		playerList.Remove(gamer.Id);
		Game1.hud.AddMessage(new StringBuilder(((Gamer)e.Gamer).Gamertag), Message.msgQuit, 0, 0, -1);
	}

	internal void StartServer(Menu menu)
	{
		Game1.netSession.netPlay.needsInit = true;
		Game1.netSession.netPlay.ID = 0;
		Game1.hud.scoreBoard.Reset();
		MapList.Scramble();
		Game1.netSession.netPlay.currentMapListIdx = 0;
		Game1.netSession.netPlay.currentMap = MapList.maplist[Game1.netSession.netPlay.currentMapListIdx];
		Game1.store.Write(0);
		Game1.gameMap.Read(new BinaryReader(File.Open("map/data/" + MapList.mapCatalog[Game1.netSession.netPlay.currentMap].path + ".zkx", FileMode.Open, FileAccess.Read)));
		Game1.nodeMgr.Refresh(Game1.gameMap);
		Game1.netSession.playerList = new Dictionary<byte, int>();
		Game1.netSession.CreateSession(Game1.netSession.netType);
		Game1.character = new Character[32];
		menu.menuLevel[4] = new Lobby(host: true);
		menu.menuLevel[4].active = true;
	}

	internal void ChangeMutator()
	{
		for (int i = 0; i < Game1.character.Length; i++)
		{
			if (Game1.character[i] != null && GetNetworkOwner(i))
			{
				Game1.character[i].Reset();
			}
		}
		if (Mutators.GetCrates(mutator))
		{
			return;
		}
		for (int j = 0; j < Game1.pMan.particle.Length; j++)
		{
			if (Game1.pMan.particle[j].exists && Game1.pMan.particle[j].type == 43)
			{
				Game1.pMan.particle[j].exists = false;
			}
		}
	}

	internal void NullCrash()
	{
		if (IsHost())
		{
			ServerCrashRehost(new Exception("Null'd!!1"));
			return;
		}
		Kill();
		GameState.mode = 2;
		Game1.menu.Close();
		Game1.menu.DoError("Error: disconnected from game.", 6);
	}
}
