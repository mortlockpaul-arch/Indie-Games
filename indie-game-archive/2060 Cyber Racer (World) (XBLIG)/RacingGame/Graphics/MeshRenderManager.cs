using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Shaders;

namespace RacingGame.Graphics;

public class MeshRenderManager
{
	public class RenderableMesh
	{
		public VertexBuffer vertexBuffer;

		public IndexBuffer indexBuffer;

		public Material material;

		public EffectTechnique usedTechnique;

		public EffectParameter worldParameter;

		public VertexDeclaration vertexDeclaration;

		public int streamOffset;

		public int vertexStride;

		public int baseVertex;

		public int numVertices;

		public int startIndex;

		public int primitiveCount;

		public List<Matrix> renderMatrices = new List<Matrix>();

		public RenderableMesh(VertexBuffer setVertexBuffer, IndexBuffer setIndexBuffer, Material setMaterial, EffectTechnique setUsedTechnique, EffectParameter setWorldParameter, VertexDeclaration setVertexDeclaration, int setStreamOffset, int setVertexStride, int setBaseVertex, int setNumVertices, int setStartIndex, int setPrimitiveCount)
		{
			vertexBuffer = setVertexBuffer;
			indexBuffer = setIndexBuffer;
			material = setMaterial;
			usedTechnique = setUsedTechnique;
			worldParameter = setWorldParameter;
			vertexDeclaration = setVertexDeclaration;
			streamOffset = setStreamOffset;
			vertexStride = setVertexStride;
			baseVertex = setBaseVertex;
			numVertices = setNumVertices;
			startIndex = setStartIndex;
			primitiveCount = setPrimitiveCount;
		}

		public void RenderMesh(Matrix worldMatrix)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			ShaderEffect.normalMapping.WorldMatrix = worldMatrix;
			ShaderEffect.normalMapping.Effect.CommitChanges();
			if (lastVertexBufferSet != vertexBuffer || lastIndexBufferSet != indexBuffer)
			{
				lastVertexBufferSet = vertexBuffer;
				lastIndexBufferSet = indexBuffer;
				BaseGame.Device.Vertices[0].SetSource(vertexBuffer, streamOffset, vertexStride);
				BaseGame.Device.Indices = indexBuffer;
			}
			BaseGame.Device.DrawIndexedPrimitives((PrimitiveType)4, baseVertex, 0, numVertices, startIndex, primitiveCount);
		}

		public void Render()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < renderMatrices.Count; i++)
			{
				RenderMesh(renderMatrices[i]);
			}
			renderMatrices.Clear();
		}
	}

	public class MeshesPerMaterial
	{
		public Material material;

		public List<RenderableMesh> meshes = new List<RenderableMesh>();

		public int NumberOfRenderMatrices
		{
			get
			{
				int num = 0;
				for (int i = 0; i < meshes.Count; i++)
				{
					num += meshes[i].renderMatrices.Count;
				}
				return num;
			}
		}

		public MeshesPerMaterial(Material setMaterial)
		{
			material = setMaterial;
		}

		public void Add(RenderableMesh addMesh)
		{
			if (addMesh.material != material)
			{
				throw new ArgumentException("Invalid material, to add a mesh to MeshesPerMaterial it must use the specified material=" + material);
			}
			meshes.Add(addMesh);
		}

		public void Render()
		{
			ShaderEffect.normalMapping.SetParametersOptimized(material);
			BaseGame.Device.VertexDeclaration = meshes[0].vertexDeclaration;
			if (material.HasAlpha)
			{
				BaseGame.Device.RenderState.AlphaTestEnable = true;
				BaseGame.Device.RenderState.ReferenceAlpha = 128;
				BaseGame.Device.RenderState.CullMode = (CullMode)1;
			}
			for (int i = 0; i < meshes.Count; i++)
			{
				RenderableMesh renderableMesh = meshes[i];
				if (renderableMesh.renderMatrices.Count > 0)
				{
					renderableMesh.Render();
				}
			}
			if (material.HasAlpha)
			{
				BaseGame.Device.RenderState.AlphaTestEnable = false;
				BaseGame.Device.RenderState.CullMode = (CullMode)3;
			}
		}
	}

	public class MeshesPerMaterialPerTechniques
	{
		public EffectTechnique technique;

		public List<MeshesPerMaterial> meshesPerMaterials = new List<MeshesPerMaterial>();

		public int NumberOfRenderMatrices
		{
			get
			{
				int num = 0;
				for (int i = 0; i < meshesPerMaterials.Count; i++)
				{
					num += meshesPerMaterials[i].NumberOfRenderMatrices;
				}
				return num;
			}
		}

		public MeshesPerMaterialPerTechniques(EffectTechnique setTechnique)
		{
			technique = setTechnique;
		}

		public void Add(RenderableMesh addMesh)
		{
			if (addMesh.usedTechnique != technique)
			{
				throw new ArgumentException("Invalid technique, to add a mesh to MeshesPerMaterialPerTechniques it must use the specified technique=" + technique.Name);
			}
			for (int i = 0; i < meshesPerMaterials.Count; i++)
			{
				MeshesPerMaterial meshesPerMaterial = meshesPerMaterials[i];
				if (meshesPerMaterial.material == addMesh.material)
				{
					meshesPerMaterial.Add(addMesh);
					return;
				}
			}
			MeshesPerMaterial meshesPerMaterial2 = new MeshesPerMaterial(addMesh.material);
			meshesPerMaterial2.Add(addMesh);
			meshesPerMaterials.Add(meshesPerMaterial2);
		}

		public void Render(Effect effect)
		{
			effect.CurrentTechnique = technique;
			try
			{
				effect.Begin((SaveStateMode)0);
				EffectPass val = effect.CurrentTechnique.Passes[0];
				val.Begin();
				for (int i = 0; i < meshesPerMaterials.Count; i++)
				{
					MeshesPerMaterial meshesPerMaterial = meshesPerMaterials[i];
					if (meshesPerMaterial.NumberOfRenderMatrices > 0)
					{
						meshesPerMaterial.Render();
					}
				}
				val.End();
			}
			finally
			{
				effect.End();
			}
		}
	}

	private static VertexBuffer lastVertexBufferSet;

	private static IndexBuffer lastIndexBufferSet;

	private List<MeshesPerMaterialPerTechniques> sortedMeshes = new List<MeshesPerMaterialPerTechniques>();

	public RenderableMesh Add(VertexBuffer vertexBuffer, IndexBuffer indexBuffer, ModelMeshPart part, Effect effect)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		string name = effect.CurrentTechnique.Name;
		MeshesPerMaterialPerTechniques meshesPerMaterialPerTechniques = null;
		for (int i = 0; i < sortedMeshes.Count; i++)
		{
			MeshesPerMaterialPerTechniques meshesPerMaterialPerTechniques2 = sortedMeshes[i];
			if (meshesPerMaterialPerTechniques2.technique.Name == name)
			{
				meshesPerMaterialPerTechniques = meshesPerMaterialPerTechniques2;
				break;
			}
		}
		if (meshesPerMaterialPerTechniques == null)
		{
			meshesPerMaterialPerTechniques = new MeshesPerMaterialPerTechniques(ShaderEffect.normalMapping.GetTechnique(name));
			sortedMeshes.Add(meshesPerMaterialPerTechniques);
		}
		Material material = new Material(effect);
		for (int j = 0; j < meshesPerMaterialPerTechniques.meshesPerMaterials.Count; j++)
		{
			MeshesPerMaterial meshesPerMaterial = meshesPerMaterialPerTechniques.meshesPerMaterials[j];
			if (meshesPerMaterial.material.diffuseTexture == material.diffuseTexture && meshesPerMaterial.material.normalTexture == material.normalTexture && meshesPerMaterial.material.ambientColor == material.ambientColor && meshesPerMaterial.material.diffuseColor == material.diffuseColor && meshesPerMaterial.material.specularColor == material.specularColor && meshesPerMaterial.material.specularPower == material.specularPower)
			{
				material = meshesPerMaterial.material;
				break;
			}
		}
		RenderableMesh renderableMesh = new RenderableMesh(vertexBuffer, indexBuffer, material, meshesPerMaterialPerTechniques.technique, ShaderEffect.normalMapping.WorldParameter, part.VertexDeclaration, part.StreamOffset, part.VertexStride, part.BaseVertex, part.NumVertices, part.StartIndex, part.PrimitiveCount);
		meshesPerMaterialPerTechniques.Add(renderableMesh);
		return renderableMesh;
	}

	public void Render()
	{
		BaseGame.Device.RenderState.DepthBufferEnable = true;
		BaseGame.Device.RenderState.DepthBufferWriteEnable = true;
		Effect effect = ShaderEffect.normalMapping.Effect;
		ShaderEffect.normalMapping.SetParametersOptimizedGeneral();
		lastVertexBufferSet = null;
		lastIndexBufferSet = null;
		for (int i = 0; i < sortedMeshes.Count; i++)
		{
			MeshesPerMaterialPerTechniques meshesPerMaterialPerTechniques = sortedMeshes[i];
			if (meshesPerMaterialPerTechniques.NumberOfRenderMatrices > 0)
			{
				meshesPerMaterialPerTechniques.Render(effect);
			}
		}
	}
}
