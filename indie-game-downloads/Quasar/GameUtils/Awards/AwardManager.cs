using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.ContentPipeline;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.Global;

namespace Quasar.GameUtils.Awards;

public class AwardManager
{
	public delegate void AwardEvent(PlayerAwardProgress progress, AwardProgress award);

	private const int SAVE_INTERVAL = 30000;

	protected static AwardManager instance;

	private Dictionary<string, Award> awards;

	private List<PlayerAwardProgress> progress;

	private long lastSave;

	public static AwardManager Instance => instance;

	public Dictionary<string, Award> Awards => awards;

	public event AwardEvent OnAchievementUnlocked;

	public event AwardEvent OnAchievementNotify;

	public static void Init()
	{
		instance = new AwardManager();
		instance.Initialize();
	}

	public PlayerAwardProgress GetPlayerProgress(PlayerIndex index)
	{
		return progress[(int)index];
	}

	protected AwardManager()
	{
		awards = new Dictionary<string, Award>(15);
		LoadAwards();
		progress = new List<PlayerAwardProgress>(4);
		for (int i = 0; i < 4; i++)
		{
			progress.Add(null);
		}
	}

	protected void Initialize()
	{
		for (int i = 0; i < 4; i++)
		{
			SwitchPlayer((PlayerIndex)i);
		}
		PlatformInterface platformInterface = PlatformInterface.Instance;
		platformInterface.SignedIn = (Action<PlayerIndex, ISignedInGamer>)Delegate.Combine(platformInterface.SignedIn, new Action<PlayerIndex, ISignedInGamer>(OnSignIn));
		PlatformInterface platformInterface2 = PlatformInterface.Instance;
		platformInterface2.SignedOut = (Action<PlayerIndex, ISignedInGamer>)Delegate.Combine(platformInterface2.SignedOut, new Action<PlayerIndex, ISignedInGamer>(OnSignOut));
	}

	private void LoadAwards()
	{
		try
		{
			XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("Awards");
			XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
			foreach (XElement item in xDocument.Root.Elements())
			{
				Award award = CreateAward();
				award.FromXml(item);
				awards.Add(award.Id, award);
			}
		}
		catch (Exception)
		{
		}
	}

	protected virtual Award CreateAward()
	{
		return new Award();
	}

	public void IncAwardProgress(PlayerIndex playerIndex, string id)
	{
		IncAwardProgress(playerIndex, id, 1u);
	}

	public void IncAwardProgress(PlayerIndex playerIndex, string id, uint progress)
	{
		PlayerAwardProgress playerAwardProgress = this.progress[(int)playerIndex];
		if (playerAwardProgress != null)
		{
			AwardProgress awardProgress;
			switch (playerAwardProgress.IncAwardProgress(id, progress, out awardProgress))
			{
			case AwardResult.Notify:
				InvokeNotify(playerAwardProgress, awardProgress);
				break;
			case AwardResult.Unlocked:
				InvokeUnlocked(playerAwardProgress, awardProgress);
				break;
			}
		}
	}

	public void UnlockAward(PlayerIndex playerIndex, string id)
	{
		PlayerAwardProgress playerAwardProgress = progress[(int)playerIndex];
		if (playerAwardProgress != null)
		{
			AwardResult awardResult = playerAwardProgress.UnlockAward(id, out var awardProgress);
			if (awardResult == AwardResult.Unlocked)
			{
				InvokeUnlocked(playerAwardProgress, awardProgress);
			}
		}
	}

	private void InvokeNotify(PlayerAwardProgress pg, AwardProgress award)
	{
		if (OnAchievementNotify != null)
		{
			OnAchievementNotify(pg, award);
		}
	}

	protected virtual void InvokeUnlocked(PlayerAwardProgress pg, AwardProgress award)
	{
		if (OnAchievementUnlocked != null)
		{
			OnAchievementUnlocked(pg, award);
		}
	}

	public void Update()
	{
		long totalTime = Timer.DefaultTimer.TotalTime;
		if (totalTime <= lastSave + 30000)
		{
			return;
		}
		foreach (PlayerAwardProgress item in progress)
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
		foreach (PlayerAwardProgress item in progress)
		{
			if (item != null && item.SavePending)
			{
				item.Save(highPriority: true);
			}
		}
	}

	private void OnSignIn(PlayerIndex playerIndex, ISignedInGamer g)
	{
		SwitchPlayer(playerIndex);
	}

	private void OnSignOut(PlayerIndex playerIndex, ISignedInGamer g)
	{
		SwitchPlayer(playerIndex);
	}

	private void SwitchPlayer(PlayerIndex playerIndex)
	{
		if (progress[(int)playerIndex] != null)
		{
			progress[(int)playerIndex].Save(highPriority: false);
			if (progress[(int)playerIndex].Gamertag == Quasar.GameUtils.Player.Player.GetPlayerName(playerIndex))
			{
				return;
			}
		}
		progress[(int)playerIndex] = PlayerAwardProgress.Load(playerIndex);
		CheckProgress(progress[(int)playerIndex]);
	}

	protected virtual void CheckProgress(PlayerAwardProgress pg)
	{
	}
}
