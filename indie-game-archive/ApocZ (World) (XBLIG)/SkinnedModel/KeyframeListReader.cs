using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SkinnedModel;

public class KeyframeListReader : ContentTypeReader<List<Keyframe[]>>
{
	protected override List<Keyframe[]> Read(ContentReader input, List<Keyframe[]> existingInstance)
	{
		int num = input.ReadInt32();
		List<Keyframe[]> list = new List<Keyframe[]>(num);
		int num2 = input.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Keyframe[] array = new Keyframe[num2];
			for (int j = 0; j < num2; j++)
			{
				int bone = input.ReadInt32();
				TimeSpan time = TimeSpan.FromTicks(input.ReadInt64());
				Matrix transform = input.ReadMatrix();
				ref Keyframe reference = ref array[j];
				reference = new Keyframe(bone, time, transform);
			}
			list.Add(array);
		}
		return list;
	}
}
