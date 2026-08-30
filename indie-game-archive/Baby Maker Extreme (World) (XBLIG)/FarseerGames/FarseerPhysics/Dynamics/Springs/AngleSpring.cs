using System;

namespace FarseerGames.FarseerPhysics.Dynamics.Springs;

public class AngleSpring : Spring
{
	private Body _body1;

	private Body _body2;

	private float _maxTorque = float.MaxValue;

	private float _targetAngle;

	private float _torqueMultiplier = 1f;

	public Body Body1
	{
		get
		{
			return _body1;
		}
		set
		{
			_body1 = value;
		}
	}

	public Body Body2
	{
		get
		{
			return _body2;
		}
		set
		{
			_body2 = value;
		}
	}

	public float TargetAngle
	{
		get
		{
			return _targetAngle;
		}
		set
		{
			_targetAngle = value;
			if ((double)_targetAngle > 5.5)
			{
				_targetAngle = 5.5f;
			}
			if (_targetAngle < -5.5f)
			{
				_targetAngle = -5.5f;
			}
		}
	}

	public float MaxTorque
	{
		get
		{
			return _maxTorque;
		}
		set
		{
			_maxTorque = value;
		}
	}

	public float TorqueMultiplier
	{
		get
		{
			return _torqueMultiplier;
		}
		set
		{
			_torqueMultiplier = value;
		}
	}

	public event SpringDelegate SpringUpdated;

	public AngleSpring()
	{
	}

	public AngleSpring(Body body1, Body body2, float springConstant, float dampingConstant)
	{
		_body1 = body1;
		_body2 = body2;
		SpringConstant = springConstant;
		DampingConstant = dampingConstant;
		_targetAngle = _body2.TotalRotation - _body1.TotalRotation;
	}

	public override void Validate()
	{
		if (_body1.IsDisposed || _body2.IsDisposed)
		{
			Dispose();
		}
	}

	public override void Update(float dt)
	{
		base.Update(dt);
		if ((_body1.isStatic && _body2.isStatic) || (!_body1.Enabled && !_body2.Enabled))
		{
			return;
		}
		float num = _body2.totalRotation - (_body1.totalRotation + _targetAngle);
		float num2 = SpringConstant * num;
		base.SpringError = num;
		bool flag = false;
		if (!_body1.IsStatic)
		{
			float num3 = num2 - DampingConstant * _body1.AngularVelocity;
			num3 = Math.Min(Math.Abs(num3 * _torqueMultiplier), _maxTorque) * (float)Math.Sign(num3);
			if (num3 != 0f)
			{
				_body1.ApplyTorque(num3);
				flag = true;
			}
		}
		if (!_body2.IsStatic)
		{
			float num4 = 0f - num2 - DampingConstant * _body2.AngularVelocity;
			num4 = Math.Min(Math.Abs(num4 * _torqueMultiplier), _maxTorque) * (float)Math.Sign(num4);
			if (num4 != 0f)
			{
				_body2.ApplyTorque(num4);
				flag = true;
			}
		}
		if (flag && SpringUpdated != null)
		{
			SpringUpdated(this, _body1, _body2);
		}
	}
}
