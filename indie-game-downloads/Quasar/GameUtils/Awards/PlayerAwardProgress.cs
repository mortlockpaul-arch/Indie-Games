using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Storage;

namespace Quasar.GameUtils.Awards;

public class PlayerAwardProgress
{
	private const string AWARDS_PROGRESS_DIR = "Awards/";

	private const string AWARDS = "Awards";

	private Dictionary<string, AwardProgress> awards;

	private string gamertag;

	private bool isDummy;

	private PlayerIndex playerIndex;

	public string Gamertag => gamertag;

	public bool IsDummy => isDummy;

	public PlayerIndex PlayerIndex => playerIndex;

	public Dictionary<string, AwardProgress> Awards => awards;

	public bool SavePending
	{
		get
		{
			foreach (KeyValuePair<string, AwardProgress> award in awards)
			{
				if (award.Value.SavePending)
				{
					return true;
				}
			}
			return false;
		}
	}

	public PlayerAwardProgress(PlayerIndex playerIndex, bool isDummy)
	{
		this.isDummy = isDummy;
		gamertag = (isDummy ? Quasar.GameUtils.Player.Player.GuestName : Quasar.GameUtils.Player.Player.GetPlayerName(playerIndex));
		this.playerIndex = playerIndex;
		awards = new Dictionary<string, AwardProgress>(AwardManager.Instance.Awards.Count);
	}

	private void AddProgress(string id, AwardProgress award)
	{
		awards.Add(id, award);
	}

	public AwardResult IncAwardProgress(string id, uint progress, out AwardProgress awardProgress)
	{
		return (awardProgress = GetAward(id))?.IncProgress(progress) ?? AwardResult.None;
	}

	public AwardResult UnlockAward(string id, out AwardProgress awardProgress)
	{
		return (awardProgress = GetAward(id))?.Unlock() ?? AwardResult.None;
	}

	public AwardProgress GetAward(string id)
	{
		AwardProgress value = null;
		if (awards.TryGetValue(id, out value))
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
			if (!container.DirectoryExists("Awards/"))
			{
				container.CreateDirectory("Awards/");
			}
			string path = "Awards/" + gamertag + ".xml";
			using Stream stream = container.OpenFile(path, FileMode.Create);
			XDocument xDocument = new XDocument();
			SaveAwards(xDocument);
			xDocument.Save(stream);
		}
		catch
		{
		}
	}

	private void SaveAwards(XDocument xd)
	{
		XElement xElement = new XElement("Awards");
		xd.Add(xElement);
		foreach (KeyValuePair<string, AwardProgress> award in awards)
		{
			XElement xElement2 = new XElement(award.Value.Award.Id);
			award.Value.ToXml(xElement2);
			award.Value.SavePending = false;
			xElement.Add(xElement2);
		}
	}

	public static PlayerAwardProgress Load(PlayerIndex playerIndex)
	{
		bool flag = !Quasar.GameUtils.Player.Player.IsSignedIn(playerIndex);
		PlayerAwardProgress playerAwardProgress = new PlayerAwardProgress(playerIndex, flag);
		foreach (KeyValuePair<string, Award> award2 in AwardManager.Instance.Awards)
		{
			AwardProgress award = new AwardProgress(award2.Value);
			playerAwardProgress.AddProgress(award2.Value.Id, award);
		}
		if (!flag)
		{
			StorageManager.Instance.Exec(loadData, playerAwardProgress, StorageManager.IOType.Read);
		}
		return playerAwardProgress;
	}

	private static void loadData(IsolatedStorageFile container, object parameters)
	{
		if (!(parameters is PlayerAwardProgress playerAwardProgress) || container == null)
		{
			return;
		}
		string path = "Awards/" + playerAwardProgress.gamertag + ".xml";
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
			foreach (XElement item in xDocument.Root.Elements())
			{
				LoadAward(item, playerAwardProgress);
			}
		}
		catch (Exception)
		{
		}
	}

	private static void LoadAward(XElement xe, PlayerAwardProgress pg)
	{
		pg.GetAward(xe.Name.LocalName)?.FromXml(xe);
	}
}
