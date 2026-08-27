using EGEngine;

namespace GameEngine;

public class GameObjectives
{
	public const int NO_TARGET = 0;

	public const int HOME_TARGET = 1;

	public const int STORE_TARGET = 2;

	public const int GAS_TARGET = 3;

	public static int TargetLocation;

	public static void CellPhonePictureFunc(object sender, LevelObjectives e)
	{
		if (e.objCallbackMsg == 3)
		{
			TargetLocation = 1;
			GenericMessages.Add("Pictures Of Bigfoot Taken", 6);
			LevelObjectives.Add("Go Home & Post Pictures On Your Social Media", float.MaxValue, FirstPicturesTakenFunc);
		}
		else if (e.objCallbackMsg == 1)
		{
			TargetLocation = 0;
			GenericMessages.Add("Failed To Get Picture Of Bigfoot", 6);
			LevelObjectives.Add("Take Pictures Of Foot Prints By Road", float.MaxValue, FootPrintFunc_00);
		}
	}

	public static void FootPrintFunc_00(object sender, LevelObjectives e)
	{
		if (e.objCallbackMsg == 4)
		{
			TargetLocation = 1;
			GenericMessages.Add("Foot Prints Pictures Taken", 6);
			LevelObjectives.Add("Go Home & Post Pictures On Your Social Media", float.MaxValue, FirstPicturesTakenFunc);
		}
	}

	public static void FirstPicturesTakenFunc(object sender, LevelObjectives e)
	{
		if (e.objCallbackMsg == 5)
		{
			SimpleZombieAI.TotalMonies += 200;
			TargetLocation = 2;
			GenericMessages.Add("Evidence Posted. $200 Made From Monitization", 6);
			LevelObjectives.Add("Purchase Better Equipment From The General Store", float.MaxValue, GenericCallBackFunc);
		}
	}

	public static void GenericCallBackFunc(object sender, LevelObjectives e)
	{
		if (e.objCallbackMsg == 5)
		{
			SimpleZombieAI.TotalMonies += 200;
			TargetLocation = 0;
			GenericMessages.Add("Evidence Posted. $200 Made From Monitization", 6);
		}
		else if (e.objCallbackMsg == 3)
		{
			TargetLocation = 0;
			GenericMessages.Add("Post Pictures/Video On Your Social Media", 6);
		}
		else
		{
			TargetLocation = 0;
		}
	}
}
