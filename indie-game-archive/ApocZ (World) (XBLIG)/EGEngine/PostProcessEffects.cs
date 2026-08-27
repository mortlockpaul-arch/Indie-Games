using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class PostProcessEffects
{
	private const int SAMPLE_COUNT = 15;

	private const int HALF_SAMPLE_COUNT = 7;

	private static int startVertex = 0;

	private static float TileCountX = 8f;

	private static float TileCountY = 8f;

	private static VS_PostStruct[] postVertices;

	public static VertexBuffer postVertexBuffer;

	public static Texture2D LensDirt;

	public static Texture2D PerlinNoise;

	public static Texture2D AvRSniperReticle;

	public static Texture2D AvRSniperReticleYaw;

	public static Texture2D AvRSniperReticlePitch;

	private Effect BloomEffects;

	private BloomSettings Settings;

	private EffectParameter SampleWeights;

	private EffectParameter SampleOffsets;

	private EffectParameter BloomThreshold;

	private EffectParameter BloomIntensity;

	private EffectParameter BaseIntensity;

	private EffectParameter BloomSaturation;

	private EffectParameter BaseSaturation;

	private EffectParameter BloomTexture;

	private EffectParameter BaseTexture;

	private EffectParameter HDRTexture;

	private EffectParameter GenericTexture;

	private EffectParameter EmissiveTexture;

	private EffectTechnique BloomExtract;

	private EffectTechnique GaussianBlur;

	private EffectTechnique BloomCombine;

	private Vector2 delta = Vector2.Zero;

	private float[] sampleWeightsArray = new float[15];

	private Vector2[] sampleOffsetsArray = new Vector2[15];

	private float Blur0 = 2f;

	private float Thresh0 = 0.25f;

	public static float Intense0 = 1f;

	private float Blur1 = 2f;

	private float Thresh1 = 0.5f;

	public static float Intense1 = 0.5f;

	private float BlurLow0 = 8f;

	private float BlurLow1 = 4f;

	private float BlurLow2 = 2f;

	private float pNoise;

	private float pNoise0;

	private float pNoise1;

	private float pNoiseBlendDirection = 1f;

	public static float pNoiseBlend = 0f;

	private int pNoiseUpdateFlag;

	private Rectangle srcRec = new Rectangle(0, 0, 300, 300);

	private Vector2 offset = new Vector2(150f, 150f);

	private Rectangle dstRec = new Rectangle(640, 360, 640, 640);

	private Viewport postViewPort = default(Viewport);

	private Vector3[] frustumCorners = new Vector3[4];

	private float CurrentBlurAmount = 1f;

	public void Initialize(int postPreSets)
	{
		Settings = BloomSettings.PresetSettings[postPreSets];
		postVertices = new VS_PostStruct[4];
		postVertexBuffer = new VertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, typeof(VS_PostStruct), 4, BufferUsage.None);
		Vector3 pos = new Vector3(-1f, 1f, 0f);
		Vector3 pos2 = new Vector3(1f, 1f, 0f);
		Vector3 pos3 = new Vector3(-1f, -1f, 0f);
		Vector3 pos4 = new Vector3(1f, -1f, 0f);
		ref VS_PostStruct reference = ref postVertices[0];
		reference = new VS_PostStruct(pos, new Vector2(0f, 0f), 0f);
		ref VS_PostStruct reference2 = ref postVertices[1];
		reference2 = new VS_PostStruct(pos2, new Vector2(1f, 0f), 1f);
		ref VS_PostStruct reference3 = ref postVertices[2];
		reference3 = new VS_PostStruct(pos3, new Vector2(0f, 1f), 3f);
		ref VS_PostStruct reference4 = ref postVertices[3];
		reference4 = new VS_PostStruct(pos4, new Vector2(1f, 1f), 2f);
		if (postPreSets == 12)
		{
			Blur0 = 2f;
			Thresh0 = 0.2f;
			Intense0 = 0.5f;
			Blur1 = 2f;
			Thresh1 = 0.7f;
			Intense1 = 0.5f;
			BlurLow0 = 4f;
			BlurLow1 = 2f;
			BlurLow2 = 1f;
		}
		postVertexBuffer.SetData(postVertices);
		BloomEffects = EndGameEngine.ContentMgr.Load<Effect>("shaders\\Bloom");
		SampleWeights = BloomEffects.Parameters["SampleWeights"];
		SampleOffsets = BloomEffects.Parameters["SampleOffsets"];
		BloomThreshold = BloomEffects.Parameters["BloomThreshold"];
		BloomIntensity = BloomEffects.Parameters["BloomIntensity"];
		BaseIntensity = BloomEffects.Parameters["BaseIntensity"];
		BloomSaturation = BloomEffects.Parameters["BloomSaturation"];
		BaseSaturation = BloomEffects.Parameters["BaseSaturation"];
		BloomTexture = BloomEffects.Parameters["BloomTexture"];
		BaseTexture = BloomEffects.Parameters["BaseTexture"];
		HDRTexture = BloomEffects.Parameters["HDRTexture"];
		GenericTexture = BloomEffects.Parameters["GenericTexture"];
		EmissiveTexture = BloomEffects.Parameters["EmissiveTexture"];
		BloomExtract = BloomEffects.Techniques["BloomExtract"];
		GaussianBlur = BloomEffects.Techniques["GaussianBlur"];
		BloomCombine = BloomEffects.Techniques["BloomCombine"];
		PerlinNoise = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\perlinNoise");
		BloomEffects.Parameters["PerlinNoise"].SetValue(PerlinNoise);
		DataEncoder.LoadContent(BloomEffects);
	}

	private void SetBlurEffectParameters(float dx, float dy, float stepNTexel, float texelOffset)
	{
	}

	public void DebugOutput(int qIndex)
	{
		GraphicsDevice graphicsDevice = BloomEffects.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.SetVertexBuffer(postVertexBuffer);
		graphicsDevice.SetRenderTarget(null);
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.Viewport = new Viewport(0, 0, EndGameEngine.GameSettings.BackBufferSizeX, EndGameEngine.GameSettings.BackBufferSizeY);
		HDRTexture.SetValue(LevelBaseMenu.bloomRenderTarget[5]);
		BloomEffects.CurrentTechnique = BloomEffects.Techniques["BlitCopy"];
		BloomEffects.CurrentTechnique.Passes[2].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
	}

	public void Bloom(int qIndex)
	{
		GraphicsDevice graphicsDevice = BloomEffects.GraphicsDevice;
		postViewPort.X = 0;
		postViewPort.Y = 0;
		postViewPort.Width = EndGameEngine.GameSettings.BackBufferSizeX;
		postViewPort.Height = EndGameEngine.GameSettings.BackBufferSizeY;
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = postViewPort;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.SetVertexBuffer(postVertexBuffer);
		BloomEffects.Parameters["BloomThreshold0"].SetValue(Thresh0);
		BloomEffects.Parameters["BloomThreshold1"].SetValue(Thresh1);
		float num = LevelBaseMenu.bloomRenderTarget[0].Width;
		float num2 = LevelBaseMenu.bloomRenderTarget[0].Height;
		BloomEffects.Parameters["blurPixelSize"].SetValue(new Vector2(1f / num, 1f / num2));
		BloomEffects.Parameters["blurStep"].SetValue(Blur0);
		graphicsDevice.SetRenderTarget(LevelBaseMenu.bloomRenderTarget[0]);
		graphicsDevice.Clear(ClearOptions.Target, Color.Black, 1f, 0);
		HDRTexture.SetValue(LevelBaseMenu.compositeRenderTarget);
		BloomEffects.Parameters["vecViewPort"].SetValue(new Vector2(EndGameEngine.GameSettings.BackBufferSizeX, EndGameEngine.GameSettings.BackBufferSizeY));
		BloomEffects.CurrentTechnique = BloomExtract;
		BloomEffects.CurrentTechnique.Passes[0].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
		BloomEffects.Parameters["blurStep"].SetValue(BlurLow2);
		SetBlurEffectParameters(1f / (float)LevelBaseMenu.bloomRenderTarget[0].Width, 0f, Settings.PixelStep, 1.5f);
		graphicsDevice.SetRenderTarget(LevelBaseMenu.bloomRenderTarget[1]);
		BloomTexture.SetValue(LevelBaseMenu.bloomRenderTarget[0]);
		BloomEffects.CurrentTechnique = GaussianBlur;
		BloomEffects.CurrentTechnique.Passes[4].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
		BloomEffects.Parameters["blurStep"].SetValue(BlurLow2);
		SetBlurEffectParameters(0f, 1f / (float)LevelBaseMenu.bloomRenderTarget[1].Height, Settings.PixelStep, 1.5f);
		graphicsDevice.SetRenderTarget(LevelBaseMenu.bloomRenderTarget[0]);
		BloomTexture.SetValue(LevelBaseMenu.bloomRenderTarget[1]);
		BloomEffects.CurrentTechnique = GaussianBlur;
		BloomEffects.CurrentTechnique.Passes[5].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
		graphicsDevice.SetRenderTarget(null);
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.None;
		EndGameEngine.DefualtViewport.Width = EndGameEngine.GameSettings.RenderTargetSizeX;
		EndGameEngine.DefualtViewport.Height = EndGameEngine.GameSettings.RenderTargetSizeY;
		graphicsDevice.Viewport = EndGameEngine.DefualtViewport;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.SetVertexBuffer(postVertexBuffer);
		HDRTexture.SetValue(LevelBaseMenu.compositeRenderTarget);
		BloomTexture.SetValue(LevelBaseMenu.bloomRenderTarget[0]);
		float num3 = 0.6f;
		float num4 = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].BloodLevel / 60f;
		num4 = ((num4 > num3) ? num3 : ((num4 < 0f) ? 0f : num4));
		BaseSaturation.SetValue(num4 - pNoiseBlend);
		BloomSaturation.SetValue(1.15f);
		pNoise = (float)EndGameEngine.randGenerator.NextDouble();
		BloomEffects.Parameters["pNoise"].SetValue(pNoise);
		BloomEffects.Parameters["pNoiseBlend"].SetValue(pNoiseBlend);
		BloomEffects.Parameters["vecViewPort"].SetValue(new Vector2(EndGameEngine.GameSettings.BackBufferSizeX, EndGameEngine.GameSettings.BackBufferSizeY));
		BloomEffects.CurrentTechnique = BloomCombine;
		BloomEffects.CurrentTechnique.Passes[9].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
	}

	public void BlitTexture(Texture2D src)
	{
		GraphicsDevice graphicsDevice = BloomEffects.GraphicsDevice;
		EndGameEngine.DefualtViewport.Width = EndGameEngine.GameSettings.BackBufferSizeX;
		EndGameEngine.DefualtViewport.Height = EndGameEngine.GameSettings.BackBufferSizeY;
		graphicsDevice.Viewport = EndGameEngine.DefualtViewport;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.None;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.SetVertexBuffer(postVertexBuffer);
		HDRTexture.SetValue(src);
		BloomEffects.CurrentTechnique = BloomEffects.Techniques["BlitCopy"];
		BloomEffects.CurrentTechnique.Passes[4].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
	}

	public void EdgeFilter(int qIndex)
	{
		GraphicsDevice graphicsDevice = BloomEffects.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.SetVertexBuffer(postVertexBuffer);
		graphicsDevice.SetRenderTarget(LevelBaseMenu.DiffuseRenderTarget);
		graphicsDevice.Viewport = new Viewport(0, 0, EndGameEngine.GameSettings.GBufferSizeX, EndGameEngine.GameSettings.GBufferSizeY);
		graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.Blue, 1f, 0);
		BaseTexture.SetValue(LevelBaseMenu.bloomRenderTarget[0]);
		BloomEffects.CurrentTechnique = BloomEffects.Techniques["BlitCopy"];
		BloomEffects.CurrentTechnique.Passes[1].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
		BaseTexture.SetValue(LevelBaseMenu.bloomRenderTarget[0]);
		BloomEffects.Parameters["NormalTexture"].SetValue(LevelBaseMenu.bloomRenderTarget[0]);
		BloomEffects.Parameters["DepthTexture"].SetValue(LevelBaseMenu.DepthRenderTarget);
		BloomEffects.CurrentTechnique = BloomEffects.Techniques["BlitCopy"];
		BloomEffects.CurrentTechnique.Passes[5].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
	}

	public void Particles(PlayerBase playerRef, int qIndex)
	{
		_ = BloomEffects.GraphicsDevice;
		EndGameEngine.MaterialParams.fBloomIntensity.SetValue(Settings.BloomIntensity);
		EndGameEngine.MaterialParams.fBaseIntensity.SetValue(Settings.BaseIntensity);
		EndGameEngine.MaterialParams.fBloomSaturation.SetValue(Settings.BloomSaturation);
		EndGameEngine.MaterialParams.fBaseSaturation.SetValue(Settings.BaseSaturation);
		playerRef.SetViewPortTestCoOp(PlayerBase.RenderPass.ForwardPass, qIndex);
		LevelBaseMenu.Particles.Draw(playerRef, 5, qIndex);
	}

	private float ComputeGaussian(float n)
	{
		float currentBlurAmount = CurrentBlurAmount;
		return (float)(1.0 / Math.Sqrt(Math.PI * 2.0 * (double)currentBlurAmount) * Math.Exp((0f - n * n) / (2f * currentBlurAmount * currentBlurAmount)));
	}

	public void BlurShadowMap(RenderTarget2D rt0, RenderTarget2D rt1)
	{
		GraphicsDevice graphicsDevice = BloomEffects.GraphicsDevice;
		postViewPort.X = 0;
		postViewPort.Y = 0;
		postViewPort.Width = rt0.Width;
		postViewPort.Height = rt0.Height;
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = postViewPort;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.SetVertexBuffer(postVertexBuffer);
		float num = rt0.Width;
		float num2 = rt0.Height;
		BloomEffects.Parameters["blurPixelSize"].SetValue(new Vector2(1f / num, 1f / num2));
		BloomEffects.Parameters["blurStep"].SetValue(BlurLow0);
		SetBlurEffectParameters(1f / (float)LevelBaseMenu.shadowRenderTarget2[2].Width, 0f, Settings.PixelStep, 1.5f);
		graphicsDevice.SetRenderTarget(rt1);
		BloomTexture.SetValue(rt0);
		Vector2 value = new Vector2(rt1.Width, rt1.Height);
		BloomEffects.Parameters["vecViewPort"].SetValue(value);
		BloomEffects.CurrentTechnique = GaussianBlur;
		BloomEffects.CurrentTechnique.Passes[9].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
	}

	public void BlurSSAOMap(RenderTarget2D rt0, RenderTarget2D rt1)
	{
		GraphicsDevice graphicsDevice = BloomEffects.GraphicsDevice;
		postViewPort.X = 0;
		postViewPort.Y = 0;
		postViewPort.Width = rt0.Width;
		postViewPort.Height = rt0.Height;
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = postViewPort;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.SetVertexBuffer(postVertexBuffer);
		float num = rt0.Width;
		float num2 = rt0.Height;
		BloomEffects.Parameters["blurPixelSize"].SetValue(new Vector2(1f / num, 1f / num2));
		BloomEffects.Parameters["blurStep"].SetValue(BlurLow0);
		SetBlurEffectParameters(1f / (float)LevelBaseMenu.shadowRenderTarget2[2].Width, 0f, Settings.PixelStep, 1.5f);
		graphicsDevice.SetRenderTarget(rt1);
		BloomTexture.SetValue(rt0);
		Vector2 value = new Vector2(rt1.Width, rt1.Height);
		BloomEffects.Parameters["vecViewPort"].SetValue(value);
		BloomEffects.CurrentTechnique = GaussianBlur;
		BloomEffects.CurrentTechnique.Passes[6].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
	}

	public void FinalizeTexture(int qIndex, Texture2D t)
	{
		GraphicsDevice graphicsDevice = BloomEffects.GraphicsDevice;
		graphicsDevice.SetRenderTarget(null);
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.None;
		EndGameEngine.DefualtViewport.Width = EndGameEngine.GameSettings.RenderTargetSizeX;
		EndGameEngine.DefualtViewport.Height = EndGameEngine.GameSettings.RenderTargetSizeY;
		graphicsDevice.Viewport = EndGameEngine.DefualtViewport;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		graphicsDevice.SetVertexBuffer(postVertexBuffer);
		BloomTexture.SetValue(t);
		BloomEffects.Parameters["vecViewPort"].SetValue(new Vector2(EndGameEngine.GameSettings.BackBufferSizeX, EndGameEngine.GameSettings.BackBufferSizeY));
		BloomEffects.CurrentTechnique = BloomCombine;
		BloomEffects.CurrentTechnique.Passes[7].Apply();
		graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
	}
}
