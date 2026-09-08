using System;
using System.Collections.Generic;
using System.IO;
using Loot.Dungeon;

namespace Loot.Encounters;

public class EncounterList
{
	private List<Type> encounterTypes = new List<Type>();

	public EncounterList()
	{
		populate();
	}

	public EncounterList(BinaryReader reader)
	{
		Load(reader);
	}

	private void populate()
	{
		List<Type> dbClone = EncounterRegistry.dbClone;
		while (dbClone.Count > 0)
		{
			int index = DM.Random.Next(dbClone.Count);
			encounterTypes.Add(dbClone[index]);
			dbClone.RemoveAt(index);
		}
	}

	public Encounter Next(Location loc)
	{
		if (encounterTypes.Count == 0)
		{
			populate();
		}
		Type t = encounterTypes[0];
		encounterTypes.RemoveAt(0);
		return EncounterRegistry.CreateEncounter(t, loc);
	}

	public void Save(BinaryWriter writer)
	{
		writer.Write(encounterTypes.Count);
		for (int i = 0; i < encounterTypes.Count; i++)
		{
			EncounterRegistry.WriteType(writer, encounterTypes[i]);
		}
	}

	private void Load(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			encounterTypes.Add(EncounterRegistry.ReadType(reader));
		}
	}
}
