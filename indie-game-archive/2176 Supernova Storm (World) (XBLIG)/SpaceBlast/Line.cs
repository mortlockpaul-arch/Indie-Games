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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Point1 = start;
		Point2 = end;
		m_LineWidth = lineWidth;
		Vector3 val = end - start;
		m_RayLength = ((Vector3)(ref val)).Length();
		((Vector3)(ref val)).Normalize();
		m_Ray = new Ray(start, val);
	}

	public bool Intersects(BoundingSphere sphere)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		sphere.Radius += (float)m_LineWidth;
		float? num = ((Ray)(ref m_Ray)).Intersects(sphere);
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
