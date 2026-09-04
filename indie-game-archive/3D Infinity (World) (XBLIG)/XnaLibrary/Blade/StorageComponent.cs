using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace XnaLibrary.Blade;

public class StorageComponent : GameComponent
{
	private StorageDevice storageDevice;

	[CompilerGenerated]
	private PlayerIndex _003CPlayer_003Ek__BackingField;

	public string ContainerName { get; set; }

	public PlayerIndex Player
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CPlayer_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CPlayer_003Ek__BackingField = value;
		}
	}

	public bool IsConnected
	{
		get
		{
			if (storageDevice == null)
			{
				return false;
			}
			return storageDevice.IsConnected;
		}
	}

	public event Action<bool> DeviceSelected;

	public StorageComponent(Game game)
		: base(game)
	{
	}

	protected override void Dispose(bool disposing)
	{
		DisposeStorage();
		((GameComponent)this).Dispose(disposing);
	}

	public void DisposeStorage()
	{
		if (storageDevice != null)
		{
			storageDevice = null;
		}
	}

	public override void Update(GameTime gameTime)
	{
		((GameComponent)this).Update(gameTime);
	}

	public void ClearAllEvents()
	{
		DeviceSelected = null;
	}

	public bool ShowStorageDeviceSelector(string container, PlayerIndex? player, int sizeInBytes, int directoryCount)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (Guide.IsVisible)
		{
			return false;
		}
		ContainerName = container;
		DisposeStorage();
		if (player.HasValue && player.HasValue)
		{
			Guide.BeginShowStorageDeviceSelector(player.Value, sizeInBytes, directoryCount, (AsyncCallback)DeviceSelectCallback, (object)null);
		}
		else
		{
			Guide.BeginShowStorageDeviceSelector(sizeInBytes, directoryCount, (AsyncCallback)DeviceSelectCallback, (object)null);
		}
		return true;
	}

	[Obsolete("このメソッドは容量がチェックできないので推奨していません。")]
	public bool ShowStorageDeviceSelector(string container, PlayerIndex? player)
	{
		return ShowStorageDeviceSelector(container, player, 0, 0);
	}

	private void DeviceSelectCallback(IAsyncResult asyncResult)
	{
		storageDevice = Guide.EndShowStorageDeviceSelector(asyncResult);
		bool obj = storageDevice == null || !storageDevice.IsConnected;
		if (DeviceSelected != null)
		{
			DeviceSelected(obj);
		}
	}

	public StorageContainer OpenContainer()
	{
		return storageDevice.OpenContainer(ContainerName);
	}

	public static string GetStoragePath(StorageContainer container, string fileName)
	{
		if (container == null)
		{
			return string.Empty;
		}
		return Path.Combine(container.Path, fileName);
	}
}
