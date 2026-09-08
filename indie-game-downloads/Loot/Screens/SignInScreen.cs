using Eyehook.Framework;
using Loot.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class SignInScreen : Screen
{
	private Texture2D pixel;

	public SignInScreen()
		: base(modal: true)
	{
	}

	public override void loadContent(ContentManager content)
	{
		pixel = content.Load<Texture2D>("Sprites\\Pixel");
	}

	public override void update(GameTime gameTime)
	{
		if (GamePadManager.scanForButtonDown(Buttons.A).HasValue)
		{
			MC.ScreenManager.removeScreen(this);
		}
	}

	private void showSignIn()
	{
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		base.spriteBatch.Draw(pixel, base.viewportRect, MC.ScreenManager.BackgroundColor);
		Text.Draw(base.spriteBatch, new Vector2(188f, 260f), "Please sign in to a profile.", Color.White);
		Text.Draw(base.spriteBatch, new Vector2(188f, 340f), "Cursed Loot requires a profile so it", Color.White);
		Text.Draw(base.spriteBatch, new Vector2(188f, 380f), "can save your progress and preferences.", Color.White);
		Text.Draw(base.spriteBatch, new Vector2(188f, 460f), "Press \"A\" to Continue...", Color.White);
		base.spriteBatch.End();
	}
}
