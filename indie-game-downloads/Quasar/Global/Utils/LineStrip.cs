using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.Global.Utils;

public class LineStrip
{
	private const float EPSILON = 1E-07f;

	public static List<Vector2> Process(IList<Vector2> contour, float width, bool switchOrder)
	{
		List<Vector2> result = new List<Vector2>(contour.Count * 2);
		Process(result, contour, width, switchOrder);
		return result;
	}

	public static void Process(List<Vector2> result, IList<Vector2> contour, float width, bool switchOrder)
	{
		result.Clear();
		if (contour.Count == 1)
		{
			return;
		}
		Vector2 vector = Vector2.Zero;
		for (int i = 0; i < contour.Count; i++)
		{
			if (i == contour.Count - 1)
			{
				vector = GameMath.Normal(contour[i] - contour[i - 1]);
			}
			else if (i == 0)
			{
				vector = GameMath.Normal(contour[1] - contour[0]);
			}
			else
			{
				Vector2 vector2 = contour[i] - contour[i - 1];
				Vector2 vector3 = contour[i + 1] - contour[i];
				if (vector2.Length() != 0f && vector3.Length() != 0f)
				{
					vector2.Normalize();
					vector3.Normalize();
					vector = GameMath.Normal(vector2 + vector3);
				}
			}
			if (switchOrder)
			{
				result.Add(contour[i] + vector * width / 2f);
			}
			result.Add(contour[i] - vector * width / 2f);
			if (!switchOrder)
			{
				result.Add(contour[i] + vector * width / 2f);
			}
		}
	}
}
