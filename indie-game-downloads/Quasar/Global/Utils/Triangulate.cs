using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace Quasar.Global.Utils;

public class Triangulate
{
	private const float EPSILON = 1E-07f;

	public static List<Vector2> Process(ReadOnlyCollection<Vector2> contour)
	{
		bool success;
		return Process(contour, out success);
	}

	public static List<Vector2> Process(ReadOnlyCollection<Vector2> contour, out bool success)
	{
		List<Vector2> list = new List<Vector2>();
		success = false;
		int count = contour.Count;
		if (count < 3)
		{
			return list;
		}
		int[] V = new int[count];
		if (0f < Area(contour))
		{
			for (int i = 0; i < count; i++)
			{
				V[i] = i;
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				V[j] = count - 1 - j;
			}
		}
		int num = count;
		int num2 = 2 * num;
		int num3 = 0;
		int num4 = num - 1;
		while (num > 2)
		{
			if (0 >= num2--)
			{
				return list;
			}
			int num5 = num4;
			if (num <= num5)
			{
				num5 = 0;
			}
			num4 = num5 + 1;
			if (num <= num4)
			{
				num4 = 0;
			}
			int num6 = num4 + 1;
			if (num <= num6)
			{
				num6 = 0;
			}
			if (Snip(contour, num5, num4, num6, num, ref V))
			{
				int index = V[num5];
				int index2 = V[num4];
				int index3 = V[num6];
				if (Area(contour[index], contour[index2], contour[index3]) > 0f)
				{
					list.Add(contour[index]);
					list.Add(contour[index2]);
					list.Add(contour[index3]);
				}
				num3++;
				int num7 = num4;
				for (int k = num4 + 1; k < num; k++)
				{
					V[num7] = V[k];
					num7++;
				}
				num--;
				num2 = 2 * num;
			}
		}
		success = true;
		return list;
	}

	public static float Area(Vector2 a, Vector2 b, Vector2 c)
	{
		List<Vector2> list = new List<Vector2>(3);
		list.Add(a);
		list.Add(b);
		list.Add(c);
		return Area(list.AsReadOnly());
	}

	public static float Area(ReadOnlyCollection<Vector2> contour)
	{
		int count = contour.Count;
		float num = 0f;
		int index = count - 1;
		int num2 = 0;
		while (num2 < count)
		{
			num += contour[index].X * contour[num2].Y - contour[num2].X * contour[index].Y;
			index = num2++;
		}
		return num * 0.5f;
	}

	public static bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
	{
		float num = C.X - B.X;
		float num2 = C.Y - B.Y;
		float num3 = A.X - C.X;
		float num4 = A.Y - C.Y;
		float num5 = B.X - A.X;
		float num6 = B.Y - A.Y;
		float num7 = P.X - A.X;
		float num8 = P.Y - A.Y;
		float num9 = P.X - B.X;
		float num10 = P.Y - B.Y;
		float num11 = P.X - C.X;
		float num12 = P.Y - C.Y;
		float num13 = num * num10 - num2 * num9;
		float num14 = num5 * num8 - num6 * num7;
		float num15 = num3 * num12 - num4 * num11;
		if (num13 >= 0f && num15 >= 0f)
		{
			return num14 >= 0f;
		}
		return false;
	}

	private static bool Snip(ReadOnlyCollection<Vector2> contour, int u, int v, int w, int n, ref int[] V)
	{
		Vector2 a = new Vector2(contour[V[u]].X, contour[V[u]].Y);
		Vector2 b = new Vector2(contour[V[v]].X, contour[V[v]].Y);
		Vector2 c = new Vector2(contour[V[w]].X, contour[V[w]].Y);
		if (1E-07f > (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X))
		{
			return false;
		}
		for (int i = 0; i < n; i++)
		{
			if (i != u && i != v && i != w && InsideTriangle(P: new Vector2(contour[V[i]].X, contour[V[i]].Y), A: a, B: b, C: c))
			{
				return false;
			}
		}
		return true;
	}
}
