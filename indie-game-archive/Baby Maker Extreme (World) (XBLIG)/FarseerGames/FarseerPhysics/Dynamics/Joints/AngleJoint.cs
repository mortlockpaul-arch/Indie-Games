using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;

namespace FarseerGames.FarseerPhysics.Dynamics.Joints;

public class AngleJoint : Joint
{
	private Body _body1;

	private Body _body2;

	private float _massFactor;

	private float _maxImpulse = float.MaxValue;

	private float _targetAngle;

	private float _velocityBias;

	[ContentSerializerIgnore]
	[XmlIgnore]
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

	[XmlIgnore]
	[ContentSerializerIgnore]
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

	public event JointDelegate JointUpdated;

	public AngleJoint()
	{
	}

	public AngleJoint(Body body1, Body body2)
	{
		_body1 = body1;
		_body2 = body2;
	}

	public AngleJoint(Body body1, Body body2, float targetAngle)
	{
		_body1 = body1;
		_body2 = body2;
		_targetAngle = targetAngle;
	}

	public override void Validate()
	{
		if (_body1.IsDisposed || _body2.IsDisposed)
		{
			Dispose();
		}
	}

	public override void PreStep(float inverseDt)
	{
		if ((!_body1.isStatic || !_body2.isStatic) && (_body1.Enabled || _body2.Enabled))
		{
			base.JointError = _body2.totalRotation - _body1.totalRotation - _targetAngle;
			_velocityBias = (0f - BiasFactor) * inverseDt * base.JointError;
			_massFactor = (1f - Softness) / (_body1.inverseMomentOfInertia + _body2.inverseMomentOfInertia);
		}
	}

	public override void Update()
	{
		base.Update();
		if ((_body1.isStatic && _body2.isStatic) || (!_body1.Enabled && !_body2.Enabled))
		{
			return;
		}
		float num = (_velocityBias - _body2.AngularVelocity + _body1.AngularVelocity) * _massFactor;
		if (num != 0f)
		{
			_body1.AngularVelocity -= _body1.inverseMomentOfInertia * (float)Math.Sign(num) * Math.Min(Math.Abs(num), _maxImpulse);
			_body2.AngularVelocity += _body2.inverseMomentOfInertia * (float)Math.Sign(num) * Math.Min(Math.Abs(num), _maxImpulse);
			if (JointUpdated != null)
			{
				JointUpdated(this, _body1, _body2);
			}
		}
	}
}
