using System;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Elements.Cameras;

public class OrbitCamera : Camera
{
	protected float heading;

	protected float pitch;

	protected float radius = 10f;

	private Transform target;

	public Transform Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
		}
	}

	public virtual void SetViewportSize(float aspectRatio)
	{
		projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, aspectRatio, 0.1f, 100000f);
	}

	public OrbitCamera()
	{
		SetViewportSize(Engine.AspectRatio);
	}

	protected override void DoUpdate()
	{
		Quaternion rotation = Quaternion.CreateFromYawPitchRoll(heading, pitch, 0f);
		rotation.Normalize();
		base.Transform.Rotation = rotation;
		base.Transform.Translation = target.Translation + base.Transform.ZVector * radius;
		base.Transform.LookAt(transform.Translation, transform.Translation - base.Transform.ZVector, base.Transform.YVector);
		base.DoUpdate();
	}
}
