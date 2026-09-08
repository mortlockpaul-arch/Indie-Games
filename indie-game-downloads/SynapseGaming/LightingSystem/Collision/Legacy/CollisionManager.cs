using System;
using System.Collections.Generic;
using _0012;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Collision.Legacy;

/// <summary>
/// Provides a built-in collision and lightweight physics implementation.
/// </summary>
public class CollisionManager : ICollisionManager, IManagerService, IRenderableManager, IUpdatableManager, IManager, IUnloadable
{
	private const double a5h = 0.0167;

	private const double a5b = 0.04;

	private int a56 = 10;

	private bool a5a;

	private float a57;

	private double a5_0006;

	private IManagerServiceProvider a5v;

	private List<SceneEntity> a5B = new List<SceneEntity>(128);

	private CollisionPoint a5X = new CollisionPoint();

	private List<SceneEntity> a5_0018 = new List<SceneEntity>(16);

	private SystemStatistic a5W = SystemConsole.GetStatistic("Collision_UpdateCalls", SystemStatisticCategory.Collision);

	private SystemStatistic a5_0002 = SystemConsole.GetStatistic("Collision_FindFastCalls", SystemStatisticCategory.Collision);

	private SystemStatistic a5_000E = SystemConsole.GetStatistic("Collision_SphereContainCalls", SystemStatisticCategory.Collision);

	private SystemStatistic a5y = SystemConsole.GetStatistic("Collision_BoxContainCalls", SystemStatisticCategory.Collision);

	private SystemStatistic a5r = SystemConsole.GetStatistic("Collision_Collisions", SystemStatisticCategory.Collision);

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
	public IManagerServiceProvider OwnerSceneInterface => a5v;

	/// <summary>
	/// Creates a new CollisionManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public CollisionManager(IManagerServiceProvider sceneinterface)
	{
		a5v = sceneinterface;
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public virtual void Unload()
	{
		GeometryExtractionHelper.Clear();
		Clear();
	}

	/// <summary>
	/// Updates all object movement, applies forces, and calculates collisions.
	///
	/// For any calculations to apply BeginFrameRendering() must be called. This prevents
	/// games from slowing down due to receiving several Update() calls per rendered frame.
	/// </summary>
	/// <param name="gametime"></param>
	public virtual void Update(GameTime gametime)
	{
		a5_0006 += gametime.ElapsedGameTime.TotalSeconds;
		if (!a5a || a5_0006 < 0.0167)
		{
			return;
		}
		double num = a5_0006;
		if (num > 0.04)
		{
			num = 0.04;
		}
		double num2 = num / 0.0167;
		if (num2 <= 0.0)
		{
			return;
		}
		a5_0006 = 0.0;
		a5W.AccumulationValue++;
		a5a = false;
		IObjectManager objectManager = (IObjectManager)a5v.GetManager(SceneInterface.ObjectManagerType, required: false);
		if (objectManager == null)
		{
			return;
		}
		LightingSystemPerformance.Begin("CollisionManager.Update");
		a5B.Clear();
		objectManager.FindFast(a5B);
		Vector3 worldforce = new Vector3(0f, 0f - CoreHelper.GetGravityForTimePeriod(a57, num), 0f);
		foreach (SceneEntity item in a5B)
		{
			SceneEntityTypeCaster sceneEntityTypeCaster = OptimizationSystem.SceneEntityTypeCasters.Get(item);
			ICollisionObject collisionObject = sceneEntityTypeCaster.CollisionObject;
			if (collisionObject != null && collisionObject.CollisionType != CollisionType.None)
			{
				CollisionMove collisionMove = collisionObject.CollisionMove as CollisionMove;
				if (collisionMove == null)
				{
					collisionMove = (CollisionMove)(collisionObject.CollisionMove = new CollisionMove(collisionObject));
				}
				if (collisionObject.AffectedByGravity && collisionObject.UpdateType == UpdateType.Automatic)
				{
					collisionMove.ApplyWorldForce(worldforce, constantforce: true);
				}
				collisionMove.Begin((float)num2);
			}
		}
		foreach (SceneEntity item2 in a5B)
		{
			if (item2.UpdateType == UpdateType.None)
			{
				continue;
			}
			SceneEntityTypeCaster sceneEntityTypeCaster2 = OptimizationSystem.SceneEntityTypeCasters.Get(item2);
			ICollisionObject collisionObject2 = sceneEntityTypeCaster2.CollisionObject;
			if (collisionObject2 != null && collisionObject2.CollisionType != CollisionType.None && collisionObject2.CollisionMove is CollisionMove { Distance: 0 } collisionMove3)
			{
				a5X.Clear();
				MoveObject(collisionObject2, objectManager, a5X);
				collisionMove3.End(a5X);
				if (a5X.ContactTime < 1f)
				{
					a5r.AccumulationValue++;
				}
			}
		}
	}

	/// <summary>
	/// Sets up the object prior to rendering.
	/// </summary>
	/// <param name="scenestate"></param>
	public virtual void BeginFrameRendering(ISceneState scenestate)
	{
		a5a = true;
		a57 = scenestate.Environment.Gravity;
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public virtual void EndFrameRendering()
	{
	}

	/// <summary>
	/// Not supported in legacy collision.
	/// </summary>
	/// <param name="startposition"></param>
	/// <param name="endposition"></param>
	/// <param name="firsthit"></param>
	/// <returns></returns>
	public bool RayCast(Vector3 startposition, Vector3 endposition, out RayCollisionPoint firsthit)
	{
		firsthit = default(RayCollisionPoint);
		return false;
	}

	/// <summary>
	/// Not supported in legacy collision.
	/// </summary>
	/// <param name="hits"></param>
	/// <param name="startposition"></param>
	/// <param name="endposition"></param>
	public void RayCast(List<RayCollisionPoint> hits, Vector3 startposition, Vector3 endposition)
	{
	}

	/// <summary>
	/// Not supported in legacy collision.
	/// </summary>
	/// <param name="ray"></param>
	/// <param name="castdistance"></param>
	/// <param name="firsthit"></param>
	/// <returns></returns>
	public bool RayCast(Ray ray, float castdistance, out RayCollisionPoint firsthit)
	{
		firsthit = default(RayCollisionPoint);
		return false;
	}

	/// <summary>
	/// Not supported in legacy collision.
	/// </summary>
	/// <param name="hits"></param>
	/// <param name="ray"></param>
	/// <param name="castdistance"></param>
	public void RayCast(List<RayCollisionPoint> hits, Ray ray, float castdistance)
	{
	}

	/// <summary>
	/// Simulates the object move, checking for collisions along the way.
	/// </summary>
	/// <param name="movingobj">Object to move (collider).</param>
	/// <param name="objectmanager"></param>
	/// <param name="worldcollisionpoint">Filled with collidee information during the call.</param>
	protected virtual void MoveObject(ICollisionObject movingobj, IObjectManager objectmanager, CollisionPoint worldcollisionpoint)
	{
		LightingSystemPerformance.Begin("CollisionManager.MoveObject (find)");
		if (!(movingobj.CollisionMove is CollisionMove { WorldSweepBoundingBox: var box }))
		{
			return;
		}
		BoundingSphere.CreateFromBoundingBox(ref box, out var result);
		a5_0018.Clear();
		a5_0002.AccumulationValue++;
		objectmanager.Find(a5_0018, box, ObjectFilter.All);
		LightingSystemPerformance.Begin("CollisionManager.MoveObject (test)");
		foreach (SceneEntity item in a5_0018)
		{
			SceneEntityTypeCaster sceneEntityTypeCaster = OptimizationSystem.SceneEntityTypeCasters.Get(item);
			ICollisionObject collisionObject = sceneEntityTypeCaster.CollisionObject;
			if (collisionObject == null || collisionObject == movingobj || collisionObject.CollisionType == CollisionType.None)
			{
				continue;
			}
			CollisionMove collisionMove2 = collisionObject.CollisionMove as CollisionMove;
			if (collisionMove2 == null)
			{
				collisionMove2 = (CollisionMove)(collisionObject.CollisionMove = new CollisionMove(collisionObject));
				collisionMove2.Begin(0f);
			}
			a5_000E.AccumulationValue++;
			collisionMove2.WorldBoundingSphere.Contains(ref result, out var result2);
			if (result2 != ContainmentType.Disjoint)
			{
				a5y.AccumulationValue++;
				collisionMove2.WorldBoundingBox.Contains(ref box, out result2);
				if (result2 != ContainmentType.Disjoint)
				{
					Collide(movingobj, collisionObject, worldcollisionpoint);
				}
			}
		}
	}

	/// <summary>
	/// Tests for collision between two objects.  Assumes CollisionMove.Begin()
	/// is already called on the moving object.
	/// </summary>
	/// <param name="movingobj">Moving / collider object.</param>
	/// <param name="staticobj">Potential collidee.</param>
	/// <param name="worldcollisionpoint">Filled with collidee information during the call.</param>
	public virtual void Collide(ICollisionObject movingobj, ICollisionObject staticobj, CollisionPoint worldcollisionpoint)
	{
		LightingSystemPerformance.Begin("CollisionManager.Collide");
		if (movingobj.HullType == HullType.Sphere)
		{
			if (staticobj.HullType == HullType.Sphere)
			{
				_0012._6.bw(movingobj, staticobj, worldcollisionpoint);
			}
			else if (staticobj.HullType == HullType.Box)
			{
				_0012._6.b_0011(movingobj, staticobj, worldcollisionpoint);
			}
			else if (staticobj.HullType == HullType.Mesh)
			{
				_0012._6.bS(movingobj, staticobj, worldcollisionpoint);
			}
		}
		else if (movingobj.HullType == HullType.Box)
		{
			if (staticobj.HullType == HullType.Sphere)
			{
				_0012.b.b9(movingobj, staticobj, worldcollisionpoint);
			}
			else if (staticobj.HullType == HullType.Box)
			{
				_0012.b.b1(movingobj, staticobj, worldcollisionpoint);
			}
			else if (staticobj.HullType == HullType.Mesh)
			{
				_0012.b.bf(movingobj, staticobj, worldcollisionpoint);
			}
		}
	}

	/// <summary>
	/// Removes resources managed by this object. Commonly used while clearing the scene.
	/// </summary>
	public virtual void Clear()
	{
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public virtual void ApplyPreferences(ISystemPreferences preferences)
	{
	}
}
