#define TRACE
using System;
using Maximinus.DebugTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class MaximinusGame : Game
{
	public enum ID
	{
		GlobeClicker,
		MissileEscape,
		Basket,
		Billard,
		Billard9Ball,
		Space,
		StockCar,
		Boulder,
		FunkyPool
	}

	public enum BackBufferSizeValue
	{
		HD_720,
		HD_1080
	}

	private static ID id;

	private static BackBufferSizeValue backBufferSize;

	private static int currentFrame = 0;

	private MultiMonitorGraphicsDeviceManager Graphics;

	public static ContentManager ContentManager;

	public static Texture2D splashTex;

	private Threading.ManagedThread threadLoadContent;

	public static Drawing2D Draw2D;

	private bool hasAlternateFontTitle;

	private Threading.ThreadTaskDelegate LoadContentCB;

	public static bool DebugFPS;

	public static bool DebugTimeRuler;

	public static bool DebugTimeRulerLog;

	public static bool DebugIncludeInTimeRuler;

	public static bool DebugLoaded = false;

	public static bool ContentLoadFinished = false;

	public static string ContentLoadString = "";

	public static readonly string PathFont = "Fonts/";

	public static readonly string PathTextures = "Textures/";

	public Camera3D Camera;

	public static MaximinusGame Instance;

	public Camera3D.Setup CameraSetup;

	public static GameTime gameTime;

	public static ID Id => id;

	public static BackBufferSizeValue BackBufferSize => backBufferSize;

	public static bool IsFullHD => backBufferSize == BackBufferSizeValue.HD_1080;

	public static string TexSizeName => "-" + (IsFullHD ? "fullhd" : "hd");

	public static int CurrentFrame => currentFrame;

	private static bool DebugInfo
	{
		get
		{
			if (!DebugFPS)
			{
				return DebugTimeRuler;
			}
			return true;
		}
	}

	public static void CurrentFrameDebugIncrement()
	{
		currentFrame++;
	}

	protected override void Initialize()
	{
		if (base.GraphicsDevice.Viewport.Width > 1280)
		{
			backBufferSize = BackBufferSizeValue.HD_1080;
		}
		else
		{
			backBufferSize = BackBufferSizeValue.HD_720;
		}
		Draw2D = new Drawing2D(base.GraphicsDevice, ContentManager, new SpriteBatch(base.GraphicsDevice), base.Content.Load<SpriteFont>(PathFont + "font720"), (backBufferSize == BackBufferSizeValue.HD_720) ? null : base.Content.Load<SpriteFont>(PathFont + "font1080"), (!hasAlternateFontTitle) ? null : base.Content.Load<SpriteFont>(PathFont + "fontTitle720"), (!hasAlternateFontTitle || backBufferSize == BackBufferSizeValue.HD_720) ? null : base.Content.Load<SpriteFont>(PathFont + "fontTitle1080"));
		Draw2D.SpriteBatch.Name = id.ToString();
		Camera = (CameraSetup.Enabled ? new Camera3D(CameraSetup) : null);
		if (DebugInfo)
		{
			DebugSystem.Initialize(this, Draw2D.Font, Draw2D.SpriteBatch);
			DebugLoaded = true;
			DebugSystem.Instance.FpsCounter.Visible = DebugFPS;
			DebugSystem.Instance.TimeRuler.Visible = DebugTimeRuler;
			DebugSystem.Instance.TimeRuler.ShowLog = DebugTimeRulerLog;
		}
		base.Initialize();
	}

	protected override void LoadContent()
	{
		splashTex = base.Content.Load<Texture2D>(PathTextures + "maximinus");
		if (LoadContentCB != null)
		{
			threadLoadContent = new Threading.ManagedThread();
			threadLoadContent.AddTask(new Threading.ThreadTask(LoadContentCB));
		}
		base.LoadContent();
	}

	public MaximinusGame(ID currentId)
	{
		id = currentId;
		ContentManager = base.Content;
		if (id == ID.Billard || id == ID.Billard9Ball || id == ID.FunkyPool)
		{
			hasAlternateFontTitle = true;
		}
		CameraSetup = new Camera3D.Setup(enabled: false);
	}

	public MaximinusGame(ID currentId, int preferredW, string gameName, Threading.ThreadTaskDelegate LoadContentCB, float aspectRatio, bool antiAliasing, bool hasAlternateFontTitle, bool fullScreen, bool debugFPS, bool debugTimeRuler, bool debugTimeRulerLog, bool debugIncludeInTimeRuler, Camera3D.Setup CameraSetup)
	{
		id = currentId;
		ContentManager = base.Content;
		base.Content.RootDirectory = "Content";
		Graphics = new MultiMonitorGraphicsDeviceManager(this, 1);
		Utils.InitializeGraphics.InitializeDevice(Graphics, preferredW, gameName, aspectRatio, antiAliasing, fullScreen);
		this.hasAlternateFontTitle = hasAlternateFontTitle;
		this.LoadContentCB = LoadContentCB;
		DebugFPS = debugFPS;
		DebugTimeRuler = debugTimeRuler;
		DebugTimeRulerLog = debugTimeRulerLog;
		DebugIncludeInTimeRuler = debugIncludeInTimeRuler;
		this.CameraSetup = CameraSetup;
		Instance = this;
		base.Components.Add(new GamerServicesComponent(this));
	}

	protected void StartOfFrame(GameTime gameTime)
	{
		if (ContentLoadFinished)
		{
			ObjUpdate.StartOfFrameAll(gameTime);
		}
	}

	protected override void Update(GameTime gameTimeValue)
	{
		gameTime = gameTimeValue;
		if (DebugLoaded)
		{
			DebugSystem.Instance.TimeRuler.StartFrame();
		}
		if (ContentLoadFinished && threadLoadContent != null)
		{
			threadLoadContent.Kill();
			threadLoadContent = null;
		}
		currentFrame++;
		if (ContentLoadFinished)
		{
			ObjUpdate.UpdateAll(gameTimeValue);
		}
		base.Update(gameTimeValue);
	}

	protected override void Draw(GameTime gameTime)
	{
		if (ContentLoadFinished)
		{
			ObjDrawUpdate.DrawAll(gameTime);
		}
		base.Draw(gameTime);
	}

	protected override void OnExiting(object sender, EventArgs args)
	{
		Threading.ManagedThread.KillAll();
		base.OnExiting(sender, args);
	}

	public static void Debug_TimeRuler_BeginMark(string markerName, Color color)
	{
		Debug_TimeRuler_BeginMark(0, markerName, color);
	}

	public static void Debug_TimeRuler_BeginMark(int barIndex, string markerName, Color color)
	{
		if (DebugLoaded && DebugTimeRuler)
		{
			DebugSystem.Instance.TimeRuler.BeginMark(barIndex, markerName, color);
		}
	}

	public static void Debug_TimeRuler_EndMark(string markerName)
	{
		Debug_TimeRuler_EndMark(0, markerName);
	}

	public static void Debug_TimeRuler_EndMark(int barIndex, string markerName)
	{
		if (DebugLoaded && DebugTimeRuler)
		{
			DebugSystem.Instance.TimeRuler.EndMark(barIndex, markerName);
		}
	}

	public static float PulsingRatio(GameTime gameTime, float durationSeconds)
	{
		return PulsingRatio(gameTime, durationSeconds, 0.0);
	}

	public static float PulsingRatio(GameTime gameTime, float durationSeconds, double startTime)
	{
		float num = (float)((gameTime.TotalGameTime.TotalSeconds - startTime) / (double)durationSeconds);
		num -= (float)(int)num;
		if (!(num < 0.5f))
		{
			return 1f - 2f * (num - 0.5f);
		}
		return num * 2f;
	}

	public static float PulsingRatioSmooth(GameTime gameTime, float durationSeconds)
	{
		return MathHelper.SmoothStep(0f, 1f, PulsingRatio(gameTime, durationSeconds));
	}
}
