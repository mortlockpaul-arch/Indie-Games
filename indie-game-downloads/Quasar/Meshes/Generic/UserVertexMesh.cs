using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Global.Vertex;

namespace Quasar.Meshes.Generic;

public abstract class UserVertexMesh<VertexStruct> : GenericMesh<VertexStruct> where VertexStruct : struct, IPositionVertex
{
	private PrimitiveType primitiveType;

	protected PrimitiveType PrimitiveType
	{
		get
		{
			return primitiveType;
		}
		set
		{
			primitiveType = value;
		}
	}

	public UserVertexMesh()
	{
	}

	protected override void RenderPrimitives()
	{
		Engine.Device.DrawUserPrimitives(PrimitiveType, verticesBuffer, 0, primitiveCount);
	}
}
