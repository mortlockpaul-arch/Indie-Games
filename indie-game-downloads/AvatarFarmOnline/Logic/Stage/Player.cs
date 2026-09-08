using System;
using System.Collections.Generic;
using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Logic.Stage.Definition;
using FarseerPhysics.Dynamics;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Stage;

internal abstract class Player : AvatarFarmOnline.Logic.Stage.Entity
{
	public enum PlayerState
	{
		Idle,
		Working
	}

	public delegate void EndWorkHandler(AvatarFarmOnline.Logic.Stage.Player player, AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition item, List<AvatarFarmOnline.Logic.Stage.FarmTile> tiles, bool usingTools, int rotation);

	protected PlayerState state;

	protected float rotation;

	private AvatarFarmOnline.Logic.Stage.PlayerSelection selection;

	protected bool usingTools;

	protected bool isRunning;

	protected AvatarFarmOnline.Logic.PlayerPermissions permissions;

	protected List<AvatarFarmOnline.Logic.Stage.FarmTile> workTiles = new List<AvatarFarmOnline.Logic.Stage.FarmTile>(4);

	protected AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition workItemDefinition;

	protected long workEndTime;

	protected AvatarFarmOnline.Logic.WorkType workType;

	protected bool workUsesTools;

	private List<AvatarFarmOnline.Logic.Stage.FarmTile> tempList = new List<AvatarFarmOnline.Logic.Stage.FarmTile>();

	public PlayerState State => state;

	public float Rotation => rotation;

	public int OrientationRotation => ((int)Math.Round((0f - rotation) / ((float)Math.PI / 2f)) + 4) % 4;

	public AvatarFarmOnline.Logic.Stage.PlayerSelection Selection => selection;

	public bool UsingTools => usingTools;

	public bool IsRunning => isRunning;

	public AvatarFarmOnline.Logic.PlayerPermissions Permissions => permissions;

	public List<AvatarFarmOnline.Logic.Stage.FarmTile> WorkTiles => workTiles;

	public AvatarFarmOnline.Logic.WorkType WorkType => workType;

	public bool WorkUsesTools => workUsesTools;

	public abstract int Level { get; }

	public event Action<AvatarFarmOnline.Logic.Stage.Player> OnStartWork;

	public event EndWorkHandler OnEndWork;

	protected Player(AvatarFarmOnline.Logic.Stage.Stage stage, AvatarFarmOnline.Logic.Stage.PlayerSelection selection, AvatarFarmOnline.Logic.PlayerPermissions permissions)
		: base(stage, 0.175f)
	{
		this.permissions = permissions;
		this.selection = selection;
	}

	public void SetPermissions(AvatarFarmOnline.Logic.PlayerPermissions permissions)
	{
		this.permissions = permissions;
	}

	protected override void InitPhysics(World world, float radius)
	{
		base.InitPhysics(world, radius);
	}

	public override void Update()
	{
		base.Update();
	}

	public bool CheckPermissions(AvatarFarmOnline.Logic.PlayerPermissions desiredLevel, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		if (permissions <= desiredLevel)
		{
			failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
			return true;
		}
		failReason = AvatarFarmOnline.Logic.FailedActionReason.NotPermitted;
		return false;
	}

	public abstract void EarnXp(int amount);

	protected void InvokeStartWork()
	{
		if (OnStartWork != null)
		{
			OnStartWork(this);
		}
	}

	protected void InvokeEndWork(AvatarFarmOnline.Logic.WorkType workType, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition itemDefinition, List<AvatarFarmOnline.Logic.Stage.FarmTile> farmTiles, int rotation)
	{
		if (OnEndWork != null)
		{
			OnEndWork(this, workType, itemDefinition, farmTiles, usingTools, rotation);
		}
	}

	protected void EndWork()
	{
		EndWork(workType, workItemDefinition, workTiles, OrientationRotation, usingTools);
	}

	public void EndWork(AvatarFarmOnline.Logic.WorkType work, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition itemDefinition, List<AvatarFarmOnline.Logic.Stage.FarmTile> farmTiles, int rotation, bool usingTools)
	{
		tempList.Clear();
		foreach (AvatarFarmOnline.Logic.Stage.FarmTile farmTile in farmTiles)
		{
			AvatarFarmOnline.Logic.FailedActionReason failReason;
			if (usingTools)
			{
				if (WorkToolTile(farmTile, work, itemDefinition, rotation, out failReason))
				{
					tempList.Add(farmTile);
				}
			}
			else if (farmTile.Work(this, work, itemDefinition, rotation, out failReason))
			{
				tempList.Add(farmTile);
			}
		}
		InvokeEndWork(work, itemDefinition, tempList, rotation);
	}

	private bool WorkToolTile(AvatarFarmOnline.Logic.Stage.FarmTile tile, AvatarFarmOnline.Logic.WorkType work, AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition itemDefinition, int rotation, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		ToolTypes toolTypes = ToolTypes.None;
		switch (work)
		{
		case AvatarFarmOnline.Logic.WorkType.Plow:
			toolTypes = ToolTypes.Plow;
			break;
		case AvatarFarmOnline.Logic.WorkType.Gather:
			toolTypes = ToolTypes.Harvest;
			break;
		case AvatarFarmOnline.Logic.WorkType.Water:
			toolTypes = ToolTypes.Water;
			break;
		case AvatarFarmOnline.Logic.WorkType.GatherTree:
			toolTypes = ToolTypes.Tree;
			break;
		case AvatarFarmOnline.Logic.WorkType.Plant:
			toolTypes = ToolTypes.Plant;
			break;
		}
		if (toolTypes == ToolTypes.None)
		{
			return tile.Work(this, work, itemDefinition, rotation, out failReason);
		}
		if (base.Stage.FarmData.ToolFuel(toolTypes) > 0)
		{
			if (tile.Work(this, work, itemDefinition, rotation, out failReason))
			{
				if (workTiles.Count > 1)
				{
					base.Stage.FarmData.UseFuel(toolTypes);
				}
				return true;
			}
			return false;
		}
		if (tile == workTiles[0])
		{
			return tile.Work(this, work, itemDefinition, rotation, out failReason);
		}
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		return false;
	}

	public AvatarFarmOnline.Logic.Stage.PlayerSnapshot GetSnapshot()
	{
		return new AvatarFarmOnline.Logic.Stage.PlayerSnapshot
		{
			position = entityBody.Position,
			rotation = Rotation,
			speed = base.Speed,
			isRunning = isRunning,
			playerState = state
		};
	}

	public virtual void UpdateData(ref AvatarFarmOnline.Logic.Stage.PlayerSnapshot snapshot, bool fullUpdate)
	{
		if (fullUpdate)
		{
			entityBody.Position = snapshot.position;
			entityBody.Rotation = snapshot.rotation;
			entityBody.LinearVelocity = snapshot.speed;
			isRunning = snapshot.isRunning;
			state = snapshot.playerState;
		}
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public static AvatarFarmOnline.Logic.Stage.Player LoadPlayer(AvatarFarmOnline.Logic.Stage.Stage stage, AvatarFarmOnline.Logic.Stage.PlayerSpawnData psd)
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		AvatarFarmOnline.Logic.PlayerPermissions playerPermissions;
		if (!farmPersistentGameData.IsOnline || farmPersistentGameData.Online.IsHost)
		{
			playerPermissions = AvatarFarmOnline.Logic.PlayerPermissions.Full;
		}
		else
		{
			INetworkGamer remoteHost = farmPersistentGameData.Online.RemoteHost;
			ILocalNetworkGamer localGamer = farmPersistentGameData.Online.LocalGamer;
			playerPermissions = (localGamer.SignedInGamer.IsFriend(remoteHost.Gamer.Gamertag) ? farmPersistentGameData.Online.FriendPermissions : farmPersistentGameData.Online.PublicPermissions);
		}
		AvatarFarmOnline.Logic.Stage.Player player = null;
		switch (psd.Selection.Type)
		{
		case AvatarFarmOnline.Logic.Stage.PlayerSelection.PlayerType.Local:
		{
			AvatarFarmOnline.Logic.Stage.LocalPlayerSelection localPlayerSelection = (AvatarFarmOnline.Logic.Stage.LocalPlayerSelection)psd.Selection;
			AvatarFarmOnline.Logic.Stage.LocalPlayer localPlayer = new AvatarFarmOnline.Logic.Stage.LocalPlayer(stage, localPlayerSelection, playerPermissions);
			player = localPlayer;
			break;
		}
		case AvatarFarmOnline.Logic.Stage.PlayerSelection.PlayerType.Network:
		{
			AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection networkPlayerSelection = (AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection)psd.Selection;
			AvatarFarmOnline.Logic.Stage.NetworkPlayer networkPlayer = new AvatarFarmOnline.Logic.Stage.NetworkPlayer(stage, networkPlayerSelection, playerPermissions);
			networkPlayer.SetLevel(psd.Level);
			player = networkPlayer;
			break;
		}
		}
		player?.SetPosition(psd.Position);
		return player;
	}
}
