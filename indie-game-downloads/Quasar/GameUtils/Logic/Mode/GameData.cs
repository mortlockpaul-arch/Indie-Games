using System;

namespace Quasar.GameUtils.Logic.Mode;

public abstract class GameData : IDisposable
{
	protected enum GameState
	{
		Loading,
		Loaded,
		Playing,
		RoundEnded
	}

	protected string modeTitle = "";

	protected string modeShortDescription = "";

	protected string modeDescription = "";

	protected GameState gameState;

	protected PersistentGameData persistentData;

	protected IStage currentStage;

	public string ModeTitle => modeTitle;

	public string ModeShortDescription => modeShortDescription;

	public string ModeDescription => modeDescription;

	public IStage Stage => currentStage;

	public event GameEventHandler OnStageFinished;

	public abstract void PrepareNextStage();

	public GameData(PersistentGameData persistentData)
	{
		this.persistentData = persistentData;
	}

	public IStage LoadStage()
	{
		gameState = GameState.Loading;
		PrepareNextStage();
		currentStage = DoLoadStage();
		gameState = GameState.Loaded;
		return currentStage;
	}

	public virtual void StartGame()
	{
		gameState = GameState.Playing;
	}

	protected abstract IStage DoLoadStage();

	public virtual void Update()
	{
		if (gameState == GameState.Playing)
		{
			currentStage.Update();
		}
	}

	public virtual void FinishStage()
	{
		gameState = GameState.RoundEnded;
		persistentData.RoundEnded();
		if (OnStageFinished != null)
		{
			OnStageFinished(this);
		}
	}

	public virtual void DisposeStage()
	{
		if (currentStage != null)
		{
			currentStage.Dispose();
		}
		currentStage = null;
	}

	public virtual void Dispose()
	{
		DisposeStage();
	}
}
