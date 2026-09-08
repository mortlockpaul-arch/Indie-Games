using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Quasar.GameUtils.Audio;
using Quasar.GameUtils.Sections;
using Quasar.GameUtils.Storage;
using Quasar.GameUtils.Tasks;
using Quasar.Global;
using Quasar.Render;

namespace Quasar.GameUtils.Game;

public abstract class BaseGame : Quasar.Game
{
	public delegate void StorageDeviceSelectedHandler(StorageDevice device, PlayerIndex who);

	private GameSection currentGameSection;

	private GameSection nextGameSection;

	private List<ExtraSection> extraSections = new List<ExtraSection>(4);

	private List<ExtraSection> newExtraSections = new List<ExtraSection>(1);

	private List<ExtraSection> removeExtraSections = new List<ExtraSection>(1);

	private StorageDevice[] playerStorageDevices = new StorageDevice[4];

	private ISignedInGamer[] gamerStorageDevices = new ISignedInGamer[4];

	private bool useXACTJukebox;

	private Jukebox menuJukebox;

	private RenderProcess mainRenderProcess;

	private RenderPass2D clearPass;

	protected int state;

	private bool gameInit;

	private int preloadTaskId;

	private static bool stopPreloading;

	public int NextGameSectionId
	{
		set
		{
			NextGameSection = GetSection(value);
		}
	}

	public GameSection CurrentGameSection => currentGameSection;

	public GameSection NextGameSection
	{
		get
		{
			return nextGameSection;
		}
		set
		{
			nextGameSection = value;
		}
	}

	public static BaseGame Instance => (BaseGame)Engine.Game;

	public int State => state;

	public void AddExtraSection(ExtraSection section)
	{
		newExtraSections.Add(section);
	}

	public void RemoveExtraSection(ExtraSection section)
	{
		removeExtraSections.Add(section);
	}

	public void ClearExtraSections()
	{
		removeExtraSections.AddRange(extraSections);
		newExtraSections.Clear();
	}

	public StorageDevice GetPlayerStorageDevice(PlayerIndex playerIndex)
	{
		if (PlatformInterface.Instance.GetGamer(playerIndex) == gamerStorageDevices[(int)playerIndex])
		{
			return playerStorageDevices[(int)playerIndex];
		}
		gamerStorageDevices[(int)playerIndex] = null;
		playerStorageDevices[(int)playerIndex] = null;
		return null;
	}

	public void ClearStorageDevices()
	{
		for (int i = 0; i < 4; i++)
		{
			playerStorageDevices[i] = null;
			gamerStorageDevices[i] = null;
		}
	}

	public static bool PlayerStorageDeviceReady(PlayerIndex playerIndex)
	{
		return PlayerStorageDevice(playerIndex)?.IsConnected ?? false;
	}

	public static StorageDevice PlayerStorageDevice(PlayerIndex playerIndex)
	{
		if (Engine.Game == null)
		{
			return null;
		}
		return ((BaseGame)Engine.Game).GetPlayerStorageDevice(playerIndex);
	}

	private void storageDeviceSelected(IAsyncResult ar)
	{
		StorageDevice storageDevice = StorageDevice.EndShowSelector(ar);
		KeyValuePair<PlayerIndex, StorageDeviceSelectedHandler> keyValuePair = (KeyValuePair<PlayerIndex, StorageDeviceSelectedHandler>)ar.AsyncState;
		gamerStorageDevices[(int)keyValuePair.Key] = PlatformInterface.Instance.GetGamer(keyValuePair.Key);
		playerStorageDevices[(int)keyValuePair.Key] = storageDevice;
		keyValuePair.Value(storageDevice, keyValuePair.Key);
	}

	public bool AskForStorageDevice(PlayerIndex who, StorageDeviceSelectedHandler handler)
	{
		try
		{
			ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(who);
			if (gamer != null && gamer.IsGuest)
			{
				handler(null, who);
				return true;
			}
			StorageDevice.BeginShowSelector(who, storageDeviceSelected, new KeyValuePair<PlayerIndex, StorageDeviceSelectedHandler>(who, handler));
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public BaseGame(Microsoft.Xna.Framework.Game game, string containerName, bool useXACTJukebox)
		: base(game)
	{
		this.useXACTJukebox = useXACTJukebox;
		StorageManager.ContainerName = containerName;
		LoadConfig();
	}

	public BaseGame(string containerName, bool useXACTJukebox)
	{
		this.useXACTJukebox = useXACTJukebox;
		StorageManager.ContainerName = containerName;
		DirectoryManager.Initialize();
		LoadConfig();
	}

	protected abstract void LoadTemplate();

	protected void LoadSection(GameSection section)
	{
		LoadSection(section, initSection: true);
	}

	protected void ReloadSection()
	{
		if (CurrentGameSection != null)
		{
			CurrentGameSection.Dispose();
		}
		LoadSection(nextGameSection, initSection: true);
		nextGameSection = null;
	}

	public override void InitGame()
	{
		if (!gameInit)
		{
			gameInit = true;
			LoadTemplate();
			if (!useXACTJukebox)
			{
				menuJukebox = new Jukebox("Menu_");
			}
			PlatformInterface instance = PlatformInterface.Instance;
			instance.SignedIn = (Action<PlayerIndex, ISignedInGamer>)Delegate.Combine(instance.SignedIn, new Action<PlayerIndex, ISignedInGamer>(SignedIn));
			PlatformInterface instance2 = PlatformInterface.Instance;
			instance2.SignedOut = (Action<PlayerIndex, ISignedInGamer>)Delegate.Combine(instance2.SignedOut, new Action<PlayerIndex, ISignedInGamer>(SignedOut));
			mainRenderProcess = new RenderProcess((RenderPass)null);
			clearPass = new RenderPass2D(createRenderTarget: false);
			clearPass.BackgroundColor = Color.Purple;
			clearPass.MustClearColor = true;
			clearPass.MustClearDepth = true;
			NextGameSectionId = 3;
			StartPreloadTextures();
		}
	}

	public virtual void FinishGame()
	{
		state = 100;
	}

	private void SignedOut(PlayerIndex playerIndex, ISignedInGamer g)
	{
		SignedIn(playerIndex, null);
	}

	public abstract void LoadConfig();

	protected abstract bool IsSignInSafe(PlayerIndex playerIndex, int state);

	protected abstract bool IsMenuState(int state);

	public virtual void GoToMainScreen()
	{
		NextGameSectionId = 0;
	}

	private void SignedIn(PlayerIndex playerIndex, ISignedInGamer g)
	{
		if (!IsSignInSafe(playerIndex, state))
		{
			GoToMainScreen();
		}
	}

	protected virtual void LoadSection(GameSection section, bool initSection)
	{
		currentGameSection = section;
		if (initSection && !currentGameSection.Initialized)
		{
			currentGameSection.InitSection();
		}
		if (section != null)
		{
			state = section.Id;
		}
		if (IsMenuState(state))
		{
			StartMenuSong();
		}
		else
		{
			StopMenuSong();
		}
	}

	private void StartMenuSong()
	{
		if (useXACTJukebox)
		{
			XACTJukebox.Instance.StartSonglist("Menu");
		}
		else if (!menuJukebox.Playing)
		{
			menuJukebox.Start();
		}
	}

	private void StopMenuSong()
	{
		if (useXACTJukebox)
		{
			if (XACTJukebox.HasInstance && XACTJukebox.Instance.CurrentSongList == "Menu")
			{
				XACTJukebox.Instance.Stop();
			}
		}
		else if (menuJukebox.Playing)
		{
			menuJukebox.Stop();
		}
	}

	public virtual GameSection GetSection(int id)
	{
		return id switch
		{
			2 => new MilkstoneSplashSection(), 
			3 => new GameSplashSection(), 
			_ => null, 
		};
	}

	public override void MainLoop()
	{
		try
		{
			PlatformInterface.Instance.Update();
			DelayedActionManager.Instance.Update();
			if (nextGameSection != null)
			{
				ReloadSection();
			}
			foreach (ExtraSection removeExtraSection in removeExtraSections)
			{
				extraSections.Remove(removeExtraSection);
			}
			removeExtraSections.Clear();
			foreach (ExtraSection newExtraSection in newExtraSections)
			{
				newExtraSection.InitSection();
				extraSections.Add(newExtraSection);
			}
			newExtraSections.Clear();
			CurrentGameSection.MainLoop();
			foreach (ExtraSection extraSection in extraSections)
			{
				extraSection.MainLoop();
			}
			TaskManager.Update();
			if (useXACTJukebox)
			{
				XACTJukebox.Instance.Update();
			}
			if (state == 100)
			{
				finished = true;
			}
			if (CurrentGameSection != null)
			{
				CurrentGameSection.UpdateScenes();
			}
			foreach (ExtraSection extraSection2 in extraSections)
			{
				extraSection2.UpdateScenes();
			}
			renderProcesses.Clear();
			foreach (RenderProcess extraRenderProcess in CurrentGameSection.ExtraRenderProcesses)
			{
				renderProcesses.Add(extraRenderProcess);
			}
			foreach (ExtraSection extraSection3 in extraSections)
			{
				foreach (RenderProcess extraRenderProcess2 in extraSection3.ExtraRenderProcesses)
				{
					renderProcesses.Add(extraRenderProcess2);
				}
			}
			mainRenderProcess.ClearPasses();
			mainRenderProcess.addIntermediatePass(clearPass);
			foreach (ExtraSection extraSection4 in extraSections)
			{
				if (extraSection4.DrawPriority == ExtraSection.Priority.Background)
				{
					mainRenderProcess.addIntermediatePass(extraSection4.MainRenderPass);
				}
			}
			mainRenderProcess.addIntermediatePass(CurrentGameSection.MainRenderPass);
			foreach (ExtraSection extraSection5 in extraSections)
			{
				if (extraSection5.DrawPriority == ExtraSection.Priority.Foreground)
				{
					mainRenderProcess.addIntermediatePass(extraSection5.MainRenderPass);
				}
			}
			renderProcesses.Add(mainRenderProcess);
		}
		catch (Exception e)
		{
			ClearExtraSections();
			NextGameSection = new ExceptionSection(e);
		}
	}

	public void WaitForPreloadFinish(bool stopPreload)
	{
		if (preloadTaskId != 0)
		{
			if (stopPreload)
			{
				stopPreloading = true;
			}
			TaskManager.WaitForTask(preloadTaskId, 180000);
		}
	}

	public void StartPreloadTextures()
	{
		preloadTaskId = TaskManager.Post(preloadTextures, null);
	}

	protected static void preloadAssetDir(string subPath)
	{
		preloadAssetDir(subPath, recursive: false, wait: false);
	}

	protected static void preloadAssetDir(string subPath, bool recursive, bool wait)
	{
		preloadAssetDir<object>(subPath, recursive, wait);
	}

	protected static void preloadAssetDir<T>(string subPath, bool recursive, bool wait)
	{
		if (string.Equals(subPath, "Shaders/"))
		{
			return;
		}
		DirectoryManager.FileInfo[] files = DirectoryManager.GetFiles(subPath, recursive);
		long currentTotalTime = Quasar.Global.Timer.DefaultTimer.CurrentTotalTime;
		DirectoryManager.FileInfo[] array = files;
		foreach (DirectoryManager.FileInfo fileInfo in array)
		{
			if (stopPreloading)
			{
				break;
			}
			try
			{
				Engine.ContentManager.Load<T>(fileInfo.FullPath);
				if (wait)
				{
					long currentTotalTime2 = Quasar.Global.Timer.DefaultTimer.CurrentTotalTime;
					if (currentTotalTime2 - currentTotalTime >= 20)
					{
						Thread.Sleep(35);
						currentTotalTime = Quasar.Global.Timer.DefaultTimer.CurrentTotalTime;
					}
				}
			}
			catch (Exception)
			{
			}
		}
		if (wait && !stopPreloading)
		{
			long currentTotalTime3 = Quasar.Global.Timer.DefaultTimer.CurrentTotalTime;
			if (currentTotalTime3 - currentTotalTime >= 50)
			{
				Thread.Sleep(35);
				currentTotalTime = Quasar.Global.Timer.DefaultTimer.CurrentTotalTime;
			}
		}
	}

	private void preloadTextures(object parameters)
	{
		try
		{
			PreloadThings();
		}
		catch (Exception)
		{
		}
		preloadTaskId = 0;
	}

	protected virtual void PreloadThings()
	{
	}

	public override void ErrorFound(Exception ex)
	{
		ClearExtraSections();
		NextGameSection = new ExceptionSection(ex);
	}

	~BaseGame()
	{
		Dispose();
	}

	public override void Dispose()
	{
		WaitForPreloadFinish(stopPreload: true);
		TaskManager.EndAllTasks();
		if (menuJukebox != null)
		{
			menuJukebox.Stop();
		}
		menuJukebox = null;
		if (currentGameSection != null)
		{
			currentGameSection.Dispose();
		}
		nextGameSection = null;
		currentGameSection = null;
		base.Dispose();
		GC.SuppressFinalize(this);
	}
}
