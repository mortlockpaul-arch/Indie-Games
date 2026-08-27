using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace EGEngine;

public class Storage : StorageHelper
{
	private static string ApocZVersion = "APOCZ_32E4CQAB86C4RLJX";

	public static string MyLeaderboardName = "EODTopKillers";

	public static int MyLeaderboardVersion = 1;

	public static List<MyLeaderboardEntry> MyLeaderboard = new List<MyLeaderboardEntry>();

	public static bool haveValidStorage = false;

	public static bool DoneStorageDeviceSelect = false;

	private static string containerName = "";

	private static IAsyncResult iarDevice;

	private static StorageDevice userStorageDevice = null;

	private static StorageContainer userStorageContainer = null;

	private static PlayerIndex StorageDeviceOwner;

	public static string PlayerCharacterFilename = "";

	public static string PlayerStatisFilename = "";

	public static string PlayerInventoryFilename = "";

	public static string PlayerTentsFilename = "";

	public static bool SavePlayerThreadRunning = false;

	private static List<string> clanTags = new List<string>();

	private static bool SavePlayerClanTagsThreadRunning = false;

	public static void SetStorageDevice(PlayerIndex pIndex, string cn)
	{
		StorageDeviceOwner = pIndex;
		containerName = cn;
		haveValidStorage = false;
		try
		{
			if (!Guide.IsVisible)
			{
				string.Concat((object)"Get Storage Device ", (object)StorageDeviceOwner.ToString());
				iarDevice = StorageDevice.BeginShowSelector(StorageDeviceSelect, null);
			}
		}
		catch
		{
			DoneStorageDeviceSelect = true;
		}
	}

	private static void StorageDeviceSelect(IAsyncResult result)
	{
		try
		{
			if (!result.IsCompleted)
			{
				return;
			}
			userStorageDevice = StorageDevice.EndShowSelector(result);
			if (userStorageDevice == null)
			{
				haveValidStorage = false;
			}
			else
			{
				IAsyncResult asyncResult = userStorageDevice.BeginOpenContainer(containerName, null, null);
				asyncResult.AsyncWaitHandle.WaitOne();
				userStorageContainer = userStorageDevice.EndOpenContainer(asyncResult);
				asyncResult.AsyncWaitHandle.Close();
				if (userStorageContainer != null)
				{
					haveValidStorage = true;
				}
				else
				{
					haveValidStorage = false;
				}
			}
			DoneStorageDeviceSelect = true;
		}
		catch (Exception ex)
		{
			_ = ex.Message;
			haveValidStorage = false;
			DoneStorageDeviceSelect = true;
		}
	}

	private static void WriteVersion(Stream e)
	{
		char[] array = ApocZVersion.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			e.WriteByte((byte)array[i]);
		}
	}

	private static bool ReadVersion(Stream e)
	{
		char[] array = new char[ApocZVersion.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (char)e.ReadByte();
		}
		string text = new string(array);
		return ApocZVersion == text;
	}

	public static void SavePlayerStatus()
	{
		PlayerBase playerRef = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		PlayerStatus.Write(DataEncoder.DataBuffer, playerRef);
		LevelBaseMenu.SavePlayerDataScheduled = true;
	}

	public static void SaveData(int qIndex)
	{
		Stream stream = null;
		try
		{
			stream = ((!userStorageContainer.FileExists(PlayerStatisFilename)) ? userStorageContainer.CreateFile(PlayerStatisFilename) : userStorageContainer.OpenFile(PlayerStatisFilename, FileMode.Truncate));
			DataEncoder.SaveData(stream);
			stream.Close();
		}
		catch (Exception ex)
		{
			stream?.Close();
			MessagePump.AddMessage(ex.Message + " SavePlayerDataFile()");
		}
		SavePlayerThreadRunning = false;
	}

	public static bool LoadCharacterStats()
	{
		PlayerBase playerRef = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		return PlayerStatus.Read(DataEncoder.DataBuffer, playerRef);
	}

	public static void LoadData()
	{
		Stream stream = null;
		if (!haveValidStorage || Guide.IsTrialMode)
		{
			return;
		}
		PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		if (userStorageContainer.FileExists(PlayerStatisFilename))
		{
			stream = userStorageContainer.OpenFile(PlayerStatisFilename, FileMode.Open);
			if (stream.Length < 1 || stream.Length > 4096)
			{
				playerBase.BloodLevel = -1f;
				return;
			}
			DataEncoder.LoadData(stream);
			stream.Close();
			return;
		}
		Stream stream2 = null;
		try
		{
			if (!userStorageContainer.FileExists(PlayerTentsFilename))
			{
				return;
			}
			stream2 = userStorageContainer.OpenFile(PlayerTentsFilename, FileMode.Open);
			if (ReadVersion(stream2))
			{
				List<ItemCls> list = new List<ItemCls>();
				List<ItemCls> list2 = new List<ItemCls>();
				for (int num = stream2.ReadByte(); num > 0; num--)
				{
					ItemCls itemCls = new ItemCls();
					itemCls.StreamRead(stream2);
					list.Add(itemCls);
				}
				for (int num = stream2.ReadByte(); num > 0; num--)
				{
					ItemCls itemCls2 = new ItemCls();
					itemCls2.StreamRead(stream2);
					list2.Add(itemCls2);
				}
				stream2.Close();
				SavePlayerWorldItems(list, list2);
			}
			else
			{
				stream2.Close();
			}
		}
		catch (Exception ex)
		{
			stream2?.Close();
			MessagePump.AddMessage("...LoadOldTentData(): " + ex.Message);
		}
	}

	public static bool NewSaveInventory()
	{
		bool result = false;
		Stream stream = null;
		try
		{
			if (!haveValidStorage || Guide.IsTrialMode)
			{
				return result;
			}
			if (EGENetWorkNext.HostMigrateTimer > 0f || ApocZSaveDataCls.SyncingToServer)
			{
				return result;
			}
			stream = ((!userStorageContainer.FileExists(PlayerInventoryFilename)) ? userStorageContainer.CreateFile(PlayerInventoryFilename) : userStorageContainer.OpenFile(PlayerInventoryFilename, FileMode.Truncate));
			WriteVersion(stream);
			AIBase.PlayerInventory.SaveInventory(stream);
			stream.Close();
			result = true;
			LevelBaseMenu.SavePlayerDataScheduled = true;
		}
		catch (Exception ex)
		{
			stream?.Close();
			MessagePump.AddMessage("SaveInventory(): " + ex.Message);
		}
		return result;
	}

	public static bool NewLoadInventory()
	{
		bool result = false;
		Stream stream = null;
		try
		{
			if (!haveValidStorage || Guide.IsTrialMode)
			{
				return result;
			}
			if (userStorageContainer.FileExists(PlayerInventoryFilename))
			{
				stream = userStorageContainer.OpenFile(PlayerInventoryFilename, FileMode.Open);
				if (!ReadVersion(stream))
				{
					stream.Close();
					return result;
				}
				AIBase.PlayerInventory.ReadInventory(stream);
				stream.Close();
				result = true;
			}
		}
		catch (Exception ex)
		{
			stream?.Close();
			MessagePump.AddMessage("LoadInventory(): " + ex.Message);
		}
		return result;
	}

	public static bool SavePlayerWorldItems(List<ItemCls> tents, List<ItemCls> contents)
	{
		PalyerWorldItems.Write(DataEncoder.DataBuffer, tents, contents);
		LevelBaseMenu.SavePlayerDataScheduled = true;
		return true;
	}

	public static bool LoadPlayerWorldItems(List<ItemCls> tents, List<ItemCls> contents)
	{
		PalyerWorldItems.Read(DataEncoder.DataBuffer, tents, contents);
		return true;
	}

	public static void SavePlayerInfo()
	{
		PlayerBase playerRef = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		PlayerCharacter.Write(DataEncoder.DataBuffer, playerRef);
	}

	public static void LoadPlayerInfo()
	{
		PlayerBase playerRef = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		PlayerCharacter.Read(DataEncoder.DataBuffer, playerRef);
	}

	public static void DeleteFile(string fileName)
	{
		try
		{
			if (userStorageContainer.FileExists(fileName))
			{
				userStorageContainer.DeleteFile(fileName);
			}
		}
		catch (Exception ex)
		{
			MessagePump.AddMessage(ex.Message + "DeleteFile()");
		}
	}

	public static void SavePlayerClanTags(List<string> e)
	{
		try
		{
			if (haveValidStorage && !Guide.IsTrialMode)
			{
				int num = 1000000;
				while (SavePlayerClanTagsThreadRunning && --num > 0)
				{
				}
				clanTags = e;
				SavePlayerClanTagsThreadRunning = true;
				Thread thread = new Thread(ThreadSavePlayerClanTags);
				thread.Start();
			}
		}
		catch (Exception ex)
		{
			MessagePump.AddMessage("SavePlayerClanTags(): " + ex.Message);
		}
	}

	private static void ThreadSavePlayerClanTags()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 5 });
		Stream stream = null;
		try
		{
			PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
			if (userStorageContainer.FileExists(playerBase.gamerTag + "ClanTags"))
			{
				userStorageContainer.DeleteFile(playerBase.gamerTag + "ClanTags");
			}
			stream = userStorageContainer.CreateFile(playerBase.gamerTag + "ClanTags");
			stream.WriteByte((byte)clanTags.Count);
			for (int i = 0; i < clanTags.Count; i++)
			{
				stream.WriteByte((byte)clanTags[i].Length);
				for (int j = 0; j < clanTags[i].Length; j++)
				{
					stream.WriteByte((byte)clanTags[i][j]);
				}
			}
			stream.Close();
		}
		catch (Exception ex)
		{
			stream?.Close();
			MessagePump.AddMessage("SavePlayerClanTags(): " + ex.Message);
		}
		SavePlayerClanTagsThreadRunning = false;
	}

	public static List<string> LoadPlayerClanTags()
	{
		Stream stream = null;
		List<string> list = new List<string>();
		try
		{
			if (!haveValidStorage || Guide.IsTrialMode)
			{
				return null;
			}
			int num = 1000000;
			while (SavePlayerClanTagsThreadRunning && --num > 0)
			{
			}
			PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
			if (userStorageContainer.FileExists(playerBase.gamerTag + "ClanTags"))
			{
				stream = userStorageContainer.OpenFile(playerBase.gamerTag + "ClanTags", FileMode.Open);
				int num2 = stream.ReadByte();
				for (int i = 0; i < num2; i++)
				{
					int num3 = stream.ReadByte();
					string text = "";
					for (int j = 0; j < num3; j++)
					{
						text += stream.ReadByte();
					}
					list.Add(text);
				}
			}
			stream.Close();
		}
		catch (Exception ex)
		{
			stream?.Close();
			MessagePump.AddMessage("LoadPlayerClanTags(): " + ex.Message);
		}
		return list;
	}
}
