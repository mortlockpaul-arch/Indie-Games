namespace Microsoft.XboxLive.MathUtilities;

public struct BoundingBox2
{
	public Vector2 minCoord;

	public Vector2 maxCoord;

	public void Clear()
	{
		minCoord.X = float.MaxValue;
		minCoord.Y = float.MaxValue;
		maxCoord.X = float.MinValue;
		maxCoord.Y = float.MinValue;
	}

	public void Extend(BoundingBox2 boundingBox2)
	{
		if (boundingBox2.minCoord.X < minCoord.X)
		{
			minCoord.X = boundingBox2.minCoord.X;
		}
		if (boundingBox2.minCoord.Y < minCoord.Y)
		{
			minCoord.Y = boundingBox2.minCoord.Y;
		}
		if (boundingBox2.maxCoord.X > maxCoord.X)
		{
			maxCoord.X = boundingBox2.maxCoord.X;
		}
		if (boundingBox2.maxCoord.Y > maxCoord.Y)
		{
			maxCoord.Y = boundingBox2.maxCoord.Y;
		}
	}

	public void Extend(Vector2 point)
	{
		if (point.X < minCoord.X)
		{
			minCoord.X = point.X;
		}
		if (point.Y < minCoord.Y)
		{
			minCoord.Y = point.Y;
		}
		if (point.X > maxCoord.X)
		{
			maxCoord.X = point.X;
		}
		if (point.Y > maxCoord.Y)
		{
			maxCoord.Y = point.Y;
		}
	}

	public void Extend(float value)
	{
		minCoord.X -= value;
		minCoord.Y -= value;
		maxCoord.X += value;
		maxCoord.Y += value;
	}

	public bool IntersectInner(BoundingBox2 boundingBox2)
	{
		if (minCoord.X >= boundingBox2.maxCoord.X)
		{
			return false;
		}
		if (minCoord.Y >= boundingBox2.maxCoord.Y)
		{
			return false;
		}
		if (maxCoord.X <= boundingBox2.minCoord.X)
		{
			return false;
		}
		if (maxCoord.Y <= boundingBox2.minCoord.Y)
		{
			return false;
		}
		return true;
	}

	public bool Contains(Vector2 point)
	{
		if (point.X < minCoord.X)
		{
			return false;
		}
		if (point.Y < minCoord.Y)
		{
			return false;
		}
		if (point.X > maxCoord.X)
		{
			return false;
		}
		if (point.Y > maxCoord.Y)
		{
			return false;
		}
		return true;
	}
}
