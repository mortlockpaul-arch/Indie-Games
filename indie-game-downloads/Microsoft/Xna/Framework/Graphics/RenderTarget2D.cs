using System;
using System.Threading;

namespace Microsoft.Xna.Framework.Graphics;

public class RenderTarget2D : Texture2D, IRenderTarget
{
	private nint glDepthStencilBuffer;

	private nint glColorBuffer;

	public DepthFormat DepthStencilFormat { get; private set; }

	public int MultiSampleCount { get; private set; }

	public RenderTargetUsage RenderTargetUsage { get; private set; }

	public bool IsContentLost => false;

	nint IRenderTarget.DepthStencilBuffer => glDepthStencilBuffer;

	nint IRenderTarget.ColorBuffer => glColorBuffer;

	public event EventHandler<EventArgs> ContentLost;

	public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height)
		: this(graphicsDevice, width, height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents)
	{
	}

	public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat)
		: this(graphicsDevice, width, height, mipMap, preferredFormat, preferredDepthFormat, 0, RenderTargetUsage.DiscardContents)
	{
	}

	public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height, bool mipMap, SurfaceFormat preferredFormat, DepthFormat preferredDepthFormat, int preferredMultiSampleCount, RenderTargetUsage usage)
		: base(graphicsDevice, width, height, mipMap, preferredFormat)
	{
		DepthStencilFormat = preferredDepthFormat;
		MultiSampleCount = FNA3D.FNA3D_GetMaxMultiSampleCount(graphicsDevice.GLDevice, base.Format, MathHelper.ClosestMSAAPower(preferredMultiSampleCount));
		RenderTargetUsage = usage;
		if (MultiSampleCount > 0)
		{
			glColorBuffer = FNA3D.FNA3D_GenColorRenderbuffer(graphicsDevice.GLDevice, base.Width, base.Height, base.Format, MultiSampleCount, texture);
		}
		if (DepthStencilFormat != DepthFormat.None)
		{
			glDepthStencilBuffer = FNA3D.FNA3D_GenDepthStencilRenderbuffer(graphicsDevice.GLDevice, base.Width, base.Height, DepthStencilFormat, MultiSampleCount);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!base.IsDisposed)
		{
			for (int i = 0; i < base.GraphicsDevice.renderTargetCount; i++)
			{
				if (base.GraphicsDevice.renderTargetBindings[i].RenderTarget == this)
				{
					throw new InvalidOperationException("Disposing target that is still bound");
				}
			}
			nint num = Interlocked.Exchange(ref glColorBuffer, IntPtr.Zero);
			if (num != IntPtr.Zero)
			{
				FNA3D.FNA3D_AddDisposeRenderbuffer(base.GraphicsDevice.GLDevice, num);
			}
			num = Interlocked.Exchange(ref glDepthStencilBuffer, IntPtr.Zero);
			if (num != IntPtr.Zero)
			{
				FNA3D.FNA3D_AddDisposeRenderbuffer(base.GraphicsDevice.GLDevice, num);
			}
		}
		base.Dispose(disposing);
	}

	protected internal override void GraphicsDeviceResetting()
	{
		base.GraphicsDeviceResetting();
	}
}
