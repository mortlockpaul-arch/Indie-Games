using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P;

namespace z
{
	internal class v : z._0006<P._7>
	{
		public v(b drawer, P._7 displayedObject)
			: base(drawer, displayedObject)
		{
		}

		public override int GetTriangleCountEstimate()
		{
			return base.DisplayedObject.Mesh.Data.Indices.Length / 3;
		}

		public override void GetMeshData(List<VertexPositionNormalTexture> vertices, List<uint> indices)
		{
			X.GetMeshData(base.DisplayedObject.Mesh, vertices, indices);
		}

		public override void Update()
		{
			base.WorldTransform = Matrix.Identity;
		}
	}
}
namespace Z
{
	internal enum v
	{
		Unclaimed,
		OwnedByFirst,
		OwnedBySecond
	}
}
