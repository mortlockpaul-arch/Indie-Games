using System;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics;

public class Fixture : IDisposable
{
	private static int _fixtureIdCounter;

	public AfterCollisionEventHandler AfterCollision;

	public BeforeCollisionEventHandler BeforeCollision;

	public OnCollisionEventHandler OnCollision;

	public OnSeparationEventHandler OnSeparation;

	public FixtureProxy[] Proxies;

	public int ProxyCount;

	public CollisionFilter CollisionFilter { get; private set; }

	public ShapeType ShapeType => Shape.ShapeType;

	public Shape Shape { get; private set; }

	public bool IsSensor { get; set; }

	public Body Body { get; internal set; }

	public object UserData { get; set; }

	public float Friction { get; set; }

	public float Restitution { get; set; }

	public int FixtureId { get; private set; }

	public Fixture(Body body, Shape shape)
		: this(body, shape, null)
	{
	}

	public Fixture(Body body, Shape shape, object userData)
	{
		CollisionFilter = new CollisionFilter(this);
		Friction = 0.2f;
		Restitution = 0f;
		IsSensor = false;
		Body = body;
		UserData = userData;
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
		FixtureId = _fixtureIdCounter++;
		if ((Body.Flags & BodyFlags.Enabled) == BodyFlags.Enabled)
		{
			BroadPhase broadPhase = Body.World.ContactManager.BroadPhase;
			CreateProxies(broadPhase, ref Body.Xf);
		}
		Body.FixtureList.Add(this);
		if (Shape._density > 0f)
		{
			Body.ResetMassData();
		}
		Body.World.Flags |= WorldFlags.NewFixture;
		if (Body.World.FixtureAdded != null)
		{
			Body.World.FixtureAdded(this);
		}
	}

	public void Dispose()
	{
		Body.Dispose();
		GC.SuppressFinalize(this);
	}

	public bool TestPoint(ref Vector2 point)
	{
		return Shape.TestPoint(ref Body.Xf, ref point);
	}

	public bool RayCast(out RayCastOutput output, ref RayCastInput input, int childIndex)
	{
		return Shape.RayCast(out output, ref input, ref Body.Xf, childIndex);
	}

	public MassData GetMassData()
	{
		return Shape.MassData;
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
			FixtureProxy userData = Proxies[i];
			Shape.ComputeAABB(out userData.AABB, ref xf, i);
			userData.Fixture = this;
			userData.ChildIndex = i;
			userData.ProxyId = broadPhase.CreateProxy(ref userData.AABB, ref userData);
			Proxies[i] = userData;
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
}
