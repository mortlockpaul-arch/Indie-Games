namespace IMAK3Z0MB1EGAEM.director;

internal class GameState
{
	public enum State
	{
		Loading,
		MainMenu,
		ZombiesMenu,
		ZombiesPlaying,
		VikingMenu,
		VikingPlaying,
		EndlessZombiesMenu,
		EndlessZombiesPlaying
	}

	public static State state;
}
