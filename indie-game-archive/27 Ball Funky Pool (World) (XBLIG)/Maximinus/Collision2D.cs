using System;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class Collision2D
{
	public static bool IntersectPixels(Rectangle rectA, bool[] dataA, Rectangle rectB, bool[] dataB)
	{
		Point texCoordInObjA = Point.Zero;
		return IntersectPixels(rectA, dataA, rectB, dataB, Vector2.Zero, ref texCoordInObjA, searchForClosestPoint: false);
	}

	public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
	{
		ComputeBounds(rectangleA, rectangleB, out var top, out var bottom, out var left, out var right);
		for (int i = top; i < bottom; i++)
		{
			for (int j = left; j < right; j++)
			{
				bool flag = dataA[Index2DArray(rectangleA, j, i)].A > 0;
				bool flag2 = dataB[Index2DArray(rectangleB, j, i)].A > 0;
				if (flag && flag2)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, bool[] dataB, Vector2 velocity, ref Point texCoordInObjA, bool searchForClosestPoint)
	{
		bool flag = false;
		Point texCoordMin = new Point(-1, -1);
		ComputeBounds(rectangleA, rectangleB, out var top, out var bottom, out var left, out var right);
		for (int i = top; i < bottom; i++)
		{
			for (int j = left; j < right; j++)
			{
				bool flag2 = dataA[Index2DArray(rectangleA, j, i)].A > 0;
				bool flag3 = dataB[Index2DArray(rectangleB, j, i)];
				if (flag2 && flag3)
				{
					texCoordInObjA.X = j - rectangleA.Left;
					texCoordInObjA.Y = i - rectangleA.Top;
					if (!searchForClosestPoint)
					{
						return true;
					}
					if (!flag)
					{
						texCoordMin.X = texCoordInObjA.X;
						texCoordMin.Y = texCoordInObjA.Y;
						flag = true;
					}
					else if (CheckMinimumPoint(texCoordMin, texCoordInObjA, velocity))
					{
						texCoordMin.X = texCoordInObjA.X;
						texCoordMin.Y = texCoordInObjA.Y;
					}
				}
			}
		}
		if (searchForClosestPoint && flag)
		{
			texCoordInObjA.X = texCoordMin.X;
			texCoordInObjA.Y = texCoordMin.Y;
			return true;
		}
		return false;
	}

	private static void ComputeBounds(Rectangle rectangleA, Rectangle rectangleB, out int top, out int bottom, out int left, out int right)
	{
		top = Math.Max(rectangleA.Top, rectangleB.Top);
		bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
		left = Math.Max(rectangleA.Left, rectangleB.Left);
		right = Math.Min(rectangleA.Right, rectangleB.Right);
	}

	private static int Index2DArray(Rectangle rectangleA, int x, int y)
	{
		return x - rectangleA.Left + (y - rectangleA.Top) * rectangleA.Width;
	}

	private static bool CheckMinimumPoint(Point texCoordMin, Point texCoordInObjA, Vector2 velocity)
	{
		if (Math.Sign(texCoordMin.X - texCoordInObjA.X) != Math.Sign(velocity.X))
		{
			return Math.Sign(texCoordMin.Y - texCoordInObjA.Y) == Math.Sign(velocity.Y);
		}
		return true;
	}

	public static bool IntersectPixels(Rectangle rectangleA, bool[] dataA, Rectangle rectangleB, bool[] dataB, Vector2 velocity, ref Point texCoordInObjA, bool searchForClosestPoint)
	{
		bool flag = false;
		Point texCoordMin = new Point(-1, -1);
		ComputeBounds(rectangleA, rectangleB, out var top, out var bottom, out var left, out var right);
		for (int i = top; i < bottom; i++)
		{
			for (int j = left; j < right; j++)
			{
				bool flag2 = dataA[Index2DArray(rectangleA, j, i)];
				bool flag3 = dataB[Index2DArray(rectangleB, j, i)];
				if (flag2 && flag3)
				{
					texCoordInObjA.X = j - rectangleA.Left;
					texCoordInObjA.Y = i - rectangleA.Top;
					if (!searchForClosestPoint)
					{
						return true;
					}
					if (!flag)
					{
						texCoordMin.X = texCoordInObjA.X;
						texCoordMin.Y = texCoordInObjA.Y;
						flag = true;
					}
					else if (CheckMinimumPoint(texCoordMin, texCoordInObjA, velocity))
					{
						texCoordMin.X = texCoordInObjA.X;
						texCoordMin.Y = texCoordInObjA.Y;
					}
				}
			}
		}
		if (searchForClosestPoint && flag)
		{
			texCoordInObjA.X = texCoordMin.X;
			texCoordInObjA.Y = texCoordMin.Y;
			return true;
		}
		return false;
	}

	public static bool IntersectPixels(Matrix transformA, int widthA, int heightA, bool[] dataA, Matrix transformB, int widthB, int heightB, bool[] dataB, ref Vector2 commonPointInObject1Data)
	{
		Matrix matrix = transformA * Matrix.Invert(transformB);
		Vector2 vector = Vector2.TransformNormal(Vector2.UnitX, matrix);
		Vector2 vector2 = Vector2.TransformNormal(Vector2.UnitY, matrix);
		Vector2 vector3 = Vector2.Transform(Vector2.Zero, matrix);
		for (int i = 0; i < heightA; i++)
		{
			Vector2 vector4 = vector3;
			for (int j = 0; j < widthA; j++)
			{
				int num = (int)Math.Round(vector4.X);
				int num2 = (int)Math.Round(vector4.Y);
				if (0 <= num && num < widthB && 0 <= num2 && num2 < heightB)
				{
					bool flag = dataA[j + i * widthA];
					bool flag2 = dataB[num + num2 * widthB];
					if (flag && flag2)
					{
						commonPointInObject1Data = new Vector2(j, i);
						return true;
					}
				}
				vector4 += vector;
			}
			vector3 += vector2;
		}
		return false;
	}

	public static bool IntersectPixels(Matrix transformA, int widthA, int heightA, bool[] dataA, Matrix transformB, int widthB, int heightB, bool[] dataB)
	{
		Vector2 commonPointInObject1Data = Vector2.Zero;
		return IntersectPixels(transformA, widthA, heightA, dataA, transformB, widthB, heightB, dataB, ref commonPointInObject1Data);
	}

	public static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
	{
		Vector2 position = new Vector2(rectangle.Left, rectangle.Top);
		Vector2 position2 = new Vector2(rectangle.Right, rectangle.Top);
		Vector2 position3 = new Vector2(rectangle.Left, rectangle.Bottom);
		Vector2 position4 = new Vector2(rectangle.Right, rectangle.Bottom);
		Vector2.Transform(ref position, ref transform, out position);
		Vector2.Transform(ref position2, ref transform, out position2);
		Vector2.Transform(ref position3, ref transform, out position3);
		Vector2.Transform(ref position4, ref transform, out position4);
		Vector2 vector = Vector2.Min(Vector2.Min(position, position2), Vector2.Min(position3, position4));
		Vector2 vector2 = Vector2.Max(Vector2.Max(position, position2), Vector2.Max(position3, position4));
		return new Rectangle((int)vector.X, (int)vector.Y, (int)(vector2.X - vector.X), (int)(vector2.Y - vector.Y));
	}

	public static bool LineIntersectLine(Line lineA, Line lineB, out Vector2? intersection)
	{
		intersection = null;
		float num = (lineB.End.Y - lineB.Start.Y) * (lineA.End.X - lineA.Start.X) - (lineB.End.X - lineB.Start.X) * (lineA.End.Y - lineA.Start.Y);
		float num2 = (lineB.End.X - lineB.Start.X) * (lineA.Start.Y - lineB.Start.Y) - (lineB.End.Y - lineB.Start.Y) * (lineA.Start.X - lineB.Start.X);
		float num3 = (lineA.End.X - lineA.Start.X) * (lineA.Start.Y - lineB.Start.Y) - (lineA.End.Y - lineA.Start.Y) * (lineA.Start.X - lineB.Start.X);
		if (num == 0f)
		{
			if (num2 == 0f && num3 == 0f)
			{
				return false;
			}
			return false;
		}
		float num4 = num2 / num;
		float num5 = num3 / num;
		if (num4 >= 0f && num4 <= 1f && num5 >= 0f && num5 <= 1f)
		{
			intersection = new Vector2(lineA.Start.X + num4 * (lineA.End.X - lineA.Start.X), lineA.Start.Y + num4 * (lineA.End.Y - lineA.Start.Y));
			return true;
		}
		return false;
	}

	public static Vector3 RotateAroundPoint(Vector3 point, Vector3 originPoint, Vector3 rotationAxis, float radiansToRotate)
	{
		Vector3 position = point - originPoint;
		Vector3 vector = Vector3.Transform(position, Matrix.CreateFromAxisAngle(rotationAxis, radiansToRotate));
		return vector + originPoint;
	}

	public static Vector2 RotateAroundPoint(Vector2 point, Vector2 origin, float rotation)
	{
		Vector2 vector = point - origin;
		if (vector == Vector2.Zero)
		{
			return point;
		}
		float num = (float)Math.Atan2(vector.Y, vector.X);
		num += rotation;
		vector = vector.Length() * new Vector2((float)Math.Cos(num), (float)Math.Sin(num));
		return vector + origin;
	}
}
