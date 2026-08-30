using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;

namespace FarseerGames.FarseerPhysics.Dynamics.Joints;

public class FixedAngleJoint : Joint
{
	private Body _body;

	private float _massFactor;

	private float _maxImpulse = float.MaxValue;

	private float _targetAngle;

	private float _velocityBias;

	[ContentSerializerIgnore]
	[XmlIgnore]
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

	public float MaxImpulse
	{
		get
		{
			return _maxImpulse;
		}
		set
		{
			_maxImpulse = value;
		}
	}

	public event FixedJointDelegate JointUpdated;

	public FixedAngleJoint()
	{
	}

	public FixedAngleJoint(Body body)
	{
		_body = body;
	}

	public FixedAngleJoint(Body body, float targetAngle)
	{
		_body = body;
		_targetAngle = targetAngle;
	}

	public override void Validate()
	{
		if (_body.IsDisposed)
		{
			Dispose();
		}
	}

	public override void PreStep(float inverseDt)
	{
		if (!_body.isStatic && _body.Enabled)
		{
			base.JointError = _body.totalRotation - _targetAngle;
			_velocityBias = (0f - BiasFactor) * inverseDt * base.JointError;
			_massFactor = (1f - Softness) / _body.inverseMomentOfInertia;
		}
	}

	public override void Update()
	{
		base.Update();
		if (_body.isStatic || !_body.Enabled)
		{
			return;
		}
		float num = (_velocityBias - _body.AngularVelocity) * _massFactor;
		if (num != 0f)
		{
			_body.AngularVelocity += _body.inverseMomentOfInertia * (float)Math.Sign(num) * Math.Min(Math.Abs(num), _maxImpulse);
			if (JointUpdated != null)
			{
				JointUpdated(this, _body);
			}
		}
	}
}
