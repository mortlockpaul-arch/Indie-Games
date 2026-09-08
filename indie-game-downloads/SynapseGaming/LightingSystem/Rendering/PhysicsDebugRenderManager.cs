using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using r;
using x;
using z;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Helper renderer that displays the collision hulls of all
/// collidable scene objects and lights.
///
/// Can help tune performance and work out bugs by seeing how
/// objects within the scene overlap and interact with each other.
/// </summary>
public class PhysicsDebugRenderManager : IManagerService, IRenderableManager, IManager, IUnloadable
{
	private int a5h = 100;

	private ISceneState a5b;

	private IManagerServiceProvider a56;

	private z.b a5a;

	private List<SceneEntity> a57 = new List<SceneEntity>();

	private List<SceneEntity> a5_0006 = new List<SceneEntity>();

	private List<r.h> a5v = new List<r.h>();

	[CompilerGenerated]
	private bool a5B;

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public Type ManagerType => typeof(PhysicsDebugRenderManager);

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
			return a5h;
		}
		set
		{
			a5h = value;
		}
	}

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => a56;

	/// <summary>
	/// Determines if debug information should render when the SunBurn editor is open.
	/// </summary>
	public bool RenderInEditor
	{
		[CompilerGenerated]
		get
		{
			return a5B;
		}
		[CompilerGenerated]
		set
		{
			a5B = value;
		}
	}

	/// <summary>
	/// Creates a new PhysicsDebugRenderManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public PhysicsDebugRenderManager(IManagerServiceProvider sceneinterface)
	{
		a56 = sceneinterface;
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
		a5b = scenestate;
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public void EndFrameRendering()
	{
		_ = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		SunBurnEditor sunBurnEditor = a56.GetManager(SceneInterface.EditorType, required: false) as SunBurnEditor;
		if (!(a56.GetManager(SceneInterface.ObjectManagerType, required: false) is IObjectManager objectManager) || (!RenderInEditor && sunBurnEditor != null && sunBurnEditor.EditorAttached))
		{
			return;
		}
		if (a5a == null)
		{
			a5a = new z._6();
		}
		a57.Clear();
		objectManager.Find(a57, ObjectFilter.All);
		a5_0006.Clear();
		objectManager.Find(a5_0006, a5b.ViewFrustum, ObjectFilter.All);
		foreach (SceneEntity item in a57)
		{
			if (item == null || !(item is ICollisionObject { CollisionType: not CollisionType.None, CollisionMove: PhysicsMove { CollisionEntity: x.h collisionEntity } }))
			{
				continue;
			}
			bool flag = a5_0006.Contains(item);
			a5v.Clear();
			collisionEntity.GetSpaceObjects(a5v);
			foreach (r.h item2 in a5v)
			{
				a5a.b_0017(item2, flag);
			}
		}
		a5a.Update();
		a5a.Draw(a5b.View, a5b.Projection);
	}

	/// <summary>
	/// Removes resources managed by this object. Commonly used while clearing the scene.
	/// </summary>
	public void Clear()
	{
		a5a.Clear();
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public void Unload()
	{
		Clear();
		a5a.Unload();
	}
}
