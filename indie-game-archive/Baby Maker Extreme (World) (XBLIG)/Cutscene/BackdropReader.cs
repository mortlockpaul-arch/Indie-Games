using ImportData;
using Microsoft.Xna.Framework.Content;

namespace Cutscene;

public class BackdropReader : ContentTypeReader<BackdropImportData>
{
	protected override BackdropImportData Read(ContentReader input, BackdropImportData existingInstance)
	{
		return new BackdropImportData(input);
	}
}
