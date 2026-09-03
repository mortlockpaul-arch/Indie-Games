using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

internal class InputState
{
	private KeyboardState _curKey;

	private KeyboardState _oldKey;

	private GamePadState _curPad;

	private GamePadState _oldPad;

	private Vector2 _mousePos;

	public bool leftHeld;

	public bool rightHeld;

	public bool oldLeft;

	public bool oldRight;

	private PlayerIndex[] possiblePlayers;

	public PlayerIndex playerIndex;

	public KeyboardState curKey
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _curKey;
		}
	}

	public KeyboardState oldKey
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _oldKey;
		}
	}

	public GamePadState curPad
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _curPad;
		}
	}

	public GamePadState oldPad
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _oldPad;
		}
	}

	public Vector2 MousePos
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _mousePos;
		}
	}

	public bool triggerHeld
	{
		get
		{
			if (!leftHeld)
			{
				return rightHeld;
			}
			return true;
		}
	}

	public bool triggerOld
	{
		get
		{
			if (!oldLeft)
			{
				return oldRight;
			}
			return true;
		}
	}

	public PlayerIndex ActivePlayerIndex
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return playerIndex;
		}
	}

	public InputState()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		_mousePos = Vector2.Zero;
		playerIndex = (PlayerIndex)0;
		possiblePlayers = (PlayerIndex[])(object)new PlayerIndex[4];
		possiblePlayers[0] = (PlayerIndex)0;
		possiblePlayers[1] = (PlayerIndex)1;
		possiblePlayers[2] = (PlayerIndex)2;
		possiblePlayers[3] = (PlayerIndex)3;
	}

	public void Update()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		_oldKey = _curKey;
		_oldPad = _curPad;
		oldLeft = leftHeld;
		oldRight = rightHeld;
		_curKey = Keyboard.GetState();
		_curPad = GamePad.GetState(playerIndex);
		if (RightTriggerDown())
		{
			rightHeld = true;
			leftHeld = false;
		}
		if (RightTriggerRelease())
		{
			rightHeld = false;
		}
		if (LeftTriggerDown())
		{
			leftHeld = true;
			rightHeld = false;
		}
		if (LeftTriggerRelease())
		{
			leftHeld = false;
		}
	}

	public void SetState(GamePadState gps)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		_curPad = gps;
	}

	public bool DirectionUp()
	{
		if (!KeyPressed((Keys)38) && (!PadPressed((Buttons)268435456) || PadDown((Buttons)2097152) || PadDown((Buttons)1073741824)))
		{
			return PadPressed((Buttons)1);
		}
		return true;
	}

	public bool DirectionDown()
	{
		if (!KeyPressed((Keys)40) && (!PadPressed((Buttons)536870912) || PadDown((Buttons)2097152) || PadDown((Buttons)1073741824)))
		{
			return PadPressed((Buttons)2);
		}
		return true;
	}

	public bool DirectionLeft()
	{
		if (!PadPressed((Buttons)4) && (!PadPressed((Buttons)2097152) || PadDown((Buttons)268435456) || PadDown((Buttons)536870912)))
		{
			return KeyPressed((Keys)37);
		}
		return true;
	}

	public bool DirectionRight()
	{
		if (!PadPressed((Buttons)8) && (!PadPressed((Buttons)1073741824) || PadDown((Buttons)268435456) || PadDown((Buttons)536870912)))
		{
			return KeyPressed((Keys)39);
		}
		return true;
	}

	public bool Select()
	{
		if (!KeyPressed((Keys)13) && !PadPressed((Buttons)16))
		{
			return PadPressed((Buttons)4096);
		}
		return true;
	}

	public bool Cancel()
	{
		return BaseGame.Get().input.PadPressed((Buttons)8192);
	}

	public bool KeyPressed(Keys key)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!((KeyboardState)(ref _oldKey)).IsKeyDown(key) && ((KeyboardState)(ref _curKey)).IsKeyDown(key))
		{
			return !Guide.IsVisible;
		}
		return false;
	}

	public bool KeyReleased(Keys key)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (((KeyboardState)(ref _oldKey)).IsKeyDown(key) && !((KeyboardState)(ref _curKey)).IsKeyDown(key))
		{
			return !Guide.IsVisible;
		}
		return false;
	}

	public bool KeyDown(Keys key)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (((KeyboardState)(ref _curKey)).IsKeyDown(key))
		{
			return !Guide.IsVisible;
		}
		return false;
	}

	public bool PadPressed(Buttons button)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!((GamePadState)(ref _oldPad)).IsButtonDown(button) && ((GamePadState)(ref _curPad)).IsButtonDown(button))
		{
			return !Guide.IsVisible;
		}
		return false;
	}

	public bool PadReleased(Buttons button)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (((GamePadState)(ref _oldPad)).IsButtonDown(button) && !((GamePadState)(ref _curPad)).IsButtonDown(button))
		{
			return !Guide.IsVisible;
		}
		return false;
	}

	public bool PadDown(Buttons button)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (((GamePadState)(ref _curPad)).IsButtonDown(button))
		{
			return !Guide.IsVisible;
		}
		return false;
	}

	public bool LeftTriggerDown()
	{
		if (PadPressed((Buttons)8388608) || KeyPressed((Keys)90))
		{
			return !Guide.IsVisible;
		}
		return false;
	}

	public bool LeftTriggerRelease()
	{
		if (PadReleased((Buttons)8388608) || KeyReleased((Keys)90))
		{
			return !Guide.IsVisible;
		}
		return false;
	}

	public bool RightTriggerDown()
	{
		if (PadPressed((Buttons)4194304) || KeyPressed((Keys)88))
		{
			return !Guide.IsVisible;
		}
		return false;
	}

	public bool RightTriggerRelease()
	{
		if (PadReleased((Buttons)4194304) || KeyReleased((Keys)88))
		{
			return !Guide.IsVisible;
		}
		return false;
	}

	public bool SetPlayerIndex()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		PlayerIndex[] array = possiblePlayers;
		foreach (PlayerIndex val in array)
		{
			GamePadState state = GamePad.GetState(val);
			if (((GamePadState)(ref state)).IsButtonDown((Buttons)16) && !Guide.IsVisible)
			{
				result = true;
				playerIndex = val;
				break;
			}
		}
		return result;
	}
}
