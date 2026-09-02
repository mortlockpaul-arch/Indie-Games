using System;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Graphics;

namespace RacingGame.Shaders;

public class ShadowMapBlur : ShaderEffect
{
	private const string Filename = "PostScreenShadowBlur.fx";

	private EffectParameter windowSize;

	private EffectParameter sceneMap;

	private EffectParameter blurMap;

	private RenderToTexture sceneMapTexture;

	private RenderToTexture blurMapTexture;

	public RenderToTexture SceneMapTexture => sceneMapTexture;

	public RenderToTexture BlurMapTexture => blurMapTexture;

	public ShadowMapBlur()
		: base("PostScreenShadowBlur.fx")
	{
		sceneMapTexture = new RenderToTexture(RenderToTexture.SizeType.HalfScreen);
		blurMapTexture = new RenderToTexture(RenderToTexture.SizeType.HalfScreen);
	}

	protected override void GetParameters()
	{
		if (effect != null)
		{
			windowSize = effect.Parameters["windowSize"];
			sceneMap = effect.Parameters["sceneMap"];
			blurMap = effect.Parameters["blurMap"];
			if (windowSize == null || sceneMap == null)
			{
				throw new NotSupportedException("windowSize and sceneMap must be valid in PostScreenShader=PostScreenShadowBlur.fx");
			}
		}
	}

	public void RenderShadows(BaseGame.RenderHandler renderCode)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (renderCode == null)
		{
			throw new ArgumentNullException("renderCode");
		}
		sceneMapTexture.SetRenderTarget();
		sceneMapTexture.Clear(Color.White);
		renderCode();
		sceneMapTexture.Resolve();
		BaseGame.ResetRenderTarget(fullResetToBackBuffer: false);
	}

	public void RenderShadows()
	{
		if (sceneMapTexture != null && base.Valid && sceneMapTexture.XnaTexture != null)
		{
			BaseGame.Device.RenderState.DepthBufferEnable = false;
			BaseGame.Device.RenderState.DepthBufferWriteEnable = false;
			BaseGame.Device.RenderState.AlphaBlendEnable = false;
			if (windowSize != null)
			{
				windowSize.SetValue(new float[2] { sceneMapTexture.Width, sceneMapTexture.Height });
			}
			if (sceneMap != null)
			{
				sceneMap.SetValue((Texture)(object)sceneMapTexture.XnaTexture);
			}
			effect.CurrentTechnique = effect.Techniques["ScreenAdvancedBlur20"];
			if (effect.CurrentTechnique.Passes.Count != 2)
			{
				throw new InvalidOperationException("This shader should have exactly 2 passes!");
			}
			try
			{
				effect.Begin((SaveStateMode)0);
				blurMapTexture.SetRenderTarget();
				EffectPass val = effect.CurrentTechnique.Passes[0];
				val.Begin();
				VBScreenHelper.Render();
				val.End();
			}
			finally
			{
				effect.End();
			}
			blurMapTexture.Resolve();
			BaseGame.ResetRenderTarget(fullResetToBackBuffer: false);
			BaseGame.Device.RenderState.DepthBufferEnable = true;
			BaseGame.Device.RenderState.DepthBufferWriteEnable = true;
			BaseGame.Device.SamplerStates[0].AddressU = (TextureAddressMode)1;
			BaseGame.Device.SamplerStates[0].AddressV = (TextureAddressMode)1;
			BaseGame.SetCurrentAlphaMode(BaseGame.AlphaMode.Default);
		}
	}

	public void ShowShadows()
	{
		if (blurMapTexture != null && base.Valid && blurMapTexture.XnaTexture != null)
		{
			BaseGame.Device.RenderState.DepthBufferEnable = false;
			BaseGame.Device.RenderState.DepthBufferWriteEnable = false;
			BaseGame.Device.SamplerStates[0].AddressU = (TextureAddressMode)3;
			BaseGame.Device.SamplerStates[0].AddressV = (TextureAddressMode)3;
			if (blurMap != null)
			{
				blurMap.SetValue((Texture)(object)blurMapTexture.XnaTexture);
			}
			effect.CurrentTechnique = effect.Techniques["ScreenAdvancedBlur20"];
			if (effect.CurrentTechnique.Passes.Count != 2)
			{
				throw new InvalidOperationException("This shader should have exactly 2 passes!");
			}
			try
			{
				effect.Begin((SaveStateMode)0);
				BaseGame.Device.RenderState.AlphaBlendEnable = true;
				BaseGame.Device.RenderState.AlphaBlendOperation = (BlendFunction)1;
				BaseGame.Device.RenderState.SourceBlend = (Blend)1;
				BaseGame.Device.RenderState.DestinationBlend = (Blend)3;
				EffectPass val = effect.CurrentTechnique.Passes[1];
				val.Begin();
				VBScreenHelper.Render();
				val.End();
			}
			finally
			{
				effect.End();
			}
			BaseGame.Device.RenderState.DepthBufferEnable = true;
			BaseGame.Device.RenderState.DepthBufferWriteEnable = true;
			BaseGame.Device.SamplerStates[0].AddressU = (TextureAddressMode)1;
			BaseGame.Device.SamplerStates[0].AddressV = (TextureAddressMode)1;
			BaseGame.SetCurrentAlphaMode(BaseGame.AlphaMode.Default);
		}
	}
}
