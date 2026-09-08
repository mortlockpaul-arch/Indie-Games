#define DEBUG
using System.Diagnostics;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarRenderPassDual : AvatarRenderPass
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
		AvatarRenderPassDual avatarRenderPassDual = new AvatarRenderPassDual();
		avatarRenderPassDual.effect = effect.Clone();
		CopyTo(avatarRenderPassDual);
		return avatarRenderPassDual;
	}
}
