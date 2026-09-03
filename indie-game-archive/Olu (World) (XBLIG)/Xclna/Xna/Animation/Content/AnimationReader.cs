using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Xclna.Xna.Animation.Content;

internal sealed class AnimationReader : ContentTypeReader<AnimationInfoCollection>
{
	protected override AnimationInfoCollection Read(ContentReader input, AnimationInfoCollection existingInstance)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		AnimationInfoCollection animationInfoCollection = new AnimationInfoCollection();
		int num = ((BinaryReader)(object)input).ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string text = ((BinaryReader)(object)input).ReadString();
			int num2 = ((BinaryReader)(object)input).ReadInt32();
			List<BoneKeyframeCollection> list = new List<BoneKeyframeCollection>();
			for (int j = 0; j < num2; j++)
			{
				string boneName = ((BinaryReader)(object)input).ReadString();
				int num3 = ((BinaryReader)(object)input).ReadInt32();
				List<BoneKeyframe> list2 = new List<BoneKeyframe>();
				for (int k = 0; k < num3; k++)
				{
					BoneKeyframe item = new BoneKeyframe(input.ReadMatrix(), ((BinaryReader)(object)input).ReadInt64());
					list2.Add(item);
				}
				BoneKeyframeCollection item2 = new BoneKeyframeCollection(boneName, list2);
				list.Add(item2);
			}
			AnimationInfo value = new AnimationInfo(text, new AnimationChannelCollection(list));
			animationInfoCollection.Add(text, value);
		}
		return animationInfoCollection;
	}
}
