using System;
using System.IO;
using InfinityLibrary;

namespace Infinity;

public class SaveData
{
	public const int DefaultHiScore = 1000;

	public const int HiScoreMax = 1000;

	private const int DataVersion = 1;

	public int HiScore { get; set; }

	public int DrawModeIndex { get; set; }

	public int DifficultIndex { get; set; }

	public AnaglyphSettings AnaglyphSettings { get; set; }

	public SaveData()
	{
		HiScore = 1000;
		DrawModeIndex = 0;
		DifficultIndex = 1;
		AnaglyphSettings = new AnaglyphSettings();
	}

	public static SaveData Read(BinaryReader reader, SaveData instance)
	{
		int num = reader.ReadInt32();
		if (num <= 0 || num > 1 || reader.BaseStream.Position == reader.BaseStream.Length)
		{
			return instance;
		}
		Func<BinaryReader, SaveData, SaveData>[] array = new Func<BinaryReader, SaveData, SaveData>[2] { Read_000, Read_001 };
		return array[num](reader, instance);
	}

	public static SaveData Read_000(BinaryReader reader, SaveData instance)
	{
		instance.HiScore = reader.ReadInt32();
		return instance;
	}

	public static SaveData Read_001(BinaryReader reader, SaveData instance)
	{
		instance.HiScore = reader.ReadInt32();
		instance.DrawModeIndex = reader.ReadInt32();
		instance.DifficultIndex = reader.ReadInt32();
		instance.HiScore = Math.Max(instance.HiScore, 0);
		instance.HiScore = Math.Min(instance.HiScore, 9999999);
		instance.DrawModeIndex = Math.Max(instance.DrawModeIndex, 0);
		instance.DrawModeIndex = Math.Min(instance.DrawModeIndex, 2);
		instance.DifficultIndex = Math.Max(instance.DifficultIndex, 0);
		instance.DifficultIndex = Math.Min(instance.DifficultIndex, 2);
		return instance;
	}

	public void Write(BinaryWriter writer)
	{
		Write_001(writer);
	}

	[Obsolete]
	private void Write_000(BinaryWriter writer)
	{
		writer.Write(HiScore);
	}

	private void Write_001(BinaryWriter writer)
	{
		writer.Write(1);
		writer.Write(HiScore);
		writer.Write(DrawModeIndex);
		writer.Write(DifficultIndex);
	}
}
