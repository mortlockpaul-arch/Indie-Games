using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Quasar.GameUtils.XBLIG.Network;

namespace Quasar.GameUtils.XBLIG.Avatar;

public static class AvatarUtils
{
	public static AvatarDescription GetAvatarDescription(PlayerIndex playerIndex)
	{
		return AvatarDescription.CreateRandom();
	}

	public static AvatarDescription GetAvatarDescription(XBLIGNetworkGamer gamer, out bool isRandom)
	{
		isRandom = false;
		return AvatarDescription.CreateRandom();
	}
}
