using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Maximinus;

public class Trial
{
	public static bool DebugSwitch;

	public static bool DebugTestCanPurchase;

	public static bool IsTrial
	{
		get
		{
			if (!DebugSwitch)
			{
				return Guide.IsTrialMode;
			}
			return true;
		}
	}

	public static bool UserCanPurchase(PlayerIndex playerIndex)
	{
		if (DebugTestCanPurchase)
		{
			return true;
		}
		try
		{
			SignedInGamer signedInGamer = Gamer.SignedInGamers[playerIndex];
			if (signedInGamer == null)
			{
				return false;
			}
			return signedInGamer.Privileges?.AllowPurchaseContent ?? false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool IsTrialAndCanPurchase(PlayerIndex playerIndex)
	{
		if (IsTrial)
		{
			return UserCanPurchase(playerIndex);
		}
		return false;
	}
}
