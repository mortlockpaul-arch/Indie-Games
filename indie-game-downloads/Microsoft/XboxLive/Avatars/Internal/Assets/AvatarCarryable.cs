using Microsoft.XboxLive.Avatars.Internal.Animations;

namespace Microsoft.XboxLive.Avatars.Internal.Assets;

public class AvatarCarryable
{
	public Skeleton m_Skeleton;

	public AvatarComponent m_ComponentModel = new AvatarComponent();

	public AvatarAnimation m_Animation;

	public AvatarAnimation Animation => m_Animation;

	public Skeleton Skeleton => m_Skeleton;

	public AvatarComponent Model => m_ComponentModel;

	public float[] GetCarryableMaxSkeletonScaling()
	{
		return m_Animation.GetCarryableMaxSkeletonScaling(m_Skeleton);
	}
}
