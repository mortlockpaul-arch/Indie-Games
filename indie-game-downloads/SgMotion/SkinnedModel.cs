using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace SgMotion;

public class SkinnedModel
{
	private SkinnedModelBoneCollection skeleton;

	private AnimationClipDictionary animationClips;

	private string modelFile;

	public SkinnedModelBoneCollection SkeletonBones => skeleton;

	public AnimationClipDictionary AnimationClips => animationClips;

	public string ModelFile => modelFile;

	internal SkinnedModel()
	{
	}

	public void Draw(TimeSpan elapsedTime)
	{
	}

	internal static SkinnedModel Read(ContentReader input)
	{
		SkinnedModel skinnedModel = new SkinnedModel();
		skinnedModel.modelFile = input.ReadString();
		skinnedModel.ReadBones(input);
		skinnedModel.ReadAnimations(input);
		return skinnedModel;
	}

	private void ReadBones(ContentReader input)
	{
		int num = input.ReadInt32();
		List<SkinnedModelBone> skinnedModelBoneList = new List<SkinnedModelBone>(num);
		for (int i = 0; i < num; i++)
		{
			input.ReadSharedResource(delegate(SkinnedModelBone skinnedBone)
			{
				skinnedModelBoneList.Add(skinnedBone);
			});
		}
		skeleton = new SkinnedModelBoneCollection(skinnedModelBoneList);
	}

	private void ReadAnimations(ContentReader input)
	{
		int num = input.ReadInt32();
		Dictionary<string, AnimationClip> animationClipDictionary = new Dictionary<string, AnimationClip>();
		for (int i = 0; i < num; i++)
		{
			input.ReadSharedResource(delegate(AnimationClip animationClip)
			{
				animationClipDictionary.Add(animationClip.Name, animationClip);
			});
		}
		animationClips = new AnimationClipDictionary(animationClipDictionary);
	}
}
