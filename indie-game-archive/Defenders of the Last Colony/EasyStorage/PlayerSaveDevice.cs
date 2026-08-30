using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace EasyStorage;

public sealed class PlayerSaveDevice : SaveDevice
{
	private const string playerException = "Player {0} must be signed in to get a player specific storage device.";

	public PlayerIndex Player { get; private set; }

	public PlayerSaveDevice(PlayerIndex player)
	{
		Player = player;
	}

	protected override void GetStorageDevice(AsyncCallback callback)
	{
		if (Gamer.SignedInGamers[Player] == null)
		{
			throw new InvalidOperationException($"Player {Player} must be signed in to get a player specific storage device.");
		}
		StorageDevice.BeginShowSelector(Player, callback, null);
	}

	protected override void PrepareEventArgs(SaveDeviceEventArgs args)
	{
		base.PrepareEventArgs(args);
		args.PlayerToPrompt = Player;
	}
}
