using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class DataEncoder
{
	private const int MinimumSwapValue = 13;

	private static VS_PostStruct[] frameVertices;

	private static VertexBuffer frameVertexBuffer;

	private static Effect DataProcessor;

	private static EffectTechnique InputData;

	private static EffectParameter InputDataKey;

	private static EffectParameter InputDataLength;

	private static EffectParameter InputDataBufferHndl;

	private static EffectTechnique DataEncoderTech;

	private static int[] BufferKey = new int[128];

	private static Texture2D InputDataBuffer;

	private static RenderTarget2D DataResult;

	private static byte[] TempBuffer = new byte[4096];

	public static byte[] DataBuffer = new byte[4096];

	public static bool DataBufferIsLoaded = false;

	private static bool savedatabusy = false;

	private static bool loaddatabusy = false;

	public static bool IsBusySave_Wait
	{
		get
		{
			while (savedatabusy)
			{
			}
			return true;
		}
		set
		{
			savedatabusy = value;
		}
	}

	public static bool IsBusyLoad_Wait
	{
		get
		{
			while (loaddatabusy)
			{
			}
			return true;
		}
		set
		{
			loaddatabusy = value;
		}
	}

	public static void LoadContent(Effect e)
	{
		frameVertices = new VS_PostStruct[4];
		frameVertexBuffer = new VertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, typeof(VS_PostStruct), 4, BufferUsage.None);
		Vector3 pos = new Vector3(-1f, 1f, 0f);
		Vector3 pos2 = new Vector3(1f, 1f, 0f);
		Vector3 pos3 = new Vector3(-1f, -1f, 0f);
		Vector3 pos4 = new Vector3(1f, -1f, 0f);
		ref VS_PostStruct reference = ref frameVertices[0];
		reference = new VS_PostStruct(pos, new Vector2(0f, 0f), 0f);
		ref VS_PostStruct reference2 = ref frameVertices[1];
		reference2 = new VS_PostStruct(pos2, new Vector2(1f, 0f), 1f);
		ref VS_PostStruct reference3 = ref frameVertices[2];
		reference3 = new VS_PostStruct(pos3, new Vector2(0f, 1f), 3f);
		ref VS_PostStruct reference4 = ref frameVertices[3];
		reference4 = new VS_PostStruct(pos4, new Vector2(1f, 1f), 2f);
		frameVertexBuffer.SetData(frameVertices);
		DataResult = new RenderTarget2D(EndGameEngine.GraphicMgr.GraphicsDevice, 32, 32, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		DataProcessor = e;
		InputDataKey = DataProcessor.Parameters["InputDataKey"];
		InputDataLength = DataProcessor.Parameters["InputDataLength"];
		InputDataBufferHndl = DataProcessor.Parameters["InputDataBuffer"];
		DataEncoderTech = DataProcessor.Techniques["DataEncoder"];
	}

	public static void SaveData(Stream sWriter)
	{
		try
		{
			if (IsBusyLoad_Wait)
			{
				IsBusySave_Wait = true;
			}
			GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
			Viewport viewport = new Viewport(0, 0, 32, 32);
			graphicsDevice.Viewport = viewport;
			graphicsDevice.SetRenderTarget(DataResult);
			graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1f, 0);
			graphicsDevice.BlendState = BlendState.Opaque;
			graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
			graphicsDevice.DepthStencilState = DepthStencilState.Default;
			graphicsDevice.SetVertexBuffer(PostProcessEffects.postVertexBuffer);
			for (int i = 0; i < 128; i++)
			{
				BufferKey[i] = EndGameEngine.randGenerator.Next(0, 256);
			}
			InputDataBuffer = new Texture2D(EndGameEngine.GraphicMgr.GraphicsDevice, 32, 32, mipMap: false, SurfaceFormat.Color);
			InputDataBuffer.SetData(DataBuffer);
			InputDataKey.SetValue(BufferKey);
			InputDataBufferHndl.SetValue(InputDataBuffer);
			DataProcessor.CurrentTechnique = DataEncoderTech;
			DataProcessor.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
			graphicsDevice.SetRenderTarget(null);
			DataResult.GetData(TempBuffer);
			sWriter.Write(TempBuffer, 0, TempBuffer.Length);
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
		IsBusySave_Wait = false;
	}

	public static void LoadData(Stream sReader)
	{
		try
		{
			if (IsBusySave_Wait)
			{
				IsBusyLoad_Wait = true;
			}
			DataBufferIsLoaded = false;
			sReader.Read(TempBuffer, 0, TempBuffer.Length);
			GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
			Viewport viewport = new Viewport(0, 0, 32, 32);
			graphicsDevice.Viewport = viewport;
			graphicsDevice.SetRenderTarget(DataResult);
			graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1f, 0);
			graphicsDevice.BlendState = BlendState.Opaque;
			graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
			graphicsDevice.DepthStencilState = DepthStencilState.Default;
			graphicsDevice.SetVertexBuffer(PostProcessEffects.postVertexBuffer);
			InputDataBuffer = new Texture2D(EndGameEngine.GraphicMgr.GraphicsDevice, 32, 32, mipMap: false, SurfaceFormat.Color);
			InputDataBuffer.SetData(TempBuffer);
			InputDataBufferHndl.SetValue(InputDataBuffer);
			DataProcessor.CurrentTechnique = DataEncoderTech;
			DataProcessor.CurrentTechnique.Passes[1].Apply();
			graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
			graphicsDevice.SetRenderTarget(null);
			DataResult.GetData(DataBuffer);
			DataBufferIsLoaded = true;
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
		IsBusyLoad_Wait = false;
	}
}
