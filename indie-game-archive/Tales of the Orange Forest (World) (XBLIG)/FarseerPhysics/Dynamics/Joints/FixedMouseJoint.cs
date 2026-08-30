using System;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class FixedMouseJoint : Joint
{
	private Vector2 _C;

	private float _beta;

	private float _gamma;

	private Vector2 _impulse;

	private Mat22 _mass;

	public Vector2 LocalAnchorA { get; private set; }

	public Vector2 LocalAnchorB { get; private set; }

	public override Vector2 WorldAnchorA => LocalAnchorB;

	public override Vector2 WorldAnchorB => base.BodyA.GetWorldPoint(LocalAnchorA);

	public Vector2 Target
	{
		get
		{
			return LocalAnchorB;
		}
		set
		{
			base.BodyA.Awake = true;
			LocalAnchorB = value;
		}
	}

	public float MaxForce { get; set; }

	public float Frequency { get; set; }

	public float DampingRatio { get; set; }

	public FixedMouseJoint(Body body, Vector2 target)
		: base(body)
	{
		base.JointType = JointType.FixedMouse;
		Frequency = 5f;
		DampingRatio = 0.7f;
		base.BodyA.GetTransform(out var transform);
		LocalAnchorB = target;
		LocalAnchorA = MathUtils.MultiplyT(ref transform, LocalAnchorB);
	}

	public override Vector2 GetReactionForce(float inv_dt)
	{
		return inv_dt * _impulse;
	}

	public override float GetReactionTorque(float inv_dt)
	{
		return inv_dt * 0f;
	}

	internal override void InitVelocityConstraints(ref TimeStep step)
	{
		Body bodyA = base.BodyA;
		float mass = bodyA.Mass;
		float num = (float)Math.PI * 2f * Frequency;
		float num2 = 2f * mass * DampingRatio * num;
		float num3 = mass * (num * num);
		_gamma = step.dt * (num2 + step.dt * num3);
		if (_gamma != 0f)
		{
			_gamma = 1f / _gamma;
		}
		_beta = step.dt * num3 * _gamma;
		bodyA.GetTransform(out var transform);
		Vector2 vector = MathUtils.Multiply(ref transform.R, LocalAnchorA - bodyA.LocalCenter);
		float invMass = bodyA.InvMass;
		float invI = bodyA.InvI;
		Mat22 A = new Mat22(new Vector2(invMass, 0f), new Vector2(0f, invMass));
		Mat22 B = new Mat22(new Vector2(invI * vector.Y * vector.Y, (0f - invI) * vector.X * vector.Y), new Vector2((0f - invI) * vector.X * vector.Y, invI * vector.X * vector.X));
		Mat22.Add(ref A, ref B, out var R);
		R.col1.X += _gamma;
		R.col2.Y += _gamma;
		_mass = R.Inverse;
		_C = bodyA.Sweep.c + vector - LocalAnchorB;
		bodyA.AngularVelocityInternal *= 0.98f;
		_impulse *= step.dtRatio;
		bodyA.LinearVelocityInternal += invMass * _impulse;
		bodyA.AngularVelocityInternal += invI * MathUtils.Cross(vector, _impulse);
	}

	internal override void SolveVelocityConstraints(ref TimeStep step)
	{
		Body bodyA = base.BodyA;
		bodyA.GetTransform(out var transform);
		Vector2 a = MathUtils.Multiply(ref transform.R, LocalAnchorA - bodyA.LocalCenter);
		Vector2 vector = bodyA.LinearVelocityInternal + MathUtils.Cross(bodyA.AngularVelocityInternal, a);
		Vector2 vector2 = MathUtils.Multiply(ref _mass, -(vector + _beta * _C + _gamma * _impulse));
		Vector2 impulse = _impulse;
		_impulse += vector2;
		float num = step.dt * MaxForce;
		if (_impulse.LengthSquared() > num * num)
		{
			_impulse *= num / _impulse.Length();
		}
		vector2 = _impulse - impulse;
		bodyA.LinearVelocityInternal += bodyA.InvMass * vector2;
		bodyA.AngularVelocityInternal += bodyA.InvI * MathUtils.Cross(a, vector2);
	}

	internal override bool SolvePositionConstraints()
	{
		return true;
	}
}
