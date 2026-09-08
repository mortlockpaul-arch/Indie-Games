using System;
using System.Diagnostics;
using _0018;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SynapseGaming.LightingSystem.Core;
using p;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Displays the SunBurn splash screen. Used when the XNA Game object is not available, such as WinForm applications.
/// </summary>
public sealed class SplashScreen
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private const double _3A_0018 = 5.0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private const double _3AL = 1.0;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private const int _3A_0019 = 100;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool _3A3;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static bool _3A6;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool _3AD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private double _3A_0017;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private int _3A_0003;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private BasicEffect _3Al;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private FullFrameQuad _3At;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Vector2 _3AF = default(Vector2);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Vector2 _3Ac = default(Vector2);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private float _3Ag = 1f;

	/// <summary>
	/// Used to determine when the SunBurn splash screen is finished displaying
	/// and it's safe to begin game rendering.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[DebuggerHidden]
	public static bool DisplayComplete
	{
		[DebuggerHidden]
		get
		{
			return _3A3;
		}
	}

	/// <summary>
	/// Determines if the splash screen was canceled early by the user.
	/// This can be used to skip later splash screens.
	/// </summary>
	[DebuggerHidden]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static bool UserCancelled
	{
		[DebuggerHidden]
		get
		{
			return _3A6;
		}
	}

	/// <summary>
	/// Used to enable or disable the SunBurn splash screen during development. Enabling the splash
	/// screen helps when making sure the screen displays properly in released projects.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[DebuggerHidden]
	public bool ShowDuringDevelopment
	{
		[DebuggerHidden]
		get
		{
			return _3AD;
		}
		[DebuggerHidden]
		set
		{
			_3AD = value;
		}
	}

	[DebuggerHidden]
	internal static void _6b()
	{
		global::_0018._0018._0019();
		if (_3A3)
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
	private void _0018()
	{
		if (_3Al == null)
		{
			GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
			_3Al = new BasicEffect(graphicsDevice);
			_3Al.DiffuseColor = Vector3.Zero;
			_3Al.AmbientLightColor = Vector3.Zero;
			_3Al.EmissiveColor = Vector3.Zero;
			_3Al.LightingEnabled = false;
			_3Al.FogEnabled = false;
			_3Al.VertexColorEnabled = false;
			Texture2D texture2D = SunBurnCoreSystem.Instance.Le();
			_3Al.TextureEnabled = true;
			_3Al.Texture = texture2D;
			_3Al.World = Matrix.Identity;
			_3Al.View = Matrix.Identity;
			_3Al.Projection = Matrix.Identity;
			Vector2 screenmin = -Vector2.One;
			Vector2 one = Vector2.One;
			float num = (float)texture2D.Width / (float)texture2D.Height;
			if (graphicsDevice.Viewport.AspectRatio > num)
			{
				float num2 = (float)graphicsDevice.Viewport.Height * num;
				float num3 = num2 / (float)graphicsDevice.Viewport.Width;
				screenmin.X = 0f - num3;
				one.X = num3;
				_3Ag = num2 / 1280f;
			}
			else
			{
				float num4 = (float)graphicsDevice.Viewport.Width / num;
				float num5 = num4 / (float)graphicsDevice.Viewport.Height;
				screenmin.Y = 0f - num5;
				one.Y = num5;
				_3Ag = num4 / 720f;
			}
			_3At = new FullFrameQuad(graphicsDevice, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, screenmin, one);
			_3AF = one * new Vector2(0.2f, 0.75f);
			Vector2 vector = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height) * 0.5f;
			_3AF *= vector;
			_3AF.Y += vector.Y;
			_3Ac = one * new Vector2(0.2f, 0.707f);
			_3Ac *= vector;
			_3Ac.Y += vector.Y;
		}
	}

	/// <summary>
	/// Called when graphics resources need to be unloaded.
	/// </summary>
	[DebuggerHidden]
	public void Unload()
	{
		p._0018._6_0006(ref _3Al);
		p._0018._6_0006(ref _3At);
	}

	/// <summary>
	/// Called periodically to allow users to click out of the splash screen.
	/// </summary>
	/// <param name="gameTime"></param>
	[DebuggerHidden]
	public void Update(GameTime gameTime)
	{
		if (_3A3 || !(_3A_0017 > 0.0) || _3A_0003 <= 100)
		{
			return;
		}
		double num = gameTime.TotalGameTime.TotalSeconds - _3A_0017;
		if (num > 5.0)
		{
			_3A3 = true;
		}
		else if (num > 1.0)
		{
			GamePadState state = GamePad.GetState(PlayerIndex.One);
			KeyboardState state2 = Keyboard.GetState();
			if (state.IsConnected && (state.IsButtonDown(Buttons.A) || state.IsButtonDown(Buttons.B)))
			{
				_3A3 = true;
			}
			else if (state2.IsKeyDown(Keys.Space) || state2.IsKeyDown(Keys.Enter) || state2.IsKeyDown(Keys.Escape))
			{
				_3A3 = true;
			}
			if (_3A3)
			{
				_3A6 = true;
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
		if (_3Al == null)
		{
			_0018();
		}
		if (_3A3)
		{
			return;
		}
		if (!_3AD && Debugger.IsAttached)
		{
			_3A3 = true;
			return;
		}
		if (_3A_0017 <= 0.0)
		{
			_3A_0017 = gameTime.TotalGameTime.TotalSeconds;
		}
		_3A_0003++;
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		graphicsDevice.RasterizerState = RasterizerState.CullNone;
		graphicsDevice.DepthStencilState = DepthStencilState.None;
		graphicsDevice.BlendState = BlendState.Opaque;
		double num = gameTime.TotalGameTime.TotalSeconds - _3A_0017;
		double num2 = 0.25;
		float num3 = 4f;
		float num4 = MathHelper.Clamp((float)(num - num2) * num3, 0f, 1f);
		float num5 = MathHelper.Clamp((float)(5.0 - (num + num2)) * num3, 0f, 1f);
		float num6 = num4 * num5;
		Vector3 vector = Vector3.One * num6;
		graphicsDevice.Clear(new Color(vector));
		_3Al.DiffuseColor = vector;
		_3At.Render(_3Al);
		GamePadState state = GamePad.GetState(PlayerIndex.One);
		if (state.Buttons.X == ButtonState.Pressed && state.Buttons.Y == ButtonState.Pressed)
		{
			SpriteFont spriteFont = SunBurnCoreSystem.Instance.LS();
			SpriteBatch spriteBatch = SunBurnCoreSystem.Instance.LQ();
			Color color = new Color(Vector3.Zero);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			spriteBatch.DrawString(spriteFont, "SunBurn " + SunBurnCoreSystem.Edition + " " + SunBurnCoreSystem.Version, _3Ac, color, 0f, Vector2.Zero, _3Ag, SpriteEffects.None, 0f);
			spriteBatch.DrawString(spriteFont, "License Id:" + global::_0018._0018._3A3, _3AF, color, 0f, Vector2.Zero, _3Ag, SpriteEffects.None, 0f);
			spriteBatch.End();
		}
	}
}
