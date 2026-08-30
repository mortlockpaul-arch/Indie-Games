namespace Kobingo.Xna.Library.Game;

public class PlayScreen : GameScreen
{
	public GameMenu GameMenu { get; set; }

	public PlayScreen(ScreenManager screenManager)
		: base(screenManager)
	{
		GameMenu = new GameMenu(screenManager);
	}

	public override void HandleInput()
	{
		if (ScreenInput.Pause)
		{
			GameMenu.Show();
		}
		base.HandleInput();
	}
}
