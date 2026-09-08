using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SgMotion;

public class SkinnedModelBone
{
	private readonly ushort index;

	private readonly string name;

	private SkinnedModelBone parent;

	private SkinnedModelBoneCollection children;

	private readonly Pose bindPose;

	private readonly Matrix inverseBindPoseTransform;

	public ushort Index => index;

	public string Name => name;

	public SkinnedModelBone Parent
	{
		get
		{
			return parent;
		}
		internal set
		{
			parent = value;
		}
	}

	public SkinnedModelBoneCollection Children
	{
		get
		{
			return children;
		}
		internal set
		{
			children = value;
		}
	}

	public Pose BindPose => bindPose;

	public Matrix InverseBindPoseTransform => inverseBindPoseTransform;

	internal SkinnedModelBone(ushort index, string name, Pose bindPose, Matrix inverseBindPoseTransform)
	{
		this.index = index;
		this.name = name;
		this.bindPose = bindPose;
		this.inverseBindPoseTransform = inverseBindPoseTransform;
	}

	public void CopyBindPoseTo(Pose[] destination)
	{
		int boneIndex = 0;
		CopyBindPoseTo(destination, ref boneIndex);
	}

	private void CopyBindPoseTo(Pose[] destination, ref int boneIndex)
	{
		ref Pose reference = ref destination[boneIndex++];
		reference = bindPose;
		for (int i = 0; i < children.Count; i++)
		{
			children[i].CopyBindPoseTo(destination, ref boneIndex);
		}
	}

	internal static SkinnedModelBone Read(ContentReader input)
	{
		ushort num = input.ReadUInt16();
		string text = input.ReadString();
		Pose pose = default(Pose);
		pose.Translation = input.ReadVector3();
		pose.Orientation = input.ReadQuaternion();
		pose.Scale = input.ReadVector3();
		Matrix matrix = input.ReadMatrix();
		SkinnedModelBone skinnedBone = new SkinnedModelBone(num, text, pose, matrix);
		input.ReadSharedResource(delegate(SkinnedModelBone parentBone)
		{
			skinnedBone.parent = parentBone;
		});
		int num2 = input.ReadInt32();
		List<SkinnedModelBone> childrenList = new List<SkinnedModelBone>(num2);
		for (int num3 = 0; num3 < num2; num3++)
		{
			input.ReadSharedResource(delegate(SkinnedModelBone childBone)
			{
				childrenList.Add(childBone);
			});
		}
		skinnedBone.children = new SkinnedModelBoneCollection(childrenList);
		return skinnedBone;
	}
}
