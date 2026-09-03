using Microsoft.Xna.Framework;

namespace Xclna.Xna.Animation;

public class InterpolationController : AnimationController
{
	private static Matrix curTransform;

	private static Matrix nextTransform;

	private static Matrix transform;

	private InterpolationMethod interpMethod;

	public InterpolationMethod InterpolationMethod
	{
		get
		{
			return interpMethod;
		}
		set
		{
			interpMethod = value;
		}
	}

	public InterpolationController(Game game, AnimationInfo source, InterpolationMethod interpMethod)
		: base(game, source)
	{
		this.interpMethod = interpMethod;
	}

	public override Matrix GetCurrentBoneTransform(BonePose pose)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		BoneKeyframeCollection boneKeyframeCollection = base.AnimationSource.AnimationChannels[pose.Name];
		int indexByTime = boneKeyframeCollection.GetIndexByTime(base.ElapsedTime);
		if (interpMethod == InterpolationMethod.None)
		{
			return boneKeyframeCollection[indexByTime].Transform;
		}
		int num = indexByTime + 1;
		if (num >= boneKeyframeCollection.Count)
		{
			return boneKeyframeCollection[indexByTime].Transform;
		}
		double num2 = base.ElapsedTime - boneKeyframeCollection[indexByTime].Time;
		double num3 = boneKeyframeCollection[num].Time - boneKeyframeCollection[indexByTime].Time;
		double num4 = num2 / num3;
		curTransform = boneKeyframeCollection[indexByTime].Transform;
		nextTransform = boneKeyframeCollection[num].Transform;
		if (interpMethod == InterpolationMethod.Linear)
		{
			Matrix.Lerp(ref curTransform, ref nextTransform, (float)num4, ref transform);
		}
		else
		{
			Util.SlerpMatrix(ref curTransform, ref nextTransform, (float)num4, out transform);
		}
		return transform;
	}
}
