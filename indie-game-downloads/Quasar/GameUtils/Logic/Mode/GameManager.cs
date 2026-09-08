namespace Quasar.GameUtils.Logic.Mode;

public static class GameManager
{
	private static GameSetup currentSetup;

	private static PersistentGameData persistentData;

	public static GameSetup CurrentSetup => currentSetup;

	public static int GameMode
	{
		get
		{
			if (currentSetup == null)
			{
				return 0;
			}
			return CurrentSetup.GameMode;
		}
	}

	public static PersistentGameData PersistentData => persistentData;

	public static GameData CurrentGame
	{
		get
		{
			if (persistentData != null)
			{
				return persistentData.CurrentGame;
			}
			return null;
		}
	}

	public static void SetSetup(GameSetup setup)
	{
		currentSetup = setup;
	}

	public static void StartGame()
	{
		persistentData = currentSetup.SetupPersistentData();
	}

	public static void PrepareNextRound()
	{
		if (persistentData != null)
		{
			persistentData.PrepareNextRound();
		}
	}

	public static void Clear()
	{
		if (persistentData != null)
		{
			persistentData.Dispose();
		}
		persistentData = null;
		if (currentSetup != null)
		{
			currentSetup.Dispose();
		}
		currentSetup = null;
	}
}
