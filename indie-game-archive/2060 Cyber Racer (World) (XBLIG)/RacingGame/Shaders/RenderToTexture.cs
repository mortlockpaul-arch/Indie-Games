using System;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Graphics;

namespace RacingGame.Shaders;

public class RenderToTexture : Texture
{
	public enum SizeType
	{
		FullScreen,
		HalfScreen,
		QuarterScreen,
		ShadowMap
	}

	private RenderTarget2D renderTarget;

	private DepthStencilBuffer zBufferSurface;

	private SizeType sizeType;

	private bool usesHighPercisionFormat;

	private static int RenderToTextureGlobalInstanceId;

	private bool alreadyResolved;

	public DepthStencilBuffer ZBufferSurface => zBufferSurface;

	public RenderTarget2D RenderTarget => renderTarget;

	public override Texture2D XnaTexture
	{
		get
		{
			if (alreadyResolved)
			{
				internalXnaTexture = renderTarget.GetTexture();
			}
			return internalXnaTexture;
		}
	}

	public bool UsesHighPercisionFormat => usesHighPercisionFormat;

	private void CalcSize()
	{
		switch (sizeType)
		{
		case SizeType.FullScreen:
			texWidth = BaseGame.Width;
			texHeight = BaseGame.Height;
			break;
		case SizeType.HalfScreen:
			texWidth = BaseGame.Width / 2;
			texHeight = BaseGame.Height / 2;
			break;
		case SizeType.QuarterScreen:
			texWidth = BaseGame.Width / 4;
			texHeight = BaseGame.Height / 4;
			break;
		case SizeType.ShadowMap:
			if (BaseGame.HighDetail)
			{
				texWidth = 2048;
				texHeight = 2048;
			}
			else
			{
				texWidth = 1024;
				texHeight = 1024;
			}
			break;
		}
		CalcHalfPixelSize();
	}

	public RenderToTexture(SizeType setSizeType)
	{
		sizeType = setSizeType;
		CalcSize();
		texFilename = "RenderToTexture instance " + RenderToTextureGlobalInstanceId++;
		Create();
		BaseGame.AddRemRenderToTexture(this);
	}

	public void HandleDeviceReset()
	{
		CalcSize();
		alreadyResolved = false;
		internalXnaTexture = null;
		Create();
	}

	private static bool CheckRenderTargetFormat(SurfaceFormat format)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		GraphicsDeviceCreationParameters creationParameters = BaseGame.Device.CreationParameters;
		GraphicsAdapter adapter = ((GraphicsDeviceCreationParameters)(ref creationParameters)).Adapter;
		GraphicsDeviceCreationParameters creationParameters2 = BaseGame.Device.CreationParameters;
		DeviceType deviceType = ((GraphicsDeviceCreationParameters)(ref creationParameters2)).DeviceType;
		DisplayMode displayMode = BaseGame.Device.DisplayMode;
		return adapter.CheckDeviceFormat(deviceType, ((DisplayMode)(ref displayMode)).Format, (TextureUsage)0, (QueryUsages)0, (ResourceType)8, format);
	}

	private void Create()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		SurfaceFormat val = (SurfaceFormat)1;
		if (sizeType == SizeType.ShadowMap)
		{
			if (CheckRenderTargetFormat((SurfaceFormat)22))
			{
				val = (SurfaceFormat)22;
			}
			else if (CheckRenderTargetFormat((SurfaceFormat)25))
			{
				val = (SurfaceFormat)25;
			}
			else if (CheckRenderTargetFormat((SurfaceFormat)26))
			{
				val = (SurfaceFormat)26;
			}
			else if (CheckRenderTargetFormat((SurfaceFormat)34))
			{
				val = (SurfaceFormat)34;
			}
		}
		MultiSampleType val2 = (MultiSampleType)2;
		if (BaseGame.Device.PresentationParameters.BackBufferHeight == 720)
		{
			val2 = (MultiSampleType)4;
		}
		if (sizeType == SizeType.ShadowMap || BaseGame.CurrentPlatform == PlatformID.Win32NT)
		{
			val2 = (MultiSampleType)0;
		}
		renderTarget = new RenderTarget2D(BaseGame.Device, texWidth, texHeight, 1, val, val2, 0, (RenderTargetUsage)2);
		if ((int)val != 1)
		{
			usesHighPercisionFormat = true;
		}
		if (sizeType == SizeType.ShadowMap && (texWidth > BaseGame.Width || texHeight > BaseGame.Height))
		{
			zBufferSurface = new DepthStencilBuffer(BaseGame.Device, texWidth, texHeight, BaseGame.BackBufferDepthFormat, (MultiSampleType)0, 0);
		}
		loaded = true;
	}

	public void Clear(Color clearColor)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (loaded && renderTarget != null)
		{
			BaseGame.Device.Clear((ClearOptions)3, clearColor, 1f, 0);
		}
	}

	public bool SetRenderTarget()
	{
		if (!loaded || renderTarget == null)
		{
			return false;
		}
		BaseGame.SetRenderTarget(renderTarget, isSceneRenderTarget: false);
		return true;
	}

	public void Resolve()
	{
		if (BaseGame.CurrentRenderTarget != renderTarget)
		{
			throw new InvalidOperationException("You can't call Resolve without first setting the render target!");
		}
		alreadyResolved = true;
		BaseGame.Device.SetRenderTarget(0, (RenderTarget2D)null);
	}
}
