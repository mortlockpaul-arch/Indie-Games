using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Displays the SunBurn splash screen.
/// </summary>
public sealed class SplashScreenGameComponent : DrawableGameComponent
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SplashScreen HCB;

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
			return SplashScreen.DisplayComplete;
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
			return SplashScreen.UserCancelled;
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
			return HCB.ShowDuringDevelopment;
		}
		[DebuggerHidden]
		set
		{
			HCB.ShowDuringDevelopment = value;
		}
	}

	/// <summary>
	/// Creates a new SplashScreenGameComponent instance.
	/// </summary>
	/// <param name="game"></param>
	[DebuggerHidden]
	public SplashScreenGameComponent(Game game)
		: base(game)
	{
		base.DrawOrder = int.MaxValue;
		HCB = new SplashScreen();
	}

	/// <summary>
	/// Called when the DrawOrder property changes. Raises the DrawOrderChanged event.
	/// </summary>
	/// <param name="sender">The DrawableGameComponent.</param>
	/// <param name="args">Arguments to the DrawOrderChanged event.</param>
	[DebuggerHidden]
	protected sealed override void OnDrawOrderChanged(object sender, EventArgs args)
	{
		if (base.DrawOrder != int.MaxValue)
		{
			base.DrawOrder = int.MaxValue;
		}
		base.OnDrawOrderChanged(sender, args);
	}

	/// <summary>
	/// Called when graphics resources need to be unloaded. Override this method to
	/// unload any component-specific graphics resources.
	/// </summary>
	[DebuggerHidden]
	protected sealed override void UnloadContent()
	{
		HCB.Unload();
		base.UnloadContent();
	}

	/// <summary>
	/// Called when the GameComponent needs to be updated. Override this method with
	/// component-specific update code.
	/// </summary>
	/// <param name="gameTime"></param>
	[DebuggerHidden]
	public sealed override void Update(GameTime gameTime)
	{
		HCB.Update(gameTime);
		base.Update(gameTime);
	}

	/// <summary>
	/// Called when the DrawableGameComponent needs to be drawn. Override this method
	/// with component-specific drawing code.
	/// </summary>
	/// <param name="gameTime"></param>
	[DebuggerHidden]
	public sealed override void Draw(GameTime gameTime)
	{
		HCB.Render(gameTime);
		base.Draw(gameTime);
	}
}
