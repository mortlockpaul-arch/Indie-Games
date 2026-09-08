using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.NPCs;

public static class NPCRegistry
{
	private class Entry
	{
		public Type Type;

		public int MinDepth;

		public int MaxDepth;

		public Entry(Type type, int min, int max)
		{
			Type = type;
			MinDepth = min;
			MaxDepth = max;
		}
	}

	private class NPCList : List<Type>
	{
		private int depth = -1;

		public void Populate(int depth)
		{
			if (this.depth == depth)
			{
				return;
			}
			this.depth = depth;
			Clear();
			for (int i = 0; i < db.Count; i++)
			{
				Entry entry = db[i];
				if (depth >= entry.MinDepth && depth <= entry.MaxDepth)
				{
					Add(entry.Type);
				}
			}
		}
	}

	private const double EpicOdds = 0.1;

	private static TypeRegistry<NPC> registry = new TypeRegistry<NPC>();

	private static List<Entry> db = new List<Entry>();

	private static NPCList npcList = new NPCList();

	private static Type[] constructorParams = new Type[2]
	{
		typeof(int),
		typeof(Location)
	};

	private static object[] constructorParamValues = new object[2];

	public static void Initialize()
	{
		Register<Orb>("N0", -1, -1);
		Register<Rat>("N1", 0, 5);
		Register<Spider>("N2", 2, 10);
		Register<Bat>("N3", 3, 10);
		Register<Witch>("N4", 6, 17);
		Register<Wolf>("N5", 8, 20);
		Register<Goblin>("N6", 12, 25);
		Register<Orc>("N7", 18, 30);
		Register<Jelly>("N8", 20, 30);
		Register<Skeleton>("N9", 25, 40);
		Register<Zombie>("N10", 25, 40);
		Register<Vampire>("N11", 30, 45);
		Register<Beholder>("N201", 35, 45);
		Register<Giant>("N12", 40, 51);
		Register<Dragon>("N13", 43, 51);
		Register<SandWorm>("N14", 46, 51);
		Register<GoblinThief>("N100", -1, -1);
		Register<Reaper>("N101", -1, -1);
	}

	public static void Register<T>(string id, int minLevel, int maxLevel) where T : NPC
	{
		registry.Register<T>(id);
		db.Add(new Entry(typeof(T), minLevel, maxLevel));
	}

	public static NPC Random(int depth, Location loc)
	{
		npcList.Populate(depth);
		ConstructorInfo constructor = npcList[DM.Random.Next(npcList.Count)].GetConstructor(constructorParams);
		constructorParamValues[0] = depth;
		constructorParamValues[1] = loc;
		NPC nPC = (NPC)constructor.Invoke(constructorParamValues);
		if (DM.Random.NextDouble() < 0.1)
		{
			nPC.Elite();
		}
		return nPC;
	}

	public static void Save(BinaryWriter writer, NPC npc)
	{
		if (npc is Orb)
		{
			npc = null;
		}
		registry.Save(writer, npc);
	}

	public static NPC Load(BinaryReader reader)
	{
		return registry.Load(reader);
	}
}
