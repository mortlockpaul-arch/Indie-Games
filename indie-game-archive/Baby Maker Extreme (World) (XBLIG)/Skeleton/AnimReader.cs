using ImportData;
using Microsoft.Xna.Framework.Content;

namespace Skeleton;

public class AnimReader : ContentTypeReader<AnimImportData>
{
	protected override AnimImportData Read(ContentReader input, AnimImportData existingInstance)
	{
		return new AnimImportData(input);
	}
}
