using System;
using System.Collections.Generic;
using AvatarFarmOnline.Logic.Mode.Farm;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Logic.Mode;
using Quasar.Global;
using Quasar.Input;

namespace AvatarFarmOnline.Template.Controls;

internal class FarmList : InteractiveControl
{
	public const string Type = "FarmList";

	public const int ResultsPerPage = 4;

	private AvatarFarmOnline.Logic.Mode.Farm.FarmManager farmManager;

	private List<AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader> availableFarms = new List<AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader>(4);

	private int baseIndex;

	private int lastSelectedIndex;

	private int selectedIndex;

	public override string ControlType => "FarmList";

	public int ResultCount => farmManager.Farms.Count + (farmManager.HasImportedFarm ? 1 : 2);

	public int EffectiveCount => farmManager.Farms.Count;

	public int BaseIndex => baseIndex;

	public int SelectedIndex => selectedIndex;

	public AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader CurrentFarm
	{
		get
		{
			return farmManager.Farms[selectedIndex];
		}
		set
		{
			int num = farmManager.Farms.IndexOf(value);
			if (num >= 0)
			{
				selectedIndex = num;
				baseIndex = GameMath.Clamp(0, ResultCount - 4, selectedIndex - 2);
				lastSelectedIndex = selectedIndex;
			}
		}
	}

	public event Action<AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader> OnSelected;

	public event Action<AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader> OnSettings;

	public event Action<AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader> OnDeleteFarm;

	public event Action OnCreateFarm;

	public event Action OnImportFarm;

	public event Action OnCancel;

	public event Action OnMove;

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		return false;
	}

	public FarmList(Layout layout)
		: base(layout)
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		farmManager = farmPersistentGameData.FarmManager;
	}

	public List<AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader> GetEntries()
	{
		availableFarms.Clear();
		for (int i = baseIndex; i < ResultCount && i < baseIndex + 4; i++)
		{
			if (i >= farmManager.Farms.Count)
			{
				availableFarms.Add(null);
			}
			else
			{
				availableFarms.Add(farmManager.Farms[i]);
			}
		}
		return availableFarms;
	}

	public override void Update()
	{
		PlayerIndex whoPressed = PlayerIndex.One;
		if (InputManager.MenuDownRepeat())
		{
			selectedIndex = GameMath.Clamp(0, ResultCount - 1, selectedIndex + 1);
		}
		if (InputManager.MenuUpRepeat())
		{
			selectedIndex = GameMath.Clamp(0, ResultCount - 1, selectedIndex - 1);
		}
		if (InputManager.MenuPrevPageRepeat())
		{
			selectedIndex = GameMath.Clamp(0, ResultCount - 1, selectedIndex - 4);
		}
		if (InputManager.MenuNextPageRepeat())
		{
			selectedIndex = GameMath.Clamp(0, ResultCount - 1, selectedIndex + 4);
		}
		if (InputManager.MenuGoToLast())
		{
			selectedIndex = Math.Max(0, ResultCount - 1);
		}
		if (InputManager.MenuGoToFirst())
		{
			selectedIndex = 0;
		}
		if (selectedIndex != lastSelectedIndex)
		{
			baseIndex = GameMath.Clamp(0, ResultCount - 4, selectedIndex - 2);
			lastSelectedIndex = selectedIndex;
			if (OnMove != null)
			{
				OnMove();
			}
		}
		if (InputManager.MenuInteract())
		{
			if (selectedIndex == EffectiveCount)
			{
				if (OnCreateFarm != null)
				{
					OnCreateFarm();
				}
			}
			else if (selectedIndex == EffectiveCount + 1)
			{
				if (OnImportFarm != null)
				{
					OnImportFarm();
				}
			}
			else if (OnSelected != null)
			{
				OnSelected(CurrentFarm);
			}
		}
		if (InputManager.MenuSecondary() && selectedIndex < EffectiveCount && OnSettings != null)
		{
			OnSettings(CurrentFarm);
		}
		if (InputManager.MenuTerciary() && selectedIndex < EffectiveCount && OnDeleteFarm != null)
		{
			OnDeleteFarm(CurrentFarm);
		}
		if (InputManager.MenuCancel(ref whoPressed) && OnCancel != null)
		{
			OnCancel();
		}
	}

	public void Refresh(bool resetSelected)
	{
		lastSelectedIndex = -1;
		if (resetSelected)
		{
			selectedIndex = 0;
		}
	}
}
