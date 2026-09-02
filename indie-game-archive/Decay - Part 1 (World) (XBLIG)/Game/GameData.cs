using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Storage;

namespace Game;

[Serializable]
public class GameData
{
	public List<GameState> m_states = new List<GameState>();

	public List<string> m_items = new List<string>();

	public string m_area = "";

	public string m_view = "";

	public void Clear()
	{
		if (m_states != null)
		{
			m_states.Clear();
			m_states = null;
		}
	}

	public static bool Save(GameData data, StorageDevice device)
	{
		StorageContainer val = null;
		FileStream fileStream = null;
		try
		{
			val = device.OpenContainer(Game.STORAGE_LOCATION);
			string path = Path.Combine(val.Path, Game.STORAGE_SAVE_FILE);
			fileStream = File.Open(path, FileMode.Create, FileAccess.ReadWrite);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameData));
			xmlSerializer.Serialize(fileStream, data);
			fileStream.Close();
			fileStream = null;
			val.Dispose();
			val = null;
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine("GameData.Save: " + ex.Message);
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

	public static GameData Load(StorageDevice device)
	{
		GameData result = null;
		StorageContainer val = null;
		FileStream fileStream = null;
		try
		{
			val = device.OpenContainer(Game.STORAGE_LOCATION);
			string path = Path.Combine(val.Path, Game.STORAGE_SAVE_FILE);
			if (File.Exists(path))
			{
				fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameData));
				result = (GameData)xmlSerializer.Deserialize(fileStream);
				fileStream.Close();
				fileStream = null;
			}
			val.Dispose();
			val = null;
		}
		catch (Exception ex)
		{
			Console.WriteLine("GameData.Load: " + ex.Message);
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
		return result;
	}

	public void SetState(string id, string state)
	{
		for (int i = 0; i < m_states.Count; i++)
		{
			if (m_states[i].m_id == id)
			{
				GameState value = m_states[i];
				value.m_state = state;
				m_states[i] = value;
				return;
			}
		}
		m_states.Add(new GameState(id, state));
	}

	public string GetState(string id)
	{
		for (int i = 0; i < m_states.Count; i++)
		{
			if (m_states[i].m_id == id)
			{
				return m_states[i].m_state;
			}
		}
		return "";
	}
}
