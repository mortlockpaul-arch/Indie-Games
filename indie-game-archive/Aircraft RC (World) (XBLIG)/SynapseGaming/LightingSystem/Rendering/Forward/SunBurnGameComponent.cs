using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Rendering.Forward;

/// <summary>
/// Provides a self-contained SunBurn rendering environment. For quickly adding
/// SunBurn to a project with minimal changes and nearly pure XNA code interaction.
/// </summary>
public class SunBurnGameComponent : DrawableGameComponent
{
	private Matrix HCB = Matrix.Identity;

	private Matrix HC_0002 = Matrix.Identity;

	private Matrix HC_0012 = Matrix.Identity;

	private SceneState HCH = new SceneState();

	private SceneEnvironment HC7 = new SceneEnvironment();

	private SystemPreferences HC_0001 = new SystemPreferences();

	private SunBurnCoreSystem HCw;

	private SceneInterface HCZ;

	private FrameBuffers HC_000F;

	/// <summary>
	/// Rendering environment's SceneInterface. Use to add scene objects and lights for rendering.
	/// </summary>
	public SceneInterface SceneInterface => HCZ;

	/// <summary>
	/// Rendering environment's shared buffers.
	/// </summary>
	public FrameBuffers FrameBuffers => HC_000F;

	/// <summary>
	/// The scene's current view matrix.
	/// </summary>
	public Matrix View
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// The scene's current projection matrix.
	/// </summary>
	public Matrix Projection
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// The scene's current environment such as fog, viewing distance, and HDR information.
	/// </summary>
	public SceneEnvironment Environment
	{
		get
		{
			return HC7;
		}
		set
		{
			HC7 = value;
		}
	}

	/// <summary>
	/// Creates a new SunBurnGameComponent instance.
	/// </summary>
	/// <param name="game"></param>
	/// <param name="managerwithactivationfile">Content manager that contains the SunBurn activation file.</param>
	/// <param name="renderingsystemtype">Determines if the component should use deferred or forward rendering.</param>
	public SunBurnGameComponent(Game game, ContentManager managerwithactivationfile, RenderingSystemType renderingsystemtype)
		: base(game)
	{
		HCw = new SunBurnCoreSystem(base.Game.Services, managerwithactivationfile);
		HCZ = new SceneInterface();
		HCZ.CreateDefaultManagers(renderingsystemtype, includeautoloadedplugins: true);
		HC_000F = new FrameBuffers(DetailPreference.Medium, DetailPreference.Medium);
		HCZ.ResourceManager.AssignOwnership(HC_000F);
		HCZ.ApplyPreferences(HC_0001);
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public void ApplyPreferences(ISystemPreferences preferences)
	{
		HCZ.ApplyPreferences(preferences);
	}

	/// <summary>
	/// Removes resources managed by this object. Commonly used while clearing the scene.
	/// </summary>
	public void Clear()
	{
		HCZ.Clear();
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public void Unload()
	{
		HCZ.Unload();
		HCw.Unload();
	}

	/// <summary>
	/// Called when graphics resources need to be unloaded. Override this method
	/// to unload any component-specific graphics resources.
	/// </summary>
	protected override void UnloadContent()
	{
		Unload();
		base.UnloadContent();
	}

	/// <summary>
	/// Releases the unmanaged resources used by the DrawableGameComponent and
	/// optionally releases the managed resources.
	/// </summary>
	/// <param name="disposing"></param>
	protected override void Dispose(bool disposing)
	{
		Unload();
		base.Dispose(disposing);
	}

	/// <summary>
	/// Called when the GameComponent needs to be updated. Override this
	/// method with component-specific update code.
	/// </summary>
	/// <param name="gameTime"></param>
	public override void Update(GameTime gameTime)
	{
		HCZ.Update(gameTime);
		base.Update(gameTime);
	}

	/// <summary>
	/// Called when the DrawableGameComponent needs to be drawn. Override
	/// this method with component-specific drawing code.
	/// </summary>
	/// <param name="gameTime"></param>
	public override void Draw(GameTime gameTime)
	{
		if (SplashScreenGameComponent.DisplayComplete)
		{
			HCH.BeginFrameRendering(HC_0002, HC_0012, gameTime, HC7, HC_000F, renderingtoscreen: true);
			HCZ.BeginFrameRendering(HCH);
			HCZ.RenderManager.Render();
			HCZ.EndFrameRendering();
			HCH.EndFrameRendering();
		}
		base.Draw(gameTime);
	}
}
