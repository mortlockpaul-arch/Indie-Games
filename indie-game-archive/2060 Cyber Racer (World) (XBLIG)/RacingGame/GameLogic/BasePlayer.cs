using Microsoft.Xna.Framework.GamerServices;
using RacingGame.GameScreens;
using RacingGame.Graphics;

namespace RacingGame.GameLogic;

public class BasePlayer
{
	public const int StartGameZoomTimeMilliseconds = 5000;

	public const int StartGameZoomedInTime = 3000;

	protected float currentGameTimeMilliseconds;

	protected int lap;

	private float bestLapTimeMilliseconds;

	private float zoomInTime = 5000f;

	public bool game_paused;

	protected bool victory;

	protected int levelNum;

	protected bool isGameOver;

	private bool alreadyUploadedHighscore;

	private bool firstFrame = true;

	public int CurrentLap => lap;

	public float BestTimeMilliseconds => bestLapTimeMilliseconds;

	public float GameTimeMilliseconds => currentGameTimeMilliseconds - zoomInTime;

	protected float ZoomInTime
	{
		get
		{
			return zoomInTime;
		}
		set
		{
			zoomInTime = value;
		}
	}

	public bool Victory => victory;

	public int LevelNum => levelNum;

	public bool GameOver => isGameOver;

	public bool WonGame => victory;

	public bool CanControlCar
	{
		get
		{
			if (zoomInTime <= 0f)
			{
				return !GameOver;
			}
			return false;
		}
	}

	protected void StartNewLap()
	{
		lap++;
		RacingGameManager.Landscape.StartNewLap();
		if (bestLapTimeMilliseconds == 0f || currentGameTimeMilliseconds < bestLapTimeMilliseconds)
		{
			bestLapTimeMilliseconds = currentGameTimeMilliseconds;
		}
		currentGameTimeMilliseconds = zoomInTime;
	}

	public void SetGameOverAndUploadHighscore()
	{
		isGameOver = true;
		if (!alreadyUploadedHighscore)
		{
			alreadyUploadedHighscore = true;
			Highscores.SubmitHighscore(levelNum, (int)currentGameTimeMilliseconds);
		}
	}

	public virtual void Reset()
	{
		levelNum = TrackSelection.SelectedTrackNumber;
		isGameOver = false;
		alreadyUploadedHighscore = false;
		currentGameTimeMilliseconds = 0f;
		bestLapTimeMilliseconds = 0f;
		lap = 0;
		victory = false;
		zoomInTime = 5000f;
		firstFrame = true;
	}

	public virtual void ClearVariablesForGameOver()
	{
	}

	public virtual void Update()
	{
		if (firstFrame)
		{
			firstFrame = false;
			return;
		}
		if (!RacingGameManager.InMenu && zoomInTime > 0f)
		{
			float num = zoomInTime;
			zoomInTime -= BaseGame.ElapsedTimeThisFrameInMilliseconds;
			if (zoomInTime < 2000f && (int)((num + 1000f) / 1000f) != (int)((zoomInTime + 1000f) / 1000f))
			{
				RacingGameManager.Landscape.ReplaceStartLightObject(2 - (int)((zoomInTime + 1000f) / 1000f));
			}
		}
		if (CanControlCar)
		{
			if (Input.IsGamePadConnected && !Guide.IsVisible && !game_paused)
			{
				currentGameTimeMilliseconds += BaseGame.ElapsedTimeThisFrameInMilliseconds;
			}
			if (Input.IsGamePadConnected && !Guide.IsVisible && Input.GamePadStartJustPressed)
			{
				game_paused = !game_paused;
			}
		}
	}
}
