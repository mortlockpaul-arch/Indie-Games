using System;
using System.Collections.Generic;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;

namespace BunnyOfWar;

public static class CustomsManager
{
	public enum Customizations
	{
		undefined,
		BigBlood,
		DisableCounters,
		SkullSlingshotMode,
		GhostEnemies,
		HelpText,
		Player2IsALog,
		ZeroGravity,
		RedBaron,
		Gigantor,
		Runner,
		YoIisUnderWater,
		Driver,
		CustomPlayerAnimation,
		MoveSpeed,
		CutSceneOrQTE,
		MeteorsEasy,
		MeteorsHard,
		HelicopterMode,
		FlappyMode,
		FlappyPoliceMode,
		FlappyDifficulty,
		ShooterMode,
		SpaceMode,
		GunSmokeMode,
		CameraRoll,
		TrampolineMode,
		BrawlerXMode,
		FailBackground
	}

	public static int importCount = 0;

	public static Dictionary<Customizations, string> LevelCustomizations = new Dictionary<Customizations, string>();

	public static string GetCustomPlayerAnimation()
	{
		if (LevelCustomizations.ContainsKey(Customizations.CustomPlayerAnimation))
		{
			return LevelCustomizations[Customizations.CustomPlayerAnimation].ToString();
		}
		return "";
	}

	public static bool IsBloodEnabled()
	{
		return true;
	}

	public static bool GetIsCollidableWithCPUs()
	{
		return false;
	}

	public static bool GetIsUnderWater()
	{
		if (LevelCustomizations.ContainsKey(Customizations.YoIisUnderWater))
		{
			return true;
		}
		return false;
	}

	public static Customizations ConvertFromString(string s)
	{
		return (Customizations)Enum.Parse(typeof(Customizations), s, ignoreCase: true);
	}

	public static string ExportData()
	{
		string text = "";
		foreach (Customizations key in LevelCustomizations.Keys)
		{
			text += string.Format("type=customization;name={0};value={1}" + Environment.NewLine, key.ToString(), LevelCustomizations[key]);
		}
		return text;
	}

	public static void ImportData(string data)
	{
		ClearData();
		importCount++;
		string[] array = data.Split(Environment.NewLine.ToCharArray());
		Customizations customizations = Customizations.undefined;
		string value = "";
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].ToString().Trim();
			if (!array[i].StartsWith("type=customization"))
			{
				continue;
			}
			string[] array2 = array[i].Split(';');
			for (int j = 0; j < array2.Length; j++)
			{
				string[] array3 = array2[j].Split('=');
				if (array3[0] == "name")
				{
					customizations = (Customizations)Enum.Parse(typeof(Customizations), array3[1], ignoreCase: true);
				}
				else if (array3[0] == "value")
				{
					value = array3[1];
				}
			}
			try
			{
				if (customizations != Customizations.undefined)
				{
					LevelCustomizations.Add(customizations, value);
				}
			}
			catch (Exception)
			{
			}
		}
		ProcessCustoms();
	}

	private static void ProcessCustoms()
	{
		foreach (Customizations key in LevelCustomizations.Keys)
		{
			switch (key)
			{
			case Customizations.FailBackground:
				RandomStaticGlobals.imgFailBackground = GraphicsManager.LoadTexture(LevelCustomizations[key], cacheResult: false);
				break;
			case Customizations.BigBlood:
			{
				string text = LevelCustomizations[key];
				Definitions.BloodSplatterSize = float.Parse(text.Replace(",", "."));
				break;
			}
			case Customizations.DisableCounters:
				RandomStaticGlobals.isCounteringEnabled = false;
				break;
			case Customizations.SkullSlingshotMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.angrybirds;
				RandomStaticGlobals.isSkullSlingshotMode = true;
				break;
			case Customizations.HelpText:
				RandomStaticGlobals.HelpTextForLevel = LevelCustomizations[key];
				break;
			case Customizations.GhostEnemies:
				GraphicsManager.isDrawingEnemiesAsGhosts = true;
				break;
			case Customizations.Gigantor:
			{
				float scale = float.Parse(LevelCustomizations[key].ToString().Replace(",", "."));
				for (int l = 0; l < FighterManager.humanPlayers.Count; l++)
				{
					FighterManager.humanPlayers[l].PROPERTIES.scale = scale;
				}
				break;
			}
			case Customizations.RedBaron:
				RandomStaticGlobals.GameMode = Definitions.GameMode.redbaron;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.Runner:
				RandomStaticGlobals.GameMode = Definitions.GameMode.runner;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				FighterManager.SetJumpSpeeds(Definitions.GameMode.runner);
				break;
			case Customizations.CutSceneOrQTE:
				RandomStaticGlobals.GameMode = Definitions.GameMode.cutsceneORqte;
				break;
			case Customizations.YoIisUnderWater:
				RandomStaticGlobals.GameMode = Definitions.GameMode.swimmer;
				FighterManager.SetJumpSpeeds(Definitions.GameMode.swimmer);
				break;
			case Customizations.HelicopterMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.helicopter;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.FlappyMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.flappy;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.FlappyPoliceMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.flappychase;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.ShooterMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.shooter;
				FighterManager.SetHumanRandomThings(null, int.Parse(LevelCustomizations[key].ToString().Replace(",", ".")));
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.SpaceMode:
				RandomStaticGlobals.GameMode = Definitions.GameMode.space;
				FighterManager.SetHumanRandomThings(null, int.Parse(LevelCustomizations[key].ToString().Replace(",", ".")));
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			case Customizations.GunSmokeMode:
			{
				RandomStaticGlobals.GameMode = Definitions.GameMode.gunsmoke;
				int value = int.Parse(LevelCustomizations[key].ToString().Replace(",", "."));
				FighterManager.SetHumanRandomThings(null, 0.0, value);
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			}
			case Customizations.BrawlerXMode:
			{
				RandomStaticGlobals.GameMode = Definitions.GameMode.brawlerX;
				if (importCount > 1)
				{
					return;
				}
				if (!(LevelCustomizations[key].ToString() != "") || !(LevelCustomizations[key].ToString() != "0"))
				{
					break;
				}
				string[] array2 = LevelCustomizations[key].ToString().Split(',');
				int num5 = 7;
				num5 = ((!(array2[0] != "R")) ? DateTime.Now.Millisecond : int.Parse(array2[0]));
				int num6 = int.Parse(array2[1]);
				int num7 = int.Parse(array2[2]);
				int.Parse(array2[3]);
				int.Parse(array2[4]);
				int dogsToo = int.Parse(array2[5]);
				Random random2 = new Random(num5);
				for (int k = 2000; k < num6; k += num7)
				{
					int num8 = random2.Next(0, num7 / 2);
					if (random2.Next(0, 10) > 5)
					{
						FighterManager.AddComputerPlayer(FighterManager.createNewEnemyX(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, k + num8, 400, isAlive: true, BunnyOfWar.AI.AI.modes.X, 0.9f, dogsToo));
					}
					else
					{
						FighterManager.AddComputerPlayer(FighterManager.createNewEnemyY(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, k + num8, 300, isAlive: true, BunnyOfWar.AI.AI.modes.Y, 1.2f, dogsToo));
					}
					if (random2.Next(0, 10) > 5)
					{
						FighterManager.AddComputerPlayer(FighterManager.createNewEnemyX(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, -k - num8, 400, isAlive: true, BunnyOfWar.AI.AI.modes.X, 0.9f, dogsToo));
					}
					else
					{
						FighterManager.AddComputerPlayer(FighterManager.createNewEnemyY(RandomStaticGlobals.Content, GraphicsManager.BoundariesDefault, -k - num8, 300, isAlive: true, BunnyOfWar.AI.AI.modes.Y, 1.2f, dogsToo));
					}
				}
				break;
			}
			case Customizations.CameraRoll:
			{
				string[] array = LevelCustomizations[key].ToString().Split(',');
				RandomStaticGlobals.CameraRollVelocity = new Vector2(0f, 0f);
				if (array[0] != null)
				{
					RandomStaticGlobals.CameraRollVelocity.X = float.Parse(array[0]);
				}
				if (array[1] != null)
				{
					RandomStaticGlobals.CameraRollVelocity.Y = float.Parse(array[1]);
				}
				break;
			}
			case Customizations.MoveSpeed:
				FighterManager.SetHumanRandomThings(null, int.Parse(LevelCustomizations[key].ToString().Replace(",", ".")));
				break;
			case Customizations.Driver:
				RandomStaticGlobals.GameMode = Definitions.GameMode.driver;
				FighterManager.SetHumanRandomThings(double.Parse(LevelCustomizations[key].ToString().Replace(",", ".")), null);
				break;
			case Customizations.MeteorsEasy:
			{
				if (importCount > 1)
				{
					return;
				}
				int num2 = int.Parse(LevelCustomizations[key].ToString().Replace(",", "."));
				Random random = new Random(3);
				for (int j = 2000; j < num2; j += 1000)
				{
					int y2 = random.Next(100, 900);
					int num4 = random.Next(250, 500);
					int xRoll2 = random.Next(10, 500) * -1;
					ObstacleManager.AddMeteorInSpace(j, y2, num4, num4, xRoll2, 0);
				}
				break;
			}
			case Customizations.MeteorsHard:
			{
				if (importCount > 1)
				{
					return;
				}
				int num2 = int.Parse(LevelCustomizations[key].ToString().Replace(",", "."));
				Random random = new Random(3);
				for (int i = 2000; i < num2; i += 500)
				{
					int y = random.Next(100, 900);
					int num3 = random.Next(250, 500);
					int xRoll = random.Next(100, 500) * -1;
					ObstacleManager.AddMeteorInSpace(i, y, num3, num3, xRoll, 0);
				}
				break;
			}
			case Customizations.TrampolineMode:
			{
				RandomStaticGlobals.GameMode = Definitions.GameMode.trampoline;
				for (int num = 1010000; num > 0; num -= 1000)
				{
					ObstacleManager.AddCoin(300, num, 150, 150, 0, 0);
					ObstacleManager.AddCoin(900, num, 150, 150, 0, 0);
				}
				FighterManager.SetZeroGravityForHumans(on: true);
				break;
			}
			}
		}
	}

	public static void ClearData()
	{
		LevelCustomizations.Clear();
	}
}
