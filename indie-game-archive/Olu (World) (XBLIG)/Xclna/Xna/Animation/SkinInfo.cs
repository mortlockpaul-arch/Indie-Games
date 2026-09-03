using Microsoft.Xna.Framework;

namespace Xclna.Xna.Animation;

public struct SkinInfo
{
	public readonly string BoneName;

	public readonly Matrix InverseBindPoseTransform;

	public readonly int PaletteIndex;

	public readonly int BoneIndex;

	public SkinInfo(string name, Matrix inverseBindPoseTransform, int paletteIndex, int boneIndex)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		BoneName = name;
		InverseBindPoseTransform = inverseBindPoseTransform;
		PaletteIndex = paletteIndex;
		BoneIndex = boneIndex;
	}
}
