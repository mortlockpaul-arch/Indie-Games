using Microsoft.Xna.Framework.Content;

namespace SgMotion.Pipeline;

internal class SkinnedModelBoneReader : ContentTypeReader<SkinnedModelBone>
{
	protected override SkinnedModelBone Read(ContentReader input, SkinnedModelBone existingInstance)
	{
		return SkinnedModelBone.Read(input);
	}
}
