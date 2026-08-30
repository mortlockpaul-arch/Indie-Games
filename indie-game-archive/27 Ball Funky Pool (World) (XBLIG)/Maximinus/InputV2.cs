using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Maximinus;

public class InputV2 : Utils.Input
{
	public class Multiplayer
	{
		private class State
		{
			public ActionMenu currentJoystickMenuAction;

			public ActionMenu previousJoystickMenuAction;

			public GamePadState currentPad;

			public GamePadState previousPad;

			private PlayerIndex Index;

			public State(PlayerIndex Index)
			{
				this.Index = Index;
				currentJoystickMenuAction = ActionMenu.NONE;
				previousJoystickMenuAction = ActionMenu.NONE;
			}

			public void GetState()
			{
				currentPad = Utils.Input.padGetState(Index);
				currentJoystickMenuAction = Utils.Input.joystickAnalogMenuIsPressed(currentPad.ThumbSticks.Left);
				if (currentJoystickMenuAction == ActionMenu.NONE)
				{
					currentJoystickMenuAction = Utils.Input.joystickAnalogMenuIsPressed(currentPad.ThumbSticks.Right);
				}
			}

			public void UpdatePrevious()
			{
				previousJoystickMenuAction = currentJoystickMenuAction;
				previousPad = currentPad;
			}
		}

		private const int MAX = 4;

		public static readonly PlayerIndex[] AllIndexes = new PlayerIndex[4]
		{
			PlayerIndex.One,
			PlayerIndex.Two,
			PlayerIndex.Three,
			PlayerIndex.Four
		};

		private static State[] states = new State[4];

		public static void Initialize()
		{
			for (int i = 0; i < 4; i++)
			{
				states[i] = new State((PlayerIndex)i);
			}
		}

		public static void Update()
		{
			State[] array = states;
			foreach (State state in array)
			{
				state.UpdatePrevious();
				state.GetState();
			}
		}

		public static GamePadState CurrentState(PlayerIndex p)
		{
			return states[(int)p].currentPad;
		}

		public static GamePadState PreviousState(PlayerIndex p)
		{
			return states[(int)p].previousPad;
		}

		public static ActionMenu CurrentJoystickAction(PlayerIndex p)
		{
			return states[(int)p].currentJoystickMenuAction;
		}

		public static ActionMenu PreviousJoystickAction(PlayerIndex p)
		{
			return states[(int)p].previousJoystickMenuAction;
		}
	}

	private bool FirstFramePadIndexFound = true;

	private bool singlePadDisconnected;

	public bool SinglePadDisconnected => singlePadDisconnected;

	protected Vector2 DPadVector => DPadVectorStatic(padState);

	public InputV2()
	{
		Multiplayer.Initialize();
		InitializePost();
	}

	protected bool UpdatePre(GameTime gameTime)
	{
		if (!CheckPlayerIndexFoundIfCondition(gameTime, !Utils.IsXboxGuideActive))
		{
			return false;
		}
		if (FirstFramePadIndexFound)
		{
			FirstFramePadIndexFound = false;
			UpdatePreviousStates();
			return true;
		}
		return true;
	}

	protected override bool GetCurrentState()
	{
		Multiplayer.Update();
		singlePadDisconnected = !base.GetCurrentState();
		return singlePadDisconnected;
	}

	protected Vector2 DPadVectorStatic(GamePadState paramState)
	{
		Vector2 zero = Vector2.Zero;
		if (isPressed(paramState, DPadMapping_Menu(ActionMenu.MENU_UP)))
		{
			zero += Vector2.UnitY * 1f;
		}
		if (isPressed(paramState, DPadMapping_Menu(ActionMenu.MENU_DOWN)))
		{
			zero += Vector2.UnitY * -1f;
		}
		if (isPressed(paramState, DPadMapping_Menu(ActionMenu.MENU_LEFT)))
		{
			zero += Vector2.UnitX * -1f;
		}
		if (isPressed(paramState, DPadMapping_Menu(ActionMenu.MENU_RIGHT)))
		{
			zero += Vector2.UnitX * 1f;
		}
		if (zero != Vector2.Zero)
		{
			zero.Normalize();
		}
		return zero;
	}
}
