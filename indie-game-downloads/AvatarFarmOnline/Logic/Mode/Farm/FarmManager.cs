using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Quasar.GameUtils.Storage;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Mode.Farm;

internal class FarmManager
{
	public const string SAVE_FILE = "farms.xml";

	private AvatarFarmOnline.Logic.Mode.Farm.PlayerExperience playerExperience = new AvatarFarmOnline.Logic.Mode.Farm.PlayerExperience();

	private List<AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader> farms = new List<AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader>();

	private AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData persistentData;

	public AvatarFarmOnline.Logic.Mode.Farm.PlayerExperience PlayerExperience => playerExperience;

	public List<AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader> Farms => farms;

	public bool HasImportedFarm
	{
		get
		{
			foreach (AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader farm in farms)
			{
				if (farm.IsImported)
				{
					return true;
				}
			}
			return false;
		}
	}

	public PlayerIndex PlayerIndex => persistentData.Setup.PlayerIndex;

	public FarmManager(AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData fpgd)
	{
		persistentData = fpgd;
		StorageManager.Instance.Exec(fpgd.Setup.PlayerIndex, Init, null, StorageManager.IOType.Read);
	}

	public void SortFarms()
	{
		farms.Sort(CompareFarms);
	}

	private int CompareFarms(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader h1, AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader h2)
	{
		return h2.LastSave.CompareTo(h1.LastSave);
	}

	public string GetDefaultFarmName()
	{
		string text = "My Farm";
		bool flag = true;
		int num = 0;
		do
		{
			text = "My Farm";
			if (num > 0)
			{
				text = text + " " + (num + 1);
			}
			string text2 = GameMath.RemoveSpecialChars(text);
			string fileName = GameMath.GetFilenameFromString(text2) + ".xml";
			flag = CheckNames(text2, fileName);
			num++;
		}
		while (!flag);
		return text;
	}

	public bool CreateFarm(string name, out AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader result)
	{
		if (name.Length > 12)
		{
			name = name.Remove(12);
		}
		string text = GameMath.RemoveSpecialChars(name);
		string text2 = GameMath.GetFilenameFromString(text) + ".xml";
		if (!CheckNames(text, text2))
		{
			result = null;
			return false;
		}
		result = new AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader(text, text2, this);
		lock (farms)
		{
			farms.Add(result);
		}
		SortFarms();
		Save();
		return true;
	}

	public bool ImportFarm(string name, uint xp, uint coins, uint cash, out AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader result)
	{
		if (name.Length > 12)
		{
			name = name.Remove(12);
		}
		string text = GameMath.RemoveSpecialChars(name);
		string text2 = GameMath.GetFilenameFromString(text) + ".xml";
		if (!CheckNames(text, text2))
		{
			result = null;
			return false;
		}
		result = new AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader(text, text2, (int)xp, (int)coins, (int)cash, this);
		lock (farms)
		{
			farms.Add(result);
		}
		SortFarms();
		Save();
		return true;
	}

	private bool CheckNames(string realName, string fileName)
	{
		if (realName.Length == 0 || fileName.Length == 0)
		{
			return false;
		}
		foreach (AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader farm in farms)
		{
			if (farm.Name.Equals(realName))
			{
				return false;
			}
			if (farm.Filename.Equals(fileName))
			{
				return false;
			}
		}
		return true;
	}

	public bool DeleteFarm(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader header)
	{
		if (farms.Contains(header))
		{
			header.Delete();
			lock (farms)
			{
				farms.Remove(header);
			}
			SortFarms();
			Save();
			return true;
		}
		return false;
	}

	private void Init(StorageContainer container, object parameters)
	{
		if (container != null)
		{
			if (container.FileExists("farms.xml"))
			{
				TryLoad(container, "farms.xml");
			}
			else
			{
				Init();
			}
		}
		else
		{
			Init();
		}
	}

	private bool TryLoad(StorageContainer container, string fileName)
	{
		try
		{
			if (container.FileExists(fileName))
			{
				using (Stream stream = container.OpenFile(fileName, FileMode.Open))
				{
					XDocument xDocument = XDocument.Load(stream);
					farms.Clear();
					XElement xElement = xDocument.Root.Element("Player");
					if (xElement != null)
					{
						playerExperience.ParseXml(xElement);
					}
					foreach (XElement item in xDocument.Root.Elements("Farm"))
					{
						AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader localFarmHeader = AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader.Parse(item, this);
						if (localFarmHeader != null)
						{
							farms.Add(localFarmHeader);
						}
					}
					SortFarms();
					return true;
				}
			}
		}
		catch (Exception)
		{
			return false;
		}
		return false;
	}

	public void Save()
	{
		StorageManager.Instance.Post(persistentData.Setup.PlayerIndex, Save, null, StorageManager.IOType.Write);
	}

	private void Save(StorageContainer container, object parameters)
	{
		if (container == null)
		{
			return;
		}
		string file = "farms.xml";
		try
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement("Farms");
			XElement xElement2 = new XElement("Player");
			playerExperience.ToXml(xElement2);
			xElement.Add(xElement2);
			lock (farms)
			{
				foreach (AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader farm in farms)
				{
					XElement xElement3 = new XElement("Farm");
					farm.ToXml(xElement3);
					xElement.Add(xElement3);
				}
			}
			xDocument.Add(xElement);
			using Stream stream = container.OpenFile(file, FileMode.Create);
			xDocument.Save(stream);
			stream.Close();
		}
		catch (Exception)
		{
		}
	}

	private void Init()
	{
		farms.Clear();
		playerExperience.Init();
	}
}
