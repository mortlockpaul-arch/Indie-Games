using ImportData;
using Microsoft.Xna.Framework.Content;

namespace Skeleton;

public class SkeletonReader : ContentTypeReader<SkeletonImportData>
{
	protected override SkeletonImportData Read(ContentReader input, SkeletonImportData existingInstance)
	{
		return new SkeletonImportData(input);
	}
}
