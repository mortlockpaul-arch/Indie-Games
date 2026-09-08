using Microsoft.Xna.Framework.Content;

namespace SgMotion.Pipeline;

internal class AnimationClipReader : ContentTypeReader<AnimationClip>
{
	protected override AnimationClip Read(ContentReader input, AnimationClip existingInstance)
	{
		return AnimationClip.Read(input);
	}
}
