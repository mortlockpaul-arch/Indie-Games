using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class PropInstanceDrawCls
{
	public bool IsValid;

	public Model propModel;

	private int MaxInstances;

	private int[] instanceCount;

	private Matrix[] propTransforms;

	private DynamicVertexBuffer[] instanceVertexBuffer;

	private int[] instanceCountShadow;

	private Matrix[] propTransformsShadow;

	private DynamicVertexBuffer[] instanceVertexBufferShadow;

	private static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0), new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1), new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2), new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3));

	public void MakeValid(int max, string n)
	{
		IsValid = true;
		MaxInstances = max;
		instanceCount = new int[2];
		instanceCount[0] = 0;
		instanceCount[1] = 0;
		instanceCountShadow = new int[2];
		instanceCountShadow[0] = 0;
		instanceCountShadow[1] = 0;
		instanceVertexBuffer = new DynamicVertexBuffer[2];
		instanceVertexBuffer[0] = new DynamicVertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, instanceVertexDeclaration, max, BufferUsage.WriteOnly);
		instanceVertexBuffer[1] = new DynamicVertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, instanceVertexDeclaration, max, BufferUsage.WriteOnly);
		instanceVertexBufferShadow = new DynamicVertexBuffer[2];
		instanceVertexBufferShadow[0] = new DynamicVertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, instanceVertexDeclaration, max / 4, BufferUsage.WriteOnly);
		instanceVertexBufferShadow[1] = new DynamicVertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, instanceVertexDeclaration, max / 4, BufferUsage.WriteOnly);
		propModel = EndGameEngine.GameAssetMgr.Load<Model>(n);
		propTransforms = new Matrix[max];
		propTransformsShadow = new Matrix[max / 4];
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			ModelMesh modelMesh = propModel.Meshes[i];
			modelMesh.Tag = new MeshAttributesParams();
			for (int j = 0; j < modelMesh.MeshParts.Count; j++)
			{
				modelMesh.MeshParts[j].Tag = new PropEffectParams(modelMesh.MeshParts[j].Effect);
			}
		}
	}

	public void Add(int qIndex, ref Matrix m)
	{
		if (instanceCount[qIndex] < MaxInstances)
		{
			ref Matrix reference = ref propTransforms[instanceCount[qIndex]];
			reference = m;
			instanceCount[qIndex]++;
		}
	}

	public void AddShadow(int qIndex, ref Matrix m)
	{
		if (instanceCountShadow[qIndex] < MaxInstances / 4)
		{
			ref Matrix reference = ref propTransformsShadow[instanceCountShadow[qIndex]];
			reference = m;
			instanceCountShadow[qIndex]++;
		}
	}

	public void Reset(int qIndex)
	{
		instanceCount[qIndex] = 0;
		instanceCountShadow[qIndex] = 0;
	}

	public void Update(int qIndex)
	{
		if (instanceCount[qIndex] > 0)
		{
			instanceVertexBuffer[qIndex].SetData(propTransforms, 0, MaxInstances, SetDataOptions.Discard);
		}
		if (instanceCountShadow[qIndex] > 0)
		{
			instanceVertexBufferShadow[qIndex].SetData(propTransformsShadow, 0, MaxInstances / 4, SetDataOptions.Discard);
		}
	}

	public void Draw(PlayerBase viewer, int qIndex)
	{
		if (instanceCount[qIndex] <= 0)
		{
			return;
		}
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			ModelMesh modelMesh = propModel.Meshes[i];
			for (int j = 0; j < modelMesh.MeshParts.Count; j++)
			{
				ModelMeshPart modelMeshPart = modelMesh.MeshParts[j];
				((PropEffectParams)modelMeshPart.Tag).matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
				((PropEffectParams)modelMeshPart.Tag).eyePosition.SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
				EndGameEngine.GraphicMgr.GraphicsDevice.SetVertexBuffer(modelMeshPart.VertexBuffer);
				EndGameEngine.GraphicMgr.GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(modelMeshPart.VertexBuffer, modelMeshPart.VertexOffset, 0), new VertexBufferBinding(instanceVertexBuffer[qIndex], 0, 1));
				modelMeshPart.Effect.GraphicsDevice.Indices = modelMeshPart.IndexBuffer;
				modelMeshPart.Effect.CurrentTechnique.Passes[26].Apply();
				modelMeshPart.Effect.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, modelMeshPart.NumVertices, modelMeshPart.StartIndex, modelMeshPart.PrimitiveCount, instanceCount[qIndex]);
			}
		}
	}

	public void DrawShadowMap(PlayerBase viewer, ref Matrix LightViewProj, ref Vector3 lightPos, int qIndex)
	{
		if (instanceCountShadow[qIndex] <= 0)
		{
			return;
		}
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			ModelMesh modelMesh = propModel.Meshes[i];
			for (int j = 0; j < modelMesh.MeshParts.Count; j++)
			{
				ModelMeshPart modelMeshPart = modelMesh.MeshParts[j];
				((PropEffectParams)modelMeshPart.Tag).matLightViewProj.SetValue(LightViewProj);
				((PropEffectParams)modelMeshPart.Tag).eyePosition.SetValue(lightPos);
				EndGameEngine.GraphicMgr.GraphicsDevice.SetVertexBuffer(modelMeshPart.VertexBuffer);
				EndGameEngine.GraphicMgr.GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(modelMeshPart.VertexBuffer, modelMeshPart.VertexOffset, 0), new VertexBufferBinding(instanceVertexBufferShadow[qIndex], 0, 1));
				modelMeshPart.Effect.GraphicsDevice.Indices = modelMeshPart.IndexBuffer;
				modelMeshPart.Effect.CurrentTechnique.Passes[27].Apply();
				modelMeshPart.Effect.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, modelMeshPart.NumVertices, modelMeshPart.StartIndex, modelMeshPart.PrimitiveCount, instanceCountShadow[qIndex]);
			}
		}
	}
}
