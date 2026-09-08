using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Render;

namespace Quasar.Meshes;

public class XMesh : Mesh
{
	private ModelMesh mesh;

	public ModelMesh ModelMesh => mesh;

	public XMesh(ModelMesh mesh)
	{
		DebugColor = Color.Gray;
		this.mesh = mesh;
		boundingSphere = mesh.BoundingSphere;
		if (boundingSphere.Radius == 0f)
		{
			boundingSphere.Radius = 10000f;
		}
		foreach (ModelMeshPart meshPart in mesh.MeshParts)
		{
			materials.Add(new Material(meshPart));
		}
	}

	protected XMesh()
	{
		DebugColor = Color.Gray;
	}

	public static List<XMesh> LoadMeshes(string filename)
	{
		List<XMesh> list = new List<XMesh>(10);
		Model model = Engine.ContentManager.Load<Model>("Models/" + filename);
		foreach (ModelMesh mesh in model.Meshes)
		{
			list.Add(new XMesh(mesh));
		}
		return list;
	}

	public XMesh(string filename)
		: this(Engine.ContentManager.Load<Model>("Models/" + filename).Meshes[0])
	{
	}

	public override void Render(Transform motion)
	{
		if (!SceneRenderData.CurrentRenderData.Camera.CheckCull(ref boundingSphere, motion))
		{
			int count = materials.Count;
			for (int i = 0; i < count; i++)
			{
				SceneRenderData.CurrentRenderData.Sorter.Add(this, motion, i);
			}
		}
	}

	public override void RenderMaterial(Transform motion, int material)
	{
		Material material2 = materials[material];
		ModelMeshPart modelMeshPart = mesh.MeshParts[material];
		Shader shader = PrepareShader(base.shader, motion, material2);
		Engine.Device.Indices = modelMeshPart.IndexBuffer;
		Engine.Device.SetVertexBuffer(modelMeshPart.VertexBuffer);
		int num = shader.PassNumber(material2);
		for (int i = 0; i < num; i++)
		{
			shader.ApplyPass(i);
			Engine.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, modelMeshPart.VertexOffset, 0, modelMeshPart.NumVertices, modelMeshPart.StartIndex, modelMeshPart.PrimitiveCount);
		}
		shader.EndRender();
	}

	public override void Dispose()
	{
		mesh = null;
		base.Dispose();
	}

	protected override Mesh DoClone()
	{
		return new XMesh();
	}

	protected override void CopyMeshData(Mesh mesh)
	{
		base.CopyMeshData(mesh);
		((XMesh)mesh).mesh = this.mesh;
	}
}
