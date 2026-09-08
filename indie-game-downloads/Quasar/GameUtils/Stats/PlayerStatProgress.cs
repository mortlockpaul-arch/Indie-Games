using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Storage;

namespace Quasar.GameUtils.Stats;

public class PlayerStatProgress
{
	private const string STATS_PROGRESS_DIR = "Stats/";

	private const string STATS = "Stats";

	private Dictionary<string, StatProgress> stats;

	private string gamertag;

	private bool isDummy;

	private PlayerIndex dummyPlayerIndex;

	public string Gamertag => gamertag;

	public bool IsDummy => isDummy;

	public PlayerIndex DummyPlayerIndex => dummyPlayerIndex;

	public Dictionary<string, StatProgress> Awards => stats;

	public bool SavePending
	{
		get
		{
			foreach (KeyValuePair<string, StatProgress> stat in stats)
			{
				if (stat.Value.SavePending)
				{
					return true;
				}
			}
			return false;
		}
	}

	public PlayerStatProgress(string gamertag)
		: this()
	{
		isDummy = false;
		this.gamertag = gamertag;
	}

	public PlayerStatProgress(PlayerIndex dummyPlayerIndex)
		: this()
	{
		isDummy = true;
		gamertag = Quasar.GameUtils.Player.Player.GuestName;
		this.dummyPlayerIndex = dummyPlayerIndex;
	}

	private PlayerStatProgress()
	{
		stats = new Dictionary<string, StatProgress>(StatsManager.Instance.Stats.Count);
	}

	private void AddProgress(string id, StatProgress award)
	{
		stats.Add(id, award);
	}

	public void IncStatProgress(string id, long progress)
	{
		GetStat(id)?.IncProgress(progress);
	}

	public void IncStatProgress(string id, long progress, int count)
	{
		GetStat(id)?.IncProgress(progress, count);
	}

	public void SetStatProgress(string id, long progress)
	{
		GetStat(id)?.SetProgress(progress);
	}

	public StatProgress GetStat(string id)
	{
		StatProgress value = null;
		if (stats.TryGetValue(id, out value))
		{
			return value;
		}
		return null;
	}

	public void Save(bool highPriority)
	{
		if (highPriority)
		{
			StorageManager.Instance.Exec(saveData, null, StorageManager.IOType.Write);
		}
		else
		{
			StorageManager.Instance.Post(saveData, null, StorageManager.IOType.WriteNotImportant);
		}
	}

	private void saveData(IsolatedStorageFile container, object parameters)
	{
		if (container == null)
		{
			return;
		}
		try
		{
			if (!container.DirectoryExists("Stats/"))
			{
				container.CreateDirectory("Stats/");
			}
			string path = "Stats/" + gamertag + ".xml";
			using Stream stream = container.OpenFile(path, FileMode.Create);
			XDocument xDocument = new XDocument();
			SaveStats(xDocument);
			xDocument.Save(stream);
		}
		catch
		{
		}
	}

	private void SaveStats(XDocument xd)
	{
		XElement xElement = new XElement("Stats");
		xd.Add(xElement);
		foreach (KeyValuePair<string, StatProgress> stat in stats)
		{
			XElement xElement2 = new XElement(stat.Value.Stat.Id);
			stat.Value.ToXml(xElement2);
			stat.Value.SavePending = false;
			xElement.Add(xElement2);
		}
	}

	public static PlayerStatProgress Load(string gamertag)
	{
		PlayerStatProgress playerStatProgress = new PlayerStatProgress(gamertag);
		foreach (KeyValuePair<string, Stat> stat in StatsManager.Instance.Stats)
		{
			StatProgress award = new StatProgress(stat.Value);
			playerStatProgress.AddProgress(stat.Value.Id, award);
		}
		StorageManager.Instance.Exec(loadData, playerStatProgress, StorageManager.IOType.Read);
		return playerStatProgress;
	}

	public static PlayerStatProgress GetDummy(PlayerIndex playerIndex)
	{
		PlayerStatProgress playerStatProgress = new PlayerStatProgress(playerIndex);
		foreach (KeyValuePair<string, Stat> stat in StatsManager.Instance.Stats)
		{
			StatProgress award = new StatProgress(stat.Value);
			playerStatProgress.AddProgress(stat.Value.Id, award);
		}
		return playerStatProgress;
	}

	private static void loadData(IsolatedStorageFile container, object parameters)
	{
		if (!(parameters is PlayerStatProgress playerStatProgress) || container == null)
		{
			return;
		}
		string path = "Stats/" + playerStatProgress.gamertag + ".xml";
		if (!container.FileExists(path))
		{
			return;
		}
		try
		{
			XDocument xDocument = null;
			using (Stream stream = container.OpenFile(path, FileMode.Open))
			{
				xDocument = XDocument.Load(stream);
			}
			foreach (XElement item in xDocument.Elements("Stats"))
			{
				LoadStats(item, playerStatProgress);
			}
		}
		catch (Exception)
		{
		}
	}

	private static void LoadStats(XElement xe, PlayerStatProgress pg)
	{
		foreach (XElement item in xe.Elements())
		{
			pg.GetStat(item.Name.LocalName)?.FromXml(item);
		}
	}
}
