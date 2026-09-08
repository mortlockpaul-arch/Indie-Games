using System;
using System.Collections.Generic;
using AvatarFarmOnline.Logic.Stage.Definition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Logic.Mode;
using Quasar.Global;
using Quasar.Language;

namespace AvatarFarmOnline.Logic.Stage;

internal class Stage : IStage, IDisposable
{
	private const long STAGE_START_COUNTDOWN = 3000L;

	private StageState state;

	private Timer stageTimer = new Timer();

	private World world;

	private AvatarFarmOnline.Logic.Stage.FarmData farmData;

	private Body worldBody;

	private List<AvatarFarmOnline.Logic.Stage.Player> players = new List<AvatarFarmOnline.Logic.Stage.Player>(4);

	private long lastSave;

	private AvatarFarmOnline.Logic.Stage.LocalPlayer localPlayer;

	private bool isShowingMessage;

	public StageState StageState => state;

	public bool IsInPlayableState => state == StageState.Playing;

	public bool IsOnShop => state == StageState.Shop;

	public Timer Timer => stageTimer;

	public World World => world;

	public AvatarFarmOnline.Logic.Stage.FarmData FarmData => farmData;

	public List<AvatarFarmOnline.Logic.Stage.Player> Players => players;

	public float TimeScale
	{
		get
		{
			return stageTimer.TimeScale;
		}
		set
		{
			stageTimer.TimeScale = value;
		}
	}

	public long LastSave => lastSave;

	public AvatarFarmOnline.Logic.Stage.LocalPlayer LocalPlayer => localPlayer;

	public bool IsShowingMessage => isShowingMessage;

	public bool IsPaused => state == StageState.Paused;

	public bool IsFinished => state == StageState.Finished;

	public event Action<AvatarFarmOnline.Logic.Stage.Stage> OnStageStart;

	public event Action<AvatarFarmOnline.Logic.Stage.Player> OnPlayerRemoved;

	public event Action<AvatarFarmOnline.Logic.Stage.Player> OnPlayerAdded;

	public event Action<AvatarFarmOnline.Logic.Stage.Stage> OnGameFinished;

	public event Action<AvatarFarmOnline.Logic.Stage.Stage, bool> OnShowShop;

	public event Action<AvatarFarmOnline.Logic.Stage.Stage, string, string> OnShowMessage;

	public AvatarFarmOnline.Logic.Stage.Player GetPlayerById(int id)
	{
		foreach (AvatarFarmOnline.Logic.Stage.Player player in players)
		{
			if (player.Selection.Id == id)
			{
				return player;
			}
		}
		return null;
	}

	public Stage(AvatarFarmOnline.Logic.Stage.StageParameters parameters, AvatarFarmOnline.Logic.Stage.FarmData farmData)
	{
		world = new World(new Vector2(0f, 0f));
		this.farmData = farmData;
		farmData.SetStage(this);
		InitWorld();
		farmData.OnIncreaseSize += farmData_OnIncreaseSize;
		foreach (AvatarFarmOnline.Logic.Stage.PlayerSpawnData playerSpawn in parameters.PlayerSpawns)
		{
			AddSelection(playerSpawn);
		}
	}

	public AvatarFarmOnline.Logic.Stage.Player AddSelection(AvatarFarmOnline.Logic.Stage.PlayerSpawnData psd)
	{
		AvatarFarmOnline.Logic.Stage.Player player = AvatarFarmOnline.Logic.Stage.Player.LoadPlayer(this, psd);
		players.Add(player);
		if (player is AvatarFarmOnline.Logic.Stage.LocalPlayer)
		{
			localPlayer = (AvatarFarmOnline.Logic.Stage.LocalPlayer)player;
		}
		if (OnPlayerAdded != null)
		{
			OnPlayerAdded(player);
		}
		return player;
	}

	public void RemovePlayer(AvatarFarmOnline.Logic.Stage.Player player)
	{
		players.Remove(player);
		if (OnPlayerRemoved != null)
		{
			OnPlayerRemoved(player);
		}
	}

	private void ShowMessage(string title, string message)
	{
		isShowingMessage = true;
		if (OnShowMessage != null)
		{
			OnShowMessage(this, title, message);
		}
	}

	public void MessageRead()
	{
		isShowingMessage = false;
	}

	private void InitWorld()
	{
		worldBody = BodyFactory.CreateBody(world, Vector2.Zero);
		worldBody.IgnoreGravity = true;
		worldBody.BodyType = BodyType.Static;
		worldBody.SleepingAllowed = false;
		worldBody.FixedRotation = true;
		Vector2 farmExtents = farmData.FarmExtents;
		Vector2 vector = new Vector2(-0.175f, -0.175f);
		Vector2 vector2 = new Vector2(farmExtents.X + 0.175f, -0.175f);
		Vector2 vector3 = new Vector2(farmExtents.X + 0.175f, farmExtents.Y + 0.175f);
		Vector2 vector4 = new Vector2(-0.175f, farmExtents.Y + 0.175f);
		FixtureFactory.CreateEdge(vector, vector2, worldBody, this);
		FixtureFactory.CreateEdge(vector2, vector3, worldBody, this);
		FixtureFactory.CreateEdge(vector3, vector4, worldBody, this);
		FixtureFactory.CreateEdge(vector4, vector, worldBody, this);
	}

	private void farmData_OnIncreaseSize(Int2 oldSize, Int2 newSize)
	{
		if (worldBody.FixtureList.Count >= 4)
		{
			Vector2 farmExtents = farmData.FarmExtents;
			Vector2 vector = new Vector2(-0.175f, -0.175f);
			Vector2 vector2 = new Vector2(farmExtents.X + 0.175f, -0.175f);
			Vector2 vector3 = new Vector2(farmExtents.X + 0.175f, farmExtents.Y + 0.175f);
			Vector2 vector4 = new Vector2(-0.175f, farmExtents.Y + 0.175f);
			while (worldBody.FixtureList.Count > 0)
			{
				worldBody.DestroyFixture(worldBody.FixtureList[0]);
			}
			FixtureFactory.CreateEdge(vector, vector2, worldBody, this);
			FixtureFactory.CreateEdge(vector2, vector3, worldBody, this);
			FixtureFactory.CreateEdge(vector3, vector4, worldBody, this);
			FixtureFactory.CreateEdge(vector4, vector, worldBody, this);
		}
	}

	public void StartGame()
	{
		stageTimer.Reset();
		stageTimer.Resume();
		state = StageState.Playing;
		if (OnStageStart != null)
		{
			OnStageStart(this);
		}
		int coins = 0;
		int cash = 0;
		if (farmData.CheckRewards(out coins, out cash))
		{
			ShowMessage("WELCOME_BACK".Translate(), string.Format("DAILY_REWARD_{0}".Translate(), GameMath.IntToString(coins, "#,0") + AvatarFarmOnline.Logic.Money.CoinChar + "  " + GameMath.IntToString(cash, "#,0") + AvatarFarmOnline.Logic.Money.CashChar));
		}
	}

	public void Pause()
	{
		if (state == StageState.Playing)
		{
			state = StageState.Paused;
		}
	}

	public void Resume()
	{
		if (state == StageState.Paused)
		{
			state = StageState.Playing;
		}
	}

	public void Update()
	{
		stageTimer.Clock(35f);
		RemovePendingEntities();
		switch (StageState)
		{
		case StageState.Playing:
		case StageState.Paused:
		case StageState.Shop:
			UpdateElements();
			break;
		case StageState.Finished:
			break;
		}
	}

	private void UpdateElements()
	{
		foreach (AvatarFarmOnline.Logic.Stage.Player player in players)
		{
			player.Update();
		}
		CheckEntityRemoval();
		if (state == StageState.Playing && farmData.IsLocal && stageTimer.TimeSince(lastSave) > 300000)
		{
			farmData.Save(now: false);
			lastSave = stageTimer.TotalTime;
		}
		world.Step(Timer.LastIntervalSeconds);
		world.ClearForces();
	}

	private void OnSaveFinished()
	{
		state = StageState.Playing;
	}

	public void Tick()
	{
		farmData.Tick();
	}

	private void CheckEntityRemoval()
	{
	}

	private void RemovePendingEntities()
	{
	}

	public void Shop(bool goToPlantsCategory)
	{
		state = StageState.Shop;
		if (OnShowShop != null)
		{
			OnShowShop(this, goToPlantsCategory);
		}
	}

	public void EndShop(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition shopItem)
	{
		state = StageState.Playing;
		LocalPlayer.EndShop(shopItem);
	}

	public void FinishStage()
	{
		state = StageState.Finished;
		_ = GameManager.PersistentData;
		if (farmData.IsLocal)
		{
			farmData.Save(now: true);
		}
		if (OnGameFinished != null)
		{
			OnGameFinished(this);
		}
	}

	~Stage()
	{
		Dispose();
	}

	public void Dispose()
	{
		RemovePendingEntities();
		stageTimer = null;
		state = StageState.Finished;
	}
}
