using System;
using System.Collections.Generic;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items;

public abstract class GenericMaster : BinaryRW
{
	private List<int> freeSprites;

	private Dictionary<Type, bool> identifyMap;

	private Dictionary<Type, int> spriteMap;

	protected abstract Sprite[] MiscSprites { get; }

	public GenericMaster(BinaryReader reader)
	{
		Read(reader);
	}

	public GenericMaster()
	{
		identifyMap = new Dictionary<Type, bool>();
		freeSprites = new List<int>();
		resetSprites();
		spriteMap = new Dictionary<Type, int>();
	}

	private void resetSprites()
	{
		for (int i = 0; i < MiscSprites.Length; i++)
		{
			freeSprites.Add(i);
		}
	}

	public void Identify(Type type)
	{
		identifyMap[type] = true;
	}

	public bool Identified(Type type)
	{
		if (identifyMap.ContainsKey(type))
		{
			return identifyMap[type];
		}
		identifyMap[type] = false;
		return false;
	}

	public Sprite GetSprite(Type type)
	{
		int num;
		if (spriteMap.ContainsKey(type))
		{
			num = spriteMap[type];
		}
		else
		{
			if (freeSprites.Count == 0)
			{
				resetSprites();
			}
			num = freeSprites[DM.Random.Next(freeSprites.Count)];
			freeSprites.Remove(num);
			spriteMap.Add(type, num);
		}
		return MiscSprites[num];
	}

	public void Read(BinaryReader reader)
	{
		identifyMap = new Dictionary<Type, bool>();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			identifyMap[ItemRegistry.ReadType(reader)] = reader.ReadBoolean();
		}
		freeSprites = new List<int>();
		int num2 = reader.ReadInt32();
		for (int j = 0; j < num2; j++)
		{
			freeSprites.Add(reader.ReadInt32());
		}
		spriteMap = new Dictionary<Type, int>();
		int num3 = reader.ReadInt32();
		for (int k = 0; k < num3; k++)
		{
			spriteMap[ItemRegistry.ReadType(reader)] = reader.ReadInt32();
		}
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write(identifyMap.Count);
		foreach (Type key in identifyMap.Keys)
		{
			ItemRegistry.WriteType(writer, key);
			writer.Write(identifyMap[key]);
		}
		writer.Write(freeSprites.Count);
		for (int i = 0; i < freeSprites.Count; i++)
		{
			writer.Write(freeSprites[i]);
		}
		writer.Write(spriteMap.Count);
		foreach (Type key2 in spriteMap.Keys)
		{
			ItemRegistry.WriteType(writer, key2);
			writer.Write(spriteMap[key2]);
		}
	}
}
