using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class MatchLoadoutMenu : Menu
{
	private enum LoadoutMenuTypes
	{
		Main,
		PrimaryWep,
		SecondaryWep,
		AttahcmentWep,
		Equipment,
		Skills,
		PrimaryAttachments,
		SecondaryAttachments,
		NumOf
	}

	public float CharacterDisplayYaw;

	public PlayerIndex SelectedPlayer;

	private Vector2 GamerTagPos = Vector2.Zero;

	private Rectangle MenuBackdropRec;

	private MenuEntry CharacterMenu = new MenuEntry();

	private MenuEntry PrimaryMenu = new MenuEntry();

	private MenuEntry SecondaryMenu = new MenuEntry();

	private MenuEntry EquipmentMenu = new MenuEntry();

	private MenuEntry SkillsMenu = new MenuEntry();

	private MenuEntry PrimaryAttachments = new MenuEntry();

	private MenuEntry SecondaryAttachments = new MenuEntry();

	public Menu[] LoadoutMenus = new Menu[8];

	public MatchLoadoutMenu()
	{
	}

	public MatchLoadoutMenu(GameMenus id)
		: base(id)
	{
	}

	public override void LoadContent()
	{
		base.LoadContent();
		Rectangle rectangle = EndGameEngine.DefualtViewport.TitleSafeArea;
		GamerTagPos.X = rectangle.Center.X - 128;
		GamerTagPos.Y = rectangle.Top + 128;
		MenuBackdropRec = new Rectangle((int)GamerTagPos.X, 180, rectangle.Right - (int)GamerTagPos.X, rectangle.Bottom - 180);
		for (int i = 0; i < 8; i++)
		{
			LoadoutMenus[i] = new Menu();
			LoadoutMenus[i].LoadContent();
			LoadoutMenus[i].State = MenuState.Hidden;
			LoadoutMenus[i].transitionAlpha = byte.MaxValue;
			LoadoutMenus[i].menuEntryList.Clear();
		}
		LoadoutMenus[0].State = MenuState.TransitionOn;
		SetupMainMenu();
		SetupPrimaryWeaponMenu();
		SetupSecondaryWeaponMenu();
		SetupEquipmentMenu();
		SetupPrimaryAttachmentsMenu();
		SetupSecondaryAttachmentsMenu();
		SetupSkillsMenu();
	}

	public override void Update(float eTime)
	{
		if (Menu.ActivePlayer != null)
		{
			if (Menu.ActivePlayer.currentGamePadState.ThumbSticks.Right.X > 0.5f)
			{
				CharacterDisplayYaw += 1f * eTime;
			}
			else if (Menu.ActivePlayer.currentGamePadState.ThumbSticks.Right.X < -0.5f)
			{
				CharacterDisplayYaw -= 1f * eTime;
			}
		}
		for (int i = 0; i < 8; i++)
		{
			if (LoadoutMenus[i].IsActive)
			{
				if (LoadoutMenus[7].IsActive)
				{
					LoadoutMenus[i].Update(eTime);
				}
				else
				{
					LoadoutMenus[i].Update(eTime);
				}
			}
		}
		if (LoadoutMenus[0].IsActive)
		{
			for (int j = 0; j < LoadoutMenus[0].menuEntryList.Count; j++)
			{
				int index = LoadoutMenus[0].menuEntryList.IndexOf(PrimaryMenu);
				if (LoadoutMenus[0].menuEntryList[index].isSelected)
				{
					PlayerSwapToPrimaryWeapon();
				}
				index = LoadoutMenus[0].menuEntryList.IndexOf(SecondaryMenu);
				if (LoadoutMenus[0].menuEntryList[index].isSelected)
				{
					PlayerSwapToSecondaryWeapon();
				}
			}
		}
		if (LoadoutMenus[5].IsActive)
		{
			for (int k = 0; k < LoadoutMenus[5].menuEntryList.Count; k++)
			{
				if (LoadoutMenus[5].menuEntryList[k].isSelected)
				{
					SkillsAdjustFunc(LoadoutMenus[5].menuEntryList[k]);
				}
			}
		}
		base.Update(eTime);
	}

	public override void Draw()
	{
		PlayerBase selectedPlayer = GetSelectedPlayer();
		Menu.spriteBatch.Begin();
		if (EndGameEngine.GameSettings.GameName.Contains("_AvR_"))
		{
			byte b = 180;
			byte a = byte.MaxValue;
			Menu.spriteBatch.Draw(Menu.AvRMenu, new Rectangle(32, 18, 1216, 684), new Color(b, b, b, a));
		}
		else
		{
			Menu.spriteBatch.Draw(Menu.texGradientVertical, Menu.menuGradientRec, bgTextureColor);
		}
		Menu.spriteBatch.End();
		for (int i = 0; i < 8; i++)
		{
			if (LoadoutMenus[i].IsActive)
			{
				LoadoutMenus[i].Draw();
				break;
			}
		}
		if (LoadoutMenus[5].IsActive)
		{
			if (selectedPlayer != null)
			{
				string b2 = "";
				Vector2 zero = Vector2.Zero;
				Menu menu = LoadoutMenus[5];
				Rectangle a2 = new Rectangle(0, 0, 200, (int)menu.menuEntryList[0].textHeight - 8);
				Rectangle a3 = new Rectangle(0, 0, 0, (int)menu.menuEntryList[0].textHeight - 12);
				Menu.spriteBatch.Begin();
				Menu.spriteBatch.Draw(LevelBaseMenu.texBrown, MenuBackdropRec, bgTextureColor);
				for (int j = 0; j < menu.menuEntryList.Count; j++)
				{
					zero = menu.menuEntryList[j].position;
					zero += menu.menuEntryList[j].textOffset;
					zero.X = MenuBackdropRec.X + 32;
					a2.X = (int)zero.X;
					a2.Y = (int)zero.Y + 4;
					a3.X = (int)zero.X + 2;
					a3.Y = (int)zero.Y + 6;
					bool isSelected = LoadoutMenus[5].menuEntryList[j].isSelected;
					Color d = Color.LightGray;
					Color c = new Color(100, 100, 100, 100);
					if (isSelected)
					{
						d = Color.White;
						c = Color.White;
					}
					switch (j)
					{
					case 0:
						a3.Width = (int)(selectedPlayer.PlayerArmor * 200f);
						b2 = "Increased health";
						break;
					case 1:
						a3.Width = (int)(selectedPlayer.CommandoSpeed * 200f);
						b2 = "Faster reload, melee and weapon swap";
						break;
					case 2:
						a3.Width = (int)(selectedPlayer.RunEndurance * 200f);
						b2 = "Further run distance";
						break;
					case 3:
						a3.Width = (int)(selectedPlayer.RunSpeed * 200f);
						b2 = "Faster run speed";
						break;
					case 4:
						a3.Width = (int)(selectedPlayer.WeaponAccuracey * 200f);
						b2 = "Better accuracy sighted and from hip";
						break;
					case 5:
						a3.Width = (int)(selectedPlayer.WeaponDamage * 200f);
						b2 = "Increased bullet damage";
						break;
					}
					a3.Width -= 4;
					Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, a2, c);
					Menu.spriteBatch.Draw(LevelBaseMenu.texBrown, a3, c);
					Menu.spriteBatch.DrawString(Menu.defaultFont, b2, zero, d, 0f, new Vector2(-300f, -8f), 0.7f, SpriteEffects.None, 0);
				}
				zero = menu.menuEntryList[0].position;
				zero += menu.menuEntryList[0].textOffset;
				zero.X = MenuBackdropRec.X + 32;
				b2 = "Total Points :\n\nTotal Kills :\n\nTotal Deaths :\n\nTotal Headshots :";
				Menu.spriteBatch.DrawString(Menu.defaultFont, b2, zero, Color.LightGray, 0f, new Vector2(0f, -250f), 1f, SpriteEffects.None, 0);
				b2 = selectedPlayer.TotalPoints + "\n\n" + selectedPlayer.TotalNumberKills + "\n\n" + selectedPlayer.TotalNumberDeaths + "\n\n" + selectedPlayer.TotalNumberHeadShots;
				Menu.spriteBatch.DrawString(Menu.defaultFont, b2, zero, Color.LightGray, 0f, new Vector2(-300f, -250f), 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.End();
			}
		}
		else if (selectedPlayer != null)
		{
			selectedPlayer.LoadContent(0);
			Menu.spriteBatch.Begin();
			if (!EndGameEngine.GameSettings.GameName.Contains("_AvR_"))
			{
				Menu.spriteBatch.Draw(LevelBaseMenu.texBrown, MenuBackdropRec, bgTextureColor);
			}
			Vector2 vector = Menu.defaultFont.MeasureString(selectedPlayer.gamerTag);
			vector.X = 160f - vector.X / 2f;
			vector.Y = 0f;
			Menu.spriteBatch.DrawString(Menu.defaultFont, selectedPlayer.gamerTag, GamerTagPos + vector, Color.LightGray, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			vector.X += 232f;
			vector.Y += 350f;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Frag 2", GamerTagPos + vector, Color.LightGray, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			vector.X += 140f;
			if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].SmokeGrenadesUnlocked)
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Smoke 1", GamerTagPos + vector, Color.LightGray, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			}
			else
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Smoke 0", GamerTagPos + vector, Color.LightGray, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
			}
			Menu.spriteBatch.End();
			selectedPlayer.DrawMenuPlayer(CharacterDisplayYaw);
			ShowItemPreview();
		}
		Menu.spriteBatch.Begin();
		DrawButtonControl(selectedPlayer.vpViewPort, drawSelect: true, drawBack: true, drawReady: false);
		Menu.spriteBatch.End();
	}

	public override void DrawBackground()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		LocalMakeActive(LoadoutMenuTypes.Main);
	}

	private void LocalMakeActive(LoadoutMenuTypes e)
	{
		for (int i = 0; i < 8; i++)
		{
			if (LoadoutMenus[i].IsActive)
			{
				LoadoutMenus[i].State = MenuState.TransitionOff;
				LoadoutMenus[i].ResetMenuEntries();
			}
		}
		LoadoutMenus[(int)e].State = MenuState.TransitionOn;
	}

	private void SetupMainMenu()
	{
		Vector2 zero = Vector2.Zero;
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.08f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.2f;
		if (EndGameEngine.GameSettings.GameName.Contains("_AvR_"))
		{
			int num = 0;
			LoadoutMenus[num].menuEntryList.Add(PrimaryMenu.Set("Primary Weapon", MenuTextJustify.Left, zero, PrimaryWeaponFunc, EndGameEngine.GameAssetMgr));
			PrimaryMenu.isSelected = true;
			zero.Y += PrimaryMenu.textHeight;
			LoadoutMenus[num].menuEntryList.Add(SecondaryMenu.Set("Secondary Weapon", MenuTextJustify.Left, zero, SecondaryWeaponFunc, EndGameEngine.GameAssetMgr));
			SecondaryMenu.isSelected = false;
			zero.Y += SecondaryMenu.textHeight;
			LoadoutMenus[num].BackMenuDelegate += BackMenuFunc;
		}
		else
		{
			int num2 = 0;
			LoadoutMenus[num2].menuEntryList.Add(CharacterMenu.Set("Character", MenuTextJustify.Left, zero, CharacterFunc, EndGameEngine.GameAssetMgr));
			CharacterMenu.isSelected = true;
			zero.Y += CharacterMenu.textHeight;
			LoadoutMenus[num2].menuEntryList.Add(PrimaryMenu.Set("Primary Weapon", MenuTextJustify.Left, zero, PrimaryWeaponFunc, EndGameEngine.GameAssetMgr));
			PrimaryMenu.isSelected = false;
			zero.Y += PrimaryMenu.textHeight;
			LoadoutMenus[num2].menuEntryList.Add(SecondaryMenu.Set("Secondary Weapon", MenuTextJustify.Left, zero, SecondaryWeaponFunc, EndGameEngine.GameAssetMgr));
			SecondaryMenu.isSelected = false;
			zero.Y += SecondaryMenu.textHeight;
			LoadoutMenus[num2].menuEntryList.Add(SkillsMenu.Set("Skills", MenuTextJustify.Left, zero, SkillsMenutFunc, EndGameEngine.GameAssetMgr));
			SkillsMenu.isSelected = false;
			zero.Y += SkillsMenu.textHeight;
			LoadoutMenus[num2].BackMenuDelegate += BackMenuFunc;
		}
	}

	private void CharacterFunc(object sender, MenuEntry e)
	{
		GetSelectedPlayer()?.NextCharacter();
	}

	private void PrimaryWeaponFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.PrimaryWep);
	}

	private void SecondaryWeaponFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.SecondaryWep);
	}

	private void EquipmentMenutFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.Equipment);
	}

	private void SkillsMenutFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.Skills);
	}

	private void BackMenuFunc(object sender, MenuEntry e)
	{
		SavePlayerLoadout();
		if (!IsBackDelegateNull())
		{
			HandleBackInput();
		}
		else
		{
			Manager.MakeActive(GameMenus.MatchSetupMenu);
		}
	}

	private void PlayerSwapToPrimaryWeapon()
	{
		GetSelectedPlayer()?.SetPrimaryWeapon();
	}

	private void PlayerSwapToSecondaryWeapon()
	{
		GetSelectedPlayer()?.SetSecondaryWeapon();
	}

	private void SetupPrimaryWeaponMenu()
	{
		Vector2 zero = Vector2.Zero;
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.08f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.2f;
		int num = 1;
		foreach (WeaponClass item in FPSWeaponBase.weapon)
		{
			if (item.WepSlot == WeaponSlot.Primary)
			{
				MenuEntry menuEntry = new MenuEntry();
				LoadoutMenus[num].menuEntryList.Add(menuEntry.Set(item.WepType.ToString(), MenuTextJustify.Left, zero, SetPrimaryWeaponFunc, EndGameEngine.GameAssetMgr));
				menuEntry.isSelected = false;
				zero.Y += menuEntry.textHeight;
			}
		}
		LoadoutMenus[num].menuEntryList[0].isSelected = true;
		MenuEntry menuEntry2 = new MenuEntry();
		LoadoutMenus[num].menuEntryList.Add(menuEntry2.Set("Attachments", MenuTextJustify.Left, zero, PWAttachmentsFunc, EndGameEngine.GameAssetMgr));
		menuEntry2.isSelected = false;
		zero.Y += menuEntry2.textHeight;
		LoadoutMenus[num].BackMenuDelegate += PrimaryBackMenuFunc;
	}

	private void SetPrimaryWeaponFunc(object sender, MenuEntry e)
	{
		GetSelectedPlayer()?.SetPrimaryWeapon(e.text);
	}

	private void PWAttachmentsFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.PrimaryAttachments);
		PlayerBase selectedPlayer = GetSelectedPlayer();
		if (selectedPlayer == null)
		{
			return;
		}
		List<MenuEntry> list = LoadoutMenus[6].menuEntryList;
		int num = 0;
		foreach (WeaponAttachment attachment in selectedPlayer.GetPrimaryWeapon().AttachmentList)
		{
			if (attachment != WeaponAttachment.Nothing)
			{
				list[num++].text = attachment.ToString();
			}
		}
		foreach (WeaponSkin availableSkin in selectedPlayer.GetPrimaryWeapon().AvailableSkins)
		{
			list[num++].text = availableSkin.ToString();
		}
		LoadoutMenus[6].menuListCountOverride = num;
	}

	private void PrimaryBackMenuFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.Main);
	}

	private void SetupSecondaryWeaponMenu()
	{
		Vector2 zero = Vector2.Zero;
		new MenuEntry();
		new MenuEntry();
		MenuEntry menuEntry = new MenuEntry();
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.08f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.2f;
		int num = 2;
		foreach (WeaponClass item in FPSWeaponBase.weapon)
		{
			if (item.WepSlot == WeaponSlot.Secondary)
			{
				MenuEntry menuEntry2 = new MenuEntry();
				LoadoutMenus[num].menuEntryList.Add(menuEntry2.Set(item.WepType.ToString(), MenuTextJustify.Left, zero, SetSecondaryWeaponFunc, EndGameEngine.GameAssetMgr));
				menuEntry2.isSelected = false;
				zero.Y += menuEntry2.textHeight;
			}
		}
		LoadoutMenus[num].menuEntryList[0].isSelected = true;
		LoadoutMenus[num].menuEntryList.Add(menuEntry.Set("Attachments", MenuTextJustify.Left, zero, SWAttachmentsFunc, EndGameEngine.GameAssetMgr));
		menuEntry.isSelected = false;
		zero.Y += menuEntry.textHeight;
		LoadoutMenus[num].BackMenuDelegate += SecondaryBackMenuFunc;
	}

	private void SetSecondaryWeaponFunc(object sender, MenuEntry e)
	{
		GetSelectedPlayer()?.SetSecondaryWeapon(e.text);
	}

	private void SWAttachmentsFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.SecondaryAttachments);
		PlayerBase selectedPlayer = GetSelectedPlayer();
		if (selectedPlayer == null)
		{
			return;
		}
		List<MenuEntry> list = LoadoutMenus[7].menuEntryList;
		int num = 0;
		foreach (WeaponAttachment attachment in selectedPlayer.GetSecondaryWeapon().AttachmentList)
		{
			if (attachment != WeaponAttachment.Nothing)
			{
				list[num++].text = attachment.ToString();
			}
		}
		foreach (WeaponSkin availableSkin in selectedPlayer.GetSecondaryWeapon().AvailableSkins)
		{
			list[num++].text = availableSkin.ToString();
		}
		LoadoutMenus[7].menuListCountOverride = num;
	}

	private void SecondaryBackMenuFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.Main);
	}

	private void SetupEquipmentMenu()
	{
		Vector2 zero = Vector2.Zero;
		MenuEntry menuEntry = new MenuEntry();
		MenuEntry menuEntry2 = new MenuEntry();
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.08f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.2f;
		int num = 4;
		LoadoutMenus[num].menuEntryList.Add(menuEntry.Set("Frag Grenade", MenuTextJustify.Left, zero, FragGrenadehFunc, EndGameEngine.GameAssetMgr));
		menuEntry.isSelected = false;
		zero.Y += menuEntry.textHeight;
		LoadoutMenus[num].menuEntryList.Add(menuEntry2.Set("Smoke Grenade", MenuTextJustify.Left, zero, SmokeGrenadeFunc, EndGameEngine.GameAssetMgr));
		menuEntry2.isSelected = false;
		zero.Y += menuEntry2.textHeight;
		LoadoutMenus[num].BackMenuDelegate += EquipmentBackFunc;
	}

	private void FragGrenadehFunc(object sender, MenuEntry e)
	{
	}

	private void SmokeGrenadeFunc(object sender, MenuEntry e)
	{
	}

	private void EquipmentBackFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.Main);
	}

	private void SetupSkillsMenu()
	{
		Vector2 zero = Vector2.Zero;
		MenuEntry[] array = new MenuEntry[6];
		string[] array2 = new string[6] { "Armor", "Commando", "Endurance", "Run Speed", "Weapon Accuracy", "Weapon Damage" };
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.08f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.2f;
		int num = 5;
		for (int i = 0; i < 6; i++)
		{
			array[i] = new MenuEntry();
			LoadoutMenus[num].menuEntryList.Add(array[i].Set(MenuEntryType.Text, (MenuEntryAttribute)5, array2[i], zero, null, Color.DarkGray, "menus\\button01", "menus\\button02", "menus\\button03", null, EndGameEngine.GameAssetMgr));
			array[i].isSelected = false;
			zero.Y += array[i].textHeight;
		}
		LoadoutMenus[num].BackMenuDelegate += SkillsBackFunc;
	}

	private void SkillsAdjustFunc(MenuEntry e)
	{
		PlayerBase selectedPlayer = GetSelectedPlayer();
		if (selectedPlayer == null)
		{
			return;
		}
		int num = 0;
		float inAdjust = 0f;
		if (LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuRight)
		{
			if (selectedPlayer.TotalPoints <= 0)
			{
				return;
			}
			inAdjust = 0.01f;
		}
		else if (LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuLeft)
		{
			inAdjust = -0.01f;
		}
		switch (e.text)
		{
		case "Armor":
			num = AdjustSkill(ref selectedPlayer.PlayerArmor, inAdjust);
			break;
		case "Commando":
			num = AdjustSkill(ref selectedPlayer.CommandoSpeed, inAdjust);
			break;
		case "Endurance":
			num = AdjustSkill(ref selectedPlayer.RunEndurance, inAdjust);
			break;
		case "Run Speed":
			num = AdjustSkill(ref selectedPlayer.RunSpeed, inAdjust);
			break;
		case "Weapon Accuracy":
			num = AdjustSkill(ref selectedPlayer.WeaponAccuracey, inAdjust);
			break;
		case "Weapon Damage":
			num = AdjustSkill(ref selectedPlayer.WeaponDamage, inAdjust);
			break;
		}
		selectedPlayer.TotalPoints += num;
		selectedPlayer.TotalPoints = ((selectedPlayer.TotalPoints >= 0) ? selectedPlayer.TotalPoints : 0);
	}

	private int AdjustSkill(ref float inValue, float inAdjust)
	{
		int result = 0;
		if (inAdjust > 0f && inValue < 1f)
		{
			result = -100;
			inValue = ((inValue + inAdjust < 1f) ? (inValue + inAdjust) : 1f);
		}
		else if (inAdjust < 0f && inValue > 0f)
		{
			result = 100;
			inValue = ((inValue + inAdjust > 0f) ? (inValue + inAdjust) : 0f);
		}
		return result;
	}

	private void SkillsBackFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.Main);
	}

	private void SetupPrimaryAttachmentsMenu()
	{
		Vector2 zero = Vector2.Zero;
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.08f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.2f;
		int num = 6;
		int num2 = 14;
		for (int i = 0; i < num2; i++)
		{
			MenuEntry menuEntry = new MenuEntry();
			LoadoutMenus[num].menuEntryList.Add(menuEntry.Set(WeaponAttachment.Nothing.ToString(), MenuTextJustify.Left, zero, PrimaryWeaponAttachmentsFunc, EndGameEngine.GameAssetMgr));
			menuEntry.isSelected = false;
			zero.Y += menuEntry.textHeight;
		}
		LoadoutMenus[num].BackMenuDelegate += PWABackFunc;
	}

	private void PrimaryWeaponAttachmentsFunc(object sender, MenuEntry e)
	{
		PlayerBase selectedPlayer = GetSelectedPlayer();
		if (selectedPlayer != null)
		{
			for (int i = 0; i < 6 && !(e.text == ((WeaponAttachment)i).ToString()); i++)
			{
			}
			for (int j = 0; j < 8 && !(e.text == ((WeaponSkin)j).ToString()); j++)
			{
			}
		}
	}

	private void PWABackFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.PrimaryWep);
	}

	private void SetupSecondaryAttachmentsMenu()
	{
		Vector2 zero = Vector2.Zero;
		new MenuEntry();
		new MenuEntry();
		new MenuEntry();
		new MenuEntry();
		new MenuEntry();
		new MenuEntry();
		new MenuEntry();
		new MenuEntry();
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.08f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.2f;
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.08f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.2f;
		int num = 7;
		int num2 = 14;
		for (int i = 0; i < num2; i++)
		{
			MenuEntry menuEntry = new MenuEntry();
			LoadoutMenus[num].menuEntryList.Add(menuEntry.Set(WeaponAttachment.Nothing.ToString(), MenuTextJustify.Left, zero, SecondaryWeaponAttachmentsFunc, EndGameEngine.GameAssetMgr));
			menuEntry.isSelected = false;
			zero.Y += menuEntry.textHeight;
		}
		LoadoutMenus[num].BackMenuDelegate += SWABackFunc;
	}

	private void SecondaryWeaponAttachmentsFunc(object sender, MenuEntry e)
	{
		PlayerBase selectedPlayer = GetSelectedPlayer();
		if (selectedPlayer != null)
		{
			for (int i = 0; i < 6 && !(e.text == ((WeaponAttachment)i).ToString()); i++)
			{
			}
			for (int j = 0; j < 8 && !(e.text == ((WeaponSkin)j).ToString()); j++)
			{
			}
		}
	}

	private void SWABackFunc(object sender, MenuEntry e)
	{
		LocalMakeActive(LoadoutMenuTypes.SecondaryWep);
	}

	private PlayerBase GetSelectedPlayer()
	{
		return Menu.ActivePlayer;
	}

	private void SavePlayerLoadout()
	{
		GetSelectedPlayer()?.SavePlayerStatistics();
	}

	private void UtilityCatchNoSightAttachment(PlayerBase playerRef)
	{
		if (playerRef.fpsWeapon.CurrentWeapon.Attachment != WeaponAttachment.Nothing)
		{
			return;
		}
		foreach (WeaponAttachment attachment in playerRef.fpsWeapon.CurrentWeapon.AttachmentList)
		{
			if (attachment == WeaponAttachment.IronSights)
			{
				playerRef.fpsWeapon.CurrentWeapon.Attachment = WeaponAttachment.IronSights;
				break;
			}
		}
	}

	private void ShowItemPreview()
	{
		if (LoadoutMenus[1].IsActive)
		{
			for (int i = 0; i < LoadoutMenus[1].menuEntryList.Count; i++)
			{
				if (LoadoutMenus[1].menuEntryList[i].isSelected)
				{
					PrimaryWeaponPreview(LoadoutMenus[1].menuEntryList[i]);
				}
			}
		}
		else if (LoadoutMenus[2].IsActive)
		{
			for (int j = 0; j < LoadoutMenus[2].menuEntryList.Count; j++)
			{
				if (LoadoutMenus[2].menuEntryList[j].isSelected)
				{
					SecondaryWeaponPreview(LoadoutMenus[2].menuEntryList[j]);
				}
			}
		}
		else if (LoadoutMenus[6].IsActive)
		{
			for (int k = 0; k < LoadoutMenus[6].menuEntryList.Count; k++)
			{
				if (LoadoutMenus[6].menuEntryList[k].isSelected)
				{
					AttachmentPreview(LoadoutMenus[6].menuEntryList[k]);
				}
			}
		}
		else
		{
			if (!LoadoutMenus[7].IsActive)
			{
				return;
			}
			for (int l = 0; l < LoadoutMenus[7].menuEntryList.Count; l++)
			{
				if (LoadoutMenus[7].menuEntryList[l].isSelected)
				{
					AttachmentPreview(LoadoutMenus[7].menuEntryList[l]);
				}
			}
		}
	}

	private void PrimaryWeaponPreview(MenuEntry e)
	{
		PlayerBase selectedPlayer = GetSelectedPlayer();
		if (selectedPlayer == null)
		{
			return;
		}
		WeaponType e2 = WeaponType.Russian;
		for (int i = 0; i < 44; i++)
		{
			if (e.text == ((WeaponType)i).ToString())
			{
				e2 = (WeaponType)i;
				break;
			}
		}
		selectedPlayer.DrawWeaponPreview(e2);
		DrawWeaponLockMessage(selectedPlayer, e2);
	}

	private void SecondaryWeaponPreview(MenuEntry e)
	{
		PlayerBase selectedPlayer = GetSelectedPlayer();
		if (selectedPlayer == null)
		{
			return;
		}
		WeaponType e2 = WeaponType.Russian;
		for (int i = 0; i < 44; i++)
		{
			if (e.text == ((WeaponType)i).ToString())
			{
				e2 = (WeaponType)i;
				break;
			}
		}
		selectedPlayer.DrawWeaponPreview(e2);
		DrawWeaponLockMessage(selectedPlayer, e2);
	}

	private void DrawWeaponLockMessage(PlayerBase p, WeaponType e)
	{
		_ = Vector2.Zero;
		Menu.spriteBatch.Begin();
		Menu.spriteBatch.End();
	}

	private void SetAttachmentMenuAvailable(List<MenuEntry> menuList, WeaponType wepType)
	{
		for (int i = 0; i < menuList.Count; i++)
		{
			for (int j = 0; j < 6 && !(menuList[i].text == ((WeaponAttachment)j).ToString()); j++)
			{
			}
		}
	}

	private void AttachmentPreview(MenuEntry e)
	{
		PlayerBase selectedPlayer = GetSelectedPlayer();
		if (selectedPlayer == null)
		{
			return;
		}
		WeaponAttachment e2 = WeaponAttachment.Nothing;
		for (int i = 0; i < 6; i++)
		{
			if (e.text == ((WeaponAttachment)i).ToString())
			{
				e2 = (WeaponAttachment)i;
				break;
			}
		}
		for (int j = 0; j < 8 && !(e.text == ((WeaponSkin)j).ToString()); j++)
		{
		}
		selectedPlayer.DrawAttachmentPreview(e2);
		_ = Vector2.Zero;
		Menu.spriteBatch.Begin();
		Menu.spriteBatch.End();
	}
}
