using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Scores;
using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GUI.Controls.GroupControls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Sections;
using Quasar.GameUtils.XBLIG.CrossPromotion.Template;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace AvatarFarmOnline.Sections;

internal class MainMenuSection : GUISection
{
	private const string UnlockButton = "MainMenuGroup/Unlock";

	private bool purchaseButtonRemoved = true;

	private static bool shownTrialMessage;

	public MainMenuSection()
		: base(0, new Layout("", "MAIN_MENU".Translate(), AvatarFarmOnline.Template.ExtendedGameTemplate.Template))
	{
		Group obj = new Group(base.Layout);
		obj.SetId("MainMenuGroup");
		if (GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData && farmPersistentGameData.FarmManager.Farms.Count > 0)
		{
			base.Layout.AddButton(obj, "ResumeGame", "RESUME_GAME".Translate(), ResumeGameButtonClick);
		}
		base.Layout.AddButton(obj, "MyFarms", "MY_FARMS".Translate(), MyFarmsButtonClick);
		if (PlatformInterface.Instance.IsTrial)
		{
			base.Layout.AddButton(obj, "Unlock", "UNLOCK_FULL".Translate(), delegate(Button b, PlayerIndex p)
			{
				PlatformInterface.Instance.TryBuy(p, base.Layout);
			});
			purchaseButtonRemoved = false;
		}
		base.Layout.AddButton(obj, "JoinOnline", "JOIN_ONLINE".Translate(), JoinOnlineButtonClick);
		base.Layout.AddButton(obj, "HowToPlay", "HOW_TO_PLAY".Translate(), HowToPlayButtonClick);
		base.Layout.AddButton(obj, "CrossPromo", "CROSS_PROMOTION".Translate(), CrossPromoButtonClick);
		base.Layout.AddButton(obj, "Extras", "EXTRAS".Translate(), delegate(Button b, PlayerIndex whoPressed)
		{
			InputManager.SetIndex(whoPressed);
			((BaseGame)Engine.Game).NextGameSectionId = 14;
		});
		base.Layout.AddControl(obj);
		ButtonInstructions buttonInstructions = new ButtonInstructions(base.Layout);
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Interact, "OK".Translate());
		base.Layout.AddControl(buttonInstructions);
		base.Layout.OnCancel += delegate
		{
			BaseGame.Instance.NextGameSectionId = 3;
			return true;
		};
		obj.OnCancel += delegate
		{
			BaseGame.Instance.NextGameSectionId = 3;
			return true;
		};
		if (PlatformInterface.Instance.IsTrial && !shownTrialMessage)
		{
			base.Layout.ShowMessage("TRIAL_MODE_TITLE".Translate(), string.Format("TRIAL_MODE_INFO_{0}".Translate(), "GAME_TITLE_LOWCASE".Translate()));
			shownTrialMessage = true;
		}
	}

	private void ResumeGameButtonClick(Button button, PlayerIndex whoPressed)
	{
		CompatHooks.StartOfflineFarm();
	}

	private void MyFarmsButtonClick(Button button, PlayerIndex whoPressed)
	{
		CompatHooks.StartOfflineFarm();
	}

	private void JoinOnlineButtonClick(Button button, PlayerIndex whoPressed)
	{
		if (PlatformInterface.Instance.IsTrial)
		{
			base.Layout.ShowDialog("TRIAL_MODE".Translate(), "TRIAL_MODE_UNLOCK_MULTIPLAYER".Translate(), DialogOptions.YesNo, delegate(DialogResult dr, PlayerIndex who)
			{
				if (dr == DialogResult.OkYes)
				{
					PlatformInterface.Instance.TryBuy(who, base.Layout);
				}
			});
			return;
		}
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(farmPersistentGameData.Setup.PlayerIndex);
		if (gamer == null || !gamer.IsSignedInToService || !gamer.AllowOnlineSessions)
		{
			base.Layout.ShowMessage("ERROR".Translate(), "ONLINE_SESSIONS_NOT_ALLOWED".Translate());
			return;
		}
		AvatarFarmOnline.Scores.GameScoreManager.Instance.Enabled = false;
		((AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData)GameManager.PersistentData).InitOnline();
		BaseGame.Instance.NextGameSection = new AvatarFarmOnline.Sections.MultiplayerFindSection();
	}

	private void CrossPromoButtonClick(Button button, PlayerIndex whoPressed)
	{
		BaseGame.Instance.NextGameSection = new CrossPromotionSection(whoPressed, 27, base.Id, AvatarFarmOnline.Template.ExtendedGameTemplate.Template);
	}

	private void HowToPlayButtonClick(Button button, PlayerIndex whoPressed)
	{
		InputManager.SetIndex(whoPressed);
		BaseGame.Instance.NextGameSection = new HowToPlaySection(base.Id, AvatarFarmOnline.Template.ExtendedGameTemplate.Template);
	}

	private void CheckPurchaseButton()
	{
		if (!purchaseButtonRemoved && !PlatformInterface.Instance.IsTrial)
		{
			Control control = base.Layout.GetControl("MainMenuGroup/Unlock");
			if (control != null)
			{
				base.Layout.Remove("MainMenuGroup/Unlock");
			}
			purchaseButtonRemoved = true;
		}
	}

	public override void MainLoop()
	{
		Player.SetPresence(GamerPresenceMode.AtMenu);
		CheckPurchaseButton();
		base.MainLoop();
	}
}
