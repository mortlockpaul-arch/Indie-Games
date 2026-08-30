using System;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics;

internal class Island
{
	public Body[] Bodies;

	public int BodyCount;

	public int ContactCount;

	public int JointCount;

	private int _bodyCapacity;

	private int _contactCapacity;

	private ContactManager _contactManager;

	private ContactSolver _contactSolver = new ContactSolver();

	private Contact[] _contacts;

	private int _jointCapacity;

	private Joint[] _joints;

	public void Reset(int bodyCapacity, int contactCapacity, int jointCapacity, ContactManager contactManager)
	{
		_bodyCapacity = bodyCapacity;
		_contactCapacity = contactCapacity;
		_jointCapacity = jointCapacity;
		BodyCount = 0;
		ContactCount = 0;
		JointCount = 0;
		_contactManager = contactManager;
		if (Bodies == null || Bodies.Length < bodyCapacity)
		{
			Bodies = new Body[bodyCapacity];
		}
		if (_contacts == null || _contacts.Length < contactCapacity)
		{
			_contacts = new Contact[contactCapacity * 2];
		}
		if (_joints == null || _joints.Length < jointCapacity)
		{
			_joints = new Joint[jointCapacity * 2];
		}
	}

	public void Clear()
	{
		BodyCount = 0;
		ContactCount = 0;
		JointCount = 0;
	}

	public void Solve(ref TimeStep step, Vector2 gravity)
	{
		for (int i = 0; i < BodyCount; i++)
		{
			Body body = Bodies[i];
			if (body.BodyType == BodyType.Dynamic)
			{
				if (body.IgnoreGravity)
				{
					body.LinearVelocityInternal += step.dt * (body.InvMass * body.Force);
					body.AngularVelocityInternal += step.dt * body.InvI * body.Torque;
				}
				else
				{
					body.LinearVelocityInternal += step.dt * (gravity + body.InvMass * body.Force);
					body.AngularVelocityInternal += step.dt * body.InvI * body.Torque;
				}
				body.LinearVelocityInternal *= MathUtils.Clamp(1f - step.dt * body.LinearDamping, 0f, 1f);
				body.AngularVelocityInternal *= MathUtils.Clamp(1f - step.dt * body.AngularDamping, 0f, 1f);
			}
		}
		int num = -1;
		for (int j = 0; j < ContactCount; j++)
		{
			Fixture fixtureA = _contacts[j].FixtureA;
			Fixture fixtureB = _contacts[j].FixtureB;
			Body body2 = fixtureA.Body;
			Body body3 = fixtureB.Body;
			if (body2.BodyType != BodyType.Static && body3.BodyType != BodyType.Static)
			{
				num++;
				Contact contact = _contacts[num];
				_contacts[num] = _contacts[j];
				_contacts[j] = contact;
			}
		}
		_contactSolver.Reset(_contacts, ContactCount, step.dtRatio);
		_contactSolver.WarmStart();
		for (int k = 0; k < JointCount; k++)
		{
			_joints[k].InitVelocityConstraints(ref step);
		}
		for (int l = 0; l < Settings.VelocityIterations; l++)
		{
			for (int m = 0; m < JointCount; m++)
			{
				_joints[m].SolveVelocityConstraints(ref step);
			}
			_contactSolver.SolveVelocityConstraints();
		}
		_contactSolver.StoreImpulses();
		for (int n = 0; n < BodyCount; n++)
		{
			Body body4 = Bodies[n];
			if (body4.BodyType != BodyType.Static)
			{
				Vector2 vector = step.dt * body4.LinearVelocityInternal;
				if (Vector2.Dot(vector, vector) > 4f)
				{
					float num2 = 2f / vector.Length();
					body4.LinearVelocityInternal *= num2;
				}
				float num3 = step.dt * body4.AngularVelocityInternal;
				if (num3 * num3 > 2.4674013f)
				{
					float num4 = (float)Math.PI / 2f / Math.Abs(num3);
					body4.AngularVelocityInternal *= num4;
				}
				body4.Sweep.c0 = body4.Sweep.c;
				body4.Sweep.a0 = body4.Sweep.a;
				body4.Sweep.c += step.dt * body4.LinearVelocityInternal;
				body4.Sweep.a += step.dt * body4.AngularVelocityInternal;
				body4.SynchronizeTransform();
			}
		}
		for (int num5 = 0; num5 < Settings.PositionIterations; num5++)
		{
			bool flag = _contactSolver.SolvePositionConstraints(0.2f);
			bool flag2 = true;
			for (int num6 = 0; num6 < JointCount; num6++)
			{
				bool flag3 = _joints[num6].SolvePositionConstraints();
				flag2 = flag2 && flag3;
			}
			if (flag && flag2)
			{
				break;
			}
		}
		if (_contactManager.PostSolve != null)
		{
			Report(_contactSolver.Constraints);
		}
		if (!Settings.AllowSleep)
		{
			return;
		}
		float num7 = float.MaxValue;
		for (int num8 = 0; num8 < BodyCount; num8++)
		{
			Body body5 = Bodies[num8];
			if (body5.BodyType != BodyType.Static)
			{
				if ((body5.Flags & BodyFlags.AutoSleep) == 0)
				{
					body5.SleepTime = 0f;
					num7 = 0f;
				}
				if ((body5.Flags & BodyFlags.AutoSleep) == 0 || body5.AngularVelocityInternal * body5.AngularVelocityInternal > 0.0012184697f || Vector2.Dot(body5.LinearVelocityInternal, body5.LinearVelocityInternal) > 0.0001f)
				{
					body5.SleepTime = 0f;
					num7 = 0f;
				}
				else
				{
					body5.SleepTime += step.dt;
					num7 = Math.Min(num7, body5.SleepTime);
				}
			}
		}
		if (num7 >= 0.5f)
		{
			for (int num9 = 0; num9 < BodyCount; num9++)
			{
				Body body6 = Bodies[num9];
				body6.Awake = false;
			}
		}
	}

	public void Add(Body body)
	{
		Bodies[BodyCount++] = body;
	}

	public void Add(Contact contact)
	{
		_contacts[ContactCount++] = contact;
	}

	public void Add(Joint joint)
	{
		_joints[JointCount++] = joint;
	}

	private void Report(ContactConstraint[] constraints)
	{
		if (_contactManager == null)
		{
			return;
		}
		for (int i = 0; i < ContactCount; i++)
		{
			Contact contact = _contacts[i];
			ContactConstraint contactConstraint = constraints[i];
			ContactImpulse impulse = default(ContactImpulse);
			for (int j = 0; j < contactConstraint.PointCount; j++)
			{
				impulse.NormalImpulses[j] = contactConstraint.Points[j].NormalImpulse;
				impulse.TangentImpulses[j] = contactConstraint.Points[j].TangentImpulse;
			}
			_contactManager.PostSolve(contact, ref impulse);
		}
	}
}
