using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Global.Vertex;
using Quasar.Render;

namespace Quasar.Meshes.Generic;

public abstract class GenericMesh<VertexStruct> : Mesh where VertexStruct : struct, IPositionVertex
{
	protected VertexStruct[] verticesBuffer;

	protected int primitiveCount;

	public VertexStruct[] Vertices => verticesBuffer;

	public int PrimitiveCount
	{
		get
		{
			return primitiveCount;
		}
		set
		{
			primitiveCount = value;
		}
	}

	public GenericMesh()
	{
		DebugColor = Color.DarkGreen;
	}

	protected void initMesh(int vertexCount)
	{
		verticesBuffer = new VertexStruct[vertexCount];
	}

	protected void updateBoundingSphere(int usedVertices)
	{
		if (usedVertices > 0)
		{
			boundingSphere = GameMath.CreateBoundingSphereFromPoints(verticesBuffer, usedVertices);
		}
		else
		{
			boundingSphere.Radius = 0f;
		}
	}

	public override void Render(Transform motion)
	{
		if (primitiveCount <= 0)
		{
			return;
		}
		if (boundingSphere.Radius != 0f)
		{
			if (!boundingSphereIgnoreTransform)
			{
				if (SceneRenderData.CurrentRenderData.Camera.CheckCull(ref boundingSphere, motion))
				{
					return;
				}
			}
			else if (SceneRenderData.CurrentRenderData.Camera.CheckCull(ref boundingSphere))
			{
				return;
			}
		}
		SceneRenderData.CurrentRenderData.Sorter.Add(this, motion, 0);
	}

	public override void RenderMaterial(Transform motion, int material)
	{
		Shader shader = PrepareShader(base.shader, motion, materials[material]);
		PrepareRender();
		int num = shader.PassNumber(materials[material]);
		for (int i = 0; i < num; i++)
		{
			shader.ApplyPass(i);
			RenderPrimitives();
		}
		shader.EndRender();
	}

	protected virtual void PrepareRender()
	{
	}

	protected abstract void RenderPrimitives();

	public override void Dispose()
	{
		base.Dispose();
	}
}
