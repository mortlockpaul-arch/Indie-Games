using System;
using System.Xml.Linq;
using AvatarFarmOnline.Logic.Stage;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Mode.Farm;

internal class LocalFarmHeader : AvatarFarmOnline.Logic.Mode.Farm.FarmHeader
{
	private string name;

	private string filename;

	private bool isImported;

	private AvatarFarmOnline.Logic.Mode.Farm.FarmManager farmManager;

	private DateTime lastSave;

	protected AvatarFarmOnline.Logic.SimulationSpeeds simulationSpeed;

	private Vector2 playerPosition = AvatarFarmOnline.Logic.GameGlobals.InitialFarmSize * 2f * 0.5f;

	public string Name => name;

	public string Filename => filename;

	public bool IsImported => isImported;

	public AvatarFarmOnline.Logic.Mode.Farm.FarmManager Manager => farmManager;

	public DateTime LastSave => lastSave;

	public AvatarFarmOnline.Logic.SimulationSpeeds SimulationSpeed
	{
		get
		{
			return simulationSpeed;
		}
		set
		{
			simulationSpeed = value;
		}
	}

	public Vector2 PlayerPosition => playerPosition;

	public LocalFarmHeader(string name, string filename, AvatarFarmOnline.Logic.Mode.Farm.FarmManager farmManager)
	{
		this.farmManager = farmManager;
		this.name = name;
		this.filename = filename;
		isImported = false;
		Initialize();
	}

	public LocalFarmHeader(string name, string filename, int xp, int coins, int cash, AvatarFarmOnline.Logic.Mode.Farm.FarmManager farmManager)
	{
		this.farmManager = farmManager;
		this.name = name;
		this.filename = filename;
		isImported = true;
		Initialize(xp, coins, cash);
	}

	private LocalFarmHeader(AvatarFarmOnline.Logic.Mode.Farm.FarmManager farmManager)
	{
		this.farmManager = farmManager;
	}

	public void SetLevel(int level, int coins, int cash)
	{
		base.level = level;
		base.coins = coins;
		base.cash = cash;
		lastSave = DateTime.Now;
		Manager.SortFarms();
	}

	public void SetPlayerPosition(Vector2 pos)
	{
		playerPosition = pos;
	}

	private void Initialize()
	{
		level = 1;
		playMode = PlayMode.Local;
		if (GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData)
		{
			ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(farmPersistentGameData.Setup.PlayerIndex);
			if (gamer != null && gamer.IsSignedInToService && gamer.AllowOnlineSessions)
			{
				playMode = PlayMode.Public;
				friendPermissions = AvatarFarmOnline.Logic.PlayerPermissions.Full;
				publicPermissions = AvatarFarmOnline.Logic.PlayerPermissions.Harvest;
			}
		}
		simulationSpeed = AvatarFarmOnline.Logic.SimulationSpeeds.Normal;
		lastSave = DateTime.Now;
		AvatarFarmOnline.Logic.Stage.FarmData.InitializeFarm(this);
	}

	private void Initialize(int xp, int coins, int cash)
	{
		level = 1;
		playMode = PlayMode.Local;
		if (GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData)
		{
			ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(farmPersistentGameData.Setup.PlayerIndex);
			if (gamer != null && gamer.IsSignedInToService && gamer.AllowOnlineSessions)
			{
				playMode = PlayMode.Public;
				friendPermissions = AvatarFarmOnline.Logic.PlayerPermissions.Full;
				publicPermissions = AvatarFarmOnline.Logic.PlayerPermissions.Harvest;
			}
		}
		simulationSpeed = AvatarFarmOnline.Logic.SimulationSpeeds.Normal;
		lastSave = DateTime.Now;
		AvatarFarmOnline.Logic.Stage.FarmData.InitializeFarm(this, xp, coins, cash);
	}

	public void Delete()
	{
		AvatarFarmOnline.Logic.Stage.FarmData.DeleteFarm(this);
	}

	public override AvatarFarmOnline.Logic.Stage.FarmData LoadFarm()
	{
		return new AvatarFarmOnline.Logic.Stage.FarmData(this);
	}

	public void ToXml(XElement xe)
	{
		xe.SetAttribute("name", name);
		xe.SetAttribute("filename", filename);
		xe.SetIntAttribute("level", level);
		xe.SetIntAttribute("coins", coins);
		xe.SetIntAttribute("cash", cash);
		xe.SetIntAttribute("playMode", (int)playMode);
		xe.SetIntAttribute("friendPermissions", (int)friendPermissions);
		xe.SetIntAttribute("publicPermissions", (int)publicPermissions);
		xe.SetIntAttribute("simulationSpeed", (int)simulationSpeed);
		xe.SetBoolAttribute("imported", isImported);
		xe.SetVector2Attribute("playerPosition", playerPosition);
		xe.SetLongAttribute("last_save", lastSave.Ticks);
	}

	public static AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader Parse(XElement xe, AvatarFarmOnline.Logic.Mode.Farm.FarmManager manager)
	{
		AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader localFarmHeader = new AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader(manager);
		localFarmHeader.name = xe.GetAttribute("name");
		localFarmHeader.filename = xe.GetAttribute("filename");
		localFarmHeader.level = xe.ParseIntAttribute("level");
		localFarmHeader.playMode = (PlayMode)xe.ParseIntAttribute("playMode");
		localFarmHeader.friendPermissions = (AvatarFarmOnline.Logic.PlayerPermissions)xe.ParseIntAttribute("friendPermissions");
		localFarmHeader.publicPermissions = (AvatarFarmOnline.Logic.PlayerPermissions)xe.ParseIntAttribute("publicPermissions");
		localFarmHeader.simulationSpeed = (AvatarFarmOnline.Logic.SimulationSpeeds)xe.ParseIntAttribute("simulationSpeed", 4);
		localFarmHeader.isImported = xe.ParseBoolAttribute("imported");
		localFarmHeader.playerPosition = xe.ParseVector2Attribute("playerPosition", AvatarFarmOnline.Logic.GameGlobals.InitialFarmSize * 2f * 0.5f);
		localFarmHeader.coins = xe.ParseIntAttribute("coins");
		localFarmHeader.cash = xe.ParseIntAttribute("cash");
		localFarmHeader.lastSave = new DateTime(xe.ParseLongAttribute("last_save"));
		return localFarmHeader;
	}
}
