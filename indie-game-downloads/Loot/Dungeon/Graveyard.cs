using System;
using System.IO;
using Eyehook.Framework;
using Loot.PC;
using Microsoft.Xna.Framework.Storage;

namespace Loot.Dungeon;

public static class Graveyard
{
	private const int VERSION = 1;

	public const string FileName = "graveyard.dat";

	private static Tombstone[] tombstones = new Tombstone[51];

	private static bool updated;

	public static bool Updated => updated;

	public static void AddTombstone(Player p)
	{
		updated = true;
		tombstones[p.Depth] = new Tombstone(p);
	}

	public static Tombstone GetTombstone(int depth)
	{
		if (tombstones[depth] == null)
		{
			return null;
		}
		updated = true;
		Tombstone result = tombstones[depth];
		tombstones[depth] = null;
		return result;
	}

	public static void Load()
	{
		updated = false;
		for (int i = 0; i < tombstones.Length; i++)
		{
			tombstones[i] = null;
		}
		try
		{
			using StorageContainer storageContainer = MC.StorageManager.OpenContainer(Profile.GlobalContainer);
			if (!storageContainer.FileExists("graveyard.dat"))
			{
				return;
			}
			using Stream input = storageContainer.OpenFile("graveyard.dat", FileMode.Open, FileAccess.Read);
			using BinaryReader binaryReader = new BinaryReader(input);
			Read(binaryReader);
			binaryReader.Close();
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public static void Save(StorageContainer container)
	{
		using Stream output = container.OpenFile("graveyard.dat", FileMode.Create);
		using BinaryWriter binaryWriter = new BinaryWriter(output);
		Write(binaryWriter);
		binaryWriter.Close();
		updated = false;
	}

	public static void Read(BinaryReader reader)
	{
		if (reader.ReadInt32() == 1)
		{
			for (int i = 0; i < tombstones.Length; i++)
			{
				tombstones[i] = ((!reader.ReadBoolean()) ? new Tombstone(reader) : null);
			}
		}
	}

	public static void Write(BinaryWriter writer)
	{
		writer.Write(1);
		for (int i = 0; i < tombstones.Length; i++)
		{
			if (tombstones[i] == null)
			{
				writer.Write(value: true);
				continue;
			}
			writer.Write(value: false);
			tombstones[i].Write(writer);
		}
	}
}
