using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Storage;

namespace Core;

public class GameSettings
{
	public float m_brightness = 5f;

	public float m_sound_volume = 10f;

	public bool m_extras_unlocked;

	public void Clear()
	{
	}

	public static bool Save(GameSettings data, StorageDevice device)
	{
		StorageContainer storageContainer = null;
		FileStream fileStream = null;
		try
		{
			IAsyncResult asyncResult = device.BeginOpenContainer(Game.STORAGE_LOCATION, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			storageContainer = device.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			string sTORAGE_SETTINGS_FILE = Game.STORAGE_SETTINGS_FILE;
			fileStream = (FileStream)storageContainer.OpenFile(sTORAGE_SETTINGS_FILE, FileMode.Create, FileAccess.ReadWrite);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameSettings));
			xmlSerializer.Serialize(fileStream, data);
			fileStream.Close();
			fileStream = null;
			storageContainer.Dispose();
			storageContainer = null;
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine("GameSettings.Save: " + ex.Message);
			if (fileStream != null)
			{
				fileStream.Close();
				fileStream = null;
			}
			if (storageContainer != null)
			{
				storageContainer.Dispose();
				storageContainer = null;
			}
		}
		return false;
	}

	public static GameSettings Load(StorageDevice device)
	{
		GameSettings result = null;
		StorageContainer storageContainer = null;
		FileStream fileStream = null;
		try
		{
			IAsyncResult asyncResult = device.BeginOpenContainer(Game.STORAGE_LOCATION, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			storageContainer = device.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			string sTORAGE_SETTINGS_FILE = Game.STORAGE_SETTINGS_FILE;
			if (storageContainer.FileExists(sTORAGE_SETTINGS_FILE))
			{
				fileStream = (FileStream)storageContainer.OpenFile(sTORAGE_SETTINGS_FILE, FileMode.Open, FileAccess.ReadWrite);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameSettings));
				result = (GameSettings)xmlSerializer.Deserialize(fileStream);
				fileStream.Close();
				fileStream = null;
			}
			storageContainer.Dispose();
			storageContainer = null;
			return result;
		}
		catch (Exception ex)
		{
			Console.WriteLine("GameSettings.Load: " + ex.Message);
			if (fileStream != null)
			{
				fileStream.Close();
				fileStream = null;
			}
			if (storageContainer != null)
			{
				storageContainer.Dispose();
				storageContainer = null;
			}
			return null;
		}
	}
}
