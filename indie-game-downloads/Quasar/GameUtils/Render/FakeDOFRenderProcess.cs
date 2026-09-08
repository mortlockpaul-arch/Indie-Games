using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Render;
using Quasar.Scenes;
using Quasar.Shaders;

namespace Quasar.GameUtils.Render;

public class FakeDOFRenderProcess : RenderProcess
{
	private const int NUM_SAMPLES = 11;

	private float deviation = 6f;

	private Vector2 baseDistance = new Vector2(0.2f, 0.2f);

	private Vector2 aperture = new Vector2(2f, 0f);

	private float blurAmount = 1f;

	private PostprocessScene ppSceneH;

	private PostprocessScene ppSceneFinal;

	private RenderPass2D hPass;

	private PostprocessScene passThroughScene;

	private RenderPass2D passThrough;

	private Vector4[] blurWeights;

	private Vector4[] blurOffsetsW;

	private Vector4[] blurOffsetsH;

	private List<float> tempFloatList = new List<float>(11);

	private Texture srcTexture;

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

	public Vector2 BaseDistance
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

	public Vector2 Aperture
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
		for (int i = 0; i < 11; i++)
		{
			ref Vector4 reference = ref L[i];
			reference = new Vector4(tempFloatList[i], 0f, 0f, 0f);
		}
		return L;
	}

	private Vector4[] GetBlurOffsets_Height(float Size, Vector4[] L)
	{
		GetBlurOffsets(Size, tempFloatList);
		for (int i = 0; i < 11; i++)
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
		for (num = 1; num < 6; num++)
		{
			L.Add((float)num * num2 * blurAmount);
		}
		for (num = 6; num < 11; num++)
		{
			L.Add(0f - L[num - 5]);
		}
	}

	private void GetBlurWeights(float deviation, Vector4[] L)
	{
		int num = 0;
		float num2 = GaussianDistribution(0f, 0f, deviation);
		ref Vector4 reference = ref L[num];
		reference = new Vector4(num2, num2, num2, 1f);
		float num3 = num2;
		for (num = 1; num < 6; num++)
		{
			num2 = GaussianDistribution(num, 0f, deviation);
			num3 += 2f * num2;
			ref Vector4 reference2 = ref L[num];
			reference2 = new Vector4(num2, num2, num2, 1f);
		}
		for (num = 6; num < 11; num++)
		{
			ref Vector4 reference3 = ref L[num];
			reference3 = L[num - 5];
		}
		for (num = 0; num < 11; num++)
		{
			L[num] /= num3;
		}
	}

	private float GaussianDistribution(float x, float y, float rho)
	{
		float num = 1f / ((float)Math.Sqrt(6.2831854820251465) * rho);
		return num * (float)Math.Exp((0f - (x * x + y * y)) / (2f * rho * rho));
	}

	public FakeDOFRenderProcess(Texture texture)
	{
		srcTexture = texture;
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
		blurWeights = new Vector4[11];
		blurOffsetsW = new Vector4[11];
		blurOffsetsH = new Vector4[11];
		GetBlurWeights(deviation, blurWeights);
		GetBlurOffsets_Width(Engine.BackBufferWidth, blurOffsetsW);
		GetBlurOffsets_Height(Engine.BackBufferHeight, blurOffsetsH);
		hPass = new RenderPass2D(createRenderTarget: true);
		ppSceneH = new PostprocessScene(srcTexture, ShaderManager.Shaders["FakeDOFBlur"]);
		ppSceneH.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurOffsetsW);
		ppSceneH.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurWeights);
		ppSceneH.PostProcess.Material.AddVector4Parameter(Vector4.One);
		ppSceneH.PostProcess.Material.AddVector4Parameter(Vector4.One);
		hPass.addSource(ppSceneH);
		addIntermediatePass(hPass);
		ppSceneFinal = new PostprocessScene(hPass.RenderTarget, ShaderManager.Shaders["FakeDOFBlur"]);
		ppSceneFinal.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurOffsetsH);
		ppSceneFinal.PostProcess.Mesh.FirstMaterial.AddVector4ArrayParameter(blurWeights);
		ppSceneFinal.PostProcess.Material.AddVector4Parameter(Vector4.One);
		ppSceneFinal.PostProcess.Material.AddVector4Parameter(Vector4.One);
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
			ppSceneH.PostProcess.Material.SetVector4Parameter(1, new Vector4(aperture, 0f, 0f));
			ppSceneFinal.PostProcess.Material.SetVector4Parameter(1, new Vector4(aperture, 0f, 0f));
			ppSceneH.PostProcess.Material.SetVector4Parameter(0, new Vector4(baseDistance, 0f, 0f));
			ppSceneFinal.PostProcess.Material.SetVector4Parameter(0, new Vector4(baseDistance, 0f, 0f));
		}
		base.Render();
	}

	public override void addSource(SceneRenderData scene)
	{
		base.addSource(scene);
		passThrough.addSource(scene);
	}
}
