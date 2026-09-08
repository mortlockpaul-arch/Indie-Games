using System;
using System.IO;

namespace Loot.Core;

public class GameOptions
{
	private const int VERSION = 1;

	private long gameId;

	private Difficulty difficulty;

	public long GameId => gameId;

	public Difficulty Difficulty => difficulty;

	public GameOptions()
	{
		gameId = 0L;
		difficulty = Difficulty.Normal;
	}

	public GameOptions(Difficulty difficulty)
	{
		gameId = DateTime.UtcNow.Ticks;
		this.difficulty = difficulty;
	}

	public GameOptions(BinaryReader reader)
	{
		reader.ReadInt32();
		gameId = reader.ReadInt64();
		difficulty = (Difficulty)reader.ReadInt32();
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write(1);
		writer.Write(gameId);
		writer.Write((int)difficulty);
	}
}
