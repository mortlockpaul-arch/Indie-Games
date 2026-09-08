using System;
using System.Collections.Generic;
using _0002;
using D;
using E;
using Microsoft.Xna.Framework;
using P;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using r;

namespace x;

internal abstract class b<T> : ICollisionEntity, IDisposable, h, _0002.a where T : E.h
{
	private int a5h;

	private int a5b;

	private int a56;

	protected T _Entity;

	protected r.h _SpaceObject;

	protected ICollisionObject _Object;

	private CollisionPoint a5a = new CollisionPoint();

	public float Distance
	{
		get
		{
			if (_Entity == null)
			{
				return 0f;
			}
			return _Entity.LinearVelocity.Length();
		}
	}

	public Vector3 Normal
	{
		get
		{
			if (_Entity == null)
			{
				return Vector3.Zero;
			}
			Vector3 linearVelocity = _Entity.LinearVelocity;
			if (linearVelocity.LengthSquared() > 0f)
			{
				return Vector3.Normalize(linearVelocity);
			}
			return Vector3.Zero;
		}
	}

	public ICollisionObject Object => _Object;

	public b(ICollisionObject obj)
	{
		_Object = obj;
	}

	public void Build(r.a space)
	{
		Dispose();
		_SpaceObject = null;
		CreateEntity();
		if (_SpaceObject == null)
		{
			_SpaceObject = _Entity;
		}
		if (CanRecieveReactionForce())
		{
			_Entity.CollisionInformation.Events.DirectDispatchPairTouchedEventHandler = this;
		}
		_Entity.Tag = this;
		_Entity.CollisionInformation.Tag = this;
		ReaddSpaceObjectsToSpace(space);
	}

	public void GetSpaceObjects(List<r.h> spaceobjects)
	{
		spaceobjects.Add(_Entity);
	}

	public virtual void ReaddSpaceObjectsToSpace(r.a space)
	{
		if (_SpaceObject.Space != null)
		{
			_SpaceObject.Space.Remove(_SpaceObject);
		}
		space.Add(_SpaceObject);
	}

	public virtual void RemoveSpaceObjectsFromSpace()
	{
		if (_SpaceObject != null && _SpaceObject.Space != null)
		{
			_SpaceObject.Space.Remove(_SpaceObject);
		}
	}

	public virtual void Dispose()
	{
		RemoveSpaceObjectsFromSpace();
	}

	public void ApplyWorldForce(ref Vector3 worldforce)
	{
		ref T entity = ref _Entity;
		Vector3 position = _Entity.Position;
		Vector3 impulse = worldforce;
		entity.ApplyImpulse(position, impulse);
	}

	public void ApplyWorldForce(ref Vector3 worldposition, ref Vector3 worldforce)
	{
		_Entity.ApplyImpulse(ref worldposition, ref worldforce);
	}

	public void RemoveForces()
	{
		ref T entity = ref _Entity;
		Vector3 zero = Vector3.Zero;
		entity.LinearVelocity = zero;
		ref T entity2 = ref _Entity;
		Vector3 zero2 = Vector3.Zero;
		entity2.LinearMomentum = zero2;
		ref T entity3 = ref _Entity;
		Vector3 zero3 = Vector3.Zero;
		entity3.AngularVelocity = zero3;
		ref T entity4 = ref _Entity;
		Vector3 zero4 = Vector3.Zero;
		entity4.AngularMomentum = zero4;
	}

	public virtual bool CheckSceneObjectChanged()
	{
		bool flag = false;
		ICollisionMaterial defaultCollisionMaterial = _Object.DefaultCollisionMaterial;
		if (defaultCollisionMaterial != null)
		{
			flag = defaultCollisionMaterial.CollisionId != a56;
		}
		if (!flag && _Object.MoveId == a5h)
		{
			return _Object.CollisionId != a5b;
		}
		return true;
	}

	public virtual void SyncToPhysicsEntity()
	{
		ICollisionMaterial defaultCollisionMaterial = _Object.DefaultCollisionMaterial;
		if (defaultCollisionMaterial != null)
		{
			a56 = defaultCollisionMaterial.CollisionId;
		}
		a5h = _Object.MoveId;
		a5b = _Object.CollisionId;
	}

	public virtual void SyncToSceneObject()
	{
		a5h = _Object.MoveId;
	}

	protected bool CanRecieveReactionForce()
	{
		if (_Object.CollisionType == CollisionType.Collide)
		{
			return _Object.UpdateType == UpdateType.Automatic;
		}
		return false;
	}

	public void OnPairTouched(P.h sender, P.h other, D.b pair)
	{
		if (!(other.Tag is ICollisionEntity { Object: var collisionObject }))
		{
			return;
		}
		if (collisionObject.CollisionType == CollisionType.Collide)
		{
			a5a.Clear();
			a5a.ContactObject = collisionObject;
			a5a.ContactTime = pair.TimeOfImpact;
			a5a.Material = collisionObject.DefaultCollisionMaterial;
			if (pair.Contacts.Count > 0)
			{
				D._0001 obj = pair.Contacts[0];
				a5a.ContactPoint = obj.Contact.Position;
				a5a.SurfaceNormal = obj.Contact.Normal;
			}
			bool collisionhandled = false;
			_Object.OnCollisionReact(_Object, collisionObject, a5a, ref collisionhandled);
			collisionObject.OnCollisionReact(_Object, collisionObject, a5a, ref collisionhandled);
			if (collisionhandled)
			{
				throw new Exception("Default collisions can only be bypassed in legacy collision system (use the CollisionManager instead of PhysicsManager). Do not set OnCollisionReact(..., ref collisionhandled) to true.");
			}
		}
		else if (collisionObject.CollisionType == CollisionType.Trigger)
		{
			_Object.OnCollisionTrigger(_Object, collisionObject);
			collisionObject.OnCollisionTrigger(_Object, collisionObject);
		}
	}

	protected abstract void CreateEntity();
}
