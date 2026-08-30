using System;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public struct DistanceGridData
{
	public AABB AABB;

	public float GridCellSize;

	public float GridCellSizeInv;

	public float[,] Nodes;

	public bool Intersect(ref Vector2 vector, out Feature feature)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		if (AABB.Contains(ref vector))
		{
			int num = (int)Math.Floor((vector.X - AABB.Min.X) * GridCellSizeInv);
			int num2 = (int)Math.Floor((vector.Y - AABB.Min.Y) * GridCellSizeInv);
			float num3 = Nodes[num, num2];
			float num4 = Nodes[num + 1, num2];
			float num5 = Nodes[num, num2 + 1];
			float num6 = Nodes[num + 1, num2 + 1];
			if (num3 <= 0f || num4 <= 0f || num5 <= 0f || num6 <= 0f)
			{
				float num7 = (vector.X - (GridCellSize * (float)num + AABB.Min.X)) * GridCellSizeInv;
				float num8 = (vector.Y - (GridCellSize * (float)num2 + AABB.Min.Y)) * GridCellSizeInv;
				float num9 = MathHelper.Lerp(num5, num6, num7);
				float num10 = MathHelper.Lerp(num3, num4, num7);
				float num11 = MathHelper.Lerp(num10, num9, num8);
				if (num11 <= 0f)
				{
					float num12 = MathHelper.Lerp(num4, num6, num8);
					float num13 = MathHelper.Lerp(num3, num5, num8);
					Vector2 normal = Vector2.Zero;
					normal.X = num12 - num13;
					normal.Y = num9 - num10;
					if (normal.X != 0f || normal.Y != 0f)
					{
						Vector2.Normalize(ref normal, ref normal);
						feature = new Feature(ref vector, ref normal, num11);
						return true;
					}
				}
			}
		}
		feature = default(Feature);
		return false;
	}
}
