using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace girlfriend;

public static class PlayerIndexExtensions
{
	public static bool CanBuyGame(this PlayerIndex player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		SignedInGamer val = Gamer.SignedInGamers[player];
		if (val == null)
		{
			return false;
		}
		if (!val.IsSignedInToLive)
		{
			return false;
		}
		return val.Privileges.AllowPurchaseContent;
	}
}
