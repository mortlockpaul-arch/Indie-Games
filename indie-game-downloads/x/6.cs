using _0013;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using q;

namespace x;

internal class _6 : x.b<_0013._6>
{
	private Matrix a5h;

	public _6(ICollisionObject obj)
		: base(obj)
	{
	}

	protected override void CreateEntity()
	{
		_Entity = new _0013._6(Vector3.Zero, 1f);
		SyncToPhysicsEntity();
	}

	public override void SyncToPhysicsEntity()
	{
		Matrix world = _Object.World;
		CoreHelper.Decompose(world, out var scale, out var rotation, out var _);
		BoundingSphere worldBoundingSphere = _Object.WorldBoundingSphere;
		BoundingSphere objectBoundingSphere = _Object.ObjectBoundingSphere;
		if (CanRecieveReactionForce())
		{
			_Entity.Mass = _Object.Mass;
			_Entity.IsAffectedByGravity = _Object.AffectedByGravity;
		}
		else
		{
			_Entity.Mass = 0f;
			_Entity.IsAffectedByGravity = false;
		}
		if (_Object.CollisionType == CollisionType.Collide)
		{
			_Entity.CollisionInformation.CollisionRules.Personal = q.a.Normal;
		}
		else
		{
			_Entity.CollisionInformation.CollisionRules.Personal = q.a.NoSolver;
		}
		_Entity.Radius = worldBoundingSphere.Radius;
		_Entity.Orientation = rotation;
		_Entity.Position = worldBoundingSphere.Center;
		ICollisionMaterial defaultCollisionMaterial = _Object.DefaultCollisionMaterial;
		if (defaultCollisionMaterial != null)
		{
			_Entity.Material.Bounciness = 1f - defaultCollisionMaterial.Elasticity;
			_Entity.Material.KineticFriction = defaultCollisionMaterial.Friction;
			_Entity.Material.StaticFriction = defaultCollisionMaterial.Friction;
		}
		a5h = Matrix.CreateTranslation(-objectBoundingSphere.Center) * Matrix.CreateScale(scale);
		base.SyncToPhysicsEntity();
	}

	public override void SyncToSceneObject()
	{
		_Object.World = a5h * _Entity.BufferedStates.InterpolatedStates.WorldTransform;
		base.SyncToSceneObject();
	}
}
