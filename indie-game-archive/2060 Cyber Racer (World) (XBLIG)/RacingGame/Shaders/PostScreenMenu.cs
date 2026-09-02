using System;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Graphics;

namespace RacingGame.Shaders;

public class PostScreenMenu : ShaderEffect
{
	private const string Filename = "PostScreenMenu.fx";

	protected EffectParameter windowSize;

	protected EffectParameter sceneMap;

	protected EffectParameter downsampleMap;

	protected EffectParameter blurMap1;

	protected EffectParameter blurMap2;

	protected EffectParameter noiseMap;

	protected EffectParameter timer;

	protected static RenderToTexture sceneMapTexture;

	protected static RenderToTexture downsampleMapTexture;

	protected static RenderToTexture blurMap1Texture;

	protected static RenderToTexture blurMap2Texture;

	private Texture noiseMapTexture;

	protected static bool started = false;

	public static bool Started => started;

	protected PostScreenMenu(string shaderFilename)
		: base(shaderFilename)
	{
		if (sceneMapTexture == null)
		{
			sceneMapTexture = new RenderToTexture(RenderToTexture.SizeType.FullScreen);
		}
		if (downsampleMapTexture == null)
		{
			downsampleMapTexture = new RenderToTexture(RenderToTexture.SizeType.QuarterScreen);
		}
		if (blurMap1Texture == null)
		{
			blurMap1Texture = new RenderToTexture(RenderToTexture.SizeType.QuarterScreen);
		}
		if (blurMap2Texture == null)
		{
			blurMap2Texture = new RenderToTexture(RenderToTexture.SizeType.QuarterScreen);
		}
	}

	public PostScreenMenu()
		: this("PostScreenMenu.fx")
	{
	}

	protected override void GetParameters()
	{
		if (effect != null)
		{
			windowSize = effect.Parameters["windowSize"];
			sceneMap = effect.Parameters["sceneMap"];
			if (windowSize == null || sceneMap == null)
			{
				throw new NotSupportedException("windowSize and sceneMap must be valid in PostScreenShader=PostScreenMenu.fx");
			}
			downsampleMap = effect.Parameters["downsampleMap"];
			blurMap1 = effect.Parameters["blurMap1"];
			blurMap2 = effect.Parameters["blurMap2"];
			timer = effect.Parameters["Timer"];
			noiseMap = effect.Parameters["noiseMap"];
			noiseMapTexture = new Texture("Noise128x128.dds");
			noiseMap.SetValue((Texture)(object)noiseMapTexture.XnaTexture);
		}
	}

	public void Start()
	{
		if (sceneMapTexture != null && effect != null && !started && BaseGame.UsePostScreenShaders)
		{
			BaseGame.SetRenderTarget(sceneMapTexture.RenderTarget, isSceneRenderTarget: true);
			started = true;
		}
	}

	public virtual void Show()
	{
		if (sceneMapTexture == null || !base.Valid || !started)
		{
			return;
		}
		started = false;
		sceneMapTexture.Resolve();
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
		if (timer != null)
		{
			timer.SetValue(BaseGame.TotalTime + 0.75f);
		}
		effect.CurrentTechnique = effect.Techniques["ScreenGlow20"];
		if (effect.CurrentTechnique.Passes.Count != 4)
		{
			throw new InvalidOperationException("This shader should have exactly 4 passes!");
		}
		try
		{
			effect.Begin();
			for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
			{
				switch (i)
				{
				case 0:
					downsampleMapTexture.SetRenderTarget();
					break;
				case 1:
					blurMap1Texture.SetRenderTarget();
					break;
				case 2:
					blurMap2Texture.SetRenderTarget();
					break;
				default:
					BaseGame.ResetRenderTarget(fullResetToBackBuffer: true);
					break;
				}
				EffectPass val = effect.CurrentTechnique.Passes[i];
				val.Begin();
				VBScreenHelper.Render();
				val.End();
				switch (i)
				{
				case 0:
					downsampleMapTexture.Resolve();
					if (downsampleMap != null)
					{
						downsampleMap.SetValue((Texture)(object)downsampleMapTexture.XnaTexture);
					}
					effect.CommitChanges();
					break;
				case 1:
					blurMap1Texture.Resolve();
					if (blurMap1 != null)
					{
						blurMap1.SetValue((Texture)(object)blurMap1Texture.XnaTexture);
					}
					effect.CommitChanges();
					break;
				case 2:
					blurMap2Texture.Resolve();
					if (blurMap2 != null)
					{
						blurMap2.SetValue((Texture)(object)blurMap2Texture.XnaTexture);
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
