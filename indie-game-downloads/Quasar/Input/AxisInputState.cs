using Quasar.Global;

namespace Quasar.Input;

public class AxisInputState : IInputState
{
	public const float MIN_VALUE = 0.2f;

	public InputValue<float> Value;

	private InputValue<float> PreviousValue;

	private Quasar.Input.ResultRepeatData repeatDown = new Quasar.Input.ResultRepeatData();

	private Quasar.Input.ResultRepeatData repeatUp = new Quasar.Input.ResultRepeatData();

	public bool Down => Value.Value < -0.2f;

	public bool PreviousDown => PreviousValue.Value < -0.2f;

	public bool Up => Value.Value > 0.2f;

	public bool PreviousUp => PreviousValue.Value > 0.2f;

	public bool HasValue
	{
		get
		{
			if (!(Value.Value < -0.2f))
			{
				return Value.Value > 0.2f;
			}
			return true;
		}
	}

	public float SaturatedValue => GameMath.Clamp(-1f, 1f, Value.Value);

	public bool PressedDown
	{
		get
		{
			if (Down)
			{
				return !PreviousDown;
			}
			return false;
		}
	}

	public InputValue<bool> WhoPressedDown => new InputValue<bool>(Down && !PreviousDown, Value.Player);

	public bool PressedUp
	{
		get
		{
			if (Up)
			{
				return !PreviousUp;
			}
			return false;
		}
	}

	public InputValue<bool> WhoPressedUp => new InputValue<bool>(Up && !PreviousUp, Value.Player);

	public bool ReleasedUp
	{
		get
		{
			if (!Up)
			{
				return PreviousUp;
			}
			return false;
		}
	}

	public bool ReleasedDown
	{
		get
		{
			if (!Down)
			{
				return PreviousDown;
			}
			return false;
		}
	}

	public bool RepeatDown => repeatDown.Check(Down, PressedDown);

	public bool RepeatUp => repeatUp.Check(Up, PressedUp);

	public void Reset()
	{
		PreviousValue = Value;
		Value = default(InputValue<float>);
	}
}
