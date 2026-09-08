using System.IO;
using Microsoft.Xna.Framework.Content;

namespace PropModel;

public class MeshInstanceDataReader : ContentTypeReader<MeshInstanceData>
{
	protected override MeshInstanceData Read(ContentReader input, MeshInstanceData existingInstance)
	{
		MeshInstanceData meshInstanceData = new MeshInstanceData();
		meshInstanceData.ReferenceId = ((BinaryReader)input).ReadInt32();
		meshInstanceData.Name = ((BinaryReader)input).ReadString();
		meshInstanceData.matWorld = input.ReadMatrix();
		return meshInstanceData;
	}
}
