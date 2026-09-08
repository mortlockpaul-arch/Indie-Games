using Eyehook.Framework;
using Microsoft.Xna.Framework;

namespace Loot.Screens;

public class DeviceRemovedScreen : Screen
{
	public DeviceRemovedScreen()
		: base(modal: true)
	{
	}

	public override void update(GameTime gameTime)
	{
		throw new ResetException("Your storage device has been removed.\n\nCursed Loot requires storage access so it can save your progress and preferences.\n\nSorry, the game has been reset.");
	}
}
