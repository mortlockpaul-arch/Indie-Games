using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine.Graphics;

public static class BloomEffect
{
	private enum Tech
	{
		Extract,
		NormalExtract,
		GausianBlur,
		BloomCombine,
		Combine
	}

	private static bool nonBloomCalled = false;

	private static float[] sampleWeights = new float[15];

	private static Vector2[] sampleOffsets = new Vector2[15];

	private static GraphicsDevice device;

	private static SpriteBatch spriteBatch;

	private static Effect effect;

	private static EffectParameterCollection parameters;

	private static RenderTarget2D nonBloomTarget;

	private static RenderTarget2D normalAlphaTarget;

	private static RenderTarget2D bloomTarget;

	private static RenderTarget2D renderTarget1;

	private static RenderTarget2D renderTarget2;

	private static RenderTarget2D finalTarget;

	private static List<RenderTarget2D> finalRenderTargets = new List<RenderTarget2D>();

	public static bool UsingNormalMap { get; set; }

	public static float BloomThreshold
	{
		set
		{
			parameters["BloomThreshold"].SetValue(value);
		}
	}

	public static float BlurAmount { get; set; }

	public static float BloomIntensity
	{
		set
		{
			parameters["BloomIntensity"].SetValue(value);
		}
	}

	public static float BaseIntensity
	{
		set
		{
			parameters["BaseIntensity"].SetValue(value);
		}
	}

	public static float BloomSaturation
	{
		set
		{
			parameters["BloomSaturation"].SetValue(value);
		}
	}

	public static float BaseSaturation
	{
		set
		{
			parameters["BaseSaturation"].SetValue(value);
		}
	}

	public static void ApplySettings(BloomSettings settings)
	{
		parameters["BloomThreshold"].SetValue(settings.BloomThreshold);
		BlurAmount = settings.BlurAmount;
		parameters["BloomIntensity"].SetValue(settings.BloomIntensity);
		parameters["BaseIntensity"].SetValue(settings.BaseIntensity);
		parameters["BloomSaturation"].SetValue(settings.BloomSaturation);
		parameters["BaseSaturation"].SetValue(settings.BaseSaturation);
	}

	public static void Initialize(GraphicsDevice Device)
	{
		device = Device;
		spriteBatch = new SpriteBatch(device);
		AssetManager.GetAsset(EffectKeys.BloomEffect, out effect);
		parameters = effect.Parameters;
		PresentationParameters presentationParameters = device.PresentationParameters;
		int screenWidth = Global.ScreenWidth;
		int screenHeight = Global.ScreenHeight;
		bloomTarget = new RenderTarget2D(device, screenWidth, screenHeight, mipMap: false, presentationParameters.BackBufferFormat, presentationParameters.DepthStencilFormat, presentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);
		finalTarget = new RenderTarget2D(device, screenWidth, screenHeight, mipMap: false, presentationParameters.BackBufferFormat, presentationParameters.DepthStencilFormat, presentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);
		normalAlphaTarget = new RenderTarget2D(device, screenWidth, screenHeight, mipMap: false, presentationParameters.BackBufferFormat, presentationParameters.DepthStencilFormat, presentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);
		nonBloomTarget = new RenderTarget2D(device, screenWidth, screenHeight, mipMap: false, presentationParameters.BackBufferFormat, presentationParameters.DepthStencilFormat, presentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);
		renderTarget1 = new RenderTarget2D(device, screenWidth / 2, screenHeight / 2, mipMap: false, presentationParameters.BackBufferFormat, DepthFormat.None);
		renderTarget2 = new RenderTarget2D(device, screenWidth / 2, screenHeight / 2, mipMap: false, presentationParameters.BackBufferFormat, DepthFormat.None);
	}

	public static void Dispose()
	{
		if (spriteBatch == null)
		{
			spriteBatch.Dispose();
		}
		if (normalAlphaTarget == null)
		{
			normalAlphaTarget.Dispose();
		}
		if (bloomTarget == null)
		{
			bloomTarget.Dispose();
		}
		if (renderTarget1 == null)
		{
			renderTarget1.Dispose();
		}
		if (renderTarget2 == null)
		{
			renderTarget2.Dispose();
		}
		if (finalTarget == null)
		{
			finalTarget.Dispose();
		}
		foreach (RenderTarget2D finalRenderTarget in finalRenderTargets)
		{
			finalRenderTarget.Dispose();
		}
	}

	public static void BeginNonBloom()
	{
		nonBloomCalled = true;
		device.SetRenderTarget(nonBloomTarget);
	}

	public static void BeginBloom()
	{
		device.SetRenderTarget(bloomTarget);
		device.Clear(Color.Transparent);
	}

	public static void BeginBloom(BloomSettings settings)
	{
		ApplySettings(settings);
		BeginBloom();
	}

	public static void GetNormalMapAlphaData(BloomSettings settings)
	{
		UsingNormalMap = true;
		ApplySettings(settings);
		device.SetRenderTarget(normalAlphaTarget);
		device.Clear(Color.Transparent);
	}

	public static void EndBloom()
	{
		device.SamplerStates[1] = SamplerState.LinearClamp;
		device.SamplerStates[2] = SamplerState.LinearClamp;
		if (!UsingNormalMap)
		{
			renderImage(bloomTarget, renderTarget1, Tech.Extract);
		}
		else
		{
			effect.CurrentTechnique = effect.Techniques[Tech.NormalExtract.ToString()];
			device.Textures[2] = normalAlphaTarget;
			renderImage(bloomTarget, renderTarget1, Tech.NormalExtract);
		}
		setBlurParameters(1f / (float)renderTarget1.Width, 0f);
		renderImage(renderTarget1, renderTarget2, Tech.GausianBlur);
		setBlurParameters(0f, 1f / (float)renderTarget1.Height);
		renderImage(renderTarget2, renderTarget1, Tech.GausianBlur);
		device.SetRenderTarget(null);
		device.Textures[1] = bloomTarget;
		if (nonBloomCalled)
		{
			device.Textures[2] = nonBloomTarget;
		}
		else
		{
			device.Textures[2] = null;
		}
		effect.CurrentTechnique = effect.Techniques[Tech.BloomCombine.ToString()];
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
		spriteBatch.Draw(renderTarget1, new Rectangle(0, 0, bloomTarget.Width, bloomTarget.Height), Color.White);
		spriteBatch.End();
	}

	private static void renderImage(Texture2D texture, RenderTarget2D renderTarget, Tech tech)
	{
		device.SetRenderTarget(renderTarget);
		effect.CurrentTechnique = effect.Techniques[tech.ToString()];
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
		spriteBatch.Draw(texture, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);
		spriteBatch.End();
	}

	private static void setBlurParameters(float dx, float dy)
	{
		EffectParameter effectParameter = parameters["SampleWeights"];
		EffectParameter effectParameter2 = parameters["SampleOffsets"];
		int count = effectParameter.Elements.Count;
		sampleWeights[0] = computeGaussian(0f);
		ref Vector2 reference = ref sampleOffsets[0];
		reference = new Vector2(0f);
		float num = sampleWeights[0];
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < count / 2; i++)
		{
			num2 = computeGaussian(i + 1);
			sampleWeights[i * 2 + 1] = num2;
			sampleWeights[i * 2 + 2] = num2;
			num += num2 * 2f;
			num3 = (float)(i * 2) + 1.5f;
			Vector2 vector = new Vector2(dx, dy) * num3;
			sampleOffsets[i * 2 + 1] = vector;
			ref Vector2 reference2 = ref sampleOffsets[i * 2 + 2];
			reference2 = -vector;
		}
		for (int j = 0; j < sampleWeights.Length; j++)
		{
			sampleWeights[j] /= num;
		}
		effectParameter.SetValue(sampleWeights);
		effectParameter2.SetValue(sampleOffsets);
	}

	private static float computeGaussian(float n)
	{
		float blurAmount = BlurAmount;
		return (float)(1.0 / Math.Sqrt(Math.PI * 2.0 * (double)blurAmount) * Math.Exp((0f - n * n) / (2f * blurAmount * blurAmount)));
	}
}
