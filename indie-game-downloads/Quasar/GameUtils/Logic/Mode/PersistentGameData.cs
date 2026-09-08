using System;

namespace Quasar.GameUtils.Logic.Mode;

public abstract class PersistentGameData : IDisposable
{
	protected GameSetup setup;

	protected GameData currentGame;

	public GameSetup Setup => setup;

	public GameData CurrentGame => currentGame;

	public PersistentGameData(GameSetup setup)
	{
		this.setup = setup;
	}

	public void PrepareNextRound()
	{
		if (currentGame != null)
		{
			currentGame.Dispose();
		}
		currentGame = DoPrepareNextRound();
	}

	protected abstract GameData DoPrepareNextRound();

	public virtual void RoundEnded()
	{
	}

	public void ClearRound()
	{
		if (currentGame != null)
		{
			currentGame.Dispose();
		}
		currentGame = null;
	}

	public virtual void Update()
	{
		if (currentGame != null)
		{
			currentGame.Update();
		}
	}

	public virtual void Dispose()
	{
		setup = null;
		if (currentGame != null)
		{
			currentGame.Dispose();
		}
		currentGame = null;
	}
}
