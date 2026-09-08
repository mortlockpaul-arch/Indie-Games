namespace Microsoft.XboxLive.Avatars.Internal.Assets;

public class Skeleton
{
	public enum SkeletonVersion
	{
		Invalid,
		Nxe,
		Natal
	}

	public const int AvatarMaxJoints = 72;

	public const int CarryableMaxJoints = 72;

	public Joint[] Joints;
}
