using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.Input;
using Quasar.Language;

namespace Quasar.GameUtils.Template.Controls;

public class PressAToStart : InteractiveControl
{
	public const string Type = "PressAToStart";

	private bool checkStorage;

	private bool mandatoryStorage;

	private bool needsSignIn;

	public override string ControlType => "PressAToStart";

	public bool CheckStorage
	{
		get
		{
			return checkStorage;
		}
		set
		{
			checkStorage = value;
		}
	}

	public bool MandatoryStorage
	{
		get
		{
			return mandatoryStorage;
		}
		set
		{
			mandatoryStorage = value;
		}
	}

	public bool NeedsSignIn
	{
		get
		{
			return needsSignIn;
		}
		set
		{
			needsSignIn = value;
		}
	}

	public event Action<PlayerIndex> OnSelected;

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		return false;
	}

	public PressAToStart(Layout layout)
		: base(layout)
	{
	}

	public override void Update()
	{
		PlayerIndex whoPressed = PlayerIndex.One;
		if (!InputManager.MenuInteract(ref whoPressed) && !InputManager.MenuStart(ref whoPressed))
		{
			return;
		}
		if (needsSignIn)
		{
			if (!Quasar.GameUtils.Player.Player.IsSignedIn(whoPressed))
			{
				PlatformInterface.Instance.ShowSignIn(1, onlineOnly: false);
			}
			else if (checkStorage && (BaseGame.PlayerStorageDevice(whoPressed) == null || !BaseGame.PlayerStorageDevice(whoPressed).IsConnected))
			{
				BaseGame.Instance.AskForStorageDevice(whoPressed, OnStorageSelected);
			}
			else if (OnSelected != null)
			{
				OnSelected(whoPressed);
			}
		}
		else if (OnSelected != null)
		{
			OnSelected(whoPressed);
		}
	}

	private void OnStorageSelected(StorageDevice sd, PlayerIndex playerIndex)
	{
		if (sd == null || !sd.IsConnected)
		{
			if (mandatoryStorage)
			{
				Layout.ShowDialog("WARNING".Translate(), "STORAGE_DEVICE_MANDATORY".Translate(), DialogOptions.YesNo, OnStorageWarningConfirm);
			}
			else
			{
				Layout.ShowDialog("WARNING".Translate(), "STORAGE_DEVICE_NOT_SELECTED".Translate(), DialogOptions.YesNo, OnStorageWarningConfirm);
			}
		}
		else if (OnSelected != null)
		{
			OnSelected(playerIndex);
		}
	}

	private void OnStorageWarningConfirm(DialogResult dr, PlayerIndex whoPressed)
	{
		if (!mandatoryStorage && dr == DialogResult.OkYes)
		{
			if (OnSelected != null)
			{
				OnSelected(whoPressed);
			}
		}
		else if (mandatoryStorage && dr == DialogResult.OkYes && (BaseGame.PlayerStorageDevice(whoPressed) == null || !BaseGame.PlayerStorageDevice(whoPressed).IsConnected))
		{
			BaseGame.Instance.AskForStorageDevice(whoPressed, OnStorageSelected);
		}
	}
}
