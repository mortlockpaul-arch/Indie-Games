using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class InputBase
{
	public static PlayerGamePadState[] playerGamePad = new PlayerGamePadState[4];

	public MenuInput menuInput;

	public MenuInput menuDPadInput;

	public MenuInput menuInputContinuos;

	public MenuInput menuInputRightStick;

	public PlayerIndex playerInputIndex;

	private static int ContinuosInputStep = 0;

	private static float ContinuosInputTime = 0f;

	public static GamePadState CurrentState(PlayerIndex i)
	{
		return playerGamePad[(int)i].currentGamePadState;
	}

	public static GamePadState LastState(PlayerIndex i)
	{
		return playerGamePad[(int)i].lastGamePadState;
	}

	public virtual void LoadContent()
	{
		for (int i = 0; i < 4; i++)
		{
			playerGamePad[i].currentGamePadState = GamePad.GetState(PlayerIndex.One);
			playerGamePad[i].lastGamePadState = playerGamePad[i].currentGamePadState;
		}
	}

	public virtual void UnLoadContent()
	{
	}

	public virtual void BeginUpdate(GameTime gameTime)
	{
		menuInput = MenuInput.None;
		menuDPadInput = MenuInput.None;
		menuInputRightStick = MenuInput.None;
		menuInputContinuos = MenuInput.None;
		for (playerInputIndex = PlayerIndex.One; playerInputIndex <= PlayerIndex.Four; playerInputIndex++)
		{
			playerGamePad[(int)playerInputIndex].lastGamePadState = playerGamePad[(int)playerInputIndex].currentGamePadState;
			playerGamePad[(int)playerInputIndex].currentGamePadState = GamePad.GetState(playerInputIndex);
			if (playerGamePad[(int)playerInputIndex].currentGamePadState.IsConnected)
			{
				LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.None;
				LevelBaseMenu.Players[(int)playerInputIndex].menuDPadInput = MenuInput.None;
				if (playerGamePad[(int)playerInputIndex].currentGamePadState.DPad.Up == ButtonState.Pressed && playerGamePad[(int)playerInputIndex].lastGamePadState.DPad.Up == ButtonState.Released)
				{
					menuDPadInput = MenuInput.MenuDPadUp;
					LevelBaseMenu.Players[(int)playerInputIndex].menuDPadInput = MenuInput.MenuDPadUp;
					menuInput = MenuInput.MenuUp;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuUp;
				}
				else if (playerGamePad[(int)playerInputIndex].currentGamePadState.DPad.Down == ButtonState.Pressed && playerGamePad[(int)playerInputIndex].lastGamePadState.DPad.Down == ButtonState.Released)
				{
					menuDPadInput = MenuInput.MenuDPadDown;
					LevelBaseMenu.Players[(int)playerInputIndex].menuDPadInput = MenuInput.MenuDPadDown;
					menuInput = MenuInput.MenuDown;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuDown;
				}
				if (playerGamePad[(int)playerInputIndex].currentGamePadState.DPad.Right == ButtonState.Pressed && playerGamePad[(int)playerInputIndex].lastGamePadState.DPad.Right == ButtonState.Released)
				{
					menuDPadInput = MenuInput.MenuDPadRight;
					LevelBaseMenu.Players[(int)playerInputIndex].menuDPadInput = MenuInput.MenuDPadRight;
					menuInput = MenuInput.MenuRight;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuRight;
				}
				else if (playerGamePad[(int)playerInputIndex].currentGamePadState.DPad.Left == ButtonState.Pressed && playerGamePad[(int)playerInputIndex].lastGamePadState.DPad.Left == ButtonState.Released)
				{
					menuDPadInput = MenuInput.MenuDPadLeft;
					LevelBaseMenu.Players[(int)playerInputIndex].menuDPadInput = MenuInput.MenuDPadLeft;
					menuInput = MenuInput.MenuLeft;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuLeft;
				}
				if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Left.Y > 0.5f && playerGamePad[(int)playerInputIndex].lastGamePadState.ThumbSticks.Left.Y < 0.5f)
				{
					menuInput = MenuInput.MenuUp;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuUp;
				}
				else if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Left.Y < -0.5f && playerGamePad[(int)playerInputIndex].lastGamePadState.ThumbSticks.Left.Y > -0.5f)
				{
					menuInput = MenuInput.MenuDown;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuDown;
				}
				if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Left.X > 0.5f && playerGamePad[(int)playerInputIndex].lastGamePadState.ThumbSticks.Left.X < 0.5f)
				{
					menuInput = MenuInput.MenuRight;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuRight;
				}
				else if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Left.X < -0.5f && playerGamePad[(int)playerInputIndex].lastGamePadState.ThumbSticks.Left.X > -0.5f)
				{
					menuInput = MenuInput.MenuLeft;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuLeft;
				}
				ContinuosInputTime -= 0.03334f;
				if (ContinuosInputTime < 0f)
				{
					if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Left.Y > 0.5f || playerGamePad[(int)playerInputIndex].currentGamePadState.DPad.Up == ButtonState.Pressed)
					{
						menuInputContinuos = MenuInput.MenuUp;
						LevelBaseMenu.Players[(int)playerInputIndex].menuInputContinuos = MenuInput.MenuUp;
					}
					else if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Left.Y < -0.5f || playerGamePad[(int)playerInputIndex].currentGamePadState.DPad.Down == ButtonState.Pressed)
					{
						menuInputContinuos = MenuInput.MenuDown;
						LevelBaseMenu.Players[(int)playerInputIndex].menuInputContinuos = MenuInput.MenuDown;
					}
					if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Left.X > 0.5f || playerGamePad[(int)playerInputIndex].currentGamePadState.DPad.Right == ButtonState.Pressed)
					{
						menuInputContinuos = MenuInput.MenuRight;
						LevelBaseMenu.Players[(int)playerInputIndex].menuInputContinuos = MenuInput.MenuRight;
					}
					else if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Left.X < -0.5f || playerGamePad[(int)playerInputIndex].currentGamePadState.DPad.Left == ButtonState.Pressed)
					{
						menuInputContinuos = MenuInput.MenuLeft;
						LevelBaseMenu.Players[(int)playerInputIndex].menuInputContinuos = MenuInput.MenuLeft;
					}
					if (menuInputContinuos != MenuInput.None)
					{
						if (ContinuosInputTime < -0.04f)
						{
							ContinuosInputStep = 3;
							ContinuosInputTime = 0.2f;
						}
						else if (ContinuosInputStep > 0)
						{
							ContinuosInputStep--;
							ContinuosInputTime = (float)ContinuosInputStep * 0.1f;
						}
						else
						{
							ContinuosInputTime = 0f;
						}
					}
				}
				if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Right.Y > 0.5f)
				{
					menuInputRightStick = MenuInput.MenuUp;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInputRightStick = MenuInput.MenuUp;
				}
				else if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Right.Y < -0.5f)
				{
					menuInputRightStick = MenuInput.MenuDown;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInputRightStick = MenuInput.MenuDown;
				}
				if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Right.X > 0.5f)
				{
					menuInputRightStick = MenuInput.MenuRight;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInputRightStick = MenuInput.MenuRight;
				}
				else if (playerGamePad[(int)playerInputIndex].currentGamePadState.ThumbSticks.Right.X < -0.5f)
				{
					menuInputRightStick = MenuInput.MenuLeft;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInputRightStick = MenuInput.MenuLeft;
				}
				if (playerGamePad[(int)playerInputIndex].currentGamePadState.IsButtonDown(Buttons.A) && playerGamePad[(int)playerInputIndex].lastGamePadState.IsButtonUp(Buttons.A))
				{
					menuInput = MenuInput.MenuSelect;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuSelect;
				}
				else if (playerGamePad[(int)playerInputIndex].currentGamePadState.IsButtonDown(Buttons.B) && playerGamePad[(int)playerInputIndex].lastGamePadState.IsButtonUp(Buttons.B))
				{
					menuInput = MenuInput.MenuBack;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuBack;
				}
				else if (playerGamePad[(int)playerInputIndex].currentGamePadState.IsButtonDown(Buttons.X) && playerGamePad[(int)playerInputIndex].lastGamePadState.IsButtonUp(Buttons.X))
				{
					menuInput = MenuInput.MenuReady;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuReady;
				}
				else if (playerGamePad[(int)playerInputIndex].currentGamePadState.IsButtonDown(Buttons.Y) && playerGamePad[(int)playerInputIndex].lastGamePadState.IsButtonUp(Buttons.Y))
				{
					menuInput = MenuInput.MenuY;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuY;
				}
				if (playerGamePad[(int)playerInputIndex].currentGamePadState.IsButtonDown(Buttons.Start) && playerGamePad[(int)playerInputIndex].lastGamePadState.IsButtonUp(Buttons.Start))
				{
					menuInput = MenuInput.MenuStart;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuStart;
				}
				if (playerGamePad[(int)playerInputIndex].currentGamePadState.IsButtonDown(Buttons.Back) && playerGamePad[(int)playerInputIndex].lastGamePadState.IsButtonUp(Buttons.Back))
				{
					menuInput = MenuInput.MenuBack;
					LevelBaseMenu.Players[(int)playerInputIndex].menuInput = MenuInput.MenuBack;
				}
				Update(gameTime);
				if (menuInput == MenuInput.MenuSelect || menuInput == MenuInput.MenuBack)
				{
					break;
				}
			}
		}
	}

	public virtual void Update(GameTime gameTime)
	{
		EndUpdate();
	}

	public virtual void EndUpdate()
	{
	}

	public virtual void Draw()
	{
	}
}
