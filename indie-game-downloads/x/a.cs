using _0013;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using q;

namespace x;

internal class a : x.b<_0013.b>
{
	private Matrix a5h;

	public a(ICollisionObject obj)
		: base(obj)
	{
	}

	protected override void CreateEntity()
	{
		_Entity = new _0013.b(Vector3.Zero, 1f, 1f, 1f);
		SyncToPhysicsEntity();
	}

	public override void SyncToPhysicsEntity()
	{
		CoreHelper.Decompose(_Object.World, out var scale, out var rotation, out var _);
		BoundingBox objectBoundingBox = _Object.ObjectBoundingBox;
		Vector3 vector = (objectBoundingBox.Min + objectBoundingBox.Max) * 0.5f;
		Vector3 vector2 = objectBoundingBox.Max - objectBoundingBox.Min;
		vector2 *= scale;
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
		_Entity.Width = vector2.X;
		_Entity.Height = vector2.Y;
		_Entity.Length = vector2.Z;
		_Entity.Orientation = rotation;
		_Entity.Position = Vector3.Transform(vector, _Object.World);
		ICollisionMaterial defaultCollisionMaterial = _Object.DefaultCollisionMaterial;
		if (defaultCollisionMaterial != null)
		{
			_Entity.Material.Bounciness = 1f - defaultCollisionMaterial.Elasticity;
			_Entity.Material.KineticFriction = defaultCollisionMaterial.Friction;
			_Entity.Material.StaticFriction = defaultCollisionMaterial.Friction;
		}
		a5h = Matrix.CreateTranslation(-vector) * Matrix.CreateScale(scale);
		base.SyncToPhysicsEntity();
	}

	public override void SyncToSceneObject()
	{
		_Object.World = a5h * _Entity.BufferedStates.InterpolatedStates.WorldTransform;
		base.SyncToSceneObject();
	}
}
