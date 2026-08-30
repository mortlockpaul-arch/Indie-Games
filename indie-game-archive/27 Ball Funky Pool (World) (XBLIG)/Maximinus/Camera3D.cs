using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Maximinus;

public class Camera3D
{
	public enum ControlMode
	{
		LookAroundPosition,
		ChaseWithControl,
		ChaseWithSpring,
		Custom
	}

	public class Setup
	{
		public bool Enabled;

		public float fieldOfView;

		public float aspectRatio;

		public float nearPlaneDistance;

		public float farPlaneDistance;

		public readonly bool IsTwoDimensional;

		public Setup(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
			: this(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance, isTwoDimensional: false)
		{
		}

		public Setup(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, bool isTwoDimensional)
		{
			Enabled = true;
			this.fieldOfView = fieldOfView;
			this.aspectRatio = aspectRatio;
			this.nearPlaneDistance = nearPlaneDistance;
			this.farPlaneDistance = farPlaneDistance;
			IsTwoDimensional = isTwoDimensional;
		}

		public Setup(bool enabled)
		{
			if (Enabled)
			{
				throw new Exception("this is the DISABLE 3D CAMERA constructor");
			}
			Enabled = false;
			IsTwoDimensional = false;
		}
	}

	public ControlMode Control;

	public bool DoHandleInput;

	private ChaseCamera chaseSpring;

	private Matrix view;

	private Matrix proj;

	private Vector3 pos;

	private Vector3 lookAt;

	private Vector3 up;

	public float ChaseDirection;

	public float zoomDoubleRatio;

	private Setup setup;

	private Vector2 fixedLookAt_DistanceRange = Vector2.Zero;

	private float fixedLookAt_Distance;

	private float fixedLookAt_Rotation;

	private float fixedLookAt_Height;

	public Matrix View => view;

	public Matrix Proj => proj;

	public Matrix ViewProj => view * proj;

	public Vector3 Pos
	{
		get
		{
			return pos;
		}
		set
		{
			pos = value;
			UpdateView();
		}
	}

	public Vector3 LookAt
	{
		get
		{
			return lookAt;
		}
		set
		{
			lookAt = value;
			Update();
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
			UpdateView();
		}
	}

	public float ZoomDoubleRatio
	{
		get
		{
			return zoomDoubleRatio;
		}
		set
		{
			zoomDoubleRatio = Math.Max(0f, value);
			UpdateProj();
		}
	}

	public Camera3D(Setup setup)
	{
		if (!setup.Enabled)
		{
			throw new Exception("this is the ENABLE 3D CAMERA constructor");
		}
		this.setup = setup;
		Control = ControlMode.LookAroundPosition;
		DoHandleInput = false;
		pos = Vector3.Zero;
		lookAt = Vector3.Zero;
		up = Vector3.Up;
		UpdateView();
		zoomDoubleRatio = 0f;
		UpdateProj();
		chaseSpring = new ChaseCamera();
	}

	private void UpdateProj()
	{
		if (setup.IsTwoDimensional)
		{
			proj = Matrix.CreateOrthographicOffCenter(0f, MaximinusGame.Draw2D.ScreenSizePoint.X, MaximinusGame.Draw2D.ScreenSizePoint.Y, 0f, 0f, 1f);
			return;
		}
		float fieldOfView = MathHelper.Clamp((zoomDoubleRatio > 0f) ? MathHelper.Lerp(setup.fieldOfView, 0f, zoomDoubleRatio) : MathHelper.Lerp(setup.fieldOfView, (float)Math.PI, 0f - zoomDoubleRatio), 0.0001f, 3.1414928f);
		proj = Matrix.CreatePerspectiveFieldOfView(fieldOfView, setup.aspectRatio, setup.nearPlaneDistance, setup.farPlaneDistance);
	}

	public void EnableLookAround(Vector3 lookAtValue, float near, float far)
	{
		DoHandleInput = true;
		Control = ControlMode.LookAroundPosition;
		lookAt = lookAtValue;
		fixedLookAt_DistanceRange = new Vector2(near, far);
		fixedLookAt_Distance = (near + far) / 2f;
		fixedLookAt_Rotation = 0f;
		fixedLookAt_Height = (float)Math.PI / 4f;
		Update();
	}

	public void EnableChase(Vector3 lookAtValue, float direction, float near, float far, float height)
	{
		DoHandleInput = true;
		Control = ControlMode.ChaseWithControl;
		ChaseDirection = direction;
		lookAt = lookAtValue;
		fixedLookAt_DistanceRange = new Vector2(near, far);
		fixedLookAt_Distance = (near + far) / 2f;
		fixedLookAt_Rotation = 0f;
		fixedLookAt_Height = height;
		ChaseDirection = 0f;
		Update();
	}

	public void EnableChaseSpring(Vector3 posValue, Vector3 dirValue, Vector3 posOffset, Vector3 looktAtOffset, float stiffness, float maxDistance)
	{
		DoHandleInput = false;
		Control = ControlMode.ChaseWithSpring;
		chaseSpring.DesiredPositionOffset = posOffset;
		chaseSpring.LookAtOffset = looktAtOffset;
		chaseSpring.Stiffness = stiffness;
		chaseSpring.MaxDistance = maxDistance;
		UpdateChaseSpring(posValue, dirValue);
		chaseSpring.Reset();
	}

	public void EnableCustom(Vector3 position, Vector3 lookAt)
	{
		DoHandleInput = false;
		Control = ControlMode.Custom;
		pos = position;
		LookAt = lookAt;
	}

	public void UpdateChaseSpring(Vector3 chasePos, Vector3 chaseDir)
	{
		chaseSpring.ChasePosition = chasePos;
		chaseSpring.ChaseDirection = chaseDir;
		chaseSpring.Up = Vector3.Up;
		Update();
	}

	private void UpdateView()
	{
		if (setup.IsTwoDimensional)
		{
			view = Matrix.Identity;
		}
		else
		{
			view = Matrix.CreateLookAt(pos, lookAt, up);
		}
	}

	public void UpdateCustom(Vector3 newPos, Vector3 newLookAt)
	{
		UpdateCustom(newPos, newLookAt, Vector3.Up);
	}

	public void UpdateCustom(Vector3 newPos, Vector3 newLookAt, Vector3 newUp)
	{
		pos = newPos;
		lookAt = newLookAt;
		up = newUp;
		Update();
	}

	private void Update()
	{
		switch (Control)
		{
		case ControlMode.LookAroundPosition:
			pos = lookAt + fixedLookAt_Distance * Vector3.Normalize(Vector3.Transform(new Vector3(1f, 0f, 0f), Matrix.CreateRotationZ(fixedLookAt_Height) * Matrix.CreateRotationY(fixedLookAt_Rotation)));
			UpdateView();
			break;
		case ControlMode.ChaseWithControl:
		{
			Vector2 vector = MyMath.VectorNormFromAngleRad(0f - ChaseDirection) * -1f;
			Vector3 vector2 = new Vector3(vector.X, 0f, vector.Y);
			pos = lookAt + vector2 * fixedLookAt_Distance + Vector3.UnitY * fixedLookAt_Height;
			lookAt.Y = pos.Y;
			UpdateView();
			break;
		}
		case ControlMode.ChaseWithSpring:
			chaseSpring.Update(MaximinusGame.gameTime);
			view = chaseSpring.View;
			break;
		case ControlMode.Custom:
			UpdateView();
			break;
		}
	}

	public void HandleInputDigital(Buttons b, Utils.Input.PressOrRelease pressOrRelease)
	{
	}

	public void HandleInputAnalog(Vector2 stickLeft, Vector2 stickRight, Vector2 dpad, Vector2 triggers)
	{
		switch (Control)
		{
		case ControlMode.LookAroundPosition:
		case ControlMode.ChaseWithControl:
		{
			Vector2 vector = stickRight;
			if (vector.X != 0f && Control == ControlMode.LookAroundPosition)
			{
				fixedLookAt_Rotation += vector.X * 0.1f;
				if (fixedLookAt_Rotation < 0f)
				{
					fixedLookAt_Rotation += (float)Math.PI * 2f;
				}
				else
				{
					fixedLookAt_Rotation %= (float)Math.PI * 2f;
				}
			}
			if (vector.Y != 0f)
			{
				fixedLookAt_Height += vector.Y * 0.031f;
				fixedLookAt_Height = MathHelper.Clamp(fixedLookAt_Height, (Control == ControlMode.LookAroundPosition) ? ((float)Math.PI * -9f / 20f) : 0.01f, (float)Math.PI * 9f / 20f);
			}
			if (triggers != Vector2.Zero)
			{
				fixedLookAt_Distance += (triggers.X - triggers.Y) * 0.01f * (fixedLookAt_DistanceRange.Y - fixedLookAt_DistanceRange.X);
				fixedLookAt_Distance = MathHelper.Clamp(fixedLookAt_Distance, fixedLookAt_DistanceRange.X, fixedLookAt_DistanceRange.Y);
			}
			if (vector != Vector2.Zero || triggers != Vector2.Zero)
			{
				Update();
			}
			break;
		}
		}
	}
}
