using System;
using System.Diagnostics;
using B;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Displays the SunBurn splash screen. Used when the XNA Game object is not available, such as WinForm applications.
/// </summary>
public sealed class SplashScreen
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private const double HCB = 5.0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private const double HC_0002 = 1.0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private const int HC_0012 = 100;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool HCH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool HC7;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool HC_0001;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private double HCw;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int HCZ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private BasicEffect HC_000F;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FullFrameQuad HCy;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Vector2 HC6 = default(Vector2);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Vector2 HCD = default(Vector2);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float HC_0011 = 1f;

	/// <summary>
	/// Used to determine when the SunBurn splash screen is finished displaying
	/// and it's safe to begin game rendering.
	/// </summary>
	[DebuggerHidden]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static bool DisplayComplete
	{
		[DebuggerHidden]
		get
		{
			return HCH;
		}
	}

	/// <summary>
	/// Determines if the splash screen was canceled early by the user.
	/// This can be used to skip later splash screens.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[DebuggerHidden]
	public static bool UserCancelled
	{
		[DebuggerHidden]
		get
		{
			return HC7;
		}
	}

	/// <summary>
	/// Used to enable or disable the SunBurn splash screen during development. Enabling the splash
	/// screen helps when making sure the screen displays properly in released projects.
	/// </summary>
	[DebuggerHidden]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public bool ShowDuringDevelopment
	{
		[DebuggerHidden]
		get
		{
			return HC_0001;
		}
		[DebuggerHidden]
		set
		{
			HC_0001 = value;
		}
	}

	[DebuggerHidden]
	internal static void _7z()
	{
		global::B.B._0012();
		if (HCH)
		{
			return;
		}
		throw new Exception("SunBurn splash screen required for rendering, please display splash screen before calling this method.");
	}

	/// <summary>
	/// Creates a new SplashScreen instance.
	/// </summary>
	[DebuggerHidden]
	public SplashScreen()
	{
	}

	[DebuggerHidden]
	private void B()
	{
		if (HC_000F == null)
		{
			GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
			HC_000F = new BasicEffect(graphicsDevice);
			HC_000F.DiffuseColor = Vector3.Zero;
			HC_000F.AmbientLightColor = Vector3.Zero;
			HC_000F.EmissiveColor = Vector3.Zero;
			HC_000F.LightingEnabled = false;
			HC_000F.FogEnabled = false;
			HC_000F.VertexColorEnabled = false;
			Texture2D texture2D = SunBurnCoreSystem.Instance._0002g();
			HC_000F.TextureEnabled = true;
			HC_000F.Texture = texture2D;
			HC_000F.World = Matrix.Identity;
			HC_000F.View = Matrix.Identity;
			HC_000F.Projection = Matrix.Identity;
			Vector2 screenmin = -Vector2.One;
			Vector2 one = Vector2.One;
			float num = (float)texture2D.Width / (float)texture2D.Height;
			if (graphicsDevice.Viewport.AspectRatio > num)
			{
				float num2 = (float)graphicsDevice.Viewport.Height * num;
				float num3 = num2 / (float)graphicsDevice.Viewport.Width;
				screenmin.X = 0f - num3;
				one.X = num3;
				HC_0011 = num2 / 1280f;
			}
			else
			{
				float num4 = (float)graphicsDevice.Viewport.Width / num;
				float num5 = num4 / (float)graphicsDevice.Viewport.Height;
				screenmin.Y = 0f - num5;
				one.Y = num5;
				HC_0011 = num4 / 720f;
			}
			HCy = new FullFrameQuad(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, screenmin, one);
			HC6 = one * new Vector2(0.2f, 0.75f);
			Vector2 vector = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height) * 0.5f;
			HC6 *= vector;
			HC6.Y += vector.Y;
			HCD = one * new Vector2(0.2f, 0.707f);
			HCD *= vector;
			HCD.Y += vector.Y;
		}
	}

	/// <summary>
	/// Called when graphics resources need to be unloaded.
	/// </summary>
	[DebuggerHidden]
	public void Unload()
	{
		F.B._7_0004(ref HC_000F);
		F.B._7_0004(ref HCy);
	}

	/// <summary>
	/// Called periodically to allow users to click out of the splash screen.
	/// </summary>
	/// <param name="gameTime"></param>
	[DebuggerHidden]
	public void Update(GameTime gameTime)
	{
		if (HCH || !(HCw > 0.0) || HCZ <= 100)
		{
			return;
		}
		double num = gameTime.TotalGameTime.TotalSeconds - HCw;
		if (num > 5.0)
		{
			HCH = true;
		}
		else if (num > 1.0)
		{
			GamePadState state = GamePad.GetState(PlayerIndex.One);
			KeyboardState state2 = Keyboard.GetState();
			if (state.IsConnected && (state.IsButtonDown(Buttons.A) || state.IsButtonDown(Buttons.B)))
			{
				HCH = true;
			}
			else if (state2.IsKeyDown(Keys.Space) || state2.IsKeyDown(Keys.Enter) || state2.IsKeyDown(Keys.Escape))
			{
				HCH = true;
			}
			if (HCH)
			{
				HC7 = true;
			}
		}
	}

	/// <summary>
	/// Renders the SunBurn splash screen (require by the SunBurn license).
	/// </summary>
	/// <param name="gameTime"></param>
	[DebuggerHidden]
	public void Render(GameTime gameTime)
	{
		if (HC_000F == null)
		{
			B();
		}
		if (HCH)
		{
			return;
		}
		if (!HC_0001 && Debugger.IsAttached)
		{
			HCH = true;
			return;
		}
		if (HCw <= 0.0)
		{
			HCw = gameTime.TotalGameTime.TotalSeconds;
		}
		HCZ++;
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		graphicsDevice.RasterizerState = RasterizerState.CullNone;
		graphicsDevice.DepthStencilState = DepthStencilState.None;
		graphicsDevice.BlendState = BlendState.Opaque;
		double num = gameTime.TotalGameTime.TotalSeconds - HCw;
		double num2 = 0.25;
		float num3 = 4f;
		float num4 = MathHelper.Clamp((float)(num - num2) * num3, 0f, 1f);
		float num5 = MathHelper.Clamp((float)(5.0 - (num + num2)) * num3, 0f, 1f);
		float num6 = num4 * num5;
		Vector3 vector = Vector3.One * num6;
		graphicsDevice.Clear(new Color(vector));
		HC_000F.DiffuseColor = vector;
		HCy.Render(HC_000F);
		GamePadState state = GamePad.GetState(PlayerIndex.One);
		if (state.Buttons.X == ButtonState.Pressed && state.Buttons.Y == ButtonState.Pressed)
		{
			SpriteFont spriteFont = SunBurnCoreSystem.Instance._0002n();
			SpriteBatch spriteBatch = SunBurnCoreSystem.Instance._00025();
			Color color = new Color(Vector3.Zero);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			spriteBatch.DrawString(spriteFont, "SunBurn " + SunBurnCoreSystem.Edition + " " + SunBurnCoreSystem.Version, HCD, color, 0f, Vector2.Zero, HC_0011, SpriteEffects.None, 0f);
			spriteBatch.DrawString(spriteFont, "License Id:" + global::B.B.HCH, HC6, color, 0f, Vector2.Zero, HC_0011, SpriteEffects.None, 0f);
			spriteBatch.End();
		}
	}
}
