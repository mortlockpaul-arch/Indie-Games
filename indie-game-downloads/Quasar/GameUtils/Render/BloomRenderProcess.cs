using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Render;
using Quasar.Scenes;
using Quasar.Shaders;

namespace Quasar.GameUtils.Render;

public class BloomRenderProcess : RenderProcess
{
	private const int NUM_SAMPLES = 15;

	private float bloomThreshold = 0.4f;

	private float bloomMultiplier = 1.75f;

	private float deviation = 6f;

	private float blurAmount = 1f;

	private PostprocessScene ppSceneH;

	private PostprocessScene ppSceneFinal;

	private RenderPass2D hPass;

	private PostprocessScene passThroughScene;

	private RenderPass2D passThrough;

	private Vector4[] blurWeights;

	private Vector4[] blurOffsetsW;

	private Vector4[] blurOffsetsH;

	private List<float> tempFloatList = new List<float>(15);

	private Texture2D srcTexture;

	public float BloomThreshold
	{
		get
		{
			return bloomThreshold;
		}
		set
		{
			bloomThreshold = value;
		}
	}

	public float BloomMultiplier
	{
		get
		{
			return bloomMultiplier;
		}
		set
		{
			bloomMultiplier = value;
		}
	}

	public float Deviation
	{
		get
		{
			return deviation;
		}
		set
		{
			deviation = value;
			GetBlurWeights(deviation, blurWeights);
		}
	}

	public float BlurAmount
	{
		get
		{
			return blurAmount;
		}
		set
		{
			blurAmount = value;
			GetBlurOffsets_Width(Engine.BackBufferWidth, blurOffsetsW);
			GetBlurOffsets_Height(Engine.BackBufferHeight, blurOffsetsH);
		}
	}

	private Vector4[] GetBlurOffsets_Width(float Size, Vector4[] L)
	{
		GetBlurOffsets(Size, tempFloatList);
		for (int i = 0; i < 15; i++)
		{
			ref Vector4 reference = ref L[i];
			reference = new Vector4(tempFloatList[i], 0f, 0f, 0f);
		}
		return L;
	}

	private Vector4[] GetBlurOffsets_Height(float Size, Vector4[] L)
	{
		GetBlurOffsets(Size, tempFloatList);
		for (int i = 0; i < 15; i++)
		{
			ref Vector4 reference = ref L[i];
			reference = new Vector4(0f, tempFloatList[i], 0f, 0f);
		}
		return L;
	}

	private void GetBlurOffsets(float Size, List<float> L)
	{
		L.Clear();
		int num = 0;
		float num2 = 1f / Size;
		L.Add(0f);
		for (num = 1; num < 8; num++)
		{
			L.Add((float)num * num2 * blurAmount);
		}
		for (num = 8; num < 15; num++)
		{
			L.Add(0f - L[num - 7]);
		}
	}

	private void GetBlurWeights(float deviation, Vector4[] L)
	{
		int num = 0;
		float num2 = GaussianDistribution(0f, 0f, deviation);
		ref Vector4 reference = ref L[num];
		reference = new Vector4(num2, num2, num2, 1f);
		float num3 = num2;
		for (num = 1; num < 8; num++)
		{
			num2 = GaussianDistribution(num, 0f, deviation);
			num3 += 2f * num2;
			ref Vector4 reference2 = ref L[num];
			reference2 = new Vector4(num2, num2, num2, 1f);
		}
		for (num = 8; num < 15; num++)
		{
			ref Vector4 reference3 = ref L[num];
			reference3 = L[num - 7];
		}
		for (num = 0; num < 15; num++)
		{
			L[num] /= num3;
		}
	}

	private float GaussianDistribution(float x, float y, float rho)
	{
		float num = 1f / ((float)Math.Sqrt(6.2831854820251465) * rho);
		return num * (float)Math.Exp((0f - (x * x + y * y)) / (2f * rho * rho));
	}

	public BloomRenderProcess(Texture2D texture)
	{
		srcTexture = texture;
		initTargets();
	}

	public void SetSource(Texture2D texture)
	{
		srcTexture = texture;
		ppSceneH.PostProcess.Material.Texture = srcTexture;
		ppSceneFinal.PostProcess.Material.Texture = srcTexture;
		passThroughScene.PostProcess.Material.Texture = srcTexture;
		int num = ((srcTexture != null) ? srcTexture.Width : Engine.BackBufferWidth);
		int num2 = ((srcTexture != null) ? srcTexture.Height : Engine.BackBufferHeight);
		if (hPass.RenderTarget.Width != num || hPass.RenderTarget.Height != num2)
		{
			hPass.SetRenderTarget(RenderPass2D.CreateRenderTarget(new Vector2(num, num2)), setOwner: true);
			ppSceneFinal.PostProcess.Material.SetTexture(1, hPass.RenderTarget);
		}
		GetBlurOffsets_Width(num, blurOffsetsW);
		GetBlurOffsets_Height(num2, blurOffsetsH);
	}

	private void initTargets()
	{
		blurWeights = new Vector4[15];
		blurOffsetsW = new Vector4[15];
		blurOffsetsH = new Vector4[15];
		GetBlurWeights(deviation, blurWeights);
		int num = ((srcTexture != null) ? srcTexture.Width : Engine.BackBufferWidth);
		int num2 = ((srcTexture != null) ? srcTexture.Height : Engine.BackBufferHeight);
		GetBlurOffsets_Width(num, blurOffsetsW);
		GetBlurOffsets_Height(num2, blurOffsetsH);
		hPass = new RenderPass2D(new Vector2(num, num2));
		ppSceneH = new PostprocessScene(srcTexture, ShaderManager.Shaders["BloomBlur"]);
		ppSceneH.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurOffsetsW);
		ppSceneH.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurWeights);
		ppSceneH.PostProcess.Material.AddFloatParameter(1f);
		hPass.addSource(ppSceneH);
		addIntermediatePass(hPass);
		ppSceneFinal = new PostprocessScene(srcTexture, ShaderManager.Shaders["Bloom"]);
		ppSceneFinal.PostProcess.Material.Textures.Add(hPass.RenderTarget);
		ppSceneFinal.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurOffsetsH);
		ppSceneFinal.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurWeights);
		ppSceneFinal.PostProcess.Material.AddFloatParameter(1f);
		finalRenderPass.addSource(ppSceneFinal);
		finalRenderPass.MustClearColor = false;
		finalRenderPass.MustClearDepth = false;
		passThrough = new RenderPass2D(createRenderTarget: false);
		passThroughScene = new PostprocessScene(srcTexture);
		passThrough.addSource(passThroughScene);
		passThrough.Enabled = false;
		addIntermediatePass(passThrough);
	}

	public override void Render()
	{
		if (bloomMultiplier < 0.01f)
		{
			hPass.Enabled = false;
			finalRenderPass.Enabled = false;
			passThrough.SetRenderTarget(RenderTarget, setOwner: false);
			passThrough.Enabled = true;
		}
		else
		{
			hPass.Enabled = true;
			finalRenderPass.Enabled = true;
			passThrough.Enabled = false;
			ppSceneH.PostProcess.Material.SetFloatParameter(0, bloomThreshold);
			ppSceneFinal.PostProcess.Material.SetFloatParameter(0, bloomMultiplier);
		}
		base.Render();
	}
}
