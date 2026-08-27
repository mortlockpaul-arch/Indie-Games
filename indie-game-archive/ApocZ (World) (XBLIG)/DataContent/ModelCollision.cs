using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.ModelCollision, DataContent")]
public struct ModelCollision
{
	public List<Vector3> PostionList;

	public List<MeshPartCollision> ModelPartList;
}
