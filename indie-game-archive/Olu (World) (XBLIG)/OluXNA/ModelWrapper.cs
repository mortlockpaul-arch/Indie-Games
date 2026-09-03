using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class ModelWrapper
{
	public Model model;

	public EffectParameterCollectionRedux[] epc;

	public Matrix[] transforms;

	public Matrix[][] palette;

	public VertexNormalTex[][] vertices;

	public short[][] indices;

	public List<int>[] indicesToDraw;

	public Dictionary<string, int[]> boneNames;

	public VertexBuffer[] vertBuffer;

	public IndexBuffer[] indexBuffer;

	public VertexDeclaration[] vertDec;

	public int[] parentBones;

	public int[] vertCount;

	public ModelWrapper()
	{
	}

	public ModelWrapper(Model _model)
		: this(_model, copyData: false)
	{
	}

	public ModelWrapper(Model _model, bool copyData)
	{
		SetModel(_model, copyData);
	}

	public ModelWrapper(ModelWrapper other)
		: this(other, copyEPC: false)
	{
	}

	public ModelWrapper(ModelWrapper other, bool copyEPC)
	{
		model = other.model;
		epc = new EffectParameterCollectionRedux[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count];
		if (copyEPC)
		{
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count; i++)
			{
				epc[i] = new EffectParameterCollectionRedux(other.epc[i]);
			}
		}
		else
		{
			for (int j = 0; j < ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count; j++)
			{
				epc[j] = other.epc[j];
			}
		}
		transforms = (Matrix[])other.transforms.Clone();
		if (other.palette != null)
		{
			palette = new Matrix[other.palette.Length][];
			for (int k = 0; k < other.palette.Length; k++)
			{
				palette[k] = (Matrix[])other.palette[k].Clone();
			}
		}
		vertices = other.vertices;
		indices = other.indices;
		if (other.indicesToDraw != null)
		{
			indicesToDraw = new List<int>[other.indicesToDraw.Length];
			for (int l = 0; l < other.indicesToDraw.Length; l++)
			{
				indicesToDraw[l] = new List<int>(other.indicesToDraw[l]);
			}
		}
	}

	public static ModelWrapper CombineModels(ModelWrapper from, ModelWrapper to)
	{
		ModelWrapper modelWrapper = new ModelWrapper(to, copyEPC: true);
		modelWrapper.MergeModel(from, to);
		modelWrapper.ResetIndicesToDraw();
		return modelWrapper;
	}

	public void MergeModel(ModelWrapper from, ModelWrapper to)
	{
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		VertexTransform[][] array = new VertexTransform[((ReadOnlyCollection<ModelMesh>)(object)from.model.Meshes).Count][];
		int[][] array2 = new int[((ReadOnlyCollection<ModelMesh>)(object)from.model.Meshes).Count][];
		vertBuffer = (VertexBuffer[])(object)new VertexBuffer[((ReadOnlyCollection<ModelMesh>)(object)from.model.Meshes).Count];
		indexBuffer = (IndexBuffer[])(object)new IndexBuffer[((ReadOnlyCollection<ModelMesh>)(object)from.model.Meshes).Count];
		vertDec = (VertexDeclaration[])(object)new VertexDeclaration[((ReadOnlyCollection<ModelMesh>)(object)from.model.Meshes).Count];
		vertCount = new int[((ReadOnlyCollection<ModelMesh>)(object)from.model.Meshes).Count];
		parentBones = new int[((ReadOnlyCollection<ModelMesh>)(object)from.model.Meshes).Count];
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)from.model.Meshes).Count; i++)
		{
			array[i] = new VertexTransform[from.indices[i].Length];
			array2[i] = new int[from.indices[i].Length];
			for (int j = 0; j < from.indices[i].Length; j++)
			{
				ref VertexTransform reference = ref array[i][j];
				reference = new VertexTransform(from.vertices[i][from.indices[i][j]], to.vertices[i][to.indices[i][j]]);
				array2[i][j] = j;
			}
			vertBuffer[i] = new VertexBuffer(BaseGame.Get().graphics.GraphicsDevice, VertexTransform.SizeInBytes() * array[i].Length, (BufferUsage)8);
			vertBuffer[i].SetData<VertexTransform>(0, array[i], 0, array[i].Length, VertexTransform.SizeInBytes());
			indexBuffer[i] = new IndexBuffer(BaseGame.Get().graphics.GraphicsDevice, typeof(int), array2[i].Length, (BufferUsage)8);
			indexBuffer[i].SetData<int>(array2[i]);
			vertCount[i] = array[i].Length;
			parentBones[i] = ((ReadOnlyCollection<ModelMesh>)(object)from.model.Meshes)[i].ParentBone.Index;
		}
	}

	public void SetModel(Model _model, bool copyData)
	{
		model = _model;
		transforms = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)model.Bones).Count];
		model.CopyAbsoluteBoneTransformsTo(transforms);
		if (copyData)
		{
			int vertexStrideSize = ((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[0].MeshParts)[0].VertexDeclaration.GetVertexStrideSize(0);
			vertices = new VertexNormalTex[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count][];
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count; i++)
			{
				switch (vertexStrideSize)
				{
				case 32:
				{
					VertexNormal[] inArr2 = new VertexNormal[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[i].VertexBuffer.SizeInBytes / 32];
					((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[i].VertexBuffer.GetData<VertexNormal>(inArr2);
					ConvertVNToVNT(ref inArr2, out vertices[i]);
					break;
				}
				case 24:
				{
					VectorPositionNormal[] inArr = new VectorPositionNormal[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[i].VertexBuffer.SizeInBytes / 24];
					((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[i].VertexBuffer.GetData<VectorPositionNormal>(inArr);
					ConvertVPNToVNT(ref inArr, out vertices[i]);
					break;
				}
				case 40:
					vertices[i] = new VertexNormalTex[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[i].VertexBuffer.SizeInBytes / 40];
					((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[i].VertexBuffer.GetData<VertexNormalTex>(vertices[i]);
					break;
				}
			}
			indices = new short[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count][];
			for (int j = 0; j < ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count; j++)
			{
				indices[j] = new short[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[j].IndexBuffer.SizeInBytes / 2];
				((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[j].IndexBuffer.GetData<short>(indices[j]);
			}
		}
		ResetIndicesToDraw();
	}

	public void ResetIndicesToDraw()
	{
		indicesToDraw = new List<int>[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count];
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count; i++)
		{
			indicesToDraw[i] = new List<int>();
			indicesToDraw[i].Add(0);
			indicesToDraw[i].Add(((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[i].IndexBuffer.SizeInBytes / 2 - 1);
		}
	}

	public Matrix GetFirstTransform()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return transforms[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[0].ParentBone.Index];
	}

	public unsafe void GetEffectParameters(GraphicsDevice graphicsDevice, Effect effect)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		epc = new EffectParameterCollectionRedux[((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count];
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count; i++)
		{
			epc[i] = new EffectParameterCollectionRedux(effect);
		}
		for (int j = 0; j < ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count; j++)
		{
			ModelMesh val = ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[j];
			ModelMeshPart val2 = ((ReadOnlyCollection<ModelMeshPart>)(object)val.MeshParts)[0];
			epc[j]["DiffuseColor"] = val2.Effect.Parameters["DiffuseColor"].GetValueVector3();
			epc[j]["Alpha"] = val2.Effect.Parameters["Alpha"].GetValueSingle();
			epc[j]["DirLight0Direction"] = val2.Effect.Parameters["DirLight0Direction"].GetValueVector3();
			epc[j]["EmissiveColor"] = val2.Effect.Parameters["EmissiveColor"].GetValueVector3();
			epc[j]["SpecularColor"] = val2.Effect.Parameters["SpecularColor"].GetValueVector3();
			epc[j]["SpecularPower"] = val2.Effect.Parameters["SpecularPower"].GetValueSingle();
			if (val2.Effect.Parameters["BasicTexture"] != null)
			{
				epc[j]["BasicTexture"] = val2.Effect.Parameters["BasicTexture"].GetValueTexture2D();
				if (epc[j]["BasicTexture"] == null)
				{
					epc[j]["TextureEnabled"] = false;
				}
				else
				{
					epc[j]["TextureEnabled"] = true;
				}
			}
			else
			{
				epc[j]["TextureEnabled"] = false;
			}
			epc[j]["TextureMix"] = BaseGame.T_MIX;
			if (val2.Effect.Parameters["ShinePos"] != null)
			{
				epc[j]["ShinePos"] = val2.Effect.Parameters["ShinePos"].GetValueVector3();
				epc[j]["ShineDist"] = val2.Effect.Parameters["ShineDist"].GetValueSingle();
			}
		}
		Enumerator enumerator = model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.MeshParts.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						ModelMeshPart current2 = ((Enumerator)(ref enumerator2)).Current;
						current2.Effect = effect;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public void ConvertVNToVNT(ref VertexNormal[] inArr, out VertexNormalTex[] outArr)
	{
		outArr = new VertexNormalTex[inArr.Length];
		for (int i = 0; i < inArr.Length; i++)
		{
			ref VertexNormalTex reference = ref outArr[i];
			reference = new VertexNormalTex(inArr[i]);
		}
	}

	public void ConvertVPNToVNT(ref VectorPositionNormal[] inArr, out VertexNormalTex[] outArr)
	{
		outArr = new VertexNormalTex[inArr.Length];
		for (int i = 0; i < inArr.Length; i++)
		{
			ref VertexNormalTex reference = ref outArr[i];
			reference = new VertexNormalTex(inArr[i]);
		}
	}
}
