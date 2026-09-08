using System;
using System.Collections.Generic;
using System.IO;
using Loot.Dungeon;
using Loot.PC;
using Microsoft.Xna.Framework.Storage;

namespace Loot.Awardments;

public class AwardmentProfile
{
	private const int Version = 1;

	private const string fileName = "Awardments.dat";

	private int points;

	private bool updated;

	private Dictionary<int, DateTime> unlocked = new Dictionary<int, DateTime>();

	public int Points => points;

	public int MaxPoints => Awardment.MaxPoints;

	public bool Updated => updated;

	public AwardmentProfile(StorageContainer container)
	{
		if (container.FileExists("Awardments.dat"))
		{
			using Stream input = container.OpenFile("Awardments.dat", FileMode.Open, FileAccess.Read);
			using BinaryReader binaryReader = new BinaryReader(input);
			read(binaryReader);
			binaryReader.Close();
		}
		updated = false;
	}

	public bool IsUnlocked(Awardment award)
	{
		return unlocked.ContainsKey(award.Id);
	}

	public DateTime UnlockDate(Awardment award)
	{
		return unlocked[award.Id];
	}

	public bool Unlock(Awardment award)
	{
		if (DM.Player is DemoPlayer || unlocked.ContainsKey(award.Id))
		{
			return false;
		}
		points += award.Points;
		unlocked.Add(award.Id, DateTime.Now);
		updated = true;
		LootGame.AwardComponent.Display(award);
		return true;
	}

	public void Save(StorageContainer container)
	{
		if (!updated)
		{
			return;
		}
		using (Stream output = container.OpenFile("Awardments.dat", FileMode.Create))
		{
			using BinaryWriter binaryWriter = new BinaryWriter(output);
			write(binaryWriter);
			binaryWriter.Close();
		}
		updated = false;
	}

	private void read(BinaryReader reader)
	{
		if (reader.ReadInt32() != 1)
		{
			throw new Exception("AwardHistory version mismatch");
		}
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int num2 = reader.ReadInt32();
			DateTime value = new DateTime(reader.ReadInt64());
			points += Awardment.Get(num2).Points;
			unlocked.Add(num2, value);
		}
	}

	private void write(BinaryWriter writer)
	{
		writer.Write(1);
		writer.Write(unlocked.Count);
		foreach (int key in unlocked.Keys)
		{
			writer.Write(key);
			writer.Write(unlocked[key].Ticks);
		}
	}
}
