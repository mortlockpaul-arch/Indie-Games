using Microsoft.Xna.Framework.Content;

namespace PropModel;

public class SpawnPositionDataReader : ContentTypeReader<SpawnPositionData>
{
	protected override SpawnPositionData Read(ContentReader input, SpawnPositionData existingInstance)
	{
		SpawnPositionData spawnPositionData = new SpawnPositionData();
		spawnPositionData.itemRange = input.ReadByte();
		spawnPositionData.spawnType = input.ReadUInt16();
		spawnPositionData.spawmPosition = input.ReadVector3();
		return spawnPositionData;
	}
}
