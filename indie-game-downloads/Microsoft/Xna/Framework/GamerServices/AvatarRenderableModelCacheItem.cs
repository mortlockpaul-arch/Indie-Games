using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarRenderableModelCacheItem
{
	public AvatarRenderableModel RenderableModel { get; set; }

	public ShaderId ShaderId { get; set; }

	public AvatarRenderableModelCacheItem(AvatarRenderableModel arm, ShaderId shaderId)
	{
		RenderableModel = arm;
		ShaderId = shaderId;
	}
}
