using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using N;
using s;

namespace z;

internal static class W
{
	public static void GetShapeMeshData(s.b collidable, List<VertexPositionNormalTexture> vertices, List<uint> indices)
	{
		if (!(collidable is s._6 obj))
		{
			throw new ArgumentException("Wrong shape type.");
		}
		List<uint> list = new List<uint>();
		List<VertexPositionNormalTexture> list2 = new List<VertexPositionNormalTexture>();
		for (int i = 0; i < obj.Children.Count; i++)
		{
			s._7 obj2 = obj.Children[i];
			if (b.ShapeMeshGetters.TryGetValue(obj2.CollisionInformation.GetType(), out var value))
			{
				value(obj2.CollisionInformation, list2, list);
				for (int j = 0; j < list.Count; j++)
				{
					indices.Add((uint)(list[j] + vertices.Count));
				}
				N._0006 transform = obj2.Entry.LocalTransform;
				Vector3 value2 = obj2.CollisionInformation.LocalPosition;
				for (int k = 0; k < list2.Count; k++)
				{
					VertexPositionNormalTexture item = list2[k];
					Vector3.Add(ref item.Position, ref value2, out item.Position);
					N._0006.Transform(ref item.Position, ref transform, out item.Position);
					Vector3.Transform(ref item.Normal, ref transform.Orientation, out item.Normal);
					vertices.Add(item);
				}
				list2.Clear();
				list.Clear();
			}
		}
	}
}
