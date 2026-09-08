using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PerformanceMeasuring.GameDebugTools;
using Quasar.Input;

namespace Quasar.Global;

public class Engine : IEngine, IDisposable
{
	private const float WIDE_ASPECT = 1.7777778f;

	private GraphicsDevice device;

	private Game game;

	private static Engine instance;

	private ContentTracker contentTracker;

	private TimeRuler timeRuler;

	private static bool wideScreen = false;

	private static int guiWidth = 1280;

	private Thread mainThread;

	private RenderTargetBinding[] currentRenderTargets;

	private RenderTargetBinding[] singleRenderTarget = new RenderTargetBinding[1];

	private static Viewport defaultViewport = default(Viewport);

	private bool finished;

	private uint elapsedFrames;

	private bool isActive = true;

	private BlendState alphaBlendState;

	public static GraphicsDevice Device => Instance.device;

	public Game CurrentGame => game;

	public static Game Game => Instance.game;

	public static Engine Instance => instance;

	public ContentManager Content => contentTracker;

	public static ContentManager ContentManager => Instance.Content;

	public static ContentTracker ContentTracker => (ContentTracker)Instance.Content;

	public TimeRuler TimeRuler => timeRuler;

	public static int BackBufferWidth => Device.PresentationParameters.BackBufferWidth;

	public static int BackBufferHeight => Device.PresentationParameters.BackBufferHeight;

	public static float AspectRatio
	{
		get
		{
			if (!wideScreen)
			{
				return (float)BackBufferWidth / (float)BackBufferHeight;
			}
			return 1.7777778f;
		}
	}

	public static Int2 BackBufferSize => new Int2(BackBufferWidth, BackBufferHeight);

	public static Vector2 GUISize => new Vector2(guiWidth, (float)guiWidth / AspectRatio);

	public static float GUIWidth => guiWidth;

	public static float GUIHeight => (float)guiWidth / AspectRatio;

	public static float GUIScale => (float)BackBufferWidth / (float)guiWidth;

	public Thread MainThread => mainThread;

	public object RenderLock => contentTracker.LoadSyncRoot;

	public Texture2D CurrentRenderTarget
	{
		get
		{
			if (currentRenderTargets != null && currentRenderTargets.Length >= 1)
			{
				return currentRenderTargets[0].RenderTarget as Texture2D;
			}
			return null;
		}
	}

	public Vector2 PixelSize
	{
		get
		{
			Texture2D currentRenderTarget = CurrentRenderTarget;
			if (currentRenderTarget != null)
			{
				return new Vector2(1f / (float)currentRenderTarget.Width, 1f / (float)currentRenderTarget.Height);
			}
			return new Vector2(1f / (float)BackBufferWidth, 1f / (float)BackBufferHeight);
		}
	}

	public static Viewport DefaultViewport => defaultViewport;

	public bool Finished => finished;

	public uint ElapsedFrames => elapsedFrames;

	TimeRuler IEngine.TimeRuler
	{
		get
		{
			return timeRuler;
		}
		set
		{
			timeRuler = value;
		}
	}

	public bool IsActive => isActive;

	private event EngineDisposeHandler OnDispose;

	public event Action OnDeviceReset;

	public static void RegisterDisposeHandler(EngineDisposeHandler handler)
	{
		Instance.OnDispose += handler;
	}

	public static void UnregisterDisposeHandler(EngineDisposeHandler handler)
	{
		Instance.OnDispose -= handler;
	}

	public static void SetGUIWidth(int value)
	{
		guiWidth = value;
	}

	private void UpdateViewport()
	{
		defaultViewport = default(Viewport);
		Texture texture = null;
		if (currentRenderTargets != null && currentRenderTargets.Length >= 1)
		{
			texture = currentRenderTargets[0].RenderTarget;
		}
		if (texture is Texture2D texture2D)
		{
			defaultViewport.Height = texture2D.Height;
			defaultViewport.Width = texture2D.Width;
		}
		else if (texture is TextureCube textureCube)
		{
			int height = (defaultViewport.Width = textureCube.Size);
			defaultViewport.Height = height;
		}
		else
		{
			defaultViewport.Height = BackBufferHeight;
			defaultViewport.Width = BackBufferWidth;
		}
		defaultViewport.X = 0;
		defaultViewport.Y = 0;
		defaultViewport.MinDepth = 0f;
		defaultViewport.MaxDepth = 1f;
	}

	public Engine()
	{
		if (instance != null)
		{
			throw new Exception("An instance of Barrage Engine already exists");
		}
		instance = this;
		mainThread = Thread.CurrentThread;
	}

	void IEngine.InitGame()
	{
		game.InitGame();
	}

	void IEngine.Initialize(GraphicsDevice device)
	{
		this.device = device;
		wideScreen = device.Adapter.IsWideScreen;
		device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PlatformContents;
		device.DeviceReset += device_DeviceReset;
		alphaBlendState = new BlendState();
		alphaBlendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
		alphaBlendState.ColorSourceBlend = Blend.SourceAlpha;
		alphaBlendState.AlphaSourceBlend = Blend.SourceAlpha;
		alphaBlendState.AlphaDestinationBlend = Blend.One;
		currentRenderTargets = device.GetRenderTargets();
		UpdateViewport();
	}

	private void device_DeviceReset(object sender, EventArgs e)
	{
		if (OnDeviceReset != null)
		{
			OnDeviceReset();
		}
	}

	void IEngine.SetGame(Game game)
	{
		this.game = game;
	}

	void IEngine.SetContent(ContentTracker tracker)
	{
		contentTracker = tracker;
	}

	void IEngine.Update()
	{
		SingleLoop();
	}

	void IEngine.Draw()
	{
		Draw();
	}

	void IEngine.SetActive(bool active)
	{
		isActive = active;
	}

	public void SetDeviceRenderTargets(RenderTargetBinding[] rt)
	{
		currentRenderTargets = rt;
		UpdateViewport();
		Device.SetRenderTargets(rt);
	}

	public RenderTargetBinding[] GetDeviceRenderTargets()
	{
		return currentRenderTargets;
	}

	public void SetDeviceRenderTarget(RenderTarget2D rt)
	{
		RenderTargetBinding renderTargetBinding = ((rt != null) ? new RenderTargetBinding(rt) : default(RenderTargetBinding));
		singleRenderTarget[0] = renderTargetBinding;
		Device.SetRenderTarget(rt);
		currentRenderTargets = singleRenderTarget;
		UpdateViewport();
	}

	public void SetDeviceRenderTarget(RenderTargetCube rt, CubeMapFace face)
	{
		ref RenderTargetBinding reference = ref singleRenderTarget[0];
		reference = ((rt != null) ? new RenderTargetBinding(rt, face) : default(RenderTargetBinding));
		Device.SetRenderTarget(rt, face);
		currentRenderTargets = singleRenderTarget;
		UpdateViewport();
	}

	private void Draw()
	{
		CompatHooks.DrawEngineSafely(this);
	}

	public static string ProcessPath(string currentPath, string filePath)
	{
		if (filePath.Length > 0 && filePath[0] == '/')
		{
			return filePath.Substring(1);
		}
		return currentPath + filePath;
	}

	private void SingleLoop()
	{
		try
		{
			Keyboard.Instance.UpdateStatus();
			Gamepad.UpdateStatus();
			Timer.DefaultTimer.Clock();
			InputManager.Instance.Update();
			game.MainLoop();
			elapsedFrames++;
		}
		catch (Exception ex)
		{
			game.ErrorFound(ex);
		}
	}

	~Engine()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (game != null)
		{
			game.Dispose();
		}
		game = null;
		GC.Collect();
		GC.WaitForPendingFinalizers();
		if (OnDispose != null)
		{
			OnDispose();
		}
		OnDispose = null;
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
		GC.WaitForPendingFinalizers();
		device.Dispose();
		device = null;
		GC.SuppressFinalize(this);
	}
}
