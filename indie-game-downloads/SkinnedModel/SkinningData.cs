using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SkinnedModel;

public class SkinningData
{
	private IDictionary<string, AnimationClip> animationClipsValue;

	private IList<Matrix> bindPoseValue;

	private IList<Matrix> inverseBindPoseValue;

	private IList<int> skeletonHierarchyValue;

	private Texture2D animationTextureValue;

	private IList<int> boneIndicesValue;

	public static string[] BoneIndiceNames = new string[32]
	{
		"root", "head", "spine", "right_arm", "right_elbow", "right_hand", "right_mag", "right_bullet", "right_bolt", "left_arm",
		"left_elbow", "left_hand", "bone_pelvis", "bone_spine0", "bone_spine1", "bone_pelvis2", "bone_neck", "bone_head", "bone_right_arm", "bone_right_elbow",
		"bone_right_hand", "bone_left_arm", "bone_left_elbow", "bone_left_hand", "bone_right_thigh", "bone_right_knee", "bone_right_foot", "bone_right_toe", "bone_left_thigh", "bone_left_knee",
		"bone_left_foot", "bone_left_toe"
	};

	public IDictionary<string, AnimationClip> AnimationClips
	{
		get
		{
			return animationClipsValue;
		}
		set
		{
			animationClipsValue = value;
		}
	}

	public IList<Matrix> BindPose
	{
		get
		{
			return bindPoseValue;
		}
		set
		{
			bindPoseValue = value;
		}
	}

	public IList<Matrix> InverseBindPose
	{
		get
		{
			return inverseBindPoseValue;
		}
		set
		{
			inverseBindPoseValue = value;
		}
	}

	public IList<int> SkeletonHierarchy
	{
		get
		{
			return skeletonHierarchyValue;
		}
		set
		{
			skeletonHierarchyValue = value;
		}
	}

	public Texture2D AnimationTexture
	{
		get
		{
			return animationTextureValue;
		}
		set
		{
			animationTextureValue = value;
		}
	}

	public IList<int> BoneIndices
	{
		get
		{
			return boneIndicesValue;
		}
		set
		{
			boneIndicesValue = value;
		}
	}

	public SkinningData()
	{
	}

	public SkinningData(IList<int> boneIndices, IDictionary<string, AnimationClip> animationClips, IList<Matrix> bindPose, IList<Matrix> inverseBindPose, IList<int> skeletonHierarchy, Texture2D animationTexture)
	{
		animationClipsValue = animationClips;
		bindPoseValue = bindPose;
		inverseBindPoseValue = inverseBindPose;
		skeletonHierarchyValue = skeletonHierarchy;
		animationTextureValue = animationTexture;
		boneIndicesValue = boneIndices;
	}

	public void Set(IList<int> boneIndices, IDictionary<string, AnimationClip> animationClips, IList<Matrix> bindPose, IList<Matrix> inverseBindPose, IList<int> skeletonHierarchy)
	{
		animationClipsValue = animationClips;
		bindPoseValue = bindPose;
		inverseBindPoseValue = inverseBindPose;
		skeletonHierarchyValue = skeletonHierarchy;
		boneIndicesValue = boneIndices;
	}
}
