using System.IO;
using Eyehook.Framework;
using Microsoft.Xna.Framework.Storage;

namespace Loot;

public class Preferences
{
	private const int VERSION = 3;

	private const string fileName = "profile.dat";

	public const int MaxVolume = 10;

	public bool Tips;

	public bool TextFX;

	public bool Gore;

	public bool HPBar;

	public bool Vibrate;

	public bool Wobble;

	private int musicVolume;

	private int effectsVolume;

	public int MusicVolume => musicVolume;

	public int EffectsVolume => effectsVolume;

	public Preferences()
	{
		defaultValues();
	}

	public Preferences(StorageContainer container)
	{
		if (!container.FileExists("profile.dat"))
		{
			defaultValues();
			return;
		}
		using Stream input = container.OpenFile("profile.dat", FileMode.Open, FileAccess.Read);
		using BinaryReader binaryReader = new BinaryReader(input);
		Read(binaryReader);
		binaryReader.Close();
	}

	public Preferences(Preferences p)
	{
		Tips = p.Tips;
		TextFX = p.TextFX;
		Gore = p.Gore;
		HPBar = p.HPBar;
		Vibrate = p.Vibrate;
		Wobble = p.Wobble;
		musicVolume = p.musicVolume;
		effectsVolume = p.effectsVolume;
	}

	public bool Equals(Preferences p)
	{
		return Tips == p.Tips && TextFX == p.TextFX && Gore == p.Gore && HPBar == p.HPBar && Vibrate == p.Vibrate && Wobble == p.Wobble && musicVolume == p.musicVolume && effectsVolume == p.effectsVolume;
	}

	private void defaultValues()
	{
		Tips = true;
		TextFX = true;
		Gore = true;
		HPBar = true;
		Vibrate = true;
		Wobble = true;
		SetMusicVolume(5);
		SetEffectsVolume(5);
	}

	public void SetMusicVolume(int volume)
	{
		if (volume < 0)
		{
			volume = 0;
		}
		if (volume > 10)
		{
			volume = 10;
		}
		musicVolume = volume;
		MC.AudioManager.setMusicVolume((float)musicVolume / 5f);
	}

	public void SetEffectsVolume(int volume)
	{
		if (volume < 0)
		{
			volume = 0;
		}
		if (volume > 10)
		{
			volume = 10;
		}
		effectsVolume = volume;
		MC.AudioManager.setEffectsVolume((float)effectsVolume / 5f);
	}

	public void Save(StorageContainer container)
	{
		using Stream output = container.OpenFile("profile.dat", FileMode.Create);
		using BinaryWriter binaryWriter = new BinaryWriter(output);
		Write(binaryWriter);
		binaryWriter.Close();
	}

	private void Read(BinaryReader reader)
	{
		if (reader.ReadInt32() != 3)
		{
			defaultValues();
			return;
		}
		Tips = reader.ReadBoolean();
		TextFX = reader.ReadBoolean();
		Gore = reader.ReadBoolean();
		HPBar = reader.ReadBoolean();
		Vibrate = reader.ReadBoolean();
		Wobble = reader.ReadBoolean();
		SetMusicVolume(reader.ReadInt32());
		SetEffectsVolume(reader.ReadInt32());
	}

	private void Write(BinaryWriter writer)
	{
		writer.Write(3);
		writer.Write(Tips);
		writer.Write(TextFX);
		writer.Write(Gore);
		writer.Write(HPBar);
		writer.Write(Vibrate);
		writer.Write(Wobble);
		writer.Write(musicVolume);
		writer.Write(effectsVolume);
	}
}
