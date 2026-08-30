using System;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class WeldJoint : Joint
{
	private Vector3 _impulse;

	private Mat33 _mass;

	public Vector2 LocalAnchorA { get; private set; }

	public Vector2 LocalAnchorB { get; private set; }

	public override Vector2 WorldAnchorA => base.BodyA.GetWorldPoint(LocalAnchorA);

	public override Vector2 WorldAnchorB => base.BodyB.GetWorldPoint(LocalAnchorB);

	public float ReferenceAngle { get; private set; }

	public WeldJoint(Body bodyA, Body bodyB, Vector2 anchorA, Vector2 anchorB)
		: base(bodyA, bodyB)
	{
		base.JointType = JointType.Weld;
		LocalAnchorA = anchorA;
		LocalAnchorB = anchorB;
		ReferenceAngle = base.BodyB.Rotation - base.BodyA.Rotation;
	}

	public override Vector2 GetReactionForce(float inv_dt)
	{
		return inv_dt * new Vector2(_impulse.X, _impulse.Y);
	}

	public override float GetReactionTorque(float inv_dt)
	{
		return inv_dt * _impulse.Z;
	}

	internal override void InitVelocityConstraints(ref TimeStep step)
	{
		Body bodyA = base.BodyA;
		Body bodyB = base.BodyB;
		bodyA.GetTransform(out var transform);
		bodyB.GetTransform(out var transform2);
		Vector2 a = MathUtils.Multiply(ref transform.R, LocalAnchorA - bodyA.LocalCenter);
		Vector2 a2 = MathUtils.Multiply(ref transform2.R, LocalAnchorB - bodyB.LocalCenter);
		float invMass = bodyA.InvMass;
		float invMass2 = bodyB.InvMass;
		float invI = bodyA.InvI;
		float invI2 = bodyB.InvI;
		_mass.col1.X = invMass + invMass2 + a.Y * a.Y * invI + a2.Y * a2.Y * invI2;
		_mass.col2.X = (0f - a.Y) * a.X * invI - a2.Y * a2.X * invI2;
		_mass.col3.X = (0f - a.Y) * invI - a2.Y * invI2;
		_mass.col1.Y = _mass.col2.X;
		_mass.col2.Y = invMass + invMass2 + a.X * a.X * invI + a2.X * a2.X * invI2;
		_mass.col3.Y = a.X * invI + a2.X * invI2;
		_mass.col1.Z = _mass.col3.X;
		_mass.col2.Z = _mass.col3.Y;
		_mass.col3.Z = invI + invI2;
		if (Settings.EnableWarmstarting)
		{
			_impulse *= step.dtRatio;
			Vector2 vector = new Vector2(_impulse.X, _impulse.Y);
			bodyA.LinearVelocityInternal -= invMass * vector;
			bodyA.AngularVelocityInternal -= invI * (MathUtils.Cross(a, vector) + _impulse.Z);
			bodyB.LinearVelocityInternal += invMass2 * vector;
			bodyB.AngularVelocityInternal += invI2 * (MathUtils.Cross(a2, vector) + _impulse.Z);
		}
		else
		{
			_impulse = Vector3.Zero;
		}
	}

	internal override void SolveVelocityConstraints(ref TimeStep step)
	{
		Body bodyA = base.BodyA;
		Body bodyB = base.BodyB;
		Vector2 linearVelocityInternal = bodyA.LinearVelocityInternal;
		float angularVelocityInternal = bodyA.AngularVelocityInternal;
		Vector2 linearVelocityInternal2 = bodyB.LinearVelocityInternal;
		float angularVelocityInternal2 = bodyB.AngularVelocityInternal;
		float invMass = bodyA.InvMass;
		float invMass2 = bodyB.InvMass;
		float invI = bodyA.InvI;
		float invI2 = bodyB.InvI;
		bodyA.GetTransform(out var transform);
		bodyB.GetTransform(out var transform2);
		Vector2 a = MathUtils.Multiply(ref transform.R, LocalAnchorA - bodyA.LocalCenter);
		Vector2 a2 = MathUtils.Multiply(ref transform2.R, LocalAnchorB - bodyB.LocalCenter);
		Vector2 vector = linearVelocityInternal2 + MathUtils.Cross(angularVelocityInternal2, a2) - linearVelocityInternal - MathUtils.Cross(angularVelocityInternal, a);
		float z = angularVelocityInternal2 - angularVelocityInternal;
		Vector3 vector2 = new Vector3(vector.X, vector.Y, z);
		Vector3 vector3 = _mass.Solve33(-vector2);
		_impulse += vector3;
		Vector2 vector4 = new Vector2(vector3.X, vector3.Y);
		linearVelocityInternal -= invMass * vector4;
		angularVelocityInternal -= invI * (MathUtils.Cross(a, vector4) + vector3.Z);
		linearVelocityInternal2 += invMass2 * vector4;
		angularVelocityInternal2 += invI2 * (MathUtils.Cross(a2, vector4) + vector3.Z);
		bodyA.LinearVelocityInternal = linearVelocityInternal;
		bodyA.AngularVelocityInternal = angularVelocityInternal;
		bodyB.LinearVelocityInternal = linearVelocityInternal2;
		bodyB.AngularVelocityInternal = angularVelocityInternal2;
	}

	internal override bool SolvePositionConstraints()
	{
		Body bodyA = base.BodyA;
		Body bodyB = base.BodyB;
		float invMass = bodyA.InvMass;
		float invMass2 = bodyB.InvMass;
		float num = bodyA.InvI;
		float num2 = bodyB.InvI;
		bodyA.GetTransform(out var transform);
		bodyB.GetTransform(out var transform2);
		Vector2 vector = MathUtils.Multiply(ref transform.R, LocalAnchorA - bodyA.LocalCenter);
		Vector2 vector2 = MathUtils.Multiply(ref transform2.R, LocalAnchorB - bodyB.LocalCenter);
		Vector2 vector3 = bodyB.Sweep.c + vector2 - bodyA.Sweep.c - vector;
		float num3 = bodyB.Sweep.a - bodyA.Sweep.a - ReferenceAngle;
		float num4 = vector3.Length();
		float num5 = Math.Abs(num3);
		if (num4 > 0.049999997f)
		{
			num *= 1f;
			num2 *= 1f;
		}
		_mass.col1.X = invMass + invMass2 + vector.Y * vector.Y * num + vector2.Y * vector2.Y * num2;
		_mass.col2.X = (0f - vector.Y) * vector.X * num - vector2.Y * vector2.X * num2;
		_mass.col3.X = (0f - vector.Y) * num - vector2.Y * num2;
		_mass.col1.Y = _mass.col2.X;
		_mass.col2.Y = invMass + invMass2 + vector.X * vector.X * num + vector2.X * vector2.X * num2;
		_mass.col3.Y = vector.X * num + vector2.X * num2;
		_mass.col1.Z = _mass.col3.X;
		_mass.col2.Z = _mass.col3.Y;
		_mass.col3.Z = num + num2;
		Vector3 vector4 = new Vector3(vector3.X, vector3.Y, num3);
		Vector3 vector5 = _mass.Solve33(-vector4);
		Vector2 vector6 = new Vector2(vector5.X, vector5.Y);
		bodyA.Sweep.c -= invMass * vector6;
		bodyA.Sweep.a -= num * (MathUtils.Cross(vector, vector6) + vector5.Z);
		bodyB.Sweep.c += invMass2 * vector6;
		bodyB.Sweep.a += num2 * (MathUtils.Cross(vector2, vector6) + vector5.Z);
		bodyA.SynchronizeTransform();
		bodyB.SynchronizeTransform();
		if (num4 <= 0.005f)
		{
			return num5 <= (float)Math.PI / 90f;
		}
		return false;
	}
}
