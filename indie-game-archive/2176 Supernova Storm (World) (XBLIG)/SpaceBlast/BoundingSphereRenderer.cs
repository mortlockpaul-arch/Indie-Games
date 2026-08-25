using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast;

public static class BoundingSphereRenderer
{
	private static VertexBuffer vertBuffer;

	private static VertexDeclaration vertDecl;

	private static BasicEffect effect;

	private static int sphereResolution;

	public static void InitializeGraphics(GraphicsDevice graphicsDevice, int sphereResolution)
	{
		BoundingSphereRenderer.sphereResolution = sphereResolution;
		vertDecl = new VertexDeclaration(graphicsDevice, VertexPositionColor.VertexElements);
		effect = new BasicEffect(graphicsDevice, null);
		effect.LightingEnabled = false;
		effect.VertexColorEnabled = false;
		VertexPositionColor[] array = new VertexPositionColor[(sphereResolution + 1) * 3];
		int num = 0;
		float num2 = (float)Math.PI * 2f / (float)sphereResolution;
		for (float num3 = 0f; num3 <= (float)Math.PI * 2f; num3 += num2)
		{
			ref VertexPositionColor reference = ref array[num++];
			reference = new VertexPositionColor(new Vector3((float)Math.Cos(num3), (float)Math.Sin(num3), 0f), Color.White);
		}
		for (float num4 = 0f; num4 <= (float)Math.PI * 2f; num4 += num2)
		{
			ref VertexPositionColor reference2 = ref array[num++];
			reference2 = new VertexPositionColor(new Vector3((float)Math.Cos(num4), 0f, (float)Math.Sin(num4)), Color.White);
		}
		for (float num5 = 0f; num5 <= (float)Math.PI * 2f; num5 += num2)
		{
			ref VertexPositionColor reference3 = ref array[num++];
			reference3 = new VertexPositionColor(new Vector3(0f, (float)Math.Cos(num5), (float)Math.Sin(num5)), Color.White);
		}
		vertBuffer = new VertexBuffer(graphicsDevice, array.Length * VertexPositionColor.SizeInBytes, BufferUsage.None);
		vertBuffer.SetData(array);
	}

	public static void Render(BoundingSphere sphere, GraphicsDevice graphicsDevice, Matrix view, Matrix projection, Color xyColor, Color xzColor, Color yzColor)
	{
		if (vertBuffer == null)
		{
			InitializeGraphics(graphicsDevice, 30);
		}
		graphicsDevice.VertexDeclaration = vertDecl;
		graphicsDevice.Vertices[0].SetSource(vertBuffer, 0, VertexPositionColor.SizeInBytes);
		effect.World = Matrix.CreateScale(sphere.Radius) * Matrix.CreateTranslation(sphere.Center);
		effect.View = view;
		effect.Projection = projection;
		effect.DiffuseColor = xyColor.ToVector3();
		effect.Begin();
		foreach (EffectPass pass in effect.CurrentTechnique.Passes)
		{
			pass.Begin();
			graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, sphereResolution);
			if (xzColor.A != 0)
			{
				effect.DiffuseColor = xzColor.ToVector3();
				effect.CommitChanges();
				graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, sphereResolution + 1, sphereResolution);
			}
			if (yzColor.A != 0)
			{
				effect.DiffuseColor = yzColor.ToVector3();
				effect.CommitChanges();
				graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, (sphereResolution + 1) * 2, sphereResolution);
			}
			pass.End();
		}
		effect.End();
	}

	public static void Render(BoundingSphere sphere, GraphicsDevice graphicsDevice, Matrix view, Matrix projection, Color color)
	{
		if (vertBuffer == null)
		{
			InitializeGraphics(graphicsDevice, 30);
		}
		graphicsDevice.VertexDeclaration = vertDecl;
		graphicsDevice.Vertices[0].SetSource(vertBuffer, 0, VertexPositionColor.SizeInBytes);
		effect.World = Matrix.CreateScale(sphere.Radius) * Matrix.CreateTranslation(sphere.Center);
		effect.View = view;
		effect.Projection = projection;
		effect.DiffuseColor = color.ToVector3();
		effect.Begin();
		foreach (EffectPass pass in effect.CurrentTechnique.Passes)
		{
			pass.Begin();
			graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, 0, sphereResolution);
			graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, sphereResolution + 1, sphereResolution);
			graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, (sphereResolution + 1) * 2, sphereResolution);
			pass.End();
		}
		effect.End();
	}
}
