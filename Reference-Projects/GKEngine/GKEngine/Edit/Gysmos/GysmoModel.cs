using System.Collections.Generic;
using GKEngine.Cameras;
using GKEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Edit.Gysmos;

public class GysmoModel
{
	public List<GysmoModelPart> modelParts = new List<GysmoModelPart>();

	public Gysmo parent;

	internal GysmoModel(ContentReader input)
	{
		int num = input.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			GysmoModelPart modelPart = new GysmoModelPart();
			modelPart.name = input.ReadString();
			modelPart.triangleCount = input.ReadInt32();
			modelPart.vertexCount = input.ReadInt32();
			modelPart.vertexStride = input.ReadInt32();
			modelPart.vertexDeclaration = input.ReadObject<VertexDeclaration>();
			modelPart.vertexBuffer = input.ReadObject<VertexBuffer>();
			modelPart.indexBuffer = input.ReadObject<IndexBuffer>();
			modelPart.transform = input.ReadObject<Matrix>();
			input.ReadSharedResource(delegate(Effect value)
			{
				modelPart.effect = value;
			});
			MeshData meshData = new MeshData();
			int num2 = input.ReadInt32();
			for (int num3 = 0; num3 < num2; num3++)
			{
				Vector3 xVectorA = input.ReadObject<Vector3>();
				Vector3 xVectorB = input.ReadObject<Vector3>();
				Vector3 xVectorC = input.ReadObject<Vector3>();
				Vector3 xNormal = input.ReadObject<Vector3>();
				meshData.triangles.Add(new Poly(xVectorA, xVectorB, xVectorC, xNormal));
			}
			modelPart.collision = meshData;
			modelPart.model = this;
			modelParts.Add(modelPart);
		}
	}

	public void Render(Matrix world, Matrix view, Matrix projection)
	{
		modelParts.Sort(Compare_ModelPart_Depth);
		for (int i = 0; i < modelParts.Count; i++)
		{
			GysmoModelPart gysmoModelPart = modelParts[i];
			BasicEffect basicEffect = (BasicEffect)gysmoModelPart.effect;
			if (basicEffect.IsDisposed)
			{
				continue;
			}
			basicEffect.EnableDefaultLighting();
			basicEffect.PreferPerPixelLighting = true;
			basicEffect.World = world;
			basicEffect.View = view;
			basicEffect.Projection = projection;
			GraphicsDevice graphicsDevice = GameEngine.Graphics.GraphicsDevice;
			graphicsDevice.DepthStencilState.DepthBufferEnable = false;
			graphicsDevice.SetVertexBuffer(gysmoModelPart.vertexBuffer);
			graphicsDevice.Indices = gysmoModelPart.indexBuffer;
			foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, gysmoModelPart.vertexCount, 0, gysmoModelPart.triangleCount);
			}
			graphicsDevice.DepthStencilState.DepthBufferEnable = true;
		}
	}

	public void Dispose()
	{
		for (int i = 0; i < modelParts.Count; i++)
		{
			modelParts[i].Dispose();
		}
		modelParts.Clear();
	}

	public void Flush()
	{
		for (int i = 0; i < modelParts.Count; i++)
		{
			modelParts[i].Flush();
		}
	}

	public int Compare_ModelPart_Depth(GysmoModelPart oEnt1, GysmoModelPart oEnt2)
	{
		Camera camera = parent.scene.cameras.camera;
		if (oEnt1 == null)
		{
			if (oEnt2 == null)
			{
				return 0;
			}
			return -1;
		}
		if (oEnt2 == null)
		{
			return 1;
		}
		Base3D base3D = new Base3D(oEnt1.transform * oEnt1.model.parent.matrix);
		Base3D base3D2 = new Base3D(oEnt2.transform * oEnt2.model.parent.matrix);
		float value = Vector3.Distance(camera.position, base3D.position);
		return Vector3.Distance(camera.position, base3D2.position).CompareTo(value);
	}
}
