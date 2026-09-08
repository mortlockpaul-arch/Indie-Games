namespace Quasar.Input;

public class ButtonInputState : IInputState
{
	public InputValue<bool> Value;

	private InputValue<bool> PreviousValue;

	private Quasar.Input.ResultRepeatData repeatData = new Quasar.Input.ResultRepeatData();

	public bool Released
	{
		get
		{
			if (!Value.Value)
			{
				return PreviousValue.Value;
			}
			return false;
		}
	}

	public InputValue<bool> WhoReleased
	{
		get
		{
			if (!Value.Value)
			{
				return PreviousValue;
			}
			return default(InputValue<bool>);
		}
	}

	public bool Pressed
	{
		get
		{
			if (Value.Value)
			{
				return !PreviousValue.Value;
			}
			return false;
		}
	}

	public InputValue<bool> WhoPressed
	{
		get
		{
			if (!PreviousValue.Value)
			{
				return Value;
			}
			return default(InputValue<bool>);
		}
	}

	public bool Repeat => repeatData.Check(Value.Value, Pressed);

	public void Reset()
	{
		PreviousValue = Value;
		Value = default(InputValue<bool>);
	}
}
