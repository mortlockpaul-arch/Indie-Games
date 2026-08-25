using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar.Screens;

public class Credits
{
	private DateTime startTime = DateTime.MinValue;

	public Texture2D background => GraphicsManager.LoadTexture("screens/credits.png", cacheResult: true);

	public Credits()
	{
		Load(RandomStaticGlobals.Content);
	}

	public void Draw()
	{
		if (startTime == DateTime.MinValue)
		{
			startTime = DateTime.Now;
		}
		int num = -(int)((DateTime.Now - startTime).TotalSeconds * 100.0);
		num += 500;
		if (num > 0)
		{
			num = 0;
		}
		int num2 = -8120;
		if (num < num2)
		{
			num = num2;
		}
		GraphicsManager.DrawTexture(GraphicsManager.LoadTexture("screens/credits1.png", cacheResult: true), new Rectangle(0, num, 1920, 1200), Color.White);
		GraphicsManager.DrawTexture(GraphicsManager.LoadTexture("screens/credits2.png", cacheResult: true), new Rectangle(0, 1200 + num, 1920, 2000), Color.White);
		GraphicsManager.DrawTexture(GraphicsManager.LoadTexture("screens/credits3.png", cacheResult: true), new Rectangle(0, 3200 + num, 1920, 2000), Color.White);
		GraphicsManager.DrawTexture(GraphicsManager.LoadTexture("screens/credits4.png", cacheResult: true), new Rectangle(0, 5200 + num, 1920, 2000), Color.White);
		GraphicsManager.DrawTexture(GraphicsManager.LoadTexture("screens/credits5.png", cacheResult: true), new Rectangle(0, 7200 + num, 1920, 2000), Color.White);
	}

	public void ProcessUnsignedInput()
	{
		if (GamePad.GetState(PlayerIndex.One).IsConnected)
		{
			InputFromAnywhere playerInput = InputManager.GetPlayerInput(PlayerIndex.One, ref InputManager.gamePad1previous, ref InputManager.nullKeyboard);
			FigureOutInput(playerInput, PlayerIndex.One);
		}
		if (GamePad.GetState(PlayerIndex.Two).IsConnected)
		{
			InputFromAnywhere playerInput = InputManager.GetPlayerInput(PlayerIndex.Two, ref InputManager.gamePad2previous, ref InputManager.nullKeyboard);
			FigureOutInput(playerInput, PlayerIndex.Two);
		}
		if (GamePad.GetState(PlayerIndex.Three).IsConnected)
		{
			InputFromAnywhere playerInput = InputManager.GetPlayerInput(PlayerIndex.Three, ref InputManager.gamePad3previous, ref InputManager.nullKeyboard);
			FigureOutInput(playerInput, PlayerIndex.Three);
		}
		if (GamePad.GetState(PlayerIndex.Four).IsConnected)
		{
			InputFromAnywhere playerInput = InputManager.GetPlayerInput(PlayerIndex.Four, ref InputManager.gamePad4previous, ref InputManager.nullKeyboard);
			FigureOutInput(playerInput, PlayerIndex.Four);
		}
	}

	public void ProcessInput()
	{
		List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: false, canBeDying: true);
		if (humanPlayers.Count == 0)
		{
			ScreenManager.ShowMainMenu();
		}
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.HasValue && FighterManager.humanPlayers[i].PROPERTIES.isLocal)
			{
				InputFromAnywhere playerInput = InputManager.GetPlayerInput(FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.Value, ref FighterManager.humanPlayers[i].PROPERTIES.previousGamePadState, ref InputManager.previousKeyboardStateMenu);
				FigureOutInput(playerInput, FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.Value);
			}
		}
	}

	private void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex pi)
	{
		if (anywhereInput.B_pressed || anywhereInput.A_pressed)
		{
			exit();
		}
	}

	private void exit()
	{
		SoundManager.PlayMenuClick();
		startTime = DateTime.MinValue;
		ScreenManager.ShowWorldMap();
	}

	public void Load(ContentManager Content)
	{
		_ = background;
	}

	public void Clear()
	{
		background.Dispose();
	}
}
