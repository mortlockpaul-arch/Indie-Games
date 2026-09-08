using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Scores;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace Quasar.GameUtils.Template.Controls;

public class ProHighscores<T> : InteractiveControl where T : Highscore, IEquatable<T>, IComparable<T>, new()
{
	public enum ScoreLocations
	{
		Global,
		Friends,
		Local,
		Count
	}

	public const string Type = "HighScores";

	private ScoreManager<T> scoreManager;

	private int scoresPerPage = 15;

	private ScorePeriod scorePeriod;

	private ScoreLocations scoreLocation;

	private List<T> scores;

	private List<T> scoreList;

	private int scoreCount;

	private int baseIndex;

	private bool allowSendMessages = true;

	private bool allowChangeLocation = true;

	public override string ControlType => "HighScores";

	public ScoreManager<T> ScoreManager => scoreManager;

	public int ScoresPerPage
	{
		get
		{
			return scoresPerPage;
		}
		set
		{
			scoresPerPage = value;
		}
	}

	public ScorePeriod ScorePeriod
	{
		get
		{
			return scorePeriod;
		}
		set
		{
			scorePeriod = value;
		}
	}

	public ScoreLocations ScoreLocation
	{
		get
		{
			return scoreLocation;
		}
		set
		{
			scoreLocation = value;
		}
	}

	public int BaseIndex => baseIndex;

	public bool AllowSendMessages
	{
		get
		{
			return allowSendMessages;
		}
		set
		{
			allowSendMessages = value;
		}
	}

	public bool AllowChangeLocation
	{
		get
		{
			return allowChangeLocation;
		}
		set
		{
			allowChangeLocation = value;
		}
	}

	public event Action<ProHighscores<T>, PlayerIndex> OnCancel;

	public event Action<ProHighscores<T>> OnMove;

	public event Action<ProHighscores<T>> OnDataUpdated;

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		return false;
	}

	public List<T> GetEntries()
	{
		scoreList.Clear();
		for (int i = baseIndex; i < scoreCount && i < baseIndex + ScoresPerPage; i++)
		{
			scoreList.Add(scores[i]);
		}
		return scoreList;
	}

	private void updateData()
	{
		switch (scoreLocation)
		{
		case ScoreLocations.Friends:
		{
			List<PlayerIndex> playerIndices = InputManager.PlayerIndices;
			if (playerIndices.Count > 0)
			{
				PlayerIndex playerIndex = playerIndices[0];
				scoreCount = scoreManager.ScoreOrganizer.GetFriendHighScores(scores, playerIndex, scorePeriod);
			}
			break;
		}
		case ScoreLocations.Global:
			scoreCount = scoreManager.ScoreOrganizer.GetHighscores(scores, isGlobal: true, scorePeriod);
			break;
		case ScoreLocations.Local:
			scoreCount = scoreManager.ScoreOrganizer.GetHighscores(scores, isGlobal: false, scorePeriod);
			break;
		}
		baseIndex = GameMath.Clamp(0, scoreCount - 1, baseIndex);
	}

	public void goToFirstLocal()
	{
		baseIndex = 0;
		for (int i = 0; i < scoreCount; i++)
		{
			T val = scores[i];
			if (val.IsLocal)
			{
				baseIndex = Math.Max(0, i - (ScoresPerPage / 2 - 1));
				break;
			}
		}
	}

	public void goToFirstResult(PlayerIndex index)
	{
		string playerName = Quasar.GameUtils.Player.Player.GetPlayerName(index);
		baseIndex = 0;
		for (int i = 0; i < scoreCount; i++)
		{
			T val = scores[i];
			if (val.CompareGamer(playerName))
			{
				baseIndex = Math.Max(0, i - (ScoresPerPage / 2 - 1));
				break;
			}
		}
	}

	public T bestScore(string gamertag)
	{
		for (int i = 0; i < scoreCount; i++)
		{
			T result = scores[i];
			if (result.CompareGamer(gamertag))
			{
				return result;
			}
		}
		return null;
	}

	public ProHighscores(ScoreManager<T> scoreManager, Layout layout)
		: base(layout)
	{
		this.scoreManager = scoreManager;
		scoreManager.OnHighscoresChanged += scoreManager_OnHighscoresChanged;
		scoreList = new List<T>(ScoresPerPage);
		scores = new List<T>(Math.Max(scoreManager.LocalAggregateHighscores(ScorePeriod.AllTime).Count, scoreManager.AggregateHighscores(ScorePeriod.AllTime).Count));
		updateData();
	}

	private void scoreManager_OnHighscoresChanged()
	{
		updateData();
		if (OnDataUpdated != null)
		{
			OnDataUpdated(this);
		}
	}

	public void NotifyScores(PlayerIndex who)
	{
		if (!Layout.IsDialogShown && PlatformInterface.Instance.HasMessaging)
		{
			if (PlatformInterface.Instance.CanSendMessages(who))
			{
				scoreManager.ScoreOrganizer.NotifyScores(who);
			}
			else
			{
				Layout.ShowMessage("DOH".Translate(), "ERROR_CHALLENGE".Translate());
			}
		}
	}

	public override void Update()
	{
		bool flag = false;
		if (PlatformInterface.Instance.SupportsFriends && allowChangeLocation && InputManager.MenuSecondary())
		{
			scoreLocation = (ScoreLocations)((int)(scoreLocation + 1) % 3);
			flag = true;
		}
		if (scoreManager.SupportsPeriods && InputManager.MenuTerciary())
		{
			scorePeriod = (ScorePeriod)((int)(scorePeriod + 1) % 4);
			flag = true;
		}
		int num = baseIndex;
		if (InputManager.MenuDownRepeat())
		{
			baseIndex = GameMath.Clamp(0, scoreCount - ScoresPerPage, baseIndex + 1);
		}
		if (InputManager.MenuUpRepeat())
		{
			baseIndex = GameMath.Clamp(0, scoreCount - ScoresPerPage, baseIndex - 1);
		}
		if (InputManager.MenuPrevPageRepeat())
		{
			baseIndex = GameMath.Clamp(0, scoreCount - ScoresPerPage, baseIndex - ScoresPerPage);
		}
		if (InputManager.MenuNextPageRepeat())
		{
			baseIndex = GameMath.Clamp(0, scoreCount - ScoresPerPage, baseIndex + ScoresPerPage);
		}
		if (InputManager.MenuGoToLast())
		{
			baseIndex = Math.Max(0, scoreCount - ScoresPerPage);
		}
		if (InputManager.MenuGoToFirst())
		{
			baseIndex = 0;
		}
		PlayerIndex whoPressed = PlayerIndex.One;
		if (InputManager.MenuInteract(ref whoPressed))
		{
			goToFirstResult(whoPressed);
		}
		if (allowSendMessages && InputManager.MenuStart(ref whoPressed))
		{
			NotifyScores(whoPressed);
		}
		if (InputManager.MenuCancel(ref whoPressed) && OnCancel != null)
		{
			OnCancel(this, whoPressed);
		}
		if (flag)
		{
			updateData();
		}
		if ((num != baseIndex || flag) && OnMove != null)
		{
			OnMove(this);
		}
	}

	public override void Dispose()
	{
		if (scoreManager != null)
		{
			scoreManager.OnHighscoresChanged -= scoreManager_OnHighscoresChanged;
		}
		base.Dispose();
	}
}
