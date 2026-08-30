using System;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace Billard3;

public class Input : InputV2
{
	public enum Action
	{
		EXIT = 0,
		ACTIVATE = 1,
		CAM_SWITCH = 5,
		PAUSE = 6,
		BACK = 7,
		PURCHASE = 8,
		SWITCH_MUSIC = 9
	}

	private bool init;

	public Input()
	{
		ButtonEventTriggered(PlayerIndex.One, Action.ACTIVATE);
	}

	private static Buttons Mapping(Action action)
	{
		return action switch
		{
			Action.EXIT => Buttons.Back, 
			Action.ACTIVATE => Buttons.A, 
			Action.CAM_SWITCH => Buttons.X, 
			Action.PAUSE => Buttons.Start, 
			Action.BACK => Buttons.B, 
			Action.PURCHASE => Buttons.Y, 
			Action.SWITCH_MUSIC => Buttons.Back, 
			_ => throw new Exception("action not supported " + action), 
		};
	}

	public static PressOrRelease IsActionOnPressOrRelease(Action action)
	{
		if (action == Action.ACTIVATE)
		{
			return GameState.Current switch
			{
				GameState.Type.AIMING => PressOrRelease.Press, 
				_ => PressOrRelease.Release, 
			};
		}
		return PressOrRelease.Release;
	}

	private bool ButtonEventTriggered(PlayerIndex pIndex, Action action)
	{
		if (IsActionOnPressOrRelease(action) == PressOrRelease.Press)
		{
			return isPressed(Multiplayer.CurrentState(pIndex), Mapping(action));
		}
		return justReleased(Multiplayer.CurrentState(pIndex), Multiplayer.PreviousState(pIndex), Mapping(action));
	}

	public void Update(GameTime gameTime)
	{
		if (Statics.ContentLoadedTime < 0.0 || Timer.Ratio(gameTime, Statics.ContentLoadedTime + 0.5, 2.0) < 1f || (!base.PadIndexFound && Statics.menus.PromptEndTrial) || !UpdatePre(gameTime))
		{
			return;
		}
		if (!init)
		{
			if (Statics.callbacks.DeferredEnableMenusTime != -2.0)
			{
				return;
			}
			Statics.menus.WantedScreenId = 0;
			Statics.menus.Enable();
			init = true;
		}
		GetCurrentState();
		HandleInput(gameTime);
		UpdatePreviousStates();
	}

	private void HandleInput(GameTime gameTime)
	{
		if (ButtonEventTriggered(base.PlayerIndex, Action.SWITCH_MUSIC))
		{
			Audio.SongStatus = !Audio.SongStatus;
		}
		if (GameState.Current == GameState.Type.MENUS)
		{
			ActionMenu[] actionMenuArray = Utils.Input.ActionMenuArray;
			foreach (ActionMenu action in actionMenuArray)
			{
				if (ButtonEventTriggered(action))
				{
					Statics.menus.HandleInput(action);
				}
			}
			if (Statics.menus.PromptPurchase && ButtonEventTriggered(base.PlayerIndex, Action.PURCHASE))
			{
				Guide.ShowMarketplace(base.PlayerIndex);
			}
			if (Statics.menus.PromptEndTrial)
			{
				if (Statics.callbacks.GameIsActive && Trial.UserCanPurchase(base.PlayerIndex) && ButtonEventTriggered(base.PlayerIndex, Action.PURCHASE))
				{
					Guide.ShowMarketplace(base.PlayerIndex);
				}
				else if (ButtonEventTriggered(ActionMenu.MENU_BACK))
				{
					Statics.callbacks.Exit();
				}
			}
			return;
		}
		if (GameState.SpecialMenuInput)
		{
			PlayerIndex[] allIndexes = Multiplayer.AllIndexes;
			foreach (PlayerIndex playerIndex in allIndexes)
			{
				bool flag = Statics.menus.Paused && playerIndex == Statics.menus.PauseController;
				if (flag && (ButtonEventTriggered(playerIndex, Action.PAUSE) || ButtonEventTriggered(playerIndex, Action.BACK)))
				{
					Statics.callbacks.PauseOFF();
				}
				ActionMenu[] actionMenuArray2 = Utils.Input.ActionMenuArray;
				foreach (ActionMenu actionMenu in actionMenuArray2)
				{
					if (ButtonEventTriggered(Multiplayer.CurrentJoystickAction(playerIndex), Multiplayer.PreviousJoystickAction(playerIndex), Multiplayer.CurrentState(playerIndex), Multiplayer.PreviousState(playerIndex), actionMenu))
					{
						if (flag)
						{
							Statics.callbacks.PauseMenuHandleInput(actionMenu);
						}
						else if (GameState.Current == GameState.Type.LOBBY)
						{
							Statics.lobby.HandleInput(gameTime, playerIndex, actionMenu);
						}
						else if (GameState.Current == GameState.Type.CHEAT_PROMPT)
						{
							Statics.cheatPrompt.HandleInput(gameTime, playerIndex, actionMenu);
						}
					}
				}
			}
			return;
		}
		PlayerIndex playerIndex2 = (GameState.AllowMultiplayerInput ? GameModeRules.CurrentPlayer : base.PlayerIndex);
		if (playerIndex2 == GameModeRules.IndexCPU)
		{
			if (GameModeRules.type == GameModeRules.Type.SinglePlayer && ButtonEventTriggered(base.PlayerIndex, Action.PAUSE))
			{
				Statics.callbacks.PauseON(base.PlayerIndex);
			}
			return;
		}
		if (!GameState.DisableActivate && ButtonEventTriggered(playerIndex2, Action.ACTIVATE))
		{
			Statics.callbacks.Activate(gameTime);
		}
		if (GameState.Current != GameState.Type.CHEAT_PROMPT && GameState.Current != GameState.Type.CHOOSING_POWER && GameState.Current != GameState.Type.GAME_OVER && GameState.Current != GameState.Type.LOBBY && GameState.Current != GameState.Type.MENUS && ButtonEventTriggered(playerIndex2, Action.PAUSE))
		{
			Statics.callbacks.PauseON(playerIndex2);
		}
		if (GameState.Current != GameState.Type.GAME_OVER && ButtonEventTriggered(playerIndex2, Action.CAM_SWITCH))
		{
			Statics.cam.Switch(gameTime);
		}
		_ = Vector2.Zero;
		Statics.callbacks.InputDirection(DPadVectorStatic(Multiplayer.CurrentState(playerIndex2)), highSensitivity: true);
		Statics.callbacks.InputDirection(Multiplayer.CurrentState(playerIndex2).ThumbSticks.Left, highSensitivity: false);
		Statics.callbacks.InputDirection(Multiplayer.CurrentState(playerIndex2).ThumbSticks.Right, highSensitivity: true);
	}
}
