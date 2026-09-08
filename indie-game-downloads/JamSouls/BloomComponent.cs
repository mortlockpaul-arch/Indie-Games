using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class BloomComponent : DrawableGameComponent
{
	public enum IntermediateBuffer
	{
		PreBloom,
		BlurredHorizontally,
		BlurredBothWays,
		FinalResult
	}

	private SpriteBatch spriteBatch;

	private Effect bloomExtractEffect;

	private Effect bloomCombineEffect;

	private Effect gaussianBlurEffect;

	private RenderTarget2D sceneRenderTarget;

	private RenderTarget2D renderTarget1;

	private RenderTarget2D renderTarget2;

	private BloomSettings settings = BloomSettings.PresetSettings[0];

	private IntermediateBuffer showBuffer = IntermediateBuffer.FinalResult;

	public BloomSettings Settings
	{
		get
		{
			return settings;
		}
		set
		{
			settings = value;
		}
	}

	public IntermediateBuffer ShowBuffer
	{
		get
		{
			return showBuffer;
		}
		set
		{
			showBuffer = value;
		}
	}

	public BloomComponent(Game game)
		: base(game)
	{
		if (game == null)
		{
			throw new ArgumentNullException("game");
		}
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		bloomExtractEffect = base.Game.Content.Load<Effect>("Fx/PostProcess/BloomExtract");
		bloomCombineEffect = base.Game.Content.Load<Effect>("Fx/PostProcess/BloomCombine");
		gaussianBlurEffect = base.Game.Content.Load<Effect>("Fx/PostProcess/GaussianBlur");
		PresentationParameters presentationParameters = base.GraphicsDevice.PresentationParameters;
		int backBufferWidth = presentationParameters.BackBufferWidth;
		int backBufferHeight = presentationParameters.BackBufferHeight;
		SurfaceFormat backBufferFormat = presentationParameters.BackBufferFormat;
		sceneRenderTarget = new RenderTarget2D(base.GraphicsDevice, backBufferWidth, backBufferHeight, mipMap: false, backBufferFormat, presentationParameters.DepthStencilFormat, presentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);
		backBufferWidth /= 2;
		backBufferHeight /= 2;
		renderTarget1 = new RenderTarget2D(base.GraphicsDevice, backBufferWidth, backBufferHeight, mipMap: false, backBufferFormat, DepthFormat.None);
		renderTarget2 = new RenderTarget2D(base.GraphicsDevice, backBufferWidth, backBufferHeight, mipMap: false, backBufferFormat, DepthFormat.None);
	}

	protected override void UnloadContent()
	{
		sceneRenderTarget.Dispose();
		renderTarget1.Dispose();
		renderTarget2.Dispose();
	}

	public void BeginDraw()
	{
		if (base.Visible)
		{
			base.GraphicsDevice.SetRenderTarget(sceneRenderTarget);
			base.GraphicsDevice.Clear(Color.Black);
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
		bloomExtractEffect.Parameters["BloomThreshold"].SetValue(Settings.BloomThreshold);
		DrawFullscreenQuad(sceneRenderTarget, renderTarget1, bloomExtractEffect, IntermediateBuffer.PreBloom);
		SetBlurEffectParameters(1f / (float)renderTarget1.Width, 0f);
		DrawFullscreenQuad(renderTarget1, renderTarget2, gaussianBlurEffect, IntermediateBuffer.BlurredHorizontally);
		SetBlurEffectParameters(0f, 1f / (float)renderTarget1.Height);
		DrawFullscreenQuad(renderTarget2, renderTarget1, gaussianBlurEffect, IntermediateBuffer.BlurredBothWays);
		base.GraphicsDevice.SetRenderTarget(null);
		EffectParameterCollection parameters = bloomCombineEffect.Parameters;
		parameters["BloomIntensity"].SetValue(Settings.BloomIntensity);
		parameters["BaseIntensity"].SetValue(Settings.BaseIntensity);
		parameters["BloomSaturation"].SetValue(Settings.BloomSaturation);
		parameters["BaseSaturation"].SetValue(Settings.BaseSaturation);
		base.GraphicsDevice.Textures[1] = sceneRenderTarget;
		Viewport viewport = base.GraphicsDevice.Viewport;
		DrawFullscreenQuad(renderTarget1, viewport.Width, viewport.Height, bloomCombineEffect, IntermediateBuffer.FinalResult);
	}

	private void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, Effect effect, IntermediateBuffer currentBuffer)
	{
		base.GraphicsDevice.SetRenderTarget(renderTarget);
		DrawFullscreenQuad(texture, renderTarget.Width, renderTarget.Height, effect, currentBuffer);
	}

	private void DrawFullscreenQuad(Texture2D texture, int width, int height, Effect effect, IntermediateBuffer currentBuffer)
	{
		if (showBuffer < currentBuffer)
		{
			effect = null;
		}
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
		spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
		spriteBatch.End();
	}

	private void SetBlurEffectParameters(float dx, float dy)
	{
		EffectParameter effectParameter = gaussianBlurEffect.Parameters["SampleWeights"];
		EffectParameter effectParameter2 = gaussianBlurEffect.Parameters["SampleOffsets"];
		int count = effectParameter.Elements.Count;
		float[] array = new float[count];
		Vector2[] array2 = new Vector2[count];
		array[0] = ComputeGaussian(0f);
		ref Vector2 reference = ref array2[0];
		reference = new Vector2(0f);
		float num = array[0];
		for (int i = 0; i < count / 2; i++)
		{
			num += (array[i * 2 + 2] = (array[i * 2 + 1] = ComputeGaussian(i + 1))) * 2f;
			float num2 = (float)(i * 2) + 1.5f;
			Vector2 vector = new Vector2(dx, dy) * num2;
			array2[i * 2 + 1] = vector;
			ref Vector2 reference2 = ref array2[i * 2 + 2];
			reference2 = -vector;
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j] /= num;
		}
		effectParameter.SetValue(array);
		effectParameter2.SetValue(array2);
	}

	private float ComputeGaussian(float n)
	{
		float blurAmount = Settings.BlurAmount;
		return (float)(1.0 / Math.Sqrt(Math.PI * 2.0 * (double)blurAmount) * Math.Exp((0f - n * n) / (2f * blurAmount * blurAmount)));
	}
}
