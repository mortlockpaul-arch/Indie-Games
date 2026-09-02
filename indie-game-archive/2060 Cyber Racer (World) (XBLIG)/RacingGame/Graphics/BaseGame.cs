using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.GameLogic;
using RacingGame.GameScreens;
using RacingGame.Helpers;
using RacingGame.Properties;
using RacingGame.Shaders;
using RacingGame.Sounds;

namespace RacingGame.Graphics;

public class BaseGame : Game
{
	public enum AlphaMode
	{
		DisableAlpha,
		Default,
		SourceAlphaOne,
		OneOne
	}

	public delegate void RenderHandler();

	private const float FieldOfView = (float)Math.PI / 2f;

	private const float NearPlane = 0.5f;

	private const float FarPlane = 1750f;

	public const float ViewableFieldOfView = (float)Math.PI * 4f / 9f;

	public const float Epsilon = 1E-06f;

	private static readonly Color BackgroundColor;

	public static PlatformID CurrentPlatform;

	public static GraphicsDeviceManager graphicsManager;

	protected static ContentManager content;

	protected static UIRenderer ui;

	protected static int width;

	protected static int height;

	private static float aspectRatio;

	private static string remWindowsTitle;

	private static LineManager2D lineManager2D;

	private static LineManager3D lineManager3D;

	private static MeshRenderManager meshRenderManager;

	private static Matrix worldMatrix;

	private static Matrix viewMatrix;

	private static Matrix projectionMatrix;

	private static Vector3 lightDirection;

	private static float elapsedTimeThisFrameInMs;

	private static float totalTimeMs;

	private static float lastFrameTotalTimeMs;

	private static float startTimeThisSecond;

	private static int frameCountThisSecond;

	private static int totalFrameCount;

	private static int fpsLastSecond;

	private static GamerServicesComponent gamerServicesComponent;

	private static DepthFormat backBufferDepthFormat;

	private static bool alreadyCheckedGraphicsOptions;

	private static bool highDetail;

	private static bool allowShadowMapping;

	private static bool usePostScreenShaders;

	private static bool mustApplyDeviceChanges;

	private static float fpsInterpolated;

	private static Vector3 camPos;

	private static Vector3 cameraRotation;

	private static Matrix invViewMatrix;

	private static bool isAppActive;

	private int renderLoopErrorCount;

	private static RenderTarget2D remSceneRenderTarget;

	private static RenderTarget2D lastSetRenderTarget;

	private static List<RenderToTexture> remRenderToTextures;

	public static string WindowsTitle => remWindowsTitle;

	public static Vector3 LightDirection
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return lightDirection;
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			lightDirection = value;
			((Vector3)(ref lightDirection)).Normalize();
		}
	}

	public static GamerServicesComponent GamerServicesComponent => gamerServicesComponent;

	public static GraphicsDevice Device => graphicsManager.GraphicsDevice;

	public static DepthFormat BackBufferDepthFormat
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return backBufferDepthFormat;
		}
	}

	public static bool Fullscreen => graphicsManager.IsFullScreen;

	public static bool HighDetail
	{
		get
		{
			if (!alreadyCheckedGraphicsOptions)
			{
				CheckOptionsAndPSVersion();
			}
			return highDetail;
		}
	}

	public static bool AllowShadowMapping
	{
		get
		{
			if (!alreadyCheckedGraphicsOptions)
			{
				CheckOptionsAndPSVersion();
			}
			return allowShadowMapping;
		}
	}

	public static bool UsePostScreenShaders
	{
		get
		{
			if (!alreadyCheckedGraphicsOptions)
			{
				CheckOptionsAndPSVersion();
			}
			return usePostScreenShaders;
		}
	}

	public static ContentManager Content => content;

	public static UIRenderer UI => ui;

	public static MeshRenderManager MeshRenderManager => meshRenderManager;

	public static int Width => width;

	public static int Height => height;

	public static float AspectRatio => aspectRatio;

	public static Rectangle ResolutionRect
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return new Rectangle(0, 0, width, height);
		}
	}

	public static int Fps => fpsLastSecond;

	public static int TotalFrames => totalFrameCount;

	public static float ElapsedTimeThisFrameInMilliseconds => elapsedTimeThisFrameInMs;

	public static float TotalTime => totalTimeMs / 1000f;

	public static float TotalTimeMilliseconds => totalTimeMs;

	public static float MoveFactorPerSecond => elapsedTimeThisFrameInMs / 1000f;

	public static Matrix WorldMatrix
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return worldMatrix;
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			worldMatrix = value;
		}
	}

	public static Matrix ViewMatrix
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return viewMatrix;
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			viewMatrix = value;
			invViewMatrix = Matrix.Invert(viewMatrix);
			camPos = ((Matrix)(ref invViewMatrix)).Translation;
			cameraRotation = Vector3.TransformNormal(new Vector3(0f, 0f, 1f), invViewMatrix);
		}
	}

	public static Matrix ProjectionMatrix
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return projectionMatrix;
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			projectionMatrix = value;
		}
	}

	public static Vector3 CameraPos
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return camPos;
		}
	}

	public static Vector3 CameraRotation
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return cameraRotation;
		}
	}

	public static Matrix InverseViewMatrix
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return invViewMatrix;
		}
	}

	public static Matrix ViewProjectionMatrix
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return ViewMatrix * ProjectionMatrix;
		}
	}

	public static Matrix WorldViewProjectionMatrix
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			return WorldMatrix * ViewMatrix * ProjectionMatrix;
		}
	}

	public static bool IsAppActive => isAppActive;

	public static RenderTarget2D CurrentRenderTarget => lastSetRenderTarget;

	public static bool EveryMillisecond(int checkMilliseconds)
	{
		return (int)(lastFrameTotalTimeMs / (float)checkMilliseconds) != (int)(totalTimeMs / (float)checkMilliseconds);
	}

	internal static void CheckOptionsAndPSVersion()
	{
		GraphicsDevice device = Device;
		if (device == null)
		{
			throw new InvalidOperationException("Device is not created yet!");
		}
		alreadyCheckedGraphicsOptions = true;
		usePostScreenShaders = GameSettings.Default.PostScreenEffects;
		allowShadowMapping = GameSettings.Default.ShadowMapping;
		highDetail = GameSettings.Default.HighDetail;
	}

	internal static void ApplyResolutionChange()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		int num = ((GameSettings.Default != null) ? GameSettings.Default.ResolutionWidth : 0);
		int num2 = ((GameSettings.Default != null) ? GameSettings.Default.ResolutionHeight : 0);
		if (num <= 0 || num2 <= 0)
		{
			DisplayMode currentDisplayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			num = ((DisplayMode)(ref currentDisplayMode)).Width;
			DisplayMode currentDisplayMode2 = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			num2 = ((DisplayMode)(ref currentDisplayMode2)).Height;
		}
		graphicsManager.IsFullScreen = true;
		GraphicsDeviceManager obj = graphicsManager;
		DisplayMode currentDisplayMode3 = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
		obj.PreferredBackBufferWidth = ((DisplayMode)(ref currentDisplayMode3)).Width;
		GraphicsDeviceManager obj2 = graphicsManager;
		DisplayMode currentDisplayMode4 = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
		obj2.PreferredBackBufferHeight = ((DisplayMode)(ref currentDisplayMode4)).Height;
	}

	public static int XToRes(int xIn1024px)
	{
		return (int)Math.Round((float)(xIn1024px * Width) / 1024f);
	}

	public static int YToRes(int yIn640px)
	{
		return (int)Math.Round((float)(yIn640px * Height) / 640f);
	}

	public static int YToRes768(int yIn768px)
	{
		return (int)Math.Round((float)(yIn768px * Height) / 768f);
	}

	public static int XToRes1600(int xIn1600px)
	{
		return (int)Math.Round((float)(xIn1600px * Width) / 1600f);
	}

	public static int YToRes1200(int yIn1200px)
	{
		return (int)Math.Round((float)(yIn1200px * Height) / 1200f);
	}

	public static int XToRes1400(int xIn1400px)
	{
		return (int)Math.Round((float)(xIn1400px * Width) / 1400f);
	}

	public static int YToRes1050(int yIn1050px)
	{
		return (int)Math.Round((float)(yIn1050px * Height) / 1050f);
	}

	public static Rectangle CalcRectangle(int relX, int relY, int relWidth, int relHeight)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)width / 1024f;
		float num2 = (float)height / 640f;
		return new Rectangle((int)Math.Round((float)relX * num), (int)Math.Round((float)relY * num2), (int)Math.Round((float)relWidth * num), (int)Math.Round((float)relHeight * num2));
	}

	public static Rectangle CalcRectangleWithBounce(int relX, int relY, int relWidth, int relHeight, float bounceEffect)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)width / 1024f;
		float num2 = (float)height / 640f;
		float num3 = (float)(relX + relWidth / 2) * num;
		float num4 = (float)(relY + relHeight / 2) * num2;
		float num5 = (float)relWidth * num * bounceEffect;
		float num6 = (float)relHeight * num2 * bounceEffect;
		return new Rectangle((int)Math.Round(num3 - num5 / 2f), (int)Math.Round(num4 - num6 / 2f), (int)Math.Round(num5), (int)Math.Round(num6));
	}

	public static Rectangle CalcRectangleKeep4To3(int relX, int relY, int relWidth, int relHeight)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)width / 1024f;
		float num2 = (float)height / 768f;
		return new Rectangle((int)Math.Round((float)relX * num), (int)Math.Round((float)relY * num2), (int)Math.Round((float)relWidth * num), (int)Math.Round((float)relHeight * num2));
	}

	public static Rectangle CalcRectangleKeep4To3(Rectangle gfxRect)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)width / 1024f;
		float num2 = (float)height / 768f;
		return new Rectangle((int)Math.Round((float)gfxRect.X * num), (int)Math.Round((float)gfxRect.Y * num2), (int)Math.Round((float)gfxRect.Width * num), (int)Math.Round((float)gfxRect.Height * num2));
	}

	public static Rectangle CalcRectangle1600(int relX, int relY, int relWidth, int relHeight)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)width / 1600f;
		float num2 = (float)height / 1200f;
		return new Rectangle((int)Math.Round((float)relX * num), (int)Math.Round((float)relY * num2), (int)Math.Round((float)relWidth * num), (int)Math.Round((float)relHeight * num2));
	}

	public static Rectangle CalcRectangle2000(int relX, int relY, int relWidth, int relHeight)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)width / 2000f;
		float num2 = (float)height / 1500f;
		return new Rectangle((int)Math.Round((float)relX * num), (int)Math.Round((float)relY * num2), (int)Math.Round((float)relWidth * num), (int)Math.Round((float)relHeight * num2));
	}

	public static Rectangle CalcRectangleKeep4To3AlignBottom(int relX, int relY, int relWidth, int relHeight)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)width / 1024f;
		float num2 = (float)height / 640f;
		float num3 = (float)height / 768f;
		return new Rectangle((int)((float)relX * num), (int)((float)relY * num2) - (int)Math.Round((float)relHeight * num3), (int)Math.Round((float)relWidth * num), (int)Math.Round((float)relHeight * num3));
	}

	public static Rectangle CalcRectangleKeep4To3AlignBottomRight(int relX, int relY, int relWidth, int relHeight)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)width / 1024f;
		float num2 = (float)height / 640f;
		float num3 = (float)height / 768f;
		return new Rectangle((int)((float)relX * num) - (int)Math.Round((float)relWidth * num), (int)((float)relY * num2) - (int)Math.Round((float)relHeight * num3), (int)Math.Round((float)relWidth * num), (int)Math.Round((float)relHeight * num3));
	}

	public static Rectangle CalcRectangleCenteredWithGivenHeight(int relX, int relY, int relHeight, Rectangle gfxRect)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)width / 1024f;
		float num2 = (float)height / 640f;
		int num3 = (int)Math.Round((float)relHeight * num2);
		int num4 = (int)Math.Round((float)(gfxRect.Width * num3) / (float)gfxRect.Height);
		return new Rectangle(Math.Max(0, (int)Math.Round((float)relX * num) - num4 / 2), Math.Max(0, (int)Math.Round((float)relY * num2) - num3 / 2), num4, num3);
	}

	public static void SetAlphaBlendingEnabled(bool value)
	{
		if (value)
		{
			Device.RenderState.AlphaBlendEnable = true;
			Device.RenderState.SourceBlend = (Blend)5;
			Device.RenderState.DestinationBlend = (Blend)6;
		}
		else
		{
			Device.RenderState.AlphaBlendEnable = false;
		}
	}

	public static void SetCurrentAlphaMode(AlphaMode value)
	{
		switch (value)
		{
		case AlphaMode.DisableAlpha:
			Device.RenderState.SourceBlend = (Blend)1;
			Device.RenderState.DestinationBlend = (Blend)2;
			break;
		case AlphaMode.Default:
			Device.RenderState.SourceBlend = (Blend)5;
			Device.RenderState.DestinationBlend = (Blend)6;
			break;
		case AlphaMode.SourceAlphaOne:
			Device.RenderState.SourceBlend = (Blend)5;
			Device.RenderState.DestinationBlend = (Blend)2;
			break;
		case AlphaMode.OneOne:
			Device.RenderState.SourceBlend = (Blend)2;
			Device.RenderState.DestinationBlend = (Blend)2;
			break;
		}
	}

	protected BaseGame(string setWindowsTitle)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		((Game)this)._002Ector();
		gamerServicesComponent = new GamerServicesComponent((Game)(object)this);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)gamerServicesComponent);
		graphicsManager = new GraphicsDeviceManager((Game)(object)this);
		graphicsManager.MinimumPixelShaderProfile = (ShaderProfile)4;
		graphicsManager.MinimumVertexShaderProfile = (ShaderProfile)11;
		ApplyResolutionChange();
		graphicsManager.PreparingDeviceSettings += graphics_PrepareDevice;
		((Game)this).IsFixedTimeStep = false;
		content = ((Game)this).Content;
		((Game)this).Content.RootDirectory = string.Empty;
		((Game)this).Window.Title = setWindowsTitle;
		remWindowsTitle = setWindowsTitle;
		Sound.Initialize();
	}

	protected BaseGame()
		: this("Game")
	{
	}

	private void graphics_PrepareDevice(object sender, PreparingDeviceSettingsEventArgs e)
	{
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			PresentationParameters presentationParameters = e.GraphicsDeviceInformation.PresentationParameters;
			presentationParameters.RenderTargetUsage = (RenderTargetUsage)2;
			if (graphicsManager.PreferredBackBufferHeight == 720)
			{
				presentationParameters.MultiSampleType = (MultiSampleType)4;
				presentationParameters.PresentationInterval = (PresentInterval)1;
			}
			else
			{
				presentationParameters.MultiSampleType = (MultiSampleType)2;
				presentationParameters.PresentationInterval = (PresentInterval)2;
			}
		}
	}

	protected override void Initialize()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		((Game)this).Initialize();
		GameSettings.Initialize();
		ApplyResolutionChange();
		Sound.SetVolumes(GameSettings.Default.SoundVolume, GameSettings.Default.MusicVolume);
		Highscores.Initialize();
		Log.Initialize();
		backBufferDepthFormat = graphicsManager.PreferredDepthStencilFormat;
		graphicsManager.DeviceReset += graphics_DeviceReset;
		graphics_DeviceReset(null, EventArgs.Empty);
		WorldMatrix = Matrix.Identity;
		ViewMatrix = Matrix.CreateLookAt(new Vector3(0f, 0f, 250f), Vector3.Zero, Vector3.Up);
		lineManager2D = new LineManager2D();
		lineManager3D = new LineManager3D();
		ui = new UIRenderer();
	}

	private void graphics_DeviceReset(object sender, EventArgs e)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Viewport viewport = Device.Viewport;
		width = ((Viewport)(ref viewport)).Width;
		Viewport viewport2 = Device.Viewport;
		height = ((Viewport)(ref viewport2)).Height;
		aspectRatio = (float)width / (float)height;
		ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 2f, aspectRatio, 0.5f, 1750f);
		Device.RenderState.DepthBufferEnable = true;
		Device.RenderState.DepthBufferWriteEnable = true;
		Device.SamplerStates[0].AddressU = (TextureAddressMode)1;
		Device.SamplerStates[0].AddressV = (TextureAddressMode)1;
		SetCurrentAlphaMode(AlphaMode.Default);
		Device.RenderState.ReferenceAlpha = 128;
		Device.RenderState.AlphaFunction = (CompareFunction)5;
		foreach (RenderToTexture remRenderToTexture in remRenderToTextures)
		{
			remRenderToTexture.HandleDeviceReset();
		}
	}

	public static Point Convert3DPointTo2D(Vector3 point)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		Vector4 val = Vector4.Transform(point, ViewProjectionMatrix);
		if (val.W == 0f)
		{
			val.W = 1E-06f;
		}
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(val.X / val.W, val.Y / val.W, val.Z / val.W);
		return new Point((int)Math.Round(val2.X * (float)(width / 2)) + width / 2, (int)Math.Round((0f - val2.Y) * (float)(height / 2)) + height / 2);
	}

	public static bool IsInFrontOfCamera(Vector3 point)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector4 val = Vector4.Transform(new Vector4(point.X, point.Y, point.Z, 1f), ViewProjectionMatrix);
		return val.Z > val.W - 0.5f;
	}

	public static bool IsVisible(Vector3 point, float checkOffset)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector4 val = Vector4.Transform(new Vector4(point.X, point.Y, point.Z, 1f), ViewProjectionMatrix);
		if (val.Z > val.W - 0.5f)
		{
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))._002Ector(val.X / val.W, val.Y / val.W);
			float num = Math.Abs(val.Z);
			if (num < 5f)
			{
				return true;
			}
			checkOffset = 1f + checkOffset / num;
			if (val2.X >= 0f - checkOffset && val2.X <= checkOffset && val2.Y >= 0f - checkOffset)
			{
				return val2.Y <= checkOffset;
			}
			return false;
		}
		return false;
	}

	public static void DrawLine(Point startPoint, Point endPoint, Color color)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		lineManager2D.AddLine(startPoint, endPoint, color);
	}

	public static void DrawLine(Point startPoint, Point endPoint)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		lineManager2D.AddLine(startPoint, endPoint, Color.White);
	}

	public static void DrawLine(Vector3 startPos, Vector3 endPos, Color color)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		lineManager3D.AddLine(startPos, endPos, color);
	}

	public static void DrawLine(Vector3 startPos, Vector3 endPos, Color startColor, Color endColor)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		lineManager3D.AddLine(startPos, startColor, endPos, endColor);
	}

	public static void DrawLine(Vector3 startPos, Vector3 endPos)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		lineManager3D.AddLine(startPos, endPos, Color.White);
	}

	public static void FlushLineManager2D()
	{
		lineManager2D.Render();
	}

	public static void FlushLineManager3D()
	{
		lineManager3D.Render();
	}

	protected override void Update(GameTime gameTime)
	{
		((Game)this).Update(gameTime);
		Input.Update();
		lastFrameTotalTimeMs = totalTimeMs;
		elapsedTimeThisFrameInMs = (float)gameTime.ElapsedRealTime.TotalMilliseconds;
		totalTimeMs += elapsedTimeThisFrameInMs;
		if (elapsedTimeThisFrameInMs <= 0f)
		{
			elapsedTimeThisFrameInMs = 0.001f;
		}
		frameCountThisSecond++;
		totalFrameCount++;
		if (totalTimeMs - startTimeThisSecond > 1000f)
		{
			fpsLastSecond = (int)((float)frameCountThisSecond * 1000f / (totalTimeMs - startTimeThisSecond));
			startTimeThisSecond = totalTimeMs;
			frameCountThisSecond = 0;
			fpsInterpolated = MathHelper.Lerp(fpsInterpolated, (float)fpsLastSecond, 0.1f);
			if (fpsInterpolated < 5f)
			{
				Model.MaxViewDistance = 50;
			}
			else if (fpsInterpolated < 12f)
			{
				Model.MaxViewDistance = 70;
			}
			else if (fpsInterpolated < 16f)
			{
				Model.MaxViewDistance = 90;
			}
			else if (fpsInterpolated < 20f)
			{
				Model.MaxViewDistance = 120;
			}
			else if (fpsInterpolated < 25f)
			{
				Model.MaxViewDistance = 150;
			}
			else if (fpsInterpolated < 30f || !HighDetail)
			{
				Model.MaxViewDistance = 175;
			}
		}
		Sound.Update();
	}

	protected override void OnActivated(object sender, EventArgs args)
	{
		((Game)this).OnActivated(sender, args);
		isAppActive = true;
	}

	protected override void OnDeactivated(object sender, EventArgs args)
	{
		((Game)this).OnDeactivated(sender, args);
		isAppActive = false;
	}

	protected override void Draw(GameTime gameTime)
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ClearBackground();
			Texture.additiveSprite.Begin((SpriteBlendMode)2);
			Texture.alphaSprite.Begin((SpriteBlendMode)1);
			Render();
			meshRenderManager.Render();
			lineManager3D.Render();
			UIRenderer.Render(lineManager2D);
			PostUIRender();
			if (RacingGameManager.InGame && RacingGameManager.Player.Victory)
			{
				Texture.alphaSprite.Begin((SpriteBlendMode)1);
				int rankFromCurrentTime = Highscores.GetRankFromCurrentTime(RacingGameManager.Player.LevelNum, (int)RacingGameManager.Player.BestTimeMilliseconds);
				UI.GetTrophyTexture(rankFromCurrentTime switch
				{
					1 => UIRenderer.TrophyType.Silver, 
					0 => UIRenderer.TrophyType.Gold, 
					_ => UIRenderer.TrophyType.Bronze, 
				}).RenderOnScreen(new Rectangle(Width / 2 - Width / 8, Height / 2 - YToRes(10), Width / 4, Height * 2 / 5));
				Texture.alphaSprite.End();
			}
			ui.RenderTextsAndMouseCursor();
		}
		catch (Exception ex)
		{
			Log.Write("Render loop error: " + ex.ToString());
			if (renderLoopErrorCount++ > 100)
			{
				throw;
			}
		}
		((Game)this).Draw(gameTime);
		if (mustApplyDeviceChanges)
		{
			graphicsManager.ApplyChanges();
			mustApplyDeviceChanges = false;
		}
	}

	protected virtual void Render()
	{
	}

	protected virtual void PostUIRender()
	{
	}

	public static void ClearBackground()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Device.Clear((ClearOptions)3, BackgroundColor, 1f, 0);
	}

	public static void AddRemRenderToTexture(RenderToTexture renderToTexture)
	{
		remRenderToTextures.Add(renderToTexture);
	}

	internal static void SetRenderTarget(RenderTarget2D renderTarget, bool isSceneRenderTarget)
	{
		Device.SetRenderTarget(0, renderTarget);
		if (isSceneRenderTarget)
		{
			remSceneRenderTarget = renderTarget;
		}
		lastSetRenderTarget = renderTarget;
	}

	internal static void ResetRenderTarget(bool fullResetToBackBuffer)
	{
		if (remSceneRenderTarget == null || fullResetToBackBuffer)
		{
			remSceneRenderTarget = null;
			lastSetRenderTarget = null;
			Device.SetRenderTarget(0, (RenderTarget2D)null);
		}
		else
		{
			Device.SetRenderTarget(0, remSceneRenderTarget);
			lastSetRenderTarget = remSceneRenderTarget;
		}
	}

	static BaseGame()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		BackgroundColor = Color.Black;
		CurrentPlatform = Environment.OSVersion.Platform;
		graphicsManager = null;
		content = null;
		ui = null;
		aspectRatio = 1f;
		remWindowsTitle = "";
		lineManager2D = null;
		lineManager3D = null;
		meshRenderManager = new MeshRenderManager();
		lightDirection = new Vector3(0f, 0f, 1f);
		elapsedTimeThisFrameInMs = 0.001f;
		totalTimeMs = 0f;
		lastFrameTotalTimeMs = 0f;
		startTimeThisSecond = 0f;
		frameCountThisSecond = 0;
		totalFrameCount = 0;
		fpsLastSecond = 60;
		gamerServicesComponent = null;
		backBufferDepthFormat = (DepthFormat)52;
		alreadyCheckedGraphicsOptions = false;
		highDetail = true;
		allowShadowMapping = true;
		usePostScreenShaders = true;
		mustApplyDeviceChanges = false;
		fpsInterpolated = 100f;
		cameraRotation = new Vector3(0f, 0f, 1f);
		isAppActive = true;
		remSceneRenderTarget = null;
		lastSetRenderTarget = null;
		remRenderToTextures = new List<RenderToTexture>();
	}
}
