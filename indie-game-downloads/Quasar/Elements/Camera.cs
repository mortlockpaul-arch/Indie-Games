using System;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Elements;

public class Camera : Element
{
	protected Matrix projection = default(Matrix);

	private Matrix view;

	private Matrix viewProjection;

	private Plane[] frustumPlanes = new Plane[6];

	public Matrix Projection => projection;

	public Matrix View => view;

	public Matrix ViewProjection => viewProjection;

	public Ray GetPickingRay(Vector2 screenPosition)
	{
		Vector4 vector = UnprojectPoint(new Vector3(2f * (screenPosition.X / (float)Engine.BackBufferWidth), 2f * (screenPosition.Y / (float)Engine.BackBufferHeight), 0f));
		vector /= vector.W;
		Vector4 vector2 = UnprojectPoint(new Vector3(2f * (screenPosition.X / (float)Engine.BackBufferWidth), 2f * (screenPosition.Y / (float)Engine.BackBufferHeight), 1f));
		vector2 /= vector2.W;
		Vector3 direction = GameMath.ToVector3(vector2 - vector);
		direction.Normalize();
		return new Ray(GameMath.ToVector3(vector), direction);
	}

	public Vector4 ProjectPoint(Vector3 value)
	{
		return Vector4.Transform(new Vector4(value, 1f), ViewProjection);
	}

	public Vector4 UnprojectPoint(Vector3 value)
	{
		return Vector4.Transform(new Vector4(value, 1f), Matrix.Invert(ViewProjection));
	}

	public Vector4 UnprojectPoint(Vector4 value)
	{
		return Vector4.Transform(new Vector4(GameMath.ToVector3(value) / value.W, 1f), Matrix.Invert(ViewProjection));
	}

	public bool CheckCull(ref BoundingSphere sphere, Transform transform)
	{
		transform.TransformPoint(ref sphere.Center, out var result);
		return CheckCull(ref result, sphere.Radius * transform.ScaleBounds);
	}

	public bool CheckCull(BoundingSphere sphere, Transform transform)
	{
		Vector3 point = sphere.Center;
		transform.TransformPoint(ref point, out var result);
		return CheckCull(ref result, sphere.Radius * transform.ScaleBounds);
	}

	public bool CheckCull(ref Vector3 center, float radius)
	{
		if (radius == 0f)
		{
			return false;
		}
		Vector4 value = new Vector4(center, 1f);
		for (int i = 0; i < 6; i++)
		{
			if (frustumPlanes[i].Dot(value) + radius < 0f)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckCull(ref BoundingSphere sphere)
	{
		return CheckCull(ref sphere.Center, sphere.Radius);
	}

	public bool CheckCull(BoundingSphere sphere)
	{
		return CheckCull(ref sphere.Center, sphere.Radius);
	}

	protected override void DoUpdate()
	{
		base.DoUpdate();
		UpdateMatrix();
	}

	protected virtual void UpdateMatrix()
	{
		view = Matrix.Invert(base.Transform.WorldMatrix);
		Matrix.Multiply(ref view, ref projection, out viewProjection);
		UpdateFrustum();
	}

	protected void UpdateFrustum()
	{
		frustumPlanes[0].Normal = new Vector3(viewProjection.M14 + viewProjection.M11, viewProjection.M24 + viewProjection.M21, viewProjection.M34 + viewProjection.M31);
		frustumPlanes[0].D = viewProjection.M44 + viewProjection.M41;
		frustumPlanes[1].Normal = new Vector3(viewProjection.M14 - viewProjection.M11, viewProjection.M24 - viewProjection.M21, viewProjection.M34 - viewProjection.M31);
		frustumPlanes[1].D = viewProjection.M44 - viewProjection.M41;
		frustumPlanes[2].Normal = new Vector3(viewProjection.M14 - viewProjection.M12, viewProjection.M24 - viewProjection.M22, viewProjection.M34 - viewProjection.M32);
		frustumPlanes[2].D = viewProjection.M44 - viewProjection.M42;
		frustumPlanes[3].Normal = new Vector3(viewProjection.M14 + viewProjection.M12, viewProjection.M24 + viewProjection.M22, viewProjection.M34 + viewProjection.M32);
		frustumPlanes[3].D = viewProjection.M44 + viewProjection.M42;
		frustumPlanes[4].Normal = new Vector3(viewProjection.M13, viewProjection.M23, viewProjection.M33);
		frustumPlanes[4].D = viewProjection.M43;
		frustumPlanes[5].Normal = new Vector3(viewProjection.M14 - viewProjection.M13, viewProjection.M24 - viewProjection.M23, viewProjection.M34 - viewProjection.M33);
		frustumPlanes[5].D = viewProjection.M44 - viewProjection.M43;
		for (int i = 0; i < 6; i++)
		{
			frustumPlanes[i].Normalize();
		}
	}

	public void GetWVP(Transform transform, out Matrix result)
	{
		Transform.Multiply(transform, ref viewProjection, out result);
	}

	public static bool TransformProjections(Camera sourceCamera, Camera destCamera, Vector3 sourcePos, out Vector3 destPos)
	{
		destPos = Vector3.Zero;
		Vector4 vector = sourceCamera.ProjectPoint(sourcePos);
		if (Math.Abs(vector.W) <= 1E-05f)
		{
			return false;
		}
		Vector3 value = new Vector3(vector.X, vector.Y, vector.Z);
		value /= vector.W;
		Vector4 vector2 = destCamera.UnprojectPoint(value);
		if (Math.Abs(vector2.W) <= 1E-05f)
		{
			return false;
		}
		destPos = new Vector3(vector2.X, vector2.Y, vector2.Z);
		destPos /= vector2.W;
		return true;
	}
}
