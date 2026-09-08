using System.IO;
using Microsoft.Xna.Framework.Content;

namespace PropModel;

public class SpawnPositionDataReader : ContentTypeReader<SpawnPositionData>
{
	protected override SpawnPositionData Read(ContentReader input, SpawnPositionData existingInstance)
	{
		SpawnPositionData spawnPositionData = new SpawnPositionData();
		spawnPositionData.itemRange = ((BinaryReader)input).ReadByte();
		spawnPositionData.spawnType = ((BinaryReader)input).ReadUInt16();
		spawnPositionData.spawmPosition = input.ReadVector3();
		return spawnPositionData;
	}
}
