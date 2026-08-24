using System;
using Microsoft.Xna.Framework;

namespace RenegadeEngine;

public class Camera
{
	protected float aspectRatio = 1.3333334f;

	protected float NearClip = 0.1f;

	protected float FarClip = 1000f;

	protected float yaw;

	protected float pitch;

	protected float roll;

	protected Vector3 up = Vector3.Up;

	public Quaternion Orientation = Quaternion.Identity;

	public Matrix View = Matrix.Identity;

	public Matrix Projection = Matrix.Identity;

	public Matrix World = Matrix.Identity;

	public float RotationRate = (float)Math.PI / 2f;

	public float ViewAngle = (float)Math.PI / 4f;

	public float MovementRate = 100f;

	public float AngleDeltaRate = 5f;

	public float OrthoScale = 30f;

	public Vector3 Position = Vector3.Backward;

	public Vector3 PositionMin = Vector3.Backward;

	public Vector3 PositionMax = Vector3.Backward;

	public Vector3 LookAt = Vector3.Zero;

	public Vector3 LookAtMin = Vector3.Zero;

	public Vector3 LookAtMax = Vector3.Zero;

	public Vector3 CameraUp
	{
		get
		{
			return up;
		}
		set
		{
			if (value.X == 1f)
			{
				up.X = 1f;
			}
			else if (value.Y == 1f)
			{
				up.Y = 1f;
			}
			else if (value.Z == 1f)
			{
				up.Z = 1f;
			}
		}
	}

	public Camera(Vector3 up, float aspectRatio, float viewAngle, float nearClip, float farClip)
	{
		if (up.X != 1f || up.Y != 1f || up.Z != 1f)
		{
			up = Vector3.Up;
		}
		else
		{
			this.up = up;
		}
		this.aspectRatio = aspectRatio;
		ViewAngle = viewAngle;
		NearClip = nearClip;
		FarClip = farClip;
		Projection = Matrix.CreatePerspectiveFieldOfView(ViewAngle, aspectRatio, NearClip, FarClip);
		View = Matrix.CreateLookAt(Position, LookAt, up);
		Global.ResolutionChanged += on_ResolutionChanged;
	}

	public Camera(Vector3 up, float aspectRatio)
	{
		this.up = up;
		this.aspectRatio = aspectRatio;
		NearClip = -100f;
		Projection = Matrix.CreateOrthographicOffCenter((float)(-Global.ScreenWidth) / OrthoScale, (float)Global.ScreenWidth / OrthoScale, (float)(-Global.ScreenHeight) / OrthoScale, (float)Global.ScreenHeight / OrthoScale, NearClip, FarClip);
		View = Matrix.CreateLookAt(Position, LookAt, up);
		Global.ResolutionChanged += on_ResolutionChanged;
	}

	public Camera(float aspectRatio)
	{
		up = Vector3.Up;
		this.aspectRatio = aspectRatio;
		Projection = Matrix.CreatePerspectiveFieldOfView(ViewAngle, aspectRatio, NearClip, FarClip);
		View = Matrix.CreateLookAt(Position, LookAt, up);
		Global.ResolutionChanged += on_ResolutionChanged;
	}

	public virtual void Update()
	{
		View = Matrix.CreateLookAt(Position, LookAt, up);
	}

	public virtual void UpdateProjection()
	{
		ViewAngle = Math.Abs(ViewAngle);
		Projection = Matrix.CreatePerspectiveFieldOfView(ViewAngle, aspectRatio, NearClip, FarClip);
		Update();
	}

	public void UpdateOrthographicCam()
	{
		Matrix matrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
		Projection = Matrix.CreateOrthographicOffCenter((float)(-Global.ScreenWidth) / OrthoScale, (float)Global.ScreenWidth / OrthoScale, (float)(-Global.ScreenHeight) / OrthoScale, (float)Global.ScreenHeight / OrthoScale, NearClip, FarClip);
		View = Matrix.CreateLookAt(Position, LookAt, up) * matrix;
	}

	public void UpdateFlyingCamera(GameTime gameTime, float speed)
	{
		Vector2 left = Input.GetCurrGPS(PlayerIndex.One).ThumbSticks.Left;
		Vector2 right = Input.GetCurrGPS(PlayerIndex.One).ThumbSticks.Right;
		float left2 = Input.GetCurrGPS(PlayerIndex.One).Triggers.Left;
		float right2 = Input.GetCurrGPS(PlayerIndex.One).Triggers.Right;
		yaw += (0f - left.X) * (float)gameTime.ElapsedGameTime.TotalSeconds;
		pitch += (0f - left.Y) * (float)gameTime.ElapsedGameTime.TotalSeconds;
		Vector3 position = new Vector3(right.X, right.Y, right2 - left2);
		position *= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
		Matrix matrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
		position = Vector3.Transform(position, matrix);
		Position += position;
		matrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix.CreateTranslation(Position);
		LookAt = Position + Vector3.UnitX;
		LookAt = Vector3.Transform(LookAt, matrix);
		Update();
	}

	private void on_ResolutionChanged(object sender, EventArgs e)
	{
		aspectRatio = Global.AspectRatio;
		Projection = Matrix.CreatePerspectiveFieldOfView(ViewAngle, aspectRatio, NearClip, FarClip);
		View = Matrix.CreateLookAt(Position, LookAt, up);
	}
}
