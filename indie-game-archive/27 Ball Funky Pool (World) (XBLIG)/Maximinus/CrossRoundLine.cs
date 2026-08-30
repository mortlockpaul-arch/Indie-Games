using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class CrossRoundLine
{
	public List<RoundLine> list;

	private Vector2 pos;

	public Vector2 Position => pos;

	public CrossRoundLine(Vector2 center, float radius)
	{
		list = new List<RoundLine>();
		pos = center;
		Vector2 p = center;
		Vector2 p2 = center;
		Vector2 p3 = center;
		Vector2 p4 = center;
		float num = radius * (float)Math.Sqrt(2.0);
		p.X -= num;
		p.Y += num;
		p2.X += num;
		p2.Y += num;
		p3.X += num;
		p3.Y -= num;
		p4.X -= num;
		p4.Y -= num;
		list.Add(new RoundLine(p, p3));
		list.Add(new RoundLine(p2, p4));
	}
}
