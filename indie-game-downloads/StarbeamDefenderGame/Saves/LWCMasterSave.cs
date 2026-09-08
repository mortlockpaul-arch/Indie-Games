using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Storage;

namespace StarbeamDefenderGame.Saves;

public class LWCMasterSave
{
	private LWCMasterSaveStruct savedata;

	private bool wasloaded;

	public bool WasLoadedFromFile => wasloaded;

	public float ScreenScale
	{
		get
		{
			return savedata.screenscale;
		}
		set
		{
			savedata.screenscale = value;
		}
	}

	public LWCMasterSave()
	{
		savedata = default(LWCMasterSaveStruct);
		savedata.ownsgames = new List<string>();
		savedata.version = 1f;
		savedata.screenscale = 0.8f;
		wasloaded = false;
	}

	public void LoadFromStorage(StorageDevice device)
	{
		StorageContainer storageContainer;
		try
		{
			IAsyncResult asyncResult = device.BeginOpenContainer("LWC", null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			storageContainer = device.EndOpenContainer(asyncResult);
		}
		catch
		{
			return;
		}
		try
		{
			if (storageContainer.FileExists("LWCSetup.dat"))
			{
				Stream stream = storageContainer.OpenFile("LWCSetup.dat", FileMode.Open);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(LWCMasterSaveStruct));
				savedata = (LWCMasterSaveStruct)xmlSerializer.Deserialize(stream);
				wasloaded = true;
				stream.Close();
			}
			else
			{
				savedata = default(LWCMasterSaveStruct);
				savedata.ownsgames = new List<string>();
				savedata.ownsgames.Add("AlienSiege");
				savedata.screenscale = 0.8f;
				wasloaded = false;
			}
		}
		catch
		{
			savedata = default(LWCMasterSaveStruct);
			savedata.ownsgames = new List<string>();
			savedata.ownsgames.Add("AlienSiege");
			savedata.screenscale = 0.8f;
			wasloaded = false;
		}
		storageContainer.Dispose();
		SaveToStorage(device);
	}

	public void SaveToStorage(StorageDevice device)
	{
		StorageContainer storageContainer;
		try
		{
			IAsyncResult asyncResult = device.BeginOpenContainer("LWC", null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			storageContainer = device.EndOpenContainer(asyncResult);
		}
		catch
		{
			return;
		}
		try
		{
			if (storageContainer.FileExists("LWCSetup.dat"))
			{
				storageContainer.DeleteFile("LWCSetup.dat");
			}
			Stream stream = storageContainer.OpenFile("LWCSetup.dat", FileMode.CreateNew);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(LWCMasterSaveStruct));
			xmlSerializer.Serialize(stream, savedata);
			stream.Close();
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
		storageContainer.Dispose();
	}

	public void AddGameToFile(string gamename)
	{
		for (int i = 0; i < savedata.ownsgames.Count(); i++)
		{
			if (savedata.ownsgames[i] == gamename)
			{
				return;
			}
		}
		savedata.ownsgames.Add(gamename);
	}
}
