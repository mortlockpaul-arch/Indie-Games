using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using s;
using y;

namespace z;

internal static class r
{
	public static void GetShapeMeshData(s.b collidable, List<VertexPositionNormalTexture> vertices, List<uint> indices)
	{
		if (!(collidable.Shape is global::y.v v2))
		{
			throw new ArgumentException("Wrong shape type.");
		}
		Vector3 localNormal = v2.GetLocalNormal();
		vertices.Add(new VertexPositionNormalTexture(v2.VertexA, -localNormal, new Vector2(0f, 0f)));
		vertices.Add(new VertexPositionNormalTexture(v2.VertexB, -localNormal, new Vector2(0f, 1f)));
		vertices.Add(new VertexPositionNormalTexture(v2.VertexC, -localNormal, new Vector2(1f, 0f)));
		vertices.Add(new VertexPositionNormalTexture(v2.VertexA, localNormal, new Vector2(0f, 0f)));
		vertices.Add(new VertexPositionNormalTexture(v2.VertexB, localNormal, new Vector2(0f, 1f)));
		vertices.Add(new VertexPositionNormalTexture(v2.VertexC, localNormal, new Vector2(1f, 0f)));
		indices.Add(0u);
		indices.Add(1u);
		indices.Add(2u);
		indices.Add(3u);
		indices.Add(5u);
		indices.Add(4u);
	}
}
