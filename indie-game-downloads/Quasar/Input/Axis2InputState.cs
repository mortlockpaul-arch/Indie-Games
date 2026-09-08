using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Input;

public class Axis2InputState : IInputState
{
	public const float MIN_VALUE = 0.2f;

	public InputValue<Vector2> Value;

	public InputValue<Vector2> PreviousValue;

	private Quasar.Input.ResultRepeatData repeatLeft = new Quasar.Input.ResultRepeatData();

	private Quasar.Input.ResultRepeatData repeatRight = new Quasar.Input.ResultRepeatData();

	private Quasar.Input.ResultRepeatData repeatDown = new Quasar.Input.ResultRepeatData();

	private Quasar.Input.ResultRepeatData repeatUp = new Quasar.Input.ResultRepeatData();

	public Vector2 SaturatedValue => GameMath.Clamp(new Vector2(-1f), new Vector2(1f), Value.Value);

	public bool Left => Value.Value.X < -0.2f;

	public bool Right => Value.Value.X > 0.2f;

	public bool Down => Value.Value.Y < -0.2f;

	public bool Up => Value.Value.Y > 0.2f;

	public bool PreviousLeft => PreviousValue.Value.X < -0.2f;

	public bool PreviousRight => PreviousValue.Value.X > 0.2f;

	public bool PreviousDown => PreviousValue.Value.Y < -0.2f;

	public bool PreviousUp => PreviousValue.Value.Y > 0.2f;

	public bool PressedLeft
	{
		get
		{
			if (Left)
			{
				return !PreviousLeft;
			}
			return false;
		}
	}

	public InputValue<bool> WhoPressedLeft => new InputValue<bool>(PressedLeft, Value.Player);

	public bool PressedRight
	{
		get
		{
			if (Right)
			{
				return !PreviousRight;
			}
			return false;
		}
	}

	public InputValue<bool> WhoPressedRight => new InputValue<bool>(PressedRight, Value.Player);

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

	public InputValue<bool> WhoPressedDown => new InputValue<bool>(PressedDown, Value.Player);

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

	public InputValue<bool> WhoPressedUp => new InputValue<bool>(PressedUp, Value.Player);

	public bool ReleasedLeft
	{
		get
		{
			if (!Left)
			{
				return PreviousLeft;
			}
			return false;
		}
	}

	public InputValue<bool> WhoReleasedLeft => new InputValue<bool>(ReleasedLeft, PreviousValue.Player);

	public bool ReleasedRight
	{
		get
		{
			if (!Right)
			{
				return PreviousRight;
			}
			return false;
		}
	}

	public InputValue<bool> WhoReleasedRight => new InputValue<bool>(ReleasedRight, PreviousValue.Player);

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

	public InputValue<bool> WhoReleasedDown => new InputValue<bool>(ReleasedDown, PreviousValue.Player);

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

	public InputValue<bool> WhoReleasedUp => new InputValue<bool>(ReleasedUp, PreviousValue.Player);

	public bool RepeatLeft => repeatLeft.Check(Left, PressedLeft);

	public bool RepeatRight => repeatRight.Check(Right, PressedRight);

	public bool RepeatDown => repeatDown.Check(Down, PressedDown);

	public bool RepeatUp => repeatUp.Check(Up, PressedUp);

	public Vector2 Direction => new Vector2((Right ? 1 : 0) + (Left ? (-1) : 0), (Up ? 1 : 0) + (Down ? (-1) : 0));

	public Vector2 PreviousDirection => new Vector2((PreviousRight ? 1 : 0) + (PreviousLeft ? (-1) : 0), (PreviousUp ? 1 : 0) + (PreviousDown ? (-1) : 0));

	public void SetRepeatConfig(int repeatStart, int repeatInterval)
	{
		repeatLeft.SetRepeatConfig(repeatStart, repeatInterval);
		repeatRight.SetRepeatConfig(repeatStart, repeatInterval);
		repeatDown.SetRepeatConfig(repeatStart, repeatInterval);
		repeatUp.SetRepeatConfig(repeatStart, repeatInterval);
	}

	public void Reset()
	{
		PreviousValue = Value;
		Value = default(InputValue<Vector2>);
	}
}
