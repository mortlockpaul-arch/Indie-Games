using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Quasar.Meshes.SkinnedBodies;

public class SkinningData
{
	[ContentSerializer]
	public Dictionary<string, AnimationClip> AnimationClips { get; private set; }

	[ContentSerializer]
	public List<Matrix> BindPose { get; private set; }

	[ContentSerializer]
	public List<Matrix> InverseBindPose { get; private set; }

	[ContentSerializer]
	public List<int> SkeletonHierarchy { get; private set; }

	public SkinningData(Dictionary<string, AnimationClip> animationClips, List<Matrix> bindPose, List<Matrix> inverseBindPose, List<int> skeletonHierarchy)
	{
		AnimationClips = animationClips;
		BindPose = bindPose;
		InverseBindPose = inverseBindPose;
		SkeletonHierarchy = skeletonHierarchy;
	}

	private SkinningData()
	{
	}
}
