using Microsoft.Xna.Framework.Content;

namespace PropModel;

public class MeshUserDataReader : ContentTypeReader<MeshUserData>
{
	protected override MeshUserData Read(ContentReader input, MeshUserData existingInstance)
	{
		MeshUserData meshUserData = new MeshUserData();
		int num = input.ReadInt32();
		meshUserData.instanceData = new MeshInstanceData[num];
		for (int i = 0; i < num; i++)
		{
			meshUserData.instanceData[i] = input.ReadObject<MeshInstanceData>();
		}
		meshUserData.collisionData = input.ReadObject<CollisionData>();
		return meshUserData;
	}
}
