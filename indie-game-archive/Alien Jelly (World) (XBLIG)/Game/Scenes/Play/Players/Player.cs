using GKEngine.Entities;
using GKEngine.Input;
using Game.Data;
using Game.History;
using Game.QBits;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Scenes.Play.Players;

public class Player : Base3D, IReversible
{
	private const float INPUT_QBIT_MOVE_DEADZONE = 0.6f;

	public PlayerManager manager;

	public bool paused;

	public bool active;

	public int index;

	public QBit qbit;

	public bool leaning;

	private bool inputSet;

	public InputEntity inputQBitMove;

	public InputEntity inputSwitch;

	public InputEntity inputStart;

	public InputEntity inputBack;

	public InputEntity inputReverse;

	public InputEntity inputPush;

	public InputEntity inputStick;

	public InputEntity inputHint;

	public bool historyLocked;

	public Player(PlayerManager oManager, int xIndex)
	{
		manager = oManager;
		index = xIndex;
		position = default(Vector3);
		rotation = Quaternion.Identity;
		Init();
	}

	public void Init()
	{
		Input_Set();
	}

	public void Update(GameTime oGameTime)
	{
		if (active)
		{
			QBit_Update(oGameTime);
		}
	}

	public void Dispose()
	{
		Input_Dispose();
	}

	public void Activate(QBit oQBit)
	{
		QBit_Set(oQBit);
		active = true;
		Input_Activate();
	}

	public void Deactivate()
	{
		if (qbit != null)
		{
			qbit.player = null;
			qbit = null;
		}
		active = false;
		Input_Deactivate();
	}

	public void QBit_Update(GameTime oGameTime)
	{
		if (qbit != null)
		{
			position = qbit.position;
		}
	}

	private void QBit_Move(Vector2 oDir, bool xPush)
	{
		if (qbit != null)
		{
			DataManager.local.settings.InversionMove(ref oDir);
			oDir.Y *= -1f;
			Quaternion quaternion = Quaternion.CreateFromAxisAngle(Vector3.Up, manager.camera.yaw);
			Vector3 vDir = Vector3.Transform(new Vector3(oDir.X, 0f, oDir.Y), quaternion);
			qbit.Move(vDir, 1, xPush);
		}
	}

	private void QBit_Sticky()
	{
		if (qbit != null)
		{
			qbit.Sticky_Toggle();
		}
	}

	public void QBit_Set(QBit oQBit)
	{
		manager.universe.history.Open(this, HistoryItem.Action.SetQBit);
		if (qbit != null)
		{
			qbit.Event_Switched(qbit.player);
		}
		qbit = oQBit;
		if (qbit == null)
		{
			manager.Deactivate(index);
		}
		else
		{
			manager.scene.audio.EventCues_Trigger("Switch");
			qbit.player = this;
			qbit.Particles_Switch_Start();
			manager.ui.Resolve();
		}
		manager.universe.history.Close(this, HistoryItem.Action.SetQBit);
	}

	private void QBit_TrySet(QBit.QBitType? oType)
	{
		if (qbit != null)
		{
			QBit freeQBit = manager.GetFreeQBit(qbit.index, oType);
			if (freeQBit != null && freeQBit.player == null)
			{
				QBit_Set(freeQBit);
			}
		}
	}

	public void History_Set(ref HistoryItemData oItem, HistoryItem.Action oAction)
	{
		if (oAction == HistoryItem.Action.SetQBit)
		{
			oItem.item = qbit;
		}
	}

	public void History_Reverse(ref HistoryItem oItem, float xRatio, GameTime oGameTime)
	{
	}

	public void History_Event_Lock()
	{
		historyLocked = true;
	}

	public void History_Event_Unlock()
	{
		historyLocked = false;
	}

	public void History_Event_Replayed(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.SetQBit)
		{
			QBit_Set(oItem.end.item as QBit);
		}
	}

	public void History_Event_Reverse_End(ref HistoryItem oItem)
	{
		if (oItem.action == HistoryItem.Action.SetQBit)
		{
			QBit_Set(oItem.start.item as QBit);
		}
	}

	public void History_Event_Reverse_Start(ref HistoryItem oItem)
	{
	}

	public bool History_IsNotInteruptable(HistoryItem.Action oAction)
	{
		return true;
	}

	public void History_Event_Resume(ref HistoryItem oItem)
	{
	}

	public virtual void History_Event_ForceClose(ref HistoryItem oItem)
	{
	}

	private void Input_Set()
	{
		inputQBitMove = new InputEntity(InputEntity.Type.Analog2D, "QBitMove_" + index, InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputQBitMove);
		inputQBitMove.Add(new InputAnalog2D(GamePadAnalog2D.Left, index));
		inputQBitMove.active = true;
		inputSwitch = new InputEntity(InputEntity.Type.Button, "QBitSwitch_" + index, InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputSwitch);
		inputSwitch.Add(new InputButton(GamePadButton.A, index));
		inputSwitch.Add(new InputButton(Keys.Z));
		inputSwitch.active = true;
		inputStart = new InputEntity(InputEntity.Type.Button, "QBitStart_" + index, InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputStart);
		inputStart.Add(new InputButton(GamePadButton.Start, index));
		inputStart.Add(new InputButton(Keys.Enter));
		inputStart.active = true;
		inputBack = new InputEntity(InputEntity.Type.Button, "QBitBack_" + index, InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputBack);
		inputBack.Add(new InputButton(GamePadButton.Back, index));
		inputBack.active = true;
		inputReverse = new InputEntity(InputEntity.Type.Button, "QBitReverse_" + index, InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputReverse);
		inputReverse.Add(new InputButton(GamePadButton.B, index));
		inputReverse.Add(new InputButton(Keys.X));
		inputReverse.active = true;
		inputPush = new InputEntity(InputEntity.Type.Button, "QBitPush_" + index, InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputPush);
		inputPush.Add(new InputButton(GamePadButton.X, index));
		inputPush.Add(new InputButton(Keys.C));
		inputPush.active = true;
		inputStick = new InputEntity(InputEntity.Type.Button, "QBitSticky_" + index, InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputStick);
		inputStick.Add(new InputButton(GamePadButton.Y, index));
		inputStick.Add(new InputButton(Keys.V));
		inputStick.active = true;
		inputHint = new InputEntity(InputEntity.Type.Button, "QBitHint_" + index, InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputHint);
		inputHint.Add(new InputButton(GamePadButton.ShoulderRight, index));
		inputHint.Add(new InputButton(GamePadButton.AnalogRight, index));
		inputHint.active = true;
		inputSet = true;
	}

	public void Input_Activate()
	{
		inputQBitMove.active = true;
		inputSwitch.active = true;
		inputStart.active = true;
		inputReverse.active = true;
		inputBack.active = true;
		inputPush.active = true;
		inputStick.active = true;
		inputHint.active = true;
	}

	public void Input_Deactivate()
	{
		inputQBitMove.active = false;
		inputSwitch.active = false;
		inputReverse.active = false;
		inputBack.active = false;
		inputPush.active = false;
		inputStick.active = false;
		inputHint.active = true;
	}

	public void Input_Dispose()
	{
		UniversalInput.InputEntity_Remove(inputQBitMove);
		UniversalInput.InputEntity_Remove(inputSwitch);
		UniversalInput.InputEntity_Remove(inputReverse);
		UniversalInput.InputEntity_Remove(inputStart);
		UniversalInput.InputEntity_Remove(inputBack);
		UniversalInput.InputEntity_Remove(inputPush);
		UniversalInput.InputEntity_Remove(inputStick);
		UniversalInput.InputEntity_Remove(inputHint);
		inputQBitMove = null;
		inputSwitch = null;
		inputStart = null;
		inputReverse = null;
		inputBack = null;
		inputPush = null;
		inputStick = null;
		inputHint = null;
	}

	public void Input_Update(GameTime oGameTime)
	{
		_ = oGameTime.ElapsedGameTime.Milliseconds;
		if (!inputSet || manager.universe.paused || historyLocked || ((qbit == null || qbit.historyLocked || !qbit.playable) && qbit != null))
		{
			return;
		}
		if (!active)
		{
			if (index == manager.primaryPlayer.index && inputStart.pressed)
			{
				manager.universe.scene.dialogs.Show("MainMenu");
			}
			else if (inputStart.pressed && !manager.universe.intro.playing && !manager.universe.qbits.conversation.talking)
			{
				manager.Activate(index);
			}
		}
		else if (inputQBitMove.value2D.X >= 0.6f || inputQBitMove.value2D.X <= -0.6f || inputQBitMove.value2D.Y >= 0.6f || inputQBitMove.value2D.Y <= -0.6f)
		{
			QBit_Move(inputQBitMove.value2D, inputPush.isDown);
		}
		else if (inputSwitch.pressed)
		{
			QBit_TrySet(null);
		}
		else if (inputReverse.downed)
		{
			manager.universe.history.Reverse(this);
		}
		else if (inputStick.downed)
		{
			QBit_Sticky();
		}
		else if (inputStart.pressed)
		{
			if (index == manager.primaryPlayer.index)
			{
				manager.universe.scene.dialogs.Show("MainMenu");
			}
			else
			{
				manager.Deactivate(index);
			}
		}
		else if (inputBack.pressed)
		{
			if (index == 0)
			{
				manager.universe.scene.dialogs.Show("PlayerHelp");
			}
		}
		else if (inputHint.downed)
		{
			manager.Hint();
		}
		else if (inputHint.pressed)
		{
			manager.Hint_Halt();
		}
	}
}
