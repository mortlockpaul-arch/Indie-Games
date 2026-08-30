using Microsoft.Xna.Framework;

namespace Maximinus;

public class RectangleFull
{
	public RoundLine list;

	public int lineRadius;

	public RectangleFull(Vector2 size, Vector2 pos)
	{
		list = new RoundLine(new Vector2(pos.X - size.X / 2f, pos.Y), new Vector2(pos.X + size.X / 2f, pos.Y));
		lineRadius = (int)(size.Y * 0.5f);
	}
}
