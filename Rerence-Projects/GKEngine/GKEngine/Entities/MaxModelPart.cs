using GKEngine.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Entities;

public class MaxModelPart
{
	private int _i;

	public string name;

	public int triangleCount;

	public string materialData;

	public VertexBuffer vertexBuffer;

	public IndexBuffer indexBuffer;

	public Vector3 min;

	public Vector3 max;

	public float radius;

	public Vector3 center;

	public Material material;

	public MaxModel model;

	private EffectPass pass;

	public Matrix local = Matrix.Identity;

	public bool hasLocal;

	public bool built;

	public bool visible = true;

	public MaxModelPart()
	{
	}

	public MaxModelPart(string xName, int xTriangleCount, string xMaterialData, Vector3 xMin, Vector3 xMax, float xRadius, Vector3 xCenter, VertexBuffer xVertexBuffer, IndexBuffer xIndexBuffer)
	{
		name = xName;
		triangleCount = xTriangleCount;
		materialData = xMaterialData;
		min = xMin;
		max = xMax;
		center = xCenter;
		radius = xRadius;
		vertexBuffer = xVertexBuffer;
		indexBuffer = xIndexBuffer;
	}

	public virtual void Build(MaxModel oMaxModel)
	{
		model = oMaxModel;
		Build();
	}

	public virtual void Build()
	{
		built = false;
		material = new Material(materialData);
		built = true;
	}

	public MaxModelPart Clone()
	{
		MaxModelPart maxModelPart = new MaxModelPart();
		maxModelPart.name = name;
		maxModelPart.triangleCount = triangleCount;
		maxModelPart.materialData = materialData;
		maxModelPart.min = min;
		maxModelPart.max = max;
		maxModelPart.radius = radius;
		maxModelPart.center = center;
		maxModelPart.vertexBuffer = vertexBuffer;
		maxModelPart.indexBuffer = indexBuffer;
		return maxModelPart;
	}

	public void Render(ref Matrix world, Camera camera)
	{
		if (built && visible)
		{
			GraphicsDevice graphicsDevice = GameEngine.instance.GraphicsDevice;
			if (hasLocal)
			{
				material.Set(Matrix.Multiply(local, world), camera);
			}
			else
			{
				material.Set(world, camera);
			}
			Effect effect = material.effect;
			if (model.animation != null && material.useBones)
			{
				effect.Parameters["Bones"].SetValue(model.animation.skinTransforms);
			}
			graphicsDevice.SetVertexBuffer(vertexBuffer);
			graphicsDevice.Indices = indexBuffer;
			for (_i = 0; _i < material.effectPassCount; _i++)
			{
				pass = effect.CurrentTechnique.Passes[_i];
				pass.Apply();
				graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, triangleCount);
			}
		}
	}

	public void RenderManual()
	{
		GraphicsDevice graphicsDevice = GameEngine.instance.GraphicsDevice;
		Effect effect = material.effect;
		graphicsDevice.SetVertexBuffer(vertexBuffer);
		graphicsDevice.Indices = indexBuffer;
		EffectPass effectPass = effect.CurrentTechnique.Passes[0];
		effectPass.Apply();
		graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, triangleCount);
	}

	public virtual void Dispose()
	{
		if (material != null)
		{
			material.Dispose();
		}
		_ = vertexBuffer;
		_ = indexBuffer;
		vertexBuffer = null;
		indexBuffer = null;
		pass = null;
		material = null;
	}

	public static MaxModelPart Read(ref ContentReader input)
	{
		MaxModelPart maxModelPart = new MaxModelPart();
		maxModelPart.name = input.ReadString();
		maxModelPart.triangleCount = input.ReadInt32();
		maxModelPart.materialData = input.ReadString();
		maxModelPart.min = input.ReadObject<Vector3>();
		maxModelPart.max = input.ReadObject<Vector3>();
		maxModelPart.radius = input.ReadSingle();
		maxModelPart.center = input.ReadObject<Vector3>();
		maxModelPart.vertexBuffer = input.ReadObject<VertexBuffer>();
		maxModelPart.indexBuffer = input.ReadObject<IndexBuffer>();
		return maxModelPart;
	}
}
