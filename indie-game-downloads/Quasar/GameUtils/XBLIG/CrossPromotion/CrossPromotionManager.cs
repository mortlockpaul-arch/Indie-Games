using Microsoft.Xna.Framework;

namespace Quasar.GameUtils.XBLIG.CrossPromotion;

public class CrossPromotionManager
{
	private static readonly CrossPromotionManager instance = new CrossPromotionManager();

	public static CrossPromotionManager Instance => instance;

	public static void Init()
	{
	}

	public bool IsUnlocked(PlayerIndex playerIndex, string gameId)
	{
		return false;
	}

	public string GetGameName(string gameId)
	{
		return gameId ?? string.Empty;
	}
}
