using System;
using _000F;
using _0013;
using Microsoft.Xna.Framework;
using N;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using q;
using r;
using x;

namespace q
{
	internal interface h
	{
		_7 CollisionRules { get; set; }
	}
}
namespace Q
{
	internal class h : x.b<_0013.h>
	{
		private float a5h;

		private Matrix a5b;

		private Vector3 a56;

		private _000F.h a5a = new _000F.h();

		private b a57;

		public _000F.h Controller => a5a;

		public b RotationalMotionConstraint => a57;

		public float MovementSmoothing
		{
			get
			{
				return a5h;
			}
			set
			{
				a5h = MathHelper.Clamp(value, 0f, 0.95f);
			}
		}

		public h(ICollisionObject obj)
			: base(obj)
		{
			a5h = 0.25f;
			a57 = new b(a5a);
		}

		protected override void CreateEntity()
		{
			_Entity = a5a.Body;
			_SpaceObject = a5a;
			SyncToPhysicsEntity();
		}

		public override void ReaddSpaceObjectsToSpace(r.a space)
		{
			base.ReaddSpaceObjectsToSpace(space);
			r.h h2 = a57;
			if (h2.Space != null)
			{
				h2.Space.Remove(h2);
			}
			space.Add(h2);
		}

		public override void RemoveSpaceObjectsFromSpace()
		{
			base.RemoveSpaceObjectsFromSpace();
			r.h h2 = a57;
			if (h2.Space != null)
			{
				h2.Space.Remove(h2);
			}
		}

		public override void SyncToPhysicsEntity()
		{
			bool ignoreShapeChanges = _Entity.IgnoreShapeChanges;
			_Entity.IgnoreShapeChanges = false;
			Matrix world = _Object.World;
			CoreHelper.Decompose(world, out var scale, out var rotation, out var _);
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
			a5a.StanceManager.StandingHeight = vector2.Y;
			a5a.StanceManager.CrouchingHeight = vector2.Y * 0.7f;
			_Entity.Height = vector2.Y;
			_Entity.Radius = Math.Max(vector2.X, vector2.Z) * 0.5f;
			_Entity.Orientation = rotation;
			_Entity.Position = Vector3.Transform(vector, _Object.World);
			ICollisionMaterial defaultCollisionMaterial = _Object.DefaultCollisionMaterial;
			if (defaultCollisionMaterial != null)
			{
				_Entity.Material.Bounciness = 1f - defaultCollisionMaterial.Elasticity;
				_Entity.Material.KineticFriction = defaultCollisionMaterial.Friction;
				_Entity.Material.StaticFriction = defaultCollisionMaterial.Friction;
			}
			a5b = Matrix.CreateTranslation(-vector) * Matrix.CreateScale(scale);
			a56 = _Entity.BufferedStates.InterpolatedStates.WorldTransform.Translation;
			base.SyncToPhysicsEntity();
			_Entity.IgnoreShapeChanges = ignoreShapeChanges;
			_Entity.LocalInertiaTensorInverse = default(N._7);
		}

		public override void SyncToSceneObject()
		{
			if (a5h <= 0f)
			{
				_Object.World = a5b * _Entity.BufferedStates.InterpolatedStates.WorldTransform;
			}
			else
			{
				Matrix worldTransform = _Entity.BufferedStates.InterpolatedStates.WorldTransform;
				a56 = Vector3.Lerp(worldTransform.Translation, a56, a5h);
				worldTransform.Translation = a56;
				_Object.World = a5b * worldTransform;
			}
			base.SyncToSceneObject();
		}
	}
}
