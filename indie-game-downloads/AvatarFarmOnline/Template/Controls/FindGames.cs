using System;
using System.Collections.Generic;
using AvatarFarmOnline.Logic.Mode.Farm;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Network;
using Quasar.GameUtils.Tasks;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace AvatarFarmOnline.Template.Controls;

internal class FindGames : InteractiveControl
{
	public enum FindGamesState
	{
		Finding,
		Found,
		Joining,
		Joined,
		ErrorJoining,
		NotFound
	}

	public const string Type = "FindGames";

	public const int ResultsPerPage = 4;

	private FindGamesState state;

	private List<IAvailableSession> availableSessions = new List<IAvailableSession>(4);

	private IAvailableSessionCollection results;

	private int baseIndex;

	private int lastSelectedIndex;

	private int selectedIndex;

	private SessionJoinError error;

	private bool successJoining;

	public override string ControlType => "FindGames";

	public FindGamesState State => state;

	public int ResultCount => results.Count;

	public int BaseIndex => baseIndex;

	public int SelectedIndex => selectedIndex;

	public event Action<IAvailableSession> OnSelected;

	public event Action<AvatarFarmOnline.Template.Controls.FindGames, PlayerIndex> OnCancel;

	public event Action<AvatarFarmOnline.Template.Controls.FindGames> OnMove;

	public event Action<AvatarFarmOnline.Template.Controls.FindGames> OnNewResults;

	public event Action<AvatarFarmOnline.Template.Controls.FindGames> OnJoined;

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		return false;
	}

	public FindGames(Layout layout)
		: base(layout)
	{
		Find();
	}

	private void Find()
	{
		state = FindGamesState.Finding;
		TaskManager.Post(Search, null, SearchFinished);
	}

	public List<IAvailableSession> GetEntries()
	{
		availableSessions.Clear();
		for (int i = baseIndex; i < ResultCount && i < baseIndex + 4; i++)
		{
			availableSessions.Add(results[i]);
		}
		return availableSessions;
	}

	private void Search(object parameters)
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		results = farmPersistentGameData.Online.FindGames();
	}

	private void SearchFinished(object parameters)
	{
		state = FindGamesState.NotFound;
		if (results != null && results.Count > 0)
		{
			baseIndex = (lastSelectedIndex = (selectedIndex = 0));
			state = FindGamesState.Found;
			if (OnNewResults != null)
			{
				OnNewResults(this);
			}
		}
	}

	public override void Update()
	{
		PlayerIndex whoPressed = PlayerIndex.One;
		switch (state)
		{
		case FindGamesState.Finding:
			if (InputManager.MenuCancel(ref whoPressed) && OnCancel != null)
			{
				OnCancel(this, whoPressed);
			}
			break;
		case FindGamesState.ErrorJoining:
		case FindGamesState.NotFound:
			if (InputManager.MenuSecondary())
			{
				Find();
			}
			if (InputManager.MenuCancel(ref whoPressed) && OnCancel != null)
			{
				OnCancel(this, whoPressed);
			}
			break;
		case FindGamesState.Found:
			if (InputManager.MenuDownRepeat())
			{
				selectedIndex = GameMath.Clamp(0, ResultCount - 1, selectedIndex + 1);
			}
			if (InputManager.MenuUpRepeat())
			{
				selectedIndex = GameMath.Clamp(0, ResultCount - 1, selectedIndex - 1);
			}
			if (InputManager.MenuPrevPageRepeat())
			{
				selectedIndex = GameMath.Clamp(0, ResultCount - 1, selectedIndex - 4);
			}
			if (InputManager.MenuNextPageRepeat())
			{
				selectedIndex = GameMath.Clamp(0, ResultCount - 1, selectedIndex + 4);
			}
			if (InputManager.MenuGoToLast())
			{
				selectedIndex = Math.Max(0, ResultCount - 1);
			}
			if (InputManager.MenuGoToFirst())
			{
				selectedIndex = 0;
			}
			if (selectedIndex != lastSelectedIndex)
			{
				baseIndex = GameMath.Clamp(0, ResultCount - 4, selectedIndex - 2);
				lastSelectedIndex = selectedIndex;
				if (OnMove != null)
				{
					OnMove(this);
				}
			}
			if (InputManager.MenuInteract())
			{
				state = FindGamesState.Joining;
				TaskManager.Post(TryJoin, results[selectedIndex], TryJoinFinished);
				if (OnSelected != null)
				{
					OnSelected(results[selectedIndex]);
				}
			}
			if (InputManager.MenuSecondary())
			{
				Find();
			}
			if (InputManager.MenuCancel(ref whoPressed) && OnCancel != null)
			{
				OnCancel(this, whoPressed);
			}
			break;
		case FindGamesState.Joining:
		case FindGamesState.Joined:
			break;
		}
	}

	private void TryJoin(object parameters)
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		IAvailableSession ans = (IAvailableSession)parameters;
		successJoining = farmPersistentGameData.Online.TryJoin(ans, out error);
	}

	private void TryJoinFinished(object parameters)
	{
		if (successJoining)
		{
			state = FindGamesState.Joined;
			if (OnJoined != null)
			{
				OnJoined(this);
			}
		}
		else
		{
			state = FindGamesState.ErrorJoining;
			string message = error switch
			{
				SessionJoinError.SessionFull => "ERROR_SESSION_FULL".Translate(), 
				SessionJoinError.SessionNotJoinable => "ERROR_SESSION_NOT_JOINABLE".Translate(), 
				_ => "ERROR_SESSION_NOT_FOUND".Translate(), 
			};
			Layout.ShowMessage("ERROR".Translate(), message);
		}
	}
}
