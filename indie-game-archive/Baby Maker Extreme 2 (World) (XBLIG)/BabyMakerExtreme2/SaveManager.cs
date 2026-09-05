using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using Screens;

namespace BabyMakerExtreme2;

public static class SaveManager
{
	private const int CUR_SAVE_VER = 5;

	private static bool m_bSaveOptionsRequested;

	private static bool m_bLoadOptionsRequested;

	private static bool m_bDisplayedFailure;

	private static bool m_bCanSave;

	private static bool m_bIsShowingDeviceSelector;

	private static IAsyncResult getDeviceResult;

	private static StorageDevice m_StorageDevice;

	private static optionSet m_savedOptions;

	public static void Init()
	{
		m_bSaveOptionsRequested = false;
		m_bLoadOptionsRequested = false;
		m_bDisplayedFailure = false;
		m_bCanSave = true;
		m_bIsShowingDeviceSelector = false;
		getDeviceResult = null;
		m_StorageDevice = null;
		m_savedOptions = default(optionSet);
	}

	public static void SaveGlobalOptions()
	{
		if (m_bCanSave && !Guide.IsVisible && !m_bSaveOptionsRequested)
		{
			m_bSaveOptionsRequested = true;
			GetStorageDevice(out var _);
		}
	}

	public static void LoadGlobalOptions()
	{
		if (m_bCanSave && !Guide.IsVisible && !m_bLoadOptionsRequested)
		{
			m_bLoadOptionsRequested = true;
			GetStorageDevice(out var _);
		}
	}

	public static void HandleSaveLoadOptions()
	{
		if (m_bSaveOptionsRequested)
		{
			try
			{
				StorageDevice storageDevice = GetStorageDevice(out var isRetrieving);
				if (!isRetrieving && storageDevice != null && storageDevice.IsConnected)
				{
					optionSet savedOptions = m_savedOptions;
					savedOptions.SaveVersion = 5;
					IAsyncResult asyncResult = m_StorageDevice.BeginOpenContainer("BMX2Storage", null, null);
					asyncResult.AsyncWaitHandle.WaitOne();
					StorageContainer storageContainer = m_StorageDevice.EndOpenContainer(asyncResult);
					asyncResult.AsyncWaitHandle.Close();
					string file = "SaveData.sav";
					Stream stream = storageContainer.OpenFile(file, FileMode.Create);
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(optionSet));
					xmlSerializer.Serialize(stream, savedOptions);
					stream.Close();
					storageContainer.Dispose();
				}
				if (!isRetrieving)
				{
					m_bSaveOptionsRequested = false;
					UpdateCanSave();
					if (!CanSave())
					{
						DisplaySaveFailure();
					}
				}
				return;
			}
			catch (Exception)
			{
				m_bSaveOptionsRequested = false;
				UpdateCanSave();
				if (!CanSave())
				{
					DisplaySaveFailure();
				}
				return;
			}
		}
		if (!m_bLoadOptionsRequested)
		{
			return;
		}
		StorageDevice storageDevice2 = GetStorageDevice(out var isRetrieving2);
		if (!isRetrieving2 && storageDevice2 != null && storageDevice2.IsConnected)
		{
			try
			{
				IAsyncResult asyncResult2 = storageDevice2.BeginOpenContainer("BMX2Storage", null, null);
				asyncResult2.AsyncWaitHandle.WaitOne();
				StorageContainer storageContainer2 = storageDevice2.EndOpenContainer(asyncResult2);
				asyncResult2.AsyncWaitHandle.Close();
				string file2 = "SaveData.sav";
				m_bCanSave = true;
				if (storageContainer2.FileExists(file2))
				{
					Stream stream2 = storageContainer2.OpenFile(file2, FileMode.Open);
					optionSet optionSet2 = default(optionSet);
					XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(optionSet));
					optionSet2 = (optionSet)xmlSerializer2.Deserialize(stream2);
					if (optionSet2.SaveVersion == 5)
					{
						m_savedOptions = optionSet2;
					}
					else
					{
						SetOptionsDefault();
					}
					stream2.Close();
				}
				else
				{
					SetOptionsDefault();
				}
				storageContainer2.Dispose();
				MasterOfUnlocking.Init(m_savedOptions.PowerupUnlocks, m_savedOptions.OutfitUnlocks, m_savedOptions.ModeUnlocks);
			}
			catch (Exception)
			{
				SetOptionsDefault();
				MasterOfUnlocking.Init(m_savedOptions.PowerupUnlocks, m_savedOptions.OutfitUnlocks, m_savedOptions.ModeUnlocks);
			}
		}
		else if (!isRetrieving2)
		{
			SetOptionsDefault();
			MasterOfUnlocking.Init(m_savedOptions.PowerupUnlocks, m_savedOptions.OutfitUnlocks, m_savedOptions.ModeUnlocks);
			m_bCanSave = false;
		}
		if (!isRetrieving2)
		{
			m_bLoadOptionsRequested = false;
			m_bDisplayedFailure = false;
			if (!CanSave())
			{
				DisplaySaveFailure();
			}
		}
	}

	public static void ResetSaveState()
	{
		getDeviceResult = null;
		m_StorageDevice = null;
		m_bSaveOptionsRequested = false;
		m_bLoadOptionsRequested = false;
		m_bIsShowingDeviceSelector = false;
	}

	public static bool IsSaving()
	{
		return m_bSaveOptionsRequested;
	}

	public static StorageDevice GetStorageDevice(out bool isRetrieving)
	{
		bool flag = false;
		if (m_StorageDevice != null && !flag)
		{
			PlayerIndex playerIndex = ControlManager.GetPlayerIndex(ControlManager.ActiveMenuIndex);
			SignedInGamer signedInGamer = Gamer.SignedInGamers[playerIndex];
			if (signedInGamer != null)
			{
				isRetrieving = false;
				return m_StorageDevice;
			}
			m_StorageDevice = null;
		}
		else if (m_StorageDevice != null)
		{
			isRetrieving = false;
			return m_StorageDevice;
		}
		PlayerIndex playerIndex2 = ControlManager.GetPlayerIndex(ControlManager.ActiveMenuIndex);
		SignedInGamer signedInGamer2 = null;
		foreach (SignedInGamer signedInGamer3 in Gamer.SignedInGamers)
		{
			if (signedInGamer3.PlayerIndex == playerIndex2)
			{
				signedInGamer2 = signedInGamer3;
			}
		}
		if (!flag && signedInGamer2 == null)
		{
			isRetrieving = false;
			return null;
		}
		if (getDeviceResult == null && !Guide.IsVisible)
		{
			getDeviceResult = StorageDevice.BeginShowSelector(null, null);
			m_bIsShowingDeviceSelector = true;
		}
		if (getDeviceResult.IsCompleted)
		{
			if (m_bIsShowingDeviceSelector)
			{
				m_StorageDevice = StorageDevice.EndShowSelector(getDeviceResult);
				m_bIsShowingDeviceSelector = false;
			}
			isRetrieving = false;
			return m_StorageDevice;
		}
		isRetrieving = true;
		return null;
	}

	public static bool CanSave()
	{
		return m_bCanSave;
	}

	public static void UpdateCanSave()
	{
		if (m_bCanSave)
		{
			StorageDevice storageDevice = GetStorageDevice(out var isRetrieving);
			if (!isRetrieving && (storageDevice == null || !storageDevice.IsConnected))
			{
				m_bCanSave = false;
			}
		}
	}

	public static void DisplaySaveFailure()
	{
		if (!m_bDisplayedFailure)
		{
			m_bDisplayedFailure = true;
			new GenericErrScreen(ControlManager.ActiveMenuIndex, "Either your memory device got\ndisconnected at some point or\nyou are not using a profile\nthat can save.\nSaving of progress has \nbeen disabled.");
		}
	}

	public static bool DisplayedSaveFailure()
	{
		return m_bDisplayedFailure;
	}

	public static void SetOptionsDefault()
	{
		List<bool> list = new List<bool>();
		List<bool> list2 = new List<bool>();
		List<bool> list3 = new List<bool>();
		MasterOfUnlocking.LoadDefault(list, list2, list3);
		m_savedOptions.OutfitUnlocks = list2;
		m_savedOptions.ModeUnlocks = list3;
		m_savedOptions.PowerupUnlocks = list;
		m_savedOptions.SaveVersion = 5;
		m_savedOptions.HighScores = new List<List<int>>();
		m_savedOptions.HighScoresBabyTypes = new List<List<int>>();
		new List<int>();
		m_savedOptions.HighScoreNames = new List<List<string>>();
		new List<string>();
		for (int i = 0; i < 5; i++)
		{
			m_savedOptions.HighScores.Add(new List<int>());
			m_savedOptions.HighScoresBabyTypes.Add(new List<int>());
			m_savedOptions.HighScoreNames.Add(new List<string>());
			for (int num = 20; num > 0; num--)
			{
				m_savedOptions.HighScores[i].Add(num * 200);
				m_savedOptions.HighScoresBabyTypes[i].Add(0);
				m_savedOptions.HighScoreNames[i].Add("ScoreToBeat" + (21 - num));
			}
		}
	}

	public static int AddScore(string name, int score, int babyType, int roomType, bool repeats)
	{
		int index = 0;
		if (repeats)
		{
			index = roomType + 1;
		}
		for (int i = 0; i < m_savedOptions.HighScores[index].Count; i++)
		{
			if (score > m_savedOptions.HighScores[index][i])
			{
				m_savedOptions.HighScores[index].Insert(i, score);
				m_savedOptions.HighScoreNames[index].Insert(i, name);
				m_savedOptions.HighScoresBabyTypes[index].Insert(i, babyType);
				while (m_savedOptions.HighScores[index].Count > 20)
				{
					m_savedOptions.HighScores[index].RemoveAt(20);
					m_savedOptions.HighScoreNames[index].RemoveAt(20);
					m_savedOptions.HighScoresBabyTypes[index].RemoveAt(20);
				}
				return i;
			}
		}
		return -1;
	}

	public static void AddDist(int dist)
	{
		m_savedOptions.TotalDist += dist;
	}

	public static optionSet GetSavedData()
	{
		return m_savedOptions;
	}
}
