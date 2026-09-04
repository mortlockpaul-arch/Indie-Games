using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using SpaceBlast.AI;
using SpaceBlast.Networking;

namespace SpaceBlast;

internal class PlayerList
{
	private Dictionary<byte, Player> m_Players = new Dictionary<byte, Player>();

	private Dictionary<ETeam, int> m_TeamScores = new Dictionary<ETeam, int>();

	public Dictionary<byte, Player> PlayerMap => m_Players;

	public int Count => m_Players.Count;

	public PlayerList()
	{
		Clear();
	}

	public byte AddHumanPlayer(Gamer gamer, ShipColor? col, ETeam? team, bool primaryPlayer)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		byte b = (byte)m_Players.Count;
		ShipColor colour = (ShipColor)b;
		if (col.HasValue)
		{
			colour = col.Value;
		}
		RespawnLocation playerStartPosition = MainGame.LevelData.GetPlayerStartPosition(b);
		ETeam eTeam = (team.HasValue ? team.Value : ETeam.None);
		HumanPlayer value = new HumanPlayer(b, playerStartPosition.Position, gamer, colour, eTeam, primaryPlayer);
		m_Players[b] = value;
		if (eTeam != ETeam.None && !m_TeamScores.ContainsKey(eTeam))
		{
			m_TeamScores.Add(eTeam, 0);
		}
		return b;
	}

	public byte AddLocalPlayer(LocalNetworkGamer gamer, ShipColor? col, ETeam? team)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		byte id = ((NetworkGamer)gamer).Id;
		int count = m_Players.Count;
		ShipColor colour = (ShipColor)count;
		if (col.HasValue)
		{
			colour = col.Value;
		}
		RespawnLocation playerStartPosition = MainGame.LevelData.GetPlayerStartPosition(count);
		ETeam eTeam = (team.HasValue ? team.Value : ETeam.None);
		HumanPlayer value = new HumanPlayer(id, playerStartPosition.Position, (Gamer)(object)gamer, colour, eTeam, primary: true);
		m_Players[id] = value;
		if (eTeam != ETeam.None && !m_TeamScores.ContainsKey(eTeam))
		{
			m_TeamScores.Add(eTeam, 0);
		}
		return id;
	}

	public byte AddRemotePlayer(NetworkGamer gamer, ShipColor? col, ETeam? team)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		byte id = gamer.Id;
		int count = m_Players.Count;
		ShipColor colour = (ShipColor)count;
		if (col.HasValue)
		{
			colour = col.Value;
		}
		RespawnLocation playerStartPosition = MainGame.LevelData.GetPlayerStartPosition(count);
		ETeam eTeam = (team.HasValue ? team.Value : ETeam.None);
		RemotePlayer value = new RemotePlayer(id, playerStartPosition.Position, (Gamer)(object)gamer, colour, eTeam);
		m_Players[id] = value;
		if (eTeam != ETeam.None && !m_TeamScores.ContainsKey(eTeam))
		{
			m_TeamScores.Add(eTeam, 0);
		}
		return id;
	}

	public byte AddRemotePlayer(NetworkGamer gamer, ShipColor? col, ETeam? team, Vector3 pos, float angle)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		byte id = gamer.Id;
		int count = m_Players.Count;
		ShipColor colour = (ShipColor)count;
		if (col.HasValue)
		{
			colour = col.Value;
		}
		ETeam eTeam = (team.HasValue ? team.Value : ETeam.None);
		RemotePlayer remotePlayer = new RemotePlayer(id, pos, (Gamer)(object)gamer, colour, eTeam);
		m_Players[id] = remotePlayer;
		remotePlayer.TheShip.Rotation = angle;
		if (eTeam != ETeam.None && !m_TeamScores.ContainsKey(eTeam))
		{
			m_TeamScores.Add(eTeam, 0);
		}
		return id;
	}

	public byte AddAIPlayer(ShipColor? col, ETeam? team, AISkill skill)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		byte b = (byte)m_Players.Count;
		ShipColor colour = (ShipColor)b;
		if (col.HasValue)
		{
			colour = col.Value;
		}
		RespawnLocation playerStartPosition = MainGame.LevelData.GetPlayerStartPosition(b);
		ETeam eTeam = (team.HasValue ? team.Value : ETeam.None);
		AIPlayer value = new AIPlayer(b, skill, playerStartPosition.Position, colour, eTeam);
		m_Players[b] = value;
		if (eTeam != ETeam.None && !m_TeamScores.ContainsKey(eTeam))
		{
			m_TeamScores.Add(eTeam, 0);
		}
		return b;
	}

	public Player GetPlayer(byte id)
	{
		return m_Players[id];
	}

	public bool DoesPlayerExist(byte id)
	{
		return m_Players.ContainsKey(id);
	}

	public List<Player> GetPlayers()
	{
		List<Player> list = new List<Player>();
		foreach (Player value in m_Players.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public Player GetCheckedPlayer(byte id)
	{
		if (m_Players.ContainsKey(id))
		{
			return m_Players[id];
		}
		throw new PlayerDoesntExistException(id);
	}

	public void DeletePlayer(byte id)
	{
		m_Players.Remove(id);
	}

	public void Update()
	{
		foreach (Player value in m_Players.Values)
		{
			value.Update();
		}
	}

	public void Draw(byte owningplayerid)
	{
		foreach (KeyValuePair<byte, Player> player in m_Players)
		{
			player.Value.Draw(player.Key == owningplayerid);
		}
	}

	public void Clear()
	{
		foreach (Player value in m_Players.Values)
		{
			if (value is AIPlayer)
			{
				((AIPlayer)value).Terminate();
			}
		}
		m_Players.Clear();
		m_TeamScores.Clear();
	}

	public void ResetTeamScores()
	{
		foreach (ETeam key in m_TeamScores.Keys)
		{
			m_TeamScores[key] = 0;
		}
	}

	public float FindNearestVisibleEnemy(Player player, out Player enemy)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = player.TheShip.Position;
		Player player2 = null;
		float num = float.MaxValue;
		foreach (Player value in m_Players.Values)
		{
			if (value != null && !object.ReferenceEquals(player, value) && (player.Team == ETeam.None || value.Team != player.Team) && value.IsActive && !value.IsCloakActive)
			{
				Vector3 val = value.TheShip.Position - position;
				float num2 = ((Vector3)(ref val)).Length();
				if (num2 < num)
				{
					num = num2;
					player2 = value;
				}
			}
		}
		enemy = player2;
		return num;
	}

	public void IncreasePlayerScore(Player killedby, Player deadplayer)
	{
		if (killedby.Team != ETeam.None && killedby.Team == deadplayer.Team)
		{
			return;
		}
		killedby.Kills++;
		if (killedby.Team != ETeam.None)
		{
			m_TeamScores[killedby.Team]++;
		}
		bool flag = false;
		if (!MainGame.IsDemoMode)
		{
			if (killedby.Kills >= GameConstants.KillsToWin)
			{
				flag = true;
			}
			else if (killedby.Team != ETeam.None && m_TeamScores[killedby.Team] >= GameConstants.TeamKillsToWin)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (MainGame.NetMan.IsNetworkGame)
			{
				if (MainGame.NetMan.IsHost)
				{
					MainGame.NetMan.SetGameStatus(GameStatus.GameOver);
					MainGame.NetMan.SendGameOverPacket();
					MainGame.Instance.ShowGameOverPage();
				}
			}
			else
			{
				MainGame.Instance.ShowGameOverPage();
			}
		}
		else if (killedby.Kills >= GameConstants.KillsToNearbyOverThreshold && MainGame.NetMan.IsNetworkGame && MainGame.NetMan.IsHost)
		{
			MainGame.NetMan.SetGameStatus(GameStatus.NearlyFinished);
		}
	}

	public int GetTeamScore(ETeam team)
	{
		return m_TeamScores[team];
	}

	public static string GetTeamName(ETeam team)
	{
		string result = "";
		switch (team)
		{
		case ETeam.None:
			result = "None";
			break;
		case ETeam.Red:
			result = "Red";
			break;
		case ETeam.Orange:
			result = "Orange";
			break;
		case ETeam.Yellow:
			result = "Yellow";
			break;
		case ETeam.Green:
			result = "Green";
			break;
		case ETeam.Cyan:
			result = "Cyan";
			break;
		case ETeam.Blue:
			result = "Blue";
			break;
		case ETeam.Purple:
			result = "Purple";
			break;
		}
		return result;
	}
}
