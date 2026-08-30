using Microsoft.Xna.Framework;

namespace Maximinus;

public struct RotatedRectangle
{
	public enum RotationOrigin
	{
		Center,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	public Line Top;

	public Line Right;

	public Line Left;

	public Line Bottom;

	public readonly Vector2 leftTop;

	public readonly Vector2 rightTop;

	public readonly Vector2 leftBottom;

	public readonly Vector2 rightBottom;

	private static Vector2 Origin(Rectangle rectangle, RotationOrigin rotationOrigin)
	{
		Vector2 result = new Vector2(rectangle.Left, rectangle.Top);
		switch (rotationOrigin)
		{
		case RotationOrigin.TopRight:
			result = new Vector2(rectangle.Right, rectangle.Y);
			break;
		case RotationOrigin.BottomLeft:
			result = new Vector2(rectangle.Left, rectangle.Bottom);
			break;
		case RotationOrigin.BottomRight:
			result = new Vector2(rectangle.Right, rectangle.Bottom);
			break;
		case RotationOrigin.Center:
			result = new Vector2(rectangle.Center.X, rectangle.Center.Y);
			break;
		}
		return result;
	}

	public RotatedRectangle(Rectangle rectangle, float rotation, RotationOrigin rotationOrigin)
		: this(rectangle, rotation, Origin(rectangle, rotationOrigin))
	{
	}

	public RotatedRectangle(Rectangle rectangle, float rotation, Vector2 origin)
	{
		leftTop = Collision2D.RotateAroundPoint(new Vector2(rectangle.Left, rectangle.Top), origin, rotation);
		rightTop = Collision2D.RotateAroundPoint(new Vector2(rectangle.Right, rectangle.Top), origin, rotation);
		leftBottom = Collision2D.RotateAroundPoint(new Vector2(rectangle.Left, rectangle.Bottom), origin, rotation);
		rightBottom = Collision2D.RotateAroundPoint(new Vector2(rectangle.Right, rectangle.Bottom), origin, rotation);
		Top = new Line(leftTop, rightTop);
		Bottom = new Line(leftBottom, rightBottom);
		Left = new Line(leftTop, leftBottom);
		Right = new Line(rightTop, rightBottom);
	}

	public bool IntersectsWith(RotatedRectangle target)
	{
		Vector2? intersection;
		return IntersectsWith(target, out intersection);
	}

	public bool IntersectsWith(RotatedRectangle target, out Vector2? intersection)
	{
		if (Collision2D.LineIntersectLine(Top, target.Top, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Top, target.Left, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Top, target.Right, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Top, target.Bottom, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Left, target.Top, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Left, target.Left, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Left, target.Right, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Left, target.Bottom, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Right, target.Top, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Right, target.Left, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Right, target.Right, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Right, target.Bottom, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Bottom, target.Top, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Bottom, target.Left, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Bottom, target.Right, out intersection))
		{
			return true;
		}
		if (Collision2D.LineIntersectLine(Bottom, target.Bottom, out intersection))
		{
			return true;
		}
		return false;
	}
}
