using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Render;
using Quasar.Scenes;
using Quasar.Shaders;

namespace Quasar.GameUtils.Render;

public class DOFRenderProcess : RenderProcess
{
	private const int NUM_SAMPLES = 15;

	private float deviation = 6f;

	private float baseDistance = 5f;

	private float aperture = 2f;

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

	private Texture srcTexture;

	private Texture dofTexture;

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

	public float BaseDistance
	{
		get
		{
			return baseDistance;
		}
		set
		{
			baseDistance = value;
		}
	}

	public float Aperture
	{
		get
		{
			return aperture;
		}
		set
		{
			aperture = value;
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

	public DOFRenderProcess(Texture texture, Texture dofTexture)
	{
		srcTexture = texture;
		this.dofTexture = dofTexture;
		initTargets();
	}

	public void SetSource(Texture texture)
	{
		srcTexture = texture;
		ppSceneH.PostProcess.Material.Texture = srcTexture;
		ppSceneFinal.PostProcess.Material.Texture = srcTexture;
		passThroughScene.PostProcess.Material.Texture = srcTexture;
	}

	private void initTargets()
	{
		blurWeights = new Vector4[15];
		blurOffsetsW = new Vector4[15];
		blurOffsetsH = new Vector4[15];
		GetBlurWeights(deviation, blurWeights);
		GetBlurOffsets_Width(Engine.BackBufferWidth, blurOffsetsW);
		GetBlurOffsets_Height(Engine.BackBufferHeight, blurOffsetsH);
		hPass = new RenderPass2D(createRenderTarget: true);
		ppSceneH = new PostprocessScene(srcTexture, ShaderManager.Shaders["DOFBlur"]);
		ppSceneH.PostProcess.Mesh.FirstMaterial.Textures.Add(dofTexture);
		ppSceneH.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurOffsetsW);
		ppSceneH.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurWeights);
		ppSceneH.PostProcess.Material.AddFloatParameter(1f);
		ppSceneH.PostProcess.Material.AddFloatParameter(1f);
		hPass.addSource(ppSceneH);
		addIntermediatePass(hPass);
		ppSceneFinal = new PostprocessScene(hPass.RenderTarget, ShaderManager.Shaders["DOFBlur"]);
		ppSceneFinal.PostProcess.Mesh.FirstMaterial.Textures.Add(dofTexture);
		ppSceneFinal.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurOffsetsH);
		ppSceneFinal.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurWeights);
		ppSceneFinal.PostProcess.Material.AddFloatParameter(1f);
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
			ppSceneH.PostProcess.Material.SetFloatParameter(1, aperture);
			ppSceneFinal.PostProcess.Material.SetFloatParameter(1, aperture);
			ppSceneH.PostProcess.Material.SetFloatParameter(0, baseDistance);
			ppSceneFinal.PostProcess.Material.SetFloatParameter(0, baseDistance);
		}
		base.Render();
	}
}
