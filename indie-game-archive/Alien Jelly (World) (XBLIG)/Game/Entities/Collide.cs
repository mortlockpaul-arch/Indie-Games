using GKEngine.Entities;
using GKEngine.Utils;
using Microsoft.Xna.Framework;

namespace Game.Entities;

public class Collide
{
	public const float RAY_Y_START = 10000f;

	public Vector3 position;

	public Vector3 normal;

	public Collide(Vector3 vPosition, Vector3 vNormal)
	{
		position = vPosition;
		normal = vNormal;
	}

	public Collide()
	{
		position = Vector3.Zero;
		normal = Vector3.Up;
	}

	public static Collide YAt(float xX, float xZ, MeshData mesh)
	{
		Collide result = null;
		for (int i = 0; i < mesh.triangles.Count; i++)
		{
			Poly poly = mesh.triangles[i];
			if (MathUtils.VectInTriangle(new Vector2(poly.vertex[0].X, poly.vertex[0].Z), new Vector2(poly.vertex[1].X, poly.vertex[1].Z), new Vector2(poly.vertex[2].X, poly.vertex[2].Z), new Vector2(xX, xZ)))
			{
				Plane plane = new Plane(poly.vertex[0], poly.vertex[1], poly.vertex[2]);
				float? num = new Ray(new Vector3(xX, 10000f, xZ), Vector3.Down).Intersects(plane);
				if (num.HasValue)
				{
					result = new Collide(new Vector3(xX, 10000f - num.Value, xZ), poly.normal);
					break;
				}
			}
		}
		return result;
	}

	public static Collide Intersect(Vector3 vStart, Vector3 vEnd, MeshData mesh, Matrix meshMatrix)
	{
		Collide result = null;
		Vector3 vector = Vector3.Normalize(vEnd - vStart);
		float num = Vector3.Distance(vStart, vEnd);
		Vector3 zero = Vector3.Zero;
		Vector3 zero2 = Vector3.Zero;
		Vector3 zero3 = Vector3.Zero;
		Vector3 zero4 = Vector3.Zero;
		Vector3 zero5 = Vector3.Zero;
		Vector3 zero6 = Vector3.Zero;
		Matrix matrix = Matrix.Invert(Matrix.Multiply(Matrix.CreateBillboard(Vector3.Zero, vector, Vector3.Up, Vector3.Forward), Matrix.CreateTranslation(vStart)));
		Vector3.Transform(vStart, matrix);
		for (int i = 0; i < mesh.triangles.Count; i++)
		{
			Poly poly = mesh.triangles[i];
			zero4 = Vector3.Transform(poly.vertex[0], meshMatrix);
			zero5 = Vector3.Transform(poly.vertex[1], meshMatrix);
			zero6 = Vector3.Transform(poly.vertex[2], meshMatrix);
			zero4 = poly.vertex[0];
			zero5 = poly.vertex[1];
			zero6 = poly.vertex[2];
			zero = Vector3.Transform(zero4, matrix);
			zero2 = Vector3.Transform(zero5, matrix);
			zero3 = Vector3.Transform(zero6, matrix);
			if (MathUtils.VectInTriangle(new Vector2(zero.X, zero.Y), new Vector2(zero2.X, zero2.Y), new Vector2(zero3.X, zero3.Y), new Vector2(0f, 0f)))
			{
				Plane plane = new Plane(zero4, zero5, zero6);
				float? num2 = new Ray(vStart, vector).Intersects(plane);
				if (num2.HasValue && num2 <= num)
				{
					result = new Collide(vStart + vector * num2.Value, poly.normal);
					break;
				}
			}
		}
		return result;
	}
}
