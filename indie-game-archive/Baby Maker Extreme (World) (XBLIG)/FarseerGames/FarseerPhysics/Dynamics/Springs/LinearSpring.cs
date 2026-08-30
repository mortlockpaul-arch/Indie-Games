using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics.Springs;

public class LinearSpring : Spring
{
	private const float _epsilon = 1E-05f;

	private Vector2 _attachPoint1;

	private Vector2 _attachPoint2;

	private Body _body1;

	private Body _body2;

	private float _restLength;

	private float _dampningForce;

	private Vector2 _differenceNormalized;

	private Vector2 _force;

	private float _springForce;

	private float _temp;

	private Vector2 _difference;

	private Vector2 _relativeVelocity;

	private Vector2 _velocityAtPoint1;

	private Vector2 _velocityAtPoint2;

	private Vector2 _worldPoint1;

	private Vector2 _worldPoint2;

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

	public Vector2 AttachPoint1
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _attachPoint1;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_attachPoint1 = value;
		}
	}

	public Vector2 AttachPoint2
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _attachPoint2;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_attachPoint2 = value;
		}
	}

	public float RestLength
	{
		get
		{
			return _restLength;
		}
		set
		{
			_restLength = value;
		}
	}

	public event SpringDelegate SpringUpdated;

	public LinearSpring()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		_difference = Vector2.Zero;
		_relativeVelocity = Vector2.Zero;
		_velocityAtPoint1 = Vector2.Zero;
		_velocityAtPoint2 = Vector2.Zero;
		_worldPoint1 = Vector2.Zero;
		_worldPoint2 = Vector2.Zero;
		base._002Ector();
	}

	public LinearSpring(Body body1, Vector2 attachPoint1, Body body2, Vector2 attachPoint2, float springConstant, float dampingConstant)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		_difference = Vector2.Zero;
		_relativeVelocity = Vector2.Zero;
		_velocityAtPoint1 = Vector2.Zero;
		_velocityAtPoint2 = Vector2.Zero;
		_worldPoint1 = Vector2.Zero;
		_worldPoint2 = Vector2.Zero;
		base._002Ector();
		_body1 = body1;
		_body2 = body2;
		_attachPoint1 = attachPoint1;
		_attachPoint2 = attachPoint2;
		SpringConstant = springConstant;
		DampingConstant = dampingConstant;
		_difference = body2.GetWorldPosition(attachPoint2) - body1.GetWorldPosition(attachPoint1);
		_restLength = ((Vector2)(ref _difference)).Length();
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
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		base.Update(dt);
		if ((_body1.isStatic && _body2.isStatic) || (!_body1.Enabled && !_body2.Enabled))
		{
			return;
		}
		_body1.GetWorldPosition(ref _attachPoint1, out _worldPoint1);
		_body2.GetWorldPosition(ref _attachPoint2, out _worldPoint2);
		Vector2.Subtract(ref _worldPoint1, ref _worldPoint2, ref _difference);
		float num = ((Vector2)(ref _difference)).Length();
		if (num < 1E-05f)
		{
			return;
		}
		base.SpringError = num - _restLength;
		Vector2.Normalize(ref _difference, ref _differenceNormalized);
		_springForce = SpringConstant * base.SpringError;
		_body1.GetVelocityAtLocalPoint(ref _attachPoint1, out _velocityAtPoint1);
		_body2.GetVelocityAtLocalPoint(ref _attachPoint2, out _velocityAtPoint2);
		Vector2.Subtract(ref _velocityAtPoint1, ref _velocityAtPoint2, ref _relativeVelocity);
		Vector2.Dot(ref _relativeVelocity, ref _difference, ref _temp);
		_dampningForce = DampingConstant * _temp / num;
		Vector2.Multiply(ref _differenceNormalized, 0f - (_springForce + _dampningForce), ref _force);
		bool flag = false;
		if (_force != Vector2.Zero)
		{
			if (!_body1.IsStatic)
			{
				_body1.ApplyForceAtLocalPoint(ref _force, ref _attachPoint1);
				flag = true;
			}
			if (!_body2.IsStatic)
			{
				Vector2.Multiply(ref _force, -1f, ref _force);
				_body2.ApplyForceAtLocalPoint(ref _force, ref _attachPoint2);
				flag = true;
			}
		}
		if (flag && SpringUpdated != null)
		{
			SpringUpdated(this, _body1, _body2);
		}
	}
}
