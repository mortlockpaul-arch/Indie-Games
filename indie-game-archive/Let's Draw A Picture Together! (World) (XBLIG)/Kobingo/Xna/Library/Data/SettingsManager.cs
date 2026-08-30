using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Storage;

namespace Kobingo.Xna.Library.Data;

public class SettingsManager
{
	public string Filename { get; private set; }

	private Dictionary<string, SettingsEntry> Settings { get; set; }

	public SettingsEntry this[string name]
	{
		get
		{
			return Settings[name];
		}
		set
		{
			Settings[name] = value;
		}
	}

	public IEnumerable<string> Names
	{
		get
		{
			foreach (string key in Settings.Keys)
			{
				yield return key;
			}
		}
	}

	public SettingsManager(string filename)
	{
		Filename = filename;
		Settings = new Dictionary<string, SettingsEntry>();
	}

	public void Save(StorageContainer container)
	{
		using FileStream output = new FileStream(Path.Combine(container.Path, Filename), FileMode.Create);
		using BinaryWriter binaryWriter = new BinaryWriter(output);
		foreach (string name in Names)
		{
			binaryWriter.Write(name);
			binaryWriter.Write(Settings[name].Value);
		}
	}

	public void Load(StorageContainer container)
	{
		using FileStream input = new FileStream(Path.Combine(container.Path, Filename), FileMode.OpenOrCreate);
		using BinaryReader binaryReader = new BinaryReader(input);
		while (binaryReader.PeekChar() > 0)
		{
			string key = binaryReader.ReadString();
			string value = binaryReader.ReadString();
			if (Settings.ContainsKey(key))
			{
				Settings[key].Value = value;
			}
		}
	}
}
