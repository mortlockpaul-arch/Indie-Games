using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using u;

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
	private int HCB = 100;

	private ISceneState HC_0002;

	private IManagerServiceProvider HC_0012;

	private BasicEffect HCH;

	private BoundingBoxRenderHelper HC7;

	private List<SceneEntity> HC_0001 = new List<SceneEntity>();

	private List<BaseLight> HCw = new List<BaseLight>();

	[CompilerGenerated]
	private bool HCZ;

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
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => HC_0012;

	/// <summary>
	/// Determines if debug information should render when the SunBurn editor is open.
	/// </summary>
	public bool RenderInEditor
	{
		[CompilerGenerated]
		get
		{
			return HCZ;
		}
		[CompilerGenerated]
		set
		{
			HCZ = value;
		}
	}

	/// <summary>
	/// Creates a new DebugRenderManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public DebugRenderManager(IManagerServiceProvider sceneinterface)
	{
		HC_0012 = sceneinterface;
		HC7 = new BoundingBoxRenderHelper();
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
		HC_0002 = scenestate;
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
		IObjectManager objectManager = (IObjectManager)HC_0012.GetManager(SceneInterface.ObjectManagerType, required: false);
		ILightManager lightManager = (ILightManager)HC_0012.GetManager(SceneInterface.LightManagerType, required: false);
		u.y._7D(graphicsDevice);
		if (HCH == null)
		{
			HCH = new BasicEffect(graphicsDevice);
			HCH.FogEnabled = false;
			HCH.LightingEnabled = false;
			HCH.PreferPerPixelLighting = false;
			HCH.TextureEnabled = false;
			HCH.SpecularColor = Vector3.Zero;
			HCH.VertexColorEnabled = true;
		}
		HCH.World = Matrix.Identity;
		HCH.View = HC_0002.View;
		HCH.Projection = HC_0002.Projection;
		if (objectManager != null)
		{
			HC_0001.Clear();
			objectManager.Find(HC_0001, HC_0002.ViewFrustum, ObjectFilter.All);
			foreach (SceneEntity item in HC_0001)
			{
				if (item != null && !(item is SceneObject { Visible: false }))
				{
					HC7.Submit(item.WorldBoundingBox, Color.LimeGreen);
				}
			}
		}
		if (lightManager != null)
		{
			HCw.Clear();
			lightManager.Find(HCw, HC_0002.ViewFrustum, ObjectFilter.EnabledDynamicAndStatic);
			foreach (BaseLight item2 in HCw)
			{
				if (item2 != null && item2.Enabled && item2 is IPointSource)
				{
					HC7.Submit(item2.WorldBoundingBox, Color.Yellow);
				}
			}
		}
		HC7.Render(HCH);
		HC7.Clear();
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
		F.B._7_0004(ref HCH);
	}
}
