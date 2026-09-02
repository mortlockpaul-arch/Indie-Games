using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Graphics;

namespace RacingGame.Shaders;

public static class VBScreenHelper
{
	private class VBScreen
	{
		private VertexBuffer vbScreen;

		private VertexDeclaration decl;

		public VBScreen()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Expected O, but got Unknown
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Expected O, but got Unknown
			base._002Ector();
			VertexPositionTexture[] array = (VertexPositionTexture[])(object)new VertexPositionTexture[4]
			{
				new VertexPositionTexture(new Vector3(-1f, -1f, 0.5f), new Vector2(0f, 1f)),
				new VertexPositionTexture(new Vector3(-1f, 1f, 0.5f), new Vector2(0f, 0f)),
				new VertexPositionTexture(new Vector3(1f, -1f, 0.5f), new Vector2(1f, 1f)),
				new VertexPositionTexture(new Vector3(1f, 1f, 0.5f), new Vector2(1f, 0f))
			};
			vbScreen = new VertexBuffer(BaseGame.Device, typeof(VertexPositionTexture), array.Length, (BufferUsage)8);
			vbScreen.SetData<VertexPositionTexture>(array);
			decl = new VertexDeclaration(BaseGame.Device, VertexPositionTexture.VertexElements);
		}

		public void Render()
		{
			BaseGame.Device.VertexDeclaration = decl;
			BaseGame.Device.Vertices[0].SetSource(vbScreen, 0, VertexPositionTexture.SizeInBytes);
			BaseGame.Device.DrawPrimitives((PrimitiveType)5, 0, 2);
		}
	}

	private class GridScreen
	{
		private int gridWidth;

		private int gridHeight;

		private IndexBuffer indexBuffer;

		private VertexBuffer vertexBuffer;

		private VertexDeclaration decl;

		public GridScreen(int setGridWidth, int setGridHeight)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Expected O, but got Unknown
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Expected O, but got Unknown
			base._002Ector();
			if (setGridWidth < 2 || setGridHeight < 2)
			{
				throw new ArgumentException("setGridWidth=" + setGridWidth + ", setGridHeight=" + setGridHeight, "Grid size must be at least (2, 2).");
			}
			gridWidth = setGridWidth;
			gridHeight = setGridHeight;
			vertexBuffer = new VertexBuffer(BaseGame.Device, typeof(VertexPositionTexture), gridWidth * gridHeight, (BufferUsage)8);
			VertexPositionTexture[] array = (VertexPositionTexture[])(object)new VertexPositionTexture[gridWidth * gridHeight];
			for (int i = 0; i < gridWidth; i++)
			{
				for (int j = 0; j < gridHeight; j++)
				{
					ref VertexPositionTexture reference = ref array[i + j * gridWidth];
					reference = new VertexPositionTexture(new Vector3(-1f + 2f * (float)i / (float)(gridWidth - 1), -1f + 2f * (float)j / (float)(gridHeight - 1), 0.5f), new Vector2((float)i / (float)(gridWidth - 1), 1f - (float)j / (float)(gridHeight - 1)));
				}
			}
			vertexBuffer.SetData<VertexPositionTexture>(array);
			indexBuffer = new IndexBuffer(BaseGame.Device, typeof(ushort), (gridWidth - 1) * (gridHeight - 1) * 2 * 3, (BufferUsage)8);
			ushort[] array2 = new ushort[(gridWidth - 1) * (gridHeight - 1) * 3 * 2];
			int num = 0;
			for (int k = 0; k < gridWidth - 1; k++)
			{
				for (int l = 0; l < gridHeight - 1; l++)
				{
					ushort num2 = (ushort)(k + l * gridWidth);
					ushort num3 = (ushort)(k + 1 + l * gridWidth);
					ushort num4 = (ushort)(k + 1 + (l + 1) * gridWidth);
					ushort num5 = (ushort)(k + (l + 1) * gridWidth);
					array2[num] = num2;
					array2[num + 1] = num4;
					array2[num + 2] = num3;
					array2[num + 3] = num2;
					array2[num + 4] = num5;
					array2[num + 5] = num4;
					num += 6;
				}
			}
			indexBuffer.SetData<ushort>(array2);
			decl = new VertexDeclaration(BaseGame.Device, VertexPositionTexture.VertexElements);
		}

		public void Render()
		{
			BaseGame.Device.VertexDeclaration = decl;
			BaseGame.Device.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionTexture.SizeInBytes);
			BaseGame.Device.Indices = indexBuffer;
			BaseGame.Device.DrawIndexedPrimitives((PrimitiveType)4, 0, 0, gridWidth * gridHeight, 0, (gridWidth - 1) * (gridHeight - 1) * 2);
		}
	}

	private static VBScreen vbScreenInstance;

	private static GridScreen gridScreen10x10Instance;

	public static void Render()
	{
		if (vbScreenInstance == null)
		{
			vbScreenInstance = new VBScreen();
		}
		vbScreenInstance.Render();
	}

	public static void Render10x10Grid()
	{
		if (gridScreen10x10Instance == null)
		{
			gridScreen10x10Instance = new GridScreen(10, 10);
		}
		gridScreen10x10Instance.Render();
	}
}
