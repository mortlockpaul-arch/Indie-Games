using System;
using System.Collections.Generic;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics;

public class Body
{
	internal float AngularVelocityInternal;

	internal BodyFlags Flags;

	internal Vector2 Force;

	internal float InvI;

	internal float InvMass;

	internal Vector2 LinearVelocityInternal;

	internal float SleepTime;

	internal Sweep Sweep;

	internal float Torque;

	internal BodyType Type;

	internal World World;

	internal Transform Xf;

	private float _inertia;

	private float _mass;

	public float Revolutions => Rotation / (float)Math.PI;

	public BodyType BodyType
	{
		get
		{
			return Type;
		}
		set
		{
			if (Type != value)
			{
				Type = value;
				ResetMassData();
				if (Type == BodyType.Static)
				{
					LinearVelocityInternal = Vector2.Zero;
					AngularVelocityInternal = 0f;
				}
				Awake = true;
				Force = Vector2.Zero;
				Torque = 0f;
				for (ContactEdge contactEdge = ContactList; contactEdge != null; contactEdge = contactEdge.Next)
				{
					contactEdge.Contact.FlagForFiltering();
				}
			}
		}
	}

	public Vector2 LinearVelocity
	{
		get
		{
			return LinearVelocityInternal;
		}
		set
		{
			if (Type != BodyType.Static)
			{
				if (Vector2.Dot(value, value) > 0f)
				{
					Awake = true;
				}
				LinearVelocityInternal = value;
			}
		}
	}

	public float AngularVelocity
	{
		get
		{
			return AngularVelocityInternal;
		}
		set
		{
			if (Type != BodyType.Static)
			{
				if (value * value > 0f)
				{
					Awake = true;
				}
				AngularVelocityInternal = value;
			}
		}
	}

	public float LinearDamping { get; set; }

	public float AngularDamping { get; set; }

	public bool IsBullet
	{
		get
		{
			return (Flags & BodyFlags.Bullet) == BodyFlags.Bullet;
		}
		set
		{
			if (value)
			{
				Flags |= BodyFlags.Bullet;
			}
			else
			{
				Flags &= ~BodyFlags.Bullet;
			}
		}
	}

	public bool SleepingAllowed
	{
		get
		{
			return (Flags & BodyFlags.AutoSleep) == BodyFlags.AutoSleep;
		}
		set
		{
			if (value)
			{
				Flags |= BodyFlags.AutoSleep;
				return;
			}
			Flags &= ~BodyFlags.AutoSleep;
			Awake = true;
		}
	}

	public bool Awake
	{
		get
		{
			return (Flags & BodyFlags.Awake) == BodyFlags.Awake;
		}
		set
		{
			if (value)
			{
				if ((Flags & BodyFlags.Awake) == 0)
				{
					Flags |= BodyFlags.Awake;
					SleepTime = 0f;
				}
			}
			else
			{
				Flags &= ~BodyFlags.Awake;
				SleepTime = 0f;
				LinearVelocityInternal = Vector2.Zero;
				AngularVelocityInternal = 0f;
				Force = Vector2.Zero;
				Torque = 0f;
			}
		}
	}

	public bool Active
	{
		get
		{
			return (Flags & BodyFlags.Active) == BodyFlags.Active;
		}
		set
		{
			if (value == Active)
			{
				return;
			}
			if (value)
			{
				Flags |= BodyFlags.Active;
				BroadPhase broadPhase = World.ContactManager.BroadPhase;
				{
					foreach (Fixture fixture in FixtureList)
					{
						fixture.CreateProxies(broadPhase, ref Xf);
					}
					return;
				}
			}
			Flags &= ~BodyFlags.Active;
			BroadPhase broadPhase2 = World.ContactManager.BroadPhase;
			foreach (Fixture fixture2 in FixtureList)
			{
				fixture2.DestroyProxies(broadPhase2);
			}
			ContactEdge contactEdge = ContactList;
			while (contactEdge != null)
			{
				ContactEdge contactEdge2 = contactEdge;
				contactEdge = contactEdge.Next;
				World.ContactManager.Destroy(contactEdge2.Contact);
			}
			ContactList = null;
		}
	}

	public bool FixedRotation
	{
		get
		{
			return (Flags & BodyFlags.FixedRotation) == BodyFlags.FixedRotation;
		}
		set
		{
			if (value)
			{
				Flags |= BodyFlags.FixedRotation;
			}
			else
			{
				Flags &= ~BodyFlags.FixedRotation;
			}
			ResetMassData();
		}
	}

	public List<Fixture> FixtureList { get; internal set; }

	public JointEdge JointList { get; internal set; }

	public ContactEdge ContactList { get; internal set; }

	public object UserData { get; set; }

	public Vector2 Position
	{
		get
		{
			return Xf.Position;
		}
		set
		{
			SetTransform(ref value, Rotation);
		}
	}

	public float Rotation
	{
		get
		{
			return Sweep.a;
		}
		set
		{
			SetTransform(ref Xf.Position, value);
		}
	}

	public bool IsStatic
	{
		get
		{
			return Type == BodyType.Static;
		}
		set
		{
			if (value)
			{
				Type = BodyType.Static;
			}
		}
	}

	public bool IgnoreGravity
	{
		get
		{
			return (Flags & BodyFlags.IgnoreGravity) == BodyFlags.IgnoreGravity;
		}
		set
		{
			if (value)
			{
				Flags |= BodyFlags.IgnoreGravity;
			}
			else
			{
				Flags &= ~BodyFlags.IgnoreGravity;
			}
		}
	}

	public Vector2 WorldCenter => Sweep.c;

	public Vector2 LocalCenter
	{
		get
		{
			return Sweep.LocalCenter;
		}
		set
		{
			if (!World.IsLocked && Type == BodyType.Dynamic)
			{
				Vector2 c = Sweep.c;
				Sweep.LocalCenter = value;
				Sweep.c0 = (Sweep.c = MathUtils.Multiply(ref Xf, Sweep.LocalCenter));
				LinearVelocityInternal += MathUtils.Cross(AngularVelocityInternal, Sweep.c - c);
			}
		}
	}

	public float Mass
	{
		get
		{
			return _mass;
		}
		set
		{
			if (!World.IsLocked && Type == BodyType.Dynamic)
			{
				_mass = value;
				if (_mass <= 0f)
				{
					_mass = 1f;
				}
				InvMass = 1f / _mass;
			}
		}
	}

	public float Inertia
	{
		get
		{
			return _inertia + Mass * Vector2.Dot(Sweep.LocalCenter, Sweep.LocalCenter);
		}
		set
		{
			if (!World.IsLocked && Type == BodyType.Dynamic && value > 0f && (Flags & BodyFlags.FixedRotation) == 0)
			{
				_inertia = value - Mass * Vector2.Dot(LocalCenter, LocalCenter);
				InvI = 1f / _inertia;
			}
		}
	}

	internal Body(World world)
	{
		FixtureList = new List<Fixture>(32);
		World = world;
		FixedRotation = false;
		IsBullet = false;
		SleepingAllowed = true;
		Awake = true;
		BodyType = BodyType.Static;
		Active = true;
		Xf.R.Set(0f);
	}

	public Fixture CreateFixture(Shape shape, float density)
	{
		if (World.IsLocked)
		{
			return null;
		}
		Fixture fixture = new Fixture(this, shape, density);
		if ((Flags & BodyFlags.Active) == BodyFlags.Active)
		{
			BroadPhase broadPhase = World.ContactManager.BroadPhase;
			fixture.CreateProxies(broadPhase, ref Xf);
		}
		FixtureList.Add(fixture);
		fixture.Body = this;
		if (fixture.Density > 0f)
		{
			ResetMassData();
		}
		World.Flags |= WorldFlags.NewFixture;
		if (World.FixtureAdded != null)
		{
			World.FixtureAdded(fixture);
		}
		return fixture;
	}

	public Fixture CreateFixture(Shape shape)
	{
		return CreateFixture(shape, 1f);
	}

	public void DestroyFixture(Fixture fixture)
	{
		if (World.IsLocked)
		{
			return;
		}
		ContactEdge contactEdge = ContactList;
		while (contactEdge != null)
		{
			Contact contact = contactEdge.Contact;
			contactEdge = contactEdge.Next;
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			if (fixture == fixtureA || fixture == fixtureB)
			{
				World.ContactManager.Destroy(contact);
			}
		}
		if ((Flags & BodyFlags.Active) == BodyFlags.Active)
		{
			BroadPhase broadPhase = World.ContactManager.BroadPhase;
			fixture.DestroyProxies(broadPhase);
		}
		FixtureList.Remove(fixture);
		fixture.Destroy();
		fixture.Body = null;
		ResetMassData();
	}

	public void SetTransform(ref Vector2 position, float rotation)
	{
		SetTransformIgnoreContacts(ref position, rotation);
		World.ContactManager.FindNewContacts();
	}

	public void SetTransform(Vector2 position, float rotation)
	{
		SetTransform(ref position, rotation);
	}

	public void SetTransformIgnoreContacts(ref Vector2 position, float angle)
	{
		if (World.IsLocked)
		{
			return;
		}
		Xf.R.Set(angle);
		Xf.Position = position;
		Sweep.c0 = (Sweep.c = MathUtils.Multiply(ref Xf, Sweep.LocalCenter));
		Sweep.a0 = (Sweep.a = angle);
		BroadPhase broadPhase = World.ContactManager.BroadPhase;
		foreach (Fixture fixture in FixtureList)
		{
			fixture.Synchronize(broadPhase, ref Xf, ref Xf);
		}
	}

	public void GetTransform(out Transform transform)
	{
		transform = Xf;
	}

	public void ApplyForce(Vector2 force, Vector2 point)
	{
		ApplyForce(ref force, ref point);
	}

	public void ApplyForce(ref Vector2 force)
	{
		ApplyForce(ref force, ref Xf.Position);
	}

	public void ApplyForce(Vector2 force)
	{
		ApplyForce(ref force, ref Xf.Position);
	}

	public void ApplyForce(ref Vector2 force, ref Vector2 point)
	{
		if (Type == BodyType.Dynamic)
		{
			if (!Awake)
			{
				Awake = true;
			}
			Force += force;
			Torque += MathUtils.Cross(point - Sweep.c, force);
		}
	}

	public void ApplyTorque(float torque)
	{
		if (Type == BodyType.Dynamic)
		{
			if (!Awake)
			{
				Awake = true;
			}
			Torque += torque;
		}
	}

	public void ApplyLinearImpulse(Vector2 impulse, Vector2 point)
	{
		if (Type == BodyType.Dynamic)
		{
			if (!Awake)
			{
				Awake = true;
			}
			LinearVelocityInternal += InvMass * impulse;
			AngularVelocityInternal += InvI * MathUtils.Cross(point - Sweep.c, impulse);
		}
	}

	public void ApplyAngularImpulse(float impulse)
	{
		if (Type == BodyType.Dynamic)
		{
			if (!Awake)
			{
				Awake = true;
			}
			AngularVelocityInternal += InvI * impulse;
		}
	}

	public void ResetMassData()
	{
		_mass = 0f;
		InvMass = 0f;
		_inertia = 0f;
		InvI = 0f;
		Sweep.LocalCenter = Vector2.Zero;
		if (BodyType == BodyType.Kinematic)
		{
			Sweep.c0 = (Sweep.c = Xf.Position);
			return;
		}
		Vector2 zero = Vector2.Zero;
		foreach (Fixture fixture in FixtureList)
		{
			if (!MathUtils.FloatEquals(fixture.Density, 0f))
			{
				fixture.GetMassData(out var massData);
				_mass += massData.Mass;
				zero += massData.Mass * massData.Center;
				_inertia += massData.Inertia;
			}
		}
		if (BodyType == BodyType.Static)
		{
			Sweep.c0 = (Sweep.c = Xf.Position);
			return;
		}
		if (_mass > 0f)
		{
			InvMass = 1f / _mass;
			zero *= InvMass;
		}
		else
		{
			_mass = 1f;
			InvMass = 1f;
		}
		if (_inertia > 0f && (Flags & BodyFlags.FixedRotation) == 0)
		{
			_inertia -= _mass * Vector2.Dot(zero, zero);
			InvI = 1f / _inertia;
		}
		else
		{
			_inertia = 0f;
			InvI = 0f;
		}
		Vector2 c = Sweep.c;
		Sweep.LocalCenter = zero;
		Sweep.c0 = (Sweep.c = MathUtils.Multiply(ref Xf, Sweep.LocalCenter));
		LinearVelocityInternal += MathUtils.Cross(AngularVelocityInternal, Sweep.c - c);
	}

	public Vector2 GetWorldPoint(ref Vector2 localPoint)
	{
		return MathUtils.Multiply(ref Xf, ref localPoint);
	}

	public Vector2 GetWorldPoint(Vector2 localPoint)
	{
		return GetWorldPoint(ref localPoint);
	}

	public Vector2 GetWorldVector(ref Vector2 localVector)
	{
		return MathUtils.Multiply(ref Xf.R, ref localVector);
	}

	public Vector2 GetWorldVector(Vector2 localVector)
	{
		return GetWorldVector(ref localVector);
	}

	public Vector2 GetLocalPoint(ref Vector2 worldPoint)
	{
		return MathUtils.MultiplyT(ref Xf, ref worldPoint);
	}

	public Vector2 GetLocalPoint(Vector2 worldPoint)
	{
		return GetLocalPoint(ref worldPoint);
	}

	public Vector2 GetLocalVector(ref Vector2 worldVector)
	{
		return MathUtils.MultiplyT(ref Xf.R, ref worldVector);
	}

	public Vector2 GetLocalVector(Vector2 worldVector)
	{
		return GetLocalVector(ref worldVector);
	}

	public Vector2 GetLinearVelocityFromWorldPoint(Vector2 worldPoint)
	{
		return GetLinearVelocityFromWorldPoint(ref worldPoint);
	}

	public Vector2 GetLinearVelocityFromWorldPoint(ref Vector2 worldPoint)
	{
		return LinearVelocityInternal + MathUtils.Cross(AngularVelocityInternal, worldPoint - Sweep.c);
	}

	public Vector2 GetLinearVelocityFromLocalPoint(Vector2 localPoint)
	{
		return GetLinearVelocityFromLocalPoint(ref localPoint);
	}

	public Vector2 GetLinearVelocityFromLocalPoint(ref Vector2 localPoint)
	{
		return GetLinearVelocityFromWorldPoint(GetWorldPoint(ref localPoint));
	}

	internal void SynchronizeFixtures()
	{
		Transform transform = default(Transform);
		transform.R.Set(Sweep.a0);
		transform.Position = Sweep.c0 - MathUtils.Multiply(ref transform.R, ref Sweep.LocalCenter);
		BroadPhase broadPhase = World.ContactManager.BroadPhase;
		foreach (Fixture fixture in FixtureList)
		{
			fixture.Synchronize(broadPhase, ref transform, ref Xf);
		}
	}

	internal void SynchronizeTransform()
	{
		Xf.R.Set(Sweep.a);
		Xf.Position = Sweep.c - MathUtils.Multiply(ref Xf.R, ref Sweep.LocalCenter);
	}

	internal bool ShouldCollide(Body other)
	{
		if (Type != BodyType.Dynamic && other.Type != BodyType.Dynamic)
		{
			return false;
		}
		for (JointEdge jointEdge = JointList; jointEdge != null; jointEdge = jointEdge.Next)
		{
			if (jointEdge.Other == other && !jointEdge.Joint.CollideConnected)
			{
				return false;
			}
		}
		return true;
	}

	internal void Advance(float t)
	{
		Sweep.Advance(t);
		Sweep.c = Sweep.c0;
		Sweep.a = Sweep.a0;
		SynchronizeTransform();
	}
}
