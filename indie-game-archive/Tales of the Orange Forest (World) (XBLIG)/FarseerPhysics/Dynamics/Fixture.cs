using System;
using System.Collections.Generic;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics;

public class Fixture
{
	private static int _fixtureIdCounter;

	public CollisionEventHandler OnCollision;

	public SeparationEventHandler OnSeparation;

	public Action<ContactConstraint> PostSolve;

	public FixtureProxy[] Proxies;

	public int ProxyCount;

	private CollisionCategory _collidesWith;

	private CollisionCategory _collisionCategories;

	private short _collisionGroup;

	private Dictionary<int, bool> _collisionIgnores = new Dictionary<int, bool>();

	public ShapeType ShapeType => Shape.ShapeType;

	public Shape Shape { get; private set; }

	public bool IsSensor { get; set; }

	public Body Body { get; internal set; }

	public object UserData { get; set; }

	public float Density { get; set; }

	public float Friction { get; set; }

	public float Restitution { get; set; }

	public short CollisionGroup
	{
		get
		{
			return _collisionGroup;
		}
		set
		{
			if (Body != null && _collisionGroup != value)
			{
				_collisionGroup = value;
				FilterChanged();
			}
		}
	}

	public CollisionCategory CollidesWith
	{
		get
		{
			return _collidesWith;
		}
		set
		{
			if (Body != null && _collidesWith != value)
			{
				_collidesWith = value;
				FilterChanged();
			}
		}
	}

	public int FixtureId { get; private set; }

	public CollisionCategory CollisionCategories
	{
		get
		{
			return _collisionCategories;
		}
		set
		{
			if (Body != null && _collisionCategories != value)
			{
				_collisionCategories = value;
				FilterChanged();
			}
		}
	}

	internal Fixture(Body body, Shape shape, float density)
	{
		Friction = 0.2f;
		_collisionCategories = CollisionCategory.All;
		_collidesWith = CollisionCategory.All;
		IsSensor = false;
		Body = body;
		Shape = shape.Clone();
		int childCount = Shape.ChildCount;
		Proxies = new FixtureProxy[childCount];
		for (int i = 0; i < childCount; i++)
		{
			Proxies[i] = default(FixtureProxy);
			Proxies[i].Fixture = null;
			Proxies[i].ProxyId = -1;
		}
		ProxyCount = 0;
		Density = density;
		FixtureId = _fixtureIdCounter++;
	}

	public bool TestPoint(ref Vector2 point)
	{
		Body.GetTransform(out var transform);
		return Shape.TestPoint(ref transform, ref point);
	}

	public bool RayCast(out RayCastOutput output, ref RayCastInput input, int childIndex)
	{
		Body.GetTransform(out var transform);
		return Shape.RayCast(out output, ref input, ref transform, childIndex);
	}

	public void GetMassData(out MassData massData)
	{
		Shape.ComputeMass(out massData, Density);
	}

	public void GetAABB(out AABB aabb, int childIndex)
	{
		aabb = Proxies[childIndex].AABB;
	}

	internal void Destroy()
	{
		Proxies = null;
		Shape = null;
		if (Body.World.FixtureRemoved != null)
		{
			Body.World.FixtureRemoved(this);
		}
	}

	internal void CreateProxies(BroadPhase broadPhase, ref Transform xf)
	{
		ProxyCount = Shape.ChildCount;
		for (int i = 0; i < ProxyCount; i++)
		{
			FixtureProxy fixtureProxy = Proxies[i];
			Shape.ComputeAABB(out fixtureProxy.AABB, ref xf, i);
			fixtureProxy.Fixture = this;
			fixtureProxy.ChildIndex = i;
			fixtureProxy.ProxyId = broadPhase.CreateProxy(ref fixtureProxy.AABB, fixtureProxy);
			Proxies[i] = fixtureProxy;
		}
	}

	internal void DestroyProxies(BroadPhase broadPhase)
	{
		for (int i = 0; i < ProxyCount; i++)
		{
			broadPhase.DestroyProxy(Proxies[i].ProxyId);
			Proxies[i].ProxyId = -1;
		}
		ProxyCount = 0;
	}

	internal void Synchronize(BroadPhase broadPhase, ref Transform transform1, ref Transform transform2)
	{
		if (ProxyCount != 0)
		{
			for (int i = 0; i < ProxyCount; i++)
			{
				FixtureProxy fixtureProxy = Proxies[i];
				Shape.ComputeAABB(out var aabb, ref transform1, fixtureProxy.ChildIndex);
				Shape.ComputeAABB(out var aabb2, ref transform2, fixtureProxy.ChildIndex);
				fixtureProxy.AABB.Combine(ref aabb, ref aabb2);
				Vector2 displacement = transform2.Position - transform1.Position;
				broadPhase.MoveProxy(fixtureProxy.ProxyId, ref fixtureProxy.AABB, displacement);
			}
		}
	}

	public void RestoreCollisionWith(Fixture fixture)
	{
		if (_collisionIgnores.ContainsKey(fixture.FixtureId))
		{
			_collisionIgnores[fixture.FixtureId] = false;
			FilterChanged();
		}
	}

	public void IgnoreCollisionWith(Fixture fixture)
	{
		if (_collisionIgnores.ContainsKey(fixture.FixtureId))
		{
			_collisionIgnores[fixture.FixtureId] = true;
		}
		else
		{
			_collisionIgnores.Add(fixture.FixtureId, value: true);
		}
		FilterChanged();
	}

	public bool IsFixtureIgnored(Fixture fixture)
	{
		if (_collisionIgnores.ContainsKey(fixture.FixtureId))
		{
			return _collisionIgnores[fixture.FixtureId];
		}
		return false;
	}

	private void FilterChanged()
	{
		for (ContactEdge contactEdge = Body.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
		{
			Contact contact = contactEdge.Contact;
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			if (fixtureA == this || fixtureB == this)
			{
				contact.FlagForFiltering();
			}
		}
	}
}
