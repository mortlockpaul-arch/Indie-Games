using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.eMyBoundingSphere, DataContent")]
public struct eMyBoundingSphere
{
	public float Radius;

	public Vector3 Center;
}
