using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace JamSouls;

public static class SaveHandler
{
	private const string SAVE_FILE_PATH = "JamSave.sav";

	public static SaveGameData m_data;

	private static bool m_LoadRequestDone = false;

	private static bool m_SaveRequestDone = false;

	public static void InitSaveHandler()
	{
		m_data.BmgVolume = 5;
		m_data.SfxVolume = 5;
		m_data.bUseSouls = 0;
		m_data.BonusFrequency = new int[10];
		for (int i = 0; i < 10; i++)
		{
			m_data.BonusFrequency[i] = 0;
		}
	}

	public static void ResetState()
	{
		m_LoadRequestDone = false;
		m_SaveRequestDone = false;
	}

	public static void SaveGame(StorageDevice device)
	{
		m_SaveRequestDone = false;
		IAsyncResult asyncResult = null;
		if (device.IsConnected)
		{
			asyncResult = device.BeginOpenContainer("JamsoulsSave", null, null);
		}
		if (asyncResult != null && device.IsConnected)
		{
			asyncResult.AsyncWaitHandle.WaitOne();
		}
		StorageContainer storageContainer = device.EndOpenContainer(asyncResult);
		if (device.IsConnected)
		{
			asyncResult.AsyncWaitHandle.Close();
		}
		if (storageContainer != null && device.IsConnected && storageContainer.FileExists("JamSave.sav"))
		{
			storageContainer.DeleteFile("JamSave.sav");
		}
		if (storageContainer != null && device.IsConnected)
		{
			Stream stream = storageContainer.CreateFile("JamSave.sav");
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveGameData));
			xmlSerializer.Serialize(stream, m_data);
			if (storageContainer != null && storageContainer.StorageDevice != null && storageContainer.StorageDevice.IsConnected)
			{
				stream.Close();
				storageContainer.Dispose();
			}
		}
		m_SaveRequestDone = true;
	}

	public static void LoadGame(StorageDevice device)
	{
		m_LoadRequestDone = false;
		IAsyncResult asyncResult = null;
		if (device.IsConnected)
		{
			asyncResult = device.BeginOpenContainer("JamsoulsSave", null, null);
		}
		if (device.IsConnected)
		{
			asyncResult?.AsyncWaitHandle.WaitOne();
		}
		StorageContainer storageContainer = device.EndOpenContainer(asyncResult);
		if (device.IsConnected && storageContainer != null)
		{
			asyncResult?.AsyncWaitHandle.Close();
		}
		if (storageContainer != null && device.IsConnected)
		{
			if (!storageContainer.FileExists("JamSave.sav"))
			{
				m_LoadRequestDone = true;
				storageContainer.Dispose();
				return;
			}
			Stream stream = storageContainer.OpenFile("JamSave.sav", FileMode.Open);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveGameData));
			m_data = (SaveGameData)xmlSerializer.Deserialize(stream);
			stream.Close();
			storageContainer.Dispose();
		}
		AudioManager.SetSfxVolume(m_data.SfxVolume);
		MediaPlayer.Volume = (float)m_data.BmgVolume / 10f;
		m_LoadRequestDone = true;
	}

	public static void CancelSave()
	{
		m_SaveRequestDone = true;
	}

	public static void CancelLoad()
	{
		m_LoadRequestDone = true;
	}

	public static SaveGameData GetSaveData()
	{
		return m_data;
	}

	public static bool IsLoadRequestDone()
	{
		return m_LoadRequestDone;
	}

	public static bool IsSaveRequestDone()
	{
		return m_SaveRequestDone;
	}
}
