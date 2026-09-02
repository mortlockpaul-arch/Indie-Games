using System;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Graphics;

namespace RacingGame.Shaders;

public class PostScreenGlow : PostScreenMenu
{
	private const string Filename = "PostScreenGlow.fx";

	private EffectParameter radialSceneMap;

	private EffectParameter radialBlurScaleFactor;

	private EffectParameter screenBorderFadeoutMap;

	private RenderToTexture radialSceneMapTexture;

	private Texture screenBorderFadeoutMapTexture;

	private float lastUsedRadialBlurScaleFactor;

	public float RadialBlurScaleFactor
	{
		get
		{
			return lastUsedRadialBlurScaleFactor;
		}
		set
		{
			if (radialBlurScaleFactor != null && lastUsedRadialBlurScaleFactor != value)
			{
				lastUsedRadialBlurScaleFactor = value;
				radialBlurScaleFactor.SetValue(value);
			}
		}
	}

	public PostScreenGlow()
		: base("PostScreenGlow.fx")
	{
		radialSceneMapTexture = new RenderToTexture(RenderToTexture.SizeType.FullScreen);
	}

	protected override void GetParameters()
	{
		if (effect != null)
		{
			windowSize = effect.Parameters["windowSize"];
			sceneMap = effect.Parameters["sceneMap"];
			if (windowSize == null || sceneMap == null)
			{
				throw new NotSupportedException("windowSize and sceneMap must be valid in PostScreenShader=PostScreenGlow.fx");
			}
			downsampleMap = effect.Parameters["downsampleMap"];
			blurMap1 = effect.Parameters["blurMap1"];
			blurMap2 = effect.Parameters["blurMap2"];
			radialSceneMap = effect.Parameters["radialSceneMap"];
			screenBorderFadeoutMap = effect.Parameters["screenBorderFadeoutMap"];
			screenBorderFadeoutMapTexture = new Texture("ScreenBorderFadeout.dds");
			screenBorderFadeoutMap.SetValue((Texture)(object)screenBorderFadeoutMapTexture.XnaTexture);
			radialBlurScaleFactor = effect.Parameters["radialBlurScaleFactor"];
		}
	}

	public override void Show()
	{
		if (PostScreenMenu.sceneMapTexture == null || effect == null || !PostScreenMenu.started)
		{
			return;
		}
		PostScreenMenu.started = false;
		PostScreenMenu.sceneMapTexture.Resolve();
		BaseGame.Device.RenderState.DepthBufferEnable = false;
		BaseGame.Device.RenderState.DepthBufferWriteEnable = false;
		BaseGame.Device.RenderState.AlphaBlendEnable = false;
		if (windowSize != null)
		{
			windowSize.SetValue(new float[2]
			{
				PostScreenMenu.sceneMapTexture.Width,
				PostScreenMenu.sceneMapTexture.Height
			});
		}
		if (sceneMap != null)
		{
			sceneMap.SetValue((Texture)(object)PostScreenMenu.sceneMapTexture.XnaTexture);
		}
		RadialBlurScaleFactor = 0f - (0.0025f + RacingGameManager.Player.Speed * 0.005f / 47.465855f);
		effect.CurrentTechnique = effect.Techniques["ScreenGlow20"];
		if (effect.CurrentTechnique.Passes.Count != 5)
		{
			throw new InvalidOperationException("This shader should have exactly 5 passes!");
		}
		try
		{
			effect.Begin();
			for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
			{
				switch (i)
				{
				case 0:
					radialSceneMapTexture.SetRenderTarget();
					break;
				case 1:
					PostScreenMenu.downsampleMapTexture.SetRenderTarget();
					break;
				case 2:
					PostScreenMenu.blurMap1Texture.SetRenderTarget();
					break;
				case 3:
					PostScreenMenu.blurMap2Texture.SetRenderTarget();
					break;
				default:
					BaseGame.ResetRenderTarget(fullResetToBackBuffer: true);
					break;
				}
				EffectPass val = effect.CurrentTechnique.Passes[i];
				val.Begin();
				if (i == 0)
				{
					VBScreenHelper.Render10x10Grid();
				}
				else
				{
					VBScreenHelper.Render();
				}
				val.End();
				switch (i)
				{
				case 0:
					radialSceneMapTexture.Resolve();
					if (radialSceneMap != null)
					{
						radialSceneMap.SetValue((Texture)(object)radialSceneMapTexture.XnaTexture);
					}
					effect.CommitChanges();
					break;
				case 1:
					PostScreenMenu.downsampleMapTexture.Resolve();
					if (downsampleMap != null)
					{
						downsampleMap.SetValue((Texture)(object)PostScreenMenu.downsampleMapTexture.XnaTexture);
					}
					effect.CommitChanges();
					break;
				case 2:
					PostScreenMenu.blurMap1Texture.Resolve();
					if (blurMap1 != null)
					{
						blurMap1.SetValue((Texture)(object)PostScreenMenu.blurMap1Texture.XnaTexture);
					}
					effect.CommitChanges();
					break;
				case 3:
					PostScreenMenu.blurMap2Texture.Resolve();
					if (blurMap2 != null)
					{
						blurMap2.SetValue((Texture)(object)PostScreenMenu.blurMap2Texture.XnaTexture);
					}
					effect.CommitChanges();
					break;
				}
			}
		}
		finally
		{
			effect.End();
			BaseGame.Device.RenderState.DepthBufferEnable = true;
			BaseGame.Device.RenderState.DepthBufferWriteEnable = true;
		}
	}
}
