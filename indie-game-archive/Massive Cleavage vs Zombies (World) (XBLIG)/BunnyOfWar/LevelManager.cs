using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BunnyOfWar;

public static class LevelManager
{
	public static int enemiesRemaining = 10;

	public static int maxEnemiesAtOnce = 1;

	public static int bossesRemaining = 1;

	public static int currentLevel = 1;

	public static bool isCurrentLevelActuallyALevel = true;

	public static ContentManager Content;

	public static Rectangle viewportRect;

	public static Rectangle levelBoundaries = new Rectangle(0, 0, 4000, 2000);

	private static List<string> levelNames = new List<string>(100);

	private static List<string> levels = new List<string>(100);

	public static string[] mapForBabewatchTRIAL = new string[7] { "", "01introkitchen.lvl", "02tutorial.lvl", "03grandtheftauto.lvl", "04groceryparking.lvl", "05grocery.lvl", "06dogs.lvl" };

	public static string[] mapForBabewatch = new string[39]
	{
		"", "01introkitchen.lvl", "02tutorial.lvl", "03grandtheftauto.lvl", "04groceryparking.lvl", "05grocery.lvl", "06dogs.lvl", "07RickHarrisIndustrialHallway.lvl", "07.5CoverScene.lvl", "08BobJagendorFactoryRuinst.lvl",
		"09BobJagendorfFactoryRubble.lvl", "10BobJagendorfBackAlleyway.lvl", "11DanDelucaStreet.lvl", "12JasonParisStreet.lvl", "13BobJagendorfAutoparts.lvl", "14JasonParisHouse.lvl", "15UniversityofMichiganHouse.lvl", "16LHOONFactory.lvl", "17JoseMelendezGasStation.lvl", "18RickHarrisDetroitStreetReflection.lvl",
		"HARD01introkitchen.lvl", "HARD02tutorial.lvl", "HARD03grandtheftauto.lvl", "HARD04groceryparking.lvl", "HARD05grocery.lvl", "HARD06dogs.lvl", "HARD07RickHarrisIndustrialHallway.lvl", "HARD07.5CoverScene.lvl", "HARD08BobJagendorFactoryRuinst.lvl", "HARD09BobJagendorfFactoryRubble.lvl",
		"HARD10BobJagendorfBackAlleyway.lvl", "HARD11DanDelucaStreet.lvl", "HARD12JasonParisStreet.lvl", "HARD13BobJagendorfAutoparts.lvl", "HARD14JasonParisHouse.lvl", "HARD15UniversityofMichiganHouse.lvl", "HARD16LHOONFactory.lvl", "HARD17JoseMelendezGasStation.lvl", "HARD18RickHarris.lvl"
	};

	private static bool isPreloadedAlready = false;

	public static void init(ContentManager ContentX, Rectangle viewportRectX)
	{
		Content = ContentX;
		viewportRect = viewportRectX;
	}

	public static void LoadPvPLevel(int levelNumber)
	{
		LoadLevel("PvP" + levelNumber, isPvP: true);
	}

	public static void LoadPreloadData()
	{
		if (!isPreloadedAlready)
		{
			isPreloadedAlready = true;
			PreloadThread();
		}
	}

	private static void PreloadThread()
	{
	}

	public static void LoadLevel(int levelNumber)
	{
		RandomStaticGlobals.isGamePaused = false;
		FileManager.Select360StorageDevice();
		currentLevel = levelNumber;
		LoadLevel(levelNumber.ToString(), isPvP: false);
	}

	public static void LoadNextLevel()
	{
		if (isCurrentLevelActuallyALevel)
		{
			if (RandomStaticGlobals.currentlySelectedLevel == 20 || RandomStaticGlobals.currentlySelectedLevel >= mapForBabewatch.Length)
			{
				ScreenManager.ShowCredits();
				return;
			}
			currentLevel = RandomStaticGlobals.currentlySelectedLevel;
			LoadLevel(mapForBabewatch[RandomStaticGlobals.currentlySelectedLevel], isPvP: false);
		}
	}

	public static void LoadLevel(string levelNumber, bool isPvP)
	{
		GraphicsManager.ClearTextureCache();
		RandomStaticGlobals.isPvPEnabled = isPvP;
		ResetLevelDefaults();
		GraphicsManager.IsInLetterBox = false;
		RandomStaticGlobals.setIsShowingCutScene(show: false);
		ProjectileManager.Clear();
		GraphicsManager.viewportRect = new Rectangle(0, 0, 1920, 1080);
		GraphicsManager.viewableArea = GraphicsManager.viewportRect;
		NetworkGameplayManager.Load();
		ScreenManager.UpdateLoadingStatus("Loading...");
		string level = GetLevel(levelNumber.ToString());
		ScreenManager.HidePlayerFailedScreen();
		ScreenManager.UpdateLoadingStatus("Loading Scenery");
		SceneryManager.ImportData(level);
		SceneryManager.BloodStainSceneryObjects.Clear();
		ScreenManager.UpdateLoadingStatus("Loading Fighters");
		FighterManager.ImportData(level);
		TriggerManager.ImportData(level);
		ScreenManager.UpdateLoadingStatus("Loading Obstacles");
		ObstacleManager.ImportData(level);
		if (FighterManager.humanPlayers.Count == 0)
		{
			FighterManager.ResetHumanPlayers();
			FighterManager.ResetHumanPlayersXY();
		}
		FighterManager.ResetHumanPlayers();
		CustomsManager.ImportData(level);
		if (FighterManager.humanPlayers[0].PROPERTIES.CustomAnimationName != CustomsManager.GetCustomPlayerAnimation())
		{
			FighterManager.RemakeHumanPlayersForCustoms();
		}
		if (RandomStaticGlobals.isHardMode)
		{
			FighterManager.humanPlayers[0].animationPunching.FrameTime = 0.05f;
			FighterManager.humanPlayers[0].animationQuickPunching.FrameTime = 0.02f;
		}
		else
		{
			FighterManager.humanPlayers[0].animationPunching.FrameTime = 0.1f;
			FighterManager.humanPlayers[0].animationQuickPunching.FrameTime = 0.04f;
		}
		FighterManager.ResetHumanPlayersXY();
		CustomsManager.ImportData(level);
		ScreenManager.UpdateLoadingStatus("done");
		FighterManager.ClearHighScores();
		TriggerManager.SetTriggerEvent("LevelStart");
		FighterManager.StartTimers();
		Definitions.Options.SetDifficultyModeSettings();
		if (GraphicsManager.messages != null && GraphicsManager.messages.Count > 0 && !RandomStaticGlobals.isShowingCutScene())
		{
			RandomStaticGlobals.isGamePaused = true;
		}
		else
		{
			RandomStaticGlobals.isGamePaused = false;
		}
		if (isPvP)
		{
			RandomStaticGlobals.isCounteringEnabled = false;
		}
		RandomStaticGlobals.isPvPEnabled = isPvP;
		ScreenManager.ShowBlank();
	}

	public static string GetLevel(string location)
	{
		string text = Definitions.ContentRootDirectory + "/levels/" + location;
		if (!text.EndsWith(".lvl"))
		{
			text += ".lvl";
		}
		if (levelNames.Contains(text))
		{
			return levels[levelNames.IndexOf(text)];
		}
		string text2 = "";
		using (Stream stream = TitleContainer.OpenStream(text))
		{
			StreamReader streamReader = new StreamReader(stream);
			text2 = streamReader.ReadToEnd();
			stream.Close();
		}
		levels.Add(text2);
		levelNames.Add(text);
		return text2;
	}

	public static void ResetLevelDefaults()
	{
		Definitions.Options.MasterVolumeAdjustment = 0f;
		Definitions.BloodSplatterSize = 1f;
		RandomStaticGlobals.isCounteringEnabled = true;
		RandomStaticGlobals.isSkullSlingshotMode = false;
		GraphicsManager.isDrawingEnemiesAsGhosts = false;
		GraphicsManager.ClearOverlays();
		GraphicsManager.ClearMessages();
		RandomStaticGlobals.HelpTextForLevel = "";
		RandomStaticGlobals.GameMode = Definitions.GameMode.brawler;
		RandomStaticGlobals.RollVelocity = Vector2.Zero;
		WaveManager.WaveQueue.Clear();
		TriggerManager.ClearData();
		CustomsManager.importCount = 0;
		RandomStaticGlobals.ScoreCurrent = 0;
		if (RandomStaticGlobals.currentlySelectedLevel >= 20)
		{
			RandomStaticGlobals.isHardMode = true;
		}
		else
		{
			RandomStaticGlobals.isHardMode = false;
		}
	}
}
