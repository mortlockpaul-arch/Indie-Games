using Microsoft.Xna.Framework;

namespace ProjectMercury;

public struct BoundingRect
{
	public Vector2 Min;

	public Vector2 Max;

	public float Top => Min.Y;

	public float Left => Min.X;

	public float Right => Max.X;

	public float Bottom => Max.Y;

	public float Width => Max.X - Min.X;

	public float Height => Max.Y - Min.Y;

	public Vector2 Centre => new Vector2
	{
		X = Width / 2f,
		Y = Height / 2f
	};

	public BoundingBox ToBoundingBox(float z, float depth)
	{
		return new BoundingBox
		{
			Min = new Vector3(Min, z),
			Max = new Vector3(Max, z + depth)
		};
	}
}
