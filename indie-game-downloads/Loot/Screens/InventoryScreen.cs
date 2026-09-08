using System;
using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Scrolls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class InventoryScreen : Screen
{
	private enum SlotType
	{
		Equipment,
		Inventory
	}

	private class Menu
	{
		public static Menu Unequip = new Menu(new MenuItem("Unequip", activate), new MenuItem("Sort", sort));

		public static Menu Equip = new Menu(new MenuItem("Equip", activate), new MenuItem("Destroy", destroy), new MenuItem("Sort", sort));

		public static Menu Use = new Menu(new MenuItem("Use", activate), new MenuItem("Destroy", destroy), new MenuItem("Sort", sort));

		public static Menu Junk = new Menu(new MenuItem("Destroy", destroy), new MenuItem("Sort", sort));

		public static Menu Sort = new Menu(new MenuItem("Sort", sort));

		private MenuItem[] menuItems;

		public int Length => menuItems.Length;

		public Menu(params MenuItem[] menuItems)
		{
			this.menuItems = menuItems;
		}

		public string GetName(int menuId)
		{
			return menuItems[menuId].Name;
		}

		public State OnClick(int menuId)
		{
			return menuItems[menuId].OnClick();
		}
	}

	private delegate State MenuDelegate();

	private class MenuItem
	{
		public string Name;

		public MenuDelegate OnClick;

		public MenuItem(string name, MenuDelegate onClick)
		{
			Name = name;
			OnClick = onClick;
		}
	}

	private abstract class State
	{
		public static InfoState InfoState = new InfoState();

		public static MenuState MenuState = new MenuState();

		public static IdentifyState IdentifyState = new IdentifyState();

		public virtual bool UpdateSelectedItem => true;

		public abstract State Update(GameTime gameTime);

		public abstract void Draw(SpriteBatch spriteBatch);
	}

	private class InfoState : State
	{
		private const int flipVer = 368;

		private const int flipHor = 836;

		private Item lastItem;

		private bool showStats = true;

		private TimeSpan infoTimer = TimeSpan.Zero;

		private TimeSpan infoDuration = TimeSpan.FromSeconds(1.5);

		private Vector2 boxPos;

		private static Color darkRed = new Color(128, 19, 19);

		private static Color darkGreen = new Color(19, 128, 19);

		public override State Update(GameTime gameTime)
		{
			if (InventoryScreen.selectedItem != lastItem)
			{
				lastItem = InventoryScreen.selectedItem;
				infoTimer = TimeSpan.Zero;
				showStats = true;
			}
			infoTimer += gameTime.ElapsedGameTime;
			if (infoTimer >= infoDuration)
			{
				infoTimer -= infoDuration;
				showStats = !showStats;
			}
			if (MC.GamePadManager.isNewButtonDown(Buttons.A))
			{
				PlaySound.MenuClick();
				if (InventoryScreen.selectedItem == null)
				{
					State.MenuState.Open(Menu.Sort);
				}
				else if (InventoryScreen.selectedItem.Identified && InventoryScreen.selectedItem.Cursed)
				{
					if (slotType == SlotType.Equipment)
					{
						State.MenuState.Open(Menu.Sort);
					}
					else
					{
						State.MenuState.Open(Menu.Junk);
					}
				}
				else if (slotType == SlotType.Equipment)
				{
					State.MenuState.Open(Menu.Unequip);
				}
				else
				{
					Item selectedItem = InventoryScreen.selectedItem;
					Item item = selectedItem;
					if (!(item is Equipment))
					{
						if (item is Junk)
						{
							State.MenuState.Open(Menu.Junk);
						}
						else
						{
							State.MenuState.Open(Menu.Use);
						}
					}
					else
					{
						State.MenuState.Open(Menu.Equip);
					}
				}
				return State.MenuState;
			}
			if (MC.GamePadManager.isNewButtonDown(Buttons.B))
			{
				ExitScreen();
			}
			return this;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			drawHighlight(spriteBatch);
			drawInfoBox(spriteBatch);
		}

		private void setBoxPos()
		{
			boxPos = selectedItemPos + new Vector2(54f, -51f);
			if ((double)selectedItemPos.X > 836.0)
			{
				boxPos.X -= 360f;
			}
			if (!((double)selectedItemPos.Y <= 368.0))
			{
				boxPos.Y -= 72f;
			}
		}

		private void drawInfoBox(SpriteBatch spriteBatch)
		{
			if (selectedItem == null || !selectedItem.Identified || !(selectedItem is Equipment))
			{
				return;
			}
			setBoxPos();
			itemInfoBox.Draw(spriteBatch, boxPos);
			Vector2 vector = boxPos + new Vector2(26f, 32f);
			Vector2 vector2 = new Vector2(0f, 36f);
			for (int i = 0; i < 4; i++)
			{
				Vector2 v = vector + vector2 * i;
				Text.Draw(spriteBatch, ref v, Stats.Name[i], Color.Black);
				int num = compare((Equipment)selectedItem, i);
				if (num > 0)
				{
					Text.Draw(spriteBatch, ref v, '\u001d', darkGreen);
				}
				else if (num < 0)
				{
					Text.Draw(spriteBatch, ref v, '\u001e', darkRed);
				}
				else
				{
					Text.Draw(spriteBatch, ref v, ':', Color.Black);
				}
				if (slotType == SlotType.Equipment || showStats)
				{
					Text.Draw(spriteBatch, ref v, ((Equipment)selectedItem).Stats.ToString(i), Color.Black);
					continue;
				}
				if (num == 0)
				{
					Text.Draw(spriteBatch, ref v, '-', Color.Black);
					continue;
				}
				Color black = Color.Black;
				if (num < 0)
				{
					black = darkRed;
				}
				else if (num > 0)
				{
					black = darkGreen;
				}
				Text.Draw(spriteBatch, ref v, '(', black);
				if (num > 0)
				{
					Text.Draw(spriteBatch, ref v, '+', black);
				}
				Text.Draw(spriteBatch, ref v, num, black);
				Text.Draw(spriteBatch, ref v, ')', black);
			}
		}

		private int compare(Equipment item, int stat)
		{
			Stats cmp = DM.Player.GetEquipment(item.GetType())?.Stats ?? Stats.Zero;
			return item.Stats.CompareTo(stat, cmp);
		}
	}

	private class MenuState : State
	{
		private const int flipVer = 368;

		private const int flipHor = 836;

		private Menu activeMenu;

		private int menuId;

		private Vector2 boxPos;

		public override bool UpdateSelectedItem => false;

		public void Open(Menu menu)
		{
			activeMenu = menu;
			menuId = 0;
		}

		public override State Update(GameTime gameTime)
		{
			if (MC.GamePadManager.isNewButtonDown(Buttons.A))
			{
				PlaySound.MenuClick();
				return activeMenu.OnClick(menuId);
			}
			if (MC.GamePadManager.isNewButtonDown(Buttons.B))
			{
				PlaySound.MenuCancel();
				return State.InfoState;
			}
			if (MC.GamePadManager.isNewDirDown())
			{
				PlaySound.MenuMove();
				menuId++;
				if (menuId >= activeMenu.Length)
				{
					menuId = 0;
				}
				return this;
			}
			if (MC.GamePadManager.isNewDirUp())
			{
				PlaySound.MenuMove();
				menuId--;
				if (menuId < 0)
				{
					menuId = activeMenu.Length - 1;
				}
				return this;
			}
			if (!MC.GamePadManager.isNewDirLeft() && !MC.GamePadManager.isNewDirRight())
			{
				return this;
			}
			PlaySound.MenuMove();
			return State.InfoState;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			drawHighlight(spriteBatch);
			drawMenuBox(spriteBatch);
		}

		private void setBoxPos()
		{
			boxPos = selectedItemPos + new Vector2(54f, -51f);
			if ((double)selectedItemPos.X > 836.0)
			{
				boxPos.X -= 360f;
			}
			if (!((double)selectedItemPos.Y <= 368.0))
			{
				boxPos.Y -= 72f;
			}
		}

		private void drawMenuBox(SpriteBatch spriteBatch)
		{
			setBoxPos();
			itemMenuBox.Draw(spriteBatch, boxPos);
			Vector2 vector = boxPos + new Vector2(32f, 38f);
			Vector2 vector2 = boxPos + new Vector2(12f, 12f);
			Vector2 vector3 = new Vector2(0f, 48f);
			for (int i = 0; i < activeMenu.Length; i++)
			{
				Vector2 v = vector + vector3 * i;
				if (menuId == i)
				{
					itemMenuHighlight.Draw(spriteBatch, vector2 + vector3 * i);
				}
				Text.Draw(spriteBatch, v, activeMenu.GetName(i), Color.Black);
			}
		}
	}

	private class IdentifyState : State
	{
		private int scrollId;

		public void SetScroll(int id)
		{
			scrollId = id;
		}

		public override State Update(GameTime gameTime)
		{
			if (MC.GamePadManager.isNewButtonDown(Buttons.A))
			{
				PlaySound.MenuClick();
				if (selectedItem == null)
				{
					Message.Display("Select an item to identify.");
					return State.InfoState;
				}
				if (selectedItem.Identified)
				{
					Message.Display("This item is already identified.");
					return State.InfoState;
				}
				selectedItem.Identify();
				DM.Player.RemoveItem(scrollId);
				return State.InfoState;
			}
			if (!MC.GamePadManager.isNewButtonDown(Buttons.B))
			{
				return this;
			}
			PlaySound.MenuCancel();
			return State.InfoState;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			magnify.Draw(spriteBatch, selectedItemPos);
		}
	}

	private const int invOffset = 72;

	private const string shopText = "SHOP";

	private static InventoryScreen inventoryScreen = new InventoryScreen();

	private static Shop shop;

	private static State state;

	private static SlotType slotType;

	private static int selectedId;

	private static Item selectedItem;

	private static Vector2 selectedItemPos;

	private static string bannerText;

	private static GamePadRepeatUtil repeat;

	private static Sprite bg;

	private static Sprite unidentified;

	private static Sprite cursed;

	private static Sprite itemHighlight;

	private static Sprite itemInfoBox;

	private static Sprite itemMenuBox;

	private static Sprite itemMenuHighlight;

	private static Texture2D equipmentBg;

	private static Sprite magnify;

	private static Sprite shopTab;

	private static Vector2 equipPos = new Vector2(466f, 260f);

	private static Vector2 equipOffset = new Vector2(0f, 72f);

	private static Vector2 invPos = new Vector2(584f, 260f);

	private static Color shopTextColor = new Color(128, 123, 102);

	private static Color shopTextShadowColor = new Color(229, 220, 184);

	private InventoryScreen()
		: base(modal: true)
	{
	}

	public static void Display(Shop s)
	{
		Display(s, -1);
	}

	public static void Display(Shop s, int playerInventoryId)
	{
		shop = s;
		state = State.InfoState;
		if (playerInventoryId >= 0 && playerInventoryId < DM.Player.Inventory.Length)
		{
			slotType = SlotType.Inventory;
			selectedId = playerInventoryId;
			selectedItem = DM.Player.Inventory[selectedId];
		}
		else
		{
			slotType = SlotType.Equipment;
			selectedId = 0;
			selectedItem = DM.Player.Equipment[0];
		}
		selectedItemPos = getPos(slotType, selectedId);
		bannerText = "Level " + DM.Player.Level + " " + DM.Player.ClassName;
		repeat = new GamePadRepeatUtil(MC.GamePadManager, TimeSpan.FromMilliseconds(300.0), TimeSpan.FromMilliseconds(150.0));
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(inventoryScreen);
	}

	private static void ExitScreen()
	{
		PlaySound.MenuClick();
		MC.ScreenManager.removeAllScreens();
		if (shop == null)
		{
			MC.ScreenManager.addScreen(new DungeonView());
		}
		else if (slotType == SlotType.Inventory)
		{
			ShopScreen.Display(shop, selectedId);
		}
		else
		{
			ShopScreen.Display(shop);
		}
	}

	public static void Load(ContentManager content)
	{
		bg = new StillSprite(content.Load<Texture2D>("Sprites\\Inventory\\InventoryScreen"));
		unidentified = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Unidentified"));
		cursed = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Cursed"));
		itemHighlight = new AnimatedSprite(content.Load<Texture2D>("Sprites\\Inventory\\ItemHighlight"), new Rectangle(0, 0, 128, 128), new Vector2(64f, 64f), 8, TimeSpan.FromMilliseconds(75.0));
		itemInfoBox = new StillSprite(content.Load<Texture2D>("Sprites\\Inventory\\ItemInfoBox"), null, Vector2.Zero);
		itemMenuBox = new StillSprite(content.Load<Texture2D>("Sprites\\Inventory\\ItemMenuBox"), null, Vector2.Zero);
		itemMenuHighlight = new StillSprite(content.Load<Texture2D>("Sprites\\Inventory\\ItemMenuHighlight"), null, Vector2.Zero);
		magnify = new StillSprite(content.Load<Texture2D>("Sprites\\Inventory\\Magnify"), new Rectangle(0, 0, 128, 128), new Vector2(46f, 46f));
		equipmentBg = content.Load<Texture2D>("Sprites\\Inventory\\EquipmentBg");
		shopTab = new StillSprite(content.Load<Texture2D>("Sprites\\Inventory\\ShopTab"));
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.Back) || MC.GamePadManager.isNewButtonDown(Buttons.Start))
		{
			ExitScreen();
		}
		updateSprites(gameTime);
		if (state.UpdateSelectedItem)
		{
			updateSelectedItem(gameTime);
		}
		state = state.Update(gameTime);
	}

	private void updateSprites(GameTime gameTime)
	{
		itemHighlight.Update(gameTime);
	}

	private void updateSelectedItem(GameTime gameTime)
	{
		repeat.Update(gameTime);
		if (slotType == SlotType.Equipment)
		{
			if (repeat.Down())
			{
				PlaySound.MenuMove();
				selectedId++;
				if (selectedId > 3)
				{
					selectedId = 0;
				}
			}
			else if (repeat.Up())
			{
				PlaySound.MenuMove();
				selectedId--;
				if (selectedId < 0)
				{
					selectedId = 3;
				}
			}
			else if (repeat.Right())
			{
				PlaySound.MenuMove();
				selectedId *= 8;
				slotType = SlotType.Inventory;
			}
			else if (repeat.Left())
			{
				PlaySound.MenuMove();
				selectedId *= 8;
				selectedId += 7;
				slotType = SlotType.Inventory;
			}
		}
		else if (repeat.Right())
		{
			PlaySound.MenuMove();
			selectedId++;
			if (selectedId % 8 == 0)
			{
				selectedId /= 8;
				selectedId--;
				slotType = SlotType.Equipment;
			}
		}
		else if (repeat.Left())
		{
			PlaySound.MenuMove();
			if (selectedId % 8 == 0)
			{
				selectedId /= 8;
				slotType = SlotType.Equipment;
			}
			else
			{
				selectedId--;
			}
		}
		else if (repeat.Up())
		{
			PlaySound.MenuMove();
			selectedId -= 8;
			if (selectedId < 0)
			{
				selectedId += 32;
			}
		}
		else if (repeat.Down())
		{
			PlaySound.MenuMove();
			selectedId += 8;
			if (selectedId > 31)
			{
				selectedId -= 32;
			}
		}
		selectedItem = ((slotType == SlotType.Equipment) ? DM.Player.Equipment[selectedId] : DM.Player.Inventory[selectedId]);
		selectedItemPos = getPos(slotType, selectedId);
	}

	private static Vector2 getPos(SlotType type, int id)
	{
		return (type == SlotType.Equipment) ? (equipPos + equipOffset * id) : (invPos + 72f * new Vector2(id % 8, id / 8));
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		drawBackground();
		drawBanner();
		drawItemName();
		drawStats();
		drawEquipment();
		drawInventory();
		drawLantern();
		drawGP();
		drawShopTab();
		state.Draw(base.spriteBatch);
		base.spriteBatch.End();
	}

	private void drawBackground()
	{
		bg.Draw(base.spriteBatch, new Vector2(640f, 360f));
	}

	private void drawBanner()
	{
		Text.Draw(base.spriteBatch, new Vector2(212f, 114f), bannerText, Color.Black);
		Vector2 v = new Vector2(1042 - (Text.Width(DM.Player.HP) + 24 + Text.Width(DM.Player.MaxHP)), 114f);
		Text.Draw(base.spriteBatch, ref v, DM.Player.HP, Color.Black);
		Text.Draw(base.spriteBatch, ref v, '/', Color.Black);
		Text.Draw(base.spriteBatch, ref v, DM.Player.MaxHP, Color.Black);
	}

	private void drawItemName()
	{
		if (selectedItem != null)
		{
			Text.DrawCentered(base.spriteBatch, new Vector2(640f, 174f), selectedItem.Name, Color.Black);
		}
	}

	private void drawStats()
	{
		Vector2 vector = new Vector2(0f, 88f);
		for (int i = 0; i < 4; i++)
		{
			Text.DrawCentered(base.spriteBatch, new Vector2(206f, 236f) + vector * i, Stats.Name[i], Color.Black);
			Text.DrawCentered(base.spriteBatch, new Vector2(324f, 236f) + vector * i, DM.Player.Stats.ToString(i), Color.Black);
		}
	}

	private void drawEquipment()
	{
		for (int i = 0; i < 4; i++)
		{
			Item item = DM.Player.Equipment[i];
			Vector2 pos = getPos(SlotType.Equipment, i);
			if (item == null)
			{
				Rectangle value = new Rectangle(64 * i, 0, 64, 64);
				base.spriteBatch.Draw(equipmentBg, pos, value, Color.White, 0f, new Vector2(32f, 32f), 1f, SpriteEffects.None, 0f);
			}
			else
			{
				drawItem(item, pos);
			}
		}
	}

	private void drawInventory()
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				drawItem(DM.Player.Inventory[num], getPos(SlotType.Inventory, num));
				num++;
			}
		}
	}

	private void drawItem(Item item, Vector2 pos)
	{
		if (item != null)
		{
			item.Sprite.Draw(base.spriteBatch, pos, Color.White, 1f);
			if (!item.Identified)
			{
				unidentified.Draw(base.spriteBatch, pos);
			}
			else if (item.Cursed)
			{
				cursed.Draw(base.spriteBatch, pos);
			}
		}
	}

	private static void drawHighlight(SpriteBatch spriteBatch)
	{
		itemHighlight.Draw(spriteBatch, selectedItemPos);
	}

	private void drawLantern()
	{
		Vector2 v = new Vector2(226f, 596f);
		Text.Draw(base.spriteBatch, ref v, "Lamp:", Color.Black);
		Text.Draw(base.spriteBatch, ref v, (int)Math.Round((double)DM.Player.Lantern.Fuel * 100.0), Color.Black);
		Text.Draw(base.spriteBatch, ref v, '%', Color.Black);
	}

	private void drawGP()
	{
		Vector2 v = new Vector2(1078 - Text.Width(DM.Player.Gold), 596f);
		Text.Draw(base.spriteBatch, ref v, DM.Player.Gold, Color.Black);
	}

	private void drawShopTab()
	{
		if (shop != null)
		{
			shopTab.Draw(base.spriteBatch, new Vector2(640f, 612f));
			Text.Draw(base.spriteBatch, new Vector2(632f, 614f), "SHOP", shopTextShadowColor);
			Text.Draw(base.spriteBatch, new Vector2(630f, 612f), "SHOP", shopTextColor);
		}
	}

	private static State activate()
	{
		if (slotType == SlotType.Equipment)
		{
			DM.Player.Unequip(selectedId);
			return State.InfoState;
		}
		if (selectedItem is ScrollIdentify)
		{
			State.IdentifyState.SetScroll(selectedId);
			return State.IdentifyState;
		}
		DM.Player.Inventory[selectedId].onActivate(selectedId);
		return State.InfoState;
	}

	private static State sort()
	{
		DM.Player.SortInventory();
		return State.InfoState;
	}

	private static State destroy()
	{
		DM.Player.DestroyItem(selectedId);
		return State.InfoState;
	}
}
