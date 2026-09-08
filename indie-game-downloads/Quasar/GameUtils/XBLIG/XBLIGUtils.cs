using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Quasar.GameUtils.Game;

namespace Quasar.GameUtils.XBLIG;

public static class XBLIGUtils
{
	public static bool IsGuideVisible => Guide.IsVisible;

	public static bool IsSignedInToLive(PlayerIndex index)
	{
		return PlatformInterface.Instance.GetGamer(index)?.IsSignedInToService ?? false;
	}

	public static bool SendToFriends(PlayerIndex index, string message)
	{
		return PlatformInterface.Instance.SendToFriends(index, message);
	}
}
