using Microsoft.Xna.Framework;

namespace Maximinus;

public struct Line(Vector2 start, Vector2 end)
{
	public Vector2 Start = start;

	public Vector2 End = end;
}
