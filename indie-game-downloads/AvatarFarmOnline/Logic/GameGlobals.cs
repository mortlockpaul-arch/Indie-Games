using System;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace AvatarFarmOnline.Logic;

internal static class GameGlobals
{
	public const float TILE_SIZE = 2f;

	public const float PlayerRadius = 0.175f;

	public const float PlayerWalkSpeed = 2.75f;

	public const float PlayerRunSpeed = 5f;

	public const float MinYRotation = -1f;

	public const float MaxYRotation = 0.2f;

	public const int MinGatherTime = 86400;

	public const float GatherTimeFactor = 2f;

	public const int WitheredGatherFactor = 5;

	public const float BuildingBorder = 0.22f;

	public const int DailyCoinsReward = 500;

	public const int DailyCashReward = 1;

	public const int WateredPlantGrowthBenefit = 2;

	public const int TICKS_PER_DAY = 86400;

	public const int SeasonPeriod = 900;

	public const int SaveInterval = 300000;

	public const int PlowXp = 1;

	public const int InitialCoins = 2000;

	public const int InitialCash = 10;

	public const int MaxFarmLevel = 100;

	public const int MaxPlayerLevel = 100;

	public static readonly Int2 InitialFarmSize = new Int2(20, 20);

	public static readonly Int2 MaxFarmSize = new Int2(45, 45);

	public static readonly Vector2 InitialPosition = InitialFarmSize * 2f * 0.5f;

	public static readonly Vector2 PlayerRotateSpeed = new Vector2(1.5f, 1f);

	public static readonly Vector3 SeasonFallColor = GameMath.RGBToVector(byte.MaxValue, 215, 200);

	public static readonly Vector3 SeasonSpringColor = GameMath.RGBToVector(216, byte.MaxValue, 148);

	public static readonly Vector3 SeasonSummerColor = GameMath.RGBToVector(246, byte.MaxValue, 148);

	public static readonly Vector3 SeasonWinterColor = GameMath.RGBToVector(148, 229, byte.MaxValue);

	public static readonly AvatarFarmOnline.Logic.Money PlowPrice = new AvatarFarmOnline.Logic.Money(AvatarFarmOnline.Logic.Money.MoneyType.Coins, 10);

	public static readonly AvatarFarmOnline.Logic.Money RecyclePlowedMoney = new AvatarFarmOnline.Logic.Money(AvatarFarmOnline.Logic.Money.MoneyType.Coins, 5);

	private static int[] levelUpXp = new int[99]
	{
		100, 200, 450, 850, 1500, 2450, 3750, 5500, 7750, 10600,
		14150, 18500, 23750, 30000, 37350, 45900, 55800, 67150, 80050, 94650,
		111050, 129400, 149800, 172400, 197350, 224750, 254750, 287500, 323150, 361850,
		403750, 449000, 497750, 550150, 606350, 666500, 730750, 799300, 872300, 949900,
		1032300, 1119650, 1212100, 1309850, 1413050, 1521900, 1636550, 1757200, 1884050, 2017250,
		2157000, 2303500, 2456900, 2617400, 2785200, 2960500, 3143500, 3334350, 3533250, 3740400,
		3956000, 4180250, 4413350, 4655500, 4906900, 5167750, 5438250, 5718650, 6009150, 6309950,
		6621250, 6943250, 7276200, 7620300, 7975750, 8342800, 8721650, 9112500, 9515600, 9931150,
		10359400, 10800550, 11254800, 11722400, 12203600, 12698600, 13207650, 13730950, 14268750, 14821300,
		15388800, 15971500, 16569650, 17183450, 17813150, 18459000, 19121250, 19800100, 20495800
	};

	private static int[] playerLevelUpXp = new int[99]
	{
		175, 375, 775, 1475, 2575, 4225, 6525, 9675, 13825, 19175,
		25875, 34175, 44275, 56375, 70725, 87575, 107175, 129775, 155675, 185125,
		218425, 255875, 297775, 344425, 396125, 453225, 516025, 584875, 660125, 742125,
		831225, 927775, 1032125, 1144675, 1265775, 1395825, 1535225, 1684375, 1843675, 2013525,
		2194325, 2386525, 2590525, 2806775, 3035675, 3277675, 3533225, 3802775, 4086775, 4385675,
		4699925, 5030025, 5376425, 5739625, 6120075, 6518275, 6934725, 7369925, 7824375, 8298575,
		8793025, 9308225, 9844675, 10402925, 10983475, 11586875, 12213675, 12864375, 13539525, 14239675,
		14965375, 15717175, 16495625, 17301275, 18134725, 18996525, 19887275, 20807525, 21757875, 22738875,
		23751125, 24795225, 25871775, 26981375, 28124625, 29302125, 30514475, 31762325, 33046275, 34366925,
		35724925, 37120925, 38555525, 40029375, 41543125, 43097375, 44692775, 46329975, 48009625
	};

	public static Int2 FarmSize(int level)
	{
		return new Int2(20 + Math.Min(100, level + 2) / 4, 20 + Math.Min(100, level) / 4);
	}

	public static Vector3 SeasonColor(AvatarFarmOnline.Logic.Seasons season)
	{
		return season switch
		{
			AvatarFarmOnline.Logic.Seasons.Fall => SeasonFallColor, 
			AvatarFarmOnline.Logic.Seasons.Summer => SeasonSummerColor, 
			AvatarFarmOnline.Logic.Seasons.Winter => SeasonWinterColor, 
			_ => SeasonSpringColor, 
		};
	}

	public static int WorkDuration(AvatarFarmOnline.Logic.WorkType type)
	{
		switch (type)
		{
		case AvatarFarmOnline.Logic.WorkType.Gather:
		case AvatarFarmOnline.Logic.WorkType.GatherTree:
		case AvatarFarmOnline.Logic.WorkType.GatherBuilding:
			return 1000;
		default:
			return 1000;
		case AvatarFarmOnline.Logic.WorkType.Refill:
		case AvatarFarmOnline.Logic.WorkType.Water:
		case AvatarFarmOnline.Logic.WorkType.FeedAnimal:
			return 1000;
		case AvatarFarmOnline.Logic.WorkType.Build:
		case AvatarFarmOnline.Logic.WorkType.Plant:
		case AvatarFarmOnline.Logic.WorkType.PlantTree:
		case AvatarFarmOnline.Logic.WorkType.PlantAnimal:
			return 1000;
		}
	}

	public static Int2 PositionTile(Vector2 position)
	{
		return new Int2((int)(position.X / 2f), (int)(position.Y / 2f));
	}

	public static Vector3 WorldPosition(Vector2 stagePosition)
	{
		return new Vector3(stagePosition.X, 0f, stagePosition.Y);
	}

	public static Vector2 TilePosition(Int2 tilePosition)
	{
		return tilePosition * 2f;
	}

	public static Vector3 TileWorldPosition(Int2 tilePosition)
	{
		Vector2 vector = tilePosition * 2f;
		return new Vector3(vector.X, 0f, vector.Y);
	}

	public static int LevelUpCashReward(int level)
	{
		return Math.Min(1, level);
	}

	public static int LevelUpCoinsReward(int level)
	{
		return level * 200;
	}

	public static int FarmLevelUpXp(int currentLevel)
	{
		if (currentLevel > 0 && currentLevel <= levelUpXp.Length)
		{
			return levelUpXp[currentLevel - 1];
		}
		return 0;
	}

	public static int PlayerLevelUpXp(int currentLevel)
	{
		if (currentLevel > 0 && currentLevel <= playerLevelUpXp.Length)
		{
			return playerLevelUpXp[currentLevel - 1];
		}
		return 0;
	}
}
