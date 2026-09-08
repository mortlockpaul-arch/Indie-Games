using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

internal class ControlsScreen : MenuScreen
{
	private ContentManager content;

	private Texture2D screen;

	public ControlsScreen()
		: base("", Vector2.Zero)
	{
		MenuEntry menuEntry = new MenuEntry("");
		menuEntry.Selected += base.OnCancelOptions;
		base.MenuEntries.Add(menuEntry);
	}

	private void SetMenuEntryText()
	{
	}

	public override void LoadContent()
	{
		base.LoadContent();
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		screen = content.Load<Texture2D>("Background/Controls");
	}

	private void useless(object sender, PlayerIndexEventArgs e)
	{
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
	}

	public override void HandleInput(InputState input)
	{
		base.HandleInput(input);
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
		SpriteBatch spriteBatch = new SpriteBatch(base.ScreenManager.GraphicsDevice);
		if (!base.IsExiting)
		{
			spriteBatch.Begin();
			spriteBatch.Draw(screen, Vector2.Zero, Color.White);
			spriteBatch.End();
		}
	}
}
