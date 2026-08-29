using System;
using System.Collections.Generic;
using Common;
using GKEngine.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Entities;

public class MaxModel
{
	public Base3D parent;

	public List<MaxModelPart> modelParts = new List<MaxModelPart>();

	public int modelPartsCount;

	public MeshData collision = new MeshData();

	public bool built;

	public List<Bone> bones = new List<Bone>();

	public Dictionary<string, Matrix> tracers = new Dictionary<string, Matrix>();

	public Dictionary<string, MeshData> areas = new Dictionary<string, MeshData>();

	public AnimationClip clip;

	public MaxBonePlayer animation;

	public bool visible
	{
		get
		{
			bool flag = false;
			for (int i = 0; i < modelPartsCount; i++)
			{
				flag |= modelParts[i].visible;
			}
			return flag;
		}
		set
		{
			for (int i = 0; i < modelPartsCount; i++)
			{
				modelParts[i].visible = value;
			}
		}
	}

	public MaxModel()
	{
	}

	internal MaxModel(ContentReader input)
	{
		int num = input.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			modelParts.Add(MaxModelPart.Read(ref input));
		}
		modelPartsCount = modelParts.Count;
		num = input.ReadInt32();
		for (int j = 0; j < num; j++)
		{
			tracers.Add(input.ReadString(), input.ReadObject<Matrix>());
		}
		num = input.ReadInt32();
		for (int k = 0; k < num; k++)
		{
			string key = input.ReadString();
			MeshData value = MeshData.Read(ref input);
			areas.Add(key, value);
		}
		collision = MeshData.Read(ref input);
		num = input.ReadInt32();
		for (int l = 0; l < num; l++)
		{
			bones.Add(new Bone(input.ReadInt32(), input.ReadInt32(), input.ReadString(), input.ReadObject<Matrix>(), input.ReadObject<Matrix>()));
		}
		num = input.ReadInt32();
		for (int m = 0; m < num; m++)
		{
			string text = input.ReadString();
			TimeSpan duration = input.ReadObject<TimeSpan>();
			int num2 = input.ReadInt32();
			List<Keyframe> list = new List<Keyframe>();
			for (int n = 0; n < num2; n++)
			{
				list.Add(new Keyframe(input.ReadInt32(), input.ReadObject<TimeSpan>(), input.ReadObject<Matrix>()));
			}
			if (text == MaxBonePlayer.DEFAULT_CLIP_NAME)
			{
				clip = new AnimationClip(duration, list);
			}
		}
		if (clip != null)
		{
			animation = new MaxBonePlayer(this);
			animation.SetClip(clip);
		}
	}

	public void Build(Base3D oParent)
	{
		parent = oParent;
		foreach (MaxModelPart modelPart in modelParts)
		{
			modelPart.Build(this);
		}
		if (animation != null)
		{
			animation.GoToAndStop(TimeSpan.Zero);
		}
		built = true;
	}

	public void Render(Camera camera)
	{
		Render(parent.matrix, camera);
	}

	public void Render(Matrix world, Camera camera)
	{
		for (int i = 0; i < modelPartsCount; i++)
		{
			modelParts[i].Render(ref world, camera);
		}
	}

	public void RenderEffect(ref Effect effect)
	{
		GraphicsDevice graphicsDevice = GameEngine.instance.GraphicsDevice;
		for (int i = 0; i < modelPartsCount; i++)
		{
			MaxModelPart maxModelPart = modelParts[i];
			if (animation != null && maxModelPart.material.useBones)
			{
				effect.Parameters["Bones"].SetValue(animation.skinTransforms);
			}
			graphicsDevice.SetVertexBuffer(maxModelPart.vertexBuffer);
			graphicsDevice.Indices = maxModelPart.indexBuffer;
			EffectPass effectPass = effect.CurrentTechnique.Passes[0];
			effectPass.Apply();
			graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, maxModelPart.vertexBuffer.VertexCount, 0, maxModelPart.triangleCount);
		}
	}

	public MaxModel Clone()
	{
		MaxModel maxModel = new MaxModel();
		for (int i = 0; i < modelParts.Count; i++)
		{
			maxModel.modelParts.Add(modelParts[i].Clone());
		}
		maxModel.modelPartsCount = maxModel.modelParts.Count;
		foreach (KeyValuePair<string, Matrix> tracer in tracers)
		{
			maxModel.tracers.Add(tracer.Key, tracer.Value);
		}
		foreach (KeyValuePair<string, MeshData> area in areas)
		{
			maxModel.areas.Add(area.Key, area.Value.Clone());
		}
		maxModel.collision = collision.Clone();
		for (int j = 0; j < bones.Count; j++)
		{
			maxModel.bones.Add(bones[j].Clone());
		}
		for (int k = 0; k < maxModel.bones.Count; k++)
		{
			if (maxModel.bones[k].parentIndex <= -1)
			{
				continue;
			}
			for (int l = 0; l < maxModel.bones.Count; l++)
			{
				if (maxModel.bones[k].index == maxModel.bones[l].parentIndex)
				{
					maxModel.bones[l].children.Add(maxModel.bones[k]);
					break;
				}
			}
		}
		return maxModel;
	}

	public void Update(GameTime oGameTime)
	{
		if (animation != null)
		{
			animation.Update(oGameTime.ElapsedGameTime);
		}
	}

	public void Dispose()
	{
		if (modelParts != null)
		{
			for (int i = 0; i < modelParts.Count; i++)
			{
				modelParts[i].Dispose();
			}
			modelParts.Clear();
		}
		modelParts = null;
		if (areas != null)
		{
			foreach (KeyValuePair<string, MeshData> area in areas)
			{
				area.Value.Dispose();
			}
			areas.Clear();
		}
		areas = null;
		if (collision != null)
		{
			collision.Dispose();
		}
		collision = null;
		if (bones != null)
		{
			for (int j = 0; j < bones.Count; j++)
			{
				bones[j] = null;
			}
			bones.Clear();
		}
		bones = null;
		if (tracers != null)
		{
			tracers.Clear();
		}
		tracers = null;
	}

	public MaxModelPart PartFromName(string xName)
	{
		MaxModelPart result = null;
		for (int i = 0; i < modelParts.Count; i++)
		{
			if (modelParts[i].name.ToLower() == xName.ToLower())
			{
				result = modelParts[i];
				break;
			}
		}
		return result;
	}
}
