using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Storage;
using StarbeamDefenderGame.Saves;

namespace StarbeamDefenderGame;

public class SavesManager
{
	private StorageDevice device;

	private GameSave singleplayerscores;

	private LWCMasterSave lwcsave;

	private GameSaveMulti multiplayerscores;

	public bool NeedsSetup
	{
		get
		{
			if (!lwcsave.WasLoadedFromFile)
			{
				return true;
			}
			return false;
		}
	}

	public GameSave SinglePlayerScores
	{
		get
		{
			return singleplayerscores;
		}
		set
		{
			singleplayerscores = value;
		}
	}

	public GameSaveMulti MultiplayerScores
	{
		get
		{
			return multiplayerscores;
		}
		set
		{
			multiplayerscores = value;
		}
	}

	public LWCMasterSave LWCMasterFile
	{
		get
		{
			return lwcsave;
		}
		set
		{
			lwcsave = value;
		}
	}

	public SavesManager(StorageDevice device)
	{
		this.device = device;
		singleplayerscores = new GameSave();
		multiplayerscores = new GameSaveMulti();
		lwcsave = new LWCMasterSave();
	}

	public void LoadFiles()
	{
		if (device == null)
		{
			return;
		}
		StorageContainer storageContainer;
		try
		{
			IAsyncResult asyncResult = device.BeginOpenContainer("AlienSiege", null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			storageContainer = device.EndOpenContainer(asyncResult);
		}
		catch
		{
			return;
		}
		try
		{
			if (storageContainer.FileExists("singlesaves.dat"))
			{
				Stream stream = storageContainer.OpenFile("singlesaves.dat", FileMode.Open);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameSave));
				singleplayerscores = (GameSave)xmlSerializer.Deserialize(stream);
				if (singleplayerscores == null)
				{
					singleplayerscores = new GameSave();
				}
				stream.Close();
			}
			else
			{
				singleplayerscores = new GameSave();
			}
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
		try
		{
			if (storageContainer.FileExists("multiplayerscores.dat"))
			{
				Stream stream2 = storageContainer.OpenFile("multiplayerscores.dat", FileMode.Open);
				XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(GameSaveMulti));
				multiplayerscores = (GameSaveMulti)xmlSerializer2.Deserialize(stream2);
				if (multiplayerscores == null)
				{
					multiplayerscores = new GameSaveMulti();
				}
				stream2.Close();
			}
			else
			{
				multiplayerscores = new GameSaveMulti();
			}
		}
		catch
		{
		}
		storageContainer.Dispose();
		lwcsave.LoadFromStorage(device);
	}

	public void SaveFiles()
	{
		if (device == null)
		{
			return;
		}
		StorageContainer storageContainer;
		try
		{
			IAsyncResult asyncResult = device.BeginOpenContainer("AlienSiege", null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			storageContainer = device.EndOpenContainer(asyncResult);
		}
		catch
		{
			return;
		}
		try
		{
			if (storageContainer.FileExists("singlesaves.dat"))
			{
				storageContainer.DeleteFile("singlesaves.dat");
			}
			Stream stream = storageContainer.OpenFile("singlesaves.dat", FileMode.CreateNew);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameSave));
			xmlSerializer.Serialize(stream, singleplayerscores);
			stream.Close();
		}
		catch
		{
		}
		try
		{
			if (storageContainer.FileExists("multiplayerscores.dat"))
			{
				storageContainer.DeleteFile("multiplayerscores.dat");
			}
			Stream stream2 = storageContainer.OpenFile("multiplayerscores.dat", FileMode.CreateNew);
			XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(GameSaveMulti));
			xmlSerializer2.Serialize(stream2, multiplayerscores);
			stream2.Close();
		}
		catch
		{
		}
		storageContainer.Dispose();
		lwcsave.AddGameToFile("AlienSiege");
		lwcsave.SaveToStorage(device);
	}
}
