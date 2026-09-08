using Microsoft.Xna.Framework;

namespace Eyehook.Framework;

public class GamerManager
{
	private PlayerIndex playerIndex;

	~GamerManager()
	{
	}

	public GamerManager(PlayerIndex playerIndex)
	{
		this.playerIndex = playerIndex;
	}

	public void Initialize()
	{
	}

	public void dispose()
	{
	}

	public static bool HasStorageAccess(PlayerIndex playerIndex)
	{
		return true;
	}

	private void updateSignedInGamer()
	{
	}
}
