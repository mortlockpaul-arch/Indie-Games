using System;

namespace FarseerGames.FarseerPhysics.Dynamics.Springs;

public class FixedAngleSpring : Spring
{
	private Body _body;

	private float _maxTorque = float.MaxValue;

	private float _targetAngle;

	private float _torqueMultiplier = 1f;

	public Body Body
	{
		get
		{
			return _body;
		}
		set
		{
			_body = value;
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

	public event FixedSpringDelegate SpringUpdated;

	public FixedAngleSpring()
	{
	}

	public FixedAngleSpring(Body body, float springConstant, float dampingConstant)
	{
		_body = body;
		SpringConstant = springConstant;
		DampingConstant = dampingConstant;
		_targetAngle = body.TotalRotation;
	}

	public override void Validate()
	{
		if (_body.IsDisposed)
		{
			Dispose();
		}
	}

	public override void Update(float dt)
	{
		base.Update(dt);
		if (_body.isStatic || !_body.Enabled)
		{
			return;
		}
		float num = _targetAngle - _body.totalRotation;
		float num2 = SpringConstant * num;
		base.SpringError = num;
		float num3 = num2 - DampingConstant * _body.AngularVelocity;
		num3 = Math.Min(Math.Abs(num3 * _torqueMultiplier), _maxTorque) * (float)Math.Sign(num3);
		if (num3 != 0f)
		{
			_body.ApplyTorque(num3);
			if (SpringUpdated != null)
			{
				SpringUpdated(this, _body);
			}
		}
	}
}
