using System;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic;
using Quasar.GameUtils.Tasks;
using Quasar.Global;

namespace Quasar.GameUtils.Storage;

public class StorageManager : IDisposable
{
	public delegate void PlayerIOOperation(StorageContainer container, object parameters);

	private class PlayerIOOperationData
	{
		public PlayerIOOperation Operation;

		public IOType Type;

		public PlayerIndex playerIndex;

		public object parameters;

		public void Initialize(PlayerIOOperation operation, object parameters, IOType type, PlayerIndex playerIndex)
		{
			Operation = operation;
			Type = type;
			this.parameters = parameters;
			this.playerIndex = playerIndex;
		}

		public void Clear()
		{
			Operation = null;
			parameters = null;
			playerIndex = PlayerIndex.One;
		}
	}

	public delegate void GlobalIOOperation(IsolatedStorageFile container, object parameters);

	public enum IOType
	{
		Read,
		Write,
		WriteNotImportant
	}

	private class GlobalIOOperationData
	{
		public GlobalIOOperation Operation;

		public IOType Type;

		public object parameters;

		public void Initialize(GlobalIOOperation operation, object parameters, IOType type)
		{
			Operation = operation;
			Type = type;
			this.parameters = parameters;
		}

		public void Clear()
		{
			Operation = null;
			parameters = null;
		}
	}

	private static string containerName = "";

	private bool[] playerSavePending = new bool[4];

	private object[] playerContainerLocks = new object[4]
	{
		new object(),
		new object(),
		new object(),
		new object()
	};

	private StorageContainer[] playerContainers = new StorageContainer[4];

	private static StorageManager instance = null;

	private bool saveEnabled = true;

	private object isolatedStorageLock = new object();

	private IsolatedStorageFile isolatedStorage;

	public static string ContainerName
	{
		set
		{
			containerName = value;
		}
	}

	public static StorageManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new StorageManager();
			}
			return instance;
		}
	}

	public bool SaveEnabled
	{
		get
		{
			return saveEnabled;
		}
		set
		{
			saveEnabled = value;
			if (saveEnabled)
			{
				SaveNow();
			}
		}
	}

	public void Exec(PlayerIndex playerIndex, PlayerIOOperation operation, object parameters, IOType ioType)
	{
		TaskManager.Exec(performPlayerOperation, fetchPlayerOperation(operation, parameters, ioType, playerIndex));
	}

	private PlayerIOOperationData fetchPlayerOperation(PlayerIOOperation operation, object parameters, IOType type, PlayerIndex playerIndex)
	{
		PlayerIOOperationData playerIOOperationData = Pool<PlayerIOOperationData>.Fetch();
		playerIOOperationData.Initialize(operation, parameters, type, playerIndex);
		return playerIOOperationData;
	}

	private void operationFinished(PlayerIOOperationData data)
	{
		data.Clear();
		Pool<PlayerIOOperationData>.Insert(data);
	}

	public int Post(PlayerIndex playerIndex, PlayerIOOperation operation, object parameters, IOType ioType, Action<object> finishCallback)
	{
		return TaskManager.Post(performPlayerOperation, fetchPlayerOperation(operation, parameters, ioType, playerIndex), finishCallback);
	}

	public int Post(PlayerIndex playerIndex, PlayerIOOperation operation, object parameters, IOType ioType)
	{
		return Post(playerIndex, operation, parameters, ioType, null);
	}

	private void performPlayerOperation(object parameters)
	{
		PlayerIOOperationData playerIOOperationData = (PlayerIOOperationData)parameters;
		try
		{
			lock (playerContainerLocks[(int)playerIOOperationData.playerIndex])
			{
				checkPlayerContainer(playerIOOperationData.playerIndex);
				try
				{
					playerIOOperationData.Operation(playerContainers[(int)playerIOOperationData.playerIndex], playerIOOperationData.parameters);
				}
				catch (Exception)
				{
				}
				if (playerIOOperationData.Type == IOType.Write)
				{
					if (saveEnabled)
					{
						reopenPlayerContainer(playerIOOperationData.playerIndex);
						playerSavePending[(int)playerIOOperationData.playerIndex] = false;
					}
					else
					{
						playerSavePending[(int)playerIOOperationData.playerIndex] = true;
					}
				}
				else if (playerIOOperationData.Type == IOType.WriteNotImportant)
				{
					playerSavePending[(int)playerIOOperationData.playerIndex] = true;
				}
			}
			operationFinished(playerIOOperationData);
		}
		catch (Exception)
		{
		}
	}

	private void checkPlayerContainer(PlayerIndex playerIndex)
	{
		StorageDevice device = getDevice(playerIndex);
		if (device == null || !device.IsConnected)
		{
			closePlayerContainer(playerIndex);
		}
		else if (playerContainers[(int)playerIndex] == null)
		{
			openPlayerContainer(playerIndex);
		}
	}

	private StorageDevice getDevice(PlayerIndex playerIndex)
	{
		return BaseGame.PlayerStorageDevice(playerIndex);
	}

	private void openPlayerContainer(PlayerIndex playerIndex)
	{
		if (playerContainers[(int)playerIndex] != null)
		{
			return;
		}
		StorageDevice device = getDevice(playerIndex);
		if (device != null && device.IsConnected)
		{
			try
			{
				IAsyncResult asyncResult = device.BeginOpenContainer(containerName, null, null);
				asyncResult.AsyncWaitHandle.WaitOne();
				playerContainers[(int)playerIndex] = device.EndOpenContainer(asyncResult);
			}
			catch (Exception)
			{
				playerContainers[(int)playerIndex] = null;
			}
		}
	}

	private void closePlayerContainer(PlayerIndex playerIndex)
	{
		if (playerContainers[(int)playerIndex] != null)
		{
			playerContainers[(int)playerIndex].Dispose();
			playerContainers[(int)playerIndex] = null;
		}
	}

	private void reopenPlayerContainer(PlayerIndex playerIndex)
	{
	}

	private void ReopenContainer(StorageContainer container, object parameters)
	{
		reopenPlayerContainer((PlayerIndex)parameters);
	}

	public void SaveNow()
	{
		for (int i = 0; i < 4; i++)
		{
			if (playerSavePending[i])
			{
				Post((PlayerIndex)i, ReopenContainer, i, IOType.Read);
				playerSavePending[i] = false;
			}
		}
	}

	private StorageManager()
	{
		if (containerName.Length == 0)
		{
			throw new Exception("Container name has not been set up!");
		}
		Engine.RegisterDisposeHandler(OnDispose);
		Pool<GlobalIOOperationData>.ThreadSafe = true;
		Pool<PlayerIOOperationData>.ThreadSafe = true;
	}

	public void Exec(GlobalIOOperation operation, object parameters, IOType ioType)
	{
		TaskManager.Exec(performGlobalOperation, fetchGlobalOperation(operation, parameters, ioType));
	}

	private GlobalIOOperationData fetchGlobalOperation(GlobalIOOperation operation, object parameters, IOType type)
	{
		GlobalIOOperationData globalIOOperationData = Pool<GlobalIOOperationData>.Fetch();
		globalIOOperationData.Initialize(operation, parameters, type);
		return globalIOOperationData;
	}

	private void operationFinished(GlobalIOOperationData data)
	{
		data.Clear();
		Pool<GlobalIOOperationData>.Insert(data);
	}

	public int Post(GlobalIOOperation operation, object parameters, IOType ioType, Action<object> finishCallback)
	{
		return TaskManager.Post(performGlobalOperation, fetchGlobalOperation(operation, parameters, ioType), finishCallback);
	}

	public int Post(GlobalIOOperation operation, object parameters, IOType ioType)
	{
		return Post(operation, parameters, ioType, null);
	}

	private void performGlobalOperation(object parameters)
	{
		GlobalIOOperationData globalIOOperationData = (GlobalIOOperationData)parameters;
		try
		{
			lock (isolatedStorageLock)
			{
				checkIsolatedStorage();
				try
				{
					globalIOOperationData.Operation(isolatedStorage, globalIOOperationData.parameters);
				}
				catch (Exception)
				{
				}
			}
			operationFinished(globalIOOperationData);
		}
		catch (Exception)
		{
		}
	}

	private void checkIsolatedStorage()
	{
		if (isolatedStorage == null)
		{
			openIsolatedStorage();
		}
	}

	private void openIsolatedStorage()
	{
		isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
	}

	private void closeIsolatedStorage()
	{
		if (isolatedStorage != null)
		{
			isolatedStorage.Dispose();
			isolatedStorage = null;
		}
	}

	private void OnDispose()
	{
		Dispose();
	}

	public void Dispose()
	{
		closeIsolatedStorage();
		for (int i = 0; i < 4; i++)
		{
			closePlayerContainer((PlayerIndex)i);
		}
	}
}
