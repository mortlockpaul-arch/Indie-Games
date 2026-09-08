using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Quasar.Input;

public class InputManager
{
	public enum MenuInputCodes
	{
		Interact,
		Cancel,
		Secondary,
		Terciary,
		Start,
		Back,
		GoToFirst,
		GoToLast,
		PrevPage,
		NextPage,
		Move,
		Aim
	}

	public static int REPEAT_START = 750;

	public static int REPEAT_INTERVAL = 125;

	private static InputManager instance = null;

	private Dictionary<int, InputGroup> groups = new Dictionary<int, InputGroup>();

	private static int NextGroupId = 0;

	private InputGroup menuGroup;

	private static bool enabled = true;

	private static bool forcePlayerIndex = false;

	private static List<PlayerIndex> forcedPlayerIndex = new List<PlayerIndex> { PlayerIndex.One };

	private static bool ignoreIndexList = false;

	private static List<PlayerIndex> playerIndices = new List<PlayerIndex>
	{
		PlayerIndex.One,
		PlayerIndex.Two,
		PlayerIndex.Three,
		PlayerIndex.Four
	};

	private static readonly List<PlayerIndex> constPlayerIndices = new List<PlayerIndex>
	{
		PlayerIndex.One,
		PlayerIndex.Two,
		PlayerIndex.Three,
		PlayerIndex.Four
	};

	public static InputManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new InputManager();
			}
			return instance;
		}
	}

	public static bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	public static bool IgnoreIndices
	{
		get
		{
			return ignoreIndexList;
		}
		set
		{
			ignoreIndexList = value;
		}
	}

	public static List<PlayerIndex> PlayerIndices
	{
		get
		{
			if (!ignoreIndexList)
			{
				if (!forcePlayerIndex)
				{
					return playerIndices;
				}
				return forcedPlayerIndex;
			}
			return constPlayerIndices;
		}
	}

	public static Vector2 MenuMovement()
	{
		return instance.menuGroup.GetInputState<Axis2InputState>(10)?.SaturatedValue ?? Vector2.Zero;
	}

	public static bool MenuLeftRepeat()
	{
		return CompatHooks.WasMenuDirectionRepeat(37);
	}

	public static bool MenuRightRepeat()
	{
		return CompatHooks.WasMenuDirectionRepeat(39);
	}

	public static bool MenuDownRepeat()
	{
		return CompatHooks.WasMenuDirectionRepeat(40);
	}

	public static bool MenuUpRepeat()
	{
		return CompatHooks.WasMenuDirectionRepeat(38);
	}

	public static bool MenuLeft()
	{
		return CompatHooks.WasMenuDirectionPressed(37);
	}

	public static bool MenuRight()
	{
		return CompatHooks.WasMenuDirectionPressed(39);
	}

	public static bool MenuDown()
	{
		return CompatHooks.WasMenuDirectionPressed(40);
	}

	public static bool MenuUp()
	{
		return CompatHooks.WasMenuDirectionPressed(38);
	}

	public static bool MenuInteract()
	{
		return CompatHooks.WasMenuInteractPressed();
	}

	public static bool MenuInteract(ref PlayerIndex whoPressed)
	{
		whoPressed = PlayerIndex.One;
		return CompatHooks.WasMenuInteractPressed();
	}

	public static bool MenuInteract(PlayerIndex whoPressed)
	{
		return CompatHooks.WasMenuInteractPressed();
	}

	public static bool MenuStart()
	{
		return CompatHooks.WasMenuStartPressed();
	}

	public static bool MenuStart(ref PlayerIndex whoPressed)
	{
		whoPressed = PlayerIndex.One;
		return CompatHooks.WasMenuStartPressed();
	}

	public static bool MenuStart(PlayerIndex whoPressed)
	{
		return CompatHooks.WasMenuStartPressed();
	}

	public static bool MenuBack()
	{
		return CompatHooks.WasMenuBackPressed();
	}

	public static bool MenuBack(ref PlayerIndex whoPressed)
	{
		whoPressed = PlayerIndex.One;
		return CompatHooks.WasMenuBackPressed();
	}

	public static bool MenuCancel()
	{
		return CompatHooks.WasMenuCancelPressed();
	}

	public static bool MenuCancel(ref PlayerIndex whoPressed)
	{
		whoPressed = PlayerIndex.One;
		return CompatHooks.WasMenuCancelPressed();
	}

	public static bool MenuCancel(PlayerIndex whoPressed)
	{
		return CompatHooks.WasMenuCancelPressed();
	}

	public static bool MenuGoToFirst()
	{
		IInputState inputState = instance.menuGroup.GetInputState(6);
		if (inputState is ButtonInputState buttonInputState)
		{
			return buttonInputState.Pressed;
		}
		return false;
	}

	public static bool MenuGoToFirst(ref PlayerIndex whoPressed)
	{
		IInputState inputState = instance.menuGroup.GetInputState(6);
		if (inputState is ButtonInputState { WhoPressed: var whoPressed2 })
		{
			whoPressed = whoPressed2.Player;
			return whoPressed2.Value;
		}
		return false;
	}

	public static bool MenuGoToLast()
	{
		IInputState inputState = instance.menuGroup.GetInputState(7);
		if (inputState is ButtonInputState buttonInputState)
		{
			return buttonInputState.Pressed;
		}
		return false;
	}

	public static bool MenuGoToLast(ref PlayerIndex whoPressed)
	{
		IInputState inputState = instance.menuGroup.GetInputState(7);
		if (inputState is ButtonInputState { WhoPressed: var whoPressed2 })
		{
			whoPressed = whoPressed2.Player;
			return whoPressed2.Value;
		}
		return false;
	}

	public static bool MenuNextPage()
	{
		IInputState inputState = instance.menuGroup.GetInputState(9);
		if (inputState is ButtonInputState buttonInputState)
		{
			return buttonInputState.Pressed;
		}
		return false;
	}

	public static bool MenuNextPageRepeat()
	{
		IInputState inputState = instance.menuGroup.GetInputState(9);
		if (inputState is ButtonInputState buttonInputState)
		{
			return buttonInputState.Repeat;
		}
		return false;
	}

	public static bool MenuNextPage(ref PlayerIndex whoPressed)
	{
		IInputState inputState = instance.menuGroup.GetInputState(9);
		if (inputState is ButtonInputState { WhoPressed: var whoPressed2 })
		{
			whoPressed = whoPressed2.Player;
			return whoPressed2.Value;
		}
		return false;
	}

	public static bool MenuPrevPage()
	{
		IInputState inputState = instance.menuGroup.GetInputState(8);
		if (inputState is ButtonInputState buttonInputState)
		{
			return buttonInputState.Pressed;
		}
		return false;
	}

	public static bool MenuPrevPageRepeat()
	{
		IInputState inputState = instance.menuGroup.GetInputState(8);
		if (inputState is ButtonInputState buttonInputState)
		{
			return buttonInputState.Repeat;
		}
		return false;
	}

	public static bool MenuPrevPage(ref PlayerIndex whoPressed)
	{
		IInputState inputState = instance.menuGroup.GetInputState(8);
		if (inputState is ButtonInputState { WhoPressed: var whoPressed2 })
		{
			whoPressed = whoPressed2.Player;
			return whoPressed2.Value;
		}
		return false;
	}

	public static Vector2 MenuAim()
	{
		return instance.menuGroup.GetInputState<Axis2InputState>(11)?.SaturatedValue ?? Vector2.Zero;
	}

	public static bool MenuSecondary()
	{
		return instance.menuGroup.GetInputState<ButtonInputState>(2)?.Pressed ?? false;
	}

	public static bool MenuSecondary(ref PlayerIndex whoPressed)
	{
		ButtonInputState inputState = instance.menuGroup.GetInputState<ButtonInputState>(2);
		if (inputState != null)
		{
			InputValue<bool> whoPressed2 = inputState.WhoPressed;
			whoPressed = whoPressed2.Player;
			return whoPressed2.Value;
		}
		return false;
	}

	public static bool MenuTerciary()
	{
		return instance.menuGroup.GetInputState<ButtonInputState>(3)?.Pressed ?? false;
	}

	public static bool MenuTerciary(ref PlayerIndex whoPressed)
	{
		ButtonInputState inputState = instance.menuGroup.GetInputState<ButtonInputState>(3);
		if (inputState != null)
		{
			InputValue<bool> whoPressed2 = inputState.WhoPressed;
			whoPressed = whoPressed2.Player;
			return whoPressed2.Value;
		}
		return false;
	}

	public static void SetIndices(List<PlayerIndex> indices)
	{
		playerIndices.Clear();
		foreach (PlayerIndex index in indices)
		{
			playerIndices.Add(index);
		}
	}

	public static void ClearIndices()
	{
		playerIndices.Clear();
		for (int i = 0; i < 4; i++)
		{
			playerIndices.Add((PlayerIndex)i);
		}
	}

	public static void SetForcedPlayerIndex(PlayerIndex index)
	{
		forcePlayerIndex = true;
		forcedPlayerIndex.Clear();
		forcedPlayerIndex.Add(index);
	}

	public static void ClearForcedPlayerIndex()
	{
		forcePlayerIndex = false;
	}

	public static void SetIndex(PlayerIndex index)
	{
		playerIndices.Clear();
		playerIndices.Add(index);
	}

	private InputManager()
	{
		LoadDefaultValues();
	}

	public void Update()
	{
		lock (((ICollection)groups).SyncRoot)
		{
			foreach (KeyValuePair<int, InputGroup> group in groups)
			{
				group.Value.Update();
			}
		}
	}

	public InputGroup GetActionGroup(int id)
	{
		lock (((ICollection)groups).SyncRoot)
		{
			if (groups.TryGetValue(id, out var value))
			{
				return value;
			}
			return null;
		}
	}

	public void RemoveGroup(int id)
	{
		lock (((ICollection)groups).SyncRoot)
		{
			groups.Remove(id);
		}
	}

	public void RemoveGroup(InputGroup group)
	{
		lock (((ICollection)groups).SyncRoot)
		{
			groups.Remove(group.Id);
		}
	}

	private void LoadDefaultValues()
	{
		menuGroup = CreateInputGroup();
		menuGroup.BaseGlyph = 2000;
		menuGroup.CreateInputList(0, InputType.Button);
		menuGroup.CreateInputList(10, InputType.Axis2);
		menuGroup.CreateInputList(1, InputType.Button);
		menuGroup.CreateInputList(6, InputType.Button);
		menuGroup.CreateInputList(7, InputType.Button);
		menuGroup.CreateInputList(9, InputType.Button);
		menuGroup.CreateInputList(8, InputType.Button);
		menuGroup.CreateInputList(2, InputType.Button);
		menuGroup.CreateInputList(3, InputType.Button);
		menuGroup.CreateInputList(11, InputType.Axis2);
		menuGroup.CreateInputList(4, InputType.Button);
		menuGroup.CreateInputList(5, InputType.Button);
		menuGroup.AddInput(0, new KeyboardButtonInput(Keys.Enter));
		menuGroup.AddInput(0, new GamepadButtonInput(Buttons.A));
		menuGroup.AddInput(2, new KeyboardButtonInput(Keys.LeftShift));
		menuGroup.AddInput(2, new KeyboardButtonInput(Keys.RightShift));
		menuGroup.AddInput(2, new GamepadButtonInput(Buttons.X));
		menuGroup.AddInput(3, new KeyboardButtonInput(Keys.LeftAlt));
		menuGroup.AddInput(3, new KeyboardButtonInput(Keys.RightAlt));
		menuGroup.AddInput(3, new GamepadButtonInput(Buttons.Y));
		menuGroup.AddInput(1, new KeyboardButtonInput(Keys.Escape));
		menuGroup.AddInput(1, new GamepadButtonInput(Buttons.B));
		menuGroup.AddInput(10, new KeyboardAxis2Input(Keys.Left, Keys.Right, Keys.Down, Keys.Up));
		menuGroup.AddInput(10, new GamepadButtonAxis2Input(Buttons.DPadLeft, Buttons.DPadRight, Buttons.DPadDown, Buttons.DPadUp));
		menuGroup.AddInput(10, new GamepadAxis2Input(Stick.LeftStick));
		menuGroup.AddInput(6, new GamepadTriggerButtonInput(Trigger.LeftTrigger));
		menuGroup.AddInput(6, new KeyboardButtonInput(Keys.Home));
		menuGroup.AddInput(7, new GamepadTriggerButtonInput(Trigger.RightTrigger));
		menuGroup.AddInput(7, new KeyboardButtonInput(Keys.End));
		menuGroup.AddInput(9, new GamepadButtonInput(Buttons.RightShoulder));
		menuGroup.AddInput(9, new KeyboardButtonInput(Keys.PageDown));
		menuGroup.AddInput(8, new GamepadButtonInput(Buttons.LeftShoulder));
		menuGroup.AddInput(8, new KeyboardButtonInput(Keys.PageUp));
		menuGroup.AddInput(4, new GamepadButtonInput(Buttons.Start));
		menuGroup.AddInput(4, new KeyboardButtonInput(Keys.Tab));
		menuGroup.AddInput(5, new GamepadButtonInput(Buttons.Back));
		menuGroup.AddInput(5, new KeyboardButtonInput(Keys.Back));
		menuGroup.AddInput(11, new KeyboardAxis2Input(Keys.NumPad4, Keys.NumPad6, Keys.NumPad2, Keys.NumPad8));
		menuGroup.AddInput(11, new GamepadAxis2Input(Stick.RightStick));
		AddInputGroup(menuGroup);
	}

	public static char GetInputGlyph(MenuInputCodes code)
	{
		return Instance.menuGroup.GetInputGlyph((int)code);
	}

	public InputGroup CreateInputGroup()
	{
		return new InputGroup(NextGroupId++);
	}

	public void AddInputGroup(InputGroup inputGroup)
	{
		lock (((ICollection)groups).SyncRoot)
		{
			groups.Add(inputGroup.Id, inputGroup);
		}
	}

	public DefaultXboxGamepadGroup CreateXboxGamepadGroup(PlayerIndex playerIndex)
	{
		return new DefaultXboxGamepadGroup(NextGroupId++, playerIndex);
	}
}
