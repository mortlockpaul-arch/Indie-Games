namespace XnaLibrary.Input;

public class InputState
{
	private int startInterval = 20;

	private int repeatInterval = 3;

	private VirtualKeyState state;

	private int pressCount;

	private bool isPress;

	public bool this[VirtualKeyState virtualKeyState] => virtualKeyState == State;

	public int StartInterval
	{
		get
		{
			return startInterval;
		}
		set
		{
			startInterval = value;
		}
	}

	public int RepeatInterval
	{
		get
		{
			return repeatInterval;
		}
		set
		{
			repeatInterval = value;
		}
	}

	public VirtualKeyState State => state & VirtualKeyState.Press;

	public bool Repeat
	{
		get
		{
			if (State == VirtualKeyState.Free || State == VirtualKeyState.Release)
			{
				return false;
			}
			if (State == VirtualKeyState.Push)
			{
				return true;
			}
			if (pressCount < StartInterval)
			{
				return false;
			}
			return (pressCount - StartInterval) % RepeatInterval == 0;
		}
	}

	public bool IsPress
	{
		get
		{
			return isPress;
		}
		set
		{
			isPress = value;
		}
	}

	public void Update()
	{
		state = (VirtualKeyState)((int)State << 1);
		if (IsPress)
		{
			state = State | VirtualKeyState.Push;
		}
		pressCount = (IsPress ? (pressCount + 1) : 0);
	}

	public void SetPress(bool press)
	{
		isPress = press;
	}

	public static bool IsPush(InputState key)
	{
		return key[VirtualKeyState.Push];
	}

	public static bool IsPush(InputState[] keys)
	{
		foreach (InputState key in keys)
		{
			if (IsPush(key))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsPushRepeat(InputState key)
	{
		if (!key[VirtualKeyState.Push])
		{
			return key.Repeat;
		}
		return true;
	}

	public static bool IsPushRepeat(params InputState[] keys)
	{
		foreach (InputState key in keys)
		{
			if (IsPushRepeat(key))
			{
				return true;
			}
		}
		return false;
	}
}
