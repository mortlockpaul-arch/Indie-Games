using System;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Elements.Cameras;

public class LookAtCamera : Camera
{
	protected Vector3 upVector = Vector3.UnitY;

	protected Vector3 target = Vector3.Zero;

	public Vector3 UpVector
	{
		get
		{
			return upVector;
		}
		set
		{
			upVector = value;
		}
	}

	public Vector3 Target
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

	public void SetViewportSize(float aspectRatio)
	{
		SetViewportSize(aspectRatio, (float)Math.PI / 4f);
	}

	public void SetViewportSize(float aspectRatio, float fov)
	{
		projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, 0.1f, 100000f);
	}

	public void SetOrthoProjection(Vector2 screenSize, float zNear, float zFar)
	{
		projection = Matrix.CreateOrthographic(screenSize.X, screenSize.Y, zNear, zFar);
	}

	public LookAtCamera()
	{
		SetViewportSize(Engine.AspectRatio);
	}

	public LookAtCamera(Vector3 position, Vector3 target, Vector3 upVector)
		: this()
	{
		transform.Translation = position;
		base.Transform.LookAt(base.Transform.Translation, target, upVector);
		this.target = target;
		this.upVector = upVector;
	}

	public LookAtCamera(Vector3 position, Vector3 target)
		: this(position, target, Vector3.UnitY)
	{
	}

	protected override void DoUpdate()
	{
		base.Transform.LookAt(base.Transform.Translation, target, upVector);
		base.DoUpdate();
	}
}
