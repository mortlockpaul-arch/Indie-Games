using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.GameUtils.Logic.Mode;

public abstract class GameSetup : IDisposable
{
	private int gameMode;

	public int GameMode => gameMode;

	public abstract List<PlayerIndex> PlayerIndices { get; }

	public virtual bool Ready => true;

	protected GameSetup(int gameMode)
	{
		this.gameMode = gameMode;
	}

	public abstract PersistentGameData SetupPersistentData();

	public virtual void Dispose()
	{
	}
}
