using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;
using x;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Provides the built-in implement of object physics movement
/// and applying force.
/// </summary>
public class PhysicsMove : BaseCollisionMove
{
	private ICollisionEntity a5h;

	private PhysicsManager a5b;

	/// <summary />
	protected PhysicsManager PhysicsManager => a5b;

	/// <summary>
	/// Creates a new PhysicsMove instance.
	/// </summary>
	/// <param name="parent">Collision object to move.</param>
	public PhysicsMove(ICollisionObject parent)
		: base(parent)
	{
	}

	/// <summary>
	/// Called when the parent object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public override void OnSubmittedToManager(IManagerService manager)
	{
		if (manager.OwnerSceneInterface.GetManager(SceneInterface.CollisionManagerType, required: false) is PhysicsManager physicsManager)
		{
			a5b = physicsManager;
		}
		base.OnSubmittedToManager(manager);
	}

	/// <summary>
	/// Called when the parent object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public override void OnRemovedFromManager(IManagerService manager)
	{
		a5b = null;
		a5h = base.CollisionEntity;
		base.OnRemovedFromManager(manager);
	}

	/// <summary>
	/// Creates a new collision / physics system entity, which represents the ParentObject in the simulation.
	/// </summary>
	/// <returns></returns>
	protected override ICollisionEntity CreateCollisionEntity()
	{
		if (a5b == null)
		{
			return null;
		}
		x.h h2 = a5h as x.h;
		if (base.ParentObject.HullType == HullType.Sphere)
		{
			if (h2 == null || (object)h2.GetType() != typeof(x._6))
			{
				x._6 obj = new x._6(base.ParentObject);
				obj.Build(a5b.Space);
				return obj;
			}
		}
		else if (base.ParentObject.HullType == HullType.Mesh)
		{
			if (h2 == null || (object)h2.GetType() != typeof(x._7))
			{
				x._7 obj2 = new x._7(base.ParentObject);
				obj2.Build(a5b.Space);
				return obj2;
			}
		}
		else if (h2 == null || (object)h2.GetType() != typeof(x.a))
		{
			x.a a2 = new x.a(base.ParentObject);
			a2.Build(a5b.Space);
			return a2;
		}
		h2.ReaddSpaceObjectsToSpace(a5b.Space);
		return a5h;
	}
}
