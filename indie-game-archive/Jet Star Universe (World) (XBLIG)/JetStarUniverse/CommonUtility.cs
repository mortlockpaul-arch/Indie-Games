using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace JetStarUniverse;

public static class CommonUtility
{
	public static bool CanBuyGame(this PlayerIndex player)
	{
		return Gamer.SignedInGamers[player]?.Privileges.AllowPurchaseContent ?? false;
	}
}
