using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Storage;

namespace Kobingo.Xna.Library.Data;

public class HighscoreManager
{
	public string Filename { get; private set; }

	private List<HighscoreEntry> Entries { get; set; }

	public bool IsEmpty => Entries.Count == 0;

	public HighscoreManager(string filename)
	{
		Entries = new List<HighscoreEntry>();
		Filename = filename;
	}

	public void AddEntryByScore(string name, int score, int maxEntries)
	{
		AddEntryByScore(name, 0, score, maxEntries);
	}

	public void AddEntryByScore(string name, int type, int score, int maxEntries)
	{
		AddEntryByScore(new HighscoreEntry
		{
			Name = name,
			Type = type,
			Created = DateTime.Now,
			Score = score
		}, maxEntries);
	}

	public void AddEntryByScore(HighscoreEntry entry, int maxEntries)
	{
		AddEntryByDesc(entry, (HighscoreEntry x) => x.Score, maxEntries);
	}

	public void AddEntryBy(HighscoreEntry entry, Func<HighscoreEntry, IComparable> orderBy, int maxEntries)
	{
		Entries.Add(entry);
		if (maxEntries > 0)
		{
			IEnumerable<HighscoreEntry> entriesBy = GetEntriesBy(entry.Type, orderBy);
			while (GetEntries(entry.Type).Count() > maxEntries)
			{
				Entries.Remove(entriesBy.Last());
			}
		}
	}

	public void AddEntryByDesc(HighscoreEntry entry, Func<HighscoreEntry, IComparable> orderBy, int maxEntries)
	{
		Entries.Add(entry);
		if (maxEntries > 0)
		{
			IEnumerable<HighscoreEntry> entriesByDesc = GetEntriesByDesc(entry.Type, orderBy);
			while (GetEntries(entry.Type).Count() > maxEntries)
			{
				Entries.Remove(entriesByDesc.Last());
			}
		}
	}

	public IEnumerable<HighscoreEntry> GetEntries()
	{
		return Entries;
	}

	public IEnumerable<HighscoreEntry> GetEntries(int type)
	{
		return Entries.Where((HighscoreEntry entry) => entry.Type == type);
	}

	public IEnumerable<HighscoreEntry> GetEntriesByScore()
	{
		return GetEntriesByScore(0);
	}

	public IEnumerable<HighscoreEntry> GetEntriesByScore(int type)
	{
		return GetEntriesByDesc(0, (HighscoreEntry x) => x.Score);
	}

	public IEnumerable<HighscoreEntry> GetEntriesBy(int type, Func<HighscoreEntry, IComparable> orderBy)
	{
		return from entry in Entries
			where entry.Type == type
			orderby orderBy
			select entry;
	}

	public IEnumerable<HighscoreEntry> GetEntriesByDesc(int type, Func<HighscoreEntry, IComparable> orderBy)
	{
		return from entry in Entries
			where entry.Type == type
			orderby orderBy descending
			select entry;
	}

	public void Load(StorageContainer container)
	{
		Entries.Clear();
		using FileStream input = new FileStream(Path.Combine(container.Path, Filename), FileMode.OpenOrCreate);
		using BinaryReader binaryReader = new BinaryReader(input);
		while (binaryReader.PeekChar() > 0)
		{
			HighscoreEntry highscoreEntry = new HighscoreEntry();
			highscoreEntry.Name = binaryReader.ReadString();
			highscoreEntry.Type = binaryReader.ReadInt32();
			highscoreEntry.Created = new DateTime(binaryReader.ReadInt64());
			highscoreEntry.Score = binaryReader.ReadInt32();
			HighscoreEntry highscoreEntry2 = highscoreEntry;
			for (int i = 0; i < highscoreEntry2.Values.Length; i++)
			{
				highscoreEntry2.Values[i] = binaryReader.ReadInt32();
			}
			Entries.Add(highscoreEntry2);
		}
	}

	public void Save(StorageContainer container)
	{
		using FileStream output = new FileStream(Path.Combine(container.Path, Filename), FileMode.Create);
		using BinaryWriter binaryWriter = new BinaryWriter(output);
		HighscoreEntry[] array = Entries.ToArray();
		HighscoreEntry[] array2 = array;
		foreach (HighscoreEntry highscoreEntry in array2)
		{
			binaryWriter.Write(highscoreEntry.Name);
			binaryWriter.Write(highscoreEntry.Type);
			binaryWriter.Write(highscoreEntry.Created.Ticks);
			binaryWriter.Write(highscoreEntry.Score);
			for (int j = 0; j < highscoreEntry.Values.Length; j++)
			{
				binaryWriter.Write(highscoreEntry.Values[j]);
			}
		}
	}
}
