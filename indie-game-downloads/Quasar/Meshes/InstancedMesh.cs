using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Render;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class InstancedMesh : Mesh
{
	protected struct DrawPart
	{
		public int Begin;

		public int Count;

		public BoundingSphere Sphere;
	}

	public const string INSTANCING_SHADER = "Instancing";

	public const int MAX_INSTANCES = 1024;

	public const int GROUP_SIZE = 16;

	protected ModelMesh mesh;

	protected BoundingSphere meshSphere;

	protected VertexBuffer meshBuffer;

	protected IndexBuffer meshIndexBuffer;

	protected int instanceNumber;

	protected Matrix[] instanceData;

	private Matrix[] drawData = new Matrix[16];

	private VertexDeclaration meshDeclaration;

	private List<DrawPart> drawParts = new List<DrawPart>();

	private int vertexCount;

	public ModelMesh ModelMesh => mesh;

	public int InstanceNumber => instanceNumber;

	public bool AddInstance(ref Matrix data, bool checkCull)
	{
		if (instanceNumber >= instanceData.Length)
		{
			return true;
		}
		if (checkCull)
		{
			BoundingSphere sphere = ReplaceSphere(meshSphere, ref data);
			if (Scene.CurrentInstance != null && Scene.CurrentInstance.Camera.CheckCull(ref sphere))
			{
				return true;
			}
			if (SceneRenderData.CurrentRenderData != null && SceneRenderData.CurrentRenderData.Camera.CheckCull(ref sphere))
			{
				return true;
			}
		}
		ref Matrix reference = ref instanceData[instanceNumber++];
		reference = data;
		return false;
	}

	public bool AddInstance(Matrix data, bool checkCull)
	{
		return AddInstance(ref data, checkCull);
	}

	public void ClearInstances()
	{
		instanceNumber = 0;
	}

	public void UpdateData()
	{
		UpdateParts();
	}

	protected void UpdateParts()
	{
		drawParts.Clear();
		int num = 0;
		int num2 = instanceNumber;
		while (num2 > 0)
		{
			DrawPart item = new DrawPart
			{
				Begin = num,
				Count = Math.Min(num2, 16)
			};
			BoundingSphere boundingSphere = ReplaceSphere(meshSphere, ref instanceData[num]);
			for (int i = num + 1; i < num + item.Count; i++)
			{
				boundingSphere = BoundingSphere.CreateMerged(boundingSphere, ReplaceSphere(meshSphere, ref instanceData[i]));
			}
			item.Sphere = boundingSphere;
			drawParts.Add(item);
			num += item.Count;
			num2 -= item.Count;
		}
	}

	protected BoundingSphere ReplaceSphere(BoundingSphere sphere, ref Matrix data)
	{
		float num = GameMath.VectorMaxValue(GameMath.MatrixScale(ref data));
		return new BoundingSphere(sphere.Center + data.Translation, sphere.Radius * num);
	}

	public InstancedMesh(ModelMesh mesh)
		: this(mesh, 1024)
	{
	}

	public InstancedMesh(ModelMesh mesh, int maxInstances)
	{
		DebugColor = Color.Orange;
		instanceData = new Matrix[maxInstances];
		this.mesh = mesh;
		VertexDeclaration vertexDeclaration = null;
		VertexBuffer vertexBuffer = null;
		IndexBuffer indexBuffer = null;
		if (mesh.MeshParts.Count > 1)
		{
			throw new Exception("More than 1 MeshPart per mesh is not compatible with Instancing!");
		}
		foreach (ModelMeshPart meshPart in mesh.MeshParts)
		{
			vertexDeclaration = meshPart.VertexBuffer.VertexDeclaration;
			vertexBuffer = meshPart.VertexBuffer;
			indexBuffer = meshPart.IndexBuffer;
			Material material = new Material(meshPart);
			material.AddMatrixArrayParameter(drawData);
			material.AddIntParameter(0);
			materials.Add(material);
		}
		shader = ShaderManager.Shaders["Instancing"];
		meshDeclaration = vertexDeclaration;
		vertexCount = vertexBuffer.VertexCount;
		meshBuffer = vertexBuffer;
		ReplicateIndexData(indexBuffer);
		meshSphere = mesh.BoundingSphere;
	}

	public void SetMaxInstances(int maxInstances)
	{
		instanceData = new Matrix[maxInstances];
		instanceNumber = 0;
	}

	private void ReplicateIndexData(IndexBuffer indexBuffer)
	{
		int indexCount = indexBuffer.IndexCount;
		ushort[] array = new ushort[indexCount];
		indexBuffer.GetData(array);
		ushort[] array2 = new ushort[indexCount * 16];
		int num = 0;
		for (int i = 0; i < 16; i++)
		{
			int num2 = i * vertexCount;
			for (int j = 0; j < indexCount; j++)
			{
				array2[num] = (ushort)(array[j] + num2);
				num++;
			}
		}
		meshIndexBuffer = new IndexBuffer(Engine.Device, typeof(ushort), array2.Length, BufferUsage.None);
		meshIndexBuffer.SetData(array2);
	}

	public InstancedMesh(string filename)
		: this(filename, 1024)
	{
	}

	public InstancedMesh(string filename, int maxInstances)
		: this(Engine.ContentManager.Load<Model>(filename.StartsWith("Models/") ? filename : ("Models/" + filename)).Meshes[0], maxInstances)
	{
	}

	public static List<InstancedMesh> LoadMeshes(string filename, Shader shader)
	{
		return LoadMeshes(filename, shader, 1024);
	}

	public static List<InstancedMesh> LoadMeshes(string filename, Shader shader, int maxInstances)
	{
		List<InstancedMesh> list = LoadMeshes(filename, maxInstances);
		foreach (InstancedMesh item in list)
		{
			item.shader = shader;
		}
		return list;
	}

	public static List<InstancedMesh> LoadMeshes(string filename)
	{
		return LoadMeshes(filename, 1024);
	}

	public static List<InstancedMesh> LoadMeshes(string filename, int maxInstances)
	{
		List<InstancedMesh> list = new List<InstancedMesh>(10);
		Model model = Engine.ContentManager.Load<Model>("Models/" + filename);
		foreach (ModelMesh mesh in model.Meshes)
		{
			list.Add(new InstancedMesh(mesh, maxInstances));
		}
		return list;
	}

	public override void Render(Transform motion)
	{
		int count = materials.Count;
		for (int i = 0; i < count; i++)
		{
			SceneRenderData.CurrentRenderData.Sorter.Add(this, motion, i);
		}
	}

	public override void RenderMaterial(Transform motion, int material)
	{
		foreach (DrawPart drawPart in drawParts)
		{
			if (!SceneRenderData.CurrentRenderData.Camera.CheckCull(drawPart.Sphere, motion))
			{
				PrepareDrawPart(drawPart.Begin, drawPart.Count);
				Engine.Device.Indices = meshIndexBuffer;
				Material material2 = materials[material];
				material2.SetIntParameter(0, vertexCount);
				ModelMeshPart modelMeshPart = mesh.MeshParts[material];
				shader.BeginRender(motion, material2);
				Engine.Device.SetVertexBuffer(meshBuffer, modelMeshPart.VertexOffset);
				int num = shader.PassNumber(material2);
				for (int i = 0; i < num; i++)
				{
					shader.ApplyPass(i);
					Engine.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawPart.Count * vertexCount, modelMeshPart.StartIndex, drawPart.Count * modelMeshPart.PrimitiveCount);
				}
				shader.EndRender();
			}
		}
	}

	protected virtual void PrepareDrawPart(int begin, int count)
	{
		Array.Copy(instanceData, begin, drawData, 0, count);
	}

	public override void Dispose()
	{
		if (meshIndexBuffer != null)
		{
			meshIndexBuffer.Dispose();
			meshIndexBuffer = null;
		}
		base.Dispose();
	}
}
