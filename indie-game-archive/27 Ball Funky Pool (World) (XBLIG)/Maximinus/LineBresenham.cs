using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class LineBresenham
{
	private Point p = Point.Zero;

	public static List<Point> Compute(Point P0, Point P1)
	{
		List<Point> list = new List<Point>();
		int i = P0.X;
		int i2 = P1.X;
		int i3 = P0.Y;
		int i4 = P1.Y;
		bool flag = Math.Abs(i4 - i3) > Math.Abs(i2 - i);
		if (flag)
		{
			Utils.Swap(ref i, ref i3);
			Utils.Swap(ref i2, ref i4);
		}
		if (i > i2)
		{
			Utils.Swap(ref i, ref i2);
			Utils.Swap(ref i3, ref i4);
		}
		int num = i2 - i;
		int num2 = Math.Abs(i4 - i3);
		int num3 = num / 2;
		int num4 = i3;
		int num5 = ((i3 < i4) ? 1 : (-1));
		bool flag2 = i2 > i;
		for (int j = i; flag2 ? (j <= i2) : (j >= i2); j += (flag2 ? 1 : (-1)))
		{
			if (flag)
			{
				list.Add(new Point(num4, j));
			}
			else
			{
				list.Add(new Point(j, num4));
			}
			num3 -= num2;
			if (num3 < 0)
			{
				num4 += num5;
				num3 += num;
			}
		}
		return list;
	}
}
