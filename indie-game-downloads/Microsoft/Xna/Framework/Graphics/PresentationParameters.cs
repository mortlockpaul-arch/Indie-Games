using System;

namespace Microsoft.Xna.Framework.Graphics;

[Serializable]
public class PresentationParameters
{
	internal FNA3D.FNA3D_PresentationParameters parameters;

	public SurfaceFormat BackBufferFormat
	{
		get
		{
			return parameters.backBufferFormat;
		}
		set
		{
			parameters.backBufferFormat = value;
		}
	}

	public int BackBufferHeight
	{
		get
		{
			return parameters.backBufferHeight;
		}
		set
		{
			parameters.backBufferHeight = value;
		}
	}

	public int BackBufferWidth
	{
		get
		{
			return parameters.backBufferWidth;
		}
		set
		{
			parameters.backBufferWidth = value;
		}
	}

	public Rectangle Bounds => new Rectangle(0, 0, BackBufferWidth, BackBufferHeight);

	public nint DeviceWindowHandle
	{
		get
		{
			return FNAPlatform.UnwrapWindow(parameters.deviceWindowHandle);
		}
		set
		{
			parameters.deviceWindowHandle = FNAPlatform.WrapWindow(value);
		}
	}

	public DepthFormat DepthStencilFormat
	{
		get
		{
			return parameters.depthStencilFormat;
		}
		set
		{
			parameters.depthStencilFormat = value;
		}
	}

	public bool IsFullScreen
	{
		get
		{
			return parameters.isFullScreen == 1;
		}
		set
		{
			parameters.isFullScreen = (byte)(value ? 1u : 0u);
		}
	}

	public int MultiSampleCount
	{
		get
		{
			return parameters.multiSampleCount;
		}
		set
		{
			parameters.multiSampleCount = value;
		}
	}

	public PresentInterval PresentationInterval
	{
		get
		{
			return parameters.presentationInterval;
		}
		set
		{
			parameters.presentationInterval = value;
		}
	}

	public DisplayOrientation DisplayOrientation
	{
		get
		{
			return parameters.displayOrientation;
		}
		set
		{
			parameters.displayOrientation = value;
		}
	}

	public RenderTargetUsage RenderTargetUsage
	{
		get
		{
			return parameters.renderTargetUsage;
		}
		set
		{
			parameters.renderTargetUsage = value;
		}
	}

	public PresentationParameters()
	{
		BackBufferFormat = SurfaceFormat.Color;
		BackBufferWidth = 0;
		BackBufferHeight = 0;
		DeviceWindowHandle = IntPtr.Zero;
		IsFullScreen = true;
		DepthStencilFormat = DepthFormat.None;
		MultiSampleCount = 0;
		PresentationInterval = PresentInterval.Default;
		DisplayOrientation = DisplayOrientation.Default;
		RenderTargetUsage = RenderTargetUsage.DiscardContents;
	}

	public PresentationParameters Clone()
	{
		return new PresentationParameters
		{
			BackBufferFormat = BackBufferFormat,
			BackBufferHeight = BackBufferHeight,
			BackBufferWidth = BackBufferWidth,
			DeviceWindowHandle = DeviceWindowHandle,
			IsFullScreen = IsFullScreen,
			DepthStencilFormat = DepthStencilFormat,
			MultiSampleCount = MultiSampleCount,
			PresentationInterval = PresentationInterval,
			DisplayOrientation = DisplayOrientation,
			RenderTargetUsage = RenderTargetUsage
		};
	}
}
