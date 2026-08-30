using System;
using System.Xml.Serialization;
using FarseerGames.FarseerPhysics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace FarseerGames.FarseerPhysics.Dynamics;

public sealed class Body : IIsDisposable, IDisposable
{
	private float _momentOfInertia;

	private float _previousAngularVelocity;

	private Vector2 _previousLinearVelocity;

	private Vector2 _previousPosition;

	private float _previousRotation;

	private int _revolutions;

	private float _torque;

	private bool _isDisposed;

	internal Vector2 impulse;

	internal float inverseMass;

	internal float inverseMomentOfInertia;

	internal float totalRotation;

	internal float rotation;

	internal Vector2 linearVelocityBias;

	internal float mass;

	internal Vector2 position;

	internal float angularVelocityBias;

	internal Vector2 force;

	internal bool isStatic;

	public float AngularVelocity;

	public bool Enabled;

	public bool IgnoreGravity;

	public bool IsQuadraticDragEnabled;

	public float LinearDragCoefficient;

	public Vector2 LinearVelocity;

	public float QuadraticDragCoefficient;

	public float RotationalDragCoefficient;

	[ContentSerializerIgnore]
	[XmlIgnore]
	public object Tag;

	[XmlIgnore]
	[ContentSerializerIgnore]
	public UpdatedEventHandler Updated;

	public float IdleTime;

	public bool IsAutoIdle;

	public float MinimumVelocity;

	private Vector2 _velocityTemp;

	private Vector2 _worldPositionTemp;

	private float _tempAngularVelocity;

	private Vector2 _tempLinearVelocity;

	private Vector2 _tempDeltaPosition;

	private float _tempDeltaRotation;

	private bool _changed;

	private Vector2 _acceleration;

	private Vector2 _dv;

	private float _dw;

	private Vector2 _linearDrag;

	private float _rotationalDrag;

	private float _speed;

	private Vector2 _diff;

	private Vector2 _tempVelocity;

	private Vector2 _r1;

	private Vector2 _localPositionTemp;

	private Matrix _bodyMatrixTemp;

	private Matrix _rotationMatrixTemp;

	private Matrix _translationMatrixTemp;

	public bool Moves => ((Vector2)(ref LinearVelocity)).Length() >= MinimumVelocity;

	public float Mass
	{
		get
		{
			return mass;
		}
		set
		{
			if (value == 0f)
			{
				throw new ArgumentException("Mass cannot be 0", "value");
			}
			mass = value;
			if (isStatic)
			{
				inverseMass = 0f;
			}
			else
			{
				inverseMass = 1f / value;
			}
		}
	}

	public float InverseMass => inverseMass;

	public float MomentOfInertia
	{
		get
		{
			return _momentOfInertia;
		}
		set
		{
			if (value <= 0f)
			{
				throw new ArgumentException("Moment of inertia cannot be 0 or less", "value");
			}
			_momentOfInertia = value;
			if (isStatic)
			{
				inverseMomentOfInertia = 0f;
			}
			else
			{
				inverseMomentOfInertia = 1f / value;
			}
		}
	}

	public float InverseMomentOfInertia => inverseMomentOfInertia;

	public bool IsStatic
	{
		get
		{
			return isStatic;
		}
		set
		{
			isStatic = value;
			if (isStatic)
			{
				inverseMass = 0f;
				inverseMomentOfInertia = 0f;
			}
			else
			{
				inverseMass = 1f / mass;
				inverseMomentOfInertia = 1f / _momentOfInertia;
			}
		}
	}

	public Vector2 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return position;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			position = value;
			if (Updated != null)
			{
				Updated(ref position, ref rotation);
			}
		}
	}

	public int Revolutions => _revolutions;

	public float Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
			int num = (int)(rotation / ((float)Math.PI * 2f));
			if (rotation < 0f)
			{
				num--;
			}
			rotation -= (float)num * ((float)Math.PI * 2f);
			_revolutions += num;
			totalRotation = rotation + (float)_revolutions * ((float)Math.PI * 2f);
			if (Updated != null)
			{
				Updated(ref position, ref rotation);
			}
		}
	}

	public float TotalRotation => totalRotation;

	public Vector2 Force
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return force;
		}
	}

	public float Torque => _torque;

	public Vector2 XVectorInWorldCoordinates
	{
		get
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			GetBodyMatrix(out _bodyMatrixTemp);
			return new Vector2(((Matrix)(ref _bodyMatrixTemp)).Right.X, ((Matrix)(ref _bodyMatrixTemp)).Right.Y);
		}
	}

	public Vector2 YVectorInWorldCoordinates
	{
		get
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			GetBodyMatrix(out _bodyMatrixTemp);
			return new Vector2(((Matrix)(ref _bodyMatrixTemp)).Up.X, ((Matrix)(ref _bodyMatrixTemp)).Up.Y);
		}
	}

	[ContentSerializerIgnore]
	[XmlIgnore]
	public bool IsDisposed
	{
		get
		{
			return _isDisposed;
		}
		set
		{
			_isDisposed = value;
		}
	}

	public event EventHandler<EventArgs> Disposed;

	public Body()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		_momentOfInertia = 1f;
		_previousLinearVelocity = Vector2.Zero;
		_previousPosition = Vector2.Zero;
		impulse = Vector2.Zero;
		inverseMass = 1f;
		inverseMomentOfInertia = 1f;
		linearVelocityBias = Vector2.Zero;
		mass = 1f;
		position = Vector2.Zero;
		force = Vector2.Zero;
		Enabled = true;
		LinearDragCoefficient = 0.001f;
		LinearVelocity = Vector2.Zero;
		QuadraticDragCoefficient = 0.001f;
		RotationalDragCoefficient = 0.001f;
		MinimumVelocity = 55f;
		_velocityTemp = Vector2.Zero;
		_worldPositionTemp = Vector2.Zero;
		_acceleration = Vector2.Zero;
		_dv = Vector2.Zero;
		_linearDrag = Vector2.Zero;
		_tempVelocity = Vector2.Zero;
		_r1 = Vector2.Zero;
		_localPositionTemp = Vector2.Zero;
		_bodyMatrixTemp = Matrix.Identity;
		_rotationMatrixTemp = Matrix.Identity;
		_translationMatrixTemp = Matrix.Identity;
		base._002Ector();
	}

	public Body(Body body)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		_momentOfInertia = 1f;
		_previousLinearVelocity = Vector2.Zero;
		_previousPosition = Vector2.Zero;
		impulse = Vector2.Zero;
		inverseMass = 1f;
		inverseMomentOfInertia = 1f;
		linearVelocityBias = Vector2.Zero;
		mass = 1f;
		position = Vector2.Zero;
		force = Vector2.Zero;
		Enabled = true;
		LinearDragCoefficient = 0.001f;
		LinearVelocity = Vector2.Zero;
		QuadraticDragCoefficient = 0.001f;
		RotationalDragCoefficient = 0.001f;
		MinimumVelocity = 55f;
		_velocityTemp = Vector2.Zero;
		_worldPositionTemp = Vector2.Zero;
		_acceleration = Vector2.Zero;
		_dv = Vector2.Zero;
		_linearDrag = Vector2.Zero;
		_tempVelocity = Vector2.Zero;
		_r1 = Vector2.Zero;
		_localPositionTemp = Vector2.Zero;
		_bodyMatrixTemp = Matrix.Identity;
		_rotationMatrixTemp = Matrix.Identity;
		_translationMatrixTemp = Matrix.Identity;
		base._002Ector();
		Mass = body.Mass;
		MomentOfInertia = body.MomentOfInertia;
		LinearDragCoefficient = body.LinearDragCoefficient;
		RotationalDragCoefficient = body.RotationalDragCoefficient;
		IsQuadraticDragEnabled = body.IsQuadraticDragEnabled;
		QuadraticDragCoefficient = body.QuadraticDragCoefficient;
		Enabled = body.Enabled;
		Tag = body.Tag;
		IgnoreGravity = body.IgnoreGravity;
		IsStatic = body.isStatic;
	}

	public void ResetDynamics()
	{
		LinearVelocity.X = 0f;
		LinearVelocity.Y = 0f;
		_previousLinearVelocity.X = 0f;
		_previousLinearVelocity.Y = 0f;
		AngularVelocity = 0f;
		_previousAngularVelocity = 0f;
		position.X = 0f;
		position.Y = 0f;
		_previousPosition.X = 0f;
		_previousPosition.Y = 0f;
		rotation = 0f;
		_revolutions = 0;
		totalRotation = 0f;
		force.X = 0f;
		force.Y = 0f;
		_torque = 0f;
		impulse.X = 0f;
		impulse.Y = 0f;
		_linearDrag.X = 0f;
		_linearDrag.Y = 0f;
		_rotationalDrag = 0f;
		if (Updated != null)
		{
			Updated(ref position, ref rotation);
		}
	}

	public Matrix GetBodyMatrix()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Matrix.CreateTranslation(position.X, position.Y, 0f, ref _translationMatrixTemp);
		Matrix.CreateRotationZ(rotation, ref _rotationMatrixTemp);
		Matrix.Multiply(ref _rotationMatrixTemp, ref _translationMatrixTemp, ref _bodyMatrixTemp);
		return _bodyMatrixTemp;
	}

	public void GetBodyMatrix(out Matrix bodyMatrix)
	{
		Matrix.CreateTranslation(position.X, position.Y, 0f, ref _translationMatrixTemp);
		Matrix.CreateRotationZ(rotation, ref _rotationMatrixTemp);
		Matrix.Multiply(ref _rotationMatrixTemp, ref _translationMatrixTemp, ref bodyMatrix);
	}

	public Matrix GetBodyRotationMatrix()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Matrix.CreateRotationZ(rotation, ref _rotationMatrixTemp);
		return _rotationMatrixTemp;
	}

	public void GetBodyRotationMatrix(out Matrix rotationMatrix)
	{
		Matrix.CreateRotationZ(rotation, ref rotationMatrix);
	}

	public Vector2 GetWorldPosition(Vector2 localPosition)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		GetBodyMatrix(out _bodyMatrixTemp);
		Vector2.Transform(ref localPosition, ref _bodyMatrixTemp, ref _worldPositionTemp);
		return _worldPositionTemp;
	}

	public void GetWorldPosition(ref Vector2 localPosition, out Vector2 worldPosition)
	{
		GetBodyMatrix(out _bodyMatrixTemp);
		Vector2.Transform(ref localPosition, ref _bodyMatrixTemp, ref worldPosition);
	}

	public Vector2 GetLocalPosition(Vector2 worldPosition)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		GetBodyRotationMatrix(out _rotationMatrixTemp);
		Matrix.Transpose(ref _rotationMatrixTemp, ref _rotationMatrixTemp);
		Vector2.Subtract(ref worldPosition, ref position, ref _localPositionTemp);
		Vector2.Transform(ref _localPositionTemp, ref _rotationMatrixTemp, ref _localPositionTemp);
		return _localPositionTemp;
	}

	public void GetLocalPosition(ref Vector2 worldPosition, out Vector2 localPosition)
	{
		GetBodyRotationMatrix(out _rotationMatrixTemp);
		Matrix.Transpose(ref _rotationMatrixTemp, ref _rotationMatrixTemp);
		Vector2.Subtract(ref worldPosition, ref position, ref localPosition);
		Vector2.Transform(ref localPosition, ref _rotationMatrixTemp, ref localPosition);
	}

	public Vector2 GetVelocityAtLocalPoint(Vector2 localPoint)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		GetVelocityAtLocalPoint(ref localPoint, out _tempVelocity);
		return _tempVelocity;
	}

	public void GetVelocityAtLocalPoint(ref Vector2 localPoint, out Vector2 velocity)
	{
		GetWorldPosition(ref localPoint, out _r1);
		Vector2.Subtract(ref _r1, ref position, ref _r1);
		GetVelocityAtWorldOffset(ref _r1, out velocity);
	}

	public void GetVelocityAtWorldPoint(ref Vector2 worldPoint, out Vector2 velocity)
	{
		Vector2.Subtract(ref worldPoint, ref position, ref _r1);
		GetVelocityAtWorldOffset(ref _r1, out velocity);
	}

	public void GetVelocityAtWorldOffset(ref Vector2 offset, out Vector2 velocity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		velocity = _velocityTemp;
		velocity.X = (0f - AngularVelocity) * offset.Y;
		velocity.Y = AngularVelocity * offset.X;
		velocity.X += LinearVelocity.X;
		velocity.Y += LinearVelocity.Y;
	}

	public void GetVelocityBiasAtWorldOffset(ref Vector2 offset, out Vector2 velocityBias)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		velocityBias = _velocityTemp;
		velocityBias.X = (0f - angularVelocityBias) * offset.Y;
		velocityBias.Y = angularVelocityBias * offset.X;
		velocityBias.X += linearVelocityBias.X;
		velocityBias.Y += linearVelocityBias.Y;
	}

	public void ApplyForce(Vector2 amount)
	{
		force.X += amount.X;
		force.Y += amount.Y;
	}

	public void ApplyForce(ref Vector2 amount)
	{
		force.X += amount.X;
		force.Y += amount.Y;
	}

	public void ApplyForceAtLocalPoint(Vector2 amount, Vector2 point)
	{
		GetWorldPosition(ref point, out _diff);
		Vector2.Subtract(ref _diff, ref position, ref _diff);
		float num = _diff.X * amount.Y - _diff.Y * amount.X;
		_torque += num;
		ref Vector2 reference = ref force;
		reference.X += amount.X;
		ref Vector2 reference2 = ref force;
		reference2.Y += amount.Y;
	}

	public void ApplyForceAtLocalPoint(ref Vector2 amount, ref Vector2 point)
	{
		GetWorldPosition(ref point, out _diff);
		Vector2.Subtract(ref _diff, ref position, ref _diff);
		float num = _diff.X * amount.Y - _diff.Y * amount.X;
		_torque += num;
		ref Vector2 reference = ref force;
		reference.X += amount.X;
		ref Vector2 reference2 = ref force;
		reference2.Y += amount.Y;
	}

	public void ApplyForceAtWorldPoint(Vector2 amount, Vector2 point)
	{
		Vector2.Subtract(ref point, ref position, ref _diff);
		float num = _diff.X * amount.Y - _diff.Y * amount.X;
		_torque += num;
		ref Vector2 reference = ref force;
		reference.X += amount.X;
		ref Vector2 reference2 = ref force;
		reference2.Y += amount.Y;
	}

	public void ApplyForceAtWorldPoint(ref Vector2 amount, ref Vector2 point)
	{
		Vector2.Subtract(ref point, ref position, ref _diff);
		float num = _diff.X * amount.Y - _diff.Y * amount.X;
		_torque += num;
		ref Vector2 reference = ref force;
		reference.X += amount.X;
		ref Vector2 reference2 = ref force;
		reference2.Y += amount.Y;
	}

	public void ClearForce()
	{
		force.X = 0f;
		force.Y = 0f;
	}

	public void ApplyTorque(float torque)
	{
		_torque += torque;
	}

	public void ClearTorque()
	{
		_torque = 0f;
	}

	public void ApplyImpulse(Vector2 amount)
	{
		ref Vector2 reference = ref impulse;
		reference.X += amount.X;
		ref Vector2 reference2 = ref impulse;
		reference2.Y += amount.Y;
	}

	public void ApplyImpulse(ref Vector2 amount)
	{
		ref Vector2 reference = ref impulse;
		reference.X += amount.X;
		ref Vector2 reference2 = ref impulse;
		reference2.Y += amount.Y;
	}

	internal void ApplyImpulses()
	{
		ApplyImmediateImpulse(ref impulse);
		impulse.X = 0f;
		impulse.Y = 0f;
	}

	internal void ApplyImmediateImpulse(ref Vector2 amount)
	{
		if (!isStatic)
		{
			_dv.X = amount.X * inverseMass;
			_dv.Y = amount.Y * inverseMass;
			LinearVelocity.X = _dv.X + LinearVelocity.X;
			LinearVelocity.Y = _dv.Y + LinearVelocity.Y;
		}
	}

	public void ClearImpulse()
	{
		impulse.X = 0f;
		impulse.Y = 0f;
	}

	public void ApplyAngularImpulse(float amount)
	{
		AngularVelocity += amount * inverseMomentOfInertia;
	}

	public void ApplyAngularImpulse(ref float amount)
	{
		AngularVelocity += amount * inverseMomentOfInertia;
	}

	internal void IntegrateVelocity(float dt)
	{
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		_linearDrag.X = (0f - LinearVelocity.X) * LinearDragCoefficient;
		_linearDrag.Y = (0f - LinearVelocity.Y) * LinearDragCoefficient;
		if (IsQuadraticDragEnabled)
		{
			float num = LinearVelocity.X * LinearVelocity.X + LinearVelocity.Y * LinearVelocity.Y;
			_speed = (float)Math.Sqrt(num);
			_linearDrag.X += (0f - _speed) * QuadraticDragCoefficient * (float)Math.Sign(LinearVelocity.X);
			_linearDrag.Y += (0f - _speed) * QuadraticDragCoefficient * (float)Math.Sign(LinearVelocity.Y);
		}
		ApplyForce(ref _linearDrag);
		_rotationalDrag = AngularVelocity * AngularVelocity * (float)Math.Sign(AngularVelocity);
		_rotationalDrag *= 0f - RotationalDragCoefficient;
		ApplyTorque(_rotationalDrag);
		_acceleration.X = force.X * inverseMass;
		_acceleration.Y = force.Y * inverseMass;
		_dv.X = _acceleration.X * dt;
		_dv.Y = _acceleration.Y * dt;
		_previousLinearVelocity = LinearVelocity;
		LinearVelocity.X = _previousLinearVelocity.X + _dv.X;
		LinearVelocity.Y = _previousLinearVelocity.Y + _dv.Y;
		_dw = _torque * inverseMomentOfInertia * dt;
		_previousAngularVelocity = AngularVelocity;
		AngularVelocity = _previousAngularVelocity + _dw;
	}

	internal void IntegratePosition(float dt)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		_tempLinearVelocity.X = LinearVelocity.X + linearVelocityBias.X;
		_tempLinearVelocity.Y = LinearVelocity.Y + linearVelocityBias.Y;
		_tempDeltaPosition.X = _tempLinearVelocity.X * dt;
		_tempDeltaPosition.Y = _tempLinearVelocity.Y * dt;
		if (_tempDeltaPosition != Vector2.Zero)
		{
			_previousPosition = position;
			position.X = _previousPosition.X + _tempDeltaPosition.X;
			position.Y = _previousPosition.Y + _tempDeltaPosition.Y;
			_changed = true;
		}
		linearVelocityBias.X = 0f;
		linearVelocityBias.Y = 0f;
		_tempAngularVelocity = AngularVelocity + angularVelocityBias;
		_tempDeltaRotation = _tempAngularVelocity * dt;
		if (_tempDeltaRotation != 0f)
		{
			_previousRotation = rotation;
			rotation = _previousRotation + _tempDeltaRotation;
			_changed = true;
			int num = (int)(rotation / ((float)Math.PI * 2f));
			if (rotation < 0f)
			{
				num--;
			}
			rotation -= (float)num * ((float)Math.PI * 2f);
			_revolutions += num;
			totalRotation = rotation + (float)_revolutions * ((float)Math.PI * 2f);
		}
		angularVelocityBias = 0f;
		if (_changed && Updated != null)
		{
			Updated(ref position, ref rotation);
		}
	}

	internal void ApplyImpulseAtWorldOffset(ref Vector2 amount, ref Vector2 offset)
	{
		_dv.X = amount.X * inverseMass;
		_dv.Y = amount.Y * inverseMass;
		LinearVelocity.X = _dv.X + LinearVelocity.X;
		LinearVelocity.Y = _dv.Y + LinearVelocity.Y;
		float num = offset.X * amount.Y - offset.Y * amount.X;
		num *= inverseMomentOfInertia;
		AngularVelocity += num;
	}

	internal void ApplyBiasImpulseAtWorldOffset(ref Vector2 impulseBias, ref Vector2 offset)
	{
		_dv.X = impulseBias.X * inverseMass;
		_dv.Y = impulseBias.Y * inverseMass;
		linearVelocityBias.X = _dv.X + linearVelocityBias.X;
		linearVelocityBias.Y = _dv.Y + linearVelocityBias.Y;
		float num = offset.X * impulseBias.Y - offset.Y * impulseBias.X;
		num *= inverseMomentOfInertia;
		angularVelocityBias += num;
	}

	public void Dispose()
	{
		IsDisposed = true;
		if (Disposed != null)
		{
			Disposed(this, EventArgs.Empty);
		}
		GC.SuppressFinalize(this);
	}
}
