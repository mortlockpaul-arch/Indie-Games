using System.Collections.Generic;
using Scene;

namespace BabyMakerExtreme2;

public static class MasterOfUnlocking
{
	private static List<bool> m_powerupsAvailable;

	private static List<bool> m_outfitsAvailable;

	private static List<bool> m_modesAvailable;

	public static void Init(List<bool> powerupUnlocksAvailable, List<bool> outfitUnlocksAvailable, List<bool> levelUnlocksAvailable)
	{
		m_powerupsAvailable = powerupUnlocksAvailable;
		m_outfitsAvailable = outfitUnlocksAvailable;
		m_modesAvailable = levelUnlocksAvailable;
	}

	public static void LoadDefault(List<bool> powerups, List<bool> outfits, List<bool> modes)
	{
		for (int i = 0; i < 8; i++)
		{
			powerups.Add(item: false);
		}
		powerups[0] = true;
		powerups[1] = true;
		for (int j = 0; j < 25; j++)
		{
			outfits.Add(item: false);
		}
		for (int k = 0; k < 7; k++)
		{
			modes.Add(item: false);
		}
		modes[0] = true;
	}

	public static bool IsPowerupAvail(int i)
	{
		return m_powerupsAvailable[i];
	}

	public static bool IsOutfitAvail(int i)
	{
		return m_outfitsAvailable[i];
	}

	public static bool IsModeAvail(int i)
	{
		return m_modesAvailable[i];
	}

	public static void GetNewAvailablePowerups(List<string> text, List<int> unlocks, SceneContainer scene)
	{
		for (int i = 0; i < m_powerupsAvailable.Count; i++)
		{
			if (!m_powerupsAvailable[i] && CheckPowerupConditions(i, scene))
			{
				m_powerupsAvailable[i] = true;
				text.Add("You've unlocked powerup " + GetPowerupName(i));
				unlocks.Add(i);
			}
		}
	}

	private static bool CheckPowerupConditions(int index, SceneContainer scene)
	{
		int totalDist = SaveManager.GetSavedData().TotalDist;
		return index switch
		{
			2 => totalDist > 3000, 
			3 => totalDist > 6500, 
			4 => totalDist > 11000, 
			5 => totalDist > 16000, 
			6 => totalDist > 20000, 
			7 => totalDist > 26000, 
			_ => totalDist > index * 2000, 
		};
	}

	public static string GetPowerupName(int index)
	{
		return index switch
		{
			0 => "Baby", 
			1 => "Avatar Baby", 
			2 => "Mini Baby", 
			3 => "Ball Launcher Baby", 
			4 => "Triple Boost Baby", 
			5 => "Big Baby", 
			6 => "Fairy Baby", 
			7 => "Jetpack Baby", 
			_ => "Baby", 
		};
	}

	public static string GetPowerupConditionString(int index)
	{
		return index switch
		{
			2 => "Travel a total of \n3000 feet", 
			3 => "Travel a total of \n6500 feet", 
			4 => "Travel a total of \n11000 feet", 
			5 => "Travel a total of \n16000 feet", 
			6 => "Travel a total of \n20000 feet", 
			7 => "Travel a total of \n26000 feet", 
			_ => "Travel a total of \n" + index * 2000 + " feet", 
		};
	}

	public static string GetPowerupDescription(int index)
	{
		return "";
	}

	public static void GetNewAvailableOutfits(List<string> text, List<int> unlocks, SceneContainer scene)
	{
		for (int i = 0; i < m_outfitsAvailable.Count; i++)
		{
			if (!m_outfitsAvailable[i] && CheckOutfitConditions(i, scene))
			{
				m_outfitsAvailable[i] = true;
				text.Add("You've unlocked outfit " + GetOutfitName(i));
				unlocks.Add(i);
			}
		}
	}

	private static bool CheckOutfitConditions(int index, SceneContainer scene)
	{
		int score = scene.GetPlayer().GetScore();
		int distanceTravelled = scene.GetPlayer().DistanceTravelled;
		bool flag = scene.GetSceneObjectSpawner().IsWorldInf();
		int worldType = scene.GetSceneObjectSpawner().GetWorldType();
		_ = SaveManager.GetSavedData().TotalDist;
		switch (index)
		{
		case 4:
			if (!flag)
			{
				return score > 100;
			}
			return false;
		case 11:
			if (!flag)
			{
				return distanceTravelled > 100;
			}
			return false;
		case 0:
			if (!flag)
			{
				return distanceTravelled > 150;
			}
			return false;
		case 16:
			if (!flag)
			{
				return distanceTravelled > 400;
			}
			return false;
		case 2:
			if (!flag)
			{
				return score > 700;
			}
			return false;
		case 7:
			if (!flag)
			{
				return distanceTravelled > 500;
			}
			return false;
		case 14:
			if (!flag)
			{
				return distanceTravelled > 750;
			}
			return false;
		case 18:
			if (!flag)
			{
				return distanceTravelled > 900;
			}
			return false;
		case 17:
			if (!flag)
			{
				return score > 1400;
			}
			return false;
		case 12:
			if (!flag)
			{
				return score > 1700;
			}
			return false;
		case 13:
			if (!flag)
			{
				return distanceTravelled > 1000;
			}
			return false;
		case 8:
			if (!flag)
			{
				return score > 2000;
			}
			return false;
		case 20:
			if (!flag)
			{
				return distanceTravelled > 1300;
			}
			return false;
		case 9:
			if (!flag)
			{
				return score > 2500;
			}
			return false;
		case 3:
			if (!flag)
			{
				return distanceTravelled > 1600;
			}
			return false;
		case 24:
			if (!flag)
			{
				return score > 3500;
			}
			return false;
		case 19:
			if (!flag)
			{
				return distanceTravelled > 2000;
			}
			return false;
		case 1:
			if (flag && worldType == 0)
			{
				return distanceTravelled > 500;
			}
			return false;
		case 21:
			if (flag && worldType == 0)
			{
				return distanceTravelled > 800;
			}
			return false;
		case 6:
			if (flag && worldType == 1)
			{
				return distanceTravelled > 1000;
			}
			return false;
		case 15:
			if (flag && worldType == 1)
			{
				return score > 1500;
			}
			return false;
		case 23:
			if (flag && worldType == 2)
			{
				return distanceTravelled > 2000;
			}
			return false;
		case 10:
			if (flag && worldType == 3)
			{
				return distanceTravelled > 900;
			}
			return false;
		case 22:
			if (flag && worldType == 3)
			{
				return score > 1300;
			}
			return false;
		case 5:
			if (flag && worldType == 3)
			{
				return score > 2500;
			}
			return false;
		default:
			return false;
		}
	}

	public static string GetOutfitName(int index)
	{
		return index switch
		{
			0 => "Denim Jacket", 
			1 => "Winter Vest", 
			2 => "Skull Shirt", 
			3 => "Tutu", 
			4 => "Diaper", 
			5 => "Barrel", 
			6 => "Suit", 
			7 => "Bowler Hat", 
			8 => "Antenae", 
			9 => "Spinner Cap", 
			10 => "Construction Helmet", 
			11 => "Helmet", 
			12 => "Elvis Hair", 
			13 => "Headphones", 
			14 => "Medieval Helm", 
			15 => "Coonskin Cap", 
			16 => "Cheap Shoes", 
			17 => "Clown Shoes", 
			18 => "Knee Boots", 
			19 => "Socks and Sandals", 
			20 => "Cleats", 
			21 => "Fingerless Gloves", 
			22 => "Arm Bands", 
			23 => "Muscles", 
			24 => "Tattoos", 
			_ => "", 
		};
	}

	public static string GetOutfitConditionString(int index)
	{
		return index switch
		{
			4 => "Score 100 points in\none turn in normal\nplay", 
			11 => "Travel 100 feet in\none turn in normal\nplay", 
			0 => "Travel 150 feet in\none turn in normal\nplay", 
			16 => "Travel 400 feet in\none turn in normal\nplay", 
			2 => "Score 700 points in\none turn in normal\nplay", 
			7 => "Travel 500 feet in\none turn in normal\nplay", 
			14 => "Travel 750 feet in\none turn in normal\nplay", 
			18 => "Travel 900 feet in\none turn in normal\nplay", 
			17 => "Score 1400 points in\none turn in normal\nplay", 
			12 => "Score 1700 points in\none turn in normal\nplay", 
			13 => "Travel 1000 feet in\none turn in normal\nplay", 
			8 => "Score 2000 points in\none turn in normal\nplay", 
			20 => "Travel 1300 feet in\none turn in normal\nplay", 
			9 => "Score 2500 points in\none turn in normal\nplay", 
			3 => "Travel 1600 feet in\none turn in normal\nplay", 
			24 => "Score 3500 points in\none turn in normal\nplay", 
			19 => "Travel 2000 feet in\none turn in normal\nplay", 
			1 => "Travel 500 feet in\none turn in Infinite\nHospital", 
			21 => "Travel 800 feet in\none turn in Infinite\nHospital", 
			6 => "Travel 1000 feet in\none turn in Infinite\nPark", 
			15 => "Score 1500 points in\none turn in Infinite\nPark", 
			23 => "Travel 2000 feet in\none turn in Infinite\nMall", 
			10 => "Travel 900 feet in\none turn in Virtual\nBaby Maker", 
			22 => "Score 1300 points in\none turn in Virtual\nBaby Maker", 
			5 => "Score 2500 points in\none turn in Virtual\nBaby Maker", 
			_ => "", 
		};
	}

	public static string GetOutfitDescription(int index)
	{
		return "";
	}

	public static void GetNewAvailableModes(List<string> text, List<int> unlocks, SceneContainer scene)
	{
		for (int i = 0; i < m_modesAvailable.Count; i++)
		{
			if (!m_modesAvailable[i] && CheckModeConditions(i, scene))
			{
				m_modesAvailable[i] = true;
				text.Add("You've unlocked mode " + GetModeName(i));
				unlocks.Add(i);
			}
		}
	}

	private static bool CheckModeConditions(int index, SceneContainer scene)
	{
		int num = scene.SceneType();
		if (!scene.GetSceneObjectSpawner().IsWorldInf())
		{
			switch (index)
			{
			case 0:
				return true;
			case 1:
				return num > 0;
			case 2:
				return num > 0;
			case 3:
				return num > 1;
			case 4:
				return num > 1;
			case 5:
				return num > 2;
			case 6:
				return num > 0;
			}
		}
		return false;
	}

	public static string GetModeName(int index)
	{
		return index switch
		{
			0 => "Hospital", 
			1 => "Infinite Hospital", 
			2 => "Public Park", 
			3 => "Infinite Park", 
			4 => "Shopping Mall", 
			5 => "Infinite Mall", 
			6 => "Virtual Baby Maker", 
			_ => "nil", 
		};
	}

	public static string GetModeConditionString(int index)
	{
		if (index == 0)
		{
			return "none";
		}
		if (index <= 2)
		{
			return "Complete the Hospital zone";
		}
		if (index <= 4)
		{
			return "Complete the Public Park zone";
		}
		if (index <= 6)
		{
			return "Complete the Mall zone";
		}
		return "Score > " + index * 3000;
	}

	public static string GetModeDescription(int index)
	{
		return index switch
		{
			0 => "You can start the game in the Hospital", 
			1 => "You can fly in an infinitely long hospital", 
			2 => "You can start the game in the Parks", 
			3 => "You can fly in an infinitely long Park", 
			4 => "You can start the game in the Mall", 
			5 => "You can fly in an infinitely long Mall", 
			6 => "You can play in the Virtual Baby Maker", 
			_ => "nil", 
		};
	}
}
