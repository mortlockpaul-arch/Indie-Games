using Quasar.Global;

namespace Quasar.Input;

public class TriggerInputState : IInputState
{
	public const float MIN_VALUE = 0.2f;

	public InputValue<float> Value;

	private InputValue<float> PreviousValue;

	private Quasar.Input.ResultRepeatData repeatDown = new Quasar.Input.ResultRepeatData();

	private Quasar.Input.ResultRepeatData repeatUp = new Quasar.Input.ResultRepeatData();

	public bool Triggered => Value.Value > 0.2f;

	public bool PreviousTriggered => PreviousValue.Value > 0.2f;

	public bool HasValue => Value.Value > 0.2f;

	public float SaturatedValue => GameMath.Clamp(0f, 1f, Value.Value);

	public bool Pressed
	{
		get
		{
			if (Triggered)
			{
				return !PreviousTriggered;
			}
			return false;
		}
	}

	public InputValue<bool> WhoPressedn => new InputValue<bool>(Triggered && !PreviousTriggered, Value.Player);

	public bool Released
	{
		get
		{
			if (!Triggered)
			{
				return PreviousTriggered;
			}
			return false;
		}
	}

	public bool Repeat => repeatDown.Check(Triggered, Pressed);

	public void Reset()
	{
		PreviousValue = Value;
		Value = default(InputValue<float>);
	}
}
