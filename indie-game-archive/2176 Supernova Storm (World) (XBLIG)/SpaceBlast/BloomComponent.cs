using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast;

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

	private ResolveTexture2D resolveTarget;

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
		bloomExtractEffect = base.Game.Content.Load<Effect>("Effects/BloomExtract");
		bloomCombineEffect = base.Game.Content.Load<Effect>("Effects/BloomCombine");
		gaussianBlurEffect = base.Game.Content.Load<Effect>("Effects/GaussianBlur");
		PresentationParameters presentationParameters = base.GraphicsDevice.PresentationParameters;
		int backBufferWidth = presentationParameters.BackBufferWidth;
		int backBufferHeight = presentationParameters.BackBufferHeight;
		SurfaceFormat backBufferFormat = presentationParameters.BackBufferFormat;
		resolveTarget = new ResolveTexture2D(base.GraphicsDevice, backBufferWidth, backBufferHeight, 1, backBufferFormat);
		backBufferWidth /= 2;
		backBufferHeight /= 2;
		renderTarget1 = new RenderTarget2D(base.GraphicsDevice, backBufferWidth, backBufferHeight, 1, backBufferFormat);
		renderTarget2 = new RenderTarget2D(base.GraphicsDevice, backBufferWidth, backBufferHeight, 1, backBufferFormat);
	}

	protected override void UnloadContent()
	{
		resolveTarget.Dispose();
		renderTarget1.Dispose();
		renderTarget2.Dispose();
	}

	public override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.ResolveBackBuffer(resolveTarget);
		bloomExtractEffect.Parameters["BloomThreshold"].SetValue(Settings.BloomThreshold);
		DrawFullscreenQuad(resolveTarget, renderTarget1, bloomExtractEffect, IntermediateBuffer.PreBloom);
		SetBlurEffectParameters(1f / (float)renderTarget1.Width, 0f);
		DrawFullscreenQuad(renderTarget1.GetTexture(), renderTarget2, gaussianBlurEffect, IntermediateBuffer.BlurredHorizontally);
		SetBlurEffectParameters(0f, 1f / (float)renderTarget1.Height);
		DrawFullscreenQuad(renderTarget2.GetTexture(), renderTarget1, gaussianBlurEffect, IntermediateBuffer.BlurredBothWays);
		base.GraphicsDevice.SetRenderTarget(0, null);
		EffectParameterCollection parameters = bloomCombineEffect.Parameters;
		parameters["BloomIntensity"].SetValue(Settings.BloomIntensity);
		parameters["BaseIntensity"].SetValue(Settings.BaseIntensity);
		parameters["BloomSaturation"].SetValue(Settings.BloomSaturation);
		parameters["BaseSaturation"].SetValue(Settings.BaseSaturation);
		base.GraphicsDevice.Textures[1] = resolveTarget;
		Viewport viewport = base.GraphicsDevice.Viewport;
		DrawFullscreenQuad(renderTarget1.GetTexture(), viewport.Width, viewport.Height, bloomCombineEffect, IntermediateBuffer.FinalResult);
	}

	private void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, Effect effect, IntermediateBuffer currentBuffer)
	{
		base.GraphicsDevice.SetRenderTarget(0, renderTarget);
		DrawFullscreenQuad(texture, renderTarget.Width, renderTarget.Height, effect, currentBuffer);
		base.GraphicsDevice.SetRenderTarget(0, null);
	}

	private void DrawFullscreenQuad(Texture2D texture, int width, int height, Effect effect, IntermediateBuffer currentBuffer)
	{
		MainGame.Instance.GraphicsDevice.RenderState.StencilEnable = false;
		spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
		if (showBuffer >= currentBuffer)
		{
			effect.Begin();
			effect.CurrentTechnique.Passes[0].Begin();
		}
		spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
		spriteBatch.End();
		if (showBuffer >= currentBuffer)
		{
			effect.CurrentTechnique.Passes[0].End();
			effect.End();
		}
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
