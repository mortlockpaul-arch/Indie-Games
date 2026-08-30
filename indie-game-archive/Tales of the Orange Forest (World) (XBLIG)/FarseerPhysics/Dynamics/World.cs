using System;
using System.Collections.Generic;
using System.Diagnostics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics;

public class World
{
	public BodyDelegate BodyAdded;

	public BodyDelegate BodyRemoved;

	internal Queue<Contact> ContactPool = new Queue<Contact>(256);

	public FixtureDelegate FixtureAdded;

	public FixtureDelegate FixtureRemoved;

	internal WorldFlags Flags;

	public JointDelegate JointAdded;

	public JointDelegate JointRemoved;

	private float _invDt0;

	private Island _island = new Island();

	private Func<FixtureProxy, bool> _queryAABBCallback;

	private Func<int, bool> _queryAABBCallbackWrapper;

	private RayCastCallback _rayCastCallback;

	private RayCastCallbackInternal _rayCastCallbackWrapper;

	private Body[] _stack = new Body[64];

	private Contact[] _toiContacts = new Contact[32];

	private TOISolver _toiSolver = new TOISolver();

	private Stopwatch _watch = new Stopwatch();

	public List<Controller> Controllers { get; private set; }

	public List<BreakableBody> BreakableBodyList { get; private set; }

	public float UpdateTime { get; private set; }

	public float ContinuousPhysicsTime { get; private set; }

	public float NewContactsTime { get; private set; }

	public float ControllersUpdateTime { get; private set; }

	public float ContactsUpdateTime { get; private set; }

	public float SolveUpdateTime { get; private set; }

	public float BreakableBodyTime { get; private set; }

	public int ProxyCount => ContactManager.BroadPhase.ProxyCount;

	public int ContactCount => ContactManager.ContactCount;

	public Vector2 Gravity { get; set; }

	public bool IsLocked
	{
		get
		{
			return (Flags & WorldFlags.Locked) == WorldFlags.Locked;
		}
		set
		{
			if (value)
			{
				Flags |= WorldFlags.Locked;
			}
			else
			{
				Flags &= ~WorldFlags.Locked;
			}
		}
	}

	public bool AutoClearForces
	{
		get
		{
			return (Flags & WorldFlags.ClearForces) == WorldFlags.ClearForces;
		}
		set
		{
			if (value)
			{
				Flags |= WorldFlags.ClearForces;
			}
			else
			{
				Flags &= ~WorldFlags.ClearForces;
			}
		}
	}

	public ContactManager ContactManager { get; private set; }

	public List<Body> BodyList { get; private set; }

	public List<Joint> JointList { get; private set; }

	public Contact ContactList => ContactManager.ContactList;

	public World(Vector2 gravity)
	{
		ContactManager = new ContactManager();
		Gravity = gravity;
		Flags = WorldFlags.ClearForces;
		_queryAABBCallbackWrapper = QueryAABBCallbackWrapper;
		_rayCastCallbackWrapper = RayCastCallbackWrapper;
		new DefaultContactFilter(this);
		Controllers = new List<Controller>();
		BreakableBodyList = new List<BreakableBody>();
		BodyList = new List<Body>(32);
		JointList = new List<Joint>(32);
	}

	public Body CreateBody()
	{
		if (IsLocked)
		{
			return null;
		}
		Body body = new Body(this);
		BodyList.Add(body);
		if (BodyAdded != null)
		{
			BodyAdded(body);
		}
		return body;
	}

	public void RemoveBody(Body body)
	{
		if (IsLocked)
		{
			return;
		}
		JointEdge jointEdge = body.JointList;
		while (jointEdge != null)
		{
			JointEdge jointEdge2 = jointEdge;
			jointEdge = jointEdge.Next;
			RemoveJoint(jointEdge2.Joint);
		}
		body.JointList = null;
		ContactEdge contactEdge = body.ContactList;
		while (contactEdge != null)
		{
			ContactEdge contactEdge2 = contactEdge;
			contactEdge = contactEdge.Next;
			ContactManager.Destroy(contactEdge2.Contact);
		}
		body.ContactList = null;
		foreach (Fixture fixture in body.FixtureList)
		{
			fixture.DestroyProxies(ContactManager.BroadPhase);
			fixture.Destroy();
		}
		body.FixtureList = null;
		BodyList.Remove(body);
		if (BodyRemoved != null)
		{
			BodyRemoved(body);
		}
	}

	public void AddJoint(Joint joint)
	{
		if (IsLocked)
		{
			return;
		}
		JointList.Add(joint);
		joint.EdgeA.Joint = joint;
		joint.EdgeA.Other = joint.BodyB;
		joint.EdgeA.Prev = null;
		joint.EdgeA.Next = joint.BodyA.JointList;
		if (joint.BodyA.JointList != null)
		{
			joint.BodyA.JointList.Prev = joint.EdgeA;
		}
		joint.BodyA.JointList = joint.EdgeA;
		if (!joint.IsFixedType())
		{
			joint.EdgeB.Joint = joint;
			joint.EdgeB.Other = joint.BodyA;
			joint.EdgeB.Prev = null;
			joint.EdgeB.Next = joint.BodyB.JointList;
			if (joint.BodyB.JointList != null)
			{
				joint.BodyB.JointList.Prev = joint.EdgeB;
			}
			joint.BodyB.JointList = joint.EdgeB;
		}
		if (!joint.IsFixedType())
		{
			Body bodyA = joint.BodyA;
			Body bodyB = joint.BodyB;
			if (!joint.CollideConnected)
			{
				for (ContactEdge contactEdge = bodyB.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
				{
					if (contactEdge.Other == bodyA)
					{
						contactEdge.Contact.FlagForFiltering();
					}
				}
			}
		}
		if (JointAdded != null)
		{
			JointAdded(joint);
		}
	}

	public void RemoveJoint(Joint joint)
	{
		if (IsLocked)
		{
			return;
		}
		bool collideConnected = joint.CollideConnected;
		JointList.Remove(joint);
		Body bodyA = joint.BodyA;
		Body bodyB = joint.BodyB;
		bodyA.Awake = true;
		if (!joint.IsFixedType())
		{
			bodyB.Awake = true;
		}
		if (joint.EdgeA.Prev != null)
		{
			joint.EdgeA.Prev.Next = joint.EdgeA.Next;
		}
		if (joint.EdgeA.Next != null)
		{
			joint.EdgeA.Next.Prev = joint.EdgeA.Prev;
		}
		if (joint.EdgeA == bodyA.JointList)
		{
			bodyA.JointList = joint.EdgeA.Next;
		}
		joint.EdgeA.Prev = null;
		joint.EdgeA.Next = null;
		if (!joint.IsFixedType())
		{
			if (joint.EdgeB.Prev != null)
			{
				joint.EdgeB.Prev.Next = joint.EdgeB.Next;
			}
			if (joint.EdgeB.Next != null)
			{
				joint.EdgeB.Next.Prev = joint.EdgeB.Prev;
			}
			if (joint.EdgeB == bodyB.JointList)
			{
				bodyB.JointList = joint.EdgeB.Next;
			}
			joint.EdgeB.Prev = null;
			joint.EdgeB.Next = null;
		}
		if (!joint.IsFixedType() && !collideConnected)
		{
			for (ContactEdge contactEdge = bodyB.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
			{
				if (contactEdge.Other == bodyA)
				{
					contactEdge.Contact.FlagForFiltering();
				}
			}
		}
		if (JointRemoved != null)
		{
			JointRemoved(joint);
		}
	}

	public void Step(float dt)
	{
		if (Settings.EnableDiagnostics)
		{
			_watch.Start();
		}
		if ((Flags & WorldFlags.NewFixture) == WorldFlags.NewFixture)
		{
			ContactManager.FindNewContacts();
			Flags &= ~WorldFlags.NewFixture;
		}
		if (Settings.EnableDiagnostics)
		{
			NewContactsTime = _watch.ElapsedTicks;
		}
		Flags |= WorldFlags.Locked;
		TimeStep step = default(TimeStep);
		step.dt = dt;
		if (dt > 0f)
		{
			step.inv_dt = 1f / dt;
		}
		else
		{
			step.inv_dt = 0f;
		}
		step.dtRatio = _invDt0 * dt;
		foreach (Controller controller in Controllers)
		{
			controller.Update(dt);
		}
		if (Settings.EnableDiagnostics)
		{
			ControllersUpdateTime = (float)_watch.ElapsedTicks - NewContactsTime;
		}
		ContactManager.Collide();
		if (Settings.EnableDiagnostics)
		{
			ContactsUpdateTime = (float)_watch.ElapsedTicks - (NewContactsTime + ControllersUpdateTime);
		}
		if (step.dt > 0f)
		{
			Solve(ref step);
		}
		if (Settings.EnableDiagnostics)
		{
			SolveUpdateTime = (float)_watch.ElapsedTicks - (NewContactsTime + ControllersUpdateTime + ContactsUpdateTime);
		}
		if (Settings.ContinuousPhysics && step.dt > 0f)
		{
			SolveTOI();
		}
		if (Settings.EnableDiagnostics)
		{
			ContinuousPhysicsTime = (float)_watch.ElapsedTicks - (NewContactsTime + ControllersUpdateTime + ContactsUpdateTime + SolveUpdateTime);
		}
		if (step.dt > 0f)
		{
			_invDt0 = step.inv_dt;
		}
		if ((Flags & WorldFlags.ClearForces) != 0)
		{
			ClearForces();
		}
		Flags &= ~WorldFlags.Locked;
		foreach (BreakableBody breakableBody in BreakableBodyList)
		{
			breakableBody.Update();
		}
		if (Settings.EnableDiagnostics)
		{
			BreakableBodyTime = (float)_watch.ElapsedTicks - (NewContactsTime + ControllersUpdateTime + ContactsUpdateTime + SolveUpdateTime + ContinuousPhysicsTime);
		}
		if (Settings.EnableDiagnostics)
		{
			_watch.Stop();
			UpdateTime = _watch.ElapsedTicks;
			_watch.Reset();
		}
	}

	public void ClearForces()
	{
		foreach (Body body in BodyList)
		{
			body.Force = Vector2.Zero;
			body.Torque = 0f;
		}
	}

	public void QueryAABB(Func<FixtureProxy, bool> callback, ref AABB aabb)
	{
		_queryAABBCallback = callback;
		ContactManager.BroadPhase.Query(_queryAABBCallbackWrapper, ref aabb);
		_queryAABBCallback = null;
	}

	private bool QueryAABBCallbackWrapper(int proxyId)
	{
		FixtureProxy userData = ContactManager.BroadPhase.GetUserData<FixtureProxy>(proxyId);
		return _queryAABBCallback(userData);
	}

	public void RayCast(RayCastCallback callback, Vector2 point1, Vector2 point2)
	{
		RayCastInput input = new RayCastInput
		{
			MaxFraction = 1f,
			Point1 = point1,
			Point2 = point2
		};
		_rayCastCallback = callback;
		ContactManager.BroadPhase.RayCast(_rayCastCallbackWrapper, ref input);
		_rayCastCallback = null;
	}

	private float RayCastCallbackWrapper(ref RayCastInput input, int proxyId)
	{
		FixtureProxy userData = ContactManager.BroadPhase.GetUserData<FixtureProxy>(proxyId);
		Fixture fixture = userData.Fixture;
		int childIndex = userData.ChildIndex;
		if (fixture.RayCast(out var output, ref input, childIndex))
		{
			float fraction = output.Fraction;
			Vector2 point = (1f - fraction) * input.Point1 + fraction * input.Point2;
			return _rayCastCallback(fixture, point, output.Normal, fraction);
		}
		return input.MaxFraction;
	}

	private void Solve(ref TimeStep step)
	{
		_island.Reset(BodyList.Count, ContactManager.ContactCount, JointList.Count, ContactManager);
		foreach (Body body4 in BodyList)
		{
			body4.Flags &= ~BodyFlags.Island;
		}
		for (Contact contact = ContactManager.ContactList; contact != null; contact = contact.Next)
		{
			contact.Flags &= ~ContactFlags.Island;
		}
		foreach (Joint joint in JointList)
		{
			joint.IslandFlag = false;
		}
		int count = BodyList.Count;
		if (count > _stack.Length)
		{
			_stack = new Body[Math.Max(_stack.Length * 2, count)];
		}
		for (int num = BodyList.Count - 1; num >= 0; num--)
		{
			Body body = BodyList[num];
			if ((body.Flags & BodyFlags.Island) == 0 && body.Awake && body.Active && body.BodyType != BodyType.Static)
			{
				_island.Clear();
				int num2 = 0;
				_stack[num2++] = body;
				body.Flags |= BodyFlags.Island;
				while (num2 > 0)
				{
					Body body2 = _stack[--num2];
					_island.Add(body2);
					body2.Awake = true;
					if (body2.BodyType == BodyType.Static)
					{
						continue;
					}
					for (ContactEdge contactEdge = body2.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
					{
						Contact contact2 = contactEdge.Contact;
						if ((contact2.Flags & ContactFlags.Island) == 0 && contactEdge.Contact.Enabled && contactEdge.Contact.IsTouching())
						{
							bool isSensor = contact2.FixtureA.IsSensor;
							bool isSensor2 = contact2.FixtureB.IsSensor;
							if (!isSensor && !isSensor2)
							{
								_island.Add(contact2);
								contact2.Flags |= ContactFlags.Island;
								Body other = contactEdge.Other;
								if ((other.Flags & BodyFlags.Island) == 0)
								{
									_stack[num2++] = other;
									other.Flags |= BodyFlags.Island;
								}
							}
						}
					}
					for (JointEdge jointEdge = body2.JointList; jointEdge != null; jointEdge = jointEdge.Next)
					{
						if (!jointEdge.Joint.IslandFlag)
						{
							Body other2 = jointEdge.Other;
							if (other2 != null)
							{
								if (other2.Active)
								{
									_island.Add(jointEdge.Joint);
									jointEdge.Joint.IslandFlag = true;
									if ((other2.Flags & BodyFlags.Island) == 0)
									{
										_stack[num2++] = other2;
										other2.Flags |= BodyFlags.Island;
									}
								}
							}
							else
							{
								_island.Add(jointEdge.Joint);
								jointEdge.Joint.IslandFlag = true;
							}
						}
					}
				}
				_island.Solve(ref step, Gravity);
				for (int i = 0; i < _island.BodyCount; i++)
				{
					Body body3 = _island.Bodies[i];
					if (body3.BodyType == BodyType.Static)
					{
						body3.Flags &= ~BodyFlags.Island;
					}
				}
			}
		}
		foreach (Body body5 in BodyList)
		{
			if ((body5.Flags & BodyFlags.Island) == BodyFlags.Island && body5.BodyType != BodyType.Static)
			{
				body5.SynchronizeFixtures();
			}
		}
		ContactManager.FindNewContacts();
	}

	private void SolveTOI(Body body)
	{
		Contact contact = null;
		float num = 1f;
		Body body2 = null;
		int num2 = 0;
		bool isBullet = body.IsBullet;
		bool flag;
		int num3;
		do
		{
			num3 = 0;
			flag = false;
			for (ContactEdge contactEdge = body.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
			{
				if (contactEdge.Contact == contact)
				{
					continue;
				}
				Body other = contactEdge.Other;
				BodyType bodyType = other.BodyType;
				if (isBullet)
				{
					if ((other.Flags & BodyFlags.Toi) == 0 || (bodyType != BodyType.Static && (contactEdge.Contact.Flags & ContactFlags.BulletHit) != ContactFlags.None))
					{
						continue;
					}
				}
				else if (bodyType == BodyType.Dynamic)
				{
					continue;
				}
				Contact contact2 = contactEdge.Contact;
				if (!contact2.Enabled || contact2.TOICount > 10)
				{
					continue;
				}
				Fixture fixtureA = contact2.FixtureA;
				Fixture fixtureB = contact2.FixtureB;
				int childIndexA = contact2.ChildIndexA;
				int childIndexB = contact2.ChildIndexB;
				if (!fixtureA.IsSensor && !fixtureB.IsSensor)
				{
					Body body3 = fixtureA.Body;
					Body body4 = fixtureB.Body;
					TOIInput input = default(TOIInput);
					input.ProxyA.Set(fixtureA.Shape, childIndexA);
					input.ProxyB.Set(fixtureB.Shape, childIndexB);
					input.SweepA = body3.Sweep;
					input.SweepB = body4.Sweep;
					input.TMax = num;
					TimeOfImpact.CalculateTimeOfImpact(out var output, ref input);
					if (output.State == TOIOutputState.Touching && output.T < num)
					{
						contact = contact2;
						num = output.T;
						body2 = other;
						flag = true;
					}
					num3++;
				}
			}
			num2++;
		}
		while (flag && num3 > 1 && num2 < 50);
		if (contact == null)
		{
			body.Advance(1f);
			return;
		}
		Sweep sweep = body.Sweep;
		body.Advance(num);
		contact.Update(ContactManager);
		if (!contact.Enabled)
		{
			body.Sweep = sweep;
			SolveTOI(body);
		}
		contact.TOICount++;
		num3 = 0;
		ContactEdge contactEdge2 = body.ContactList;
		while (contactEdge2 != null && num3 < 32)
		{
			Body other2 = contactEdge2.Other;
			BodyType bodyType2 = other2.BodyType;
			if (bodyType2 != BodyType.Dynamic)
			{
				Contact contact3 = contactEdge2.Contact;
				if (contact3.Enabled)
				{
					Fixture fixtureA2 = contact3.FixtureA;
					Fixture fixtureB2 = contact3.FixtureB;
					if (!fixtureA2.IsSensor && !fixtureB2.IsSensor)
					{
						if (contact3 != contact)
						{
							contact3.Update(ContactManager);
						}
						if (contact3.Enabled && contact3.IsTouching())
						{
							_toiContacts[num3] = contact3;
							num3++;
						}
					}
				}
			}
			contactEdge2 = contactEdge2.Next;
		}
		_toiSolver.Initialize(_toiContacts, num3, body);
		for (int i = 0; i < 20; i++)
		{
			if (_toiSolver.Solve(0.75f))
			{
				break;
			}
		}
		if (body2.BodyType != BodyType.Static)
		{
			contact.Flags |= ContactFlags.BulletHit;
		}
	}

	private void SolveTOI()
	{
		for (Contact contact = ContactManager.ContactList; contact != null; contact = contact.Next)
		{
			contact.Flags |= ContactFlags.Enabled;
			contact.TOICount = 0;
		}
		foreach (Body body in BodyList)
		{
			if ((body.Flags & BodyFlags.Island) == 0 || body.BodyType == BodyType.Kinematic || body.BodyType == BodyType.Static)
			{
				body.Flags |= BodyFlags.Toi;
			}
			else
			{
				body.Flags &= ~BodyFlags.Toi;
			}
		}
		foreach (Body body2 in BodyList)
		{
			if ((body2.Flags & BodyFlags.Toi) == 0 && !body2.IsBullet)
			{
				SolveTOI(body2);
				body2.Flags |= BodyFlags.Toi;
			}
		}
		foreach (Body body3 in BodyList)
		{
			if ((body3.Flags & BodyFlags.Toi) == 0 && body3.IsBullet)
			{
				SolveTOI(body3);
				body3.Flags |= BodyFlags.Toi;
			}
		}
	}

	public void AddController(Controller controller)
	{
		controller.World = this;
		Controllers.Add(controller);
	}

	public void RemoveController(Controller controller)
	{
		if (Controllers.Contains(controller))
		{
			Controllers.Remove(controller);
		}
	}

	public void AddBreakableBody(BreakableBody breakableBody)
	{
		BreakableBodyList.Add(breakableBody);
	}

	public void RemoveBreakableBody(BreakableBody breakableBody)
	{
		BreakableBodyList.Remove(breakableBody);
	}
}
