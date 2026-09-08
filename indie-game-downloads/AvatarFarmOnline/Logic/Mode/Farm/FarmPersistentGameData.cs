using System;
using AvatarFarmOnline.Logic.Mode.Farm.Online;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Mode.Farm;

internal class FarmPersistentGameData : SimplePersistentGameData<AvatarFarmOnline.Logic.Mode.Farm.FarmGameSetup>
{
	public const SessionType ONLINE_MODE = SessionType.Online;

	private AvatarFarmOnline.Logic.Mode.Farm.FarmManager farmManager;

	private AvatarFarmOnline.Logic.Mode.Farm.FarmHeader currentFarm;

	private AvatarFarmOnline.Logic.Mode.Farm.Online.Online online;

	public AvatarFarmOnline.Logic.Mode.Farm.FarmManager FarmManager => farmManager;

	public AvatarFarmOnline.Logic.Mode.Farm.FarmHeader CurrentFarm => currentFarm;

	public AvatarFarmOnline.Logic.Mode.Farm.Online.Online Online => online;

	public bool IsOnline
	{
		get
		{
			if (online != null)
			{
				return online.Active;
			}
			return false;
		}
	}

	public FarmPersistentGameData(AvatarFarmOnline.Logic.Mode.Farm.FarmGameSetup config)
		: base(config)
	{
		farmManager = new AvatarFarmOnline.Logic.Mode.Farm.FarmManager(this);
	}

	public void InitOnline()
	{
		if (online != null)
		{
			online.Dispose();
		}
		online = new AvatarFarmOnline.Logic.Mode.Farm.Online.Online(this, SessionType.Online);
	}

	public void DisposeOnline()
	{
		if (online != null)
		{
			online.Dispose();
			online = null;
		}
	}

	public override void Update()
	{
		base.Update();
		if (online != null)
		{
			online.Update();
		}
	}

	public void SetFarm(AvatarFarmOnline.Logic.Mode.Farm.FarmHeader header)
	{
		currentFarm = header;
	}

	protected override GameData DoPrepareNextRound()
	{
		if (currentFarm.IsOnline)
		{
			InitOnline();
			AvatarFarmOnline.Logic.Mode.Farm.Online.Online.HostMode hostMode = currentFarm.PlayMode switch
			{
				PlayMode.Private => AvatarFarmOnline.Logic.Mode.Farm.Online.Online.HostMode.Private, 
				_ => AvatarFarmOnline.Logic.Mode.Farm.Online.Online.HostMode.Public, 
			};
			if (!online.SetupMultiplayer(hostMode, currentFarm.FriendPermissions, currentFarm.PublicPermissions))
			{
				throw new Exception("Could not create game");
			}
		}
		return new AvatarFarmOnline.Logic.Mode.Farm.FarmGameData(this);
	}

	public override void RoundEnded()
	{
		base.RoundEnded();
		if (!IsOnline || online.IsHost)
		{
			farmManager.Save();
		}
	}

	public override void Dispose()
	{
		DisposeOnline();
		base.Dispose();
	}
}
