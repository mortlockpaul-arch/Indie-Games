using Microsoft.Xna.Framework;

namespace PlatformerFromHell.Asset_Classes;

internal class Platform : Asset
{
	private readonly char switchCharacterID;

	public Platform(Level level, Vector2 position, string texturename, int frameCount, Dir newFlip, char newSwitchID)
		: base(level, position, texturename, frameCount, newFlip)
	{
		switchCharacterID = newSwitchID;
	}

	public char GetSwitchID()
	{
		return switchCharacterID;
	}

	public override void LoadContent()
	{
		base.LoadContent();
	}
}
