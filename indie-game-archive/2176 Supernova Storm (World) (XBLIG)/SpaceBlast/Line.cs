using Microsoft.Xna.Framework;

namespace SpaceBlast;

internal class Line
{
	public Vector3 Point1;

	public Vector3 Point2;

	private Ray m_Ray;

	private float m_RayLength;

	private int m_LineWidth;

	public Line(ref Vector3 start, ref Vector3 end, int lineWidth)
	{
		Point1 = start;
		Point2 = end;
		m_LineWidth = lineWidth;
		Vector3 direction = end - start;
		m_RayLength = direction.Length();
		direction.Normalize();
		m_Ray = new Ray(start, direction);
	}

	public bool Intersects(BoundingSphere sphere)
	{
		sphere.Radius += m_LineWidth;
		float? num = m_Ray.Intersects(sphere);
		if (!num.HasValue)
		{
			return false;
		}
		if (num.Value <= m_RayLength)
		{
			return true;
		}
		return false;
	}
}
