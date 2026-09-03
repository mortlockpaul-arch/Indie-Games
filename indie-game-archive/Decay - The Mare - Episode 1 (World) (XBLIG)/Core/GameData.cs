using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Storage;

namespace Core;

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
		StorageContainer storageContainer = null;
		FileStream fileStream = null;
		try
		{
			IAsyncResult asyncResult = device.BeginOpenContainer(Game.STORAGE_LOCATION, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			storageContainer = device.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			string sTORAGE_SAVE_FILE = Game.STORAGE_SAVE_FILE;
			fileStream = (FileStream)storageContainer.OpenFile(sTORAGE_SAVE_FILE, FileMode.Create, FileAccess.ReadWrite);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameData));
			xmlSerializer.Serialize(fileStream, data);
			fileStream.Close();
			fileStream = null;
			storageContainer.Dispose();
			storageContainer = null;
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
			if (storageContainer != null)
			{
				storageContainer.Dispose();
				storageContainer = null;
			}
		}
		return false;
	}

	public static GameData Load(StorageDevice device)
	{
		GameData result = null;
		StorageContainer storageContainer = null;
		FileStream fileStream = null;
		try
		{
			IAsyncResult asyncResult = device.BeginOpenContainer(Game.STORAGE_LOCATION, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			storageContainer = device.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			string sTORAGE_SAVE_FILE = Game.STORAGE_SAVE_FILE;
			if (storageContainer.FileExists(sTORAGE_SAVE_FILE))
			{
				fileStream = (FileStream)storageContainer.OpenFile(sTORAGE_SAVE_FILE, FileMode.Open, FileAccess.ReadWrite);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameData));
				result = (GameData)xmlSerializer.Deserialize(fileStream);
				fileStream.Close();
				fileStream = null;
				Console.WriteLine("GameData loaded from file: " + sTORAGE_SAVE_FILE);
			}
			storageContainer.Dispose();
			storageContainer = null;
		}
		catch (Exception ex)
		{
			Console.WriteLine("GameData.Load: " + ex.Message);
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
		return result;
	}

	public void SetState(string name, string value)
	{
		for (int i = 0; i < m_states.Count; i++)
		{
			if (m_states[i].m_name == name)
			{
				GameState value2 = m_states[i];
				value2.m_value = value;
				m_states[i] = value2;
				return;
			}
		}
		m_states.Add(new GameState(name, value));
	}

	public string GetState(string name)
	{
		for (int i = 0; i < m_states.Count; i++)
		{
			if (m_states[i].m_name == name)
			{
				return m_states[i].m_value;
			}
		}
		return "";
	}
}
