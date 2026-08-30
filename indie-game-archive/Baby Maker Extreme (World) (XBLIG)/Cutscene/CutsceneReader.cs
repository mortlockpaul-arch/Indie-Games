using ImportData;
using Microsoft.Xna.Framework.Content;

namespace Cutscene;

public class CutsceneReader : ContentTypeReader<CutsceneImportData>
{
	protected override CutsceneImportData Read(ContentReader input, CutsceneImportData existingInstance)
	{
		return new CutsceneImportData(input);
	}
}
