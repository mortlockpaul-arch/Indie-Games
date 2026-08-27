using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PropModel;

public class CollisionDataReader : ContentTypeReader<CollisionData>
{
	protected override CollisionData Read(ContentReader input, CollisionData existingInstance)
	{
		CollisionData collisionData = new CollisionData();
		collisionData.bSphere = input.ReadObject<BoundingSphere>();
		collisionData.bBox = input.ReadObject<BoundingBox>();
		collisionData.transform = input.ReadMatrix();
		int num = input.ReadInt32();
		collisionData.indices = new short[num];
		for (int i = 0; i < num; i++)
		{
			collisionData.indices[i] = input.ReadInt16();
		}
		int num2 = input.ReadInt32();
		collisionData.vertices = new Vector3[num2];
		for (int j = 0; j < num2; j++)
		{
			ref Vector3 reference = ref collisionData.vertices[j];
			reference = input.ReadVector3();
		}
		return collisionData;
	}
}
