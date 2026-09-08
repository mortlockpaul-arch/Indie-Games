using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Quasar.Meshes.SkinnedBodies;

public class SkinningDataReader : ContentTypeReader<SkinningData>
{
	protected override SkinningData Read(ContentReader input, SkinningData existingInstance)
	{
		Dictionary<string, AnimationClip> animationClips = input.ReadObject<Dictionary<string, AnimationClip>>();
		List<Matrix> bindPose = input.ReadObject<List<Matrix>>();
		List<Matrix> inverseBindPose = input.ReadObject<List<Matrix>>();
		List<int> skeletonHierarchy = input.ReadObject<List<int>>();
		return new SkinningData(animationClips, bindPose, inverseBindPose, skeletonHierarchy);
	}
}
