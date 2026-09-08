using System;
using System.IO;
using System.Threading;
using Loot.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace Eyehook.Framework;

public class StorageManager
{
	public delegate void ReadDelegate(BinaryReader reader);

	public delegate void WriteDelegate(BinaryWriter writer);

	private PlayerIndex playerIndex;

	private volatile bool initialized;

	private volatile bool storageEnabled;

	private volatile StorageDevice device;

	private volatile bool initializing;

	private object initializeLock = new object();

	public bool Initialized => initialized;

	public bool Enabled => storageEnabled;

	public StorageDevice Device
	{
		get
		{
			if (!Enabled)
			{
				throw new ResetIODisabledException();
			}
			return device;
		}
	}

	~StorageManager()
	{
	}

	public StorageManager(PlayerIndex playerIndex)
	{
		this.playerIndex = playerIndex;
		StorageDevice.DeviceChanged += deviceChanged;
	}

	public void Dispose()
	{
		StorageDevice.DeviceChanged -= deviceChanged;
	}

	private void deviceChanged(object o, EventArgs args)
	{
		if (Enabled && !device.IsConnected)
		{
			storageEnabled = false;
			MC.ScreenManager.addScreen(new DeviceRemovedScreen());
		}
	}

	private bool deviceIsReady()
	{
		return device != null && device.IsConnected;
	}

	public void Initialize()
	{
		lock (initializeLock)
		{
			while (initializing)
			{
				Thread.Sleep(100);
			}
			initializing = true;
		}
		showDeviceSelector();
	}

	private void showDeviceSelector()
	{
		initialized = false;
		device = null;
		storageEnabled = false;
		if (!GamerManager.HasStorageAccess(playerIndex))
		{
			noDevice();
		}
		else
		{
			new Thread(selectStorageDevice).Start();
		}
	}

	private void selectStorageDevice()
	{
		bool flag = false;
		while (!flag)
		{
			try
			{
				StorageDevice.BeginShowSelector(deviceSelected, null);
				flag = true;
			}
			catch (Exception)
			{
				flag = false;
			}
		}
	}

	private void deviceSelected(IAsyncResult result)
	{
		device = StorageDevice.EndShowSelector(result);
		deviceReady();
	}

	private void noDevice()
	{
		storageEnabled = false;
		initialized = true;
		initializing = false;
	}

	private void deviceReady()
	{
		storageEnabled = true;
		initialized = true;
		initializing = false;
	}

	public StorageContainer OpenContainer(string containerName)
	{
		if (!Enabled)
		{
			throw new ResetIODisabledException();
		}
		IAsyncResult asyncResult = device.BeginOpenContainer(containerName, null, null);
		asyncResult.AsyncWaitHandle.WaitOne();
		return device.EndOpenContainer(asyncResult);
	}

	public bool Exists(string containerName, string fileName)
	{
		try
		{
			using StorageContainer storageContainer = OpenContainer(containerName);
			return storageContainer.FileExists(fileName);
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public bool Delete(string containerName, string fileName)
	{
		try
		{
			using StorageContainer storageContainer = OpenContainer(containerName);
			storageContainer.DeleteFile(fileName);
			return true;
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public bool DeleteAll(string containerName, string pattern)
	{
		try
		{
			using StorageContainer storageContainer = OpenContainer(containerName);
			string[] fileNames = storageContainer.GetFileNames(pattern);
			foreach (string file in fileNames)
			{
				storageContainer.DeleteFile(file);
			}
			return true;
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public void Load(string containerName, string fileName, ReadDelegate reader)
	{
		try
		{
			using StorageContainer storageContainer = OpenContainer(containerName);
			using Stream input = storageContainer.OpenFile(fileName, FileMode.Open, FileAccess.Read);
			using BinaryReader binaryReader = new BinaryReader(input);
			reader(binaryReader);
			binaryReader.Close();
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public void Save(string containerName, string fileName, WriteDelegate writer)
	{
		try
		{
			using StorageContainer storageContainer = OpenContainer(containerName);
			using Stream output = storageContainer.OpenFile(fileName, FileMode.Create);
			using BinaryWriter binaryWriter = new BinaryWriter(output);
			writer(binaryWriter);
			binaryWriter.Close();
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}
}
