using System;
using System.Collections.Generic;
using System.IO;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class InventoryCls : PropModelBase
{
	private const int MaxPocketsSlots = 12;

	private const int MaxWeaponSlots = 4;

	private const int MaxBackPackSlots = 20;

	public static bool InventoryOpen = false;

	private int CurrentWeaponSlots = 4;

	private int CurrentPocketSlots = 12;

	private int CurrentBackPackSlots = 8;

	private int MaxItemsPerSlot = 20;

	public InventoryStorage[] InventoryArray = new InventoryStorage[4];

	private bool HaveBackpack = true;

	private bool HaveVest = true;

	private int selectedSlot;

	private int selectedHorizontal;

	private int selectedVertical;

	private Vector2[] SlotGridExtents = new Vector2[4]
	{
		new Vector2(3f, 4f),
		new Vector2(1f, 4f),
		new Vector2(4f, 3f),
		new Vector2(1f, 5f)
	};

	private InventoryItemCls invDrawItem;

	private InventoryItemCls CurrentBackPack;

	private InventoryItemCls CurrentVest;

	private static int[] VacItemStartIndex = new int[2];

	public static List<InventoryItemCls>[] VacinityItemList = new List<InventoryItemCls>[2];

	private bool SecondRifleSling = true;

	private bool PistolTwoHolster = true;

	public static Texture2D[] EquipmentTexture = new Texture2D[20];

	public static Texture2D[] ConsumableTexture = new Texture2D[9];

	public static Texture2D[] WeaponTexture = new Texture2D[44];

	private static Texture2D Menu00;

	private static Texture2D BackpackLarge;

	private static Texture2D BackpackMedium;

	private static Texture2D BackpackSmall;

	private static Texture2D InventorySelect;

	private static Texture2D WeaponSelect;

	private static Texture2D VacinitySelect;

	private static Texture2D rightbutton;

	private static Texture2D leftbutton;

	private static Texture2D selected;

	private static Texture2D selectedPistol;

	private static Texture2D selectedRifle;

	private static Texture2D selectedVicinity;

	private static Texture2D tentUnSelected;

	private static Texture2D tentSelected;

	private static Texture2D multiItemTex;

	private static bool Initialized = false;

	private static Vector2 uiPos = Vector2.Zero;

	private static Vector3 tmpDir = Vector3.Zero;

	private static Rectangle tmpRec = Rectangle.Empty;

	private static Color clr = Color.White;

	private static Color selectedClr = new Color(180, 180, 180, 180);

	private static Color unselectedClr = new Color(120, 120, 120, 120);

	private static Rectangle tRec = Rectangle.Empty;

	private float haveCompasstimer;

	private bool haveCompass;

	public int GetCurrentBackPack
	{
		get
		{
			if (CurrentBackPack != null)
			{
				if (CurrentBackPackSlots == 8)
				{
					return 1;
				}
				if (CurrentBackPackSlots == 12)
				{
					return 2;
				}
				return 3;
			}
			return 0;
		}
		set
		{
		}
	}

	public override void Load(string s)
	{
		if (!Initialized)
		{
			Initialized = true;
			CurrentWeaponSlots = 4;
			CurrentPocketSlots = 12;
			CurrentBackPackSlots = 8;
			InventoryArray[0] = new InventoryStorage();
			InventoryArray[0].slot = InventorySlot.Pockets;
			InventoryArray[0].slotCount = CurrentPocketSlots;
			InventoryArray[0].list = new List<InventoryItemCls>(12);
			for (int i = 0; i < InventoryArray[0].list.Count; i++)
			{
				InventoryArray[0].list.Add(new InventoryItemCls());
			}
			InventoryArray[1] = new InventoryStorage();
			InventoryArray[1].slot = InventorySlot.Weapons;
			InventoryArray[1].slotCount = CurrentWeaponSlots;
			InventoryArray[1].list = new List<InventoryItemCls>(4);
			for (int j = 0; j < InventoryArray[1].list.Count; j++)
			{
				InventoryArray[1].list.Add(new InventoryItemCls());
			}
			InventoryArray[2] = new InventoryStorage();
			InventoryArray[2].slot = InventorySlot.Backpack;
			InventoryArray[2].slotCount = CurrentBackPackSlots;
			InventoryArray[2].list = new List<InventoryItemCls>(20);
			for (int k = 0; k < InventoryArray[2].list.Count; k++)
			{
				InventoryArray[2].list.Add(new InventoryItemCls());
			}
			InventoryArray[3] = new InventoryStorage();
			InventoryArray[3].slot = InventorySlot.Vacinity;
			InventoryArray[3].slotCount = 5;
			InventoryArray[3].list = new List<InventoryItemCls>(5);
			for (int l = 0; l < InventoryArray[3].list.Count; l++)
			{
				InventoryArray[3].list.Add(new InventoryItemCls());
			}
			InventoryArray[0].slotCount = CurrentPocketSlots;
			InventoryArray[1].slotCount = CurrentWeaponSlots;
			InventoryArray[2].slotCount = CurrentBackPackSlots;
			InventoryArray[3].slotCount = 5;
			InventoryArray[0].valid = true;
			InventoryArray[1].valid = true;
			InventoryArray[2].valid = false;
			InventoryArray[3].valid = true;
			VacItemStartIndex[0] = 0;
			VacItemStartIndex[1] = 0;
			VacinityItemList[0] = new List<InventoryItemCls>();
			VacinityItemList[1] = new List<InventoryItemCls>();
			HaveBackpack = true;
			HaveVest = true;
			EquipmentTexture[1] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\tentdeployed");
			EquipmentTexture[2] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\jerrycan");
			EquipmentTexture[3] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\jerrycan");
			EquipmentTexture[4] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\iconbackpacklarge");
			EquipmentTexture[5] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\iconbackpackmedium");
			EquipmentTexture[6] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\iconbackpacksmall");
			EquipmentTexture[7] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\tacticalflashlight");
			EquipmentTexture[8] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\toolbox");
			EquipmentTexture[9] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\compass");
			EquipmentTexture[10] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\tire");
			EquipmentTexture[11] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\tent");
			EquipmentTexture[12] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\clipak47");
			EquipmentTexture[13] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\clipstanag");
			EquipmentTexture[14] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\clip762nato");
			EquipmentTexture[15] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\clip249");
			EquipmentTexture[16] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\clipm9");
			EquipmentTexture[17] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\clip50cal");
			EquipmentTexture[18] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\clipshotgun");
			EquipmentTexture[19] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\clipsniper");
			ConsumableTexture[1] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\emptycan");
			ConsumableTexture[2] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\mushroomsoup");
			ConsumableTexture[3] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\beans");
			ConsumableTexture[4] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\water");
			ConsumableTexture[5] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\canteen");
			ConsumableTexture[6] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\canteen");
			ConsumableTexture[7] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\bandage");
			ConsumableTexture[8] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\painpills");
			WeaponTexture[42] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\hatchet");
			WeaponTexture[2] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\scarl");
			WeaponTexture[3] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\iconM4");
			WeaponTexture[4] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\rass");
			WeaponTexture[6] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\ak74u");
			WeaponTexture[7] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\m249saw");
			WeaponTexture[8] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\remington870");
			WeaponTexture[34] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\deagle");
			WeaponTexture[33] = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\m9");
			multiItemTex = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\multiitem");
			Menu00 = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\Menu00");
			BackpackLarge = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\BackpackLarge");
			BackpackMedium = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\BackpackMedium");
			BackpackSmall = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\BackpackSmall");
			InventorySelect = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\InventorySelect");
			WeaponSelect = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\WeaponSelect");
			VacinitySelect = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\Vacinity");
			rightbutton = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\rightbutton");
			leftbutton = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\leftbutton");
			selected = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\selected");
			selectedPistol = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\selectedPistol");
			selectedRifle = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\selectedRifle");
			selectedVicinity = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\selectedVicinity");
			tentSelected = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\TentSelected");
			tentUnSelected = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Menu\\TentUnselected");
		}
	}

	public void CloseInventory()
	{
		AIBase.DispHelpInfo = 12f;
		Storage.NewSaveInventory();
		ApocZSaveDataCls.Save();
		InventoryOpen = false;
	}

	public void Update(float eTime, int qIndex, PlayerBase playerRef)
	{
		if (InventoryOpen)
		{
			try
			{
				playerRef.OverrideInput = true;
				AIBase.DispHelpInfo = 0f;
				bool flag = VacinityItemList[qIndex].Count == 0 || playerRef.IsAttached0;
				if ((playerRef.currentGamePadState.IsButtonDown(Buttons.Back) && playerRef.lastGamePadState.IsButtonUp(Buttons.Back)) || (playerRef.currentGamePadState.IsButtonDown(Buttons.B) && playerRef.lastGamePadState.IsButtonUp(Buttons.B)))
				{
					CloseInventory();
					Menu.PlaySelect();
				}
				if (playerRef.currentGamePadState.IsButtonDown(Buttons.LeftShoulder) && playerRef.lastGamePadState.IsButtonUp(Buttons.LeftShoulder))
				{
					if (selectedSlot > 0)
					{
						selectedSlot--;
						selectedHorizontal = 0;
						selectedVertical = 0;
						Menu.PlayQuickSelect();
					}
				}
				else if (playerRef.currentGamePadState.IsButtonDown(Buttons.RightShoulder) && playerRef.lastGamePadState.IsButtonUp(Buttons.RightShoulder))
				{
					int num = (flag ? 2 : 3);
					if (selectedSlot < num)
					{
						selectedSlot++;
						selectedHorizontal = 0;
						selectedVertical = 0;
						Menu.PlayQuickSelect();
					}
				}
				if (flag && selectedSlot > 2)
				{
					selectedSlot = 2;
					selectedHorizontal = 0;
					selectedVertical = 0;
				}
				int num2 = (int)SlotGridExtents[selectedSlot].X;
				int num3 = (int)SlotGridExtents[selectedSlot].Y;
				if (selectedSlot == 2)
				{
					if (CurrentBackPack != null && selectedSlot == 2)
					{
						if (CurrentBackPack.ItemType == 4)
						{
							num3 = 5;
						}
						else if (CurrentBackPack.ItemType == 5)
						{
							num3 = 3;
						}
						else if (CurrentBackPack.ItemType == 6)
						{
							num3 = 2;
						}
					}
					else
					{
						num2 = 0;
						num3 = 0;
					}
				}
				if (selectedSlot == 3)
				{
					num2 = 0;
					num3 = VacinityItemList[qIndex].Count;
				}
				else
				{
					VacItemStartIndex[qIndex] = 0;
				}
				if ((playerRef.currentGamePadState.IsButtonDown(Buttons.DPadLeft) && playerRef.lastGamePadState.IsButtonUp(Buttons.DPadLeft)) || (playerRef.currentGamePadState.ThumbSticks.Left.X > -0.5f && playerRef.lastGamePadState.ThumbSticks.Left.X <= -0.5f))
				{
					if (selectedHorizontal > 0)
					{
						selectedHorizontal--;
						Menu.PlayQuickSelect();
					}
					else if (selectedSlot > 0)
					{
						selectedSlot--;
						selectedHorizontal = 0;
						selectedVertical = 0;
						Menu.PlayQuickSelect();
					}
				}
				else if ((playerRef.currentGamePadState.IsButtonDown(Buttons.DPadRight) && playerRef.lastGamePadState.IsButtonUp(Buttons.DPadRight)) || (playerRef.currentGamePadState.ThumbSticks.Left.X < 0.5f && playerRef.lastGamePadState.ThumbSticks.Left.X >= 0.5f))
				{
					if (selectedHorizontal + 1 < num2)
					{
						selectedHorizontal++;
						Menu.PlayQuickSelect();
					}
					else
					{
						int num4 = (flag ? 2 : 3);
						if (selectedSlot < num4)
						{
							selectedSlot++;
							selectedHorizontal = 0;
							selectedVertical = 0;
							Menu.PlayQuickSelect();
						}
					}
				}
				if ((playerRef.currentGamePadState.IsButtonDown(Buttons.DPadUp) && playerRef.lastGamePadState.IsButtonUp(Buttons.DPadUp)) || (playerRef.currentGamePadState.ThumbSticks.Left.Y < 0.5f && playerRef.lastGamePadState.ThumbSticks.Left.Y >= 0.5f))
				{
					if (selectedVertical > 0)
					{
						selectedVertical--;
						Menu.PlayQuickSelect();
					}
				}
				else if (((playerRef.currentGamePadState.IsButtonDown(Buttons.DPadDown) && playerRef.lastGamePadState.IsButtonUp(Buttons.DPadDown)) || (playerRef.currentGamePadState.ThumbSticks.Left.Y > -0.5f && playerRef.lastGamePadState.ThumbSticks.Left.Y <= -0.5f)) && selectedVertical + 1 < num3)
				{
					selectedVertical++;
					Menu.PlayQuickSelect();
				}
				if (selectedSlot < 3)
				{
					int num5 = selectedVertical * num2 + selectedHorizontal;
					InventoryItemCls inventoryItemCls = InventoryArray[selectedSlot].list[num5];
					invDrawItem = InventoryArray[selectedSlot].list[num5];
					bool flag2 = inventoryItemCls.desc != 0 && inventoryItemCls.desc != 16384;
					if (playerRef.currentGamePadState.IsButtonDown(Buttons.A) && playerRef.lastGamePadState.IsButtonUp(Buttons.A))
					{
						if (!ApocZSaveDataCls.SyncingToServer && flag2)
						{
							Menu.PlaySelect();
							if ((inventoryItemCls.desc & 0x200) > 0)
							{
								if (playerRef.PrimaryWeapon == WeaponsCls.itemsModels[inventoryItemCls.ItemType].WepType)
								{
									playerRef.SetPrimaryWeapon(WeaponType.EmptyHands);
								}
								else if (playerRef.SecondaryWeapon == WeaponsCls.itemsModels[inventoryItemCls.ItemType].WepType)
								{
									playerRef.SetSecondaryWeapon(WeaponType.EmptyHands);
								}
							}
							DropItem(inventoryItemCls, playerRef);
							InventoryArray[selectedSlot].list[num5].desc = 0;
							InventoryArray[selectedSlot].list[num5].item = null;
							for (int i = num5 + 1; i < InventoryArray[selectedSlot].slotCount && InventoryArray[selectedSlot].list[i].desc == 16384; i++)
							{
								InventoryArray[selectedSlot].list[i].desc = 0;
								InventoryArray[selectedSlot].list[i].item = null;
							}
							if (!HaveItem(1024, 7))
							{
								LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].FlashLightOn = false;
								PlayerBase.NetworkUpdateFrameCount = -1;
							}
						}
						else
						{
							Menu.PlayInvalidSelect();
						}
					}
					else if (playerRef.currentGamePadState.IsButtonDown(Buttons.X) && playerRef.lastGamePadState.IsButtonUp(Buttons.X))
					{
						if (flag2)
						{
							Menu.PlaySelect();
							if (UseItem(inventoryItemCls, playerRef))
							{
								InventoryArray[selectedSlot].list[num5].desc = 0;
								InventoryArray[selectedSlot].list[num5].item = null;
								for (int j = num5 + 1; j < InventoryArray[selectedSlot].slotCount && InventoryArray[selectedSlot].list[j].desc == 16384; j++)
								{
									InventoryArray[selectedSlot].list[j].desc = 0;
									InventoryArray[selectedSlot].list[j].item = null;
								}
								UpdateInventoryUseItem(retValue: false);
							}
						}
						else
						{
							Menu.PlayInvalidSelect();
						}
					}
				}
				else
				{
					if (selectedVertical >= VacinityItemList[qIndex].Count)
					{
						selectedVertical = ((VacinityItemList[qIndex].Count - 1 > 0) ? (VacinityItemList[qIndex].Count - 1) : 0);
					}
					if (selectedVertical > VacItemStartIndex[qIndex] + 4)
					{
						VacItemStartIndex[qIndex] = selectedVertical - 4;
					}
					else if (selectedVertical < VacItemStartIndex[qIndex])
					{
						VacItemStartIndex[qIndex] = selectedVertical;
					}
					if (VacinityItemList[qIndex].Count > selectedVertical)
					{
						invDrawItem = VacinityItemList[qIndex][selectedVertical];
					}
					else
					{
						invDrawItem.desc = 0;
					}
					if (playerRef.currentGamePadState.IsButtonDown(Buttons.A) && playerRef.lastGamePadState.IsButtonUp(Buttons.A))
					{
						if (!ApocZSaveDataCls.SyncingToServer && selectedVertical < VacinityItemList[qIndex].Count)
						{
							InventoryItemCls inventoryItemCls2 = VacinityItemList[qIndex][selectedVertical];
							Menu.PlaySelect();
							ItemCls itemCls = new ItemCls();
							itemCls.uid = ((ItemCls)inventoryItemCls2.item).uid;
							itemCls.desc = ((ItemCls)inventoryItemCls2.item).desc;
							itemCls.pos = ((ItemCls)inventoryItemCls2.item).pos;
							itemCls.ownerNetId = (byte)((EGENetWorkNext.networkSession != null) ? EGENetWorkNext.networkSession.LocalGamers[0].Id : 0);
							itemCls.reserved0 = ((ItemCls)inventoryItemCls2.item).reserved0;
							AIBase.AllWorldItems.RequestItem(itemCls);
						}
						else
						{
							Menu.PlayInvalidSelect();
						}
					}
				}
				return;
			}
			catch (Exception ex)
			{
				MessagePump.AddMessage("InvUpdate: " + ex.Message);
				return;
			}
		}
		if (playerRef.currentGamePadState.IsButtonDown(Buttons.Back) && playerRef.lastGamePadState.IsButtonUp(Buttons.Back))
		{
			InventoryOpen = true;
			selectedSlot = 0;
			selectedHorizontal = 0;
			selectedVertical = 0;
			Menu.PlaySelect();
		}
	}

	private void DropItem(InventoryItemCls item, PlayerBase e)
	{
		try
		{
			try
			{
				if ((item.desc & 0x200) > 0)
				{
					PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
					if (WeaponsCls.itemsModels[item.ItemType].WepType == playerBase.fpsWeapon.CurrentWeapon.WepType)
					{
						((ItemCls)item.item).reserved0 = (byte)playerBase.fpsWeapon.CurrentWeapon.BulletsInMag;
					}
				}
			}
			catch
			{
			}
			ItemCls itemCls = new ItemCls();
			itemCls.desc = item.desc;
			if (itemCls.IsEquipment || itemCls.IsWeapon)
			{
				itemCls.reserved0 = ((ItemCls)item.item).reserved0;
			}
			else
			{
				itemCls.reserved0 = 0;
			}
			itemCls.ownerNetId = (byte)((EGENetWorkNext.networkSession != null) ? EGENetWorkNext.networkSession.LocalGamers[0].Id : 0);
			if (WorldItemsCls.TentMenuOpen)
			{
				itemCls.desc |= 32768;
				itemCls.pos = WorldItemsCls.TentPosition;
				ApocZSaveDataCls.AddTentOrContents(itemCls);
				AIBase.AllWorldItems.DropItemInworld(WorldItemsCls.TentPosition, itemCls);
			}
			else
			{
				itemCls.desc &= 32767;
				itemCls.pos = e.vecPosition + WorldAreaCls.GetItemPlaceRandOffset();
				itemCls.pos.Y = HeightMapPhysics.GetHeight(ref itemCls.pos) + 8f;
				AIBase.AllWorldItems.DropItemInworld(itemCls.pos, itemCls);
			}
			Storage.NewSaveInventory();
		}
		catch (Exception ex)
		{
			MessagePump.AddMessage(ex.Message);
		}
	}

	public bool UseItem(InventoryItemCls item, PlayerBase e)
	{
		if (item.desc == 16384)
		{
			return false;
		}
		if ((item.desc & 0x100) > 0)
		{
			if (item.ItemType != 1)
			{
				if (item.ItemType == 3 || item.ItemType <= 2)
				{
					e.FoodLevel = 100f;
					e.BloodLevel = ((e.BloodLevel + 10f > 100f) ? 100f : (e.BloodLevel + 10f));
					return true;
				}
				if (item.ItemType == 4)
				{
					e.WaterLevel = 100f;
					return true;
				}
				if (item.ItemType == 6)
				{
					e.WaterLevel = 100f;
					item.desc = (ushort)((item.desc & 0xFF00) | 5);
					return UpdateInventoryUseItem(retValue: false);
				}
				if (item.ItemType == 7)
				{
					e.BloodLoss = 0f;
					return true;
				}
				if (item.ItemType == 8)
				{
					e.PainPillTimer = 60f;
					return true;
				}
				return true;
			}
		}
		else if ((item.desc & 0x400) > 0)
		{
			if (item.ItemType == 9)
			{
				return false;
			}
			if (item.ItemType == 11)
			{
				ItemCls itemCls = new ItemCls();
				itemCls.uid = WorldItemsCls.UniqueId;
				itemCls.desc = 1025;
				itemCls.pos = e.vecPosition - new Vector3(0f, 66f, 0f) + WorldAreaCls.GetItemPlaceRandOffset();
				itemCls.ownerNetId = (byte)((EGENetWorkNext.networkSession != null) ? EGENetWorkNext.networkSession.LocalGamers[0].Id : 0);
				itemCls.reserved0 = ((ItemCls)item.item).reserved0;
				ApocZSaveDataCls.AddTentOrContents(itemCls);
				AIBase.AllWorldItems.DropItemInworld(itemCls.pos, itemCls);
				return true;
			}
			if (item.ItemType == 7)
			{
				e.ToggleFlashLight();
				return false;
			}
		}
		else if ((item.desc & 0x200) > 0)
		{
			int wepIndex = 0;
			if (e.fpsWeapon.SwitchWeapon(WeaponsCls.itemsModels[item.ItemType].WepType, ref wepIndex))
			{
				FPSWeaponBase.weapon[wepIndex].InventoryItemRef = (ItemCls)item.item;
				FPSWeaponBase.weapon[wepIndex].BulletsInMag = ((ItemCls)item.item).reserved0;
			}
			return UpdateInventoryUseItem(retValue: false);
		}
		return false;
	}

	private bool UpdateInventoryUseItem(bool retValue)
	{
		Storage.NewSaveInventory();
		return retValue;
	}

	public override void Draw(PlayerBase viewer, int qIndex)
	{
		ShaderPass = 0;
	}

	public override void DrawPost(PlayerBase e, int qIndex)
	{
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		Menu.spriteBatch.Begin();
		if (InventoryOpen && invDrawItem != null)
		{
			try
			{
				byte b = (clr.B = byte.MaxValue);
				byte b3 = (clr.G = b);
				byte a = (clr.R = b3);
				clr.A = a;
				byte b6 = (selectedClr.B = 180);
				byte b8 = (selectedClr.G = b6);
				byte a2 = (selectedClr.R = b8);
				selectedClr.A = a2;
				byte b11 = (unselectedClr.B = 120);
				byte b13 = (unselectedClr.G = b11);
				byte a3 = (unselectedClr.R = b13);
				unselectedClr.A = a3;
				tRec.X = 0;
				tRec.Y = 0;
				tRec.Width = 1;
				tRec.Height = 1;
				float num = (float)viewport.TitleSafeArea.Width / 1280f;
				float num2 = (float)viewport.TitleSafeArea.Height / 720f;
				float[] array = new float[3] { 28f, 131f, 234f };
				float[] array2 = new float[4] { 86f, 183f, 280f, 377f };
				float[] array3 = new float[4] { 608f, 711f, 814f, 917f };
				float[] array4 = new float[5] { 86f, 183f, 280f, 377f, 474f };
				float[] array5 = new float[4] { 104f, 176f, 248f, 320f };
				float[] array6 = new float[5] { 86f, 183f, 280f, 377f, 474f };
				bool flag = VacinityItemList[qIndex].Count == 0 || e.IsAttached0;
				tRec.X = viewport.TitleSafeArea.Left;
				tRec.Y = viewport.TitleSafeArea.Top;
				if (flag)
				{
					tmpRec.X = 0;
					tmpRec.Y = 0;
					tmpRec.Width = 1638;
					tmpRec.Height = 1024;
					tRec.Width = (int)(1024f * num);
					tRec.Height = (int)(576f * num2);
					Menu.spriteBatch.Draw(Menu00, tRec, tmpRec, Color.White);
				}
				else
				{
					tRec.Width = (int)(1280f * num);
					tRec.Height = (int)(576f * num2);
					Menu.spriteBatch.Draw(Menu00, tRec, Color.White);
				}
				if (selectedSlot != 3 && WorldItemsCls.TentMenuOpen)
				{
					tRec.X = viewport.TitleSafeArea.Left + (int)(1022f * num);
					tRec.Y = viewport.TitleSafeArea.Top;
					tRec.Width = (int)(258f * num);
					tRec.Height = (int)(128f * num2);
					Menu.spriteBatch.Draw(tentUnSelected, tRec, Color.White);
				}
				if (selectedSlot > 0)
				{
					tRec.X = viewport.TitleSafeArea.Left + (int)(14f * num);
					tRec.Y = viewport.TitleSafeArea.Top + (int)(26f * num2);
					tRec.Width = (int)(64f * num);
					tRec.Height = (int)(40f * num2);
					Menu.spriteBatch.Draw(leftbutton, tRec, Color.White);
				}
				if ((flag && selectedSlot < 2) || (!flag && selectedSlot < 3))
				{
					if (flag)
					{
						tRec.X = viewport.TitleSafeArea.Left + (int)(948f * num);
					}
					else
					{
						tRec.X = viewport.TitleSafeArea.Left + (int)(1204f * num);
					}
					tRec.Y = viewport.TitleSafeArea.Top + (int)(26f * num2);
					tRec.Width = (int)(64f * num);
					tRec.Height = (int)(40f * num2);
					Menu.spriteBatch.Draw(rightbutton, tRec, Color.White);
				}
				if (selectedSlot == 0)
				{
					tRec.X = viewport.TitleSafeArea.Left;
					tRec.Y = viewport.TitleSafeArea.Top;
					tRec.Width = (int)(335f * num);
					tRec.Height = (int)(480f * num2);
					Menu.spriteBatch.Draw(InventorySelect, tRec, Color.White);
					tRec.X = viewport.TitleSafeArea.Left + (int)((array[selectedHorizontal] - 9f) * num);
					tRec.Y = viewport.TitleSafeArea.Top + (int)((array2[selectedVertical] - 9f) * num2);
					tRec.Width = (int)(102f * num);
					tRec.Height = (int)(102f * num2);
					Menu.spriteBatch.Draw(selected, tRec, Color.White);
				}
				else if (selectedSlot == 1)
				{
					tRec.X = viewport.TitleSafeArea.Left + (int)(332f * num);
					tRec.Y = viewport.TitleSafeArea.Top;
					tRec.Width = (int)(260f * num);
					tRec.Height = (int)(400f * num2);
					Menu.spriteBatch.Draw(WeaponSelect, tRec, Color.White);
					tRec.X = viewport.TitleSafeArea.Left + (int)(336f * num);
					tRec.Y = viewport.TitleSafeArea.Top + (int)((array5[selectedVertical] - 9f) * num2);
					tRec.Height = (int)(80f * num2);
					if (selectedVertical < 2)
					{
						tRec.Width = (int)(122f * num);
						Menu.spriteBatch.Draw(selectedPistol, tRec, Color.White);
					}
					else
					{
						tRec.Width = (int)(254f * num);
						Menu.spriteBatch.Draw(selectedRifle, tRec, Color.White);
					}
				}
				else if (selectedSlot == 2)
				{
					tRec.X = viewport.TitleSafeArea.Left + (int)(594f * num);
					tRec.Y = viewport.TitleSafeArea.Top;
					tRec.Width = (int)(432f * num);
					tRec.Height = (int)(576f * num2);
					if (CurrentBackPack != null)
					{
						if (CurrentBackPack.ItemType == 4)
						{
							Menu.spriteBatch.Draw(BackpackLarge, tRec, Color.White);
						}
						else if (CurrentBackPack.ItemType == 5)
						{
							Menu.spriteBatch.Draw(BackpackMedium, tRec, Color.White);
						}
						else if (CurrentBackPack.ItemType == 6)
						{
							Menu.spriteBatch.Draw(BackpackSmall, tRec, Color.White);
						}
					}
					tRec.X = viewport.TitleSafeArea.Left + (int)((array3[selectedHorizontal] - 9f) * num);
					tRec.Y = viewport.TitleSafeArea.Top + (int)((array4[selectedVertical] - 9f) * num2);
					tRec.Width = (int)(102f * num);
					tRec.Height = (int)(102f * num2);
					Menu.spriteBatch.Draw(selected, tRec, Color.White);
				}
				else if (selectedSlot == 3)
				{
					if (VacinityItemList[qIndex].Count > 0)
					{
						tRec.X = viewport.TitleSafeArea.Left + (int)(1022f * num);
						tRec.Y = viewport.TitleSafeArea.Top;
						tRec.Width = (int)(258f * num);
						tRec.Height = (int)(576f * num2);
						if (WorldItemsCls.TentMenuOpen)
						{
							Menu.spriteBatch.Draw(tentSelected, tRec, Color.White);
						}
						else
						{
							Menu.spriteBatch.Draw(VacinitySelect, tRec, Color.White);
						}
					}
					int num3 = selectedVertical - VacItemStartIndex[qIndex];
					num3 = ((num3 >= 0) ? ((num3 > 4) ? 4 : num3) : 0);
					tRec.X = viewport.TitleSafeArea.Left + (int)(1087f * num);
					tRec.Y = viewport.TitleSafeArea.Top + (int)((array6[num3] - 9f) * num2);
					tRec.Width = (int)(186f * num);
					tRec.Height = (int)(102f * num2);
					Menu.spriteBatch.Draw(selectedVicinity, tRec, Color.White);
				}
				if (selectedSlot == 0)
				{
					clr = selectedClr;
				}
				else
				{
					clr = unselectedClr;
				}
				int num4 = 0;
				string text = "";
				Vector2 zero = Vector2.Zero;
				Color white = clr;
				Texture2D texture2D = multiItemTex;
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						tRec.X = viewport.TitleSafeArea.Left + (int)(array[j] * num);
						tRec.Y = viewport.TitleSafeArea.Top + (int)(array2[i] * num2);
						tRec.Width = (int)(84f * num);
						tRec.Height = (int)(84f * num2);
						white = clr;
						if (selectedSlot == 0 && selectedHorizontal == j && selectedVertical == i)
						{
							white = Color.White;
						}
						texture2D = multiItemTex;
						if (InventoryArray[0].list[i * 3 + j].desc != 16384)
						{
							texture2D = DrawInventoryIcon(InventoryArray[0].list[i * 3 + j]);
						}
						if (texture2D != null)
						{
							Menu.spriteBatch.Draw(texture2D, tRec, white);
							if (texture2D != multiItemTex && (InventoryArray[0].list[i * 3 + j].desc & 0x400) > 0 && InventoryArray[0].list[i * 3 + j].ItemType >= 12)
							{
								text = ((ItemCls)InventoryArray[0].list[i * 3 + j].item).reserved0.ToString();
								Menu.spriteBatch.DrawString(p: new Vector2(tRec.X + 4, tRec.Y), f: Menu.defaultFont, s: text, c: white);
							}
						}
						num4++;
					}
				}
				if (selectedSlot == 1)
				{
					clr = selectedClr;
				}
				else
				{
					clr = unselectedClr;
				}
				for (int k = 0; k < 4; k++)
				{
					tRec.X = viewport.TitleSafeArea.Left + (int)(345f * num);
					tRec.Y = viewport.TitleSafeArea.Top + (int)(array5[k] * num2);
					if (k < 2)
					{
						tRec.Width = (int)(102f * num);
					}
					else
					{
						tRec.Width = (int)(232f * num);
					}
					tRec.Height = (int)(64f * num2);
					white = clr;
					if (selectedSlot == 1 && selectedVertical == k)
					{
						white = Color.White;
					}
					texture2D = DrawInventoryIcon(InventoryArray[1].list[k]);
					if (texture2D != null)
					{
						Menu.spriteBatch.Draw(texture2D, tRec, white);
						if (WeaponsCls.itemsModels[InventoryArray[1].list[k].ItemType].WepCategory != WeaponCategory.Melee)
						{
							text = ((ItemCls)InventoryArray[1].list[k].item).reserved0.ToString();
							Menu.spriteBatch.DrawString(p: new Vector2(tRec.X + 4, tRec.Y), f: Menu.defaultFont, s: text, c: white);
						}
					}
				}
				if (selectedSlot == 2)
				{
					clr = selectedClr;
				}
				else
				{
					clr = unselectedClr;
				}
				num4 = 0;
				int num5 = 0;
				if (CurrentBackPack != null)
				{
					tRec.X = viewport.TitleSafeArea.Left + (int)(612f * num);
					tRec.Y = viewport.TitleSafeArea.Top + (int)(11f * num2);
					tRec.Width = (int)(75f * num);
					tRec.Height = (int)(75f * num2);
					Menu.spriteBatch.Draw(EquipmentTexture[CurrentBackPack.ItemType], tRec, clr);
					if (CurrentBackPack.ItemType == 4)
					{
						num5 = 5;
					}
					else if (CurrentBackPack.ItemType == 5)
					{
						num5 = 3;
					}
					else if (CurrentBackPack.ItemType == 6)
					{
						num5 = 2;
					}
				}
				for (int l = 0; l < num5; l++)
				{
					for (int m = 0; m < 4; m++)
					{
						tRec.X = viewport.TitleSafeArea.Left + (int)(array3[m] * num);
						tRec.Y = viewport.TitleSafeArea.Top + (int)(array4[l] * num2);
						tRec.Width = (int)(84f * num);
						tRec.Height = (int)(84f * num2);
						white = clr;
						if (selectedSlot == 2 && selectedHorizontal == m && selectedVertical == l)
						{
							white = Color.White;
						}
						texture2D = multiItemTex;
						if (InventoryArray[2].list[l * 4 + m].desc != 16384)
						{
							texture2D = DrawInventoryIcon(InventoryArray[2].list[l * 4 + m]);
						}
						if (texture2D != null)
						{
							Menu.spriteBatch.Draw(texture2D, tRec, white);
							if (texture2D != multiItemTex && (InventoryArray[2].list[l * 4 + m].desc & 0x400) > 0 && InventoryArray[2].list[l * 4 + m].ItemType >= 12)
							{
								text = ((ItemCls)InventoryArray[2].list[l * 4 + m].item).reserved0.ToString();
								Menu.spriteBatch.DrawString(p: new Vector2(tRec.X + 4, tRec.Y), f: Menu.defaultFont, s: text, c: white);
							}
						}
						num4++;
					}
				}
				if (!flag)
				{
					if (selectedSlot == 3)
					{
						clr = selectedClr;
					}
					else
					{
						clr = unselectedClr;
					}
					int num6 = 0;
					for (int n = VacItemStartIndex[qIndex]; n < VacinityItemList[qIndex].Count; n++)
					{
						if (num6 >= 5)
						{
							break;
						}
						tRec.X = viewport.TitleSafeArea.Left + (int)(1096f * num);
						tRec.Y = viewport.TitleSafeArea.Top + (int)(array6[num6] * num2);
						tRec.Width = (int)(84f * num);
						tRec.Height = (int)(84f * num2);
						int num7 = selectedVertical - VacItemStartIndex[qIndex];
						if (selectedSlot == 3)
						{
							if (num7 == num6)
							{
								clr = Color.White;
							}
							else
							{
								clr = selectedClr;
							}
						}
						else
						{
							clr = unselectedClr;
						}
						Texture2D texture2D2 = null;
						if (VacinityItemList[qIndex][n].desc != 0)
						{
							if ((VacinityItemList[qIndex][n].desc & 0x100) > 0)
							{
								texture2D2 = ConsumableTexture[VacinityItemList[qIndex][n].ItemType];
								Menu.spriteBatch.Draw(texture2D2, tRec, clr);
							}
							else if ((VacinityItemList[qIndex][n].desc & 0x400) > 0)
							{
								texture2D2 = EquipmentTexture[VacinityItemList[qIndex][n].ItemType];
								Menu.spriteBatch.Draw(texture2D2, tRec, clr);
								if (VacinityItemList[qIndex][n].ItemType >= 12)
								{
									text = ((ItemCls)VacinityItemList[qIndex][n].item).reserved0.ToString();
									Menu.spriteBatch.DrawString(p: new Vector2(tRec.X + 4, tRec.Y), f: Menu.defaultFont, s: text, c: white);
								}
							}
							else if ((VacinityItemList[qIndex][n].desc & 0x200) > 0)
							{
								texture2D2 = WeaponTexture[(int)WeaponsCls.itemsModels[VacinityItemList[qIndex][n].ItemType].WepType];
								tRec.Y = viewport.TitleSafeArea.Top + (int)((array6[num6] + 8f) * num2);
								tRec.Height = (int)(84f * num2);
								if (WeaponsCls.itemsModels[VacinityItemList[qIndex][n].ItemType].WepCategory == WeaponCategory.Melee || WeaponsCls.itemsModels[VacinityItemList[qIndex][n].ItemType].WepCategory == WeaponCategory.Pistol)
								{
									tmpRec.X = 0;
									tmpRec.Width = 102;
									tRec.Width = (int)(102f * num);
								}
								else
								{
									tmpRec.X = 0;
									tmpRec.Width = 158;
									tRec.Width = (int)(158f * num);
								}
								tmpRec.Y = 0;
								tmpRec.Height = 84;
								Menu.spriteBatch.Draw(texture2D2, tRec, tmpRec, clr);
								if (WeaponsCls.itemsModels[VacinityItemList[qIndex][n].ItemType].WepCategory != WeaponCategory.Melee)
								{
									text = ((ItemCls)VacinityItemList[qIndex][n].item).reserved0.ToString();
									Menu.spriteBatch.DrawString(p: new Vector2(tRec.X + 4, tRec.Y), f: Menu.defaultFont, s: text, c: white);
								}
							}
						}
						num6++;
					}
					if (selectedSlot == 3)
					{
						clr = Color.White;
					}
					else
					{
						clr = unselectedClr;
					}
					if (VacItemStartIndex[qIndex] > 0)
					{
						tRec.X = viewport.TitleSafeArea.Left + (int)(1042f * num);
						tRec.Y = viewport.TitleSafeArea.Top + (int)(108f * num2);
						tRec.Width = (int)(44f * num);
						tRec.Height = (int)(44f * num2);
						Menu.DrawButton(tRec, Buttons.DPadUp, clr);
					}
					if (VacinityItemList[qIndex].Count > 4 && VacItemStartIndex[qIndex] < VacinityItemList[qIndex].Count - 5)
					{
						tRec.X = viewport.TitleSafeArea.Left + (int)(1042f * num);
						tRec.Y = viewport.TitleSafeArea.Top + (int)(496f * num2);
						tRec.Width = (int)(44f * num);
						tRec.Height = (int)(44f * num2);
						Menu.DrawButton(tRec, Buttons.DPadDown, clr);
					}
				}
				uiPos.X = viewport.TitleSafeArea.Left + (int)(32f * num);
				uiPos.Y = viewport.TitleSafeArea.Top + (int)(516f * num2);
				tRec.X = (int)uiPos.X;
				tRec.Y = (int)(uiPos.Y + 2f * num2);
				tRec.Width = (int)(44f * num);
				tRec.Height = (int)(44f * num2);
				uiPos.X += 32f * num;
				string b16 = " Use";
				if ((invDrawItem.desc & 0x400) > 0)
				{
					b16 = ((invDrawItem.ItemType != 11) ? " Equip" : " Deploy");
				}
				if (selectedSlot == 3)
				{
					b16 = " Pickup";
					Menu.DrawButton(tRec, Buttons.A, Color.White);
				}
				else
				{
					Menu.DrawButton(tRec, Buttons.X, Color.White);
				}
				Menu.spriteBatch.DrawString(Menu.defaultFont, b16, uiPos, new Color(42, 62, 99), 0f, new Vector2(-2f, -2f), 1.25f * num, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, b16, uiPos, new Color(102, 122, 159), 0f, Vector2.Zero, 1.25f * num, SpriteEffects.None, 0);
				if (selectedSlot != 3)
				{
					uiPos.X += 140f;
					tRec.X = (int)(uiPos.X - 32f * num);
					tRec.Y = (int)(uiPos.Y + 2f * num2);
					tRec.Width = (int)(44f * num);
					tRec.Height = (int)(44f * num2);
					Menu.DrawButton(tRec, Buttons.A, Color.White);
					Menu.spriteBatch.DrawString(Menu.defaultFont, " Drop", uiPos, new Color(119, 14, 0), 0f, new Vector2(-2f, -2f), 1.25f * num, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, " Drop", uiPos, new Color(179, 74, 51), 0f, Vector2.Zero, 1.25f * num, SpriteEffects.None, 0);
				}
				uiPos.X += 140f;
				tRec.X = (int)(uiPos.X - 34f * num);
				tRec.Y = (int)(uiPos.Y + 2f * num2);
				tRec.Width = (int)(44f * num);
				tRec.Height = (int)(44f * num2);
				Menu.DrawButton(tRec, Buttons.B, Color.White);
				Menu.spriteBatch.DrawString(Menu.defaultFont, " Close", uiPos, Color.DarkGray, 0f, new Vector2(-2f, -2f), 1.25f * num, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, " Close", uiPos, Color.LightGray, 0f, Vector2.Zero, 1.25f * num, SpriteEffects.None, 0);
				uiPos.X = viewport.TitleSafeArea.Left + (int)(32f * num);
				uiPos.Y = viewport.TitleSafeArea.Top + (int)(476f * num2);
				if (selectedSlot < 3)
				{
					DrawInventoryItem(uiPos, InventoryArray[selectedSlot].list[selectedVertical * (int)SlotGridExtents[selectedSlot].X + selectedHorizontal], selected: true, 1.1f * num);
				}
				else if (VacinityItemList[qIndex].Count > selectedVertical && VacinityItemList[qIndex][selectedVertical] != null)
				{
					DrawInventoryItem(uiPos, VacinityItemList[qIndex][selectedVertical], selected: true, 1.1f * num);
				}
			}
			catch (Exception ex)
			{
				MessagePump.AddMessage("InvDraw: " + ex.Message);
			}
		}
		Menu.spriteBatch.End();
	}

	private void DrawInventoryItem(Vector2 pos, InventoryItemCls e, bool selected, float scale)
	{
		string b = "Empty";
		if (e.desc != 0)
		{
			if ((e.desc & 0x100) > 0)
			{
				b = ConsumableCls.ConsumableItemsDesc[e.ItemType];
			}
			else if ((e.desc & 0x400) > 0)
			{
				b = EquipmentCls.EquipmentItemDesc[e.ItemType];
			}
			else if ((e.desc & 0x200) > 0)
			{
				b = WeaponsCls.WeaponsItemsDesc[e.ItemType];
			}
		}
		Menu.spriteBatch.DrawString(d: new Color(82, 100, 110), a: Menu.defaultFont, b: b, c: pos, e: 0f, f: Vector2.Zero, g: scale, h: SpriteEffects.None, i: 0);
	}

	private Texture2D DrawInventoryIcon(InventoryItemCls e)
	{
		Texture2D result = null;
		if (e.desc != 0)
		{
			if ((e.desc & 0x100) > 0)
			{
				result = ConsumableTexture[e.ItemType];
			}
			else if ((e.desc & 0x400) > 0)
			{
				result = EquipmentTexture[e.ItemType];
			}
			else if ((e.desc & 0x200) > 0)
			{
				result = WeaponTexture[(int)WeaponsCls.itemsModels[e.ItemType].WepType];
			}
		}
		return result;
	}

	public void Setup()
	{
	}

	public virtual void Reset()
	{
		CurrentBackPack = null;
		InventoryArray[0].slotCount = CurrentPocketSlots;
		InventoryArray[0].list.Clear();
		for (int i = 0; i < 12; i++)
		{
			InventoryArray[0].list.Add(new InventoryItemCls());
		}
		InventoryArray[1].slotCount = CurrentWeaponSlots;
		InventoryArray[1].list.Clear();
		for (int j = 0; j < 4; j++)
		{
			InventoryArray[1].list.Add(new InventoryItemCls());
		}
		InventoryArray[2].slotCount = CurrentBackPackSlots;
		InventoryArray[2].list.Clear();
		for (int k = 0; k < 20; k++)
		{
			InventoryArray[2].list.Add(new InventoryItemCls());
		}
		InventoryArray[3].slotCount = 5;
		InventoryArray[3].list.Clear();
		for (int l = 0; l < 5; l++)
		{
			InventoryArray[3].list.Add(new InventoryItemCls());
		}
	}

	public bool AddItem(InventorySlot slot, InventoryItemCls item)
	{
		ApocZSaveDataCls.RemoveLocalPlayerTent((ItemCls)item.item);
		if ((item.desc & 0x400) > 0 && (item.ItemType == 4 || item.ItemType == 5 || item.ItemType == 6))
		{
			int num = 8;
			if (item.ItemType == 4)
			{
				num = 20;
			}
			else if (item.ItemType == 5)
			{
				num = 12;
			}
			if (CurrentBackPack != null)
			{
				DropItem(CurrentBackPack, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value]);
				if (num < CurrentBackPackSlots)
				{
					int i = 0;
					int num2 = 0;
					for (; i < CurrentBackPackSlots; i++)
					{
						InventoryItemCls inventoryItemCls = InventoryArray[2].list[i];
						if (inventoryItemCls.desc != 0 && inventoryItemCls.desc != 16384)
						{
							for (num2 = i + 1; num2 < CurrentBackPackSlots && InventoryArray[2].list[num2].desc == 16384; num2++)
							{
							}
							if (num2 > num)
							{
								break;
							}
						}
					}
					for (; i < CurrentBackPackSlots; i++)
					{
						InventoryItemCls inventoryItemCls2 = InventoryArray[2].list[i];
						if (inventoryItemCls2.desc != 0)
						{
							DropItem(inventoryItemCls2, LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value]);
							InventoryArray[2].list[i] = new InventoryItemCls();
							for (int j = i + 1; j < CurrentBackPackSlots && InventoryArray[2].list[j].desc == 16384; j++)
							{
								InventoryArray[2].list[j].desc = 0;
								InventoryArray[2].list[j].item = null;
							}
						}
					}
					if (!HaveItem(1024, 7))
					{
						LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].FlashLightOn = false;
						PlayerBase.NetworkUpdateFrameCount = -1;
					}
				}
			}
			CurrentBackPack = item;
			CurrentBackPackSlots = num;
			InventoryArray[2].valid = true;
			InventoryArray[2].slotCount = CurrentBackPackSlots;
			PlayerBase.NetworkUpdateFrameCount = -1;
			return true;
		}
		if ((item.desc & 0x200) > 0)
		{
			if (WeaponsCls.itemsModels[item.ItemType].WepCategory == WeaponCategory.Pistol || WeaponsCls.itemsModels[item.ItemType].WepCategory == WeaponCategory.Melee)
			{
				if (InventoryArray[1].list[0].desc == 0)
				{
					InventoryArray[1].list[0] = item;
					return true;
				}
				if (PistolTwoHolster && InventoryArray[1].list[1].desc == 0)
				{
					InventoryArray[1].list[1] = item;
					return true;
				}
				MessagePump.AddMessage("Cant Pick Up Weapon");
				return false;
			}
			if (InventoryArray[1].list[2].desc == 0)
			{
				InventoryArray[1].list[2] = item;
				return true;
			}
			if (SecondRifleSling && InventoryArray[1].list[3].desc == 0)
			{
				InventoryArray[1].list[3] = item;
				return true;
			}
			MessagePump.AddMessage("Cant Pick Up Weapon");
			return false;
		}
		if (slot == InventoryArray[0].slot && InventoryArray[0].valid && AddItemToInventory(item, InventoryArray[0].list, InventoryArray[0].slotCount))
		{
			return true;
		}
		if (slot != InventoryArray[2].slot && InventoryArray[2].valid && AddItemToInventory(item, InventoryArray[2].list, InventoryArray[2].slotCount))
		{
			return true;
		}
		return false;
	}

	private bool AddItemToInventory(InventoryItemCls item, List<InventoryItemCls> e, int n)
	{
		int num = CountEmptySlot(e, n);
		int num2 = 1;
		if ((item.desc & 0x100) > 0)
		{
			num2 = ConsumableCls.ItemSlotUse[item.ItemType];
		}
		else if ((item.desc & 0x400) > 0)
		{
			num2 = EquipmentCls.ItemSlotUse[item.ItemType];
		}
		if (num >= num2)
		{
			ConsolidateInventory(e, n);
			for (int i = 0; i < n; i++)
			{
				if (e[i].desc == 0)
				{
					item.desc &= 32767;
					((ItemCls)item.item).desc &= 32767;
					e[i] = item;
					for (int j = 1; j < num2; j++)
					{
						e[i + j].desc = 16384;
						e[i + j].item = null;
					}
					return true;
				}
			}
		}
		return false;
	}

	private void ConsolidateInventory(List<InventoryItemCls> e, int n)
	{
		for (int i = 0; i < n; i++)
		{
			if (e[i].desc != 0)
			{
				continue;
			}
			for (int j = i + 1; j < n; j++)
			{
				if (e[j].desc != 0)
				{
					e[i].desc = e[j].desc;
					e[i].item = e[j].item;
					e[j].desc = 0;
					i++;
				}
			}
		}
	}

	public bool CanPickUpItem(ItemCls e)
	{
		_ = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		if ((e.desc & 0x400) > 0 && (e.ItemType == 4 || e.ItemType == 5 || e.ItemType == 6))
		{
			return true;
		}
		if ((e.desc & 0x200) > 0)
		{
			if (WeaponsCls.itemsModels[e.ItemType].WepCategory == WeaponCategory.Pistol || WeaponsCls.itemsModels[e.ItemType].WepCategory == WeaponCategory.Melee)
			{
				if (InventoryArray[1].list[0].desc == 0)
				{
					return true;
				}
				if (PistolTwoHolster && InventoryArray[1].list[1].desc == 0)
				{
					return true;
				}
				MessagePump.AddMessage("Cant Pick Up Weapon");
				return false;
			}
			if (InventoryArray[1].list[2].desc == 0)
			{
				return true;
			}
			if (SecondRifleSling && InventoryArray[1].list[3].desc == 0)
			{
				return true;
			}
			MessagePump.AddMessage("Cant Pick Up Weapon");
			return false;
		}
		for (int i = 0; i < 3; i++)
		{
			if (i == 1 || (i == 2 && CurrentBackPack == null))
			{
				continue;
			}
			int num = CountEmptySlot(InventoryArray[i].list, InventoryArray[i].slotCount);
			if (e.IsConsumable)
			{
				if (num >= ConsumableCls.ItemSlotUse[e.ItemType])
				{
					return true;
				}
			}
			else if (e.IsEquipment && num >= EquipmentCls.ItemSlotUse[e.ItemType])
			{
				return true;
			}
		}
		MessagePump.AddMessage("Cant Pick Up Item, Inventory Full");
		return false;
	}

	private int CountEmptySlot(List<InventoryItemCls> e, int n)
	{
		int num = 0;
		for (int i = 0; i < n; i++)
		{
			if (e[i].desc == 0)
			{
				num++;
			}
		}
		return num;
	}

	public bool HaveCompass()
	{
		haveCompasstimer -= 0.03334f;
		if (haveCompasstimer < 0f)
		{
			haveCompasstimer = 1f;
			haveCompass = HaveItem(1024, 9);
		}
		return haveCompass;
	}

	public bool HaveItem(ushort category, ushort item)
	{
		ItemCls itemRef = null;
		return HaveItem(category, item, ref itemRef);
	}

	public bool HaveItem(ushort category, ushort item, ref ItemCls itemRef)
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < InventoryArray[i].slotCount; j++)
			{
				if ((InventoryArray[i].list[j].desc & category) > 0 && InventoryArray[i].list[j].ItemType == item)
				{
					itemRef = (ItemCls)InventoryArray[i].list[j].item;
					return true;
				}
			}
		}
		return false;
	}

	public void DropAll(PlayerBase e)
	{
		e.FlashLightOn = false;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < InventoryArray[i].slotCount; j++)
			{
				if (InventoryArray[i].list[j].desc != 0)
				{
					DropItem(InventoryArray[i].list[j], e);
					InventoryArray[i].list[j].desc = 0;
					for (int k = j + 1; k < InventoryArray[i].slotCount && InventoryArray[i].list[k].desc == 16384; k++)
					{
						InventoryArray[i].list[k].desc = 0;
					}
				}
			}
		}
		if (CurrentBackPack != null)
		{
			DropItem(CurrentBackPack, e);
			CurrentBackPack = null;
		}
		PlayerBase.NetworkUpdateFrameCount = -1;
	}

	public void DestroyItem(ushort category, ItemCls itemRef)
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < InventoryArray[i].slotCount; j++)
			{
				if (InventoryArray[i].list[j].item == itemRef)
				{
					InventoryArray[i].list[j].desc = 0;
					for (j++; j < InventoryArray[i].slotCount && InventoryArray[i].list[j].desc == 16384; j++)
					{
						InventoryArray[i].list[j].desc = 0;
					}
					return;
				}
			}
		}
	}

	public void DestroyItem(ushort category, ushort item)
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < InventoryArray[i].slotCount; j++)
			{
				if (InventoryArray[i].list[j].ItemType == item)
				{
					InventoryArray[i].list[j].desc = 0;
					for (j++; j < InventoryArray[i].slotCount && InventoryArray[i].list[j].desc == 16384; j++)
					{
						InventoryArray[i].list[j].desc = 0;
					}
					return;
				}
			}
		}
	}

	public void EmptyItem(ushort category, ushort item)
	{
		for (int i = 0; i < 3; i++)
		{
			if (i == 1)
			{
				continue;
			}
			for (int j = 0; j < InventoryArray[i].slotCount; j++)
			{
				if ((InventoryArray[i].list[j].desc & category) > 0 && InventoryArray[i].list[j].ItemType == item)
				{
					InventoryArray[i].list[j].desc = (ushort)(category | (ushort)(item - 1));
					return;
				}
			}
		}
	}

	public void SaveInventory(Stream sr)
	{
		if (CurrentBackPack != null)
		{
			sr.WriteByte(1);
			sr.WriteByte((byte)(CurrentBackPack.desc & 0xFF));
			sr.WriteByte((byte)((CurrentBackPack.desc >> 8) & 0xFF));
		}
		else
		{
			sr.WriteByte(0);
		}
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < InventoryArray[i].slotCount; j++)
			{
				if (InventoryArray[i].list[j].desc != 0 && InventoryArray[i].list[j].desc != 16384)
				{
					num++;
				}
			}
		}
		sr.WriteByte((byte)num);
		for (int k = 0; k < 3; k++)
		{
			for (int l = 0; l < InventoryArray[k].slotCount; l++)
			{
				if (InventoryArray[k].list[l].desc != 0 && InventoryArray[k].list[l].desc != 16384)
				{
					sr.WriteByte((byte)(InventoryArray[k].list[l].desc & 0xFF));
					sr.WriteByte((byte)((InventoryArray[k].list[l].desc >> 8) & 0xFF));
					sr.WriteByte(((ItemCls)InventoryArray[k].list[l].item).reserved0);
				}
			}
		}
	}

	public void ReadInventory(Stream sr)
	{
		int num = sr.ReadByte();
		if (num == 1)
		{
			InventoryItemCls inventoryItemCls = new InventoryItemCls();
			ItemCls itemCls = new ItemCls();
			itemCls.desc = (ushort)sr.ReadByte();
			itemCls.desc |= (ushort)((ushort)sr.ReadByte() << 8);
			itemCls.uid = WorldItemsCls.UniqueId;
			itemCls.pos = Vector3.Zero;
			if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerRef != null)
			{
				itemCls.ownerNetId = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerRef.Id;
			}
			else
			{
				itemCls.ownerNetId = 1;
			}
			inventoryItemCls.desc = itemCls.desc;
			inventoryItemCls.item = itemCls;
			AddItem(InventorySlot.Pockets, inventoryItemCls);
		}
		else
		{
			CurrentBackPack = null;
		}
		int num2 = sr.ReadByte();
		for (int i = 0; i < num2; i++)
		{
			ushort num3 = 0;
			num3 = (ushort)sr.ReadByte();
			num3 |= (ushort)((ushort)sr.ReadByte() << 8);
			byte reserved = (byte)sr.ReadByte();
			ItemCls itemCls2 = new ItemCls();
			itemCls2.desc = num3;
			itemCls2.uid = WorldItemsCls.UniqueId;
			itemCls2.pos = Vector3.Zero;
			if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerRef != null)
			{
				itemCls2.ownerNetId = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerRef.Id;
			}
			else
			{
				itemCls2.ownerNetId = 1;
			}
			itemCls2.reserved0 = reserved;
			InventoryItemCls inventoryItemCls2 = new InventoryItemCls();
			inventoryItemCls2.desc = itemCls2.desc;
			inventoryItemCls2.item = itemCls2;
			AddItem(InventorySlot.Pockets, inventoryItemCls2);
		}
	}

	public void SaveInventory(byte[] buff, ref int idx)
	{
		if (CurrentBackPack != null)
		{
			buff[idx++] = 1;
			buff[idx++] = (byte)(CurrentBackPack.desc & 0xFF);
			buff[idx++] = (byte)((CurrentBackPack.desc >> 8) & 0xFF);
		}
		else
		{
			buff[idx++] = 0;
		}
		int num = 0;
		int num2 = idx++;
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < InventoryArray[i].slotCount; j++)
			{
				if (InventoryArray[i].list[j].desc != 0 && InventoryArray[i].list[j].desc != 16384)
				{
					buff[idx++] = (byte)(InventoryArray[i].list[j].desc & 0xFF);
					buff[idx++] = (byte)((InventoryArray[i].list[j].desc >> 8) & 0xFF);
					buff[idx++] = ((ItemCls)InventoryArray[i].list[j].item).reserved0;
					num++;
				}
			}
		}
		buff[num2] = (byte)num;
	}

	public void ReadInventory(byte[] buff, ref int idx)
	{
		int num = buff[idx++];
		if (num == 1)
		{
			InventoryItemCls inventoryItemCls = new InventoryItemCls();
			ItemCls itemCls = new ItemCls();
			itemCls.desc = buff[idx++];
			itemCls.desc |= (ushort)(buff[idx++] << 8);
			itemCls.uid = WorldItemsCls.UniqueId;
			itemCls.pos = Vector3.Zero;
			if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerRef != null)
			{
				itemCls.ownerNetId = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerRef.Id;
			}
			else
			{
				itemCls.ownerNetId = 1;
			}
			inventoryItemCls.desc = itemCls.desc;
			inventoryItemCls.item = itemCls;
			AddItem(InventorySlot.Pockets, inventoryItemCls);
		}
		else
		{
			CurrentBackPack = null;
		}
		int num2 = buff[idx++];
		for (int i = 0; i < num2; i++)
		{
			ushort num3 = 0;
			num3 = buff[idx++];
			num3 |= (ushort)(buff[idx++] << 8);
			byte reserved = buff[idx++];
			ItemCls itemCls2 = new ItemCls();
			itemCls2.desc = num3;
			itemCls2.uid = WorldItemsCls.UniqueId;
			itemCls2.pos = Vector3.Zero;
			if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerRef != null)
			{
				itemCls2.ownerNetId = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].NetGamerRef.Id;
			}
			else
			{
				itemCls2.ownerNetId = 1;
			}
			itemCls2.reserved0 = reserved;
			InventoryItemCls inventoryItemCls2 = new InventoryItemCls();
			inventoryItemCls2.desc = itemCls2.desc;
			inventoryItemCls2.item = itemCls2;
			AddItem(InventorySlot.Pockets, inventoryItemCls2);
		}
	}
}
