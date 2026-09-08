using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Render;
using Quasar.Scenes;
using Quasar.Shaders;

namespace Quasar.GameUtils.Render;

public class BlurRenderProcess : RenderProcess
{
	private float blurAmount;

	private PostprocessScene ppSceneH;

	private PostprocessScene ppSceneV;

	private PostprocessScene passThroughScene;

	private RenderPass2D passThrough;

	private RenderPass2D hPass;

	private Vector4[] blurWeights;

	private Texture srcTexture;

	public float Blur
	{
		get
		{
			return blurAmount;
		}
		set
		{
			blurAmount = value;
		}
	}

	public Shader Shader
	{
		set
		{
			ppSceneH.PostProcess.Mesh.Shader = value;
			ppSceneV.PostProcess.Mesh.Shader = value;
		}
	}

	private Vector4[] GetBlurOffsets_Width(int Size)
	{
		List<float> blurOffsets = GetBlurOffsets(Size);
		Vector4[] array = new Vector4[15];
		for (int i = 0; i < 15; i++)
		{
			ref Vector4 reference = ref array[i];
			reference = new Vector4(blurOffsets[i], 0f, 0f, 0f);
		}
		return array;
	}

	private Vector4[] GetBlurOffsets_Height(int Size)
	{
		List<float> blurOffsets = GetBlurOffsets(Size);
		Vector4[] array = new Vector4[15];
		for (int i = 0; i < 15; i++)
		{
			ref Vector4 reference = ref array[i];
			reference = new Vector4(0f, blurOffsets[i], 0f, 0f);
		}
		return array;
	}

	private List<float> GetBlurOffsets(int Size)
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
		float num = 1f / (float)Math.Sqrt((float)Math.PI * 2f * rho * rho);
		return num * (float)Math.Exp((0f - (x * x + y * y)) / (2f * rho * rho));
	}

	public BlurRenderProcess(Texture srcTexture)
	{
		this.srcTexture = srcTexture;
		initTargets();
	}

	public void SetSource(Texture texture)
	{
		srcTexture = texture;
		ppSceneH.PostProcess.Material.Texture = srcTexture;
		passThroughScene.PostProcess.Material.Texture = srcTexture;
	}

	private void initTargets()
	{
		blurWeights = new Vector4[15];
		GetBlurWeights(3f, blurWeights);
		hPass = new RenderPass2D(createRenderTarget: true);
		ppSceneH = new PostprocessScene(srcTexture, ShaderManager.Shaders["Blur"]);
		ppSceneH.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(GetBlurOffsets_Width(Engine.BackBufferWidth));
		ppSceneH.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurWeights);
		ppSceneH.PostProcess.Material.AddFloatParameter(1f);
		hPass.addSource(ppSceneH);
		addIntermediatePass(hPass);
		ppSceneV = new PostprocessScene(hPass.RenderTarget, ShaderManager.Shaders["Blur"]);
		ppSceneV.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(GetBlurOffsets_Height(Engine.BackBufferHeight));
		ppSceneV.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurWeights);
		ppSceneV.PostProcess.Material.AddFloatParameter(1f);
		finalRenderPass.addSource(ppSceneV);
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
		if (blurAmount < 0.01f)
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
			ppSceneH.PostProcess.Material.SetFloatParameter(0, blurAmount);
			ppSceneV.PostProcess.Material.SetFloatParameter(0, blurAmount);
		}
		base.Render();
	}

	public override void addSource(SceneRenderData scene)
	{
		base.addSource(scene);
		passThrough.addSource(scene);
	}
}
