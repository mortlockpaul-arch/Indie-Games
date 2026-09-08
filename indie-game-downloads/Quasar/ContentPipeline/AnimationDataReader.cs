using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;

namespace Quasar.ContentPipeline;

public class AnimationDataReader : ContentTypeReader<AnimationData>
{
	protected override AnimationData Read(ContentReader input, AnimationData existingInstance)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		AnimationData animationData = new AnimationData();
		animationData.Length = new TimeSpan(input.ReadInt64());
		animationData.Title = input.ReadString();
		for (int i = 0; i < 31; i++)
		{
			AvatarAnimationPreset val = (AvatarAnimationPreset)i;
			if (animationData.Title == ((object)val).ToString())
			{
				animationData.correspondingPreset = val;
			}
		}
		int num = input.ReadInt32();
		animationData.animationData.Capacity = num;
		for (int j = 0; j < num; j++)
		{
			AnimationFrameData animationFrameData = new AnimationFrameData();
			animationFrameData.FramePosition = new TimeSpan(input.ReadInt64());
			int num2 = input.ReadInt32();
			animationFrameData.BoneTransforms.Capacity = num2;
			for (int k = 0; k < num2; k++)
			{
				animationFrameData.BoneTransforms.Add(input.ReadMatrix());
			}
			animationData.animationData.Add(animationFrameData);
		}
		return animationData;
	}
}
