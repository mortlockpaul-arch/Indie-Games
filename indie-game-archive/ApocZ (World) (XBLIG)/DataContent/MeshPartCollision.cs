using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.MeshPartCollision, DataContent")]
public struct MeshPartCollision
{
	public string MeshName;

	public List<TriangleData> triangleData;
}
