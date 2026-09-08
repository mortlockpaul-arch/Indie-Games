using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Game;
using Quasar.Global;
using Quasar.Input;

namespace Quasar.GameUtils.Template.Controls;

public class PlayerSelect : InteractiveControl
{
	private enum InputActions
	{
		Ready,
		NotReady
	}

	public const string Type = "PlayerSelect";

	private const int TIMEOUT_TIME = 3000;

	private InputGroup[] inputGroup = new InputGroup[4];

	private bool hasTimeout;

	private int timeLeftToTimeOut = 3000;

	private int minimumPlayers = 1;

	private int maximumPlayers = 4;

	private PlayerLevel playerLevel;

	private List<PlayerIndex> validPlayers = new List<PlayerIndex>();

	private bool[] ready = new bool[4];

	public override string ControlType => "PlayerSelect";

	public bool HasTimeout
	{
		get
		{
			return hasTimeout;
		}
		set
		{
			hasTimeout = value;
		}
	}

	public int TimeLeft => timeLeftToTimeOut;

	public int MinimumPlayers
	{
		get
		{
			return minimumPlayers;
		}
		set
		{
			minimumPlayers = value;
		}
	}

	public int MaximumPlayers
	{
		get
		{
			return maximumPlayers;
		}
		set
		{
			maximumPlayers = value;
		}
	}

	public PlayerLevel PlayerLevel
	{
		get
		{
			return playerLevel;
		}
		set
		{
			playerLevel = value;
		}
	}

	public List<PlayerIndex> ValidPlayers => validPlayers;

	public bool IsInValidState
	{
		get
		{
			if (validPlayers.Count >= minimumPlayers)
			{
				return validPlayers.Count <= maximumPlayers;
			}
			return false;
		}
	}

	private int PaneCount
	{
		get
		{
			if (maximumPlayers == 3)
			{
				return 4;
			}
			return maximumPlayers;
		}
	}

	public event Func<PlayerSelect, bool> OnAccept;

	public event Action OnAccepted;

	public event Action<PlayerSelect> OnNotEnoughPlayers;

	public event Func<PlayerSelect, bool> OnCancel;

	public event Action OnCancelled;

	public PlayerSelect(Layout layout)
		: base(layout)
	{
		for (int i = 0; i < 4; i++)
		{
			inputGroup[i] = InputManager.Instance.CreateInputGroup();
			inputGroup[i].CreateInputList(0, (PlayerIndex)i, new GamepadButtonInput(Buttons.A));
			inputGroup[i].CreateInputList(1, (PlayerIndex)i, new GamepadButtonInput(Buttons.B));
			if (i == 0)
			{
				inputGroup[i].AddInput(0, new KeyboardButtonInput(Keys.Enter));
				inputGroup[i].AddInput(1, new KeyboardButtonInput(Keys.Escape));
			}
			InputManager.Instance.AddInputGroup(inputGroup[i]);
			inputGroup[i].Update();
		}
	}

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		return false;
	}

	public void SetAsReady(PlayerIndex index)
	{
		if (CheckValidPlayer(index))
		{
			timeLeftToTimeOut = 3000;
			ready[(int)index] = true;
		}
	}

	public bool Connected(PlayerIndex index)
	{
		return Gamepad.Instance(index).Connected;
	}

	public bool Ready(PlayerIndex index)
	{
		return ready[(int)index];
	}

	private bool CheckValidPlayer(PlayerIndex index)
	{
		ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(index);
		return PlayerLevel switch
		{
			PlayerLevel.SignedIn => gamer != null, 
			PlayerLevel.LiveEnabled => gamer?.AllowOnlineSessions ?? false, 
			_ => true, 
		};
	}

	public void AddPlayer(PlayerIndex pi)
	{
		ready[(int)pi] = true;
	}

	public override void Update()
	{
		validPlayers.Clear();
		bool flag = false;
		for (int i = 0; i < 4; i++)
		{
			PlayerIndex index = (PlayerIndex)i;
			if (inputGroup[i].IsPressed(0) && !ready[i])
			{
				flag = true;
				ready[i] = true;
				if (!CheckValidPlayer(index))
				{
					PlatformInterface.Instance.ShowSignIn(PaneCount, PlayerLevel == PlayerLevel.LiveEnabled);
					ready[i] = false;
					continue;
				}
			}
			if (inputGroup[i].IsPressed(1))
			{
				ready[i] = false;
				flag = true;
			}
			else if (!Gamepad.Instance(index).Connected)
			{
				ready[i] = false;
			}
			else if (!CheckValidPlayer(index))
			{
				ready[i] = false;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			if (ready[j])
			{
				validPlayers.Add((PlayerIndex)j);
			}
		}
		if (InputManager.MenuSecondary())
		{
			PlatformInterface.Instance.ShowSignIn(PaneCount, PlayerLevel == PlayerLevel.LiveEnabled);
		}
		bool flag2 = false;
		if (hasTimeout)
		{
			if (IsInValidState)
			{
				if (flag)
				{
					timeLeftToTimeOut = 3000;
				}
				else if (InputManager.Enabled)
				{
					timeLeftToTimeOut -= Timer.DefaultTimer.LastInterval;
					if (timeLeftToTimeOut <= 0)
					{
						flag2 = true;
						if (OnAccept != null && OnAccept(this) && OnAccepted != null)
						{
							OnAccepted();
						}
					}
				}
			}
		}
		else
		{
			PlayerIndex whoPressed = PlayerIndex.One;
			if (!hasTimeout && InputManager.MenuStart(ref whoPressed))
			{
				if (OnAccept != null && IsInValidState && ready[(int)whoPressed])
				{
					flag2 = true;
				}
				if (flag2)
				{
					if (OnAccept(this) && OnAccepted != null)
					{
						OnAccepted();
					}
				}
				else if (OnNotEnoughPlayers != null)
				{
					OnNotEnoughPlayers(this);
				}
			}
		}
		if (!flag2 && OnCancel != null && InputManager.MenuBack() && OnCancel(this) && OnCancelled != null)
		{
			OnCancelled();
		}
	}

	public override void Dispose()
	{
		OnAccept = null;
		OnAccepted = null;
		OnNotEnoughPlayers = null;
		OnCancel = null;
		OnCancelled = null;
		for (int i = 0; i < 4; i++)
		{
			InputManager.Instance.RemoveGroup(inputGroup[i]);
		}
		base.Dispose();
	}
}
