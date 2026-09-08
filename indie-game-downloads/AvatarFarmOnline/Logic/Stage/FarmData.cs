using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Logic.Stage.Buildings;
using AvatarFarmOnline.Logic.Stage.Contents;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Network;
using Quasar.GameUtils.Storage;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage;

internal class FarmData
{
	public class FarmLoadException : Exception
	{
	}

	private const string SAVE_FOLDER = "Farms/";

	private const string BACKUP_FOLDER = "Backups/";

	private Int2 farmSize = AvatarFarmOnline.Logic.GameGlobals.InitialFarmSize;

	private AvatarFarmOnline.Logic.Stage.FarmTile[,] tiles;

	private List<AvatarFarmOnline.Logic.Stage.Buildings.Building> buildings;

	private int nextBuildingId = 1;

	private List<AvatarFarmOnline.Logic.Stage.Buildings.Decoration> decorations;

	private int nextDecorationId = 1;

	private List<AvatarFarmOnline.Logic.Stage.Buildings.Tool> tools;

	private int nextToolId = 1;

	private AvatarFarmOnline.Logic.Stage.PlayerData playerData;

	private AvatarFarmOnline.Logic.Seasons currentSeason;

	private int nextSeasonChange;

	private int worldTicks;

	private int lastDailyReward;

	private List<KeyValuePair<AvatarFarmOnline.Logic.Seasons, int>> startedSeasons = new List<KeyValuePair<AvatarFarmOnline.Logic.Seasons, int>>();

	private bool initError;

	private int saveCount;

	private bool saving;

	private AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader localHeader;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private bool savePending;

	private int lastSavedTicks = -1;

	public Int2 FarmSize => farmSize;

	public Vector2 FarmExtents => FarmSize * 2f;

	public AvatarFarmOnline.Logic.Stage.FarmTile[,] Tiles => tiles;

	public List<AvatarFarmOnline.Logic.Stage.Buildings.Building> Buildings => buildings;

	public List<AvatarFarmOnline.Logic.Stage.Buildings.Decoration> Decorations => decorations;

	public List<AvatarFarmOnline.Logic.Stage.Buildings.Tool> Tools => tools;

	public AvatarFarmOnline.Logic.Stage.PlayerData PlayerData => playerData;

	public AvatarFarmOnline.Logic.Seasons CurrentSeason => currentSeason;

	public int NextSeasonChange => nextSeasonChange;

	public float SeasonProgress => 1f - (float)(nextSeasonChange - worldTicks) / 900f;

	public int WorldTicks => worldTicks;

	public List<KeyValuePair<AvatarFarmOnline.Logic.Seasons, int>> StartedSeasons => startedSeasons;

	public bool Saving => saving;

	public bool IsLocal => localHeader != null;

	public Vector2 CenterPosition => farmSize * 2f * 0.5f;

	public int HackCheck
	{
		get
		{
			int num = (worldTicks + lastDailyReward + nextSeasonChange) * 50 + nextBuildingId * nextDecorationId * nextToolId * (int)currentSeason * 7;
			num += playerData.HackCheck;
			for (int i = 0; i < farmSize.X; i++)
			{
				for (int j = 0; j < farmSize.Y; j++)
				{
					num += tiles[i, j].HackCheck;
				}
			}
			foreach (AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration in decorations)
			{
				num += decoration.HackCheck;
			}
			foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building in buildings)
			{
				num += building.HackCheck;
			}
			return num;
		}
	}

	public int TicksToNextDeath
	{
		get
		{
			int num = int.MaxValue;
			for (int i = 0; i < farmSize.X; i++)
			{
				for (int j = 0; j < farmSize.Y; j++)
				{
					AvatarFarmOnline.Logic.Stage.FarmTile farmTile = tiles[i, j];
					if (farmTile.IsEmpty)
					{
						continue;
					}
					AvatarFarmOnline.Logic.Stage.TileContents contents = farmTile.Contents;
					if (contents.Type == TileContentsType.Plant)
					{
						AvatarFarmOnline.Logic.Stage.Contents.Plant plant = (AvatarFarmOnline.Logic.Stage.Contents.Plant)contents;
						if (plant.State != AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Withered)
						{
							num = Math.Min(plant.DeathRemainingTime, num);
						}
					}
				}
			}
			if (num == int.MaxValue)
			{
				num = 0;
			}
			if (localHeader != null)
			{
				switch (localHeader.SimulationSpeed)
				{
				case AvatarFarmOnline.Logic.SimulationSpeeds.Half:
					num *= 2;
					break;
				case AvatarFarmOnline.Logic.SimulationSpeeds.Quarter:
					num *= 4;
					break;
				case AvatarFarmOnline.Logic.SimulationSpeeds.Stop:
					num = 0;
					break;
				case AvatarFarmOnline.Logic.SimulationSpeeds.ThreeQuarters:
					num = num * 4 / 3;
					break;
				}
			}
			return num;
		}
	}

	public event Action<AvatarFarmOnline.Logic.Stage.Player, AvatarFarmOnline.Logic.Stage.Buildings.Building> OnBuildingPlaced;

	public event Action<AvatarFarmOnline.Logic.Stage.Player, AvatarFarmOnline.Logic.Stage.Buildings.Building> OnBuildingRemoved;

	public event Action<AvatarFarmOnline.Logic.Stage.FarmTile, AvatarFarmOnline.Logic.Stage.TileContents> OnTileChangedContents;

	public event Action<Int2, Int2> OnIncreaseSize;

	public event Action<AvatarFarmOnline.Logic.Stage.Player, AvatarFarmOnline.Logic.Stage.Buildings.Decoration> OnDecorationPlaced;

	public event Action<AvatarFarmOnline.Logic.Stage.Player, AvatarFarmOnline.Logic.Stage.Buildings.Decoration> OnDecorationRemoved;

	public event Action<AvatarFarmOnline.Logic.Stage.Player, AvatarFarmOnline.Logic.Stage.Buildings.Tool> OnToolPlaced;

	public event Action<AvatarFarmOnline.Logic.Stage.Player, AvatarFarmOnline.Logic.Stage.Buildings.Tool> OnToolRemoved;

	public event Action<AvatarFarmOnline.Logic.Seasons> OnSeasonChange;

	public event Action<Vector2, int, AvatarFarmOnline.Logic.Money> OnActionPerformed;

	public AvatarFarmOnline.Logic.Stage.FarmTile OffsetTile(AvatarFarmOnline.Logic.Stage.FarmTile tile, Int2 offset)
	{
		if (tile == null)
		{
			return null;
		}
		Int2 @int = tile.Tile + offset;
		if (GameMath.Between(0, farmSize.X - 1, @int.X) && GameMath.Between(0, farmSize.Y - 1, @int.Y))
		{
			return tiles[@int.X, @int.Y];
		}
		return null;
	}

	public AvatarFarmOnline.Logic.Stage.FarmTile GetTile(Int2 tile)
	{
		tile.X = GameMath.Clamp(0, farmSize.X - 1, tile.X);
		tile.Y = GameMath.Clamp(0, farmSize.Y - 1, tile.Y);
		return tiles[tile.X, tile.Y];
	}

	public AvatarFarmOnline.Logic.Stage.Buildings.Building GetBuilding(int id)
	{
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building in buildings)
		{
			if (building.Id == id)
			{
				return building;
			}
		}
		return null;
	}

	public AvatarFarmOnline.Logic.Stage.Buildings.Decoration GetDecoration(int id)
	{
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration in decorations)
		{
			if (decoration.Id == id)
			{
				return decoration;
			}
		}
		return null;
	}

	public AvatarFarmOnline.Logic.Stage.Buildings.Tool GetTool(int id)
	{
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Tool tool in tools)
		{
			if (tool.Id == id)
			{
				return tool;
			}
		}
		return null;
	}

	public FarmData(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader header)
		: this()
	{
		localHeader = header;
		StorageManager.Instance.Exec(header.Manager.PlayerIndex, Init, null, StorageManager.IOType.Read);
		if (initError)
		{
			throw new FarmLoadException();
		}
	}

	private FarmData()
	{
		buildings = new List<AvatarFarmOnline.Logic.Stage.Buildings.Building>(10);
		decorations = new List<AvatarFarmOnline.Logic.Stage.Buildings.Decoration>(10);
		tools = new List<AvatarFarmOnline.Logic.Stage.Buildings.Tool>(10);
		playerData = new AvatarFarmOnline.Logic.Stage.PlayerData();
	}

	private void Init(int xp, int coins, int cash)
	{
		Init();
		EarnXp(xp);
		playerData.IncCoins(coins);
		playerData.IncCash(cash);
	}

	private void Init()
	{
		Init(AvatarFarmOnline.Logic.GameGlobals.InitialFarmSize);
		int num = farmSize.X / 2 - 3;
		int num2 = farmSize.Y / 2 - 2;
		for (int i = num2; i < num2 + 4; i++)
		{
			AvatarFarmOnline.Logic.Stage.FarmTile farmTile = tiles[num, i];
			AvatarFarmOnline.Logic.Stage.Contents.Tree content = new AvatarFarmOnline.Logic.Stage.Contents.Tree(AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetTree("PineTree"), farmTile);
			farmTile.SetContent(content);
		}
		PlaceBuilding(null, AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetBuilding("Cornucopia"), new Int2(num, num2 - 1), 0, out var _);
		for (int j = num2; j < num2 + 4; j++)
		{
			AvatarFarmOnline.Logic.Stage.FarmTile farmTile2 = tiles[num + 1, j];
			AvatarFarmOnline.Logic.Stage.Contents.Plant plant = new AvatarFarmOnline.Logic.Stage.Contents.Plant(AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetPlant("Wheat"), farmTile2);
			plant.Randomize();
			farmTile2.SetContent(plant);
		}
		for (int k = num2; k < num2 + 4; k++)
		{
			for (int l = num + 2; l < num + 4; l++)
			{
				AvatarFarmOnline.Logic.Stage.FarmTile farmTile3 = tiles[l, k];
				AvatarFarmOnline.Logic.Stage.Contents.Plant plant2 = new AvatarFarmOnline.Logic.Stage.Contents.Plant(AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetPlant("AloeVera"), farmTile3);
				plant2.Randomize();
				farmTile3.SetContent(plant2);
			}
		}
		for (int m = num2; m < num2 + 4; m++)
		{
			for (int n = num + 4; n < num + 6; n++)
			{
				AvatarFarmOnline.Logic.Stage.FarmTile farmTile4 = tiles[n, m];
				AvatarFarmOnline.Logic.Stage.Contents.Plant plant3 = new AvatarFarmOnline.Logic.Stage.Contents.Plant(AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetPlant("Cotton"), farmTile4);
				plant3.Randomize();
				farmTile4.SetContent(plant3);
			}
		}
		for (int num3 = num2; num3 < num2 + 4; num3++)
		{
			AvatarFarmOnline.Logic.Stage.FarmTile farmTile5 = tiles[num + 6, num3];
			AvatarFarmOnline.Logic.Stage.Contents.Plant plant4 = new AvatarFarmOnline.Logic.Stage.Contents.Plant(AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetPlant("Artichoke"), farmTile5);
			plant4.Randomize();
			farmTile5.SetContent(plant4);
		}
		for (int num4 = num + 1; num4 < num + 7; num4++)
		{
			AvatarFarmOnline.Logic.Stage.FarmTile farmTile6 = tiles[num4, num2 - 1];
			AvatarFarmOnline.Logic.Stage.Contents.Land content2 = new AvatarFarmOnline.Logic.Stage.Contents.Land(farmTile6, AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Plowed);
			farmTile6.SetContent(content2);
			farmTile6 = tiles[num4, num2 + 4];
			content2 = new AvatarFarmOnline.Logic.Stage.Contents.Land(farmTile6, AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Plowed);
			farmTile6.SetContent(content2);
		}
		for (int num5 = num2 - 1; num5 < num2 + 5; num5++)
		{
			AvatarFarmOnline.Logic.Stage.FarmTile farmTile7 = tiles[num + 7, num5];
			AvatarFarmOnline.Logic.Stage.Contents.Land content3 = new AvatarFarmOnline.Logic.Stage.Contents.Land(farmTile7, AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Gathered);
			farmTile7.SetContent(content3);
		}
	}

	private void Init(Int2 farmSize)
	{
		this.farmSize = farmSize;
		tiles = new AvatarFarmOnline.Logic.Stage.FarmTile[farmSize.X, farmSize.Y];
		for (int i = 0; i < farmSize.X; i++)
		{
			for (int j = 0; j < farmSize.Y; j++)
			{
				tiles[i, j] = new AvatarFarmOnline.Logic.Stage.FarmTile(this, new Int2(i, j));
			}
		}
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building in buildings)
		{
			building.Dispose();
		}
		buildings.Clear();
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration in decorations)
		{
			decoration.Dispose();
		}
		decorations.Clear();
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Tool tool in tools)
		{
			tool.Dispose();
		}
		tools.Clear();
		playerData.Init();
		worldTicks = 0;
		currentSeason = AvatarFarmOnline.Logic.Seasons.Spring;
		nextSeasonChange = 900;
	}

	public void SetStage(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building in buildings)
		{
			building.SetStage(stage);
		}
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Tool tool in tools)
		{
			tool.SetStage(stage);
		}
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration in decorations)
		{
			decoration.SetStage(stage);
		}
	}

	public void ActionPerformed(Vector2 tile, int xp, AvatarFarmOnline.Logic.Money money)
	{
		if (OnActionPerformed != null)
		{
			OnActionPerformed(tile, xp, money);
		}
	}

	public void TileChangedContents(AvatarFarmOnline.Logic.Stage.FarmTile tile, AvatarFarmOnline.Logic.Stage.TileContents contents)
	{
		if (OnTileChangedContents != null)
		{
			OnTileChangedContents(tile, contents);
		}
	}

	public void Tick()
	{
		Tick(1);
	}

	public void Tick(int ticksPassed)
	{
		lock (this)
		{
			startedSeasons.Clear();
			if (worldTicks + ticksPassed >= nextSeasonChange)
			{
				while (nextSeasonChange <= worldTicks + ticksPassed)
				{
					currentSeason = AvatarFarmOnline.Logic.Parsing.NextSeason(currentSeason);
					if (startedSeasons.Count < 4)
					{
						startedSeasons.Add(new KeyValuePair<AvatarFarmOnline.Logic.Seasons, int>(currentSeason, nextSeasonChange));
					}
					else
					{
						startedSeasons.RemoveAt(0);
						startedSeasons.Add(new KeyValuePair<AvatarFarmOnline.Logic.Seasons, int>(currentSeason, nextSeasonChange));
					}
					nextSeasonChange += 900;
				}
				if (OnSeasonChange != null)
				{
					OnSeasonChange(currentSeason);
				}
			}
			foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building in buildings)
			{
				building.Tick(ticksPassed);
			}
			for (int i = 0; i < farmSize.X; i++)
			{
				for (int j = 0; j < farmSize.Y; j++)
				{
					tiles[i, j].Tick(ticksPassed);
				}
			}
			worldTicks += ticksPassed;
		}
	}

	private void GetBuildingBounds(AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition definition, Int2 topLeftTile, int rotation, out Int2 min, out Int2 max)
	{
		min = (max = topLeftTile);
		switch (rotation)
		{
		case 0:
			max.X = topLeftTile.X + definition.Size.X - 1;
			max.Y = topLeftTile.Y + definition.Size.Y - 1;
			break;
		case 1:
			min.X = topLeftTile.X - (definition.Size.Y - 1);
			max.Y = topLeftTile.Y + definition.Size.X - 1;
			break;
		case 2:
			min.X = topLeftTile.X - (definition.Size.X - 1);
			min.Y = topLeftTile.Y - (definition.Size.Y - 1);
			break;
		case 3:
			max.X = topLeftTile.X + definition.Size.Y - 1;
			min.Y = topLeftTile.Y - (definition.Size.X - 1);
			break;
		}
	}

	public bool CanPlaceBuilding(AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition definition, Int2 tile, int rotation, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		failReason = AvatarFarmOnline.Logic.FailedActionReason.NotEnoughSpace;
		GetBuildingBounds(definition, tile, rotation, out var min, out var max);
		if (max.X >= farmSize.X)
		{
			return false;
		}
		if (max.Y >= farmSize.Y)
		{
			return false;
		}
		if (min.X < 0)
		{
			return false;
		}
		if (min.Y < 0)
		{
			return false;
		}
		for (int i = min.X; i <= max.X; i++)
		{
			for (int j = min.Y; j <= max.Y; j++)
			{
				AvatarFarmOnline.Logic.Stage.FarmTile farmTile = tiles[i, j];
				if (!farmTile.IsEmpty)
				{
					return false;
				}
			}
		}
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		return true;
	}

	public AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding PlaceBuilding(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition definition, Int2 tile, int rotation, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (!CanPlaceBuilding(definition, tile, rotation, out failReason))
		{
			return null;
		}
		GetBuildingBounds(definition, tile, rotation, out var min, out var max);
		lock (this)
		{
			switch (definition.Category)
			{
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Building:
			{
				AvatarFarmOnline.Logic.Stage.Buildings.Building building = new AvatarFarmOnline.Logic.Stage.Buildings.Building(stage, definition as AvatarFarmOnline.Logic.Stage.Definition.BuildingDefinition, tile, rotation, nextBuildingId++);
				buildings.Add(building);
				for (int k = min.X; k <= max.X; k++)
				{
					for (int l = min.Y; l <= max.Y; l++)
					{
						AvatarFarmOnline.Logic.Stage.FarmTile farmTile2 = tiles[k, l];
						farmTile2.SetContent(new AvatarFarmOnline.Logic.Stage.Contents.BuildingTile(building, farmTile2));
					}
				}
				if (OnBuildingPlaced != null)
				{
					OnBuildingPlaced(player, building);
				}
				return building;
			}
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration:
			{
				AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration = new AvatarFarmOnline.Logic.Stage.Buildings.Decoration(stage, definition as AvatarFarmOnline.Logic.Stage.Definition.DecorationDefinition, tile, rotation, nextDecorationId++);
				decorations.Add(decoration);
				for (int m = min.X; m <= max.X; m++)
				{
					for (int n = min.Y; n <= max.Y; n++)
					{
						AvatarFarmOnline.Logic.Stage.FarmTile farmTile3 = tiles[m, n];
						farmTile3.SetContent(new AvatarFarmOnline.Logic.Stage.Contents.DecorationTile(decoration, farmTile3));
					}
				}
				if (OnDecorationPlaced != null)
				{
					OnDecorationPlaced(player, decoration);
				}
				return decoration;
			}
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tool:
			{
				AvatarFarmOnline.Logic.Stage.Buildings.Tool tool = new AvatarFarmOnline.Logic.Stage.Buildings.Tool(stage, definition as AvatarFarmOnline.Logic.Stage.Definition.ToolDefinition, tile, rotation, nextToolId++);
				tools.Add(tool);
				for (int i = min.X; i <= max.X; i++)
				{
					for (int j = min.Y; j <= max.Y; j++)
					{
						AvatarFarmOnline.Logic.Stage.FarmTile farmTile = tiles[i, j];
						farmTile.SetContent(new AvatarFarmOnline.Logic.Stage.Contents.ToolTile(tool, farmTile));
					}
				}
				if (OnToolPlaced != null)
				{
					OnToolPlaced(player, tool);
				}
				return tool;
			}
			}
		}
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		return null;
	}

	public void RemoveBuilding(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Buildings.Building building)
	{
		GetBuildingBounds(building.Definition, building.TopLeftTile, building.Rotation, out var min, out var max);
		lock (this)
		{
			for (int i = min.X; i <= max.X; i++)
			{
				for (int j = min.Y; j <= max.Y; j++)
				{
					tiles[i, j].ClearContents();
				}
			}
			buildings.Remove(building);
		}
		if (OnBuildingRemoved != null)
		{
			OnBuildingRemoved(player, building);
		}
	}

	public void RemoveDecoration(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration)
	{
		GetBuildingBounds(decoration.Definition, decoration.TopLeftTile, decoration.Rotation, out var min, out var max);
		lock (this)
		{
			for (int i = min.X; i <= max.X; i++)
			{
				for (int j = min.Y; j <= max.Y; j++)
				{
					tiles[i, j].ClearContents();
				}
			}
			decorations.Remove(decoration);
		}
		if (OnDecorationRemoved != null)
		{
			OnDecorationRemoved(player, decoration);
		}
	}

	public void RemoveTool(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Buildings.Tool tool)
	{
		GetBuildingBounds(tool.Definition, tool.TopLeftTile, tool.Rotation, out var min, out var max);
		lock (this)
		{
			for (int i = min.X; i <= max.X; i++)
			{
				for (int j = min.Y; j <= max.Y; j++)
				{
					tiles[i, j].ClearContents();
				}
			}
			tools.Remove(tool);
		}
		if (OnToolRemoved != null)
		{
			OnToolRemoved(player, tool);
		}
	}

	public bool CanBuy(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition definition, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (!definition.AvailableInTrial && PlatformInterface.Instance.IsTrial)
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.Trialmode;
			return false;
		}
		if (playerData.Level < definition.MinLevel)
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.XpLevelNeeded;
			return false;
		}
		if (definition.IsBuildingNeeded && !HasBuilding(definition.BuildingNeeded))
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.BuildingNeeded;
			return false;
		}
		if (definition.Category == AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant)
		{
			AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition plantDefinition = definition as AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition;
			if ((plantDefinition.PlantSeasons & currentSeason) == 0)
			{
				failReason = AvatarFarmOnline.Logic.FailedActionReason.IncorrectSeason;
				return false;
			}
		}
		if (playerData.CanBuy(definition.Price))
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
			return true;
		}
		failReason = AvatarFarmOnline.Logic.FailedActionReason.NotEnoughMoney;
		return false;
	}

	public bool TryPlantTree(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition td, AvatarFarmOnline.Logic.Stage.FarmTile tile, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (CanBuy(td, out failReason))
		{
			lock (this)
			{
				if (tile.PlantTree(td, out failReason))
				{
					playerData.TryBuy(td.Price);
					EarnXp(td.PlantXp);
					player.EarnXp(td.PlantXp);
					ActionPerformed((tile.Tile + new Vector2(0.5f)) * 2f, td.PlantXp, -td.Price);
					return true;
				}
			}
		}
		return false;
	}

	public bool TryPlantAnimal(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition td, AvatarFarmOnline.Logic.Stage.FarmTile tile, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (CanBuy(td, out failReason))
		{
			lock (this)
			{
				if (tile.PlantAnimal(td, out failReason))
				{
					playerData.TryBuy(td.Price);
					EarnXp(td.PlantXp);
					player.EarnXp(td.PlantXp);
					ActionPerformed((tile.Tile + new Vector2(0.5f)) * 2f, td.PlantXp, -td.Price);
					return true;
				}
			}
		}
		return false;
	}

	public bool TryPlant(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition pd, AvatarFarmOnline.Logic.Stage.FarmTile tile, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (CanBuy(pd, out failReason))
		{
			lock (this)
			{
				if (tile.Plant(pd, out failReason))
				{
					playerData.TryBuy(pd.Price);
					EarnXp(pd.PlantXp);
					player.EarnXp(pd.PlantXp);
					ActionPerformed((tile.Tile + new Vector2(0.5f)) * 2f, pd.PlantXp, -pd.Price);
					return true;
				}
			}
		}
		return false;
	}

	public bool TryPlaceBuilding(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition bbd, Int2 tile, int rotation, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (CanBuy(bbd, out failReason))
		{
			lock (this)
			{
				AvatarFarmOnline.Logic.Stage.Buildings.BaseBuilding baseBuilding = PlaceBuilding(player, bbd, tile, rotation, out failReason);
				if (baseBuilding != null)
				{
					playerData.TryBuy(bbd.Price);
					EarnXp(bbd.BuildXp);
					player.EarnXp(bbd.BuildXp);
					ActionPerformed(baseBuilding.CenterPosition, bbd.BuildXp, -bbd.Price);
					return true;
				}
			}
		}
		return false;
	}

	public bool CanPlow(AvatarFarmOnline.Logic.Stage.FarmTile tile, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		if (!tile.IsEmpty)
		{
			switch (tile.Contents.Type)
			{
			case TileContentsType.Plant:
				if (((AvatarFarmOnline.Logic.Stage.Contents.Plant)tile.Contents).State == AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Withered)
				{
					return true;
				}
				return false;
			case TileContentsType.Land:
				break;
			default:
				failReason = AvatarFarmOnline.Logic.FailedActionReason.TileNotEmpty;
				return false;
			}
			if (((AvatarFarmOnline.Logic.Stage.Contents.Land)tile.Contents).State != AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Gathered)
			{
				failReason = AvatarFarmOnline.Logic.FailedActionReason.TileNotEmpty;
				return false;
			}
		}
		if (!playerData.CanBuy(AvatarFarmOnline.Logic.GameGlobals.PlowPrice))
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.NotEnoughMoney;
			return false;
		}
		return true;
	}

	public bool TryPlow(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.Stage.FarmTile tile, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (CanPlow(tile, out failReason))
		{
			lock (this)
			{
				tile.ClearContents();
				tile.SetContent(new AvatarFarmOnline.Logic.Stage.Contents.Land(tile, AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Plowed));
				bool flag = playerData.TryBuy(AvatarFarmOnline.Logic.GameGlobals.PlowPrice);
				EarnXp(1);
				player.EarnXp(1);
				ActionPerformed((tile.Tile + new Vector2(0.5f)) * 2f, 1, flag ? (-AvatarFarmOnline.Logic.GameGlobals.PlowPrice) : default(AvatarFarmOnline.Logic.Money));
				return true;
			}
		}
		return false;
	}

	public int ToolFuel(ToolTypes toolType)
	{
		if (GetUsableToolSize(toolType, out var toolSize))
		{
			int num = 0;
			{
				foreach (AvatarFarmOnline.Logic.Stage.Buildings.Tool tool in tools)
				{
					if ((tool.Definition.ToolType & toolType) != ToolTypes.None && tool.Definition.ToolSize == toolSize)
					{
						num += tool.RemainingFuel;
					}
				}
				return num;
			}
		}
		return 0;
	}

	public void UseFuel(ToolTypes toolType)
	{
		if (!GetUsableToolSize(toolType, out var toolSize))
		{
			return;
		}
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Tool tool in tools)
		{
			if ((tool.Definition.ToolType & toolType) != ToolTypes.None && tool.Definition.ToolSize == toolSize && tool.RemainingFuel > 0)
			{
				lock (this)
				{
					tool.UseFuel();
					break;
				}
			}
		}
	}

	public bool GetUsableToolSize(ToolTypes toolType, out int toolSize)
	{
		AvatarFarmOnline.Logic.Stage.Buildings.Tool tool = null;
		toolSize = 0;
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Tool tool2 in tools)
		{
			if ((tool2.Definition.ToolType & toolType) != ToolTypes.None && tool2.RemainingFuel > 0)
			{
				if (tool == null)
				{
					tool = tool2;
					toolSize = tool2.Definition.ToolSize;
				}
				else if (tool2.Definition.ToolSize > toolSize)
				{
					tool = tool2;
					toolSize = tool2.Definition.ToolSize;
				}
			}
		}
		return tool != null;
	}

	public void EarnXp(int xp)
	{
		lock (this)
		{
			if (playerData.IncXp(xp))
			{
				Int2 newSize = AvatarFarmOnline.Logic.GameGlobals.FarmSize(playerData.Level);
				if (farmSize.X < newSize.X)
				{
					Resize(newSize);
				}
			}
		}
	}

	private void Resize(Int2 newSize)
	{
		lock (this)
		{
			if (newSize.X < farmSize.X || newSize.Y < farmSize.Y)
			{
				return;
			}
			AvatarFarmOnline.Logic.Stage.FarmTile[,] array = tiles;
			Int2 arg = farmSize;
			tiles = new AvatarFarmOnline.Logic.Stage.FarmTile[newSize.X, newSize.Y];
			for (int i = 0; i < arg.X; i++)
			{
				for (int j = 0; j < arg.Y; j++)
				{
					tiles[i, j] = array[i, j];
				}
			}
			for (int k = arg.X; k < newSize.X; k++)
			{
				for (int l = 0; l < arg.Y; l++)
				{
					tiles[k, l] = new AvatarFarmOnline.Logic.Stage.FarmTile(this, new Int2(k, l));
				}
			}
			for (int m = 0; m < newSize.X; m++)
			{
				for (int n = farmSize.Y; n < newSize.Y; n++)
				{
					tiles[m, n] = new AvatarFarmOnline.Logic.Stage.FarmTile(this, new Int2(m, n));
				}
			}
			farmSize = newSize;
			if (OnIncreaseSize != null)
			{
				OnIncreaseSize(arg, newSize);
			}
		}
	}

	public void EarnMoney(AvatarFarmOnline.Logic.Money money)
	{
		lock (this)
		{
			playerData.IncMoney(money);
		}
	}

	public void ItemGathered(string itemCategory)
	{
		lock (this)
		{
			foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building in buildings)
			{
				if (building.ItemGathered(itemCategory))
				{
					break;
				}
			}
		}
	}

	public bool CheckRewards(out int coins, out int cash)
	{
		if (worldTicks >= lastDailyReward + 86400)
		{
			int num = 0;
			int num2 = 0;
			while (worldTicks >= lastDailyReward + 86400)
			{
				num += 500;
				num2++;
				lastDailyReward += 86400;
			}
			coins = num;
			cash = num2;
			playerData.IncCoins(num);
			playerData.IncCash(num2);
			return true;
		}
		coins = 0;
		cash = 0;
		return false;
	}

	public bool HasBuilding(string buildingId)
	{
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building in buildings)
		{
			if (building.Definition.Id == buildingId)
			{
				return true;
			}
		}
		return false;
	}

	public void Save(bool now)
	{
		if (!savePending && worldTicks != lastSavedTicks)
		{
			if (now)
			{
				StorageManager.Instance.Exec(localHeader.Manager.PlayerIndex, Save, null, StorageManager.IOType.Write);
				return;
			}
			savePending = true;
			StorageManager.Instance.Post(localHeader.Manager.PlayerIndex, Save, null, StorageManager.IOType.Write);
		}
	}

	private void Save(StorageContainer container, object parameters)
	{
		saving = true;
		if (container != null)
		{
			string file = "Farms/" + localHeader.Filename;
			string file2 = "Backups/" + localHeader.Filename;
			try
			{
				try
				{
					if (container.FileExists(file))
					{
						using Stream stream = container.OpenFile(file, FileMode.Open);
						if (!container.DirectoryExists("Backups/"))
						{
							container.CreateDirectory("Backups/");
						}
						using (Stream stream2 = container.OpenFile(file2, FileMode.Create))
						{
							byte[] buffer = new byte[1024];
							int num = 0;
							do
							{
								num = stream.Read(buffer, 0, 1024);
								if (num > 0)
								{
									stream2.Write(buffer, 0, num);
								}
							}
							while (num > 0);
							stream2.Close();
						}
						stream.Close();
					}
				}
				catch
				{
				}
				XDocument xDocument = new XDocument();
				lock (this)
				{
					XElement xElement = new XElement("Farm");
					xElement.SetLongAttribute("save_date", DateTime.Now.Ticks);
					xElement.SetInt2Attribute("farm_size", farmSize);
					xElement.SetIntAttribute("ticks", worldTicks);
					xElement.SetIntAttribute("next_building_id", nextBuildingId);
					xElement.SetIntAttribute("next_decoration_id", nextDecorationId);
					xElement.SetIntAttribute("next_tool_id", nextToolId);
					xElement.SetIntAttribute("season", (int)currentSeason);
					xElement.SetIntAttribute("next_season_change", nextSeasonChange);
					xElement.SetIntAttribute("last_daily_reward", lastDailyReward);
					xElement.SetIntAttribute("save_count", ++saveCount);
					XElement xElement2 = new XElement("Player");
					playerData.ToXml(xElement2);
					xElement.Add(xElement2);
					foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building in buildings)
					{
						XElement xElement3 = new XElement("Building");
						building.ToXml(xElement3);
						xElement.Add(xElement3);
					}
					foreach (AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration in decorations)
					{
						XElement xElement4 = new XElement("Decoration");
						decoration.ToXml(xElement4);
						xElement.Add(xElement4);
					}
					foreach (AvatarFarmOnline.Logic.Stage.Buildings.Tool tool in tools)
					{
						XElement xElement5 = new XElement("Tool");
						tool.ToXml(xElement5);
						xElement.Add(xElement5);
					}
					for (int i = 0; i < tiles.GetLength(0); i++)
					{
						for (int j = 0; j < tiles.GetLength(1); j++)
						{
							XElement xElement6 = new XElement("Tile");
							tiles[i, j].ToXml(xElement6);
							xElement.Add(xElement6);
						}
					}
					xElement.SetIntAttribute("hack_check", HackCheck);
					xDocument.Add(xElement);
				}
				if (!container.DirectoryExists("Farms/"))
				{
					container.CreateDirectory("Farms/");
				}
				using (Stream stream3 = container.OpenFile(file, FileMode.Create))
				{
					xDocument.Save(stream3);
					stream3.Close();
				}
				lastSavedTicks = worldTicks;
			}
			catch (Exception)
			{
			}
			localHeader.SetLevel(playerData.Level, playerData.Coins, playerData.Cash);
			if (stage != null && stage.LocalPlayer != null)
			{
				localHeader.SetPlayerPosition(stage.LocalPlayer.Position);
			}
			else
			{
				localHeader.SetPlayerPosition(farmSize * 0.5f);
			}
			localHeader.Manager.Save();
		}
		savePending = false;
		saving = false;
	}

	private void Init(StorageContainer container, object parameters)
	{
		Init();
		initError = false;
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
					long num = xDocument.Root.ParseLongAttribute("save_date", DateTime.Now.Ticks);
					farmSize = xDocument.Root.ParseInt2Attribute("farm_size");
					worldTicks = xDocument.Root.ParseIntAttribute("ticks");
					nextBuildingId = xDocument.Root.ParseIntAttribute("next_building_id");
					nextDecorationId = xDocument.Root.ParseIntAttribute("next_decoration_id");
					nextToolId = xDocument.Root.ParseIntAttribute("next_tool_id", 1);
					currentSeason = (AvatarFarmOnline.Logic.Seasons)xDocument.Root.ParseIntAttribute("season");
					nextSeasonChange = xDocument.Root.ParseIntAttribute("next_season_change");
					lastDailyReward = xDocument.Root.ParseIntAttribute("last_daily_reward");
					saveCount = xDocument.Root.ParseIntAttribute("save_count", 0);
					XElement xe = xDocument.Root.Element("Player");
					playerData.FromXml(xe);
					foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building2 in buildings)
					{
						building2.Dispose();
					}
					foreach (AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration2 in decorations)
					{
						decoration2.Dispose();
					}
					foreach (AvatarFarmOnline.Logic.Stage.Buildings.Tool tool2 in tools)
					{
						tool2.Dispose();
					}
					buildings.Clear();
					decorations.Clear();
					tools.Clear();
					tiles = new AvatarFarmOnline.Logic.Stage.FarmTile[farmSize.X, farmSize.Y];
					foreach (XElement item in xDocument.Root.Elements("Building"))
					{
						AvatarFarmOnline.Logic.Stage.Buildings.Building building = AvatarFarmOnline.Logic.Stage.Buildings.Building.FromXml(this, item);
						if (building != null)
						{
							buildings.Add(building);
						}
					}
					foreach (XElement item2 in xDocument.Root.Elements("Decoration"))
					{
						AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration = AvatarFarmOnline.Logic.Stage.Buildings.Decoration.FromXml(this, item2);
						if (decoration != null)
						{
							decorations.Add(decoration);
						}
					}
					foreach (XElement item3 in xDocument.Root.Elements("Tool"))
					{
						AvatarFarmOnline.Logic.Stage.Buildings.Tool tool = AvatarFarmOnline.Logic.Stage.Buildings.Tool.FromXml(this, item3);
						if (tool != null)
						{
							tools.Add(tool);
						}
					}
					foreach (XElement item4 in xDocument.Root.Elements("Tile"))
					{
						AvatarFarmOnline.Logic.Stage.FarmTile farmTile = AvatarFarmOnline.Logic.Stage.FarmTile.FromXml(this, item4);
						if (farmTile != null && farmTile.Tile.X < farmSize.X && farmTile.Tile.Y < farmSize.Y)
						{
							tiles[farmTile.Tile.X, farmTile.Tile.Y] = farmTile;
						}
					}
					int num2 = xDocument.Root.ParseIntAttribute("hack_check");
					_ = HackCheck;
					int num3 = (int)((DateTime.Now.Ticks - num) / 10000000);
					switch (localHeader.SimulationSpeed)
					{
					case AvatarFarmOnline.Logic.SimulationSpeeds.Half:
						num3 /= 2;
						break;
					case AvatarFarmOnline.Logic.SimulationSpeeds.Quarter:
						num3 /= 4;
						break;
					case AvatarFarmOnline.Logic.SimulationSpeeds.ThreeQuarters:
						num3 = num3 * 3 / 4;
						break;
					case AvatarFarmOnline.Logic.SimulationSpeeds.Stop:
						num3 = 0;
						break;
					}
					Tick(num3);
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

	public static void InitializeFarm(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader header)
	{
		AvatarFarmOnline.Logic.Stage.FarmData farmData = new AvatarFarmOnline.Logic.Stage.FarmData();
		farmData.localHeader = header;
		farmData.Init();
		farmData.Save(now: true);
	}

	public static void InitializeFarm(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader header, int xp, int coins, int cash)
	{
		AvatarFarmOnline.Logic.Stage.FarmData farmData = new AvatarFarmOnline.Logic.Stage.FarmData();
		farmData.localHeader = header;
		farmData.Init(xp, coins, cash);
		farmData.Save(now: true);
	}

	public static void DeleteFarm(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader header)
	{
		StorageManager.Instance.Exec(header.Manager.PlayerIndex, DeleteFarm, header, StorageManager.IOType.Write);
	}

	private static void DeleteFarm(StorageContainer container, object parameters)
	{
		if (container == null)
		{
			return;
		}
		AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader localFarmHeader = parameters as AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader;
		string file = "Farms/" + localFarmHeader.Filename;
		string file2 = "Backups/" + localFarmHeader.Filename;
		try
		{
			if (container.FileExists(file))
			{
				container.DeleteFile(file);
			}
			if (container.FileExists(file2))
			{
				container.DeleteFile(file2);
			}
		}
		catch (Exception)
		{
		}
	}

	public void WritePacket(IPacketWriter packetWriter)
	{
		packetWriter.Write(FarmSize);
		packetWriter.Write(Buildings.Count);
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Building building in Buildings)
		{
			building.SendData(packetWriter);
		}
		packetWriter.Write(Decorations.Count);
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration in Decorations)
		{
			decoration.SendData(packetWriter);
		}
		packetWriter.Write(Tools.Count);
		foreach (AvatarFarmOnline.Logic.Stage.Buildings.Tool tool in Tools)
		{
			tool.SendData(packetWriter);
		}
		for (int i = 0; i < FarmSize.X; i++)
		{
			for (int j = 0; j < FarmSize.Y; j++)
			{
				SendTile(GetTile(new Int2(i, j)), packetWriter);
			}
		}
		packetWriter.Write(nextBuildingId);
		packetWriter.Write(nextDecorationId);
		packetWriter.Write(nextToolId);
		packetWriter.Write((short)currentSeason);
		packetWriter.Write(nextSeasonChange);
		packetWriter.Write(worldTicks);
		packetWriter.Write(lastDailyReward);
		playerData.WritePacket(packetWriter);
	}

	private void SendTile(AvatarFarmOnline.Logic.Stage.FarmTile tile, IPacketWriter packetWriter)
	{
		packetWriter.Write(!tile.IsEmpty);
		if (!tile.IsEmpty)
		{
			AvatarFarmOnline.Logic.Stage.TileContents contents = tile.Contents;
			packetWriter.Write((short)contents.Type);
			switch (contents.Type)
			{
			case TileContentsType.Plant:
				SendPlant((AvatarFarmOnline.Logic.Stage.Contents.Plant)contents, packetWriter);
				break;
			case TileContentsType.Land:
				SendLand((AvatarFarmOnline.Logic.Stage.Contents.Land)contents, packetWriter);
				break;
			case TileContentsType.Tree:
				SendTree((AvatarFarmOnline.Logic.Stage.Contents.Tree)contents, packetWriter);
				break;
			case TileContentsType.Animal:
				SendAnimal((AvatarFarmOnline.Logic.Stage.Contents.Animal)contents, packetWriter);
				break;
			case TileContentsType.Building:
			case TileContentsType.Decorative:
			case TileContentsType.Tool:
				break;
			}
		}
	}

	private void SendTree(AvatarFarmOnline.Logic.Stage.Contents.Tree t, IPacketWriter packetWriter)
	{
		t.SendData(packetWriter);
	}

	private void SendAnimal(AvatarFarmOnline.Logic.Stage.Contents.Animal a, IPacketWriter packetWriter)
	{
		a.SendData(packetWriter);
	}

	private void SendLand(AvatarFarmOnline.Logic.Stage.Contents.Land l, IPacketWriter packetWriter)
	{
		l.SendData(packetWriter);
	}

	private void SendPlant(AvatarFarmOnline.Logic.Stage.Contents.Plant p, IPacketWriter packetWriter)
	{
		p.SendData(packetWriter);
	}

	public static AvatarFarmOnline.Logic.Stage.FarmData CreateFromPacket(IPacketReader packetReader)
	{
		AvatarFarmOnline.Logic.Stage.FarmData farmData = new AvatarFarmOnline.Logic.Stage.FarmData();
		farmData.ReadPacket(packetReader);
		return farmData;
	}

	private void ReadPacket(IPacketReader packetReader)
	{
		Init(packetReader.ReadInt2());
		int num = packetReader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			ReceiveBuilding(packetReader);
		}
		int num2 = packetReader.ReadInt32();
		for (int j = 0; j < num2; j++)
		{
			ReceiveDecoration(packetReader);
		}
		int num3 = packetReader.ReadInt32();
		for (int k = 0; k < num3; k++)
		{
			ReceiveTool(packetReader);
		}
		for (int l = 0; l < farmSize.X; l++)
		{
			for (int m = 0; m < farmSize.Y; m++)
			{
				ReceiveTile(new Int2(l, m), packetReader);
			}
		}
		nextBuildingId = packetReader.ReadInt32();
		nextDecorationId = packetReader.ReadInt32();
		nextToolId = packetReader.ReadInt32();
		currentSeason = (AvatarFarmOnline.Logic.Seasons)packetReader.ReadInt16();
		nextSeasonChange = packetReader.ReadInt32();
		worldTicks = packetReader.ReadInt32();
		lastDailyReward = packetReader.ReadInt32();
		playerData = new AvatarFarmOnline.Logic.Stage.PlayerData();
		playerData.ReceiveData(packetReader);
	}

	private void ReceiveTile(Int2 position, IPacketReader packetReader)
	{
		if (packetReader.ReadBoolean())
		{
			TileContentsType tileContentsType = (TileContentsType)packetReader.ReadInt16();
			AvatarFarmOnline.Logic.Stage.FarmTile tile = GetTile(position);
			switch (tileContentsType)
			{
			case TileContentsType.Plant:
				ReceivePlant(tile, packetReader);
				break;
			case TileContentsType.Tree:
				ReceiveTree(tile, packetReader);
				break;
			case TileContentsType.Animal:
				ReceiveAnimal(tile, packetReader);
				break;
			case TileContentsType.Land:
				ReceiveLand(tile, packetReader);
				break;
			case TileContentsType.Building:
			case TileContentsType.Decorative:
			case TileContentsType.Tool:
				break;
			}
		}
	}

	private void ReceiveAnimal(AvatarFarmOnline.Logic.Stage.FarmTile farmTile, IPacketReader packetReader)
	{
		AvatarFarmOnline.Logic.Stage.Contents.Animal animal = new AvatarFarmOnline.Logic.Stage.Contents.Animal(farmTile);
		animal.ReceiveData(packetReader);
		farmTile.SetContent(animal);
	}

	private void ReceiveTree(AvatarFarmOnline.Logic.Stage.FarmTile farmTile, IPacketReader packetReader)
	{
		AvatarFarmOnline.Logic.Stage.Contents.Tree tree = new AvatarFarmOnline.Logic.Stage.Contents.Tree(farmTile);
		tree.ReceiveData(packetReader);
		farmTile.SetContent(tree);
	}

	private void ReceivePlant(AvatarFarmOnline.Logic.Stage.FarmTile farmTile, IPacketReader packetReader)
	{
		AvatarFarmOnline.Logic.Stage.Contents.Plant plant = new AvatarFarmOnline.Logic.Stage.Contents.Plant(farmTile);
		plant.ReceiveData(packetReader);
		farmTile.SetContent(plant);
	}

	private void ReceiveLand(AvatarFarmOnline.Logic.Stage.FarmTile farmTile, IPacketReader packetReader)
	{
		AvatarFarmOnline.Logic.Stage.Contents.Land land = new AvatarFarmOnline.Logic.Stage.Contents.Land(farmTile);
		land.ReceiveData(packetReader);
		farmTile.SetContent(land);
	}

	private void ReceiveBuilding(IPacketReader packetReader)
	{
		AvatarFarmOnline.Logic.Stage.Buildings.Building building = AvatarFarmOnline.Logic.Stage.Buildings.Building.ReceiveData(this, packetReader);
		buildings.Add(building);
		GetBuildingBounds(building.Definition, building.TopLeftTile, building.Rotation, out var min, out var max);
		for (int i = min.X; i <= max.X; i++)
		{
			for (int j = min.Y; j <= max.Y; j++)
			{
				AvatarFarmOnline.Logic.Stage.FarmTile farmTile = tiles[i, j];
				farmTile.SetContent(new AvatarFarmOnline.Logic.Stage.Contents.BuildingTile(building, farmTile));
			}
		}
	}

	private void ReceiveDecoration(IPacketReader packetReader)
	{
		AvatarFarmOnline.Logic.Stage.Buildings.Decoration decoration = AvatarFarmOnline.Logic.Stage.Buildings.Decoration.ReceiveData(this, packetReader);
		decorations.Add(decoration);
		GetBuildingBounds(decoration.Definition, decoration.TopLeftTile, decoration.Rotation, out var min, out var max);
		for (int i = min.X; i <= max.X; i++)
		{
			for (int j = min.Y; j <= max.Y; j++)
			{
				AvatarFarmOnline.Logic.Stage.FarmTile farmTile = tiles[i, j];
				farmTile.SetContent(new AvatarFarmOnline.Logic.Stage.Contents.DecorationTile(decoration, farmTile));
			}
		}
	}

	private void ReceiveTool(IPacketReader packetReader)
	{
		AvatarFarmOnline.Logic.Stage.Buildings.Tool tool = AvatarFarmOnline.Logic.Stage.Buildings.Tool.ReceiveData(this, packetReader);
		tools.Add(tool);
		GetBuildingBounds(tool.Definition, tool.TopLeftTile, tool.Rotation, out var min, out var max);
		for (int i = min.X; i <= max.X; i++)
		{
			for (int j = min.Y; j <= max.Y; j++)
			{
				AvatarFarmOnline.Logic.Stage.FarmTile farmTile = tiles[i, j];
				farmTile.SetContent(new AvatarFarmOnline.Logic.Stage.Contents.ToolTile(tool, farmTile));
			}
		}
	}
}
