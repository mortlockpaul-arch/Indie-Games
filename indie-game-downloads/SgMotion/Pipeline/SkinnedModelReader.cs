using Microsoft.Xna.Framework.Content;

namespace SgMotion.Pipeline;

internal class SkinnedModelReader : ContentTypeReader<SkinnedModel>
{
	protected override SkinnedModel Read(ContentReader input, SkinnedModel existingInstance)
	{
		return SkinnedModel.Read(input);
	}
}
