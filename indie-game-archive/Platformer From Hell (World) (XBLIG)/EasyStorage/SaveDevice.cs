using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace EasyStorage;

public abstract class SaveDevice : IGameComponent, IUpdateable, IAsyncSaveDevice, ISaveDevice
{
	private class FileOperationState
	{
		public string Container;

		public string File;

		public string Pattern;

		public FileAction Action;

		public object UserState;

		public void Reset()
		{
			Container = null;
			File = null;
			Pattern = null;
			Action = null;
			UserState = null;
		}
	}

	private static string promptForCancelledMessage;

	private static string forceCancelledReselectionMessage;

	private static string promptForDisconnectedMessage;

	private static string forceDisconnectedReselectionMessage;

	private static string deviceRequiredTitle;

	private static string deviceOptionalTitle;

	private static readonly string[] deviceOptionalOptions;

	private static readonly string[] deviceRequiredOptions;

	private int updateOrder;

	private bool enabled = true;

	private bool deviceWasConnected;

	private SaveDevicePromptState state = SaveDevicePromptState.None;

	private readonly AsyncCallback storageDeviceSelectorCallback;

	private readonly AsyncCallback forcePromptCallback;

	private readonly AsyncCallback reselectPromptCallback;

	private readonly SaveDevicePromptEventArgs promptEventArgs = new SaveDevicePromptEventArgs();

	private readonly SaveDeviceEventArgs eventArgs = new SaveDeviceEventArgs();

	private StorageDevice storageDevice;

	public static readonly int[] ProcessorAffinity;

	private Queue<FileOperationState> pendingStates = new Queue<FileOperationState>(100);

	private readonly object pendingOperationCountLock = new object();

	private int pendingOperations;

	public static string PromptForCancelledMessage
	{
		get
		{
			return promptForCancelledMessage;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				promptForCancelledMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
			}
		}
	}

	public static string ForceCancelledReselectionMessage
	{
		get
		{
			return forceCancelledReselectionMessage;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				forceCancelledReselectionMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
			}
		}
	}

	public static string PromptForDisconnectedMessage
	{
		get
		{
			return promptForDisconnectedMessage;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				promptForDisconnectedMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
			}
		}
	}

	public static string ForceDisconnectedReselectionMessage
	{
		get
		{
			return forceDisconnectedReselectionMessage;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				forceDisconnectedReselectionMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
			}
		}
	}

	public static string DeviceRequiredTitle
	{
		get
		{
			return deviceRequiredTitle;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				deviceRequiredTitle = ((value.Length < 256) ? value : value.Substring(0, 256));
			}
		}
	}

	public static string DeviceOptionalTitle
	{
		get
		{
			return deviceOptionalTitle;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				deviceOptionalTitle = ((value.Length < 256) ? value : value.Substring(0, 256));
			}
		}
	}

	public static string OkOption
	{
		get
		{
			return deviceRequiredOptions[0];
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				deviceRequiredOptions[0] = ((value.Length < 256) ? value : value.Substring(0, 256));
			}
		}
	}

	public static string YesOption
	{
		get
		{
			return deviceOptionalOptions[0];
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				deviceOptionalOptions[0] = ((value.Length < 256) ? value : value.Substring(0, 256));
			}
		}
	}

	public static string NoOption
	{
		get
		{
			return deviceOptionalOptions[1];
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				deviceOptionalOptions[1] = ((value.Length < 256) ? value : value.Substring(0, 256));
			}
		}
	}

	public bool IsReady => storageDevice != null && storageDevice.IsConnected;

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			if (enabled != value)
			{
				enabled = value;
				if (EnabledChanged != null)
				{
					EnabledChanged(this, null);
				}
			}
		}
	}

	public int UpdateOrder
	{
		get
		{
			return updateOrder;
		}
		set
		{
			if (updateOrder != value)
			{
				updateOrder = value;
				if (UpdateOrderChanged != null)
				{
					UpdateOrderChanged(this, null);
				}
			}
		}
	}

	public bool IsBusy
	{
		get
		{
			lock (pendingOperationCountLock)
			{
				return pendingOperations > 0;
			}
		}
	}

	public event EventHandler<EventArgs> DeviceSelected;

	public event EventHandler<SaveDeviceEventArgs> DeviceSelectorCanceled;

	public event EventHandler<SaveDevicePromptEventArgs> DeviceReselectPromptClosed;

	public event EventHandler<SaveDeviceEventArgs> DeviceDisconnected;

	public event EventHandler<EventArgs> EnabledChanged;

	public event EventHandler<EventArgs> UpdateOrderChanged;

	public event SaveCompletedEventHandler SaveCompleted;

	public event LoadCompletedEventHandler LoadCompleted;

	public event DeleteCompletedEventHandler DeleteCompleted;

	public event FileExistsCompletedEventHandler FileExistsCompleted;

	public event GetFilesCompletedEventHandler GetFilesCompleted;

	private StorageContainer OpenContainer(string containerName)
	{
		IAsyncResult asyncResult = storageDevice.BeginOpenContainer(containerName, null, null);
		asyncResult.AsyncWaitHandle.WaitOne();
		return storageDevice.EndOpenContainer(asyncResult);
	}

	private void VerifyIsReady()
	{
		if (!IsReady)
		{
			throw new InvalidOperationException(Strings.StorageDevice_is_not_valid);
		}
	}

	public void Save(string containerName, string fileName, FileAction saveAction)
	{
		VerifyIsReady();
		lock (storageDevice)
		{
			using StorageContainer storageContainer = OpenContainer(containerName);
			using Stream stream = storageContainer.CreateFile(fileName);
			saveAction(stream);
		}
	}

	public void Load(string containerName, string fileName, FileAction loadAction)
	{
		VerifyIsReady();
		lock (storageDevice)
		{
			using StorageContainer storageContainer = OpenContainer(containerName);
			using Stream stream = storageContainer.OpenFile(fileName, FileMode.Open);
			loadAction(stream);
		}
	}

	public void Delete(string containerName, string fileName)
	{
		VerifyIsReady();
		lock (storageDevice)
		{
			using StorageContainer storageContainer = OpenContainer(containerName);
			if (storageContainer.FileExists(fileName))
			{
				storageContainer.DeleteFile(fileName);
			}
		}
	}

	public bool FileExists(string containerName, string fileName)
	{
		VerifyIsReady();
		lock (storageDevice)
		{
			using StorageContainer storageContainer = OpenContainer(containerName);
			return storageContainer.FileExists(fileName);
		}
	}

	public string[] GetFiles(string containerName)
	{
		return GetFiles(containerName, null);
	}

	public string[] GetFiles(string containerName, string pattern)
	{
		VerifyIsReady();
		lock (storageDevice)
		{
			using StorageContainer storageContainer = OpenContainer(containerName);
			return string.IsNullOrEmpty(pattern) ? storageContainer.GetFileNames() : storageContainer.GetFileNames(pattern);
		}
	}

	static SaveDevice()
	{
		deviceOptionalOptions = new string[2];
		deviceRequiredOptions = new string[1];
		ProcessorAffinity = new int[1] { 5 };
		EasyStorageSettings.ResetSaveDeviceStrings();
	}

	protected SaveDevice()
	{
		storageDeviceSelectorCallback = StorageDeviceSelectorCallback;
		reselectPromptCallback = ReselectPromptCallback;
		forcePromptCallback = ForcePromptCallback;
	}

	public virtual void Initialize()
	{
	}

	public void PromptForDevice()
	{
		if (state == SaveDevicePromptState.None)
		{
			state = SaveDevicePromptState.ShowSelector;
		}
	}

	protected abstract void GetStorageDevice(AsyncCallback callback);

	protected virtual void PrepareEventArgs(SaveDeviceEventArgs args)
	{
		args.Response = SaveDeviceEventResponse.Prompt;
		args.PlayerToPrompt = null;
	}

	public void Update(GameTime gameTime)
	{
		if (!GamerServicesDispatcher.IsInitialized)
		{
			throw new InvalidOperationException(Strings.NeedGamerService);
		}
		bool flag = storageDevice != null && storageDevice.IsConnected;
		if (!flag && deviceWasConnected)
		{
			PrepareEventArgs(eventArgs);
			if (DeviceDisconnected != null)
			{
				DeviceDisconnected(this, eventArgs);
			}
			HandleEventArgResults();
		}
		else if (!flag)
		{
			try
			{
				if (!Guide.IsVisible)
				{
					switch (state)
					{
					case SaveDevicePromptState.ShowSelector:
						state = SaveDevicePromptState.None;
						GetStorageDevice(storageDeviceSelectorCallback);
						break;
					case SaveDevicePromptState.PromptForCanceled:
						ShowMessageBox(eventArgs.PlayerToPrompt, deviceOptionalTitle, promptForCancelledMessage, deviceOptionalOptions, reselectPromptCallback);
						break;
					case SaveDevicePromptState.ForceCanceledReselection:
						ShowMessageBox(eventArgs.PlayerToPrompt, deviceRequiredTitle, forceCancelledReselectionMessage, deviceRequiredOptions, forcePromptCallback);
						break;
					case SaveDevicePromptState.PromptForDisconnected:
						ShowMessageBox(eventArgs.PlayerToPrompt, deviceOptionalTitle, promptForDisconnectedMessage, deviceOptionalOptions, reselectPromptCallback);
						break;
					case SaveDevicePromptState.ForceDisconnectedReselection:
						ShowMessageBox(eventArgs.PlayerToPrompt, deviceRequiredTitle, forceDisconnectedReselectionMessage, deviceRequiredOptions, forcePromptCallback);
						break;
					}
				}
			}
			catch (GuideAlreadyVisibleException)
			{
			}
		}
		deviceWasConnected = flag;
	}

	private void StorageDeviceSelectorCallback(IAsyncResult result)
	{
		storageDevice = StorageDevice.EndShowSelector(result);
		if (storageDevice != null && storageDevice.IsConnected)
		{
			if (DeviceSelected != null)
			{
				DeviceSelected(this, null);
			}
			return;
		}
		PrepareEventArgs(eventArgs);
		if (DeviceSelectorCanceled != null)
		{
			DeviceSelectorCanceled(this, eventArgs);
		}
		HandleEventArgResults();
	}

	private void ForcePromptCallback(IAsyncResult result)
	{
		Guide.EndShowMessageBox(result);
		state = SaveDevicePromptState.ShowSelector;
	}

	private void ReselectPromptCallback(IAsyncResult result)
	{
		int? num = Guide.EndShowMessageBox(result);
		state = ((num.HasValue && num.Value == 0) ? SaveDevicePromptState.ShowSelector : SaveDevicePromptState.None);
		promptEventArgs.ShowDeviceSelector = state == SaveDevicePromptState.ShowSelector;
		if (DeviceReselectPromptClosed != null)
		{
			DeviceReselectPromptClosed(this, promptEventArgs);
		}
	}

	private void HandleEventArgResults()
	{
		storageDevice = null;
		switch (eventArgs.Response)
		{
		case SaveDeviceEventResponse.Prompt:
			state = (deviceWasConnected ? SaveDevicePromptState.PromptForDisconnected : SaveDevicePromptState.PromptForCanceled);
			break;
		case SaveDeviceEventResponse.Force:
			state = (deviceWasConnected ? SaveDevicePromptState.ForceDisconnectedReselection : SaveDevicePromptState.ForceCanceledReselection);
			break;
		default:
			state = SaveDevicePromptState.None;
			break;
		}
	}

	private static void ShowMessageBox(PlayerIndex? player, string title, string text, IEnumerable<string> buttons, AsyncCallback callback)
	{
		if (player.HasValue)
		{
			Guide.BeginShowMessageBox(player.Value, title, text, buttons, 0, MessageBoxIcon.None, callback, null);
		}
		else
		{
			Guide.BeginShowMessageBox(title, text, buttons, 0, MessageBoxIcon.None, callback, null);
		}
	}

	public void SaveAsync(string containerName, string fileName, FileAction saveAction)
	{
		SaveAsync(containerName, fileName, saveAction, null);
	}

	public void SaveAsync(string containerName, string fileName, FileAction saveAction, object userState)
	{
		PendingOperationsIncrement();
		FileOperationState fileOperationState = GetFileOperationState();
		fileOperationState.Container = containerName;
		fileOperationState.File = fileName;
		fileOperationState.Action = saveAction;
		fileOperationState.UserState = userState;
		ThreadPool.QueueUserWorkItem(DoSaveAsync, fileOperationState);
	}

	public void LoadAsync(string containerName, string fileName, FileAction loadAction)
	{
		LoadAsync(containerName, fileName, loadAction, null);
	}

	public void LoadAsync(string containerName, string fileName, FileAction loadAction, object userState)
	{
		PendingOperationsIncrement();
		FileOperationState fileOperationState = GetFileOperationState();
		fileOperationState.Container = containerName;
		fileOperationState.File = fileName;
		fileOperationState.Action = loadAction;
		fileOperationState.UserState = userState;
		ThreadPool.QueueUserWorkItem(DoLoadAsync, fileOperationState);
	}

	public void DeleteAsync(string containerName, string fileName)
	{
		DeleteAsync(containerName, fileName, null);
	}

	public void DeleteAsync(string containerName, string fileName, object userState)
	{
		PendingOperationsIncrement();
		FileOperationState fileOperationState = GetFileOperationState();
		fileOperationState.Container = containerName;
		fileOperationState.File = fileName;
		fileOperationState.UserState = userState;
		ThreadPool.QueueUserWorkItem(DoDeleteAsync, fileOperationState);
	}

	public void FileExistsAsync(string containerName, string fileName)
	{
		FileExistsAsync(containerName, fileName, null);
	}

	public void FileExistsAsync(string containerName, string fileName, object userState)
	{
		PendingOperationsIncrement();
		FileOperationState fileOperationState = GetFileOperationState();
		fileOperationState.Container = containerName;
		fileOperationState.File = fileName;
		fileOperationState.UserState = userState;
		ThreadPool.QueueUserWorkItem(DoFileExistsAsync, fileOperationState);
	}

	public void GetFilesAsync(string containerName)
	{
		GetFilesAsync(containerName, null);
	}

	public void GetFilesAsync(string containerName, object userState)
	{
		GetFilesAsync(containerName, "*", userState);
	}

	public void GetFilesAsync(string containerName, string pattern)
	{
		GetFilesAsync(containerName, pattern, null);
	}

	public void GetFilesAsync(string containerName, string pattern, object userState)
	{
		PendingOperationsIncrement();
		FileOperationState fileOperationState = GetFileOperationState();
		fileOperationState.Container = containerName;
		fileOperationState.Pattern = pattern;
		fileOperationState.UserState = userState;
		ThreadPool.QueueUserWorkItem(DoGetFilesAsync, fileOperationState);
	}

	private void SetProcessorAffinity()
	{
		Thread.CurrentThread.SetProcessorAffinity(ProcessorAffinity);
	}

	private void DoSaveAsync(object asyncState)
	{
		SetProcessorAffinity();
		FileOperationState fileOperationState = asyncState as FileOperationState;
		Exception error = null;
		try
		{
			Save(fileOperationState.Container, fileOperationState.File, fileOperationState.Action);
		}
		catch (Exception ex)
		{
			error = ex;
		}
		FileActionCompletedEventArgs args = new FileActionCompletedEventArgs(error, fileOperationState.UserState);
		if (SaveCompleted != null)
		{
			SaveCompleted(this, args);
		}
		ReturnFileOperationState(fileOperationState);
		PendingOperationsDecrement();
	}

	private void DoLoadAsync(object asyncState)
	{
		SetProcessorAffinity();
		FileOperationState fileOperationState = asyncState as FileOperationState;
		Exception error = null;
		try
		{
			Load(fileOperationState.Container, fileOperationState.File, fileOperationState.Action);
		}
		catch (Exception ex)
		{
			error = ex;
		}
		FileActionCompletedEventArgs args = new FileActionCompletedEventArgs(error, fileOperationState.UserState);
		if (LoadCompleted != null)
		{
			LoadCompleted(this, args);
		}
		ReturnFileOperationState(fileOperationState);
		PendingOperationsDecrement();
	}

	private void DoDeleteAsync(object asyncState)
	{
		SetProcessorAffinity();
		FileOperationState fileOperationState = asyncState as FileOperationState;
		Exception error = null;
		try
		{
			Delete(fileOperationState.Container, fileOperationState.File);
		}
		catch (Exception ex)
		{
			error = ex;
		}
		FileActionCompletedEventArgs args = new FileActionCompletedEventArgs(error, fileOperationState.UserState);
		if (DeleteCompleted != null)
		{
			DeleteCompleted(this, args);
		}
		ReturnFileOperationState(fileOperationState);
		PendingOperationsDecrement();
	}

	private void DoFileExistsAsync(object asyncState)
	{
		SetProcessorAffinity();
		FileOperationState fileOperationState = asyncState as FileOperationState;
		Exception error = null;
		bool result = false;
		try
		{
			result = FileExists(fileOperationState.Container, fileOperationState.File);
		}
		catch (Exception ex)
		{
			error = ex;
		}
		FileExistsCompletedEventArgs args = new FileExistsCompletedEventArgs(error, result, fileOperationState.UserState);
		if (FileExistsCompleted != null)
		{
			FileExistsCompleted(this, args);
		}
		ReturnFileOperationState(fileOperationState);
		PendingOperationsDecrement();
	}

	private void DoGetFilesAsync(object asyncState)
	{
		SetProcessorAffinity();
		FileOperationState fileOperationState = asyncState as FileOperationState;
		Exception error = null;
		string[] result = null;
		try
		{
			result = GetFiles(fileOperationState.Container, fileOperationState.Pattern);
		}
		catch (Exception ex)
		{
			error = ex;
		}
		GetFilesCompletedEventArgs args = new GetFilesCompletedEventArgs(error, result, fileOperationState.UserState);
		if (GetFilesCompleted != null)
		{
			GetFilesCompleted(this, args);
		}
		ReturnFileOperationState(fileOperationState);
		PendingOperationsDecrement();
	}

	private void PendingOperationsIncrement()
	{
		lock (pendingOperationCountLock)
		{
			pendingOperations++;
		}
	}

	private void PendingOperationsDecrement()
	{
		lock (pendingOperationCountLock)
		{
			pendingOperations--;
		}
	}

	private FileOperationState GetFileOperationState()
	{
		lock (pendingStates)
		{
			if (pendingStates.Count > 0)
			{
				FileOperationState fileOperationState = pendingStates.Dequeue();
				fileOperationState.Reset();
				return fileOperationState;
			}
			return new FileOperationState();
		}
	}

	private void ReturnFileOperationState(FileOperationState state)
	{
		lock (pendingStates)
		{
			pendingStates.Enqueue(state);
		}
	}
}
