using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Assets;

public struct Joint
{
	public const int InvalidJointIndex = -1;

	public int Parent;

	public int Child;

	public int Sibling;

	public Vector3 BindPosition;

	public Quaternion BindRotation;

	public Pose Local;
}
