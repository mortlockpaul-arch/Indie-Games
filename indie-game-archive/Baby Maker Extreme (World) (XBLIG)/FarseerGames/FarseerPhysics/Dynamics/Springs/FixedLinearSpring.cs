using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Dynamics.Springs;

public class FixedLinearSpring : Spring
{
	private const float _epsilon = 1E-05f;

	private Body _body;

	private Vector2 _bodyAttachPoint;

	private Vector2 _worldAttachPoint;

	private float _restLength;

	private Vector2 _bodyVelocity;

	private Vector2 _bodyWorldPoint;

	private float _dampningForce;

	private Vector2 _differenceNormalized;

	private Vector2 _force;

	private float _springForce;

	private float _temp;

	private Vector2 _difference;

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

	public Vector2 BodyAttachPoint
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _bodyAttachPoint;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_bodyAttachPoint = value;
		}
	}

	public Vector2 WorldAttachPoint
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _worldAttachPoint;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_worldAttachPoint = value;
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

	public event FixedSpringDelegate SpringUpdated;

	public FixedLinearSpring()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		_bodyVelocity = Vector2.Zero;
		_bodyWorldPoint = Vector2.Zero;
		_difference = Vector2.Zero;
		base._002Ector();
	}

	public FixedLinearSpring(Body body, Vector2 bodyAttachPoint, Vector2 worldAttachPoint, float springConstant, float dampingConstant)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		_bodyVelocity = Vector2.Zero;
		_bodyWorldPoint = Vector2.Zero;
		_difference = Vector2.Zero;
		base._002Ector();
		_body = body;
		_bodyAttachPoint = bodyAttachPoint;
		_worldAttachPoint = worldAttachPoint;
		SpringConstant = springConstant;
		DampingConstant = dampingConstant;
		_difference = worldAttachPoint - _body.GetWorldPosition(bodyAttachPoint);
		_restLength = ((Vector2)(ref _difference)).Length();
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
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		base.Update(dt);
		if (_body.isStatic || !_body.Enabled)
		{
			return;
		}
		_body.GetWorldPosition(ref _bodyAttachPoint, out _bodyWorldPoint);
		Vector2.Subtract(ref _bodyWorldPoint, ref _worldAttachPoint, ref _difference);
		float num = ((Vector2)(ref _difference)).Length();
		if (num < 1E-05f)
		{
			return;
		}
		Vector2.Normalize(ref _difference, ref _differenceNormalized);
		base.SpringError = num - _restLength;
		_springForce = SpringConstant * base.SpringError;
		_body.GetVelocityAtLocalPoint(ref _bodyAttachPoint, out _bodyVelocity);
		Vector2.Dot(ref _bodyVelocity, ref _difference, ref _temp);
		_dampningForce = DampingConstant * _temp / num;
		Vector2.Multiply(ref _differenceNormalized, 0f - (_springForce + _dampningForce), ref _force);
		if (_force != Vector2.Zero)
		{
			_body.ApplyForceAtLocalPoint(ref _force, ref _bodyAttachPoint);
			if (SpringUpdated != null)
			{
				SpringUpdated(this, _body);
			}
		}
	}
}
