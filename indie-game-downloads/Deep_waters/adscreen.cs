using Microsoft.Xna.Framework;

namespace Deep_waters;

internal class adscreen : GameScreen
{
	private float timepassed;

	private bool changescreen;

	public override void LoadContent()
	{
		base.LoadContent();
		base.ScreenManager.splashsunburn();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (!base.IsActive)
		{
			return;
		}
		if (timepassed >= 3000f)
		{
			if (!changescreen)
			{
				base.ScreenManager.AddScreen(new BackgroundMenu3D("Menu", ext: true), null);
				base.ScreenManager.AddScreen(new StartScreen(), null);
				changescreen = true;
			}
		}
		else
		{
			timepassed += gameTime.ElapsedGameTime.Milliseconds;
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
