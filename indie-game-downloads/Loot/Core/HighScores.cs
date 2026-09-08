using System;
using System.Collections.Generic;
using System.IO;
using Eyehook.Framework;
using Microsoft.Xna.Framework.Storage;

namespace Loot.Core;

public static class HighScores
{
	public class Entry : IComparable<Entry>
	{
		public readonly string Tag;

		public readonly int PlayerClass;

		public readonly int Level;

		public readonly int Depth;

		public readonly double PlayTime;

		public Entry(BinaryReader reader)
		{
			Tag = reader.ReadString();
			PlayerClass = reader.ReadInt32();
			Level = reader.ReadInt32();
			Depth = reader.ReadInt32();
			PlayTime = reader.ReadDouble();
		}

		public Entry(string tag, int playerClass, int level, int depth, TimeSpan playTime)
		{
			Tag = tag;
			PlayerClass = playerClass;
			Level = level;
			Depth = depth;
			PlayTime = playTime.TotalSeconds;
		}

		public int CompareTo(Entry entry)
		{
			if (Depth > entry.Depth)
			{
				return -1;
			}
			if (Depth < entry.Depth)
			{
				return 1;
			}
			if (PlayTime < entry.PlayTime)
			{
				return -1;
			}
			return (PlayTime > entry.PlayTime) ? 1 : 0;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(Tag);
			writer.Write(PlayerClass);
			writer.Write(Level);
			writer.Write(Depth);
			writer.Write(PlayTime);
		}
	}

	private const int VERSION = 2;

	private const int size = 10;

	public static string HighScoresFile = "highscores.dat";

	public static List<Entry> EasyEntries = new List<Entry>();

	public static List<Entry> NormalEntries = new List<Entry>();

	public static List<Entry> HardEntries = new List<Entry>();

	public static int Add(Difficulty difficulty, string tag, int playerClass, int level, int depth, TimeSpan playTime)
	{
		Entry item = new Entry(tag, playerClass, level, depth, playTime);
		List<Entry> list = difficulty switch
		{
			Difficulty.Easy => EasyEntries, 
			Difficulty.Normal => NormalEntries, 
			Difficulty.Hard => HardEntries, 
			_ => throw new Exception("Unknown difficulty: " + difficulty), 
		};
		list.Add(item);
		list.Sort();
		if (list.Count > 10)
		{
			list.RemoveRange(10, list.Count - 10);
		}
		return list.IndexOf(item) + 1;
	}

	public static void Load()
	{
		EasyEntries.Clear();
		NormalEntries.Clear();
		HardEntries.Clear();
		try
		{
			using StorageContainer storageContainer = MC.StorageManager.OpenContainer(Profile.GlobalContainer);
			if (!storageContainer.FileExists(HighScoresFile))
			{
				return;
			}
			using Stream input = storageContainer.OpenFile(HighScoresFile, FileMode.Open, FileAccess.Read);
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
		using Stream output = container.OpenFile(HighScoresFile, FileMode.Create);
		using BinaryWriter binaryWriter = new BinaryWriter(output);
		Write(binaryWriter);
		binaryWriter.Close();
	}

	private static void Read(BinaryReader reader)
	{
		switch (reader.ReadInt32())
		{
		case 1:
			ReadEntryList(reader, NormalEntries);
			break;
		case 2:
			ReadEntryList(reader, EasyEntries);
			ReadEntryList(reader, NormalEntries);
			ReadEntryList(reader, HardEntries);
			break;
		}
	}

	private static void Write(BinaryWriter writer)
	{
		writer.Write(2);
		WriteEntryList(writer, EasyEntries);
		WriteEntryList(writer, NormalEntries);
		WriteEntryList(writer, HardEntries);
	}

	private static void ReadEntryList(BinaryReader reader, List<Entry> entries)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Entry item = new Entry(reader);
			entries.Add(item);
		}
	}

	private static void WriteEntryList(BinaryWriter writer, List<Entry> entries)
	{
		writer.Write(entries.Count);
		for (int i = 0; i < entries.Count; i++)
		{
			entries[i].Write(writer);
		}
	}
}
