using System;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Joints;

public class FixedLineJoint : Joint
{
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

	public override Vector2 WorldAnchorA => LocalAnchorA;

	public override Vector2 WorldAnchorB => base.BodyB.GetWorldPoint(LocalAnchorB);

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

	public Vector2 LocalAnchorA { get; set; }

	public Vector2 LocalAnchorB { get; set; }

	public float JointTranslation
	{
		get
		{
			Vector2 value = base.BodyB.GetWorldPoint(LocalAnchorB) - LocalAnchorA;
			Vector2 localXAxis = _localXAxis1;
			return Vector2.Dot(value, localXAxis);
		}
	}

	public float JointSpeed
	{
		get
		{
			base.BodyB.GetTransform(out var transform);
			Vector2 localAnchorA = LocalAnchorA;
			Vector2 vector = MathUtils.Multiply(ref transform.R, LocalAnchorB - base.BodyB.LocalCenter);
			Vector2 vector2 = localAnchorA;
			Vector2 vector3 = base.BodyB.Sweep.c + vector;
			Vector2 value = vector3 - vector2;
			Vector2 localXAxis = _localXAxis1;
			Vector2 zero = Vector2.Zero;
			Vector2 linearVelocityInternal = base.BodyB.LinearVelocityInternal;
			float angularVelocityInternal = base.BodyB.AngularVelocityInternal;
			return Vector2.Dot(value, MathUtils.Cross(0f, localXAxis)) + Vector2.Dot(localXAxis, linearVelocityInternal + MathUtils.Cross(angularVelocityInternal, vector) - zero - MathUtils.Cross(0f, localAnchorA));
		}
	}

	public FixedLineJoint(Body bodyA, Vector2 anchor, Vector2 axis)
		: base(bodyA)
	{
		base.JointType = JointType.FixedLine;
		base.BodyB = bodyA;
		LocalAnchorA = anchor;
		LocalAnchorB = base.BodyB.GetLocalPoint(anchor);
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
		Body bodyB = base.BodyB;
		LocalCenterA = Vector2.Zero;
		LocalCenterB = bodyB.LocalCenter;
		bodyB.GetTransform(out var transform);
		Vector2 localAnchorA = LocalAnchorA;
		Vector2 vector = MathUtils.Multiply(ref transform.R, LocalAnchorB - LocalCenterB);
		Vector2 vector2 = bodyB.Sweep.c + vector - localAnchorA;
		InvMassA = 0f;
		InvIA = 0f;
		InvMassB = bodyB.InvMass;
		InvIB = bodyB.InvI;
		_axis = _localXAxis1;
		_a1 = MathUtils.Cross(vector2 + localAnchorA, _axis);
		_a2 = MathUtils.Cross(vector, _axis);
		_motorMass = InvMassA + InvMassB + InvIA * _a1 * _a1 + InvIB * _a2 * _a2;
		if (_motorMass > 1.1920929E-07f)
		{
			_motorMass = 1f / _motorMass;
		}
		else
		{
			_motorMass = 0f;
		}
		_perp = _localYAxis1;
		_s1 = MathUtils.Cross(vector2 + localAnchorA, _perp);
		_s2 = MathUtils.Cross(vector, _perp);
		float invMassA = InvMassA;
		float invMassB = InvMassB;
		float invIA = InvIA;
		float invIB = InvIB;
		float x = invMassA + invMassB + invIA * _s1 * _s1 + invIB * _s2 * _s2;
		float num = invIA * _s1 * _a1 + invIB * _s2 * _a2;
		float y = invMassA + invMassB + invIA * _a1 * _a1 + invIB * _a2 * _a2;
		_K.col1 = new Vector2(x, num);
		_K.col2 = new Vector2(num, y);
		if (_enableLimit)
		{
			float num2 = Vector2.Dot(_axis, vector2);
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
			Vector2 vector3 = _impulse.X * _perp + (_motorImpulse + _impulse.Y) * _axis;
			float num3 = _impulse.X * _s2 + (_motorImpulse + _impulse.Y) * _a2;
			bodyB.LinearVelocityInternal += InvMassB * vector3;
			bodyB.AngularVelocityInternal += InvIB * num3;
		}
		else
		{
			_impulse = Vector2.Zero;
			_motorImpulse = 0f;
		}
	}

	internal override void SolveVelocityConstraints(ref TimeStep step)
	{
		Body bodyB = base.BodyB;
		Vector2 zero = Vector2.Zero;
		float num = 0f;
		Vector2 linearVelocityInternal = bodyB.LinearVelocityInternal;
		float num2 = bodyB.AngularVelocityInternal;
		if (_enableMotor && _limitState != LimitState.Equal)
		{
			float num3 = Vector2.Dot(_axis, linearVelocityInternal - zero) + _a2 * num2 - _a1 * num;
			float num4 = _motorMass * (_motorSpeed - num3);
			float motorImpulse = _motorImpulse;
			float num5 = step.dt * _maxMotorForce;
			_motorImpulse = MathUtils.Clamp(_motorImpulse + num4, 0f - num5, num5);
			num4 = _motorImpulse - motorImpulse;
			Vector2 vector = num4 * _axis;
			float num6 = num4 * _a1;
			float num7 = num4 * _a2;
			zero -= InvMassA * vector;
			num -= InvIA * num6;
			linearVelocityInternal += InvMassB * vector;
			num2 += InvIB * num7;
		}
		float num8 = Vector2.Dot(_perp, linearVelocityInternal - zero) + _s2 * num2 - _s1 * num;
		if (_enableLimit && _limitState != LimitState.Inactive)
		{
			float y = Vector2.Dot(_axis, linearVelocityInternal - zero) + _a2 * num2 - _a1 * num;
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
			float num9 = 0f - num8 - (_impulse.Y - impulse.Y) * _K.col2.X;
			float x = ((_K.col1.X == 0f) ? impulse.X : (num9 / _K.col1.X + impulse.X));
			_impulse.X = x;
			vector3 = _impulse - impulse;
			Vector2 vector4 = vector3.X * _perp + vector3.Y * _axis;
			float num10 = vector3.X * _s2 + vector3.Y * _a2;
			linearVelocityInternal += InvMassB * vector4;
			num2 += InvIB * num10;
		}
		else
		{
			float num11 = ((_K.col1.X == 0f) ? 0f : ((0f - num8) / _K.col1.X));
			_impulse.X += num11;
			Vector2 vector5 = num11 * _perp;
			float num12 = num11 * _s2;
			linearVelocityInternal += InvMassB * vector5;
			num2 += InvIB * num12;
		}
		bodyB.LinearVelocityInternal = linearVelocityInternal;
		bodyB.AngularVelocityInternal = num2;
	}

	internal override bool SolvePositionConstraints()
	{
		Body bodyB = base.BodyB;
		Vector2 zero = Vector2.Zero;
		float angle = 0f;
		Vector2 c = bodyB.Sweep.c;
		float a = bodyB.Sweep.a;
		float val = 0f;
		bool flag = false;
		float num = 0f;
		Mat22 A = new Mat22(angle);
		Mat22 A2 = new Mat22(a);
		Vector2 vector = MathUtils.Multiply(ref A, LocalAnchorA - LocalCenterA);
		Vector2 vector2 = MathUtils.Multiply(ref A2, LocalAnchorB - LocalCenterB);
		Vector2 vector3 = c + vector2 - zero - vector;
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
			_K.col1 = new Vector2(x, num4);
			_K.col2 = new Vector2(num4, y);
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
		float num6 = vector4.X * _s2 + vector4.Y * _a2;
		c += InvMassB * vector5;
		a += InvIB * num6;
		bodyB.Sweep.c = c;
		bodyB.Sweep.a = a;
		bodyB.SynchronizeTransform();
		return val <= 0.005f;
	}
}
