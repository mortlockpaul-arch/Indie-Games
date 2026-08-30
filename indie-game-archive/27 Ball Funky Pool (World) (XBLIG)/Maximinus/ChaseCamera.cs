using System;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class ChaseCamera
{
	private Vector3 chasePosition;

	private Vector3 chaseDirection;

	private Vector3 up = Vector3.Up;

	private Vector3 desiredPositionOffset = new Vector3(0f, 2f, 2f);

	private Vector3 desiredPosition;

	private Vector3 lookAtOffset = new Vector3(0f, 2.8f, 0f);

	private Vector3 lookAt;

	private float stiffness = 1800f;

	private float damping = 600f;

	private float mass = 50f;

	private Vector3 position;

	private Vector3 velocity;

	private float maxDistance = -1f;

	private float aspectRatio = 1.3333334f;

	private float fieldOfView = MathHelper.ToRadians(45f);

	private float nearPlaneDistance = 1f;

	private float farPlaneDistance = 100000f;

	private Matrix view;

	private Matrix projection;

	public Vector3 ChasePosition
	{
		get
		{
			return chasePosition;
		}
		set
		{
			chasePosition = value;
		}
	}

	public Vector3 ChaseDirection
	{
		get
		{
			return chaseDirection;
		}
		set
		{
			chaseDirection = value;
		}
	}

	public Vector3 Up
	{
		get
		{
			return up;
		}
		set
		{
			up = value;
		}
	}

	public Vector3 DesiredPositionOffset
	{
		get
		{
			return desiredPositionOffset;
		}
		set
		{
			desiredPositionOffset = value;
		}
	}

	public Vector3 DesiredPosition
	{
		get
		{
			UpdateWorldPositions();
			return desiredPosition;
		}
	}

	public Vector3 LookAtOffset
	{
		get
		{
			return lookAtOffset;
		}
		set
		{
			lookAtOffset = value;
		}
	}

	public Vector3 LookAt
	{
		get
		{
			UpdateWorldPositions();
			return lookAt;
		}
	}

	public float Stiffness
	{
		get
		{
			return stiffness;
		}
		set
		{
			stiffness = value;
		}
	}

	public float Damping
	{
		get
		{
			return damping;
		}
		set
		{
			damping = value;
		}
	}

	public float Mass
	{
		get
		{
			return mass;
		}
		set
		{
			mass = value;
		}
	}

	public Vector3 Position => position;

	public Vector3 Velocity => velocity;

	public float MaxDistance
	{
		get
		{
			return maxDistance;
		}
		set
		{
			maxDistance = value;
		}
	}

	public float AspectRatio
	{
		get
		{
			return aspectRatio;
		}
		set
		{
			aspectRatio = value;
		}
	}

	public float FieldOfView
	{
		get
		{
			return fieldOfView;
		}
		set
		{
			fieldOfView = value;
		}
	}

	public float NearPlaneDistance
	{
		get
		{
			return nearPlaneDistance;
		}
		set
		{
			nearPlaneDistance = value;
		}
	}

	public float FarPlaneDistance
	{
		get
		{
			return farPlaneDistance;
		}
		set
		{
			farPlaneDistance = value;
		}
	}

	public Matrix View => view;

	public Matrix Projection => projection;

	private void UpdateWorldPositions()
	{
		Matrix identity = Matrix.Identity;
		identity.Forward = ChaseDirection;
		identity.Up = Up;
		identity.Right = Vector3.Cross(Up, ChaseDirection);
		desiredPosition = ChasePosition + Vector3.TransformNormal(DesiredPositionOffset, identity);
		lookAt = ChasePosition + Vector3.TransformNormal(LookAtOffset, identity);
	}

	private void UpdateMatrices()
	{
		view = Matrix.CreateLookAt(Position, LookAt, Up);
		projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlaneDistance, FarPlaneDistance);
	}

	public void Reset()
	{
		UpdateWorldPositions();
		velocity = Vector3.Zero;
		position = desiredPosition;
		UpdateMatrices();
	}

	public void Update(GameTime gameTime)
	{
		if (gameTime == null)
		{
			throw new ArgumentNullException("gameTime");
		}
		UpdateWorldPositions();
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Vector3 vector = position - desiredPosition;
		Vector3 vector2 = (0f - stiffness) * vector - damping * velocity;
		Vector3 vector3 = vector2 / mass;
		velocity += vector3 * num;
		position += velocity * num;
		if (MaxDistance != -1f && vector.Length() > MaxDistance)
		{
			position = desiredPosition + Vector3.Normalize(position - desiredPosition) * maxDistance;
		}
		UpdateMatrices();
	}
}
