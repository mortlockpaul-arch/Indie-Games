using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using _0014;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;
using m;
using r;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Provides a built-in collision and lightweight physics implementation.
/// </summary>
public class PhysicsManager : ICollisionManager, IRenderableManager, IUpdatableManager, IManagerService, IManager, IUnloadable
{
	private const float a5h = 0.0167f;

	private const float a5b = 0.0333f;

	private int a56 = 10;

	private bool a5a;

	private bool a57 = true;

	private bool a5_0006 = true;

	private bool a5v;

	private int a5B = 4;

	private bool a5X = true;

	private r.v a5_0018;

	private List<SceneEntity> a5W = new List<SceneEntity>();

	private List<Avatar> a5_0002 = new List<Avatar>();

	private List<r._7> a5_000E = new List<r._7>();

	private List<PhysicsMove> a5y = new List<PhysicsMove>();

	[CompilerGenerated]
	private IManagerServiceProvider a5r;

	[CompilerGenerated]
	private static Action<object> a5_0001;

	[CompilerGenerated]
	private static Action<object> a5_000F;

	[CompilerGenerated]
	private static Action<object> a5Z;

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public Type ManagerType => SceneInterface.CollisionManagerType;

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
			return a56;
		}
		set
		{
			a56 = value;
		}
	}

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface
	{
		[CompilerGenerated]
		get
		{
			return a5r;
		}
		[CompilerGenerated]
		private set
		{
			a5r = managerServiceProvider;
		}
	}

	/// <summary>
	/// Enables higher frequency physics updates. This smoothes visual movement, but also increases physics overhead.
	/// </summary>
	public bool FastUpdateEnabled
	{
		get
		{
			return a5a;
		}
		set
		{
			a5a = value;
		}
	}

	public bool LockToFrameRateEnabled
	{
		get
		{
			return a57;
		}
		set
		{
			a57 = value;
		}
	}

	/// <summary>
	/// Enables multi-threaded physics processing.
	/// </summary>
	public bool MultiThreadingEnabled
	{
		get
		{
			return a5_0006;
		}
		set
		{
			a5_0006 = value;
		}
	}

	/// <summary>
	/// Enables smooth interpolation of movement over time steps (may adversely affect objects manually rotated via the World transform).
	///
	/// This value *must* be set prior to any calls to the PhysicsManager.
	/// </summary>
	public bool BufferingEnabled
	{
		get
		{
			return a5v;
		}
		set
		{
			a5v = value;
		}
	}

	/// <summary>
	/// Maximum number of time steps processed in a single update.
	///
	/// Several time steps may queue up when the game's Update() frequency is reduced due to slow performance.
	/// </summary>
	public int MaximumTimeStepsPerFrame
	{
		get
		{
			return a5B;
		}
		set
		{
			a5B = value;
		}
	}

	internal r.v Space
	{
		get
		{
			if (a5_0018 != null)
			{
				return a5_0018;
			}
			a5_0018 = new r.v();
			a5_0018.TimeStepSettings.MaximumTimeStepsPerFrame = a5B;
			a5_0018.TimeStepSettings.TimeStepDuration = 1f / 30f;
			_0014._6.DefaultPositionUpdateMode = m._7.Continuous;
			a5_0018.BufferedStates.Enabled = a5v;
			a5_0018.Solver.AllowMultithreading = a5_0006;
			a5_0018.Solver.IterationLimit = 2;
			a5_0018.ForceUpdater.Gravity = new Vector3(0f, -18f, 0f);
			a5_0018.ThreadManager.AddThread(delegate
			{
				Thread.CurrentThread.SetProcessorAffinity(new int[1] { 1 });
			}, null);
			a5_0018.ThreadManager.AddThread(delegate
			{
				Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
			}, null);
			a5_0018.ThreadManager.AddThread(delegate
			{
				Thread.CurrentThread.SetProcessorAffinity(new int[1] { 5 });
			}, null);
			return a5_0018;
		}
	}

	/// <summary>
	/// Creates a new PhysicsManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public PhysicsManager(IManagerServiceProvider sceneinterface)
	{
		OwnerSceneInterface = sceneinterface;
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public void ApplyPreferences(ISystemPreferences preferences)
	{
	}

	/// <summary>
	/// Called when the game begins rendering the current frame.
	/// </summary>
	/// <param name="scenestate"></param>
	public void BeginFrameRendering(ISceneState scenestate)
	{
		a5X = true;
		Space.Solver.AllowMultithreading = a5_0006;
		Space.TimeStepSettings.MaximumTimeStepsPerFrame = a5B;
		Space.ForceUpdater.Gravity = new Vector3(0f, 0f - scenestate.Environment.Gravity, 0f);
		_0014.b.BouncinessVelocityThreshold = scenestate.Environment.Gravity * 0.1f;
	}

	/// <summary>
	/// Called when the game finishes rendering the current frame.
	/// </summary>
	public void EndFrameRendering()
	{
	}

	private bool bj(ref Vector3 P_0, ref Vector3 P_1, out Ray P_2, out float P_3)
	{
		Vector3 direction = new Vector3
		{
			X = P_1.X - P_0.X,
			Y = P_1.Y - P_0.Y,
			Z = P_1.Z - P_0.Z
		};
		P_3 = direction.Length();
		if (P_3 <= 0f)
		{
			P_2 = default(Ray);
			return false;
		}
		direction.X /= P_3;
		direction.Y /= P_3;
		direction.Z /= P_3;
		P_2 = new Ray(P_0, direction);
		return true;
	}

	private void bk(ref RayCollisionPoint P_0, r._7 P_1)
	{
		P_0.ContactTime = P_1.HitData.T;
		P_0.ContactPoint = P_1.HitData.Location;
		P_0.SurfaceNormal = P_1.HitData.Normal;
		if (P_0.SurfaceNormal != Vector3.Zero)
		{
			P_0.SurfaceNormal.Normalize();
		}
		if (P_1.HitObject.Tag is ICollisionEntity { Object: var collisionObject })
		{
			P_0.ContactObject = collisionObject;
			P_0.Material = collisionObject.DefaultCollisionMaterial;
		}
	}

	/// <summary>
	/// Casts a ray into the scene and returns the first intersected collidable object.
	/// </summary>
	/// <param name="startposition">World space start position of the ray.</param>
	/// <param name="endposition">World space end position of the ray.</param>
	/// <param name="firsthit">Output intersection information.</param>
	/// <returns>Returns true if an intersection occurs.</returns>
	public bool RayCast(Vector3 startposition, Vector3 endposition, out RayCollisionPoint firsthit)
	{
		if (!bj(ref startposition, ref endposition, out var ray, out var castdistance))
		{
			firsthit = default(RayCollisionPoint);
			return false;
		}
		return RayCast(ray, castdistance, out firsthit);
	}

	/// <summary>
	/// Casts a ray into the scene and returns all intersected collidable object.
	/// </summary>
	/// <param name="hits">Resulting intersection information.</param>
	/// <param name="startposition">World space start position of the ray.</param>
	/// <param name="endposition">World space end position of the ray.</param>
	public void RayCast(List<RayCollisionPoint> hits, Vector3 startposition, Vector3 endposition)
	{
		if (bj(ref startposition, ref endposition, out var ray, out var castdistance))
		{
			RayCast(hits, ray, castdistance);
		}
	}

	/// <summary>
	/// Casts a ray into the scene and returns the first intersected collidable object.
	/// </summary>
	/// <param name="ray">Normalized world space ray.</param>
	/// <param name="castdistance">Distance to cast ray.</param>
	/// <param name="firsthit">Output intersection information.</param>
	/// <returns>Returns true if an intersection occurs.</returns>
	public bool RayCast(Ray ray, float castdistance, out RayCollisionPoint firsthit)
	{
		r.v v2 = Space;
		if (!v2.RayCast(ray, castdistance, out var result))
		{
			firsthit = default(RayCollisionPoint);
			return false;
		}
		firsthit = default(RayCollisionPoint);
		bk(ref firsthit, result);
		return true;
	}

	/// <summary>
	/// Casts a ray into the scene and returns all intersected collidable object.
	/// </summary>
	/// <param name="hits">Resulting intersection information.</param>
	/// <param name="ray">Normalized world space ray.</param>
	/// <param name="castdistance">Distance to cast ray.</param>
	public void RayCast(List<RayCollisionPoint> hits, Ray ray, float castdistance)
	{
		r.v v2 = Space;
		a5_000E.Clear();
		if (!v2.RayCast(ray, castdistance, a5_000E))
		{
			return;
		}
		foreach (r._7 item2 in a5_000E)
		{
			RayCollisionPoint item = default(RayCollisionPoint);
			bk(ref item, item2);
			hits.Add(item);
		}
	}

	/// <summary>
	/// Updates all object movement, applies forces, and calculates collisions.
	///
	/// For any calculations to apply BeginFrameRendering() must be called. This prevents
	/// games from slowing down due to receiving several Update() calls per rendered frame.
	/// </summary>
	/// <param name="gametime"></param>
	public void Update(GameTime gametime)
	{
		IObjectManager objectManager = (IObjectManager)OwnerSceneInterface.GetManager(SceneInterface.ObjectManagerType, required: false);
		if (objectManager == null)
		{
			return;
		}
		float num = (float)gametime.ElapsedGameTime.TotalSeconds;
		Space.TimeStepSettings.AccumulatedTime += num;
		if ((!a5X && a57) || num <= 0f)
		{
			return;
		}
		Space.TimeStepSettings.TimeStepDuration = (FastUpdateEnabled ? 0.0333f : 0.0167f);
		if (a5v)
		{
			if (a5a && Space.TimeStepSettings.AccumulatedTime < Space.TimeStepSettings.TimeStepDuration)
			{
				return;
			}
		}
		else if (Space.TimeStepSettings.AccumulatedTime < Space.TimeStepSettings.TimeStepDuration)
		{
			return;
		}
		a5X = false;
		a5W.Clear();
		objectManager.FindFast(a5W);
		a5y.Clear();
		foreach (SceneEntity item in a5W)
		{
			SceneEntityTypeCaster sceneEntityTypeCaster = OptimizationSystem.SceneEntityTypeCasters.Get(item);
			ICollisionObject collisionObject = sceneEntityTypeCaster.CollisionObject;
			if (collisionObject != null && collisionObject.CollisionType != CollisionType.None)
			{
				PhysicsMove physicsMove = collisionObject.CollisionMove as PhysicsMove;
				if (physicsMove == null)
				{
					physicsMove = (PhysicsMove)(collisionObject.CollisionMove = new PhysicsMove(collisionObject));
				}
				physicsMove.Begin();
				if (item.UpdateType == UpdateType.Automatic)
				{
					a5y.Add(physicsMove);
				}
			}
		}
		IAvatarManager avatarManager = (IAvatarManager)OwnerSceneInterface.GetManager(SceneInterface.AvatarManagerType, required: false);
		if (avatarManager != null)
		{
			a5_0002.Clear();
			avatarManager.FindFast(a5_0002);
			foreach (Avatar item2 in a5_0002)
			{
				ICollisionObject collisionObject2 = item2;
				if (collisionObject2 != null && collisionObject2.CollisionType != CollisionType.None)
				{
					PhysicsMove physicsMove2 = collisionObject2.CollisionMove as PhysicsMove;
					if (physicsMove2 == null)
					{
						physicsMove2 = (PhysicsMove)(collisionObject2.CollisionMove = new PhysicsMove(collisionObject2));
					}
					physicsMove2.Begin();
					if (item2.UpdateType == UpdateType.Automatic)
					{
						a5y.Add(physicsMove2);
					}
				}
			}
		}
		Space.Update(0f);
		foreach (PhysicsMove item3 in a5y)
		{
			item3.End();
		}
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
		if (a5_0018 != null)
		{
			a5_0018.Dispose();
			a5_0018 = null;
		}
	}

	[CompilerGenerated]
	private static void b_0015(object P_0)
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 1 });
	}

	[CompilerGenerated]
	private static void b_0016(object P_0)
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
	}

	[CompilerGenerated]
	private static void bK(object P_0)
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 5 });
	}
}
