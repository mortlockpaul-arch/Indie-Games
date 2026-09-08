using System;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Elements.Cameras;

public class FirstPersonCamera : Camera
{
	protected float fov = (float)Math.PI / 4f;

	protected float aspectRatio = 1f;

	protected float zFar = 1000f;

	protected float heading;

	protected float pitch;

	protected float roll;

	public float ZFar
	{
		set
		{
			zFar = Math.Max(0.2f, Math.Abs(value));
			SetViewportSize(aspectRatio);
		}
	}

	public void SetViewportSize(float aspectRatio)
	{
		this.aspectRatio = aspectRatio;
		SetFOV(aspectRatio, fov);
	}

	public void SetFOV(float aspectRatio, float FOV)
	{
		this.aspectRatio = aspectRatio;
		fov = FOV;
		projection = Matrix.CreatePerspectiveFieldOfView(FOV, aspectRatio, 0.1f, zFar);
	}

	public FirstPersonCamera()
	{
		SetViewportSize(Engine.AspectRatio);
	}

	protected override void DoUpdate()
	{
		Quaternion rotation = Quaternion.CreateFromYawPitchRoll(heading, pitch, roll);
		rotation.Normalize();
		base.Transform.Rotation = rotation;
		base.DoUpdate();
	}
}
