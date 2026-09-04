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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		spriteBatch = new SpriteBatch(((DrawableGameComponent)this).GraphicsDevice);
		bloomExtractEffect = ((GameComponent)this).Game.Content.Load<Effect>("Effects/BloomExtract");
		bloomCombineEffect = ((GameComponent)this).Game.Content.Load<Effect>("Effects/BloomCombine");
		gaussianBlurEffect = ((GameComponent)this).Game.Content.Load<Effect>("Effects/GaussianBlur");
		PresentationParameters presentationParameters = ((DrawableGameComponent)this).GraphicsDevice.PresentationParameters;
		int backBufferWidth = presentationParameters.BackBufferWidth;
		int backBufferHeight = presentationParameters.BackBufferHeight;
		SurfaceFormat backBufferFormat = presentationParameters.BackBufferFormat;
		resolveTarget = new ResolveTexture2D(((DrawableGameComponent)this).GraphicsDevice, backBufferWidth, backBufferHeight, 1, backBufferFormat);
		backBufferWidth /= 2;
		backBufferHeight /= 2;
		renderTarget1 = new RenderTarget2D(((DrawableGameComponent)this).GraphicsDevice, backBufferWidth, backBufferHeight, 1, backBufferFormat);
		renderTarget2 = new RenderTarget2D(((DrawableGameComponent)this).GraphicsDevice, backBufferWidth, backBufferHeight, 1, backBufferFormat);
	}

	protected override void UnloadContent()
	{
		((GraphicsResource)resolveTarget).Dispose();
		((RenderTarget)renderTarget1).Dispose();
		((RenderTarget)renderTarget2).Dispose();
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		((DrawableGameComponent)this).GraphicsDevice.ResolveBackBuffer(resolveTarget);
		bloomExtractEffect.Parameters["BloomThreshold"].SetValue(Settings.BloomThreshold);
		DrawFullscreenQuad((Texture2D)(object)resolveTarget, renderTarget1, bloomExtractEffect, IntermediateBuffer.PreBloom);
		SetBlurEffectParameters(1f / (float)((RenderTarget)renderTarget1).Width, 0f);
		DrawFullscreenQuad(renderTarget1.GetTexture(), renderTarget2, gaussianBlurEffect, IntermediateBuffer.BlurredHorizontally);
		SetBlurEffectParameters(0f, 1f / (float)((RenderTarget)renderTarget1).Height);
		DrawFullscreenQuad(renderTarget2.GetTexture(), renderTarget1, gaussianBlurEffect, IntermediateBuffer.BlurredBothWays);
		((DrawableGameComponent)this).GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
		EffectParameterCollection parameters = bloomCombineEffect.Parameters;
		parameters["BloomIntensity"].SetValue(Settings.BloomIntensity);
		parameters["BaseIntensity"].SetValue(Settings.BaseIntensity);
		parameters["BloomSaturation"].SetValue(Settings.BloomSaturation);
		parameters["BaseSaturation"].SetValue(Settings.BaseSaturation);
		((DrawableGameComponent)this).GraphicsDevice.Textures[1] = (Texture)(object)resolveTarget;
		Viewport viewport = ((DrawableGameComponent)this).GraphicsDevice.Viewport;
		DrawFullscreenQuad(renderTarget1.GetTexture(), ((Viewport)(ref viewport)).Width, ((Viewport)(ref viewport)).Height, bloomCombineEffect, IntermediateBuffer.FinalResult);
	}

	private void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, Effect effect, IntermediateBuffer currentBuffer)
	{
		((DrawableGameComponent)this).GraphicsDevice.SetRenderTarget(0, renderTarget);
		DrawFullscreenQuad(texture, ((RenderTarget)renderTarget).Width, ((RenderTarget)renderTarget).Height, effect, currentBuffer);
		((DrawableGameComponent)this).GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
	}

	private void DrawFullscreenQuad(Texture2D texture, int width, int height, Effect effect, IntermediateBuffer currentBuffer)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		((Game)MainGame.Instance).GraphicsDevice.RenderState.StencilEnable = false;
		spriteBatch.Begin((SpriteBlendMode)0, (SpriteSortMode)0, (SaveStateMode)0);
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
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		EffectParameter val = gaussianBlurEffect.Parameters["SampleWeights"];
		EffectParameter val2 = gaussianBlurEffect.Parameters["SampleOffsets"];
		int count = val.Elements.Count;
		float[] array = new float[count];
		Vector2[] array2 = (Vector2[])(object)new Vector2[count];
		array[0] = ComputeGaussian(0f);
		ref Vector2 reference = ref array2[0];
		reference = new Vector2(0f);
		float num = array[0];
		for (int i = 0; i < count / 2; i++)
		{
			num += (array[i * 2 + 2] = (array[i * 2 + 1] = ComputeGaussian(i + 1))) * 2f;
			float num2 = (float)(i * 2) + 1.5f;
			Vector2 val3 = new Vector2(dx, dy) * num2;
			array2[i * 2 + 1] = val3;
			ref Vector2 reference2 = ref array2[i * 2 + 2];
			reference2 = -val3;
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j] /= num;
		}
		val.SetValue(array);
		val2.SetValue(array2);
	}

	private float ComputeGaussian(float n)
	{
		float blurAmount = Settings.BlurAmount;
		return (float)(1.0 / Math.Sqrt(Math.PI * 2.0 * (double)blurAmount) * Math.Exp((0f - n * n) / (2f * blurAmount * blurAmount)));
	}
}
