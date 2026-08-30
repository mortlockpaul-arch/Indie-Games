using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Games.Painter;

internal class FloodFill
{
	private struct FloodFillRange
	{
		public int StartX;

		public int EndX;

		public int Y;
	}

	private const int TOLERANCE = 50;

	private static Stack<FloodFillRange> Ranges { get; set; }

	static FloodFill()
	{
		Ranges = new Stack<FloodFillRange>();
	}

	public static void Fill(Color[] data, Point location, Color floodColor, Color targetColor, int width, int height)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		Ranges.Clear();
		LinearFill(data, ref location.X, ref location.Y, ref floodColor, ref targetColor, ref width, ref height);
		while (Ranges.Count > 0)
		{
			FloodFillRange floodFillRange = Ranges.Pop();
			int y = floodFillRange.Y - 1;
			int y2 = floodFillRange.Y + 1;
			for (int i = floodFillRange.StartX; i <= floodFillRange.EndX; i++)
			{
				if (y >= 0)
				{
					Color value = data[i + y * width];
					if (value != floodColor && IsMatchingColor(ref value, ref targetColor, 50))
					{
						LinearFill(data, ref i, ref y, ref floodColor, ref targetColor, ref width, ref height);
					}
				}
				if (y2 <= height - 1)
				{
					Color value2 = data[i + y2 * width];
					if (value2 != floodColor && IsMatchingColor(ref value2, ref targetColor, 50))
					{
						LinearFill(data, ref i, ref y2, ref floodColor, ref targetColor, ref width, ref height);
					}
				}
			}
		}
	}

	private static void LinearFill(Color[] data, ref int x, ref int y, ref Color floodColor, ref Color targetColor, ref int width, ref int height)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		int num = x;
		int num2 = x;
		while (true)
		{
			Color value = data[num + y * width];
			if (value == floodColor || !IsMatchingColor(ref value, ref targetColor, 50))
			{
				break;
			}
			ref Color reference = ref data[num + y * width];
			reference = floodColor;
			if (--num < 0)
			{
				num = 0;
				break;
			}
		}
		while (true)
		{
			if (++num2 > width - 1)
			{
				num2 = width - 1;
				break;
			}
			Color value2 = data[num2 + y * width];
			if (value2 == floodColor || !IsMatchingColor(ref value2, ref targetColor, 50))
			{
				break;
			}
			ref Color reference2 = ref data[num2 + y * width];
			reference2 = floodColor;
		}
		Ranges.Push(new FloodFillRange
		{
			StartX = num,
			EndX = num2,
			Y = y
		});
	}

	private static bool IsMatchingColor(ref Color value1, ref Color value2, int tolerance)
	{
		if (Math.Abs(((Color)(ref value1)).R - ((Color)(ref value2)).R) <= tolerance && Math.Abs(((Color)(ref value1)).G - ((Color)(ref value2)).G) <= tolerance)
		{
			return Math.Abs(((Color)(ref value1)).B - ((Color)(ref value2)).B) <= tolerance;
		}
		return false;
	}
}
