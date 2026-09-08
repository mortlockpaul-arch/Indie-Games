using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace Deep_waters;

public class StorageManager(Game game) : GameComponent(game)
{
	public IAsyncResult result;

	private object stateobj;

	private bool GameSaveRequested;

	public bool loadedSettingsrequest;

	public bool filegameexist = true;

	public GameSettings settings;

	public StorageDevice device;

	public bool waitfordeviceready;

	public bool loaded;

	private float timer;

	public override void Initialize()
	{
		base.Initialize();
	}

	public bool requestloadsettings(PlayerIndex controllingplayer)
	{
		if (result == null)
		{
			if (!Guide.IsVisible && !loadedSettingsrequest)
			{
				loadedSettingsrequest = true;
				if (result == null || device == null)
				{
					result = StorageDevice.BeginShowSelector(null, null);
					return false;
				}
				return true;
			}
			return true;
		}
		return true;
	}

	public void requestsavesettings(GameSettings s, PlayerIndex controllingplayer)
	{
		if (!Guide.IsVisible && !GameSaveRequested)
		{
			settings = s;
			GameSaveRequested = true;
			if (result == null || device == null)
			{
				result = StorageDevice.BeginShowSelector(null, null);
			}
			if (device != null && !device.IsConnected)
			{
				GameSaveRequested = false;
			}
		}
	}

	public void requierereset(PlayerIndex controllingplayer)
	{
		if (!Guide.IsVisible && device == null)
		{
			device = null;
			stateobj = string.Concat((object)"GetDevice for Player ", (object)controllingplayer.ToString());
			StorageDevice.BeginShowSelector(null, null);
		}
	}

	public override void Update(GameTime gameTime)
	{
		if (loadedSettingsrequest && result.IsCompleted)
		{
			device = StorageDevice.EndShowSelector(result);
			if (device != null && device.IsConnected)
			{
				filegameexist = DoLoadGame(device);
			}
			loadedSettingsrequest = false;
		}
		if (GameSaveRequested)
		{
			if (device == null)
			{
				device = StorageDevice.EndShowSelector(result);
			}
			if (device != null && device.IsConnected)
			{
				DoSaveGame(device);
			}
			timer = 0f;
			GameSaveRequested = false;
		}
		base.Update(gameTime);
	}

	private void GetDevice(IAsyncResult result)
	{
		if (device == null && !Guide.IsVisible)
		{
			device = StorageDevice.EndShowSelector(result);
		}
		if (device != null && device.IsConnected)
		{
			DoSaveGame(device);
			DoLoadGame(device);
		}
	}

	private void DoSaveGame(StorageDevice device)
	{
		try
		{
			if (result != null)
			{
				result = device.BeginOpenContainer("Dead Sea", null, null);
			}
			if (!device.IsConnected)
			{
				result = device.BeginOpenContainer("Dead Sea", null, null);
			}
			result.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer = device.EndOpenContainer(result);
			result.AsyncWaitHandle.Close();
			string file = "mysettings.sav";
			if (storageContainer.FileExists(file))
			{
				storageContainer.DeleteFile(file);
			}
			Stream stream = storageContainer.CreateFile(file);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameSettings));
			xmlSerializer.Serialize(stream, settings);
			stream.Close();
			storageContainer.Dispose();
			timer = 0f;
		}
		catch (Exception)
		{
			settings.enablesaving = false;
		}
	}

	private bool DoLoadGame(StorageDevice device)
	{
		try
		{
			if (result != null)
			{
				result = device.BeginOpenContainer("Dead Sea", null, null);
			}
			if (!device.IsConnected)
			{
				result = device.BeginOpenContainer("Dead Sea", null, null);
			}
			result.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer = device.EndOpenContainer(result);
			result.AsyncWaitHandle.Close();
			string file = "mysettings.sav";
			if (!storageContainer.FileExists(file))
			{
				storageContainer.Dispose();
				return false;
			}
			Stream stream = storageContainer.OpenFile(file, FileMode.Open);
			settings = new GameSettings();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameSettings));
			settings = (GameSettings)xmlSerializer.Deserialize(stream);
			stream.Close();
			storageContainer.Dispose();
			loaded = true;
			return true;
		}
		catch (Exception)
		{
			settings.enablesaving = false;
			return false;
		}
	}
}
