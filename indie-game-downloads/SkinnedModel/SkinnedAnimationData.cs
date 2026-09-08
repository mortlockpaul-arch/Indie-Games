using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SkinnedModel;

public class SkinnedAnimationData
{
	public const float TimeStepDivisor = 2f;

	[ContentSerializer]
	public SkinningData skinningData;

	[ContentSerializer]
	public Texture2D animationTexture;
}
