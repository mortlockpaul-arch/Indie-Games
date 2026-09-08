using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.Input;

public class InputGroup
{
	private int id;

	private Dictionary<int, IInputList> inputLists = new Dictionary<int, IInputList>();

	private int baseGlyph;

	public int Id => id;

	public int BaseGlyph
	{
		get
		{
			return baseGlyph;
		}
		set
		{
			baseGlyph = value;
		}
	}

	public char GetInputGlyph(int id)
	{
		IInputList inputList = GetInputList(id);
		if (inputList == null)
		{
			return '\0';
		}
		if (inputList.HasGlyph)
		{
			return inputList.Glyph;
		}
		return (char)(baseGlyph + id);
	}

	public IInputList GetInputList(int id)
	{
		if (inputLists.TryGetValue(id, out var value))
		{
			return value;
		}
		return null;
	}

	public InputGroup(int id)
	{
		this.id = id;
	}

	public void Update()
	{
		foreach (KeyValuePair<int, IInputList> inputList in inputLists)
		{
			inputList.Value.Update();
		}
	}

	public void CreateInputList(int id, InputType actionType)
	{
		if (!inputLists.ContainsKey(id))
		{
			IInputList inputList = null;
			switch (actionType)
			{
			case InputType.Button:
				inputList = new InputList<ButtonInputState>(id);
				break;
			case InputType.Axis:
				inputList = new InputList<AxisInputState>(id);
				break;
			case InputType.Axis2:
				inputList = new InputList<Axis2InputState>(id);
				break;
			}
			if (inputList != null)
			{
				inputLists.Add(id, inputList);
			}
		}
	}

	public void CreateInputList<T>(int id, BaseInput<T> inputAction) where T : IInputState, new()
	{
		InputList<T> inputList = new InputList<T>(id);
		inputLists.Add(id, inputList);
		inputList.Add(inputAction);
	}

	public void CreateInputList(int id, PlayerIndex playerIndex, InputType actionType)
	{
		if (!inputLists.ContainsKey(id))
		{
			IInputList inputList = null;
			switch (actionType)
			{
			case InputType.Button:
				inputList = new InputList<ButtonInputState>(id, playerIndex);
				break;
			case InputType.Axis:
				inputList = new InputList<AxisInputState>(id, playerIndex);
				break;
			case InputType.Axis2:
				inputList = new InputList<Axis2InputState>(id, playerIndex);
				break;
			}
			if (inputList != null)
			{
				inputLists.Add(id, inputList);
			}
		}
	}

	public InputList<T> CreateInputList<T>(int id, PlayerIndex playerIndex, BaseInput<T> inputAction) where T : IInputState, new()
	{
		InputList<T> inputList = new InputList<T>(id, playerIndex);
		inputLists.Add(id, inputList);
		inputList.Add(inputAction);
		return inputList;
	}

	public IInputState GetInputState(int id)
	{
		if (inputLists.TryGetValue(id, out var value))
		{
			return value.Result;
		}
		return null;
	}

	public T GetInputState<T>(int id) where T : class, IInputState
	{
		if (inputLists.TryGetValue(id, out var value))
		{
			return value.Result as T;
		}
		return null;
	}

	public void AddInput<T>(int id, BaseInput<T> action) where T : IInputState, new()
	{
		if (inputLists.TryGetValue(id, out var value) && value is InputList<T> inputList)
		{
			inputList.Add(action);
		}
	}

	public bool IsPressed(int id)
	{
		if (inputLists.TryGetValue(id, out var value))
		{
			if (value is InputList<ButtonInputState> inputList)
			{
				return inputList.Result.Pressed;
			}
			if (value is InputList<AxisInputState> inputList2)
			{
				return inputList2.Result.HasValue;
			}
		}
		return false;
	}

	public Vector2 GetAxis2Value(int id)
	{
		if (inputLists.TryGetValue(id, out var value) && value is InputList<Axis2InputState> inputList)
		{
			return inputList.Result.SaturatedValue;
		}
		return Vector2.Zero;
	}

	public float GetAxisValue(int id)
	{
		if (inputLists.TryGetValue(id, out var value) && value is InputList<AxisInputState> inputList)
		{
			return inputList.Result.SaturatedValue;
		}
		return 0f;
	}

	public float GetTriggerValue(int id)
	{
		if (inputLists.TryGetValue(id, out var value) && value is InputList<TriggerInputState> inputList)
		{
			return inputList.Result.SaturatedValue;
		}
		return 0f;
	}

	public bool GetButtonValue(int id)
	{
		if (inputLists.TryGetValue(id, out var value) && value is InputList<ButtonInputState> inputList)
		{
			return inputList.Result.Value.Value;
		}
		return false;
	}
}
