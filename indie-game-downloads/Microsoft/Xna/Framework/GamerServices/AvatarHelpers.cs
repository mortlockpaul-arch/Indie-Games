using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

internal static class AvatarHelpers
{
	[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
	internal static GraphicsDevice GraphicsDevice
	{
		get
		{
			if (!GamerServicesDispatcher.IsInitialized)
			{
				throw new InvalidOperationException(FrameworkResources.get_GamerServicesNotInitialized());
			}
			IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)GamerServicesDispatcher.serviceProvider.GetService(typeof(IGraphicsDeviceService));
			if (graphicsDeviceService == null)
			{
				throw new InvalidOperationException(FrameworkResources.get_NoGraphicsDevice());
			}
			if (graphicsDeviceService.GraphicsDevice == null)
			{
				throw new InvalidOperationException(FrameworkResources.get_NoGraphicsDevice());
			}
			return graphicsDeviceService.GraphicsDevice;
		}
	}
}
