using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.CollisionStruct, DataContent")]
public struct CollisionStruct
{
	public bool haveCollision;

	public float depth;

	public bool applyResponse;

	public bool onWalkable;

	public uint flags;

	public Vector3 hitPosition;

	public Vector3 hitNormal;

	public TriggerTypes hitTrigger;
}
