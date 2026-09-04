using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaLibrary.Input;

public class InputComponent : GameComponent
{
	private readonly PlayerIndex[] Players = (PlayerIndex[])(object)new PlayerIndex[4]
	{
		default(PlayerIndex),
		(PlayerIndex)1,
		(PlayerIndex)2,
		(PlayerIndex)3
	};

	private float deadZone;

	public Dictionary<int, KeyboardState> KeyboardStates { get; private set; }

	public Dictionary<int, GamePadState> GamePadStates { get; private set; }

	public Dictionary<int, VirtualPadState> VirtualPadStates { get; private set; }

	public Dictionary<InputState, Keys> VirtualKeyMaps { get; private set; }

	public float DeadZone
	{
		get
		{
			return deadZone;
		}
		set
		{
			deadZone = value;
		}
	}

	public VirtualPadState this[PlayerIndex index]
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected I4, but got Unknown
			return VirtualPadStates[(int)index];
		}
	}

	public InputComponent(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected I4, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected I4, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected I4, but got Unknown
		deadZone = 0.5f;
		KeyboardStates = new Dictionary<int, KeyboardState>();
		GamePadStates = new Dictionary<int, GamePadState>();
		VirtualPadStates = new Dictionary<int, VirtualPadState>();
		VirtualKeyMaps = new Dictionary<InputState, Keys>();
		PlayerIndex[] players = Players;
		foreach (PlayerIndex val in players)
		{
			KeyboardStates.Add((int)val, Keyboard.GetState(val));
			GamePadStates.Add((int)val, GamePad.GetState(val));
			VirtualPadStates.Add((int)val, new VirtualPadState());
		}
		InitializeVirtualKeyMaps();
		((GameComponent)this).Initialize();
	}

	public void InitializeVirtualKeyMaps()
	{
		VirtualPadState virtualPadState = VirtualPadStates[0];
		VirtualKeyMaps.Clear();
		VirtualKeyMaps.Add(virtualPadState.ThumbSticks.Left.Up, (Keys)38);
		VirtualKeyMaps.Add(virtualPadState.ThumbSticks.Left.Down, (Keys)40);
		VirtualKeyMaps.Add(virtualPadState.ThumbSticks.Left.Left, (Keys)37);
		VirtualKeyMaps.Add(virtualPadState.ThumbSticks.Left.Right, (Keys)39);
		VirtualKeyMaps.Add(virtualPadState.DPad.Up, (Keys)38);
		VirtualKeyMaps.Add(virtualPadState.DPad.Down, (Keys)40);
		VirtualKeyMaps.Add(virtualPadState.DPad.Left, (Keys)37);
		VirtualKeyMaps.Add(virtualPadState.DPad.Right, (Keys)39);
		VirtualKeyMaps.Add(virtualPadState.Buttons.A, (Keys)90);
		VirtualKeyMaps.Add(virtualPadState.Buttons.B, (Keys)88);
		VirtualKeyMaps.Add(virtualPadState.Buttons.X, (Keys)65);
		VirtualKeyMaps.Add(virtualPadState.Buttons.Y, (Keys)83);
		VirtualKeyMaps.Add(virtualPadState.Buttons.LeftShoulder, (Keys)81);
		VirtualKeyMaps.Add(virtualPadState.Buttons.RightShoulder, (Keys)87);
		VirtualKeyMaps.Add(virtualPadState.Buttons.Start, (Keys)13);
		VirtualKeyMaps.Add(virtualPadState.Buttons.Back, (Keys)27);
	}

	public override void Update(GameTime gameTime)
	{
		UpdateGamePadState();
		UpdateKeyboardState();
		AssignVirtualPad();
		UpdateVirtualPad();
		((GameComponent)this).Update(gameTime);
	}

	private void UpdateGamePadState()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected I4, but got Unknown
		PlayerIndex[] players = Players;
		foreach (PlayerIndex val in players)
		{
			GamePadStates[(int)val] = GamePad.GetState(val);
		}
	}

	private void UpdateKeyboardState()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected I4, but got Unknown
		PlayerIndex[] players = Players;
		foreach (PlayerIndex val in players)
		{
			KeyboardStates[(int)val] = Keyboard.GetState(val);
		}
	}

	private void UpdateVirtualPad()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected I4, but got Unknown
		PlayerIndex[] players = Players;
		foreach (PlayerIndex val in players)
		{
			VirtualPadStates[(int)val].Update();
		}
	}

	private void AssignVirtualPad()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		PlayerIndex[] players = Players;
		foreach (PlayerIndex index in players)
		{
			AssignVirtualPad(index);
		}
	}

	private void AssignVirtualPad(PlayerIndex index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected I4, but got Unknown
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected I4, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Expected I4, but got Unknown
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		GamePadState val = GamePadStates[(int)index];
		VirtualPadState virtualPadState = VirtualPadStates[(int)index];
		virtualPadState.SetPress(press: false);
		VirtualPadButtons buttons = virtualPadState.Buttons;
		buttons.A.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)4096);
		buttons.B.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)8192);
		buttons.X.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)16384);
		buttons.Y.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)32768);
		buttons.LeftShoulder.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)256);
		buttons.RightShoulder.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)512);
		buttons.LeftStick.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)64);
		buttons.RightStick.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)128);
		buttons.Back.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)32);
		buttons.Start.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)16);
		VirtualPadDPad dPad = virtualPadState.DPad;
		dPad.Up.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)1);
		dPad.Down.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)2);
		dPad.Left.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)4);
		dPad.Right.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)8);
		VirtualPadThumbSticks thumbSticks = virtualPadState.ThumbSticks;
		GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref val)).ThumbSticks;
		Vector2 left = ((GamePadThumbSticks)(ref thumbSticks2)).Left;
		GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref val)).ThumbSticks;
		Vector2 right = ((GamePadThumbSticks)(ref thumbSticks3)).Right;
		thumbSticks.Left.Up.IsPress = left.Y > deadZone;
		thumbSticks.Left.Down.IsPress = left.Y < 0f - deadZone;
		thumbSticks.Left.Left.IsPress = left.X < 0f - deadZone;
		thumbSticks.Left.Right.IsPress = left.X > deadZone;
		thumbSticks.Right.Up.IsPress = right.Y > deadZone;
		thumbSticks.Right.Down.IsPress = right.Y < 0f - deadZone;
		thumbSticks.Right.Left.IsPress = right.X < 0f - deadZone;
		thumbSticks.Right.Right.IsPress = right.X > deadZone;
		VirtualPadTriggers triggers = virtualPadState.Triggers;
		triggers.Left.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)8388608);
		triggers.Right.IsPress = ((GamePadState)(ref val)).IsButtonDown((Buttons)4194304);
		foreach (KeyValuePair<InputState, Keys> virtualKeyMap in VirtualKeyMaps)
		{
			KeyboardState val2 = KeyboardStates[(int)index];
			bool flag = ((KeyboardState)(ref val2)).IsKeyDown(virtualKeyMap.Value);
			virtualKeyMap.Key.IsPress = flag || virtualKeyMap.Key.IsPress;
		}
	}
}
