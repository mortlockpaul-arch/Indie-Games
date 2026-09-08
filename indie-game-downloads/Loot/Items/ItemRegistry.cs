using System;
using System.Collections.Generic;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Items.Amulets;
using Loot.Items.Armors;
using Loot.Items.Junks;
using Loot.Items.Misc;
using Loot.Items.Potions;
using Loot.Items.Rings;
using Loot.Items.Scrolls;
using Loot.Items.Weapons;

namespace Loot.Items;

public static class ItemRegistry
{
	private class Entry
	{
		public Type Type;

		public int MinDepth;

		public int MaxDepth;

		public int Weight;

		public Entry(Type type, int min, int max, int weight)
		{
			Type = type;
			MinDepth = min;
			MaxDepth = max;
			Weight = weight;
		}
	}

	public delegate bool Filter(Type t);

	private class ItemList : WeightedRandomList<Type>
	{
		private int depth = -1;

		private Filter filter;

		public void Populate(int depth)
		{
			Populate(depth, null);
		}

		public void Populate(int depth, Filter filter)
		{
			if (this.depth == depth && this.filter == filter)
			{
				return;
			}
			this.depth = depth;
			this.filter = filter;
			Clear();
			for (int i = 0; i < db.Count; i++)
			{
				Entry entry = db[i];
				if (depth >= entry.MinDepth && depth <= entry.MaxDepth)
				{
					if (filter == null)
					{
						Add(entry.Type, entry.Weight);
					}
					else if (filter(entry.Type))
					{
						Add(entry.Type, entry.Weight);
					}
				}
			}
		}
	}

	private static TypeRegistry<Item> registry = new TypeRegistry<Item>();

	private static List<Entry> db = new List<Entry>();

	private static ItemList itemList = new ItemList();

	private static Type[] EmptyTypes = new Type[0];

	public static void Initialize()
	{
		Register<Gold>("G", 400);
		Register<Emerald>("J0", 2);
		Register<Ruby>("J1", 2);
		Register<Sapphire>("J2", 2);
		Register<CharmResurrect>("M0", 0, 0, 0);
		Register<PotionHealth>("P0", 20);
		Register<PotionAntidote>("P1", 10);
		Register<PotionOil>("P2", 10);
		Register<PotionDrinkMe>("P3", 5);
		Register<PotionSkill>("P4", 5);
		Register<PotionInvisibility>("P5", 5);
		Register<PotionLevitate>("P6", 10);
		Register<PotionDirePoison>("P7", 5);
		Register<ScrollEnchantArmor>("S0", 3);
		Register<ScrollEnchantWeapon>("S1", 3);
		Register<ScrollIdentify>("S2", 20);
		Register<ScrollMap>("S3", 10);
		Register<ScrollRemoveCurse>("S4", 3);
		Register<ScrollTeleport>("S5", 10);
		Register<ScrollIllOmen>("S6", 5);
		Register<ScrollProtect>("S7", 10);
		Register<AmuletWooden>("X0", 0, 20, 10);
		Register<AmuletLeather>("X1", 0, 20, 5);
		Register<AmuletRune>("X2", 10, 30, 5);
		Register<AmuletIron>("X3", 10, 30, 5);
		Register<AmuletLapis>("X4", 20, 40, 5);
		Register<AmuletSilver>("X5", 20, 40, 5);
		Register<AmuletGold>("X6", 30, 51, 5);
		Register<AmuletBone>("X7", 40, 51, 5);
		Register<RingGold>("R0", 0, 20, 10);
		Register<RingAmethyst>("R1", 0, 20, 5);
		Register<RingSapphire>("R2", 10, 30, 5);
		Register<RingEmerald>("R3", 10, 30, 5);
		Register<RingRuby>("R4", 20, 40, 5);
		Register<RingDiamond>("R5", 20, 40, 5);
		Register<RingSignet>("R6", 30, 51, 5);
		Register<RingRoyal>("R7", 40, 51, 5);
		Register<ArmorCloak>("A0", 0, 10, 10);
		Register<ArmorClothes>("A1", 0, 10, 10);
		Register<ArmorRags>("A2", 0, 10, 10);
		Register<ArmorHarness>("A3", 0, 10, 10);
		Register<ArmorLeather>("A4", 5, 20, 10);
		Register<ArmorStudded>("A5", 10, 30, 10);
		Register<ArmorChainMail>("A6", 15, 40, 10);
		Register<ArmorSplintMail>("A7", 20, 51, 10);
		Register<ArmorPlateMail>("A8", 30, 51, 10);
		Register<ArmorFullPlate>("A9", 40, 51, 15);
		Register<WeaponClub>("W0", 0, 15, 10);
		Register<WeaponDagger>("W1", 0, 20, 5);
		Register<WeaponQuarterstaff>("W2", 5, 25, 5);
		Register<WeaponShortsword>("W3", 10, 30, 5);
		Register<WeaponMace>("W4", 15, 40, 5);
		Register<WeaponLongsword>("W5", 20, 51, 5);
		Register<WeaponGreatAxe>("W8", 25, 51, 5);
		Register<WeaponWarHammer>("W6", 30, 51, 5);
		Register<WeaponClaymore>("W7", 35, 51, 5);
		Register<WeaponSpikedClub>("W9", 3, 18, 5);
		Register<WeaponKris>("W10", 3, 23, 5);
		Register<WeaponPike>("W11", 8, 28, 5);
		Register<WeaponDirk>("W12", 13, 33, 5);
		Register<WeaponScepter>("W13", 18, 51, 5);
		Register<WeaponSai>("W14", 23, 51, 5);
		Register<WeaponSpikedHammer>("W15", 40, 51, 5);
		Register<WeaponJeweledClaymore>("W16", 40, 51, 5);
		Register<WeaponCrook>("W17", 0, 0, 0);
	}

	private static void Register<T>(string id, int weight) where T : Item
	{
		Register<T>(id, 0, 51, weight);
	}

	private static void Register<T>(string id, int minLevel, int maxLevel, int weight) where T : Item
	{
		registry.Register<T>(id);
		db.Add(new Entry(typeof(T), minLevel, maxLevel, weight));
	}

	public static bool RingFilter(Type t)
	{
		return t.IsSubclassOf(typeof(Ring));
	}

	public static bool AmuletFilter(Type t)
	{
		return t.IsSubclassOf(typeof(Amulet));
	}

	public static bool ShopFilter(Type t)
	{
		return !t.IsSubclassOf(typeof(Junk)) && (object)t != typeof(Gold) && (object)t != typeof(PotionDirePoison) && (object)t != typeof(PotionHealth) && (object)t != typeof(PotionOil) && (object)t != typeof(ScrollIdentify) && (object)t != typeof(ScrollIllOmen);
	}

	public static Item Random(int depth)
	{
		itemList.Populate(depth);
		Item item = (Item)itemList.Random().GetConstructor(EmptyTypes).Invoke(null);
		if (item is Equipment && DM.Random.Next(10) == 0)
		{
			item.Curse();
		}
		return item;
	}

	public static Item Random(int depth, Filter filter)
	{
		itemList.Populate(depth, filter);
		return (Item)itemList.Random().GetConstructor(EmptyTypes).Invoke(null);
	}

	public static Item ShopItem(int depth)
	{
		depth += 10;
		if (depth > 51)
		{
			depth = 51;
		}
		itemList.Populate(depth, ShopFilter);
		Item item = (Item)itemList.Random().GetConstructor(EmptyTypes).Invoke(null);
		if (item is Equipment)
		{
			item.Identify();
		}
		return item;
	}

	public static void WriteType(BinaryWriter writer, Type type)
	{
		registry.WriteType(writer, type);
	}

	public static Type ReadType(BinaryReader reader)
	{
		return registry.ReadType(reader);
	}

	public static void Save(BinaryWriter writer, Item item)
	{
		registry.Save(writer, item);
	}

	public static Item Load(BinaryReader reader)
	{
		return registry.Load(reader);
	}
}
