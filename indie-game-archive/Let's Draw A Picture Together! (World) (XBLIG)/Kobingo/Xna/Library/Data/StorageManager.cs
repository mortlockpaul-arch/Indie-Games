using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace Kobingo.Xna.Library.Data;

public class StorageManager
{
	public static StorageDevice Device { get; private set; }

	public static string TitleName { get; set; }

	public static bool IsBusy { get; private set; }

	public static bool HasDevice
	{
		get
		{
			if (Device != null)
			{
				return Device.IsConnected;
			}
			return false;
		}
	}

	public static void PerformOperation(StorageOperationCallback operation)
	{
		if (operation == null)
		{
			throw new ArgumentNullException("operation");
		}
		if (IsBusy)
		{
			throw new InvalidOperationException("Storage is already busy");
		}
		if (Device == null || !Device.IsConnected)
		{
			SelectStorageDevice(operation);
		}
		else
		{
			DoPerformOperationAsync(operation);
		}
	}

	public static void PerformOperation(PlayerIndex playerIndex, StorageOperationCallback operation)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (operation == null)
		{
			throw new ArgumentNullException("operation");
		}
		if (IsBusy)
		{
			throw new InvalidOperationException("Storage is already busy");
		}
		if (Device == null || !Device.IsConnected)
		{
			SelectStorageDevice(playerIndex, operation);
		}
		else
		{
			DoPerformOperationAsync(operation);
		}
	}

	public static void SelectStorageDevice()
	{
		SelectStorageDevice(null);
	}

	public static void SelectStorageDevice(StorageOperationCallback operation)
	{
		Guide.BeginShowStorageDeviceSelector((AsyncCallback)delegate(IAsyncResult result)
		{
			Device = Guide.EndShowStorageDeviceSelector(result);
			if (operation != null)
			{
				DoPerformOperationAsync(operation);
			}
		}, (object)operation);
	}

	public static void SelectStorageDevice(PlayerIndex playerIndex, StorageOperationCallback operation)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Guide.BeginShowStorageDeviceSelector(playerIndex, (AsyncCallback)delegate(IAsyncResult result)
		{
			Device = Guide.EndShowStorageDeviceSelector(result);
			if (operation != null)
			{
				DoPerformOperationAsync(operation);
			}
		}, (object)operation);
	}

	private static void DoPerformOperationAsync(StorageOperationCallback operation)
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			StorageContainer val = null;
			try
			{
				IsBusy = true;
				if (Device != null)
				{
					val = Device.OpenContainer(TitleName);
				}
				operation(val);
			}
			finally
			{
				if (val != null)
				{
					val.Dispose();
					val = null;
				}
				IsBusy = false;
			}
		});
		thread.Start();
	}

	public static void Reset()
	{
		Device = null;
	}
}
