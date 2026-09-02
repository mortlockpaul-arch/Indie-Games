using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Storage;

namespace Game;

[Serializable]
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
		StorageContainer val = null;
		FileStream fileStream = null;
		try
		{
			val = device.OpenContainer(Game.STORAGE_LOCATION);
			string path = Path.Combine(val.Path, Game.STORAGE_SETTINGS_FILE);
			fileStream = File.Open(path, FileMode.Create, FileAccess.ReadWrite);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameSettings));
			xmlSerializer.Serialize(fileStream, data);
			fileStream.Close();
			fileStream = null;
			val.Dispose();
			val = null;
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
			if (val != null)
			{
				val.Dispose();
				val = null;
			}
		}
		return false;
	}

	public static GameSettings Load(StorageDevice device)
	{
		GameSettings result = null;
		StorageContainer val = null;
		FileStream fileStream = null;
		try
		{
			val = device.OpenContainer(Game.STORAGE_LOCATION);
			string path = Path.Combine(val.Path, Game.STORAGE_SETTINGS_FILE);
			if (File.Exists(path))
			{
				fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameSettings));
				result = (GameSettings)xmlSerializer.Deserialize(fileStream);
				fileStream.Close();
				fileStream = null;
			}
			val.Dispose();
			val = null;
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
			if (val != null)
			{
				val.Dispose();
				val = null;
			}
			return null;
		}
	}
}
