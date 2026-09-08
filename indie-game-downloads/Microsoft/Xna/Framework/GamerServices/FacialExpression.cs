namespace Microsoft.Xna.Framework.GamerServices;

internal class FacialExpression
{
	public AvatarRenderPass avatarEyebrowLeft;

	public AvatarRenderPass avatarEyebrowRight;

	public AvatarRenderPass avatarEyeLeft;

	public AvatarRenderPass avatarEyeRight;

	public AvatarRenderPass avatarMouth;

	public void Set(AvatarExpression avatarExpression)
	{
		if (avatarMouth != null)
		{
			avatarMouth.TextureLayer = (int)avatarExpression.Mouth;
		}
		if (avatarEyeLeft != null)
		{
			avatarEyeLeft.TextureLayer = (int)AvatarAnimation.SwapEyeLeftRight(avatarExpression.LeftEye);
		}
		if (avatarEyeRight != null)
		{
			avatarEyeRight.TextureLayer = (int)avatarExpression.RightEye;
		}
		if (avatarEyebrowLeft != null)
		{
			avatarEyebrowLeft.TextureLayer = (int)avatarExpression.LeftEyebrow;
		}
		if (avatarEyebrowRight != null)
		{
			avatarEyebrowRight.TextureLayer = (int)avatarExpression.RightEyebrow;
		}
	}
}
