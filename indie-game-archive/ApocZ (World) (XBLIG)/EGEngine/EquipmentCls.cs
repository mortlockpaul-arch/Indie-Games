using Microsoft.Xna.Framework;

namespace EGEngine;

public class EquipmentCls : PropModelBase
{
	public static string[] EquipmentItemDesc = new string[20]
	{
		"Invalid Item", "Deployed Tent, Store Items And Rest", "Empty Jerry Can", "Full Jerry Can, 5 Liter Gasoline", "Large Backpack, 20 pockets", "Medium Backpack, 12 Pockets", "Small Backpack, 8 Pockets ", "Tactical Flashlight, Attaches To Chest", "Tool Box, Used To Repair Vehicles", "Compass",
		"Spare Tire", "Tent, Can Be Deployed", "7.62 Soviet 30 Round Magazine", "5.56 NATO 30 Round Magazine", "7.62 NATO 20 Round Magazine", "5.56 NATO 100 Round Ammo Belt", "9mm, 15 Round Magazine", ".50, 7 Round Magazine", "20 Guage 8 Shells", ".308, 20 Round Magazine"
	};

	public static byte[] Reservedbyte0 = new byte[20]
	{
		0, 32, 0, 5, 20, 12, 8, 1, 1, 1,
		1, 1, 30, 30, 20, 100, 15, 7, 8, 20
	};

	private Vector3 position = Vector3.Zero;

	private Vector3 direction = Vector3.UnitZ;

	private Vector3 right = Vector3.UnitX;

	public static PropModelBase[] itemsModels = new PropModelBase[20];

	public static int[] ItemSlotUse = new int[20];

	private static bool Initialized = false;

	public static EquipmentItemType CreateRandom(int seed)
	{
		return (EquipmentItemType)EndGameEngine.randGenerator.Next(2, 12);
	}

	public static EquipmentItemType CreateRandom(int seed, byte range)
	{
		if (range == 3)
		{
			int num = EndGameEngine.randGenerator.Next(0, 100);
			if (num < 25)
			{
				return EquipmentItemType.JerryCanEmpty;
			}
			if (num < 50)
			{
				return EquipmentItemType.JerryCanFull;
			}
			if (num < 75)
			{
				return EquipmentItemType.ToolBox;
			}
			return EquipmentItemType.Tire;
		}
		return (EquipmentItemType)EndGameEngine.randGenerator.Next(2, 12);
	}

	public static EquipmentItemType CreateRandomAmmo(int seed)
	{
		return (EquipmentItemType)EndGameEngine.randGenerator.Next(12, 20);
	}

	public static EquipmentItemType CreateRandomAmmo(int seed, byte range)
	{
		switch (range)
		{
		case 4:
			if (EndGameEngine.randGenerator.Next(0, 100) >= 50)
			{
				return EquipmentItemType.ShotgunShells;
			}
			return EquipmentItemType.PistolM9Clip;
		case 5:
		{
			EquipmentItemType equipmentItemType = (EquipmentItemType)EndGameEngine.randGenerator.Next(14, 20);
			if (equipmentItemType == EquipmentItemType.PistolM9Clip || equipmentItemType == EquipmentItemType.ShotgunShells)
			{
				equipmentItemType--;
			}
			return equipmentItemType;
		}
		default:
			return (EquipmentItemType)EndGameEngine.randGenerator.Next(12, 20);
		}
	}

	public override void Load(string s)
	{
		if (!Initialized)
		{
			Initialized = true;
			for (int i = 1; i < 20; i++)
			{
				PropModelBase propModelBase = new PropModelBase();
				propModelBase.Load("models\\items\\" + (EquipmentItemType)i);
				itemsModels[i] = propModelBase;
			}
			for (int j = 0; j < 20; j++)
			{
				ItemSlotUse[j] = 1;
			}
			ItemSlotUse[2] = 2;
			ItemSlotUse[3] = 2;
			ItemSlotUse[8] = 2;
			ItemSlotUse[10] = 4;
			ItemSlotUse[11] = 2;
			ItemSlotUse[1] = 2;
			ItemSlotUse[15] = 2;
		}
	}
}
