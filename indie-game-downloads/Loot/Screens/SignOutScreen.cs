using Eyehook.Framework;
using Microsoft.Xna.Framework;

namespace Loot.Screens;

public class SignOutScreen : Screen
{
	public SignOutScreen()
		: base(modal: true)
	{
	}

	public override void update(GameTime gameTime)
	{
		throw new ResetException("You have signed out of your profile.\n\nCursed Loot requires you to be signed in so it can save your progress and preferences.\n\nSorry, the game has been reset.");
	}
}
