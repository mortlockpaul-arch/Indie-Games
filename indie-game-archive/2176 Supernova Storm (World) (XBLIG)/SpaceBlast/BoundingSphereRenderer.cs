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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		BoundingSphereRenderer.sphereResolution = sphereResolution;
		vertDecl = new VertexDeclaration(graphicsDevice, VertexPositionColor.VertexElements);
		effect = new BasicEffect(graphicsDevice, (EffectPool)null);
		effect.LightingEnabled = false;
		effect.VertexColorEnabled = false;
		VertexPositionColor[] array = (VertexPositionColor[])(object)new VertexPositionColor[(sphereResolution + 1) * 3];
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
		vertBuffer = new VertexBuffer(graphicsDevice, array.Length * VertexPositionColor.SizeInBytes, (BufferUsage)0);
		vertBuffer.SetData<VertexPositionColor>(array);
	}

	public static void Render(BoundingSphere sphere, GraphicsDevice graphicsDevice, Matrix view, Matrix projection, Color xyColor, Color xzColor, Color yzColor)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if (vertBuffer == null)
		{
			InitializeGraphics(graphicsDevice, 30);
		}
		graphicsDevice.VertexDeclaration = vertDecl;
		graphicsDevice.Vertices[0].SetSource(vertBuffer, 0, VertexPositionColor.SizeInBytes);
		effect.World = Matrix.CreateScale(sphere.Radius) * Matrix.CreateTranslation(sphere.Center);
		effect.View = view;
		effect.Projection = projection;
		effect.DiffuseColor = ((Color)(ref xyColor)).ToVector3();
		((Effect)effect).Begin();
		foreach (EffectPass pass in ((Effect)effect).CurrentTechnique.Passes)
		{
			pass.Begin();
			graphicsDevice.DrawPrimitives((PrimitiveType)3, 0, sphereResolution);
			if (((Color)(ref xzColor)).A != 0)
			{
				effect.DiffuseColor = ((Color)(ref xzColor)).ToVector3();
				((Effect)effect).CommitChanges();
				graphicsDevice.DrawPrimitives((PrimitiveType)3, sphereResolution + 1, sphereResolution);
			}
			if (((Color)(ref yzColor)).A != 0)
			{
				effect.DiffuseColor = ((Color)(ref yzColor)).ToVector3();
				((Effect)effect).CommitChanges();
				graphicsDevice.DrawPrimitives((PrimitiveType)3, (sphereResolution + 1) * 2, sphereResolution);
			}
			pass.End();
		}
		((Effect)effect).End();
	}

	public static void Render(BoundingSphere sphere, GraphicsDevice graphicsDevice, Matrix view, Matrix projection, Color color)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (vertBuffer == null)
		{
			InitializeGraphics(graphicsDevice, 30);
		}
		graphicsDevice.VertexDeclaration = vertDecl;
		graphicsDevice.Vertices[0].SetSource(vertBuffer, 0, VertexPositionColor.SizeInBytes);
		effect.World = Matrix.CreateScale(sphere.Radius) * Matrix.CreateTranslation(sphere.Center);
		effect.View = view;
		effect.Projection = projection;
		effect.DiffuseColor = ((Color)(ref color)).ToVector3();
		((Effect)effect).Begin();
		foreach (EffectPass pass in ((Effect)effect).CurrentTechnique.Passes)
		{
			pass.Begin();
			graphicsDevice.DrawPrimitives((PrimitiveType)3, 0, sphereResolution);
			graphicsDevice.DrawPrimitives((PrimitiveType)3, sphereResolution + 1, sphereResolution);
			graphicsDevice.DrawPrimitives((PrimitiveType)3, (sphereResolution + 1) * 2, sphereResolution);
			pass.End();
		}
		((Effect)effect).End();
	}
}
