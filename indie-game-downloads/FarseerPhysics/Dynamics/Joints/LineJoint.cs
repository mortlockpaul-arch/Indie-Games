using System;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class LineJoint : Joint
{
	public Vector2 LocalAnchorA;

	public Vector2 LocalAnchorB;

	private Mat22 _K;

	private float _a1;

	private float _a2;

	private Vector2 _axis;

	private bool _enableLimit;

	private bool _enableMotor;

	private Vector2 _impulse;

	private LimitState _limitState;

	private Vector2 _localXAxis1;

	private Vector2 _localYAxis1;

	private float _lowerLimit;

	private float _maxMotorForce;

	private float _motorImpulse;

	private float _motorMass;

	private float _motorSpeed;

	private Vector2 _perp;

	private float _s1;

	private float _s2;

	private float _upperLimit;

	public override Vector2 WorldAnchorA => base.BodyA.GetWorldPoint(LocalAnchorA);

	public override Vector2 WorldAnchorB
	{
		get
		{
			return base.BodyB.GetWorldPoint(LocalAnchorB);
		}
		set
		{
		}
	}

	public bool EnableLimit
	{
		get
		{
			return _enableLimit;
		}
		set
		{
			WakeBodies();
			_enableLimit = value;
		}
	}

	public float LowerLimit
	{
		get
		{
			return _lowerLimit;
		}
		set
		{
			WakeBodies();
			_lowerLimit = value;
		}
	}

	public float UpperLimit
	{
		get
		{
			return _upperLimit;
		}
		set
		{
			WakeBodies();
			_upperLimit = value;
		}
	}

	public bool MotorEnabled
	{
		get
		{
			return _enableMotor;
		}
		set
		{
			WakeBodies();
			_enableMotor = value;
		}
	}

	public float MotorSpeed
	{
		get
		{
			return _motorSpeed;
		}
		set
		{
			WakeBodies();
			_motorSpeed = value;
		}
	}

	public float MaxMotorForce
	{
		get
		{
			return _maxMotorForce;
		}
		set
		{
			WakeBodies();
			_maxMotorForce = value;
		}
	}

	public float MotorForce
	{
		get
		{
			return _motorImpulse;
		}
		set
		{
			_motorImpulse = value;
		}
	}

	public float JointTranslation
	{
		get
		{
			Vector2 value = base.BodyB.GetWorldPoint(LocalAnchorB) - base.BodyA.GetWorldPoint(LocalAnchorA);
			Vector2 worldVector = base.BodyA.GetWorldVector(ref _localXAxis1);
			return Vector2.Dot(value, worldVector);
		}
	}

	public float JointSpeed
	{
		get
		{
			base.BodyA.GetTransform(out var transform);
			base.BodyB.GetTransform(out var transform2);
			Vector2 vector = MathUtils.Multiply(ref transform.R, LocalAnchorA - base.BodyA.LocalCenter);
			Vector2 vector2 = MathUtils.Multiply(ref transform2.R, LocalAnchorB - base.BodyB.LocalCenter);
			Vector2 vector3 = base.BodyA.Sweep.C + vector;
			Vector2 vector4 = base.BodyB.Sweep.C + vector2;
			Vector2 value = vector4 - vector3;
			Vector2 worldVector = base.BodyA.GetWorldVector(ref _localXAxis1);
			Vector2 linearVelocityInternal = base.BodyA.LinearVelocityInternal;
			Vector2 linearVelocityInternal2 = base.BodyB.LinearVelocityInternal;
			float angularVelocityInternal = base.BodyA.AngularVelocityInternal;
			float angularVelocityInternal2 = base.BodyB.AngularVelocityInternal;
			return Vector2.Dot(value, MathUtils.Cross(angularVelocityInternal, worldVector)) + Vector2.Dot(worldVector, linearVelocityInternal2 + MathUtils.Cross(angularVelocityInternal2, vector2) - linearVelocityInternal - MathUtils.Cross(angularVelocityInternal, vector));
		}
	}

	public LineJoint(Body bodyA, Body bodyB, Vector2 localAnchorA, Vector2 localAnchorB, Vector2 axis)
		: base(bodyA, bodyB)
	{
		base.JointType = JointType.Line;
		base.BodyA = bodyA;
		base.BodyB = bodyB;
		LocalAnchorA = localAnchorA;
		LocalAnchorB = localAnchorB;
		_localXAxis1 = bodyA.GetLocalVector(axis);
		_localYAxis1 = MathUtils.Cross(1f, _localXAxis1);
		_localYAxis1 = MathUtils.Cross(1f, _localXAxis1);
		_limitState = LimitState.Inactive;
	}

	public override Vector2 GetReactionForce(float inv_dt)
	{
		return inv_dt * (_impulse.X * _perp + (_motorImpulse + _impulse.Y) * _axis);
	}

	public override float GetReactionTorque(float inv_dt)
	{
		return 0f;
	}

	internal override void InitVelocityConstraints(ref TimeStep step)
	{
		Body bodyA = base.BodyA;
		Body bodyB = base.BodyB;
		LocalCenterA = bodyA.LocalCenter;
		LocalCenterB = bodyB.LocalCenter;
		bodyA.GetTransform(out var transform);
		bodyB.GetTransform(out var transform2);
		Vector2 vector = MathUtils.Multiply(ref transform.R, LocalAnchorA - LocalCenterA);
		Vector2 vector2 = MathUtils.Multiply(ref transform2.R, LocalAnchorB - LocalCenterB);
		Vector2 vector3 = bodyB.Sweep.C + vector2 - bodyA.Sweep.C - vector;
		InvMassA = bodyA.InvMass;
		InvIA = bodyA.InvI;
		InvMassB = bodyB.InvMass;
		InvIB = bodyB.InvI;
		_axis = MathUtils.Multiply(ref transform.R, _localXAxis1);
		_a1 = MathUtils.Cross(vector3 + vector, _axis);
		_a2 = MathUtils.Cross(vector2, _axis);
		_motorMass = InvMassA + InvMassB + InvIA * _a1 * _a1 + InvIB * _a2 * _a2;
		if (_motorMass > 1.1920929E-07f)
		{
			_motorMass = 1f / _motorMass;
		}
		else
		{
			_motorMass = 0f;
		}
		_perp = MathUtils.Multiply(ref transform.R, _localYAxis1);
		_s1 = MathUtils.Cross(vector3 + vector, _perp);
		_s2 = MathUtils.Cross(vector2, _perp);
		float invMassA = InvMassA;
		float invMassB = InvMassB;
		float invIA = InvIA;
		float invIB = InvIB;
		float x = invMassA + invMassB + invIA * _s1 * _s1 + invIB * _s2 * _s2;
		float num = invIA * _s1 * _a1 + invIB * _s2 * _a2;
		float y = invMassA + invMassB + invIA * _a1 * _a1 + invIB * _a2 * _a2;
		_K.Col1 = new Vector2(x, num);
		_K.Col2 = new Vector2(num, y);
		if (_enableLimit)
		{
			float num2 = Vector2.Dot(_axis, vector3);
			if (Math.Abs(UpperLimit - LowerLimit) < 0.01f)
			{
				_limitState = LimitState.Equal;
			}
			else if (num2 <= LowerLimit)
			{
				if (_limitState != LimitState.AtLower)
				{
					_limitState = LimitState.AtLower;
					_impulse.Y = 0f;
				}
			}
			else if (num2 >= UpperLimit)
			{
				if (_limitState != LimitState.AtUpper)
				{
					_limitState = LimitState.AtUpper;
					_impulse.Y = 0f;
				}
			}
			else
			{
				_limitState = LimitState.Inactive;
				_impulse.Y = 0f;
			}
		}
		else
		{
			_limitState = LimitState.Inactive;
		}
		if (!_enableMotor)
		{
			_motorImpulse = 0f;
		}
		if (Settings.EnableWarmstarting)
		{
			_impulse *= step.dtRatio;
			_motorImpulse *= step.dtRatio;
			Vector2 vector4 = _impulse.X * _perp + (_motorImpulse + _impulse.Y) * _axis;
			float num3 = _impulse.X * _s1 + (_motorImpulse + _impulse.Y) * _a1;
			float num4 = _impulse.X * _s2 + (_motorImpulse + _impulse.Y) * _a2;
			bodyA.LinearVelocityInternal -= InvMassA * vector4;
			bodyA.AngularVelocityInternal -= InvIA * num3;
			bodyB.LinearVelocityInternal += InvMassB * vector4;
			bodyB.AngularVelocityInternal += InvIB * num4;
		}
		else
		{
			_impulse = Vector2.Zero;
			_motorImpulse = 0f;
		}
	}

	internal override void SolveVelocityConstraints(ref TimeStep step)
	{
		Body bodyA = base.BodyA;
		Body bodyB = base.BodyB;
		Vector2 linearVelocityInternal = bodyA.LinearVelocityInternal;
		float num = bodyA.AngularVelocityInternal;
		Vector2 linearVelocityInternal2 = bodyB.LinearVelocityInternal;
		float num2 = bodyB.AngularVelocityInternal;
		if (_enableMotor && _limitState != LimitState.Equal)
		{
			float num3 = Vector2.Dot(_axis, linearVelocityInternal2 - linearVelocityInternal) + _a2 * num2 - _a1 * num;
			float num4 = _motorMass * (_motorSpeed - num3);
			float motorImpulse = _motorImpulse;
			float num5 = step.dt * _maxMotorForce;
			_motorImpulse = MathUtils.Clamp(_motorImpulse + num4, 0f - num5, num5);
			num4 = _motorImpulse - motorImpulse;
			Vector2 vector = num4 * _axis;
			float num6 = num4 * _a1;
			float num7 = num4 * _a2;
			linearVelocityInternal -= InvMassA * vector;
			num -= InvIA * num6;
			linearVelocityInternal2 += InvMassB * vector;
			num2 += InvIB * num7;
		}
		float num8 = Vector2.Dot(_perp, linearVelocityInternal2 - linearVelocityInternal) + _s2 * num2 - _s1 * num;
		if (_enableLimit && _limitState != LimitState.Inactive)
		{
			float y = Vector2.Dot(_axis, linearVelocityInternal2 - linearVelocityInternal) + _a2 * num2 - _a1 * num;
			Vector2 vector2 = new Vector2(num8, y);
			Vector2 impulse = _impulse;
			Vector2 vector3 = _K.Solve(-vector2);
			_impulse += vector3;
			if (_limitState == LimitState.AtLower)
			{
				_impulse.Y = Math.Max(_impulse.Y, 0f);
			}
			else if (_limitState == LimitState.AtUpper)
			{
				_impulse.Y = Math.Min(_impulse.Y, 0f);
			}
			float num9 = 0f - num8 - (_impulse.Y - impulse.Y) * _K.Col2.X;
			float x = ((_K.Col1.X == 0f) ? impulse.X : (num9 / _K.Col1.X + impulse.X));
			_impulse.X = x;
			vector3 = _impulse - impulse;
			Vector2 vector4 = vector3.X * _perp + vector3.Y * _axis;
			float num10 = vector3.X * _s1 + vector3.Y * _a1;
			float num11 = vector3.X * _s2 + vector3.Y * _a2;
			linearVelocityInternal -= InvMassA * vector4;
			num -= InvIA * num10;
			linearVelocityInternal2 += InvMassB * vector4;
			num2 += InvIB * num11;
		}
		else
		{
			float num12 = ((_K.Col1.X == 0f) ? 0f : ((0f - num8) / _K.Col1.X));
			_impulse.X += num12;
			Vector2 vector5 = num12 * _perp;
			float num13 = num12 * _s1;
			float num14 = num12 * _s2;
			linearVelocityInternal -= InvMassA * vector5;
			num -= InvIA * num13;
			linearVelocityInternal2 += InvMassB * vector5;
			num2 += InvIB * num14;
		}
		bodyA.LinearVelocityInternal = linearVelocityInternal;
		bodyA.AngularVelocityInternal = num;
		bodyB.LinearVelocityInternal = linearVelocityInternal2;
		bodyB.AngularVelocityInternal = num2;
	}

	internal override bool SolvePositionConstraints()
	{
		Body bodyA = base.BodyA;
		Body bodyB = base.BodyB;
		Vector2 c = bodyA.Sweep.C;
		float a = bodyA.Sweep.A;
		Vector2 c2 = bodyB.Sweep.C;
		float a2 = bodyB.Sweep.A;
		float val = 0f;
		bool flag = false;
		float num = 0f;
		Mat22 A = new Mat22(a);
		Mat22 A2 = new Mat22(a2);
		Vector2 vector = MathUtils.Multiply(ref A, LocalAnchorA - LocalCenterA);
		Vector2 vector2 = MathUtils.Multiply(ref A2, LocalAnchorB - LocalCenterB);
		Vector2 vector3 = c2 + vector2 - c - vector;
		if (_enableLimit)
		{
			_axis = MathUtils.Multiply(ref A, _localXAxis1);
			_a1 = MathUtils.Cross(vector3 + vector, _axis);
			_a2 = MathUtils.Cross(vector2, _axis);
			float num2 = Vector2.Dot(_axis, vector3);
			if (Math.Abs(UpperLimit - LowerLimit) < 0.01f)
			{
				num = MathUtils.Clamp(num2, -0.2f, 0.2f);
				val = Math.Abs(num2);
				flag = true;
			}
			else if (num2 <= LowerLimit)
			{
				num = MathUtils.Clamp(num2 - LowerLimit + 0.005f, -0.2f, 0f);
				val = LowerLimit - num2;
				flag = true;
			}
			else if (num2 >= UpperLimit)
			{
				num = MathUtils.Clamp(num2 - UpperLimit - 0.005f, 0f, 0.2f);
				val = num2 - UpperLimit;
				flag = true;
			}
		}
		_perp = MathUtils.Multiply(ref A, _localYAxis1);
		_s1 = MathUtils.Cross(vector3 + vector, _perp);
		_s2 = MathUtils.Cross(vector2, _perp);
		float num3 = Vector2.Dot(_perp, vector3);
		val = Math.Max(val, Math.Abs(num3));
		Vector2 vector4 = default(Vector2);
		if (flag)
		{
			float invMassA = InvMassA;
			float invMassB = InvMassB;
			float invIA = InvIA;
			float invIB = InvIB;
			float x = invMassA + invMassB + invIA * _s1 * _s1 + invIB * _s2 * _s2;
			float num4 = invIA * _s1 * _a1 + invIB * _s2 * _a2;
			float y = invMassA + invMassB + invIA * _a1 * _a1 + invIB * _a2 * _a2;
			_K.Col1 = new Vector2(x, num4);
			_K.Col2 = new Vector2(num4, y);
			Vector2 b = new Vector2(0f - num3, 0f - num);
			vector4 = _K.Solve(b);
		}
		else
		{
			float invMassA2 = InvMassA;
			float invMassB2 = InvMassB;
			float invIA2 = InvIA;
			float invIB2 = InvIB;
			float num5 = invMassA2 + invMassB2 + invIA2 * _s1 * _s1 + invIB2 * _s2 * _s2;
			float x2 = ((num5 == 0f) ? 0f : ((0f - num3) / num5));
			vector4.X = x2;
			vector4.Y = 0f;
		}
		Vector2 vector5 = vector4.X * _perp + vector4.Y * _axis;
		float num6 = vector4.X * _s1 + vector4.Y * _a1;
		float num7 = vector4.X * _s2 + vector4.Y * _a2;
		c -= InvMassA * vector5;
		a -= InvIA * num6;
		c2 += InvMassB * vector5;
		a2 += InvIB * num7;
		bodyA.Sweep.C = c;
		bodyA.Sweep.A = a;
		bodyB.Sweep.C = c2;
		bodyB.Sweep.A = a2;
		bodyA.SynchronizeTransform();
		bodyB.SynchronizeTransform();
		return val <= 0.005f;
	}
}
