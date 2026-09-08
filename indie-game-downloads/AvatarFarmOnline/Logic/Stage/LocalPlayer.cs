using System;
using System.Collections.Generic;
using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Logic.Stage.Contents;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Logic.Mode;
using Quasar.Global;
using Quasar.Input;

namespace AvatarFarmOnline.Logic.Stage;

internal class LocalPlayer : AvatarFarmOnline.Logic.Stage.Player
{
	public enum CameraStates
	{
		Normal,
		Cenital,
		Isometric,
		World,
		Count
	}

	private const long NETWORK_UPDATE_INTERVAL = 75L;

	private ControlState controlState;

	private AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition shopItem;

	private CameraStates cameraState;

	private DefaultXboxGamepadGroup inputGroup;

	private AvatarFarmOnline.Logic.Stage.LocalPlayerSelection playerSelection;

	private List<AvatarFarmOnline.Logic.Stage.FarmTile> workableTiles = new List<AvatarFarmOnline.Logic.Stage.FarmTile>(4);

	private AvatarFarmOnline.Logic.Stage.FarmTile highlightedTile;

	private long lastNetworkUpdate;

	private static List<AvatarFarmOnline.Logic.Stage.FarmTile> tempFarmTileList = new List<AvatarFarmOnline.Logic.Stage.FarmTile>(1);

	public ControlState ControlState => controlState;

	public AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition ShopItem => shopItem;

	public CameraStates CameraState => cameraState;

	public DefaultXboxGamepadGroup InputGroup => inputGroup;

	public AvatarFarmOnline.Logic.Stage.LocalPlayerSelection PlayerSelection => playerSelection;

	public PlayerIndex Owner => playerSelection.PlayerIndex;

	public List<AvatarFarmOnline.Logic.Stage.FarmTile> WorkableTiles => workableTiles;

	public AvatarFarmOnline.Logic.Stage.FarmTile HighlightedTile => highlightedTile;

	public override int Level => PlayerSelection.PlayerExperience.Level;

	public event Action<AvatarFarmOnline.Logic.Stage.Player> OnCameraStateChanged;

	public event Action<AvatarFarmOnline.Logic.Stage.Player> OnUseToolsChanged;

	public event Action<AvatarFarmOnline.Logic.FailedActionReason> OnFailedAction;

	public event Action<AvatarFarmOnline.Logic.Stage.FarmTile> OnChangeHighlightedTile;

	public void SetCameraState(CameraStates state)
	{
		cameraState = state;
		if (OnCameraStateChanged != null)
		{
			OnCameraStateChanged(this);
		}
	}

	public void SetUsingTools(bool value)
	{
		usingTools = value;
		if (OnUseToolsChanged != null)
		{
			OnUseToolsChanged(this);
		}
	}

	public void SetHighlightedTile(AvatarFarmOnline.Logic.Stage.FarmTile t)
	{
		if (t != highlightedTile)
		{
			highlightedTile = t;
			if (OnChangeHighlightedTile != null)
			{
				OnChangeHighlightedTile(t);
			}
		}
	}

	public LocalPlayer(AvatarFarmOnline.Logic.Stage.Stage stage, AvatarFarmOnline.Logic.Stage.LocalPlayerSelection selection, AvatarFarmOnline.Logic.PlayerPermissions permissions)
		: base(stage, selection, permissions)
	{
		playerSelection = selection;
		inputGroup = InputManager.Instance.CreateXboxGamepadGroup(Owner);
		InputManager.Instance.AddInputGroup(inputGroup);
		SetCameraState(AvatarFarmOnline.AvatarFarmOnlineConfig.Instance.CameraMode);
	}

	public override void EarnXp(int amount)
	{
		if (PlayerSelection.PlayerExperience.EarnXp(amount) && GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData { IsOnline: not false } farmPersistentGameData)
		{
			farmPersistentGameData.Online.SendPlayerLevelInfo(base.Stage.LocalPlayer);
		}
	}

	public override void Update()
	{
		if (base.Stage.IsInPlayableState && !base.Stage.IsShowingMessage && cameraState == CameraStates.Normal)
		{
			Vector2 vector = inputGroup.Aim();
			if (AvatarFarmOnline.AvatarFarmOnlineConfig.Instance.InvertYAxis)
			{
				vector.Y *= -1f;
			}
			orientation += vector * AvatarFarmOnline.Logic.GameGlobals.PlayerRotateSpeed * base.Stage.Timer.LastIntervalSeconds;
			orientation.Y = GameMath.Clamp(-1f, 0.2f, orientation.Y);
		}
		switch (state)
		{
		case PlayerState.Idle:
			if (base.Stage.IsInPlayableState && !base.Stage.IsShowingMessage && cameraState != CameraStates.World)
			{
				Vector2 vector2 = inputGroup.Movement();
				switch (cameraState)
				{
				case CameraStates.Normal:
					vector2 = GameMath.RotateVector(vector2, 0f - orientation.X);
					vector2.Y = 0f - vector2.Y;
					break;
				case CameraStates.Cenital:
					vector2.Y = 0f - vector2.Y;
					break;
				case CameraStates.Isometric:
					vector2.X = 0f - vector2.X;
					vector2 = GameMath.RotateVector(vector2, (float)Math.PI * 3f / 4f);
					break;
				}
				if (vector2.LengthSquared() > 1f)
				{
					vector2.Normalize();
				}
				isRunning = inputGroup.RightTriggerState() && vector2.Length() > 0.5f;
				float num = (isRunning ? 5f : 2.75f);
				entityBody.LinearVelocity = vector2 * num;
				if (entityBody.LinearVelocity.LengthSquared() > 0f)
				{
					rotation = GameMath.VectorAngle(new Vector2(entityBody.LinearVelocity.X, 0f - entityBody.LinearVelocity.Y));
				}
				Int2 tile = AvatarFarmOnline.Logic.GameGlobals.PositionTile(entityBody.Position);
				SetHighlightedTile(base.Stage.FarmData.GetTile(tile));
				UpdateWorkableTiles();
				if (inputGroup.ButtonA())
				{
					ContextualAction();
				}
				else if (inputGroup.ButtonX())
				{
					Recycle();
				}
				else if (inputGroup.ButtonY())
				{
					Shop(goToPlantsCategory: false);
				}
				else if (inputGroup.ButtonB())
				{
					Cancel();
				}
				else if (inputGroup.RightShoulder())
				{
					usingTools = !usingTools;
					if (OnUseToolsChanged != null)
					{
						OnUseToolsChanged(this);
					}
				}
			}
			else
			{
				entityBody.LinearVelocity = Vector2.Zero;
			}
			break;
		case PlayerState.Working:
			entityBody.LinearVelocity = Vector2.Zero;
			if (base.Stage.Timer.TotalTime <= workEndTime)
			{
				break;
			}
			EndTask();
			goto case PlayerState.Idle;
		}
		if (base.Stage.IsInPlayableState && inputGroup.LeftShoulder())
		{
			cameraState = (CameraStates)((int)(cameraState + 1) % 4);
			if (OnCameraStateChanged != null)
			{
				OnCameraStateChanged(this);
			}
		}
		if (GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData { IsOnline: not false } farmPersistentGameData && Timer.DefaultTimer.TotalTime > lastNetworkUpdate + 75)
		{
			lastNetworkUpdate = Timer.DefaultTimer.TotalTime;
			farmPersistentGameData.Online.SendPlayerUpdate(base.Stage.LocalPlayer);
		}
		base.Update();
	}

	public void StartWorking(AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.FarmTile workTile)
	{
		StartWork(workType, null, workTile);
	}

	public void StartWorking(AvatarFarmOnline.Logic.WorkType workType, List<AvatarFarmOnline.Logic.Stage.FarmTile> workTiles)
	{
		StartWork(workType, null, workTiles);
	}

	public virtual void StartBuilding(AvatarFarmOnline.Logic.Stage.FarmTile workTile, AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition building)
	{
		StartWork(AvatarFarmOnline.Logic.WorkType.Build, building, workTile);
	}

	private void StartWork(AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item, List<AvatarFarmOnline.Logic.Stage.FarmTile> workTiles)
	{
		if (state == PlayerState.Idle)
		{
			state = PlayerState.Working;
			base.workType = workType;
			workItemDefinition = item;
			base.workTiles.Clear();
			if (workTiles != null)
			{
				base.workTiles.AddRange(workTiles);
			}
			int num = AvatarFarmOnline.Logic.GameGlobals.WorkDuration(workType);
			workEndTime = base.Stage.Timer.TotalTime + num;
			Gamepad.Instance(Owner).vibrateFor(0.1f, 0.05f, (uint)num);
			entityBody.LinearVelocity = Vector2.Zero;
			if (GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData { IsOnline: not false } farmPersistentGameData)
			{
				farmPersistentGameData.Online.SendStartWork(workType, item, base.workTiles);
			}
			InvokeStartWork();
		}
	}

	private void StartWork(AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item, AvatarFarmOnline.Logic.Stage.FarmTile mainTile)
	{
		tempFarmTileList.Clear();
		tempFarmTileList.Add(mainTile);
		StartWork(workType, item, tempFarmTileList);
		tempFarmTileList.Clear();
	}

	public void StartPlanting(List<AvatarFarmOnline.Logic.Stage.FarmTile> workTiles, AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition plant)
	{
		StartWork(AvatarFarmOnline.Logic.WorkType.Plant, plant, workTiles);
	}

	public void StartPlantingTree(AvatarFarmOnline.Logic.Stage.FarmTile workTile, AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition plant)
	{
		StartWork(AvatarFarmOnline.Logic.WorkType.PlantTree, plant, workTile);
	}

	public void StartPlantingAnimal(AvatarFarmOnline.Logic.Stage.FarmTile workTile, AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition plant)
	{
		StartWork(AvatarFarmOnline.Logic.WorkType.PlantAnimal, plant, workTile);
	}

	protected void EndTask()
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		if (farmPersistentGameData.IsOnline)
		{
			farmPersistentGameData.Online.SendEndWork(base.Selection.Id, workType, workItemDefinition, workTiles, base.OrientationRotation, usingTools);
		}
		if (!farmPersistentGameData.IsOnline || farmPersistentGameData.Online.IsHost)
		{
			EndWork();
		}
		state = PlayerState.Idle;
	}

	private void UpdateWorkableTiles()
	{
		workableTiles.Clear();
		workUsesTools = false;
		if (!base.UsingTools)
		{
			workableTiles.Add(HighlightedTile);
			return;
		}
		ToolTypes toolTypes = ToolForContextualAction();
		if (toolTypes == ToolTypes.None)
		{
			workableTiles.Add(HighlightedTile);
			return;
		}
		if (!base.Stage.FarmData.GetUsableToolSize(toolTypes, out var toolSize))
		{
			workableTiles.Add(HighlightedTile);
			return;
		}
		_ = base.Position / 2f;
		workableTiles.Add(HighlightedTile);
		for (int i = 0; i < toolSize; i++)
		{
			for (int j = 0; j < toolSize; j++)
			{
				float x = (float)i - (float)(toolSize - 1) * 0.5f;
				float y = (float)j - (float)(toolSize - 1) * 0.5f;
				Int2 tile = AvatarFarmOnline.Logic.GameGlobals.PositionTile(entityBody.Position + new Vector2(x, y) * 2f);
				AvatarFarmOnline.Logic.Stage.FarmTile tile2 = base.Stage.FarmData.GetTile(tile);
				if (tile2 != null && tile2 != HighlightedTile && CanUseTool(tile2, toolTypes) && base.Stage.FarmData.ToolFuel(toolTypes) > i)
				{
					workableTiles.Add(tile2);
					workUsesTools = true;
				}
			}
		}
	}

	public void Recycle()
	{
		if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Full, out var failReason))
		{
			if (highlightedTile != null)
			{
				if (controlState != ControlState.Standard)
				{
					return;
				}
				if (!highlightedTile.IsEmpty)
				{
					StartWorking(AvatarFarmOnline.Logic.WorkType.Recycle, highlightedTile);
					return;
				}
			}
			if (OnFailedAction != null)
			{
				OnFailedAction(AvatarFarmOnline.Logic.FailedActionReason.CantRecycle);
			}
		}
		else
		{
			OnFailedAction(failReason);
		}
	}

	private bool ContextualAction(bool doIt, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		AvatarFarmOnline.Logic.Stage.FarmTile farmTile = highlightedTile;
		if (farmTile == null)
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.NoTile;
			return false;
		}
		switch (ControlState)
		{
		case ControlState.Standard:
			if (farmTile.IsEmpty)
			{
				if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason))
				{
					if (base.Stage.FarmData.CanPlow(farmTile, out failReason))
					{
						if (doIt)
						{
							StartWorking(AvatarFarmOnline.Logic.WorkType.Plow, WorkableTiles);
						}
						return true;
					}
					return false;
				}
				return false;
			}
			switch (farmTile.Contents.Type)
			{
			case TileContentsType.Building:
			{
				AvatarFarmOnline.Logic.Stage.Contents.BuildingTile buildingTile = farmTile.Contents as AvatarFarmOnline.Logic.Stage.Contents.BuildingTile;
				if (CheckPermissions(buildingTile.Building.IsGamble ? AvatarFarmOnline.Logic.PlayerPermissions.Build : AvatarFarmOnline.Logic.PlayerPermissions.Harvest, out failReason))
				{
					if (buildingTile.Building.CanBeGathered(out failReason))
					{
						if (doIt)
						{
							StartWorking(AvatarFarmOnline.Logic.WorkType.GatherBuilding, farmTile);
						}
						return true;
					}
					return false;
				}
				return false;
			}
			case TileContentsType.Tool:
			{
				AvatarFarmOnline.Logic.Stage.Contents.ToolTile toolTile = farmTile.Contents as AvatarFarmOnline.Logic.Stage.Contents.ToolTile;
				if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason))
				{
					if (toolTile.Tool.CanBeRefilled(out failReason))
					{
						if (doIt)
						{
							StartWorking(AvatarFarmOnline.Logic.WorkType.Refill, farmTile);
						}
						return true;
					}
					return false;
				}
				return false;
			}
			case TileContentsType.Land:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Land land = farmTile.Contents as AvatarFarmOnline.Logic.Stage.Contents.Land;
				switch (land.State)
				{
				case AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Gathered:
					if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason))
					{
						if (base.Stage.FarmData.CanPlow(farmTile, out failReason))
						{
							if (doIt)
							{
								StartWorking(AvatarFarmOnline.Logic.WorkType.Plow, WorkableTiles);
							}
							return true;
						}
						return false;
					}
					return false;
				case AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Plowed:
					if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason))
					{
						if (doIt)
						{
							Shop(goToPlantsCategory: true);
						}
						failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
						return true;
					}
					return false;
				}
				break;
			}
			case TileContentsType.Plant:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Plant plant = farmTile.Contents as AvatarFarmOnline.Logic.Stage.Contents.Plant;
				switch (plant.State)
				{
				case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Growing:
					if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Harvest, out failReason))
					{
						if (doIt)
						{
							StartWorking(AvatarFarmOnline.Logic.WorkType.Water, WorkableTiles);
						}
						failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
						return true;
					}
					return false;
				case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.ReadyToGather:
				case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Withered:
					if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Harvest, out failReason))
					{
						if (doIt)
						{
							StartWorking(AvatarFarmOnline.Logic.WorkType.Gather, WorkableTiles);
						}
						failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
						return true;
					}
					return false;
				}
				break;
			}
			case TileContentsType.Tree:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Tree tree = farmTile.Contents as AvatarFarmOnline.Logic.Stage.Contents.Tree;
				if (tree.State != AvatarFarmOnline.Logic.Stage.Contents.Tree.TreeState.ReadyToGather)
				{
					break;
				}
				if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Harvest, out failReason))
				{
					if (doIt)
					{
						StartWorking(AvatarFarmOnline.Logic.WorkType.GatherTree, WorkableTiles);
					}
					failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
					return true;
				}
				return false;
			}
			case TileContentsType.Animal:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Animal animal = farmTile.Contents as AvatarFarmOnline.Logic.Stage.Contents.Animal;
				switch (animal.State)
				{
				case AvatarFarmOnline.Logic.Stage.Contents.Animal.AnimalState.ReadyToGather:
					if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Harvest, out failReason))
					{
						if (doIt)
						{
							StartWorking(AvatarFarmOnline.Logic.WorkType.GatherAnimal, WorkableTiles);
						}
						failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
						return true;
					}
					return false;
				case AvatarFarmOnline.Logic.Stage.Contents.Animal.AnimalState.NeedsFood:
					if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason) && animal.CanBeFeed(out failReason))
					{
						if (doIt)
						{
							StartWorking(AvatarFarmOnline.Logic.WorkType.FeedAnimal, farmTile);
						}
						failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
						return true;
					}
					return false;
				case AvatarFarmOnline.Logic.Stage.Contents.Animal.AnimalState.Growing:
					failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
					return false;
				}
				break;
			}
			}
			break;
		case ControlState.Planting:
			switch (shopItem.Category)
			{
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Building:
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tool:
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Decoration:
				if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason))
				{
					if (base.Stage.FarmData.CanBuy(shopItem, out failReason) && base.Stage.FarmData.CanPlaceBuilding(shopItem as AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition, farmTile.Tile, base.OrientationRotation, out failReason))
					{
						if (doIt)
						{
							StartBuilding(farmTile, shopItem as AvatarFarmOnline.Logic.Stage.Definition.BaseBuildingDefinition);
						}
						return true;
					}
					return false;
				}
				return false;
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Tree:
				if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason))
				{
					if (base.Stage.FarmData.CanBuy(shopItem, out failReason) && farmTile.CanPlantTree(shopItem as AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition, out failReason))
					{
						if (doIt)
						{
							StartPlantingTree(farmTile, shopItem as AvatarFarmOnline.Logic.Stage.Definition.TreeDefinition);
						}
						return true;
					}
					return false;
				}
				return false;
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant:
				if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason))
				{
					if (base.Stage.FarmData.CanBuy(shopItem, out failReason) && farmTile.CanPlant(shopItem as AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition, out failReason))
					{
						if (doIt)
						{
							StartPlanting(WorkableTiles, shopItem as AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition);
						}
						return true;
					}
					return false;
				}
				return false;
			case AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Animal:
				if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason))
				{
					if (base.Stage.FarmData.CanBuy(shopItem, out failReason) && farmTile.CanPlantAnimal(shopItem as AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition, out failReason))
					{
						if (doIt)
						{
							StartPlantingAnimal(farmTile, shopItem as AvatarFarmOnline.Logic.Stage.Definition.AnimalDefinition);
						}
						return true;
					}
					return false;
				}
				return false;
			}
			break;
		}
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		return false;
	}

	public void Shop(bool goToPlantsCategory)
	{
		if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out var failReason))
		{
			base.Stage.Shop(goToPlantsCategory);
		}
		else
		{
			OnFailedAction(failReason);
		}
	}

	public void EndShop(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition shopItem)
	{
		if (shopItem != null)
		{
			controlState = ControlState.Planting;
			this.shopItem = shopItem;
		}
		else
		{
			controlState = ControlState.Standard;
		}
	}

	public bool ContextualAction()
	{
		bool flag = ContextualAction(doIt: true, out var failReason);
		if (!flag && OnFailedAction != null)
		{
			OnFailedAction(failReason);
		}
		return flag;
	}

	public bool CanPerformContextualAction(out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		return ContextualAction(doIt: false, out failReason);
	}

	public ToolTypes ToolForContextualAction()
	{
		AvatarFarmOnline.Logic.Stage.FarmTile farmTile = HighlightedTile;
		AvatarFarmOnline.Logic.FailedActionReason failReason;
		switch (controlState)
		{
		case ControlState.Standard:
			if (farmTile.IsEmpty)
			{
				if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason) && base.Stage.FarmData.CanPlow(farmTile, out failReason))
				{
					return ToolTypes.Plow;
				}
				break;
			}
			switch (farmTile.Contents.Type)
			{
			case TileContentsType.Land:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Land land = farmTile.Contents as AvatarFarmOnline.Logic.Stage.Contents.Land;
				AvatarFarmOnline.Logic.Stage.Contents.Land.LandState landState = land.State;
				if (landState == AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Gathered && CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason) && base.Stage.FarmData.CanPlow(farmTile, out failReason))
				{
					return ToolTypes.Plow;
				}
				break;
			}
			case TileContentsType.Plant:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Plant plant = farmTile.Contents as AvatarFarmOnline.Logic.Stage.Contents.Plant;
				switch (plant.State)
				{
				case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Growing:
					if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Harvest, out failReason))
					{
						return ToolTypes.Water;
					}
					break;
				case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.ReadyToGather:
				case AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Withered:
					if (CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Harvest, out failReason))
					{
						return ToolTypes.Harvest;
					}
					break;
				}
				break;
			}
			case TileContentsType.Tree:
			{
				AvatarFarmOnline.Logic.Stage.Contents.Tree tree = farmTile.Contents as AvatarFarmOnline.Logic.Stage.Contents.Tree;
				AvatarFarmOnline.Logic.Stage.Contents.Tree.TreeState treeState = tree.State;
				if (treeState == AvatarFarmOnline.Logic.Stage.Contents.Tree.TreeState.ReadyToGather && CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Harvest, out failReason))
				{
					return ToolTypes.Tree;
				}
				break;
			}
			}
			break;
		case ControlState.Planting:
		{
			AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory category = shopItem.Category;
			if (category == AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant && CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions.Build, out failReason) && base.Stage.FarmData.CanBuy(shopItem, out failReason) && farmTile.CanPlant(shopItem as AvatarFarmOnline.Logic.Stage.Definition.PlantDefinition, out failReason))
			{
				return ToolTypes.Plant;
			}
			break;
		}
		}
		return ToolTypes.None;
	}

	public bool CanUseTool(AvatarFarmOnline.Logic.Stage.FarmTile farmTile, ToolTypes toolType)
	{
		if (farmTile == null)
		{
			return false;
		}
		switch (toolType)
		{
		case ToolTypes.Harvest:
			if (farmTile.IsEmpty)
			{
				return false;
			}
			if (farmTile.Contents.Type != TileContentsType.Plant)
			{
				return false;
			}
			if (((AvatarFarmOnline.Logic.Stage.Contents.Plant)farmTile.Contents).State != AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.ReadyToGather && ((AvatarFarmOnline.Logic.Stage.Contents.Plant)farmTile.Contents).State != AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Withered)
			{
				return false;
			}
			return true;
		case ToolTypes.Water:
			if (farmTile.IsEmpty)
			{
				return false;
			}
			if (farmTile.Contents.Type != TileContentsType.Plant)
			{
				return false;
			}
			if (((AvatarFarmOnline.Logic.Stage.Contents.Plant)farmTile.Contents).State != AvatarFarmOnline.Logic.Stage.Contents.Plant.PlantState.Growing)
			{
				return false;
			}
			return true;
		case ToolTypes.Plow:
			if (farmTile.IsEmpty)
			{
				return true;
			}
			switch (farmTile.Contents.Type)
			{
			case TileContentsType.Land:
				if (((AvatarFarmOnline.Logic.Stage.Contents.Land)farmTile.Contents).State != AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Gathered)
				{
					return false;
				}
				return true;
			case TileContentsType.Plant:
				return false;
			default:
				return false;
			}
		case ToolTypes.Plant:
			if (farmTile.IsEmpty)
			{
				return false;
			}
			if (farmTile.Contents.Type != TileContentsType.Land)
			{
				return false;
			}
			if (((AvatarFarmOnline.Logic.Stage.Contents.Land)farmTile.Contents).State != AvatarFarmOnline.Logic.Stage.Contents.Land.LandState.Plowed)
			{
				return false;
			}
			return true;
		case ToolTypes.Tree:
			if (farmTile.IsEmpty)
			{
				return false;
			}
			if (farmTile.Contents.Type != TileContentsType.Tree)
			{
				return false;
			}
			if (((AvatarFarmOnline.Logic.Stage.Contents.Tree)farmTile.Contents).State != AvatarFarmOnline.Logic.Stage.Contents.Tree.TreeState.ReadyToGather)
			{
				return false;
			}
			return true;
		default:
			return false;
		}
	}

	public void Cancel()
	{
		switch (controlState)
		{
		case ControlState.Planting:
			controlState = ControlState.Standard;
			break;
		case ControlState.Standard:
			break;
		}
	}

	public override void Dispose()
	{
		InputManager.Instance.RemoveGroup(inputGroup);
		base.Dispose();
	}
}
