using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Maximinus;

public class QuakeCamera
{
	private const float rotationSpeed = 0.005f;

	private Matrix viewMatrix;

	private Matrix projectionMatrix;

	private Viewport viewPort;

	private float leftrightRot;

	private float updownRot;

	private Vector3 cameraPosition;

	public float UpDownRot
	{
		get
		{
			return updownRot;
		}
		set
		{
			updownRot = value;
		}
	}

	public float LeftRightRot
	{
		get
		{
			return leftrightRot;
		}
		set
		{
			leftrightRot = value;
		}
	}

	public Matrix ProjectionMatrix => projectionMatrix;

	public Matrix ViewMatrix => viewMatrix;

	public Vector3 Position
	{
		get
		{
			return cameraPosition;
		}
		set
		{
			cameraPosition = value;
			UpdateViewMatrix();
		}
	}

	public Vector3 TargetPosition
	{
		get
		{
			Matrix matrix = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
			Vector3 position = new Vector3(0f, 0f, -1f);
			Vector3 vector = Vector3.Transform(position, matrix);
			return cameraPosition + vector;
		}
	}

	public Vector3 Forward
	{
		get
		{
			Matrix matrix = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
			Vector3 position = new Vector3(0f, 0f, -1f);
			return Vector3.Transform(position, matrix);
		}
	}

	public Vector3 SideVector
	{
		get
		{
			Matrix matrix = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
			Vector3 position = new Vector3(1f, 0f, 0f);
			return Vector3.Transform(position, matrix);
		}
	}

	public Vector3 UpVector
	{
		get
		{
			Matrix matrix = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
			Vector3 position = new Vector3(0f, 1f, 0f);
			return Vector3.Transform(position, matrix);
		}
	}

	public QuakeCamera(Viewport viewPort)
		: this(viewPort, new Vector3(0f, 1f, 15f), 0f, 0f)
	{
	}

	public QuakeCamera(Viewport viewPort, Vector3 startingPos, float lrRot, float udRot)
	{
		leftrightRot = lrRot;
		updownRot = udRot;
		cameraPosition = startingPos;
		this.viewPort = viewPort;
		float fieldOfView = (float)Math.PI / 4f;
		float nearPlaneDistance = 0.5f;
		float farPlaneDistance = 1000f;
		projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, viewPort.AspectRatio, nearPlaneDistance, farPlaneDistance);
		UpdateViewMatrix();
	}

	public void Update(MouseState currentMouseState, KeyboardState keyState, GamePadState gamePadState)
	{
		leftrightRot -= 0.005f * gamePadState.ThumbSticks.Left.X * 5f;
		updownRot += 0.005f * gamePadState.ThumbSticks.Left.Y * 5f;
		UpdateViewMatrix();
		float y = gamePadState.Triggers.Right - gamePadState.Triggers.Left;
		AddToCameraPosition(new Vector3(gamePadState.ThumbSticks.Right.X, y, 0f - gamePadState.ThumbSticks.Right.Y));
	}

	private void AddToCameraPosition(Vector3 vectorToAdd)
	{
		float num = 0.5f;
		Matrix matrix = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
		Vector3 vector = Vector3.Transform(vectorToAdd, matrix);
		cameraPosition += num * vector;
		UpdateViewMatrix();
	}

	private void UpdateViewMatrix()
	{
		Matrix matrix = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
		Vector3 position = new Vector3(0f, 0f, -1f);
		Vector3 position2 = new Vector3(0f, 1f, 0f);
		Vector3 vector = Vector3.Transform(position, matrix);
		Vector3 cameraTarget = cameraPosition + vector;
		Vector3 vector2 = Vector3.Transform(position2, matrix);
		_ = cameraPosition + vector2;
		viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, vector2);
	}
}
