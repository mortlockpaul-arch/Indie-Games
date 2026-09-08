using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using X;
using p;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Helper renderer that displays the bounding boxes of all
/// rendered scene objects and lights.
///
/// Can help tune performance and work out bugs by seeing how
/// objects and lights within the scene overlap and interact
/// with each other.
/// </summary>
public class DebugRenderManager : IManagerService, IRenderableManager, IManager, IUnloadable
{
	private int _3A_0018 = 100;

	private ISceneState _3AL;

	private IManagerServiceProvider _3A_0019;

	private BasicEffect _3A3;

	private BoundingBoxRenderHelper _3A6;

	private List<SceneEntity> _3AD = new List<SceneEntity>();

	private List<BaseLight> _3A_0017 = new List<BaseLight>();

	[CompilerGenerated]
	private bool _3A_0003;

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public Type ManagerType => typeof(DebugRenderManager);

	/// <summary>
	/// Sets the order this manager is processed relative to other managers
	/// in the IManagerServiceProvider. Managers with lower processing order
	/// values are processed first.
	///
	/// In the case of BeginFrameRendering and EndFrameRendering, BeginFrameRendering
	/// is processed in the normal order (lowest order value to highest), however
	/// EndFrameRendering is processed in reverse order (highest to lowest) to ensure
	/// the first manager begun is the last one ended (FILO).
	/// </summary>
	public int ManagerProcessOrder
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
		}
	}

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => _3A_0019;

	/// <summary>
	/// Determines if debug information should render when the SunBurn editor is open.
	/// </summary>
	public bool RenderInEditor
	{
		[CompilerGenerated]
		get
		{
			return _3A_0003;
		}
		[CompilerGenerated]
		set
		{
			_3A_0003 = value;
		}
	}

	/// <summary>
	/// Creates a new DebugRenderManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public DebugRenderManager(IManagerServiceProvider sceneinterface)
	{
		_3A_0019 = sceneinterface;
		_3A6 = new BoundingBoxRenderHelper();
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public void ApplyPreferences(ISystemPreferences preferences)
	{
	}

	/// <summary>
	/// Sets up the object prior to rendering.
	/// </summary>
	/// <param name="scenestate"></param>
	public void BeginFrameRendering(ISceneState scenestate)
	{
		_3AL = scenestate;
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public void EndFrameRendering()
	{
		if (!RenderInEditor && SunBurnEditor.EditorAttachedStatic)
		{
			return;
		}
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		IObjectManager objectManager = (IObjectManager)_3A_0019.GetManager(SceneInterface.ObjectManagerType, required: false);
		ILightManager lightManager = (ILightManager)_3A_0019.GetManager(SceneInterface.LightManagerType, required: false);
		X.t._6c(graphicsDevice);
		if (_3A3 == null)
		{
			_3A3 = new BasicEffect(graphicsDevice);
			_3A3.FogEnabled = false;
			_3A3.LightingEnabled = false;
			_3A3.PreferPerPixelLighting = false;
			_3A3.TextureEnabled = false;
			_3A3.SpecularColor = Vector3.Zero;
			_3A3.VertexColorEnabled = true;
		}
		_3A3.World = Matrix.Identity;
		_3A3.View = _3AL.View;
		_3A3.Projection = _3AL.Projection;
		if (objectManager != null)
		{
			_3AD.Clear();
			objectManager.Find(_3AD, _3AL.ViewFrustum, ObjectFilter.All);
			foreach (SceneEntity item in _3AD)
			{
				if (item != null && !(item is SceneObject { Visible: false }))
				{
					_3A6.Submit(item.WorldBoundingBox, Color.LimeGreen);
				}
			}
		}
		if (lightManager != null)
		{
			_3A_0017.Clear();
			lightManager.Find(_3A_0017, _3AL.ViewFrustum, ObjectFilter.EnabledDynamicAndStatic);
			foreach (BaseLight item2 in _3A_0017)
			{
				if (item2 != null && item2.Enabled && item2 is IPointSource)
				{
					_3A6.Submit(item2.WorldBoundingBox, Color.Yellow);
				}
			}
		}
		_3A6.Render(_3A3);
		_3A6.Clear();
	}

	/// <summary>
	/// Removes resources managed by this object. Commonly used while clearing the scene.
	/// </summary>
	public void Clear()
	{
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public void Unload()
	{
		Clear();
		p._0018._6_0006(ref _3A3);
	}
}
