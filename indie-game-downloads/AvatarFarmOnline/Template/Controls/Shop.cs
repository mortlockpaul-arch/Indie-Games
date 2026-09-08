using System;
using System.Collections.Generic;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Logic.Stage.Definition;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Template;
using Quasar.GameUtils.XBLIG.CrossPromotion;
using Quasar.Input;
using Quasar.Language;

namespace AvatarFarmOnline.Template.Controls;

internal class Shop : InteractiveControl
{
	public const string Type = "Shop";

	public const int NUM_COLUMNS = 2;

	public const int NUM_ROWS = 3;

	public const int ITEMS_PER_PAGE = 6;

	private AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory currentCategory;

	private List<AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition> items;

	private int currentIndex;

	private int baseIndex;

	private AvatarFarmOnline.Logic.Stage.FarmData farmData;

	public override string ControlType => "Shop";

	public AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory CurrentCategory => currentCategory;

	public List<AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition> Items => items;

	public int CurrentIndex => currentIndex;

	public int BaseIndex => baseIndex;

	public AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition CurrentItem => items[currentIndex];

	public AvatarFarmOnline.Logic.Stage.FarmData FarmData => farmData;

	public event Action OnChange;

	public event Action OnCancel;

	public event Action OnAppear;

	public event Action<AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition> OnSelect;

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		return false;
	}

	public Shop(AvatarFarmOnline.Logic.Stage.FarmData farmData, Layout layout)
		: base(layout)
	{
		this.farmData = farmData;
		items = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetCategoryDefinitions(currentCategory);
	}

	public void InvokeEvent(bool goToPlantsCategory)
	{
		if (OnAppear != null)
		{
			OnAppear();
		}
		if (goToPlantsCategory && currentCategory != AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant)
		{
			currentCategory = AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory.Plant;
			items = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetCategoryDefinitions(currentCategory);
			currentIndex = 0;
			baseIndex = 0;
		}
		if (OnChange != null)
		{
			OnChange();
		}
	}

	public bool CanBuy(AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition id, out AvatarFarmOnline.Logic.FailedActionReason failReason)
	{
		bool flag = true;
		failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
		if (id.NeedsUnlockedGame)
		{
			AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
			if (!CrossPromotionManager.Instance.IsUnlocked(farmPersistentGameData.Setup.PlayerIndex, id.GameNeeded))
			{
				flag = false;
				failReason = AvatarFarmOnline.Logic.FailedActionReason.ItemLocked;
			}
		}
		if (flag && farmData.CanBuy(id, out failReason))
		{
			return true;
		}
		return false;
	}

	public override void Update()
	{
		int num = currentIndex;
		if (InputManager.MenuLeftRepeat() && currentIndex > 0)
		{
			currentIndex--;
		}
		if (InputManager.MenuRightRepeat() && currentIndex < items.Count - 1)
		{
			currentIndex++;
		}
		if (InputManager.MenuUpRepeat() && currentIndex >= 2)
		{
			currentIndex -= 2;
		}
		if (InputManager.MenuDownRepeat() && currentIndex < items.Count)
		{
			currentIndex = Math.Min(items.Count - 1, currentIndex + 2);
		}
		AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory itemCategory = currentCategory;
		if (InputManager.MenuPrevPageRepeat())
		{
			currentCategory = (AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory)((int)(currentCategory - 1 + 6) % 6);
		}
		if (InputManager.MenuNextPageRepeat())
		{
			currentCategory = (AvatarFarmOnline.Logic.Stage.Definition.ItemDefinition.ItemCategory)((int)(currentCategory + 1) % 6);
		}
		if (currentCategory != itemCategory)
		{
			items = AvatarFarmOnline.Logic.Stage.ItemDefinitionManager.Instance.GetCategoryDefinitions(currentCategory);
			currentIndex = 0;
		}
		baseIndex = currentIndex / 6 * 6;
		if ((num != currentIndex || currentCategory != itemCategory) && OnChange != null)
		{
			OnChange();
		}
		PlayerIndex whoPressed = PlayerIndex.One;
		if (InputManager.MenuInteract(ref whoPressed))
		{
			AvatarFarmOnline.Logic.FailedActionReason failReason = AvatarFarmOnline.Logic.FailedActionReason.None;
			if (CanBuy(CurrentItem, out failReason))
			{
				if (OnSelect != null)
				{
					OnSelect(CurrentItem);
				}
			}
			else
			{
				GameTemplate.LayoutCancelAudio.Start();
				if (failReason != AvatarFarmOnline.Logic.FailedActionReason.Trialmode)
				{
					string message = "COULD_NOT_BUY".Translate();
					switch (failReason)
					{
					case AvatarFarmOnline.Logic.FailedActionReason.BuildingNeeded:
						message = "SHOP_FAILED_BUILDING_NEEDED".Translate();
						break;
					case AvatarFarmOnline.Logic.FailedActionReason.IncorrectSeason:
						message = "SHOP_FAILED_INCORRECT_SEASON".Translate();
						break;
					case AvatarFarmOnline.Logic.FailedActionReason.NotEnoughMoney:
						message = "SHOP_FAILED_NOT_ENOUGH_MONEY".Translate();
						break;
					case AvatarFarmOnline.Logic.FailedActionReason.XpLevelNeeded:
						message = "SHOP_FAILED_XP_LEVEL".Translate();
						break;
					case AvatarFarmOnline.Logic.FailedActionReason.ItemLocked:
						message = string.Format("SHOP_FAILED_ITEM_LOCKED_{0}".Translate(), CrossPromotionManager.Instance.GetGameName(CurrentItem.GameNeeded));
						break;
					case AvatarFarmOnline.Logic.FailedActionReason.Trialmode:
						message = "SHOP_FAILED_TRIAL_MODE".Translate();
						break;
					}
					Layout.ShowMessage("DOH".Translate(), message);
				}
				else
				{
					Layout.ShowDialog("TRIAL_MODE".Translate(), "SHOP_FAILED_TRIAL_MODE".Translate(), DialogOptions.YesNo, delegate(DialogResult dr, PlayerIndex who)
					{
						if (dr == DialogResult.OkYes)
						{
							PlatformInterface.Instance.TryBuy(who, Layout);
						}
					});
				}
			}
		}
		if (InputManager.MenuCancel())
		{
			if (OnCancel != null)
			{
				OnCancel();
			}
		}
		else if (InputManager.MenuBack() && OnCancel != null)
		{
			OnCancel();
		}
	}

	public override void Dispose()
	{
		OnCancel = null;
		OnChange = null;
		OnSelect = null;
		base.Dispose();
	}
}
