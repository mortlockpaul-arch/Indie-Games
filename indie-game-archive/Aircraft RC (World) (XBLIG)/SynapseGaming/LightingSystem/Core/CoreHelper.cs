using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Core;

/// <summary />
public class CoreHelper
{
	private static float[] HCB = new float[4];

	private static BoundingFrustum HC_0002 = new BoundingFrustum(Matrix.Identity);

	private static Vector3[] HC_0012 = new Vector3[8];

	private static float HCH = 1f / (float)Math.Log(2.0);

	private static Random HC7 = new Random((int)DateTime.Now.Ticks);

	/// <summary />
	public static bool IsPowerOfTwo(int value)
	{
		return (value & (value - 1)) == 0;
	}

	/// <summary />
	public static void Decompose(Matrix matrix, out Vector3 scale, out Quaternion rotation, out Vector3 translation)
	{
		if (!matrix.Decompose(out scale, out rotation, out translation))
		{
			scale.X = matrix.Right.Length();
			scale.Y = matrix.Up.Length();
			scale.Z = matrix.Forward.Length();
			translation = matrix.Translation;
			Matrix matrix2 = default(Matrix);
			if (scale.X > 0f)
			{
				matrix2.Right = matrix.Right / scale.X;
			}
			if (scale.Y > 0f)
			{
				matrix2.Up = matrix.Up / scale.Y;
			}
			if (scale.Z > 0f)
			{
				matrix2.Forward = matrix.Forward / scale.Z;
			}
			matrix2.M44 = 1f;
			rotation = Quaternion.CreateFromRotationMatrix(matrix2);
		}
	}

	/// <summary />
	public static bool IsDegenerate(ref Vector3 a, ref Vector3 b, ref Vector3 c)
	{
		float num = b.X - a.X;
		float num2 = b.Y - a.Y;
		float num3 = b.Z - a.Z;
		float num4 = c.X - a.X;
		float num5 = c.Y - a.Y;
		float num6 = c.Z - a.Z;
		float num7 = num2 * num6 - num3 * num5;
		float num8 = num3 * num4 - num * num6;
		float num9 = num * num5 - num2 * num4;
		float num10 = num7 * num7 + num8 * num8 + num9 * num9;
		return num10 <= 0f;
	}

	/// <summary />
	public static float GetGravityForTimePeriod(double gravity, double seconds)
	{
		if (seconds <= 0.0)
		{
			return 0f;
		}
		double num = 1.0 / seconds;
		double num2 = 0.5 * num * (num - 1.0);
		if (num2 <= 0.0)
		{
			return 0f;
		}
		double num3 = gravity / num2;
		return (float)num3;
	}

	/// <summary />
	public static int GetUniqueId(object obj)
	{
		return GetHashCode(obj.GetHashCode(), HC7.Next());
	}

	/// <summary />
	public static float Log2(float a)
	{
		return (float)Math.Log(a) * HCH;
	}

	/// <summary />
	public static void DivergeLighting(Vector3 color, float strength, float redweight, out Vector3 color_high, out Vector3 color_low)
	{
		float num = 1f + strength;
		float num2 = 1f - strength;
		color_high = color * 2f * new Vector3(num, 1f, num2) * redweight;
		color_low = color * 2f * new Vector3(num2, 1f, num) * (1f - redweight);
	}

	/// <summary />
	public static string GetDisplayName(INamedObject obj)
	{
		Type type = obj.GetType();
		string text = obj.Name;
		if (text == null)
		{
			text = string.Empty;
		}
		return $"\"{text}\" - {type.Name}";
	}

	/// <summary />
	public static Vector3 ReplaceVectorIndex(Vector3 source, int index, float newvalue)
	{
		HCB[0] = source.X;
		HCB[1] = source.Y;
		HCB[2] = source.Z;
		HCB[index] = newvalue;
		return new Vector3(HCB[0], HCB[1], HCB[2]);
	}

	/// <summary />
	public static Vector3 SwizzleVector(Vector3 source, int xindex, int yindex, int zindex)
	{
		HCB[0] = source.X;
		HCB[1] = source.Y;
		HCB[2] = source.Z;
		return new Vector3(HCB[xindex], HCB[yindex], HCB[zindex]);
	}

	/// <summary />
	public static BoundingBox TransformBoundingBox(BoundingBox boundingbox, Matrix transform)
	{
		boundingbox.GetCorners(HC_0012);
		for (int i = 0; i < HC_0012.Length; i++)
		{
			ref Vector3 reference = ref HC_0012[i];
			reference = Vector3.Transform(HC_0012[i], transform);
		}
		return CreateBoundingBoxFromPoints(HC_0012);
	}

	/// <summary />
	public static BoundingSphere TransformBoundingSphereSlow(BoundingSphere sourceboundingsphere, Matrix transform)
	{
		TransformBoundingSphere(ref sourceboundingsphere, ref transform, out var destboundingsphere);
		return destboundingsphere;
	}

	/// <summary />
	public static void TransformBoundingSphere(ref BoundingSphere sourceboundingsphere, ref Matrix transform, out BoundingSphere destboundingsphere)
	{
		Vector3 vector = default(Vector3);
		vector.X = new Vector3(transform.M11, transform.M12, transform.M13).Length();
		vector.Y = new Vector3(transform.M21, transform.M22, transform.M23).Length();
		vector.Z = new Vector3(transform.M31, transform.M32, transform.M33).Length();
		float num = Math.Max(Math.Abs(vector.X), Math.Max(Math.Abs(vector.Y), Math.Abs(vector.Z)));
		Vector3.Transform(ref sourceboundingsphere.Center, ref transform, out destboundingsphere.Center);
		destboundingsphere.Radius = sourceboundingsphere.Radius * num;
	}

	/// <summary />
	public static BoundingBox PaddedBoundingBox(BoundingBox boundingbox, float scale)
	{
		Vector3 vector = (boundingbox.Max - boundingbox.Min) * 0.5f;
		Vector3 vector2 = boundingbox.Max - vector;
		Vector3 vector3 = vector * scale;
		return new BoundingBox(vector2 - vector3, vector2 + vector3);
	}

	/// <summary />
	public static BoundingSphere PaddedBoundingSphere(BoundingSphere boundingsphere, float scale)
	{
		BoundingSphere result = boundingsphere;
		result.Radius *= scale;
		return result;
	}

	/// <summary />
	public static Vector3 ProjectToPlane(Vector3 point, Plane plane)
	{
		float num = plane.DotCoordinate(point);
		return point - plane.Normal * num;
	}

	/// <summary />
	public static bool Intersects(Vector3 start, Vector3 end, Plane plane, ref Vector3 intersectionpoint)
	{
		Vector3 vector = end - start;
		float num = Vector3.Dot(plane.Normal, vector);
		if (num == 0f)
		{
			return false;
		}
		float num2 = (0f - (Vector3.Dot(plane.Normal, start) + plane.D)) / num;
		if (num2 >= 0f && num2 <= 1f)
		{
			intersectionpoint = start + num2 * vector;
			return true;
		}
		return false;
	}

	/// <summary />
	public static BoundingBox CreateBoundingBoxFromPoints(Vector3[] points)
	{
		if (points.Length < 1)
		{
			return default(BoundingBox);
		}
		BoundingBox result = new BoundingBox(points[0], points[0]);
		for (int i = 1; i < points.Length; i++)
		{
			result.Max = Vector3.Max(result.Max, points[i]);
			result.Min = Vector3.Min(result.Min, points[i]);
		}
		return result;
	}

	/// <summary />
	public static Plane CreatePlane(Vector3 vector, Vector3 position)
	{
		Plane result = default(Plane);
		result.D = 0f - Vector3.Dot(position, result.Normal = Vector3.Normalize(vector));
		return result;
	}

	/// <summary />
	public static Matrix CreateMatrix(Vector3 row1, Vector3 row2, Vector3 row3, Vector3 translate)
	{
		return new Matrix(row1.X, row1.Y, row1.Z, 0f, row2.X, row2.Y, row2.Z, 0f, row3.X, row3.Y, row3.Z, 0f, translate.X, translate.Y, translate.Z, 1f);
	}

	/// <summary />
	public static Matrix CreateMatrixFromNormalizedVectors(Vector3 root, Vector3 current)
	{
		float num = Vector3.Dot(root, current);
		float num2 = 1f + num;
		Quaternion quaternion;
		if ((double)num2 < 0.0001)
		{
			quaternion = new Quaternion(root.Z, root.X, root.Y, 0f);
		}
		else
		{
			Vector3 vector = Vector3.Cross(root, current);
			quaternion = new Quaternion(vector.X, vector.Y, vector.Z, num2);
		}
		quaternion.Normalize();
		return Matrix.CreateFromQuaternion(quaternion);
	}

	/// <summary />
	public static Matrix CreateMatrixFromNormalizedVectors(Vector3 root, Vector3 current, Vector3 axis)
	{
		float num = MathHelper.Clamp(Vector3.Dot(root, current), 0f, 1f);
		Quaternion quaternion = Quaternion.CreateFromAxisAngle(axis, (float)Math.Acos(num));
		return Matrix.CreateFromQuaternion(quaternion);
	}

	/// <summary />
	public static int GetHashCode(int hash1, int hash2)
	{
		return (hash1 ^ 1) + (hash2 ^ 2);
	}

	/// <summary />
	public static int GetHashCode(int hash1, int hash2, int hash3)
	{
		return (hash1 ^ 1) + (hash2 ^ 2) + (hash3 ^ 3);
	}

	/// <summary />
	public static int GetHashCode(int hash1, int hash2, int hash3, int hash4)
	{
		return (hash1 ^ 1) + (hash2 ^ 2) + (hash3 ^ 3) + (hash4 ^ 4);
	}

	/// <summary />
	public static int GetTextureElementCount(int width, int height, int miplevels)
	{
		int num = 0;
		for (int i = 0; i < miplevels; i++)
		{
			num += width * height;
			width >>= 1;
			height >>= 1;
			width = Math.Max(width, 1);
			width = Math.Max(height, 1);
		}
		return num;
	}

	/// <summary />
	public static Vector3 GetAnglesRadians(Quaternion rotation)
	{
		float w = rotation.W;
		float y = rotation.Y;
		float x = rotation.X;
		float z = rotation.Z;
		return new Vector3
		{
			X = (float)Math.Atan2(2f * (w * y + x * z), 1.0 - 2.0 * (Math.Pow(y, 2.0) + Math.Pow(x, 2.0))),
			Y = (float)Math.Asin(2f * (w * x - z * y)),
			Z = (float)Math.Atan2(2f * (w * z + y * x), 1.0 - 2.0 * (Math.Pow(x, 2.0) + Math.Pow(z, 2.0)))
		};
	}

	/// <summary />
	public static float GetClosestPointOnBoxAndDistance(ref BoundingBox box, ref Vector3 position, out Vector3 closestpoint)
	{
		Vector3.Clamp(ref position, ref box.Min, ref box.Max, out closestpoint);
		Vector3.DistanceSquared(ref position, ref closestpoint, out var result);
		return result;
	}

	/// <summary />
	public static float GetClosestPointOnBoxAndDistanceWithNormal(ref BoundingBox box, ref Vector3 position, out Vector3 closestpoint, out Vector3 surfacenormal)
	{
		closestpoint = position;
		surfacenormal = Vector3.UnitY;
		if (closestpoint.X > box.Max.X)
		{
			closestpoint.X = box.Max.X;
			surfacenormal = Vector3.UnitX;
		}
		else if (closestpoint.X < box.Min.X)
		{
			closestpoint.X = box.Min.X;
			surfacenormal = -Vector3.UnitX;
		}
		if (closestpoint.Z > box.Max.Z)
		{
			closestpoint.Z = box.Max.Z;
			surfacenormal = Vector3.UnitZ;
		}
		else if (closestpoint.Z < box.Min.Z)
		{
			closestpoint.Z = box.Min.Z;
			surfacenormal = -Vector3.UnitZ;
		}
		if (closestpoint.Y > box.Max.Y)
		{
			closestpoint.Y = box.Max.Y;
			surfacenormal = Vector3.UnitY;
		}
		else if (closestpoint.Y < box.Min.Y)
		{
			closestpoint.Y = box.Min.Y;
			surfacenormal = -Vector3.UnitY;
		}
		Vector3.DistanceSquared(ref position, ref closestpoint, out var result);
		return result;
	}

	/// <summary />
	public static void ConvertToMajorAxis(ref Vector3 vector)
	{
		if (vector == Vector3.Zero)
		{
			vector = Vector3.UnitY;
			return;
		}
		float num = Math.Abs(vector.X);
		float num2 = Math.Abs(vector.Y);
		float num3 = Math.Abs(vector.Z);
		if (num > num2)
		{
			if (num > num3)
			{
				vector.Y = 0f;
				vector.Z = 0f;
			}
			else
			{
				vector.X = 0f;
				vector.Y = 0f;
			}
		}
		else if (num2 > num3)
		{
			vector.X = 0f;
			vector.Z = 0f;
		}
		else
		{
			vector.X = 0f;
			vector.Y = 0f;
		}
		vector.Normalize();
	}

	/// <summary />
	public static Vector3 GetClosestPointOnLineSegment(Vector3 point, Vector3 linepointa, Vector3 linepointb)
	{
		float num = Vector3.DistanceSquared(linepointa, linepointb);
		if (num <= 0f)
		{
			return linepointa;
		}
		Vector3 vector = point - linepointa;
		Vector3 vector2 = linepointb - linepointa;
		float value = Vector3.Dot(vector, vector2) / num;
		value = MathHelper.Clamp(value, 0f, 1f);
		return linepointa + value * (linepointb - linepointa);
	}

	/// <summary />
	public static bool GetIntersection(Plane a, Plane b, Plane c, out Vector3 point)
	{
		Vector3 vector = Vector3.Cross(b.Normal, c.Normal);
		float num = Vector3.Dot(a.Normal, vector);
		if (num == 0f)
		{
			point = default(Vector3);
			return false;
		}
		point = a.D * vector + b.D * Vector3.Cross(c.Normal, a.Normal) + c.D * Vector3.Cross(a.Normal, b.Normal);
		point /= 0f - num;
		return true;
	}

	/// <summary />
	public static float GetScreenSize(float size, float depth, Matrix projection)
	{
		Vector4 vector = new Vector4(size, size, 0f - depth, 1f);
		Vector4 vector2 = Vector4.Transform(vector, projection);
		if (vector2.W <= 0f)
		{
			return 0f;
		}
		vector2 /= vector2.W;
		float num = Math.Max(Math.Abs(vector2.X), Math.Abs(vector2.Y));
		if (num <= 0f)
		{
			return 0f;
		}
		return num;
	}

	/// <summary />
	public static float GetScreenSize(float size, Matrix world, Matrix view, Matrix projection)
	{
		Vector4 vector = Vector4.Transform(new Vector4(world.Translation, 1f), view);
		vector.X = size;
		vector.Y = size;
		Vector4 vector2 = Vector4.Transform(vector, projection);
		if (vector2.W <= 0f)
		{
			return 0f;
		}
		vector2 /= vector2.W;
		float num = Math.Max(Math.Abs(vector2.X), Math.Abs(vector2.Y));
		if (num <= 0f)
		{
			return 0f;
		}
		return num;
	}

	/// <summary />
	public static float GetScreenScale(Matrix world, Matrix view, Matrix projection)
	{
		float screenSize = GetScreenSize(200f, world, view, projection);
		if (screenSize > 0f)
		{
			return 1f / screenSize;
		}
		return 0f;
	}

	/// <summary />
	public static bool Unproject(Vector3 screenpos, Viewport viewport, Matrix world, Matrix view, Matrix invview, Matrix projection, ref Vector3 outpos)
	{
		if (screenpos.X < (float)viewport.X || screenpos.X - (float)viewport.X > (float)viewport.Width || screenpos.Y < (float)viewport.Y || screenpos.Y - (float)viewport.Y > (float)viewport.Height)
		{
			return false;
		}
		outpos = viewport.Unproject(screenpos, projection, view, world);
		HC_0002.Matrix = view * projection;
		if (HC_0002.Near.DotCoordinate(outpos) > 0f)
		{
			outpos = invview.Translation - (outpos - invview.Translation);
		}
		return true;
	}

	/// <summary />
	public static bool Project(Vector3 inpos, Viewport viewport, Matrix world, Matrix view, Matrix invview, Matrix projection, ref Vector3 screenpos)
	{
		if (CreatePlane(invview.Forward, invview.Translation).DotCoordinate(inpos) <= 0f)
		{
			return false;
		}
		screenpos = viewport.Project(inpos, projection, view, world);
		return true;
	}

	/// <summary />
	public static Rectangle GetScreenArea(BoundingBox worldbounds, Viewport viewport, Matrix viewprojection, Matrix invview)
	{
		if (worldbounds.Contains(invview.Translation) == ContainmentType.Contains)
		{
			return new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
		}
		worldbounds.GetCorners(HC_0012);
		for (int i = 0; i < 8; i++)
		{
			Vector4 vector = Vector4.Transform(HC_0012[i], viewprojection);
			if (vector.W == 0f)
			{
				return new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
			}
			if (vector.W < 0f)
			{
				vector /= vector.W * -0.5f;
			}
			else
			{
				vector /= vector.W;
			}
			vector.Y *= -1f;
			vector = vector * 0.5f + Vector4.One * 0.5f;
			ref Vector3 reference = ref HC_0012[i];
			reference = Vector3.Clamp(new Vector3(vector.X, vector.Y, vector.Z), Vector3.Zero, Vector3.One) * new Vector3(viewport.Width, viewport.Height, 0f);
		}
		BoundingBox boundingBox = CreateBoundingBoxFromPoints(HC_0012);
		return new Rectangle(viewport.X + (int)boundingBox.Min.X, viewport.Y + (int)boundingBox.Min.Y, (int)boundingBox.Max.X - (int)boundingBox.Min.X, (int)boundingBox.Max.Y - (int)boundingBox.Min.Y);
	}

	internal static Texture2D C(GraphicsDevice P_0, Texture2D P_1)
	{
		return P_1;
	}
}
