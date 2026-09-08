using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Items;
using Quasar.Scenes;
using Quasar.Shaders;

namespace Quasar.Render.Processes;

public class HDRProcess : RenderProcess
{
	private const float MiddleGray = 0.5f;

	private static int TONE_MAPS = 4;

	private bool evenFrame;

	private RenderPass2D HDRPass;

	private RenderPass2D HDRScaledPass;

	private RenderPass2D BrightPass;

	private RenderPass2D AdaptLumaPass;

	private Postprocess AdaptPP;

	private Postprocess BrightPP;

	private Postprocess FinalPP;

	private RenderPass2D GaussianBlurPass;

	private RenderPass2D BloomPass1;

	private RenderPass2D BloomPass2;

	private RenderTarget2D AdaptationRT1;

	private RenderTarget2D AdaptationRT2;

	private List<RenderPass2D> LumaPasses = new List<RenderPass2D>(1);

	public HDRProcess()
	{
		initTargets();
	}

	protected Vector4[] GetBloomOffsets_Width(int Size)
	{
		List<float> bloomOffsets = GetBloomOffsets(Size);
		Vector4[] array = new Vector4[15];
		for (int i = 0; i < 15; i++)
		{
			ref Vector4 reference = ref array[i];
			reference = new Vector4(bloomOffsets[i], 0f, 0f, 0f);
		}
		return array;
	}

	protected Vector4[] GetBloomOffsets_Height(int Size)
	{
		List<float> bloomOffsets = GetBloomOffsets(Size);
		Vector4[] array = new Vector4[15];
		for (int i = 0; i < 15; i++)
		{
			ref Vector4 reference = ref array[i];
			reference = new Vector4(0f, bloomOffsets[i], 0f, 0f);
		}
		return array;
	}

	protected List<float> GetBloomOffsets(int Size)
	{
		List<float> list = new List<float>(15);
		int num = 0;
		float num2 = 1f / (float)Size;
		list.Add(0f);
		for (num = 1; num < 8; num++)
		{
			list.Add((float)num * num2);
		}
		for (num = 8; num < 15; num++)
		{
			list.Add(0f - list[num - 7]);
		}
		return list;
	}

	protected Vector4[] GetBloomWeights(float deviation, float multiplier)
	{
		Vector4[] array = new Vector4[15];
		int num = 0;
		float num2 = multiplier * GaussianDistribution(0f, 0f, deviation);
		ref Vector4 reference = ref array[num];
		reference = new Vector4(num2, num2, num2, 1f);
		for (num = 1; num < 8; num++)
		{
			num2 = multiplier * GaussianDistribution(num, 0f, deviation);
			ref Vector4 reference2 = ref array[num];
			reference2 = new Vector4(num2, num2, num2, 1f);
		}
		for (num = 8; num < 15; num++)
		{
			ref Vector4 reference3 = ref array[num];
			reference3 = array[num - 7];
		}
		return array;
	}

	protected Vector4[] GetGaussBlur5x5Offsets(int Width, int Height)
	{
		Vector4[] array = new Vector4[13];
		float num = 1f / (float)Width;
		float num2 = 1f / (float)Height;
		int num3 = 0;
		for (int i = -2; i <= 2; i++)
		{
			for (int j = -2; j <= 2; j++)
			{
				if (Math.Abs(i) + Math.Abs(j) <= 2)
				{
					ref Vector4 reference = ref array[num3++];
					reference = new Vector4((float)i * num, (float)j * num2, 0f, 0f);
				}
			}
		}
		return array;
	}

	protected float GaussianDistribution(float x, float y, float rho)
	{
		float num = 1f / (float)Math.Sqrt((float)Math.PI * 2f * rho * rho);
		return num * (float)Math.Exp((0f - (x * x + y * y)) / (2f * rho * rho));
	}

	protected Vector4[] GetGaussBlur5x5Weights(float fMultiplier)
	{
		Vector4[] array = new Vector4[13];
		Vector4 vector = new Vector4(1f, 1f, 1f, 1f);
		float num = 0f;
		int num2 = 0;
		for (int i = -2; i <= 2; i++)
		{
			for (int j = -2; j <= 2; j++)
			{
				if (Math.Abs(i) + Math.Abs(j) <= 2)
				{
					ref Vector4 reference = ref array[num2];
					reference = vector * GaussianDistribution(i, j, 1f);
					num += array[num2].X;
					num2++;
				}
			}
		}
		for (int k = 0; k < num2; k++)
		{
			array[k] /= num;
			array[k] *= fMultiplier;
		}
		return array;
	}

	protected Vector4[] GetLumaInitialPassOffsets(int Width, int Height)
	{
		Vector4[] array = new Vector4[9];
		float num = 1f / (3f * (float)Width);
		float num2 = 1f / (3f * (float)Height);
		int num3 = 0;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				ref Vector4 reference = ref array[num3];
				reference = new Vector4((float)i * num, (float)j * num2, 0f, 0f);
				num3++;
			}
		}
		return array;
	}

	protected Vector4[] GetLumaDownscalePassOffsets(int Width, int Height)
	{
		Vector4[] array = new Vector4[16];
		float num = 1f / (float)Width;
		float num2 = 1f / (float)Height;
		int num3 = 0;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				ref Vector4 reference = ref array[num3];
				reference = new Vector4(((float)j - 1.5f) * num, ((float)i - 1.5f) * num2, 0f, 0f);
				num3++;
			}
		}
		return array;
	}

	protected void initTargets()
	{
		int num = Engine.BackBufferWidth - Engine.BackBufferWidth % 8;
		int num2 = Engine.BackBufferHeight - Engine.BackBufferHeight % 8;
		RenderTarget2D renderTarget2D = new RenderTarget2D(Engine.Device, num, num2, mipMap: false, SurfaceFormat.HdrBlendable, DepthFormat.Depth24);
		HDRPass = new RenderPass2D(createRenderTarget: false);
		HDRPass.SetRenderTarget(renderTarget2D, setOwner: true);
		HDRPass.MustClearColor = true;
		HDRPass.MustClearDepth = true;
		addIntermediatePass(HDRPass);
		RenderTarget2D source = renderTarget2D;
		int num3 = Math.Max(num / 4, num2 / 4);
		int i;
		for (i = 0; Math.Pow(2.0, i) <= (double)num3; i++)
		{
		}
		num3 = (int)Math.Pow(2.0, i - 1);
		renderTarget2D = new RenderTarget2D(Engine.Device, num3, num3, mipMap: false, SurfaceFormat.HdrBlendable, DepthFormat.None);
		HDRScaledPass = new RenderPass2D(createRenderTarget: false);
		HDRScaledPass.SetRenderTarget(renderTarget2D, setOwner: true);
		HDRScaledPass.MustClearColor = false;
		HDRScaledPass.MustClearDepth = false;
		Scene scene = new PostprocessScene();
		Postprocess e = new Postprocess(source, ShaderManager.Shaders["PassThrough"]);
		scene.Add(e);
		HDRScaledPass.addSource(scene);
		addIntermediatePass(HDRScaledPass);
		int j = 0;
		int num4 = (int)Math.Round(Math.Pow(4.0, TONE_MAPS - 1 - j));
		for (; j < TONE_MAPS; j++)
		{
			source = renderTarget2D;
			int num5 = num4;
			num4 = (int)Math.Round(Math.Pow(4.0, TONE_MAPS - 1 - j));
			renderTarget2D = new RenderTarget2D(Engine.Device, num4, num4, mipMap: false, SurfaceFormat.HalfSingle, DepthFormat.None);
			RenderPass2D renderPass2D = new RenderPass2D(createRenderTarget: false);
			renderPass2D.SetRenderTarget(renderTarget2D, setOwner: true);
			renderPass2D.MustClearColor = false;
			renderPass2D.MustClearDepth = false;
			Scene scene2 = new PostprocessScene();
			e = new Postprocess(source, ShaderManager.Shaders["HDR"]);
			if (j == 0)
			{
				e.Meshes[0].Materials[0].Technique = "SampleAvgLum";
				e.Meshes[0].Materials[0].AddVector4ArrayParameter(GetLumaInitialPassOffsets(num4, num4));
			}
			else if (j == TONE_MAPS - 1)
			{
				e.Meshes[0].Materials[0].Technique = "ResampleAvgLumExp";
				e.Meshes[0].Materials[0].AddVector4ArrayParameter(GetLumaDownscalePassOffsets(num5, num5));
			}
			else
			{
				e.Meshes[0].Materials[0].Technique = "ResampleAvgLum";
				e.Meshes[0].Materials[0].AddVector4ArrayParameter(GetLumaDownscalePassOffsets(num5, num5));
			}
			scene2.Add(e);
			renderPass2D.addSource(scene2);
			LumaPasses.Add(renderPass2D);
			addIntermediatePass(renderPass2D);
			source = renderTarget2D;
		}
		RenderTarget2D item = renderTarget2D;
		AdaptationRT1 = new RenderTarget2D(Engine.Device, 1, 1, mipMap: false, SurfaceFormat.HalfSingle, DepthFormat.None);
		AdaptationRT2 = new RenderTarget2D(Engine.Device, 1, 1, mipMap: false, SurfaceFormat.HalfSingle, DepthFormat.None);
		AdaptLumaPass = new RenderPass2D(createRenderTarget: false);
		AdaptLumaPass.SetRenderTarget(AdaptationRT1, setOwner: false);
		AdaptLumaPass.MustClearColor = false;
		AdaptLumaPass.MustClearDepth = false;
		Scene scene3 = new PostprocessScene();
		AdaptPP = new Postprocess(AdaptationRT2, ShaderManager.Shaders["HDR"]);
		AdaptPP.Meshes[0].Materials[0].Technique = "CalculateAdaptedLum";
		AdaptPP.Meshes[0].Materials[0].Textures.Add(item);
		scene3.Add(AdaptPP);
		AdaptLumaPass.addSource(scene3);
		addIntermediatePass(AdaptLumaPass);
		renderTarget2D = new RenderTarget2D(Engine.Device, num3, num3, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
		BrightPass = new RenderPass2D(createRenderTarget: false);
		BrightPass.SetRenderTarget(renderTarget2D, setOwner: true);
		BrightPass.MustClearColor = false;
		BrightPass.MustClearDepth = false;
		Scene scene4 = new PostprocessScene();
		BrightPP = new Postprocess(HDRScaledPass.RenderTarget, ShaderManager.Shaders["HDR"]);
		BrightPP.Meshes[0].Materials[0].Technique = "BrightPassFilter";
		BrightPP.Meshes[0].Materials[0].Textures.Add(AdaptationRT1);
		BrightPP.Meshes[0].Materials[0].AddFloatParameter(0.5f);
		scene4.Add(BrightPP);
		BrightPass.addSource(scene4);
		addIntermediatePass(BrightPass);
		renderTarget2D = new RenderTarget2D(Engine.Device, num3 / 2, num3 / 2, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
		GaussianBlurPass = new RenderPass2D(createRenderTarget: false);
		GaussianBlurPass.SetRenderTarget(renderTarget2D, setOwner: true);
		GaussianBlurPass.MustClearColor = false;
		GaussianBlurPass.MustClearDepth = false;
		Scene scene5 = new PostprocessScene();
		e = new Postprocess(BrightPass.RenderTarget, ShaderManager.Shaders["HDR"]);
		e.Meshes[0].Materials[0].Technique = "GaussBlur5x5";
		e.Meshes[0].Materials[0].AddVector4ArrayParameter(GetGaussBlur5x5Offsets(num3 / 2, num3 / 2));
		e.Meshes[0].Materials[0].AddVector4ArrayParameter(GetGaussBlur5x5Weights(1f));
		scene5.Add(e);
		GaussianBlurPass.addSource(scene5);
		addIntermediatePass(GaussianBlurPass);
		renderTarget2D = new RenderTarget2D(Engine.Device, num3 / 2, num3 / 2, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
		BloomPass1 = new RenderPass2D(createRenderTarget: false);
		BloomPass1.SetRenderTarget(renderTarget2D, setOwner: true);
		BloomPass1.MustClearColor = false;
		BloomPass1.MustClearDepth = false;
		Scene scene6 = new PostprocessScene();
		e = new Postprocess(GaussianBlurPass.RenderTarget, ShaderManager.Shaders["HDR"]);
		e.Meshes[0].Materials[0].Technique = "Bloom";
		e.Meshes[0].Materials[0].AddVector4ArrayParameter(GetBloomOffsets_Width(num3 / 2));
		e.Meshes[0].Materials[0].AddVector4ArrayParameter(GetBloomWeights(3f, 2f));
		scene6.Add(e);
		BloomPass1.addSource(scene6);
		addIntermediatePass(BloomPass1);
		renderTarget2D = new RenderTarget2D(Engine.Device, num3 / 2, num3 / 2, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
		BloomPass2 = new RenderPass2D(createRenderTarget: false);
		BloomPass2.SetRenderTarget(renderTarget2D, setOwner: true);
		BloomPass2.MustClearColor = false;
		BloomPass2.MustClearDepth = false;
		Scene scene7 = new PostprocessScene();
		e = new Postprocess(BloomPass1.RenderTarget, ShaderManager.Shaders["HDR"]);
		e.Meshes[0].Materials[0].Technique = "Bloom";
		e.Meshes[0].Materials[0].AddVector4ArrayParameter(GetBloomOffsets_Height(num3 / 2));
		e.Meshes[0].Materials[0].AddVector4ArrayParameter(GetBloomWeights(3f, 2f));
		scene7.Add(e);
		BloomPass2.addSource(scene7);
		addIntermediatePass(BloomPass2);
		PostprocessScene postprocessScene = new PostprocessScene();
		FinalPP = new Postprocess(HDRPass.RenderTarget, ShaderManager.Shaders["HDR"]);
		FinalPP.Meshes[0].Materials[0].Technique = "FinalScenePass";
		FinalPP.Meshes[0].Materials[0].Textures.Add(BloomPass2.RenderTarget);
		FinalPP.Meshes[0].Materials[0].Textures.Add(AdaptationRT1);
		FinalPP.Meshes[0].Materials[0].AddFloatParameter(0.5f);
		FinalPP.Meshes[0].Materials[0].AddFloatParameter(3f);
		postprocessScene.Add(FinalPP);
		finalRenderPass.MustClearColor = true;
		finalRenderPass.MustClearDepth = true;
		finalRenderPass.addSource(postprocessScene);
	}

	public override void addSource(SceneRenderData scene)
	{
		HDRPass.addSource(scene);
	}

	public override void Render()
	{
		base.Render();
		AdaptLumaPass.SetRenderTarget(evenFrame ? AdaptationRT2 : AdaptationRT1, setOwner: false);
		AdaptPP.Meshes[0].Materials[0].Textures[0] = (evenFrame ? AdaptationRT1 : AdaptationRT2);
		BrightPP.Meshes[0].Materials[0].Textures[1] = (evenFrame ? AdaptationRT2 : AdaptationRT1);
		FinalPP.Meshes[0].Materials[0].Textures[2] = (evenFrame ? AdaptationRT2 : AdaptationRT1);
		evenFrame = !evenFrame;
	}

	public override void Dispose()
	{
		if (AdaptationRT1 != null)
		{
			AdaptationRT1.Dispose();
			AdaptationRT1 = null;
		}
		if (AdaptationRT2 != null)
		{
			AdaptationRT2.Dispose();
			AdaptationRT2 = null;
		}
		base.Dispose();
	}
}
