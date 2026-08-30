using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GKEngine.Input;

public static class UniversalInput
{
	public struct MouseDeadZone(float xInnerX, float xInnerY, float xOuterX, float xOuterY)
	{
		public Vector2 inner = new Vector2(xInnerX, xInnerY);

		public Vector2 outer = new Vector2(xOuterX, xOuterY);
	}

	public const int GAMEPAD_COUNT = 4;

	public static bool active = false;

	public static int gamePadPrimaryIndex = 0;

	public static GamePadState[] gamePadState = new GamePadState[4];

	public static GamePadState[] gamePadStateLast = new GamePadState[4];

	public static KeyboardState keyboardState;

	public static KeyboardState keyboardStateLast;

	public static Dictionary<string, InputEntity> inputEntities = new Dictionary<string, InputEntity>();

	public static List<InputEntity> inputEntitiesList = new List<InputEntity>();

	private static int inputEntitiesListCount = 0;

	public static void Init()
	{
		for (int i = 0; i < gamePadState.Length; i++)
		{
			gamePadState[i] = default(GamePadState);
		}
		keyboardState = default(KeyboardState);
		active = true;
	}

	public static void GetStates()
	{
		for (int i = 0; i < 4; i++)
		{
			ref GamePadState reference = ref gamePadStateLast[i];
			reference = gamePadState[i];
		}
		keyboardStateLast = keyboardState;
		for (int i = 0; i < 4; i++)
		{
			ref GamePadState reference2 = ref gamePadState[i];
			reference2 = GamePad.GetState((PlayerIndex)i);
		}
		keyboardState = Keyboard.GetState();
	}

	public static void FlushStates()
	{
		for (int i = 0; i < gamePadStateLast.Length; i++)
		{
			gamePadStateLast[i] = default(GamePadState);
			gamePadState[i] = default(GamePadState);
		}
		keyboardStateLast = default(KeyboardState);
		keyboardState = Keyboard.GetState();
		for (int i = 0; i < inputEntitiesList.Count; i++)
		{
			inputEntitiesList[i].Flush();
		}
	}

	public static void Update(GameTime oGameTime)
	{
		if (!active)
		{
			return;
		}
		GetStates();
		for (int i = 0; i < inputEntitiesListCount; i++)
		{
			if (inputEntitiesList[i].active)
			{
				inputEntitiesList[i].Update(oGameTime);
			}
		}
	}

	public static void Pause()
	{
		active = false;
	}

	public static void Continue()
	{
		active = true;
	}

	public static bool KeyboardPressed(Keys oKey)
	{
		bool flag = false;
		return keyboardState.IsKeyUp(oKey) && keyboardStateLast.IsKeyDown(oKey);
	}

	public static bool KeyboardDowned(Keys oKey)
	{
		bool flag = false;
		return keyboardState.IsKeyDown(oKey) && keyboardStateLast.IsKeyUp(oKey);
	}

	public static void GamePadSetPrimaryIndex(int pIndex)
	{
		gamePadPrimaryIndex = pIndex;
		for (int i = 0; i < inputEntitiesListCount; i++)
		{
			inputEntitiesList[i].SetPrimaryGamePadIndex(gamePadPrimaryIndex);
		}
	}

	public static ButtonState GamePadButtonState(GamePadButton oGamePadButton, GamePadState oState)
	{
		ButtonState result = ButtonState.Released;
		switch (oGamePadButton)
		{
		case GamePadButton.A:
			result = oState.Buttons.A;
			break;
		case GamePadButton.B:
			result = oState.Buttons.B;
			break;
		case GamePadButton.X:
			result = oState.Buttons.X;
			break;
		case GamePadButton.Y:
			result = oState.Buttons.Y;
			break;
		case GamePadButton.Start:
			result = oState.Buttons.Start;
			break;
		case GamePadButton.Back:
			result = oState.Buttons.Back;
			break;
		case GamePadButton.ShoulderLeft:
			result = oState.Buttons.LeftShoulder;
			break;
		case GamePadButton.ShoulderRight:
			result = oState.Buttons.RightShoulder;
			break;
		case GamePadButton.Up:
			result = oState.DPad.Up;
			break;
		case GamePadButton.Down:
			result = oState.DPad.Down;
			break;
		case GamePadButton.Left:
			result = oState.DPad.Left;
			break;
		case GamePadButton.Right:
			result = oState.DPad.Right;
			break;
		case GamePadButton.AnalogLeft:
			result = oState.Buttons.LeftStick;
			break;
		case GamePadButton.AnalogRight:
			result = oState.Buttons.RightStick;
			break;
		}
		return result;
	}

	public static bool GamePadButtonDown(GamePadButton oGamePadButton, int xGamePadIndex)
	{
		bool flag = false;
		ButtonState buttonState = GamePadButtonState(oGamePadButton, gamePadState[xGamePadIndex]);
		return buttonState == ButtonState.Pressed;
	}

	public static bool GamePadButtonUp(GamePadButton oGamePadButton, int xGamePadIndex)
	{
		bool flag = false;
		ButtonState buttonState = GamePadButtonState(oGamePadButton, gamePadState[xGamePadIndex]);
		return buttonState == ButtonState.Released;
	}

	public static bool GamePadButtonPressed(GamePadButton oGamePadButton, int xGamePadIndex)
	{
		bool flag = false;
		ButtonState buttonState = GamePadButtonState(oGamePadButton, gamePadState[xGamePadIndex]);
		ButtonState buttonState2 = GamePadButtonState(oGamePadButton, gamePadStateLast[xGamePadIndex]);
		return buttonState == ButtonState.Released && buttonState2 == ButtonState.Pressed;
	}

	public static bool GamePadButtonDowned(GamePadButton oGamePadButton, int xGamePadIndex)
	{
		bool flag = false;
		ButtonState buttonState = GamePadButtonState(oGamePadButton, gamePadState[xGamePadIndex]);
		ButtonState buttonState2 = GamePadButtonState(oGamePadButton, gamePadStateLast[xGamePadIndex]);
		return buttonState == ButtonState.Pressed && buttonState2 == ButtonState.Released;
	}

	public static float GamePadAnalog1DValue(GamePadAnalog1D oGamePadAnalog, int xGamePadIndex)
	{
		float result = 0f;
		switch (oGamePadAnalog)
		{
		case GamePadAnalog1D.Left:
			result = gamePadState[xGamePadIndex].Triggers.Left;
			break;
		case GamePadAnalog1D.Right:
			result = gamePadState[xGamePadIndex].Triggers.Right;
			break;
		}
		return result;
	}

	public static void GamePadAnalog2DValue(GamePadAnalog2D oGamePadAnalog, int xGamePadIndex, ref Vector2 xReturn)
	{
		switch (oGamePadAnalog)
		{
		case GamePadAnalog2D.Left:
			xReturn = gamePadState[xGamePadIndex].ThumbSticks.Left;
			break;
		case GamePadAnalog2D.Right:
			xReturn = gamePadState[xGamePadIndex].ThumbSticks.Right;
			break;
		}
	}

	public static void InputEntity_Add(InputEntity oEntity)
	{
		inputEntities.Add(oEntity.name, oEntity);
		inputEntitiesList.Add(oEntity);
		inputEntitiesListCount = inputEntitiesList.Count;
	}

	public static void InputEntity_Remove(InputEntity oEntity)
	{
		if (oEntity != null)
		{
			if (inputEntities.ContainsKey(oEntity.name))
			{
				inputEntities.Remove(oEntity.name);
			}
			inputEntitiesList.Remove(oEntity);
			inputEntitiesListCount = inputEntitiesList.Count;
		}
	}

	public static void InputEntity_Flush()
	{
		inputEntities.Clear();
		inputEntitiesList.Clear();
		inputEntitiesListCount = 0;
	}

	public static void InputEntity_Flush(InputEntity.Scope oScope)
	{
		List<string> list = new List<string>();
		List<InputEntity> list2 = new List<InputEntity>();
		foreach (KeyValuePair<string, InputEntity> inputEntity in inputEntities)
		{
			if (inputEntity.Value.scope == oScope)
			{
				list.Add(inputEntity.Key);
				list2.Add(inputEntity.Value);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			inputEntities.Remove(list[i]);
			inputEntitiesList.Remove(list2[i]);
		}
		list.Clear();
		list2.Clear();
	}
}
