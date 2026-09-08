using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework;

public class GraphicsDeviceInformation
{
	private GraphicsAdapter adapter = GraphicsAdapter.DefaultAdapter;

	public GraphicsAdapter Adapter
	{
		get
		{
			return adapter;
		}
		set
		{
			if (adapter == null)
			{
				throw new ArgumentNullException("value", "Adapter cannot be null.  Try using GraphicsAdapter.DefaultAdapter instead.");
			}
			adapter = value;
		}
	}

	public GraphicsProfile GraphicsProfile { get; set; }

	public PresentationParameters PresentationParameters { get; set; }

	public GraphicsDeviceInformation()
	{
		PresentationParameters = new PresentationParameters();
	}

	public override bool Equals(object obj)
	{
		return obj is GraphicsDeviceInformation graphicsDeviceInformation && graphicsDeviceInformation.adapter.Equals(adapter) && graphicsDeviceInformation.GraphicsProfile == GraphicsProfile && graphicsDeviceInformation.PresentationParameters.BackBufferWidth == PresentationParameters.BackBufferWidth && graphicsDeviceInformation.PresentationParameters.BackBufferHeight == PresentationParameters.BackBufferHeight && graphicsDeviceInformation.PresentationParameters.BackBufferFormat == PresentationParameters.BackBufferFormat && graphicsDeviceInformation.PresentationParameters.DepthStencilFormat == PresentationParameters.DepthStencilFormat && graphicsDeviceInformation.PresentationParameters.MultiSampleCount == PresentationParameters.MultiSampleCount && graphicsDeviceInformation.PresentationParameters.DisplayOrientation == PresentationParameters.DisplayOrientation && graphicsDeviceInformation.PresentationParameters.PresentationInterval == PresentationParameters.PresentationInterval && graphicsDeviceInformation.PresentationParameters.RenderTargetUsage == PresentationParameters.RenderTargetUsage && graphicsDeviceInformation.PresentationParameters.DeviceWindowHandle == PresentationParameters.DeviceWindowHandle && graphicsDeviceInformation.PresentationParameters.IsFullScreen == PresentationParameters.IsFullScreen;
	}

	public override int GetHashCode()
	{
		return GraphicsProfile.GetHashCode() ^ adapter.GetHashCode() ^ PresentationParameters.BackBufferWidth.GetHashCode() ^ PresentationParameters.BackBufferHeight.GetHashCode() ^ PresentationParameters.BackBufferFormat.GetHashCode() ^ PresentationParameters.DepthStencilFormat.GetHashCode() ^ PresentationParameters.MultiSampleCount.GetHashCode() ^ PresentationParameters.DisplayOrientation.GetHashCode() ^ PresentationParameters.PresentationInterval.GetHashCode() ^ PresentationParameters.RenderTargetUsage.GetHashCode() ^ ((IntPtr)PresentationParameters.DeviceWindowHandle).GetHashCode() ^ PresentationParameters.IsFullScreen.GetHashCode();
	}

	public GraphicsDeviceInformation Clone()
	{
		return new GraphicsDeviceInformation
		{
			Adapter = Adapter,
			GraphicsProfile = GraphicsProfile,
			PresentationParameters = PresentationParameters.Clone()
		};
	}
}
