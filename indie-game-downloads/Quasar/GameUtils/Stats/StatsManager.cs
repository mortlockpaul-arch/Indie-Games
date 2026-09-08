using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.ContentPipeline;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Storage;
using Quasar.Global;

namespace Quasar.GameUtils.Stats;

public class StatsManager
{
	private const int SAVE_INTERVAL = 30000;

	private static StatsManager instance;

	private Dictionary<string, Stat> stats;

	private List<PlayerStatProgress> progress;

	private long lastSave;

	public static StatsManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new StatsManager();
				instance.Initialize();
			}
			return instance;
		}
	}

	public Dictionary<string, Stat> Stats => stats;

	public PlayerStatProgress GetPlayerProgress(PlayerIndex index)
	{
		return progress[(int)index];
	}

	private StatsManager()
	{
		stats = new Dictionary<string, Stat>(15);
		LoadStats();
		progress = new List<PlayerStatProgress>(4);
		for (int i = 0; i < 4; i++)
		{
			progress.Add(null);
		}
	}

	private void Initialize()
	{
		for (int i = 0; i < 4; i++)
		{
			if (Quasar.GameUtils.Player.Player.IsSignedIn((PlayerIndex)i))
			{
				progress[i] = PlayerStatProgress.Load(Quasar.GameUtils.Player.Player.GetPlayerName((PlayerIndex)i));
			}
			else
			{
				progress[i] = PlayerStatProgress.GetDummy((PlayerIndex)i);
			}
		}
		PlatformInterface platformInterface = PlatformInterface.Instance;
		platformInterface.SignedIn = (Action<PlayerIndex, ISignedInGamer>)Delegate.Combine(platformInterface.SignedIn, new Action<PlayerIndex, ISignedInGamer>(OnSignIn));
		PlatformInterface platformInterface2 = PlatformInterface.Instance;
		platformInterface2.SignedOut = (Action<PlayerIndex, ISignedInGamer>)Delegate.Combine(platformInterface2.SignedOut, new Action<PlayerIndex, ISignedInGamer>(OnSignOut));
	}

	private void LoadStats()
	{
		try
		{
			XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("Stats");
			XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
			foreach (XElement item in xDocument.Root.Elements())
			{
				Stat stat = Stat.FromXml(item);
				stats.Add(stat.Id, stat);
			}
		}
		catch (Exception)
		{
		}
	}

	public void SetStatProgress(PlayerIndex playerIndex, string id, long progress)
	{
		this.progress[(int)playerIndex]?.SetStatProgress(id, progress);
	}

	public void IncStatProgress(PlayerIndex playerIndex, string id)
	{
		IncStatProgress(playerIndex, id, 1L);
	}

	public void IncStatProgress(PlayerIndex playerIndex, string id, long progress)
	{
		this.progress[(int)playerIndex]?.IncStatProgress(id, progress);
	}

	public void IncStatProgress(PlayerIndex playerIndex, string id, long progress, int count)
	{
		this.progress[(int)playerIndex]?.IncStatProgress(id, progress, count);
	}

	public void Update()
	{
		long totalTime = Timer.DefaultTimer.TotalTime;
		if (totalTime <= lastSave + 30000 || !StorageManager.Instance.SaveEnabled)
		{
			return;
		}
		foreach (PlayerStatProgress item in progress)
		{
			if (item != null && item.SavePending)
			{
				item.Save(highPriority: false);
				lastSave = totalTime;
			}
		}
	}

	public void Save()
	{
		foreach (PlayerStatProgress item in progress)
		{
			if (item != null && item.SavePending)
			{
				item.Save(highPriority: true);
			}
		}
	}

	private void OnSignIn(PlayerIndex playerIndex, ISignedInGamer g)
	{
		Load(playerIndex);
	}

	private void OnSignOut(PlayerIndex playerIndex, ISignedInGamer g)
	{
		SaveAndClear(playerIndex);
	}

	private void Load(PlayerIndex playerIndex)
	{
		if (progress[(int)playerIndex] != null)
		{
			if (!progress[(int)playerIndex].IsDummy)
			{
				progress[(int)playerIndex].Save(highPriority: false);
			}
			if (progress[(int)playerIndex].Gamertag == Quasar.GameUtils.Player.Player.GetPlayerName(playerIndex))
			{
				return;
			}
		}
		if (Quasar.GameUtils.Player.Player.IsSignedIn(playerIndex))
		{
			progress[(int)playerIndex] = PlayerStatProgress.Load(Quasar.GameUtils.Player.Player.GetPlayerName(playerIndex));
		}
		else
		{
			progress[(int)playerIndex] = null;
		}
	}

	private void SaveAndClear(PlayerIndex playerIndex)
	{
		if (progress[(int)playerIndex] != null && !progress[(int)playerIndex].IsDummy)
		{
			progress[(int)playerIndex].Save(highPriority: false);
		}
		progress[(int)playerIndex] = PlayerStatProgress.GetDummy(playerIndex);
	}
}
