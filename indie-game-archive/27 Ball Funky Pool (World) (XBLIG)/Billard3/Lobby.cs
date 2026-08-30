using System;
using System.Collections.Generic;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class Lobby : GameComponent
{
	public enum PlayerState
	{
		UNINIT,
		NO_TEAM,
		TEAM_A,
		TEAM_B
	}

	public class PlayerInfo
	{
		private const int updateColorTimeInFrames = 15;

		public const double transMaxTime = 0.33;

		public PlayerState statePrevious;

		public PlayerState stateNext;

		public float transitionColor;

		public double startTime;

		public float TransitionRatio;

		public bool ready;

		public int indexInTeam;

		public Point OffsetChooseTeam;

		private Vector2 transStart;

		private Vector2 transEnd;

		private double transStartTime;

		public Point TransEnd => new Point((int)transEnd.X, (int)transEnd.Y);

		public Point PositionTransition(GameTime gameTime)
		{
			float num = Utils.SmoothStep(Timer.Ratio(gameTime, transStartTime, 0.33));
			if (statePrevious != PlayerState.NO_TEAM)
			{
				num = 1f - num;
			}
			Vector2 vector = Utils.LerpVector2(transStart, transEnd, num);
			return new Point((int)vector.X, (int)vector.Y);
		}

		public void StartTransition(Vector2 start, Vector2 end)
		{
			transStart = start;
			transEnd = end;
		}

		public PlayerInfo()
		{
			statePrevious = PlayerState.UNINIT;
			stateNext = PlayerState.UNINIT;
			transitionColor = 1f;
		}

		public void Change(PlayerState newState, GameTime gameTime)
		{
			Audio.PlaySFX(Audio.SFXID.Menu);
			ready = false;
			transitionColor = 0f;
			statePrevious = stateNext;
			stateNext = newState;
			startTime = gameTime.TotalGameTime.TotalSeconds;
			switch (stateNext)
			{
			case PlayerState.NO_TEAM:
				if (statePrevious == PlayerState.UNINIT)
				{
					OffsetChooseTeam = Point.Zero;
					break;
				}
				transStartTime = gameTime.TotalGameTime.TotalSeconds;
				transStart -= new Vector2(OffsetChooseTeam.X, OffsetChooseTeam.Y);
				break;
			case PlayerState.TEAM_A:
			case PlayerState.TEAM_B:
				transStartTime = gameTime.TotalGameTime.TotalSeconds;
				break;
			}
		}

		public void Update(GameTime gameTime)
		{
			transitionColor = Utils.incrementRatio(transitionColor, 15);
			if (stateNext == PlayerState.NO_TEAM)
			{
				float num = ((statePrevious == PlayerState.UNINIT) ? 0.25f : 0.33f);
				OffsetChooseTeam = new Point((int)((double)((float)Statics.draw2D.ScreenSizePoint.X * 0.015f) * Math.Sin((gameTime.TotalGameTime.TotalSeconds - (double)num - startTime) * 4.0)), 0);
				if (statePrevious != PlayerState.UNINIT)
				{
					TransitionRatio = Timer.Ratio(gameTime, startTime, 0.33);
				}
			}
			else if (statePrevious == PlayerState.NO_TEAM && stateNext != PlayerState.UNINIT)
			{
				TransitionRatio = Timer.Ratio(gameTime, startTime, 0.33);
			}
			else
			{
				TransitionRatio = 1f;
			}
		}
	}

	public const int maxPlayers = 4;

	public const int maxTeams = 2;

	private Menus.ScreenInfo[] teamScreens = new Menus.ScreenInfo[2];

	private Menus.ScreenInfo[] pScreens = new Menus.ScreenInfo[4];

	private Menus.ScreenInfo title;

	private bool disable;

	private static readonly Color colorOFF = Utils.ColorWithAlpha(Color.White, 0.5f);

	private static readonly Color colorON = GameMenus.ColorOutline;

	private PlayerInfo[] pInfo = new PlayerInfo[4];

	public List<PlayerIndex> teamA = new List<PlayerIndex>();

	public List<PlayerIndex> teamB = new List<PlayerIndex>();

	private void ChangeMessage(int index)
	{
		pScreens[index].entries.Clear();
		pScreens[index].AddNonSelectableEntry("           " + GameModeRules.Team.NameOf((PlayerIndex)index) + "           ", overrideSelectionTransition: true);
		switch (pInfo[index].stateNext)
		{
		case PlayerState.UNINIT:
			pScreens[index].AddNonSelectableEntry("PRESS A", overrideSelectionTransition: true);
			break;
		case PlayerState.NO_TEAM:
			pScreens[index].AddNonSelectableEntry("? TEAM ?", overrideSelectionTransition: true);
			break;
		case PlayerState.TEAM_A:
		case PlayerState.TEAM_B:
			if (pInfo[index].ready)
			{
				pScreens[index].AddEntryValue(new Menus.MenuEntryValue<Texture2D>(0, GameMenus.Textures.checkMark, Statics.draw2D.Font, 0));
			}
			else
			{
				pScreens[index].AddNonSelectableEntry(" ? READY ? ", overrideSelectionTransition: true);
			}
			break;
		}
		pScreens[index].DefaultSelection = -2;
	}

	public Lobby(Game game)
		: base(game)
	{
		game.Components.Add(this);
		disable = false;
		title = new Menus.ScreenInfo(1, "", new Vector2(0.5f, 0.15f), 1f);
		title.AddEntry("   CHOOSE YOUR TEAM   ", 0);
		Statics.menus.AddScreenInfo(title);
		Point screenSizePoint = Statics.draw2D.ScreenSizePoint;
		float num = 0.15f;
		Vector2 posAsRatio = new Vector2(0.5f, 0.6f - num * 1.5f);
		int borderOverride = Menus.ManagerV2.DefaultBorder / 3;
		for (int i = 0; i < 4; i++)
		{
			pScreens[i] = new Menus.ScreenInfo(1, "", posAsRatio, borderOverride, 0.9f);
			Statics.menus.AddScreenInfo(pScreens[i]);
			posAsRatio.Y += num;
		}
		Menus.MenuEntryValue<Texture2D> e = new Menus.MenuEntryValue<Texture2D>(0, new Texture2D(game.GraphicsDevice, screenSizePoint.X / 7, (int)((float)screenSizePoint.Y * 0.45f)), Statics.draw2D.Font, 0);
		Vector2 posAsRatio2 = new Vector2(0.3f, 0.6f);
		string text = "BLUE";
		for (int j = 0; j < 2; j++)
		{
			teamScreens[j] = new Menus.ScreenInfo(1, "", posAsRatio2, 1f);
			teamScreens[j].SetColorOverlayOverride(GameModeRules.Team.Colors[j]);
			Menus.MenuEntryValue<string> menuEntryValue = new Menus.MenuEntryValue<string>(0, "TEAM " + text, Statics.draw2D.Font, 0);
			menuEntryValue.OverrideStringColorFront(GameModeRules.Team.Colors[j]);
			teamScreens[j].AddEntryValue(menuEntryValue);
			teamScreens[j].AddEntryValue(e);
			Statics.menus.AddScreenInfo(teamScreens[j]);
			posAsRatio2 = new Vector2(1f - posAsRatio2.X, posAsRatio2.Y);
			text = "RED";
		}
	}

	public void Enable(GameTime gameTime)
	{
		for (int i = 0; i < 4; i++)
		{
			pInfo[i] = new PlayerInfo();
			ChangeMessage(i);
		}
		disable = false;
		Statics.menus.SwitchAllScreenInfoWithID(gameTime, 1, value: true);
	}

	public void HandleInput(GameTime gameTime, PlayerIndex pInd, Utils.Input.ActionMenu action)
	{
		PlayerInfo playerInfo = pInfo[(int)pInd];
		_ = pScreens[(int)pInd];
		if (playerInfo.TransitionRatio < 1f)
		{
			return;
		}
		if (pInd == Statics.input.PlayerIndex && playerInfo.stateNext == PlayerState.UNINIT && action == Utils.Input.ActionMenu.MENU_BACK)
		{
			Disable(gameTime);
		}
		switch (playerInfo.stateNext)
		{
		case PlayerState.UNINIT:
			if (action == Utils.Input.ActionMenu.MENU_ACTIVATE)
			{
				playerInfo.Change(PlayerState.NO_TEAM, gameTime);
				ChangeMessage((int)pInd);
			}
			break;
		case PlayerState.NO_TEAM:
			switch (action)
			{
			case Utils.Input.ActionMenu.MENU_BACK:
				playerInfo.Change(PlayerState.UNINIT, gameTime);
				ChangeMessage((int)pInd);
				break;
			case Utils.Input.ActionMenu.MENU_LEFT:
				if (TeamNotFull(PlayerState.TEAM_A))
				{
					playerInfo.indexInTeam = (TeamNotEmpty(PlayerState.TEAM_A) ? 1 : 0);
					playerInfo.Change(PlayerState.TEAM_A, gameTime);
					ChangeMessage((int)pInd);
					StartTransition((int)pInd);
				}
				break;
			case Utils.Input.ActionMenu.MENU_RIGHT:
				if (TeamNotFull(PlayerState.TEAM_B))
				{
					playerInfo.indexInTeam = (TeamNotEmpty(PlayerState.TEAM_B) ? 1 : 0);
					playerInfo.Change(PlayerState.TEAM_B, gameTime);
					ChangeMessage((int)pInd);
					StartTransition((int)pInd);
				}
				break;
			}
			break;
		case PlayerState.TEAM_A:
		case PlayerState.TEAM_B:
			if (!playerInfo.ready && ((action == Utils.Input.ActionMenu.MENU_RIGHT && playerInfo.stateNext == PlayerState.TEAM_A) || (action == Utils.Input.ActionMenu.MENU_LEFT && playerInfo.stateNext == PlayerState.TEAM_B)))
			{
				playerInfo.Change(PlayerState.NO_TEAM, gameTime);
				ChangeMessage((int)pInd);
				break;
			}
			switch (action)
			{
			case Utils.Input.ActionMenu.MENU_ACTIVATE:
				if (!playerInfo.ready)
				{
					Audio.PlaySFX(Audio.SFXID.Menu);
				}
				playerInfo.ready = true;
				ChangeMessage((int)pInd);
				break;
			case Utils.Input.ActionMenu.MENU_BACK:
				if (playerInfo.ready)
				{
					Audio.PlaySFX(Audio.SFXID.Menu);
				}
				playerInfo.ready = false;
				ChangeMessage((int)pInd);
				break;
			}
			break;
		}
		Update(gameTime);
	}

	private bool TeamNotEmpty(PlayerState team)
	{
		PlayerInfo[] array = pInfo;
		foreach (PlayerInfo playerInfo in array)
		{
			if (playerInfo.stateNext == team)
			{
				return true;
			}
		}
		return false;
	}

	private bool TeamNotFull(PlayerState team)
	{
		int num = 0;
		PlayerInfo[] array = pInfo;
		foreach (PlayerInfo playerInfo in array)
		{
			if (playerInfo.stateNext == team)
			{
				num++;
			}
		}
		return num < 2;
	}

	private void StartTransition(int pInd)
	{
		PlayerInfo playerInfo = pInfo[pInd];
		Menus.ScreenInfo screenInfo = pScreens[pInd];
		Vector2 start = new Vector2(screenInfo.Overlay.Center.X + playerInfo.OffsetChooseTeam.X, screenInfo.Overlay.Center.Y + playerInfo.OffsetChooseTeam.Y);
		Rectangle overlay = ((playerInfo.stateNext == PlayerState.TEAM_A) ? teamScreens[0] : teamScreens[1]).Overlay;
		Vector2 end = new Vector2(overlay.Center.X, overlay.Center.Y);
		bool flag = false;
		for (int i = 0; i < 4; i++)
		{
			if (i != pInd && pInfo[i].stateNext == playerInfo.stateNext && pInfo[i].indexInTeam == 0)
			{
				flag = true;
			}
		}
		end.Y = (float)overlay.Y + (float)overlay.Height * (flag ? 0.66f : 0.33f);
		playerInfo.StartTransition(start, end);
	}

	private void Disable(GameTime gameTime)
	{
		Statics.menus.SwitchAllScreenInfoWithID(gameTime, 1, value: false);
		disable = true;
	}

	public bool HasJoined(int pInd)
	{
		return pInfo[pInd].ready;
	}

	public override void Update(GameTime gameTime)
	{
		if (GameState.Current != GameState.Type.LOBBY)
		{
			if (teamScreens[0].state == Menus.Screen.State.Active)
			{
				Disable(gameTime);
			}
			return;
		}
		int num = 0;
		int num2 = 0;
		teamA.Clear();
		teamB.Clear();
		if (disable && teamScreens[0].state == Menus.Screen.State.Hidden)
		{
			disable = false;
			GameState.Change(GameState.Type.MENUS, gameTime);
			Statics.menus.Enable();
		}
		if (teamScreens[0].state == Menus.Screen.State.Hidden)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			PlayerInfo playerInfo = pInfo[i];
			Menus.ScreenInfo screenInfo = pScreens[i];
			playerInfo.Update(gameTime);
			switch (playerInfo.stateNext)
			{
			case PlayerState.UNINIT:
				screenInfo.SetColorOverlayOverride(Utils.LerpColor(colorOFF, colorON, 1f - playerInfo.transitionColor));
				break;
			case PlayerState.NO_TEAM:
				num2++;
				if (playerInfo.statePrevious == PlayerState.UNINIT)
				{
					screenInfo.SetColorOverlayOverride(Utils.LerpColor(colorOFF, colorON, playerInfo.transitionColor));
					if (playerInfo.transitionColor == 1f)
					{
						screenInfo.UpdatePositions(playerInfo.OffsetChooseTeam);
					}
				}
				else if (playerInfo.TransitionRatio < 1f)
				{
					screenInfo.OverridePositions(playerInfo.PositionTransition(gameTime));
				}
				else
				{
					screenInfo.UpdatePositions(playerInfo.OffsetChooseTeam);
				}
				break;
			case PlayerState.TEAM_A:
			case PlayerState.TEAM_B:
			{
				if (playerInfo.ready)
				{
					num++;
					((playerInfo.stateNext == PlayerState.TEAM_A) ? teamA : teamB).Add((PlayerIndex)i);
				}
				else
				{
					num2++;
				}
				Point newPos = ((playerInfo.TransitionRatio < 1f) ? playerInfo.PositionTransition(gameTime) : playerInfo.TransEnd);
				screenInfo.OverridePositions(newPos);
				break;
			}
			}
		}
		if (num2 == 0 && num >= 2)
		{
			GameState.Change(GameState.Type.CHEAT_PROMPT, gameTime);
		}
		base.Update(gameTime);
	}
}
