using ImportData;
using Microsoft.Xna.Framework.Content;

public class SpawnerReader : ContentTypeReader<SpawnerImportData>
{
	protected override SpawnerImportData Read(ContentReader input, SpawnerImportData existingInstance)
	{
		return new SpawnerImportData(input);
	}
}
