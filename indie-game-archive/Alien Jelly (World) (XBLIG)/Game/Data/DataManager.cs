using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GKEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace Game.Data;

public static class DataManager
{
	public enum LevelType
	{
		Play,
		Build,
		Share
	}

	public delegate void DataCallback();

	public delegate void DataAsDataCallback(byte[] pData);

	public const string TITLE_NAME = "Alien Jelly";

	public const string PATH_FILE_GLOBAL = "GameData.xml";

	public const string PATH_FILE_LOCAL = "PlayerData.xml";

	public const string PATH_COMPILE = "Content/Data/";

	public const string PATH_PROJECT = "../../../../../";

	public const string PATH_GROUPS_PREFIX = "Group_";

	public const string PATH_LEVELS_PREFIX = "Level_";

	public const string PATH_LEVELS_SUFFIX = ".xml";

	public const string DEFAULT_LEVEL_SKY = "Alpha Prime";

	public static string[] PATH_LEVELS = new string[3] { "Play", "Build", "Share" };

	private static bool waitingForGuide = false;

	private static bool waitingForSaveOOP = false;

	private static bool waitingForGuideToClose = false;

	public static DataPlayer local;

	public static DataGame global;

	public static DataLevel level;

	public static uint levelIndex = 0u;

	public static uint levelType = 0u;

	public static int levelGroupIndex = -1;

	private static uint _levelIndex = 0u;

	private static uint _levelType = 0u;

	private static int _levelGroup = 0;

	private static byte[] _levelData;

	public static StorageDevice storageDevice;

	public static StorageContainer storageContainer;

	public static DataCallback __error;

	public static DataCallback __storage;

	public static DataCallback __waitingForGuildToClose;

	public static DataCallback __loaded;

	public static DataCallback __saved;

	public static DataCallback __deleted;

	public static DataAsDataCallback __loadedDataAsData;

	public static DataCallback __savingMessageShow;

	public static DataCallback __savingMessageHide;

	public static DataCallback __oop;

	public static DataCallback __oopExecute;

	public static int oopTick = 0;

	public static DataLevelHeader header => Levels_FromIndex(levelIndex, levelType, levelGroupIndex);

	public static void Update(GameTime oGameTime)
	{
		if (waitingForGuideToClose && !Guide.IsVisible)
		{
			waitingForGuideToClose = false;
			if (__waitingForGuildToClose != null)
			{
				__waitingForGuildToClose();
			}
		}
		if (waitingForGuide && !Guide.IsVisible)
		{
			waitingForGuide = false;
			Device_Get(__storage);
		}
		if (waitingForSaveOOP)
		{
			waitingForSaveOOP = false;
			if (__savingMessageHide != null)
			{
				__savingMessageHide();
			}
			if (__saved != null)
			{
				__saved();
			}
		}
		if (__oop != null)
		{
			oopTick++;
			if (oopTick > 2)
			{
				__oopExecute = __oop;
				__oop = null;
				oopTick = 0;
				__oopExecute();
			}
		}
	}

	public static void Gamer_CheckSignedIn()
	{
	}

	public static void Device_Get(DataCallback oCallback)
	{
		GameMain.instance.gamerServices.Update(new GameTime());
		if (Guide.IsVisible)
		{
			__waitingForGuildToClose = delegate
			{
				Device_Get(oCallback);
			};
			waitingForGuideToClose = true;
		}
		else if (!waitingForGuide)
		{
			if (oCallback != null)
			{
				__storage = oCallback;
			}
			if (storageDevice != null && storageDevice.IsConnected)
			{
				IAsyncResult asyncResult = storageDevice.BeginOpenContainer("Alien Jelly", null, null);
				asyncResult.AsyncWaitHandle.WaitOne();
				storageContainer = storageDevice.EndOpenContainer(asyncResult);
				asyncResult.AsyncWaitHandle.Close();
				__storage();
				storageContainer.Dispose();
			}
			else if (!Guide.IsVisible)
			{
				storageDevice = null;
				StorageDevice.BeginShowSelector(Device_Get_Callback, "Please select a storage device");
			}
			else
			{
				waitingForGuide = true;
			}
		}
	}

	public static void Device_Get_Callback(IAsyncResult result)
	{
		storageDevice = StorageDevice.EndShowSelector(result);
		Device_Get(__storage);
	}

	public static void Load(DataCallback oLoaded, DataCallback oError)
	{
		try
		{
			Stream stream = File.OpenRead("Content/Data/GameData.xml");
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(DataGame));
			global = (DataGame)xmlSerializer.Deserialize(stream);
			stream.Close();
			oLoaded?.Invoke();
		}
		catch
		{
			oError?.Invoke();
		}
	}

	public static void PlayerData_Load(DataCallback oLoaded, DataCallback oError)
	{
		GameMain.instance.gamerServices.Update(new GameTime());
		if (Guide.IsVisible)
		{
			__waitingForGuildToClose = delegate
			{
				PlayerData_Load(oLoaded, oError);
			};
			waitingForGuideToClose = true;
			return;
		}
		__loaded = oLoaded;
		__error = oError;
		try
		{
			PlayerData_Load_Do();
			__loaded();
		}
		catch
		{
			if (__error != null)
			{
				__error();
			}
		}
	}

	private static void PlayerData_Load_Do()
	{
		Device_Get(delegate
		{
			Stream stream;
			XmlSerializer xmlSerializer;
			if (!storageContainer.FileExists("PlayerData.xml"))
			{
				stream = storageContainer.CreateFile("PlayerData.xml");
				DataPlayer o = PlayerData_SetDefault();
				xmlSerializer = new XmlSerializer(typeof(DataPlayer));
				xmlSerializer.Serialize(stream, o);
				stream.Close();
			}
			stream = storageContainer.OpenFile("PlayerData.xml", FileMode.Open);
			xmlSerializer = new XmlSerializer(typeof(DataPlayer));
			local = (DataPlayer)xmlSerializer.Deserialize(stream);
			stream.Close();
		});
	}

	private static DataPlayer PlayerData_SetDefault()
	{
		DataPlayer dataPlayer = new DataPlayer(new DataSettings(), new List<DataLevelHeader>(), new List<DataProgression>());
		string xAuthor = "Player";
		if (Gamer.SignedInGamers.Count > 0)
		{
			for (int i = 0; i < Gamer.SignedInGamers.Count; i++)
			{
				if (Gamer.SignedInGamers[i].PlayerIndex == (PlayerIndex)UniversalInput.gamePadPrimaryIndex)
				{
					xAuthor = Gamer.SignedInGamers[i].Gamertag;
				}
			}
		}
		dataPlayer.settings.level = 0;
		dataPlayer.settings.seenHelp = false;
		dataPlayer.settings.moveInvertX = false;
		dataPlayer.settings.moveInvertY = false;
		dataPlayer.settings.cameraInvertX = false;
		dataPlayer.settings.cameraInvertY = false;
		dataPlayer.settings.cameraSnapping = true;
		dataPlayer.settings.volumeMusic = 5;
		dataPlayer.settings.volumeFX = 5;
		dataPlayer.settings.gamma = 4;
		dataPlayer.settings.screen = new Rectangle(0, 0, -1, -1);
		dataPlayer.settings.resolution = default(Point);
		for (int i = 0; i < 10; i++)
		{
			DataLevelHeader item = new DataLevelHeader("My Level " + (i + 1), xAuthor, (uint)i, 1u, xEdit: true, xPassed: false, -1);
			dataPlayer.levels.Add(item);
		}
		return dataPlayer;
	}

	public static void PlayerData_Save(DataCallback oSaved, DataCallback oSavingMessageShow, DataCallback oSavingMessageHide)
	{
		waitingForSaveOOP = false;
		__saved = oSaved;
		__savingMessageShow = oSavingMessageShow;
		__savingMessageHide = oSavingMessageHide;
		if (__savingMessageShow != null)
		{
			__savingMessageShow();
		}
		__oop = delegate
		{
			Device_Get(delegate
			{
				if (storageContainer.FileExists("PlayerData.xml"))
				{
					storageContainer.DeleteFile("PlayerData.xml");
				}
				Stream stream = storageContainer.CreateFile("PlayerData.xml");
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(DataPlayer));
				xmlSerializer.Serialize(stream, local);
				stream.Close();
				waitingForSaveOOP = true;
			});
		};
	}

	public static void Levels_Load(uint xIndex, uint xType, int xGroup, DataCallback oLoaded, DataCallback oError)
	{
		_levelIndex = xIndex;
		_levelType = xType;
		_levelGroup = xGroup;
		__loaded = oLoaded;
		__error = oError;
		Device_Get(delegate
		{
			string text = "";
			text = ((xGroup < 0) ? (PATH_LEVELS[xType] + "/Level_" + xIndex + ".xml") : ("Content/Data/" + PATH_LEVELS[xType] + "/Group_" + xGroup + "/Level_" + xIndex + ".xml"));
			Stream stream;
			XmlSerializer xmlSerializer;
			if (_levelType != 0)
			{
				if (!storageContainer.DirectoryExists(PATH_LEVELS[xType]))
				{
					storageContainer.CreateDirectory(PATH_LEVELS[xType]);
				}
				if (!storageContainer.FileExists(text))
				{
					stream = storageContainer.CreateFile(text);
					DataLevel o = new DataLevel(0, "Alpha Prime", 0, 0, new List<DataKeyFrame>(), new List<DataAtom>(), new List<DataConversation>());
					xmlSerializer = new XmlSerializer(typeof(DataLevel));
					xmlSerializer.Serialize(stream, o);
					stream.Close();
				}
			}
			stream = ((_levelType == 0) ? File.OpenRead(text) : storageContainer.OpenFile(text, FileMode.Open));
			xmlSerializer = new XmlSerializer(typeof(DataLevel));
			level = (DataLevel)xmlSerializer.Deserialize(stream);
			stream.Close();
			levelIndex = _levelIndex;
			levelType = _levelType;
			levelGroupIndex = _levelGroup;
			if (__loaded != null)
			{
				__loaded();
			}
		});
	}

	public static void Levels_Save(DataCallback oSaved, DataCallback oSavingMessageShow, DataCallback oSavingMessageHide)
	{
		waitingForSaveOOP = false;
		__saved = oSaved;
		__savingMessageShow = oSavingMessageShow;
		__savingMessageHide = oSavingMessageHide;
		if (__savingMessageShow != null)
		{
			__savingMessageShow();
		}
		__oop = delegate
		{
			Device_Get(delegate
			{
				string text = "";
				text = ((levelGroupIndex < 0) ? (PATH_LEVELS[levelType] + "/Level_" + levelIndex + ".xml") : ("Content/Data/" + PATH_LEVELS[levelType] + "/Group_" + levelGroupIndex + "/Level_" + levelIndex + ".xml"));
				if (!storageContainer.DirectoryExists(PATH_LEVELS[levelType]))
				{
					storageContainer.CreateDirectory(PATH_LEVELS[levelType]);
				}
				if (storageContainer.FileExists(text))
				{
					storageContainer.DeleteFile(text);
				}
				Stream stream = storageContainer.CreateFile(text);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(DataLevel));
				xmlSerializer.Serialize(stream, level);
				stream.Close();
				waitingForSaveOOP = true;
			});
		};
	}

	public static void Levels_Delete(uint xIndex, uint xType, DataCallback oCompleted, DataCallback oError)
	{
		__deleted = oCompleted;
		__error = oError;
		try
		{
			Device_Get(delegate
			{
				string file = PATH_LEVELS[xType] + "/Level_" + xIndex + ".xml";
				if (storageContainer.FileExists(file))
				{
					storageContainer.DeleteFile(file);
				}
				if (__deleted != null)
				{
					__deleted();
				}
			});
		}
		catch
		{
			if (__error != null)
			{
				__error();
			}
		}
	}

	public static DataLevelHeader Levels_FromIndex(uint xIndex, uint xType, int xGroupIndex)
	{
		DataLevelHeader result = null;
		List<DataLevelHeader> list = ((xType == 0) ? global.levels : local.levels);
		for (int i = 0; i < list.Count; i++)
		{
			if ((xGroupIndex >= 0 && list[i].group == xGroupIndex && list[i].index == xIndex && list[i].type == xType) || (xGroupIndex < 0 && list[i].index == xIndex && list[i].type == xType))
			{
				result = list[i];
				break;
			}
		}
		return result;
	}

	public static int Levels_Count(uint xType)
	{
		int num = 0;
		List<DataLevelHeader> list = ((xType == 0) ? global.levels : local.levels);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].type == xType)
			{
				num++;
			}
		}
		return num;
	}

	public static DataLevelHeader Levels_GetNextPlay()
	{
		bool flag = false;
		DataLevelHeader result = null;
		for (int i = 0; i < global.levels.Count; i++)
		{
			if (!flag)
			{
				if (global.levels[i].index == levelIndex && global.levels[i].type == levelType && global.levels[i].group == levelGroupIndex)
				{
					flag = true;
				}
				continue;
			}
			result = global.levels[i];
			break;
		}
		return result;
	}

	public static void Levels_LoadAsData(uint xIndex, uint xType, DataAsDataCallback oLoaded, DataCallback oError)
	{
		_levelIndex = xIndex;
		_levelType = xType;
		_levelGroup = -1;
		__loadedDataAsData = oLoaded;
		__error = oError;
		try
		{
			Device_Get(delegate
			{
				string text = PATH_LEVELS[xType] + "/Level_" + xIndex + ".xml";
				Stream stream = ((_levelType == 0) ? File.OpenRead(text) : storageContainer.OpenFile(text, FileMode.Open));
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, (int)stream.Length);
				stream.Close();
				if (__loadedDataAsData != null)
				{
					__loadedDataAsData(array);
				}
			});
		}
		catch
		{
			if (__error != null)
			{
				__error();
			}
		}
	}

	public static void Levels_SaveAsData(byte[] pData, uint pIndex, uint pType, DataCallback pSaved)
	{
		waitingForSaveOOP = false;
		_levelIndex = pIndex;
		_levelType = pType;
		_levelGroup = -1;
		_levelData = pData;
		__saved = pSaved;
		Device_Get(delegate
		{
			string file = PATH_LEVELS[_levelType] + "/Level_" + _levelIndex + ".xml";
			if (!storageContainer.DirectoryExists(PATH_LEVELS[_levelType]))
			{
				storageContainer.CreateDirectory(PATH_LEVELS[_levelType]);
			}
			if (storageContainer.FileExists(file))
			{
				storageContainer.DeleteFile(file);
			}
			Stream stream = storageContainer.CreateFile(file);
			stream.Write(_levelData, 0, _levelData.Length);
			stream.Close();
			waitingForSaveOOP = true;
		});
	}

	public static DataProgression Progression_Get(int xIndex, int xGroup)
	{
		DataProgression result = null;
		for (int i = 0; i < local.progression.Count; i++)
		{
			if (local.progression[i].index == xIndex && local.progression[i].group == xGroup)
			{
				result = local.progression[i];
				break;
			}
		}
		return result;
	}

	public static DataProgression Progression_GetMax()
	{
		DataProgression dataProgression = null;
		for (int i = 0; i < local.progression.Count; i++)
		{
			if (dataProgression == null || local.progression[i].group > dataProgression.group || (local.progression[i].group == dataProgression.group && local.progression[i].index > dataProgression.index))
			{
				dataProgression = local.progression[i];
			}
		}
		return dataProgression;
	}

	public static void Progression_GetNextPlayable(out int xIndex, out int xGroup)
	{
		DataProgression dataProgression = Progression_GetMax();
		xIndex = 0;
		xGroup = 0;
		if (dataProgression == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < global.levels.Count; i++)
		{
			if (global.levels[i].index == dataProgression.index + 1 && global.levels[i].group == dataProgression.group)
			{
				flag = true;
				xIndex = dataProgression.index + 1;
				xGroup = dataProgression.group;
				break;
			}
		}
		if (!flag)
		{
			for (int i = 0; i < global.levels.Count; i++)
			{
				if (global.levels[i].index == 0 && global.levels[i].group == dataProgression.group + 1)
				{
					flag = true;
					xIndex = 0;
					xGroup = dataProgression.group + 1;
					break;
				}
			}
		}
		if (!flag)
		{
			xIndex = dataProgression.index;
			xGroup = dataProgression.group;
		}
	}
}
