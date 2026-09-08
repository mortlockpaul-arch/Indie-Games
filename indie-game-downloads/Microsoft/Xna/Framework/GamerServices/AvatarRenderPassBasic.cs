using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarRenderPassBasic : AvatarRenderPass
{
	public override int TextureLayer
	{
		set
		{
			((BasicEffect)effect).Texture = textures[value];
		}
	}

	public override AvatarRenderPass Clone()
	{
		AvatarRenderPassBasic avatarRenderPassBasic = new AvatarRenderPassBasic();
		avatarRenderPassBasic.effect = effect.Clone();
		CopyTo(avatarRenderPassBasic);
		return avatarRenderPassBasic;
	}
}
