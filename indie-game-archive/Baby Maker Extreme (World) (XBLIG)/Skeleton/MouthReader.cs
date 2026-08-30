using ImportData;
using Microsoft.Xna.Framework.Content;

namespace Skeleton;

public class MouthReader : ContentTypeReader<MouthImportData>
{
	protected override MouthImportData Read(ContentReader input, MouthImportData existingInstance)
	{
		return new MouthImportData(input);
	}
}
