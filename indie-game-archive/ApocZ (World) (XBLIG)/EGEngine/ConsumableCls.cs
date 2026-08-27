using Microsoft.Xna.Framework;

namespace EGEngine;

public class ConsumableCls : PropModelBase
{
	public static string[] ConsumableItemsDesc = new string[9] { "Invalid Item", "Empty can", "Mushroom Soup", "Baked Beans", "Bottled Water", "Empty Canteen", "Full Canteen", "Bandage, Stops Bleeding", "Pain Pills, Helps When Injured Or Dehydrated" };

	public static int[] ItemSlotUse = new int[9];

	private Vector3 position = Vector3.Zero;

	private Vector3 direction = Vector3.UnitZ;

	private Vector3 right = Vector3.UnitX;

	public static PropModelBase[] itemsModels = new PropModelBase[9];

	private static bool Initialized = false;

	public static ConsumableItemType CreateRandom(int seed)
	{
		return (ConsumableItemType)EndGameEngine.randGenerator.Next(1, 9);
	}

	public static ConsumableItemType CreateRandom(int seed, byte range)
	{
		if (range == 1)
		{
			return (ConsumableItemType)EndGameEngine.randGenerator.Next(7, 9);
		}
		return (ConsumableItemType)EndGameEngine.randGenerator.Next(1, 9);
	}

	public override void Load(string s)
	{
		if (!Initialized)
		{
			Initialized = true;
			for (int i = 1; i < 9; i++)
			{
				PropModelBase propModelBase = new PropModelBase();
				propModelBase.Load("models\\items\\" + (ConsumableItemType)i);
				itemsModels[i] = propModelBase;
			}
			for (int j = 0; j < 9; j++)
			{
				ItemSlotUse[j] = 1;
			}
		}
	}
}
