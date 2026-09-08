#define DEBUG
using System.Diagnostics;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarRenderPassAlpha : AvatarRenderPass
{
	public override int TextureLayer
	{
		set
		{
			Debug.Assert(condition: false);
		}
	}

	public override AvatarRenderPass Clone()
	{
		AvatarRenderPassAlpha avatarRenderPassAlpha = new AvatarRenderPassAlpha();
		avatarRenderPassAlpha.effect = effect.Clone();
		CopyTo(avatarRenderPassAlpha);
		return avatarRenderPassAlpha;
	}
}
