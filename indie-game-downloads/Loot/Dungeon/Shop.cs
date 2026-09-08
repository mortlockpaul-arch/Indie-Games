using System;
using System.IO;
using Eyehook.Framework;
using Loot.Items;

namespace Loot.Dungeon;

public class Shop : BinaryRW
{
	public Item[] Items = new Item[12];

	public Shop()
	{
	}

	public Shop(BinaryReader reader)
	{
		Read(reader);
	}

	public bool AddItem(Item item)
	{
		for (int i = 0; i < Items.Length; i++)
		{
			if (Items[i] == null)
			{
				Items[i] = item;
				return true;
			}
		}
		return false;
	}

	public void Sort()
	{
		Array.Sort(Items, Item.Comparator);
	}

	public void Read(BinaryReader reader)
	{
		for (int i = 0; i < Items.Length; i++)
		{
			Items[i] = ItemRegistry.Load(reader);
		}
	}

	public void Write(BinaryWriter writer)
	{
		for (int i = 0; i < Items.Length; i++)
		{
			ItemRegistry.Save(writer, Items[i]);
		}
	}
}
