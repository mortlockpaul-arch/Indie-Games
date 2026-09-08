using AvatarFarmImport;
using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Scores;
using AvatarFarmOnline.Template;
using AvatarFarmOnline.Template.Controls;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Sections;
using Quasar.Input;
using Quasar.Language;

namespace AvatarFarmOnline.Sections;

internal class FarmListSection : GUISection
{
	private AvatarFarmOnline.Template.Controls.FarmList farmList;

	private AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader deleteFarm;

	private bool importValid;

	private uint importXp;

	private uint importCoins;

	private uint importCash;

	public FarmListSection(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader currentFarm)
		: base(23, new Layout("", LanguageManager.Texts["MY_FARMS"], AvatarFarmOnline.Template.ExtendedGameTemplate.Template))
	{
		farmList = new AvatarFarmOnline.Template.Controls.FarmList(base.Layout);
		farmList.CurrentFarm = currentFarm;
		farmList.OnCancel += OnCancel;
		farmList.OnCreateFarm += OnCreate;
		farmList.OnImportFarm += OnImportFarm;
		farmList.OnDeleteFarm += OnDelete;
		farmList.OnSettings += OnSettings;
		farmList.OnSelected += OnSelected;
		base.Layout.AddControl(farmList);
		ButtonInstructions buttonInstructions = new ButtonInstructions(base.Layout);
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Interact, "OK".Translate());
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Secondary, "SETTINGS".Translate());
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Terciary, "DELETE".Translate());
		base.Layout.AddControl(buttonInstructions);
		base.Layout.OnCancel += LayoutCancel;
	}

	private void OnDelete(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader obj)
	{
		deleteFarm = obj;
		base.Layout.ShowDialog("WARNING".Translate(), string.Format("DELETE_FARM_CONFIRM_{0}".Translate(), obj.Name), DialogOptions.YesNo, OnDeleteConfirm);
	}

	private void OnDeleteConfirm(DialogResult result, PlayerIndex whoPressed)
	{
		if (result == DialogResult.OkYes)
		{
			AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = (AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData)GameManager.PersistentData;
			farmPersistentGameData.FarmManager.DeleteFarm(deleteFarm);
			farmList.Refresh(resetSelected: false);
		}
	}

	private void OnSelected(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader obj)
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		if (obj.IsOnline)
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
			ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(farmPersistentGameData.Setup.PlayerIndex);
			if (gamer == null || !gamer.IsSignedInToService || !gamer.AllowOnlineSessions)
			{
				base.Layout.ShowMessage("ERROR".Translate(), "ONLINE_SESSIONS_NOT_ALLOWED".Translate());
				return;
			}
			AvatarFarmOnline.Scores.GameScoreManager.Instance.Enabled = false;
		}
		farmPersistentGameData.SetFarm(obj);
		BaseGame.Instance.NextGameSectionId = 12;
	}

	public bool LayoutCancel(Layout layout, PlayerIndex whoPressed)
	{
		Back();
		return true;
	}

	public void OnCancel()
	{
		Back();
	}

	public void OnCreate()
	{
		importValid = false;
		_ = (AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData)GameManager.PersistentData;
		AskFarmName();
	}

	private void OnImportFarm()
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = (AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData)GameManager.PersistentData;
		PlatformInterface.Instance.ShowTextInput(farmPersistentGameData.Setup.PlayerIndex, "IMPORT_FARM".Translate(), "ENTER_IMPORT_CODE".Translate(), "", OnImportText, base.Layout);
	}

	private void OnImportText(bool isValid, PlayerIndex who, string text)
	{
		bool flag = isValid;
		if (isValid)
		{
			flag = global::AvatarFarmImport.AvatarFarmImport.CheckCode(who, text, out importXp, out importCoins, out importCash);
			if (flag)
			{
				importValid = true;
				DelayedActionManager.AddAction(10, CheckFarmName);
			}
		}
		if (!flag)
		{
			base.Layout.ShowDialog("ERROR".Translate(), "INVALID_CODE_TRY_AGAIN".Translate(), DialogOptions.YesNo, OnInvalidCodeDialog);
		}
	}

	private void OnInvalidCodeDialog(DialogResult result, PlayerIndex whoPressed)
	{
		if (result == DialogResult.OkYes)
		{
			OnImportFarm();
		}
	}

	private void CheckFarmName()
	{
		if (PlatformInterface.Instance.IsGuideVisible)
		{
			DelayedActionManager.AddAction(10, CheckFarmName);
		}
		else
		{
			AskFarmName();
		}
	}

	private void AskFarmName()
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = (AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData)GameManager.PersistentData;
		PlatformInterface.Instance.ShowTextInput(farmPersistentGameData.Setup.PlayerIndex, "FARM_NAME_TITLE".Translate(), "ENTER_FARM_NAME".Translate(), farmPersistentGameData.FarmManager.GetDefaultFarmName(), OnTextInput, base.Layout);
	}

	private void OnTextInput(bool isValid, PlayerIndex who, string text)
	{
		if (!isValid)
		{
			return;
		}
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData fpgd = (AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData)GameManager.PersistentData;
		AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader header;
		if (!importValid)
		{
			if (fpgd.FarmManager.CreateFarm(text, out header))
			{
				if (header.PlayMode == PlayMode.Public)
				{
					base.Layout.ShowDialog("", "ALLOW_PLANT_PERMISSIONS_QUESTION".Translate(), DialogOptions.YesNo, delegate(DialogResult result, PlayerIndex whoPressed)
					{
						if (result == DialogResult.OkYes)
						{
							header.SetPermissions(PlayMode.Public, AvatarFarmOnline.Logic.PlayerPermissions.Full, AvatarFarmOnline.Logic.PlayerPermissions.Build);
							fpgd.FarmManager.Save();
							farmList.Refresh(resetSelected: true);
						}
					});
				}
				farmList.Refresh(resetSelected: true);
			}
			else
			{
				base.Layout.ShowMessage("ERROR".Translate(), "COULD_NOT_CREATE_FARM".Translate());
			}
		}
		else if (fpgd.FarmManager.ImportFarm(text, importXp, importCoins, importCash, out header))
		{
			if (header.PlayMode == PlayMode.Public)
			{
				base.Layout.ShowDialog("", "ALLOW_PLANT_PERMISSIONS_QUESTION".Translate(), DialogOptions.YesNo, delegate(DialogResult result, PlayerIndex whoPressed)
				{
					if (result == DialogResult.OkYes)
					{
						header.SetPermissions(PlayMode.Public, AvatarFarmOnline.Logic.PlayerPermissions.Full, AvatarFarmOnline.Logic.PlayerPermissions.Build);
						fpgd.FarmManager.Save();
						farmList.Refresh(resetSelected: true);
					}
				});
			}
			farmList.Refresh(resetSelected: true);
		}
		else
		{
			base.Layout.ShowMessage("ERROR".Translate(), "COULD_NOT_CREATE_FARM".Translate());
		}
	}

	public void OnSettings(AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader header)
	{
		BaseGame.Instance.NextGameSection = new AvatarFarmOnline.Sections.FarmDetailsSection(header);
	}

	private void Back()
	{
		BaseGame.Instance.NextGameSectionId = 0;
	}

	public override void MainLoop()
	{
		Player.SetPresence(GamerPresenceMode.AtMenu);
		base.MainLoop();
	}
}
