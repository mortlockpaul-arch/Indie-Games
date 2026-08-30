using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class ModelInstance
{
	protected Model model;

	protected Matrix[] transforms;

	protected Matrix[] originalTransforms;

	private Vector3[] transform_2_rotations;

	protected bool[] isCustomBone;

	private static readonly List<string> EmptyList = new List<string>();

	public ModelInstance(Model m)
		: this(m, EmptyList)
	{
	}

	public ModelInstance(Model m, List<string> CustomBoneNames)
	{
		model = m;
		int count = m.Bones.Count;
		isCustomBone = new bool[count];
		for (int i = 0; i < count; i++)
		{
			isCustomBone[i] = CustomBoneNames.Contains(m.Bones[i].Name);
		}
		transforms = new Matrix[count];
		originalTransforms = new Matrix[count];
		transform_2_rotations = new Vector3[count];
		foreach (ModelBone bone in m.Bones)
		{
			ref Matrix reference = ref originalTransforms[bone.Index];
			reference = bone.Transform;
			ref Vector3 reference2 = ref transform_2_rotations[bone.Index];
			reference2 = Vector3.Zero;
			UpdateTransform(bone.Index);
		}
	}

	public int FindBoneIndex(string boneName)
	{
		return model.Bones[boneName].Index;
	}

	public bool HasBone(string boneName)
	{
		ModelBone value = null;
		return model.Bones.TryGetValue(boneName, out value);
	}

	public void AddRotation(int boneIndex, Vector3 additionalRotation)
	{
		if (additionalRotation != Vector3.Zero)
		{
			transform_2_rotations[boneIndex] += additionalRotation;
			UpdateTransform(boneIndex);
		}
	}

	public void SetRotationX(int boneIndex, float v)
	{
		transform_2_rotations[boneIndex].X = v;
		UpdateTransform(boneIndex);
	}

	public void SetRotationY(int boneIndex, float v)
	{
		transform_2_rotations[boneIndex].Y = v;
		UpdateTransform(boneIndex);
	}

	public void SetRotationZ(int boneIndex, float v)
	{
		transform_2_rotations[boneIndex].Z = v;
		UpdateTransform(boneIndex);
	}

	private void UpdateTransform(int boneIndex)
	{
		ref Matrix reference = ref transforms[boneIndex];
		reference = Matrix.CreateRotationX(transform_2_rotations[boneIndex].X) * Matrix.CreateRotationY(transform_2_rotations[boneIndex].Y) * Matrix.CreateRotationZ(transform_2_rotations[boneIndex].Z) * originalTransforms[boneIndex];
	}

	public void Draw(Matrix world)
	{
		Draw(world, useDefaultLighting: true);
	}

	public virtual void Draw(Matrix world, bool useDefaultLighting)
	{
		foreach (ModelMesh mesh in model.Meshes)
		{
			if (!isCustomBone[mesh.ParentBone.Index])
			{
				Drawing3D_V2.DrawModelMesh(mesh, transforms[mesh.ParentBone.Index] * world, useDefaultLighting);
			}
		}
	}
}
