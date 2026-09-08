using System;
using System.Threading;

namespace Microsoft.Xna.Framework.Graphics;

public class OcclusionQuery : GraphicsResource
{
	private nint query;

	public bool IsComplete => FNA3D.FNA3D_QueryComplete(base.GraphicsDevice.GLDevice, query) == 1;

	public int PixelCount => FNA3D.FNA3D_QueryPixelCount(base.GraphicsDevice.GLDevice, query);

	public OcclusionQuery(GraphicsDevice graphicsDevice)
	{
		base.GraphicsDevice = graphicsDevice;
		query = FNA3D.FNA3D_CreateQuery(base.GraphicsDevice.GLDevice);
	}

	protected override void Dispose(bool disposing)
	{
		if (!base.IsDisposed)
		{
			nint num = Interlocked.Exchange(ref query, IntPtr.Zero);
			if (num != IntPtr.Zero)
			{
				FNA3D.FNA3D_AddDisposeQuery(base.GraphicsDevice.GLDevice, num);
			}
		}
		base.Dispose(disposing);
	}

	public void Begin()
	{
		FNA3D.FNA3D_QueryBegin(base.GraphicsDevice.GLDevice, query);
	}

	public void End()
	{
		FNA3D.FNA3D_QueryEnd(base.GraphicsDevice.GLDevice, query);
	}
}
