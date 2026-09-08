using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class InGameMenuE : Menu
{
	public PlayerBase playerRef;

	public MenuInput CurrentInput;

	public InGameMenuE(GameMenus id)
		: base(id)
	{
	}

	public override void ResetMenuEntries()
	{
		base.ResetMenuEntries();
	}

	public override void HandleInput()
	{
		CurrentInput = MenuInput.None;
		if (state != MenuState.Active || menuEntryList.Count < 1 || playerRef == null)
		{
			return;
		}
		Vector2 left = playerRef.currentGamePadState.ThumbSticks.Left;
		Vector2 left2 = playerRef.lastGamePadState.ThumbSticks.Left;
		Vector2 right = playerRef.currentGamePadState.ThumbSticks.Right;
		Vector2 right2 = playerRef.lastGamePadState.ThumbSticks.Right;
		GamePadButtons buttons = playerRef.currentGamePadState.Buttons;
		GamePadButtons buttons2 = playerRef.lastGamePadState.Buttons;
		GamePadDPad gamePadDPad = playerRef.currentGamePadState.DPad;
		GamePadDPad gamePadDPad2 = playerRef.lastGamePadState.DPad;
		if (buttons.A == ButtonState.Pressed && buttons2.A == ButtonState.Released)
		{
			CurrentInput = MenuInput.MenuSelect;
		}
		else if (buttons.B == ButtonState.Pressed && buttons2.B == ButtonState.Released)
		{
			CurrentInput = MenuInput.MenuBack;
		}
		else if ((left.Y > 0.5f && left2.Y < 0.5f) || (gamePadDPad.Up == ButtonState.Pressed && gamePadDPad2.Up == ButtonState.Released))
		{
			CurrentInput = MenuInput.MenuUp;
		}
		else if ((left.Y < -0.5f && left2.Y > -0.5f) || (gamePadDPad.Down == ButtonState.Pressed && gamePadDPad2.Down == ButtonState.Released))
		{
			CurrentInput = MenuInput.MenuDown;
		}
		else if ((right.X > 0.5f && right2.X < 0.5f) || (gamePadDPad.Right == ButtonState.Pressed && gamePadDPad2.Right == ButtonState.Released))
		{
			CurrentInput = MenuInput.MenuRight;
		}
		else if ((right.X < -0.5f && right2.X > -0.5f) || (gamePadDPad.Left == ButtonState.Pressed && gamePadDPad2.Left == ButtonState.Released))
		{
			CurrentInput = MenuInput.MenuLeft;
		}
		else if (right.X > 0.5f || gamePadDPad.Right == ButtonState.Pressed)
		{
			CurrentInput = MenuInput.MenuRightPressed;
		}
		else if (right.X < -0.5f || gamePadDPad.Left == ButtonState.Pressed)
		{
			CurrentInput = MenuInput.MenuLeftPressed;
		}
		if (CurrentInput == MenuInput.MenuSelect && menuEntryList[SelectedEntry].strikeOutText == null)
		{
			menuEntryList[SelectedEntry].TrySelected();
		}
		else if (CurrentInput == MenuInput.MenuUp)
		{
			SelectedEntry--;
			if (SelectedEntry < 0)
			{
				SelectedEntry = menuEntryList.Count - 1;
			}
			if (menuEntryList[SelectedEntry].strikeOutText != null)
			{
				HandleInput();
			}
		}
		else if (CurrentInput == MenuInput.MenuDown)
		{
			SelectedEntry++;
			if (SelectedEntry >= menuEntryList.Count)
			{
				SelectedEntry = 0;
			}
		}
		else if (menuEntryList[SelectedEntry].strikeOutText != null)
		{
			SelectedEntry++;
			if (SelectedEntry >= menuEntryList.Count)
			{
				SelectedEntry = 0;
			}
		}
		for (int i = 0; i < menuEntryList.Count; i++)
		{
			menuEntryList[i].isSelected = false;
		}
		menuEntryList[SelectedEntry].isSelected = true;
	}
}
