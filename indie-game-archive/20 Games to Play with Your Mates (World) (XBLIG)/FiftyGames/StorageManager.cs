using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace FiftyGames;

public class StorageManager : GameComponent
{
	public enum StorageDeviceState
	{
		NoDevice,
		NotSelected,
		Selecting,
		Disconnected,
		Full,
		Ready,
		Working
	}

	private enum StorageAction
	{
		LoadGameData,
		LoadProfile,
		LoadSettingsFromProfile,
		SaveGameData,
		SaveProfile,
		SaveFullProfile,
		DeleteProfile
	}

	private StorageDeviceState _deviceState;

	private Queue<StorageAction> _storageQueue;

	private Queue<Player> _profileQueue;

	private StorageDevice _storageDevice;

	private IAsyncResult _storageDeviceRequestResult;

	private StorageContainer _storageContainer;

	private IAsyncResult _storageContainerRequestResult;

	private Thread _storageThread;

	private int[] _storageThreadAffinity;

	private PlayerManager _playerManager;

	private SoundManager _soundManager;

	private MinigameMeta[] _minigameData;

	private MinigameMeta[] _sortedMinigameList;

	private byte _titleSafeOffsetLeft;

	private byte _titleSafeOffsetTop;

	private byte _titleSafeOffsetWidth;

	private byte _titleSafeOffsetHeight;

	public Stopwatch timer;

	public Rectangle SavedTitleSafe => new Rectangle(_titleSafeOffsetLeft, _titleSafeOffsetTop, 1024 + _titleSafeOffsetWidth, 576 + _titleSafeOffsetHeight);

	public StorageDeviceState DeviceState => _deviceState;

	public StorageManager(Game game, ref PlayerManager playerManager, ref SoundManager soundManager, ref MinigameMeta[] minigameMeta)
		: base(game)
	{
		_storageDevice = null;
		_storageDeviceRequestResult = null;
		_storageContainer = null;
		_storageContainerRequestResult = null;
		_deviceState = StorageDeviceState.NotSelected;
		_storageQueue = new Queue<StorageAction>();
		_profileQueue = new Queue<Player>();
		_playerManager = playerManager;
		_soundManager = soundManager;
		_minigameData = minigameMeta;
		_storageThreadAffinity = new int[1];
		_storageThreadAffinity[0] = 5;
		_titleSafeOffsetLeft = 128;
		_titleSafeOffsetTop = 72;
		_titleSafeOffsetWidth = 0;
		_titleSafeOffsetHeight = 0;
	}

	public override void Update(GameTime gameTime)
	{
		switch (_deviceState)
		{
		case StorageDeviceState.Selecting:
			CheckForDeviceSelection();
			break;
		case StorageDeviceState.Disconnected:
			if (_storageDevice.IsConnected)
			{
				_deviceState = StorageDeviceState.Ready;
			}
			break;
		case StorageDeviceState.Ready:
			if (!_storageDevice.IsConnected)
			{
				_storageQueue.Clear();
				_profileQueue.Clear();
				_deviceState = StorageDeviceState.Disconnected;
				GameConsole.PrintString("StorageManager: Storage device was disconnected. Clearing operations.");
			}
			else if (_storageDevice.FreeSpace < 4096)
			{
				_storageQueue.Clear();
				_profileQueue.Clear();
				_deviceState = StorageDeviceState.Full;
				GameConsole.PrintString("StorageManager: Storage device has no more available space. Clearing operations.");
			}
			if (_deviceState == StorageDeviceState.Ready && _storageQueue.Count != 0)
			{
				if (_storageContainerRequestResult == null)
				{
					_storageContainerRequestResult = _storageDevice.BeginOpenContainer("TwentyGamesSaveData", null, null);
				}
				else if (_storageContainerRequestResult.IsCompleted && _storageContainer == null)
				{
					_storageContainer = _storageDevice.EndOpenContainer(_storageContainerRequestResult);
				}
				else if (_storageContainer != null && _deviceState == StorageDeviceState.Ready)
				{
					_deviceState = StorageDeviceState.Working;
					_storageThread = new Thread(ProcessStorage);
					_storageThread.Start();
				}
			}
			break;
		case StorageDeviceState.NoDevice:
			if (_storageQueue.Count != 0)
			{
				_storageQueue.Clear();
			}
			if (_profileQueue.Count != 0)
			{
				_profileQueue.Clear();
			}
			break;
		}
		base.Update(gameTime);
	}

	public bool SelectStorageDevice(Player selectPlayer, ref MinigameMeta[] sortedMinigameList)
	{
		bool flag = false;
		_storageQueue.Clear();
		_profileQueue.Clear();
		_storageQueue.Enqueue(StorageAction.LoadGameData);
		_sortedMinigameList = sortedMinigameList;
		_storageQueue.Enqueue(StorageAction.LoadSettingsFromProfile);
		_profileQueue.Enqueue(selectPlayer);
		try
		{
			_storageDeviceRequestResult = StorageDevice.BeginShowSelector(null, null);
			_deviceState = StorageDeviceState.Selecting;
			flag = true;
			GameConsole.PrintString("StorageManager: Waiting for storage device to be selected.");
		}
		catch
		{
			_storageDeviceRequestResult = null;
			_deviceState = StorageDeviceState.NotSelected;
			_storageQueue.Clear();
			_profileQueue.Clear();
			flag = false;
			GameConsole.PrintString("StorageManager: An error occoured while showing storage device selection.");
		}
		return flag;
	}

	private void CheckForDeviceSelection()
	{
		if (_storageDeviceRequestResult == null || !_storageDeviceRequestResult.IsCompleted)
		{
			return;
		}
		_deviceState = StorageDeviceState.NoDevice;
		try
		{
			_storageDevice = StorageDevice.EndShowSelector(_storageDeviceRequestResult);
			if (_storageDevice != null)
			{
				_deviceState = StorageDeviceState.Ready;
			}
			GameConsole.PrintString("StorageManager: Storage device selected.");
		}
		catch
		{
			GameConsole.PrintString("StorageManager: Storage device could not be accessed.");
		}
	}

	private void LoadProfile(Player loadPlayer, bool settingsOnly)
	{
		float musicVolume = loadPlayer.MusicVolume;
		float effectVolume = loadPlayer.EffectVolume;
		byte sortMode = loadPlayer.SortMode;
		byte colorIndex = loadPlayer.ColorIndex;
		bool allowsVibration = loadPlayer.AllowsVibration;
		if (_storageContainer.FileExists(loadPlayer.Name))
		{
			Stream stream = _storageContainer.OpenFile(loadPlayer.Name, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(stream, Encoding.BigEndianUnicode);
			try
			{
				loadPlayer.MusicVolume = (float)Math.Round((double)(int)binaryReader.ReadByte() * 0.1, 1);
				loadPlayer.EffectVolume = (float)Math.Round((double)(int)binaryReader.ReadByte() * 0.1, 1);
				loadPlayer.SortMode = binaryReader.ReadByte();
				if (!settingsOnly)
				{
					loadPlayer.ColorIndex = binaryReader.ReadByte();
					if (!_playerManager.SelectColor(loadPlayer, loadPlayer.ColorIndex))
					{
						_playerManager.SelectNextColor(loadPlayer);
					}
					loadPlayer.AllowsVibration = binaryReader.ReadBoolean();
					GameConsole.PrintString("StorageManager: Loaded settings from profile " + loadPlayer.Name + ".");
				}
				else
				{
					GameConsole.PrintString("StorageManager: Loaded profile " + loadPlayer.Name + ".");
				}
			}
			catch
			{
				loadPlayer.MusicVolume = musicVolume;
				loadPlayer.EffectVolume = effectVolume;
				loadPlayer.SortMode = sortMode;
				loadPlayer.ColorIndex = colorIndex;
				loadPlayer.AllowsVibration = allowsVibration;
				GameConsole.PrintString("StorageManager: Failed to load profile " + loadPlayer.Name + ".");
			}
			finally
			{
				binaryReader.Close();
				binaryReader.Dispose();
				stream.Dispose();
			}
		}
		loadPlayer.WaitingForProfileLoad = false;
	}

	private void SaveProfile(Player savePlayer, bool saveSettings)
	{
		byte sortMode = savePlayer.SortMode;
		byte colorIndex = savePlayer.ColorIndex;
		bool allowsVibration = savePlayer.AllowsVibration;
		if (!saveSettings)
		{
			LoadProfile(savePlayer, settingsOnly: true);
		}
		Stream stream = _storageContainer.OpenFile(savePlayer.Name, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.BigEndianUnicode);
		try
		{
			binaryWriter.Write((byte)Math.Round(savePlayer.MusicVolume * 10f, 0));
			binaryWriter.Write((byte)Math.Round(savePlayer.EffectVolume * 10f, 0));
			binaryWriter.Write(sortMode);
			binaryWriter.Write(colorIndex);
			binaryWriter.Write(allowsVibration);
			GameConsole.PrintString("StorageManager: Saved profile " + savePlayer.Name + ".");
		}
		catch
		{
			GameConsole.PrintString("StorageManager: Failed to save profile " + savePlayer.Name + ". File write was interrupted.");
		}
		finally
		{
			binaryWriter.Close();
			binaryWriter.Dispose();
			stream.Dispose();
		}
	}

	private void LoadGameData()
	{
		if (!_storageContainer.FileExists("GameData"))
		{
			return;
		}
		byte[] array = new byte[_minigameData.Length];
		string[] array2 = new string[_minigameData.Length];
		float[] array3 = new float[_minigameData.Length];
		byte titleSafeOffsetLeft = _titleSafeOffsetLeft;
		byte titleSafeOffsetTop = _titleSafeOffsetTop;
		byte titleSafeOffsetWidth = _titleSafeOffsetWidth;
		byte titleSafeOffsetHeight = _titleSafeOffsetHeight;
		Stream stream = _storageContainer.OpenFile("GameData", FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(stream, Encoding.BigEndianUnicode);
		try
		{
			for (int i = 0; i != _minigameData.Length; i++)
			{
				_minigameData[i].Rating = binaryReader.ReadByte();
				_minigameData[i].BestWinner = binaryReader.ReadString();
				_minigameData[i].BestScore = binaryReader.ReadSingle();
			}
			_titleSafeOffsetLeft = binaryReader.ReadByte();
			_titleSafeOffsetTop = binaryReader.ReadByte();
			_titleSafeOffsetWidth = binaryReader.ReadByte();
			_titleSafeOffsetHeight = binaryReader.ReadByte();
			GameConsole.PrintString("StorageManager: Game data loaded.");
		}
		catch
		{
			for (int j = 0; j != _minigameData.Length; j++)
			{
				_minigameData[j].Rating = array[j];
				_minigameData[j].BestWinner = array2[j];
				_minigameData[j].BestScore = array3[j];
			}
			_titleSafeOffsetLeft = titleSafeOffsetLeft;
			_titleSafeOffsetTop = titleSafeOffsetTop;
			_titleSafeOffsetWidth = titleSafeOffsetWidth;
			_titleSafeOffsetHeight = titleSafeOffsetHeight;
			GameConsole.PrintString("StorageManager: Failed to load game data.");
		}
		finally
		{
			binaryReader.Close();
			binaryReader.Dispose();
			stream.Dispose();
		}
		for (int k = 0; k != _sortedMinigameList.Length; k++)
		{
			for (int l = 0; l != _minigameData.Length; l++)
			{
				if (_sortedMinigameList[k].MinigameID == _minigameData[l].MinigameID)
				{
					_sortedMinigameList[k].Rating = _minigameData[l].Rating;
					_sortedMinigameList[k].BestWinner = _minigameData[l].BestWinner;
					_sortedMinigameList[k].BestScore = _minigameData[l].BestScore;
				}
			}
		}
	}

	private void SaveGameData()
	{
		Stream stream = _storageContainer.OpenFile("GameData", FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.BigEndianUnicode);
		try
		{
			for (int i = 0; i != _minigameData.Length; i++)
			{
				binaryWriter.Write(_minigameData[i].Rating);
				binaryWriter.Write(_minigameData[i].BestWinner);
				binaryWriter.Write(_minigameData[i].BestScore);
			}
			binaryWriter.Write(_titleSafeOffsetLeft);
			binaryWriter.Write(_titleSafeOffsetTop);
			binaryWriter.Write(_titleSafeOffsetWidth);
			binaryWriter.Write(_titleSafeOffsetHeight);
			GameConsole.PrintString("StorageManager: Saved game data.");
		}
		catch
		{
			GameConsole.PrintString("StorageManager: Failed to save game data. File write was interrupted.");
		}
		finally
		{
			binaryWriter.Close();
			binaryWriter.Dispose();
			stream.Dispose();
		}
	}

	public void Load(ref MinigameMeta[] sortedMinigameList)
	{
		if (_deviceState == StorageDeviceState.Ready || _deviceState == StorageDeviceState.Working || _deviceState == StorageDeviceState.Selecting)
		{
			_storageQueue.Enqueue(StorageAction.LoadGameData);
			GameConsole.PrintString("StorageManager: Load game data request queued.");
		}
		else if (_deviceState == StorageDeviceState.NoDevice)
		{
			GameConsole.PrintString("StorageManager: No storage Device. Load Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Disconnected)
		{
			GameConsole.PrintString("StorageManager: Storage device missing. Load Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Full)
		{
			GameConsole.PrintString("StorageManager: Not enough space available on storage device. Load Cancelled");
		}
		else if (_deviceState == StorageDeviceState.NotSelected)
		{
			GameConsole.PrintString("StorageManager: No storage device has been selected. Load Cancelled");
		}
		_sortedMinigameList = sortedMinigameList;
	}

	public void Load(ref Player player, bool loadCurrentSettings)
	{
		if (_deviceState == StorageDeviceState.Ready || _deviceState == StorageDeviceState.Working || _deviceState == StorageDeviceState.Selecting)
		{
			if (loadCurrentSettings)
			{
				_storageQueue.Enqueue(StorageAction.LoadSettingsFromProfile);
				GameConsole.PrintString("StorageManager: Load profile (" + player.Name + ") settings request queued.");
			}
			else
			{
				_storageQueue.Enqueue(StorageAction.LoadProfile);
				GameConsole.PrintString("StorageManager: Load profile (" + player.Name + ") request queued.");
			}
			_profileQueue.Enqueue(player);
		}
		else if (_deviceState == StorageDeviceState.NoDevice)
		{
			GameConsole.PrintString("StorageManager: No storage Device. Load Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Disconnected)
		{
			GameConsole.PrintString("StorageManager: Storage device missing. Load Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Full)
		{
			GameConsole.PrintString("StorageManager: Not enough space available on storage device. Load Cancelled");
		}
		else if (_deviceState == StorageDeviceState.NotSelected)
		{
			GameConsole.PrintString("StorageManager: No storage device has been selected. Load Cancelled");
		}
	}

	public void Save(Rectangle titleSafeRect)
	{
		if (_deviceState == StorageDeviceState.Ready || _deviceState == StorageDeviceState.Working || _deviceState == StorageDeviceState.Selecting)
		{
			_titleSafeOffsetLeft = ((titleSafeRect.Left == 256) ? byte.MaxValue : ((byte)titleSafeRect.Left));
			_titleSafeOffsetTop = (byte)titleSafeRect.Top;
			_titleSafeOffsetWidth = ((titleSafeRect.Width == 1280) ? byte.MaxValue : ((byte)(titleSafeRect.Width - 1024)));
			_titleSafeOffsetHeight = (byte)(titleSafeRect.Height - 576);
			_storageQueue.Enqueue(StorageAction.SaveGameData);
			GameConsole.PrintString("StorageManager: Save game data (title safe) request queued.");
		}
		else if (_deviceState == StorageDeviceState.NoDevice)
		{
			GameConsole.PrintString("StorageManager: No storage Device. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Disconnected)
		{
			GameConsole.PrintString("StorageManager: Storage device missing. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Full)
		{
			GameConsole.PrintString("StorageManager: Not enough space available on storage device. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.NotSelected)
		{
			GameConsole.PrintString("StorageManager: No storage device has been selected. Save Cancelled");
		}
	}

	public void Save(MinigameMeta minigameData)
	{
		if (_deviceState == StorageDeviceState.Ready || _deviceState == StorageDeviceState.Working || _deviceState == StorageDeviceState.Selecting)
		{
			for (int i = 0; i != _minigameData.Length; i++)
			{
				if (_minigameData[i].MinigameID == minigameData.MinigameID)
				{
					_minigameData[i].Rating = minigameData.Rating;
					_minigameData[i].BestScore = minigameData.BestScore;
					_minigameData[i].BestWinner = minigameData.BestWinner;
				}
			}
			_storageQueue.Enqueue(StorageAction.SaveGameData);
			GameConsole.PrintString("StorageManager: Save game data (" + minigameData.Name + ") request queued.");
		}
		else if (_deviceState == StorageDeviceState.NoDevice)
		{
			GameConsole.PrintString("StorageManager: No storage Device. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Disconnected)
		{
			GameConsole.PrintString("StorageManager: Storage device missing. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Full)
		{
			GameConsole.PrintString("StorageManager: Not enough space available on storage device. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.NotSelected)
		{
			GameConsole.PrintString("StorageManager: No storage device has been selected. Save Cancelled");
		}
	}

	public void Save(MinigameMeta[] minigameData)
	{
		if (_deviceState == StorageDeviceState.Ready || _deviceState == StorageDeviceState.Working || _deviceState == StorageDeviceState.Selecting)
		{
			for (int i = 0; i < minigameData.Length; i++)
			{
				for (int j = 0; j != _minigameData.Length; j++)
				{
					if (_minigameData[j].MinigameID == minigameData[i].MinigameID)
					{
						_minigameData[j].Rating = minigameData[i].Rating;
						_minigameData[j].BestScore = minigameData[i].BestScore;
						_minigameData[j].BestWinner = minigameData[i].BestWinner;
					}
				}
			}
			_storageQueue.Enqueue(StorageAction.SaveGameData);
			GameConsole.PrintString("StorageManager: Save game data (minigame list) request queued.");
		}
		else if (_deviceState == StorageDeviceState.NoDevice)
		{
			GameConsole.PrintString("StorageManager: No storage Device. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Disconnected)
		{
			GameConsole.PrintString("StorageManager: Storage device missing. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Full)
		{
			GameConsole.PrintString("StorageManager: Not enough space available on storage device. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.NotSelected)
		{
			GameConsole.PrintString("StorageManager: No storage device has been selected. Save Cancelled");
		}
	}

	public void Save(Player player, bool saveCurrentSettings)
	{
		if (_deviceState == StorageDeviceState.Ready || _deviceState == StorageDeviceState.Working || _deviceState == StorageDeviceState.Selecting)
		{
			if (saveCurrentSettings)
			{
				_storageQueue.Enqueue(StorageAction.SaveFullProfile);
				GameConsole.PrintString("StorageManager: Save profile (" + player.Name + ") request queued.");
			}
			else
			{
				_storageQueue.Enqueue(StorageAction.SaveProfile);
				GameConsole.PrintString("StorageManager: Save profile (" + player.Name + ") settings request queued.");
			}
			_profileQueue.Enqueue(player);
		}
		else if (_deviceState == StorageDeviceState.NoDevice)
		{
			GameConsole.PrintString("StorageManager: No storage Device. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Disconnected)
		{
			GameConsole.PrintString("StorageManager: Storage device missing. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Full)
		{
			GameConsole.PrintString("StorageManager: Not enough space available on storage device. Save Cancelled");
		}
		else if (_deviceState == StorageDeviceState.NotSelected)
		{
			GameConsole.PrintString("StorageManager: No storage device has been selected. Save Cancelled");
		}
	}

	public void Delete(ref Player player)
	{
		if (_deviceState == StorageDeviceState.Ready || _deviceState == StorageDeviceState.Working || _deviceState == StorageDeviceState.Selecting)
		{
			_storageQueue.Enqueue(StorageAction.DeleteProfile);
			GameConsole.PrintString("StorageManager: Delete profile (" + player.Name + ") request queued.");
			_profileQueue.Enqueue(player);
		}
		else if (_deviceState == StorageDeviceState.NoDevice)
		{
			GameConsole.PrintString("StorageManager: No storage Device. Delete Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Disconnected)
		{
			GameConsole.PrintString("StorageManager: Storage device missing. Delete Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Full)
		{
			GameConsole.PrintString("StorageManager: Not enough space available on storage device. Delete Cancelled");
		}
		else if (_deviceState == StorageDeviceState.NotSelected)
		{
			GameConsole.PrintString("StorageManager: No storage device has been selected. Delete Cancelled");
		}
	}

	public void Delete(MinigameMeta minigameData, bool ratings)
	{
		if (_deviceState == StorageDeviceState.Ready || _deviceState == StorageDeviceState.Working || _deviceState == StorageDeviceState.Selecting)
		{
			for (int i = 0; i != _minigameData.Length; i++)
			{
				if (_minigameData[i].MinigameID == minigameData.MinigameID)
				{
					_minigameData[i].Rating = 0;
					_minigameData[i].BestScore = 0f;
					_minigameData[i].BestWinner = "";
				}
			}
			_storageQueue.Enqueue(StorageAction.SaveGameData);
			GameConsole.PrintString("StorageManager: Delete game data (" + minigameData.Name + (ratings ? " ratings" : " scores") + ") request queued.");
		}
		else if (_deviceState == StorageDeviceState.NoDevice)
		{
			GameConsole.PrintString("StorageManager: No storage Device. Delete Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Disconnected)
		{
			GameConsole.PrintString("StorageManager: Storage device missing. Delete Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Full)
		{
			GameConsole.PrintString("StorageManager: Not enough space available on storage device. Delete Cancelled");
		}
		else if (_deviceState == StorageDeviceState.NotSelected)
		{
			GameConsole.PrintString("StorageManager: No storage device has been selected. Delete Cancelled");
		}
	}

	public void Delete(MinigameMeta[] minigameData, bool ratings)
	{
		if (_deviceState == StorageDeviceState.Ready || _deviceState == StorageDeviceState.Working || _deviceState == StorageDeviceState.Selecting)
		{
			for (int i = 0; i < minigameData.Length; i++)
			{
				for (int j = 0; j != _minigameData.Length; j++)
				{
					if (_minigameData[j].MinigameID == minigameData[i].MinigameID)
					{
						_minigameData[j].Rating = 0;
						_minigameData[j].BestScore = 0f;
						_minigameData[j].BestWinner = "";
					}
				}
			}
			_storageQueue.Enqueue(StorageAction.SaveGameData);
			GameConsole.PrintString("StorageManager: Delete game data (minigame" + (ratings ? " ratings" : " scores") + ") request queued.");
		}
		else if (_deviceState == StorageDeviceState.NoDevice)
		{
			GameConsole.PrintString("StorageManager: No storage Device. Delete Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Disconnected)
		{
			GameConsole.PrintString("StorageManager: Storage device missing. Delete Cancelled");
		}
		else if (_deviceState == StorageDeviceState.Full)
		{
			GameConsole.PrintString("StorageManager: Not enough space available on storage device. Delete Cancelled");
		}
		else if (_deviceState == StorageDeviceState.NotSelected)
		{
			GameConsole.PrintString("StorageManager: No storage device has been selected. Delete Cancelled");
		}
	}

	private void ProcessStorage()
	{
		_storageThread.SetProcessorAffinity(_storageThreadAffinity);
		_storageThread.IsBackground = true;
		foreach (StorageAction item in _storageQueue)
		{
			_ = item;
		}
		while (_storageQueue.Count != 0)
		{
			switch (_storageQueue.Dequeue())
			{
			case StorageAction.LoadGameData:
				LoadGameData();
				break;
			case StorageAction.LoadProfile:
			{
				Player player = _profileQueue.Dequeue();
				LoadProfile(player, settingsOnly: false);
				break;
			}
			case StorageAction.LoadSettingsFromProfile:
			{
				Player player = _profileQueue.Dequeue();
				LoadProfile(player, settingsOnly: false);
				_soundManager.MusicVolume = player.MusicVolume;
				_soundManager.EffectVolume = player.EffectVolume;
				break;
			}
			case StorageAction.SaveGameData:
				SaveGameData();
				break;
			case StorageAction.SaveProfile:
			{
				Player player = _profileQueue.Dequeue();
				if (player.Gamer != null && player.Name != "Default")
				{
					SaveProfile(player, saveSettings: false);
				}
				break;
			}
			case StorageAction.SaveFullProfile:
			{
				Player player = _profileQueue.Dequeue();
				if (player.Gamer != null && player.Name != "Default")
				{
					SaveProfile(player, saveSettings: true);
				}
				break;
			}
			case StorageAction.DeleteProfile:
			{
				Player player = _profileQueue.Dequeue();
				if (_storageContainer.FileExists(player.Name))
				{
					_storageContainer.DeleteFile(player.Name);
				}
				break;
			}
			}
		}
		_storageContainer.Dispose();
		_storageContainer = null;
		_storageContainerRequestResult = null;
		_deviceState = StorageDeviceState.Ready;
	}
}
