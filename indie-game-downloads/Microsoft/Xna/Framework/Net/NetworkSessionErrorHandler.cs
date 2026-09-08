using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net;

internal static class NetworkSessionErrorHandler
{
	public static void ThrowExceptionFromResult(uint result)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected I4, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (result == 0)
		{
			return;
		}
		KernelReturnCode val = (KernelReturnCode)result;
		switch (val - -2147220983)
		{
		default:
			if ((int)val != -2147220972)
			{
				break;
			}
			throw new NetworkNotAvailableException(FrameworkResources.NetworkNotAvailable);
		case 0:
			throw new NetworkException(FrameworkResources.NetworkError);
		case 3:
			throw new NetworkSessionJoinException(FrameworkResources.SessionNotFound, NetworkSessionJoinError.SessionNotFound);
		case 4:
			throw new NetworkSessionJoinException(FrameworkResources.SessionNotJoinable, NetworkSessionJoinError.SessionNotJoinable);
		case 5:
			throw new NetworkSessionJoinException(FrameworkResources.SessionFull, NetworkSessionJoinError.SessionFull);
		case 1:
			throw new NetworkException(FrameworkResources.PacketQueueFull);
		case 2:
			break;
		}
		ErrorHandler.ThrowExceptionFromResult(result);
	}
}
