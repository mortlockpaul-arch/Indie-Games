using System;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.Manifolds;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes;

/// <summary>
///  The local space data needed by a Terrain collidable.
///  Contains the Heightmap and other information.
/// </summary>
public class TerrainShape : CollisionShape
{
	private float[,] heights;

	private QuadTriangleOrganization quadTriangleOrganization;

	/// <summary>
	///  Gets or sets the height field of the terrain shape.
	/// </summary>
	public float[,] Heights
	{
		get
		{
			return heights;
		}
		set
		{
			heights = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Gets or sets the quad triangle organization.
	/// </summary>
	public QuadTriangleOrganization QuadTriangleOrganization
	{
		get
		{
			return quadTriangleOrganization;
		}
		set
		{
			quadTriangleOrganization = value;
			OnShapeChanged();
		}
	}

	/// <summary>
	///  Constructs a TerrainShape.
	/// </summary>
	/// <param name="heights">Heights array used for the shape.</param>
	/// <param name="triangleOrganization">Triangle organization of each quad.</param>
	/// <exception cref="T:System.ArgumentException">Thrown if the heights array has less than 2x2 vertices.</exception>
	public TerrainShape(float[,] heights, QuadTriangleOrganization triangleOrganization)
	{
		if (heights.GetLength(0) <= 1 || heights.GetLength(1) <= 1)
		{
			throw new ArgumentException("Terrains must have a least 2x2 vertices (one quad).");
		}
		this.heights = heights;
		quadTriangleOrganization = triangleOrganization;
	}

	/// <summary>
	///  Constructs a TerrainShape.
	/// </summary>
	/// <param name="heights">Heights array used for the shape.</param>
	public TerrainShape(float[,] heights)
		: this(heights, QuadTriangleOrganization.BottomLeftUpperRight)
	{
	}

	/// <summary>
	///  Constructs the bounding box of the terrain given a transform.
	/// </summary>
	/// <param name="transform">Transform to apply to the terrain during the bounding box calculation.</param>
	/// <param name="boundingBox">Bounding box of the terrain shape when transformed.</param>
	public void GetBoundingBox(ref AffineTransform transform, out BoundingBox boundingBox)
	{
		boundingBox = default(BoundingBox);
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		float num4 = float.MinValue;
		float num5 = float.MaxValue;
		float num6 = float.MinValue;
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		Vector3 vector4 = default(Vector3);
		Vector3 vector5 = default(Vector3);
		Vector3 vector6 = default(Vector3);
		for (int i = 0; i < heights.GetLength(0); i++)
		{
			for (int j = 0; j < heights.GetLength(1); j++)
			{
				Vector3 v = new Vector3(i, heights[i, j], j);
				Matrix3X3.Transform(ref v, ref transform.LinearTransform, out v);
				if (v.X < num)
				{
					num = v.X;
					vector = v;
				}
				else if (v.X > num2)
				{
					num2 = v.X;
					vector2 = v;
				}
				if (v.Y < num3)
				{
					num3 = v.Y;
					vector3 = v;
				}
				else if (v.Y > num4)
				{
					num4 = v.Y;
					vector4 = v;
				}
				if (v.Z < num5)
				{
					num5 = v.Z;
					vector5 = v;
				}
				else if (v.Z > num6)
				{
					num6 = v.Z;
					vector6 = v;
				}
			}
		}
		boundingBox.Min.X = vector.X + transform.Translation.X;
		boundingBox.Min.Y = vector3.Y + transform.Translation.Y;
		boundingBox.Min.Z = vector5.Z + transform.Translation.Z;
		boundingBox.Max.X = vector2.X + transform.Translation.X;
		boundingBox.Max.Y = vector4.Y + transform.Translation.Y;
		boundingBox.Max.Z = vector6.Z + transform.Translation.Z;
	}

	/// <summary>
	///  Tests a ray against the terrain shape.
	/// </summary>
	/// <param name="ray">Ray to test against the shape.</param>
	/// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="transform">Transform to apply to the terrain shape during the test.</param>
	/// <param name="hit">Hit data of the ray cast, if any.</param>
	/// <returns>Whether or not the ray hit the transformed terrain shape.</returns>
	public bool RayCast(ref Ray ray, float maximumLength, ref AffineTransform transform, out RayHit hit)
	{
		return RayCast(ref ray, maximumLength, ref transform, TriangleSidedness.Counterclockwise, out hit);
	}

	/// <summary>
	///  Tests a ray against the terrain shape.
	/// </summary>
	/// <param name="ray">Ray to test against the shape.</param>
	/// <param name="maximumLength">Maximum length of the ray in units of the ray direction's length.</param>
	/// <param name="transform">Transform to apply to the terrain shape during the test.</param>
	/// <param name="sidedness">Sidedness of the triangles to use when raycasting.</param>
	/// <param name="hit">Hit data of the ray cast, if any.</param>
	/// <returns>Whether or not the ray hit the transformed terrain shape.</returns>
	public bool RayCast(ref Ray ray, float maximumLength, ref AffineTransform transform, TriangleSidedness sidedness, out RayHit hit)
	{
		hit = default(RayHit);
		AffineTransform.Invert(ref transform, out var inverse);
		Ray ray2 = default(Ray);
		Matrix3X3.Transform(ref ray.Direction, ref inverse.LinearTransform, out ray2.Direction);
		AffineTransform.Transform(ref ray.Position, ref inverse, out ray2.Position);
		float num = heights.GetLength(0) - 1;
		float num2 = heights.GetLength(1) - 1;
		Vector3 value = ray2.Position;
		float num3 = 0f;
		if (value.X < 0f)
		{
			if (!(ray2.Direction.X > 0f))
			{
				return false;
			}
			float num4 = (0f - value.X) / ray2.Direction.X;
			num3 += num4;
			Vector3.Multiply(ref ray2.Direction, num4, out var result);
			Vector3.Add(ref result, ref value, out value);
		}
		else if (value.X > num)
		{
			if (!(ray2.Direction.X < 0f))
			{
				return false;
			}
			float num5 = (0f - (value.X - num)) / ray2.Direction.X;
			num3 += num5;
			Vector3.Multiply(ref ray2.Direction, num5, out var result2);
			Vector3.Add(ref result2, ref value, out value);
		}
		if (value.Z < 0f)
		{
			if (!(ray2.Direction.Z > 0f))
			{
				return false;
			}
			float num6 = (0f - value.Z) / ray2.Direction.Z;
			num3 += num6;
			Vector3.Multiply(ref ray2.Direction, num6, out var result3);
			Vector3.Add(ref result3, ref value, out value);
		}
		else if (value.Z > num2)
		{
			if (!(ray2.Direction.Z < 0f))
			{
				return false;
			}
			float num7 = (0f - (value.Z - num2)) / ray2.Direction.Z;
			num3 += num7;
			Vector3.Multiply(ref ray2.Direction, num7, out var result4);
			Vector3.Add(ref result4, ref value, out value);
		}
		if (num3 > maximumLength)
		{
			return false;
		}
		int num8 = (int)value.X;
		int num9 = (int)value.Z;
		if (num8 == heights.GetLength(0) - 1 && ray2.Direction.X < 0f)
		{
			num8 = heights.GetLength(0) - 2;
		}
		if (num9 == heights.GetLength(1) - 1 && ray2.Direction.Z < 0f)
		{
			num9 = heights.GetLength(1) - 2;
		}
		while (true)
		{
			if (num8 < 0 || num9 < 0 || num8 >= heights.GetLength(0) - 1 || num9 >= heights.GetLength(1) - 1)
			{
				return false;
			}
			GetLocalPosition(num8, num9, out var v);
			GetLocalPosition(num8 + 1, num9, out var v2);
			GetLocalPosition(num8, num9 + 1, out var v3);
			GetLocalPosition(num8 + 1, num9 + 1, out var v4);
			float y = v.Y;
			float y2 = v.Y;
			if (v2.Y > y)
			{
				y = v2.Y;
			}
			else if (v2.Y < y2)
			{
				y2 = v2.Y;
			}
			if (v3.Y > y)
			{
				y = v3.Y;
			}
			else if (v3.Y < y2)
			{
				y2 = v3.Y;
			}
			if (v4.Y > y)
			{
				y = v4.Y;
			}
			else if (v4.Y < y2)
			{
				y2 = v4.Y;
			}
			if ((!(value.Y > y) || !(ray2.Direction.Y > 0f)) && (!(value.Y < y2) || !(ray2.Direction.Y < 0f)))
			{
				bool flag;
				RayHit hit2;
				bool flag2;
				RayHit hit3;
				if (quadTriangleOrganization == QuadTriangleOrganization.BottomLeftUpperRight)
				{
					flag = Toolbox.FindRayTriangleIntersection(ref ray2, maximumLength, sidedness, ref v, ref v2, ref v3, out hit2);
					flag2 = Toolbox.FindRayTriangleIntersection(ref ray2, maximumLength, sidedness, ref v2, ref v4, ref v3, out hit3);
				}
				else
				{
					flag = Toolbox.FindRayTriangleIntersection(ref ray2, maximumLength, sidedness, ref v, ref v2, ref v4, out hit2);
					flag2 = Toolbox.FindRayTriangleIntersection(ref ray2, maximumLength, sidedness, ref v, ref v4, ref v3, out hit3);
				}
				if (flag && flag2)
				{
					if (hit2.T < hit3.T)
					{
						Vector3.Multiply(ref ray.Direction, hit2.T, out hit.Location);
						Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
						Matrix3X3.TransformTranspose(ref hit2.Normal, ref inverse.LinearTransform, out hit.Normal);
						hit.T = hit2.T;
						return true;
					}
					Vector3.Multiply(ref ray.Direction, hit3.T, out hit.Location);
					Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
					Matrix3X3.TransformTranspose(ref hit3.Normal, ref inverse.LinearTransform, out hit.Normal);
					hit.T = hit3.T;
					return true;
				}
				if (flag)
				{
					Vector3.Multiply(ref ray.Direction, hit2.T, out hit.Location);
					Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
					Matrix3X3.TransformTranspose(ref hit2.Normal, ref inverse.LinearTransform, out hit.Normal);
					hit.T = hit2.T;
					return true;
				}
				if (flag2)
				{
					Vector3.Multiply(ref ray.Direction, hit3.T, out hit.Location);
					Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
					Matrix3X3.TransformTranspose(ref hit3.Normal, ref inverse.LinearTransform, out hit.Normal);
					hit.T = hit3.T;
					return true;
				}
			}
			float num10 = ((ray2.Direction.X < 0f) ? ((0f - (value.X - (float)num8)) / ray2.Direction.X) : ((!(ray.Direction.X > 0f)) ? float.MaxValue : (((float)(num8 + 1) - value.X) / ray2.Direction.X)));
			float num11 = ((ray2.Direction.Z < 0f) ? ((0f - (value.Z - (float)num9)) / ray2.Direction.Z) : ((!(ray2.Direction.Z > 0f)) ? float.MaxValue : (((float)(num9 + 1) - value.Z) / ray2.Direction.Z)));
			if (num10 < num11)
			{
				num8 = ((!(ray2.Direction.X < 0f)) ? (num8 + 1) : (num8 - 1));
				num3 += num10;
				if (num3 > maximumLength)
				{
					return false;
				}
				Vector3.Multiply(ref ray2.Direction, num10, out var result5);
				Vector3.Add(ref result5, ref value, out value);
			}
			else
			{
				num9 = ((!(ray2.Direction.Z < 0f)) ? (num9 + 1) : (num9 - 1));
				num3 += num11;
				if (num3 > maximumLength)
				{
					break;
				}
				Vector3.Multiply(ref ray2.Direction, num11, out var result6);
				Vector3.Add(ref result6, ref value, out value);
			}
		}
		return false;
	}

	/// <summary>
	///  Gets the position of a vertex at the given indices in local space.
	/// </summary>
	/// <param name="i">Index in the first dimension.</param>
	/// <param name="j">Index in the second dimension.</param>
	/// <param name="v">Local space position at the given vertice.s</param>
	public void GetLocalPosition(int i, int j, out Vector3 v)
	{
		v = default(Vector3);
		v.X = i;
		v.Y = heights[i, j];
		v.Z = j;
	}

	/// <summary>
	/// Gets the world space position of a vertex in the terrain at the given indices.
	/// </summary>
	///             <param name="i">Index in the first dimension.</param>
	///             <param name="j">Index in the second dimension.</param>
	/// <param name="transform">Transform to apply to the vertex.</param>
	/// <param name="position">Transformed position of the vertex at the given indices.</param>
	public void GetPosition(int i, int j, ref AffineTransform transform, out Vector3 position)
	{
		if (i <= 0)
		{
			i = 0;
		}
		else if (i >= heights.GetLength(0))
		{
			i = heights.GetLength(0) - 1;
		}
		if (j <= 0)
		{
			j = 0;
		}
		else if (j >= heights.GetLength(1))
		{
			j = heights.GetLength(1) - 1;
		}
		position = default(Vector3);
		position.X = i;
		position.Y = heights[i, j];
		position.Z = j;
		AffineTransform.Transform(ref position, ref transform, out position);
	}

	/// <summary>
	/// Gets the world space normal at the given indices.
	/// </summary>
	///             <param name="i">Index in the first dimension.</param>
	///             <param name="j">Index in the second dimension.</param>
	/// <param name="transform">Transform to apply to the terrain while computing the normal.</param>
	/// <param name="normal">World space normal at the given indices.</param>
	public void GetNormal(int i, int j, ref AffineTransform transform, out Vector3 normal)
	{
		if (i <= 0)
		{
			i = 0;
		}
		else if (i >= heights.GetLength(0))
		{
			i = heights.GetLength(0) - 1;
		}
		if (j <= 0)
		{
			j = 0;
		}
		else if (j >= heights.GetLength(1))
		{
			j = heights.GetLength(1) - 1;
		}
		GetPosition(i, Math.Min(j + 1, heights.GetLength(1) - 1), ref transform, out var position);
		GetPosition(i, Math.Max(j - 1, 0), ref transform, out var position2);
		GetPosition(Math.Min(i + 1, heights.GetLength(0) - 1), j, ref transform, out var position3);
		GetPosition(Math.Max(i - 1, 0), j, ref transform, out var position4);
		Vector3.Subtract(ref position, ref position2, out var result);
		Vector3.Subtract(ref position3, ref position4, out normal);
		Vector3.Cross(ref result, ref normal, out normal);
		normal.Normalize();
	}

	/// <summary>
	///  Gets overlapped triangles with the terrain shape with a bounding box in the local space of the shape.
	/// </summary>
	/// <param name="localSpaceBoundingBox">Bounding box in the local space of the terrain shape.</param>
	/// <param name="overlappedTriangles">Triangles whose bounding boxes overlap the input bounding box.</param>
	public bool GetOverlaps(BoundingBox localSpaceBoundingBox, RawList<TriangleMeshConvexContactManifold.TriangleIndices> overlappedTriangles)
	{
		int length = heights.GetLength(0);
		int num = Math.Max((int)localSpaceBoundingBox.Min.X, 0);
		int num2 = Math.Max((int)localSpaceBoundingBox.Min.Z, 0);
		int num3 = Math.Min((int)localSpaceBoundingBox.Max.X, length - 2);
		int num4 = Math.Min((int)localSpaceBoundingBox.Max.Z, heights.GetLength(1) - 2);
		for (int i = num; i <= num3; i++)
		{
			for (int j = num2; j <= num4; j++)
			{
				float num5 = heights[i, j];
				float num6 = heights[i + 1, j];
				float num7 = heights[i, j + 1];
				float num8 = heights[i + 1, j + 1];
				float num9 = num5;
				float num10 = num5;
				if (num6 > num9)
				{
					num9 = num6;
				}
				else if (num6 < num10)
				{
					num10 = num6;
				}
				if (num7 > num9)
				{
					num9 = num7;
				}
				else if (num7 < num10)
				{
					num10 = num7;
				}
				if (num8 > num9)
				{
					num9 = num8;
				}
				else if (num8 < num10)
				{
					num10 = num8;
				}
				if (!(localSpaceBoundingBox.Max.Y < num10) && !(localSpaceBoundingBox.Min.Y > num9))
				{
					TriangleMeshConvexContactManifold.TriangleIndices item = default(TriangleMeshConvexContactManifold.TriangleIndices);
					if (quadTriangleOrganization == QuadTriangleOrganization.BottomLeftUpperRight)
					{
						item.A = i + j * length;
						item.B = i + 1 + j * length;
						item.C = i + (j + 1) * length;
						overlappedTriangles.Add(item);
						item.A = i + 1 + j * length;
						item.B = i + 1 + (j + 1) * length;
						item.C = i + (j + 1) * length;
						overlappedTriangles.Add(item);
					}
					else
					{
						item.A = i + j * length;
						item.B = i + 1 + j * length;
						item.C = i + 1 + (j + 1) * length;
						overlappedTriangles.Add(item);
						item.A = i + j * length;
						item.B = i + 1 + (j + 1) * length;
						item.C = i + (j + 1) * length;
						overlappedTriangles.Add(item);
					}
				}
			}
		}
		return overlappedTriangles.count > 0;
	}

	/// <summary>
	///  Gets overlapped triangles with the terrain shape with a bounding box in the local space of the shape.
	/// </summary>
	/// <param name="localBoundingBox">Bounding box in the local space of the terrain shape.</param>
	/// <param name="overlappedElements">Indices of elements whose bounding boxes overlap the input bounding box.</param>
	public bool GetOverlaps(BoundingBox localBoundingBox, RawList<int> overlappedElements)
	{
		int length = heights.GetLength(0);
		int num = Math.Max((int)localBoundingBox.Min.X, 0);
		int num2 = Math.Max((int)localBoundingBox.Min.Z, 0);
		int num3 = Math.Min((int)localBoundingBox.Max.X, length - 2);
		int num4 = Math.Min((int)localBoundingBox.Max.Z, heights.GetLength(1) - 2);
		for (int i = num; i <= num3; i++)
		{
			for (int j = num2; j <= num4; j++)
			{
				float num5 = heights[i, j];
				float num6 = heights[i + 1, j];
				float num7 = heights[i, j + 1];
				float num8 = heights[i + 1, j + 1];
				float num9 = num5;
				float num10 = num5;
				if (num6 > num9)
				{
					num9 = num6;
				}
				else if (num6 < num10)
				{
					num10 = num6;
				}
				if (num7 > num9)
				{
					num9 = num7;
				}
				else if (num7 < num10)
				{
					num10 = num7;
				}
				if (num8 > num9)
				{
					num9 = num8;
				}
				else if (num8 < num10)
				{
					num10 = num8;
				}
				if (!(localBoundingBox.Max.Y < num10) && !(localBoundingBox.Min.Y > num9))
				{
					int num11 = (i + j * length) * 2;
					overlappedElements.Add(num11);
					overlappedElements.Add(num11 + 1);
				}
			}
		}
		return overlappedElements.count > 0;
	}

	/// <summary>
	///  Gets a world space triangle in the terrain at the given indices (as if it were a mesh).
	/// </summary>
	/// <param name="indices">Indices of the triangle.</param>
	/// <param name="transform">Transform to apply to the triangle vertices.</param>
	/// <param name="a">First vertex of the triangle.</param>
	/// <param name="b">Second vertex of the triangle.</param>
	/// <param name="c">Third vertex of the triangle.</param>
	public void GetTriangle(ref TriangleMeshConvexContactManifold.TriangleIndices indices, ref AffineTransform transform, out Vector3 a, out Vector3 b, out Vector3 c)
	{
		int length = heights.GetLength(0);
		int num = indices.A / length;
		int i = indices.A - num * length;
		int num2 = indices.B / length;
		int i2 = indices.B - num2 * length;
		int num3 = indices.C / length;
		int i3 = indices.C - num3 * length;
		GetPosition(i, num, ref transform, out a);
		GetPosition(i2, num2, ref transform, out b);
		GetPosition(i3, num3, ref transform, out c);
	}

	/// <summary>
	///  Gets a world space triangle in the terrain at the given triangle index.
	/// </summary>
	/// <param name="index">Index of the triangle.</param>
	/// <param name="transform">Transform to apply to the triangle vertices.</param>
	/// <param name="a">First vertex of the triangle.</param>
	/// <param name="b">Second vertex of the triangle.</param>
	/// <param name="c">Third vertex of the triangle.</param>
	public void GetTriangle(int index, ref AffineTransform transform, out Vector3 a, out Vector3 b, out Vector3 c)
	{
		int num = index / 2;
		bool flag = num * 2 == index;
		int num2 = num / heights.GetLength(0);
		int num3 = num - num2 * heights.GetLength(0);
		if (quadTriangleOrganization == QuadTriangleOrganization.BottomLeftUpperRight)
		{
			if (flag)
			{
				GetPosition(num3, num2, ref transform, out a);
				GetPosition(num3 + 1, num2, ref transform, out b);
				GetPosition(num3, num2 + 1, ref transform, out c);
			}
			else
			{
				GetPosition(num3, num2 + 1, ref transform, out a);
				GetPosition(num3 + 1, num2 + 1, ref transform, out b);
				GetPosition(num3 + 1, num2, ref transform, out c);
			}
		}
		else if (flag)
		{
			GetPosition(num3, num2, ref transform, out a);
			GetPosition(num3 + 1, num2, ref transform, out b);
			GetPosition(num3 + 1, num2 + 1, ref transform, out c);
		}
		else
		{
			GetPosition(num3, num2, ref transform, out a);
			GetPosition(num3, num2 + 1, ref transform, out b);
			GetPosition(num3 + 1, num2 + 1, ref transform, out c);
		}
	}
}
