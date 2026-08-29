using System.Collections.Generic;
using GKEngine.Cameras;
using GKEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GKEngine.Entities;

public class MeshData
{
	public List<Poly> triangles = new List<Poly>();

	public Vector3 max = Vector3.Zero;

	public Vector3 min = Vector3.Zero;

	public float radius = 1f;

	public Vector3 center = Vector3.Zero;

	public MeshData Clone()
	{
		MeshData meshData = new MeshData();
		meshData.min = min;
		meshData.max = max;
		meshData.radius = radius;
		meshData.center = center;
		for (int i = 0; i < triangles.Count; i++)
		{
			meshData.triangles.Add(new Poly(new Vector3(triangles[i].vertex[0].X, triangles[i].vertex[0].Y, triangles[i].vertex[0].Z), new Vector3(triangles[i].vertex[1].X, triangles[i].vertex[1].Y, triangles[i].vertex[1].Z), new Vector3(triangles[i].vertex[2].X, triangles[i].vertex[2].Y, triangles[i].vertex[2].Z), new Vector3(triangles[i].normal.X, triangles[i].normal.Y, triangles[i].normal.Z)));
		}
		return meshData;
	}

	public virtual void Dispose()
	{
		for (int i = 0; i < triangles.Count; i++)
		{
			triangles[i].Dispose();
			triangles[i] = null;
		}
		triangles.Clear();
		triangles = null;
	}

	public Poly Collide_ScreenXY(Vector2 vPoint, Matrix matrix, Camera camera)
	{
		Poly result = null;
		Matrix view = camera.view;
		Matrix projection = camera.projection;
		for (int i = 0; i < triangles.Count; i++)
		{
			Poly poly = triangles[i];
			Vector3 vector = Vector3.Transform(poly.vertex[0], matrix);
			Vector3 vector2 = Vector3.Transform(poly.vertex[1], matrix);
			Vector3 vector3 = Vector3.Transform(poly.vertex[2], matrix);
			Vector3 vector4 = Vector3.Transform(vector, Matrix.Identity * view * projection);
			Vector3 vector5 = Vector3.Transform(vector2, Matrix.Identity * view * projection);
			Vector3 vector6 = Vector3.Transform(vector3, Matrix.Identity * view * projection);
			if (vector4.Z > 0f && vector5.Z > 0f && vector6.Z > 0f)
			{
				Vector2 point = MathUtils.Vect3DTo2D(vector, view, projection, GameEngine.Graphics.GraphicsDevice.Viewport.Width, GameEngine.Graphics.GraphicsDevice.Viewport.Height);
				Vector2 point2 = MathUtils.Vect3DTo2D(vector2, view, projection, GameEngine.Graphics.GraphicsDevice.Viewport.Width, GameEngine.Graphics.GraphicsDevice.Viewport.Height);
				Vector2 point3 = MathUtils.Vect3DTo2D(vector3, view, projection, GameEngine.Graphics.GraphicsDevice.Viewport.Width, GameEngine.Graphics.GraphicsDevice.Viewport.Height);
				if (MathUtils.VectInTriangle(point, point2, point3, vPoint))
				{
					result = poly;
					break;
				}
			}
		}
		return result;
	}

	public float? Collide_ScreenRay(Vector2 vPoint, Matrix matrix, Camera camera)
	{
		float? result = null;
		Matrix view = camera.view;
		Matrix projection = camera.projection;
		for (int i = 0; i < triangles.Count; i++)
		{
			Poly poly = triangles[i];
			Vector3 vector = Vector3.Transform(poly.vertex[0], matrix);
			Vector3 vector2 = Vector3.Transform(poly.vertex[1], matrix);
			Vector3 vector3 = Vector3.Transform(poly.vertex[2], matrix);
			Vector2 point = MathUtils.Vect3DTo2D(vector, view, projection, camera.viewport.Width, camera.viewport.Height);
			Vector2 point2 = MathUtils.Vect3DTo2D(vector2, view, projection, camera.viewport.Width, camera.viewport.Height);
			Vector2 point3 = MathUtils.Vect3DTo2D(vector3, view, projection, camera.viewport.Width, camera.viewport.Height);
			if (MathUtils.VectInTriangle(point, point2, point3, vPoint))
			{
				Plane plane = new Plane(vector, vector2, vector3);
				float? result2 = camera.ScreenRay(vPoint).Intersects(plane);
				if (result2.HasValue)
				{
					return result2;
				}
			}
		}
		return result;
	}

	public static MeshData Read(ref ContentReader input)
	{
		MeshData meshData = new MeshData();
		meshData.min = input.ReadObject<Vector3>();
		meshData.max = input.ReadObject<Vector3>();
		meshData.radius = input.ReadSingle();
		meshData.center = input.ReadObject<Vector3>();
		int num = input.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Poly item = new Poly(input.ReadObject<Vector3>(), input.ReadObject<Vector3>(), input.ReadObject<Vector3>(), input.ReadObject<Vector3>());
			meshData.triangles.Add(item);
		}
		return meshData;
	}
}
