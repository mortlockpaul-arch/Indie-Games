using System;
using System.Collections.Generic;
using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Potions;
using Loot.Items.Scrolls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class ShopScreen : Screen
{
	private enum SlotType
	{
		ShopTop,
		Shop,
		Player
	}

	private class Menu
	{
		public static Menu Buy = new Menu(new MenuItem("Buy", buyItem), new MenuItem("Sell Junk", quickSell), new MenuItem("Sort", sort));

		public static Menu Sell = new Menu(new MenuItem("Sell", sellItem), new MenuItem("Sell Junk", quickSell), new MenuItem("Sort", sort));

		public static Menu Sort = new Menu(new MenuItem("Sell Junk", quickSell), new MenuItem("Sort", sort));

		public static Menu IdentifyAll = new Menu(new MenuItem("Identify", identifyAll), new MenuItem("Sell Junk", quickSell), new MenuItem("Sort", sort));

		public static Menu IdentifyOne = new Menu(new MenuItem("Identify", identifyOne), new MenuItem("Sell Junk", quickSell), new MenuItem("Sort", sort));

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

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			itemHighlight.Draw(spriteBatch, selectedItemPos);
		}
	}

	private class InfoState : State
	{
		private Item lastItem;

		private bool showStats = true;

		private TimeSpan infoTimer = TimeSpan.Zero;

		private TimeSpan infoDuration = TimeSpan.FromSeconds(1.5);

		private Vector2 boxPos;

		private Color darkRed = new Color(128, 19, 19);

		private Color darkGreen = new Color(19, 128, 19);

		public override State Update(GameTime gameTime)
		{
			if (selectedItem != lastItem)
			{
				lastItem = selectedItem;
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
				if (slotType == SlotType.ShopTop)
				{
					switch (selectedId)
					{
					case 0:
						State.MenuState.Open(Menu.IdentifyAll);
						break;
					case 1:
						State.MenuState.Open(Menu.IdentifyOne);
						break;
					default:
						State.MenuState.Open(Menu.Buy);
						break;
					}
				}
				else if (selectedItem == null)
				{
					State.MenuState.Open(Menu.Sort);
				}
				else if (slotType == SlotType.Shop)
				{
					State.MenuState.Open(Menu.Buy);
				}
				else
				{
					State.MenuState.Open(Menu.Sell);
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
			base.Draw(spriteBatch);
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

		private int compare(Equipment item, int stat)
		{
			Stats cmp = DM.Player.GetEquipment(item.GetType())?.Stats ?? Stats.Zero;
			return item.Stats.CompareTo(stat, cmp);
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
				if (showStats)
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
	}

	private class MenuState : State
	{
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
			base.Draw(spriteBatch);
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
			Vector2 vector = boxPos + new Vector2(30f, 38f);
			Vector2 vector2 = boxPos + new Vector2(12f, 12f);
			Vector2 vector3 = new Vector2(0f, 48f);
			for (int i = 0; i < activeMenu.Length; i++)
			{
				Vector2 v = vector + vector3 * i;
				if (menuId == i)
				{
					itemMenuHighlight.Draw(spriteBatch, vector2 + vector3 * i);
				}
				Text.Draw(spriteBatch, ref v, activeMenu.GetName(i), Color.Black);
			}
		}
	}

	private class IdentifyState : State
	{
		public override bool UpdateSelectedItem => false;

		public override State Update(GameTime gameTime)
		{
			if (MC.GamePadManager.isNewButtonDown(Buttons.A))
			{
				PlaySound.MenuClick();
				if (selectedItem == null)
				{
					Message.Display("Select an item to identify.");
				}
				else if (selectedItem.Identified)
				{
					Message.Display("This item is already identified.");
				}
				else
				{
					selectedItem.Identify();
					DM.Player.Gold -= IdentifyPrice;
				}
				return State.InfoState;
			}
			if (MC.GamePadManager.isNewButtonDown(Buttons.B))
			{
				PlaySound.MenuCancel();
				return State.InfoState;
			}
			repeat.Update(gameTime);
			if (repeat.Right())
			{
				PlaySound.MenuMove();
				selectedId++;
				if (selectedId % 8 == 0)
				{
					selectedId -= 8;
				}
			}
			else if (repeat.Left())
			{
				PlaySound.MenuMove();
				if (selectedId % 8 == 0)
				{
					selectedId += 7;
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
			selectedItem = DM.Player.Inventory[selectedId];
			selectedItemPos = getPos(SlotType.Player, selectedId);
			return this;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			magnify.Draw(spriteBatch, selectedItemPos);
		}
	}

	private const int shopInvOffset = 72;

	private const int invOffset = 72;

	private const string inventoryText = "INVENTORY";

	private static ShopScreen shopScreen = new ShopScreen();

	private static Shop shop;

	private static State state;

	private static SlotType slotType;

	private static int selectedId;

	private static Item selectedItem;

	private static Vector2 selectedItemPos;

	private static string bannerText;

	private static GamePadRepeatUtil repeat;

	private static int IdentifyPrice = 25;

	private static Item[] shopTopItems = new Item[4]
	{
		null,
		null,
		new PotionHealth(),
		new PotionOil()
	};

	private static Sprite bg;

	private static Sprite unidentified;

	private static Sprite cursed;

	private static Sprite itemHighlight;

	private static Sprite itemInfoBox;

	private static Sprite itemMenuBox;

	private static Sprite itemMenuHighlight;

	private static Sprite magnify;

	private static Vector2 shopInvPos = new Vector2(192f, 260f);

	private static Vector2 invPos = new Vector2(584f, 260f);

	private Vector2 textShadowOffset = new Vector2(2f, 2f);

	private Color woodTextColor = new Color(102, 76, 51);

	private Color woodTextShadowColor = new Color(189, 157, 98);

	private Color goldTextColor = new Color(255, 202, 0);

	private Color goldTextShadowColor = new Color(151, 100, 0);

	private static List<Type> idTypes = new List<Type>();

	private static Color inventoryTextColor = new Color(128, 123, 102);

	private static Color inventoryTextShadowColor = new Color(229, 220, 184);

	private ShopScreen()
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
			slotType = SlotType.Player;
			selectedId = playerInventoryId;
			selectedItem = DM.Player.Inventory[selectedId];
		}
		else
		{
			slotType = SlotType.ShopTop;
			selectedId = 0;
			selectedItem = null;
		}
		selectedItemPos = getPos(slotType, selectedId);
		bannerText = "Ye Olde Goblin Shoppe";
		IdentifyPrice = 25 - (int)(25.0 * (double)DM.Player.ShopBonus);
		repeat = new GamePadRepeatUtil(MC.GamePadManager, TimeSpan.FromMilliseconds(300.0), TimeSpan.FromMilliseconds(150.0));
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(shopScreen);
	}

	public override void transitionOn()
	{
		BgMusic.ShopOpen();
	}

	private static void ExitScreen()
	{
		PlaySound.MenuClick();
		BgMusic.ShopClose();
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(new DungeonView());
	}

	public static void Load(ContentManager content)
	{
		bg = new StillSprite(content.Load<Texture2D>("Sprites\\UI\\ShopScreen"));
		unidentified = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Unidentified"));
		cursed = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Cursed"));
		itemHighlight = new AnimatedSprite(content.Load<Texture2D>("Sprites\\Inventory\\ItemHighlight"), new Rectangle(0, 0, 128, 128), new Vector2(64f, 64f), 8, TimeSpan.FromMilliseconds(75.0));
		itemInfoBox = new StillSprite(content.Load<Texture2D>("Sprites\\Inventory\\ItemInfoBox"), null, Vector2.Zero);
		itemMenuBox = new StillSprite(content.Load<Texture2D>("Sprites\\Inventory\\ItemMenuBox"), null, Vector2.Zero);
		itemMenuHighlight = new StillSprite(content.Load<Texture2D>("Sprites\\Inventory\\ItemMenuHighlight"), null, Vector2.Zero);
		magnify = new StillSprite(content.Load<Texture2D>("Sprites\\Inventory\\Magnify"), new Rectangle(0, 0, 128, 128), new Vector2(46f, 46f));
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.Back))
		{
			PlaySound.MenuClick();
			MC.ScreenManager.removeAllScreens();
			if (slotType == SlotType.Player)
			{
				InventoryScreen.Display(shop, selectedId);
			}
			else
			{
				InventoryScreen.Display(shop);
			}
		}
		else if (MC.GamePadManager.isNewButtonDown(Buttons.Start))
		{
			ExitScreen();
		}
		else
		{
			updateSprites(gameTime);
			if (state.UpdateSelectedItem)
			{
				updateSelectedItem(gameTime);
			}
			state = state.Update(gameTime);
		}
	}

	private void updateSprites(GameTime gameTime)
	{
		itemHighlight.Update(gameTime);
	}

	private void updateSelectedItem(GameTime gameTime)
	{
		repeat.Update(gameTime);
		switch (slotType)
		{
		case SlotType.ShopTop:
			if (repeat.Down())
			{
				PlaySound.MenuMove();
				slotType = SlotType.Shop;
			}
			else if (repeat.Up())
			{
				PlaySound.MenuMove();
				slotType = SlotType.Shop;
				selectedId += 8;
			}
			else if (repeat.Right())
			{
				PlaySound.MenuMove();
				selectedId++;
				if (selectedId > 3)
				{
					slotType = SlotType.Player;
					selectedId = 0;
				}
			}
			else if (repeat.Left())
			{
				PlaySound.MenuMove();
				selectedId--;
				if (selectedId < 0)
				{
					slotType = SlotType.Player;
					selectedId = 7;
				}
			}
			break;
		case SlotType.Shop:
			if (repeat.Down())
			{
				PlaySound.MenuMove();
				selectedId += 4;
				if (selectedId >= 12)
				{
					slotType = SlotType.ShopTop;
					selectedId -= 12;
				}
			}
			else if (repeat.Up())
			{
				PlaySound.MenuMove();
				selectedId -= 4;
				if (selectedId < 0)
				{
					slotType = SlotType.ShopTop;
					selectedId += 4;
				}
			}
			else if (repeat.Right())
			{
				PlaySound.MenuMove();
				selectedId++;
				if (selectedId % 4 == 0)
				{
					slotType = SlotType.Player;
					selectedId = (selectedId / 4 - 1) * 8 + 8;
				}
			}
			else if (repeat.Left())
			{
				PlaySound.MenuMove();
				if (selectedId % 4 == 0)
				{
					slotType = SlotType.Player;
					selectedId = (selectedId / 4 + 1) * 8 - 1 + 8;
				}
				else
				{
					selectedId--;
				}
			}
			break;
		case SlotType.Player:
			if (repeat.Right())
			{
				PlaySound.MenuMove();
				selectedId++;
				if (selectedId == 8)
				{
					slotType = SlotType.ShopTop;
					selectedId = 0;
				}
				else if (selectedId % 8 == 0)
				{
					selectedId = (selectedId / 8 - 2) * 4;
					slotType = SlotType.Shop;
				}
			}
			else if (repeat.Left())
			{
				PlaySound.MenuMove();
				if (selectedId == 0)
				{
					slotType = SlotType.ShopTop;
					selectedId = 3;
				}
				else if (selectedId % 8 == 0)
				{
					slotType = SlotType.Shop;
					selectedId = selectedId / 8 * 4 - 1;
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
			break;
		}
		selectedItem = ((slotType == SlotType.ShopTop) ? shopTopItems[selectedId] : ((slotType == SlotType.Shop) ? shop.Items[selectedId] : DM.Player.Inventory[selectedId]));
		selectedItemPos = getPos(slotType, selectedId);
	}

	private static Vector2 getPos(SlotType type, int id)
	{
		Vector2 result;
		switch (type)
		{
		case SlotType.ShopTop:
			return shopInvPos + new Vector2(id % 4, id / 4) * 72f;
		default:
			result = invPos + new Vector2(id % 8, id / 8) * 72f;
			break;
		case SlotType.Shop:
			result = shopInvPos + new Vector2(id % 4, (id + 4) / 4) * 72f;
			break;
		}
		return result;
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		drawBackground();
		drawBanner();
		drawBuySell();
		drawPriceTag();
		drawShopTop();
		drawShopInventory();
		drawPlayerInventory();
		drawInventoryButton();
		drawGP();
		state.Draw(base.spriteBatch);
		base.spriteBatch.End();
	}

	private void drawBackground()
	{
		bg.Draw(base.spriteBatch, new Vector2(640f, 360f));
	}

	private void drawWoodText(Vector2 v, string text)
	{
		Text.DrawCentered(base.spriteBatch, v + textShadowOffset, text, woodTextShadowColor);
		Text.DrawCentered(base.spriteBatch, v, text, woodTextColor);
	}

	private void drawGoldText(Vector2 v, string text)
	{
		Text.DrawCentered(base.spriteBatch, v + textShadowOffset, text, goldTextShadowColor);
		Text.DrawCentered(base.spriteBatch, v, text, goldTextColor);
	}

	private void drawBanner()
	{
		drawWoodText(new Vector2(486f, 114f), bannerText);
	}

	private void drawBuySell()
	{
		if (state == State.IdentifyState)
		{
			drawGoldText(new Vector2(980f, 126f), "Identify");
		}
		else if (slotType == SlotType.ShopTop)
		{
			drawGoldText(new Vector2(980f, 126f), "Buy For");
		}
		else if (selectedItem != null)
		{
			switch (slotType)
			{
			case SlotType.Shop:
				drawGoldText(new Vector2(980f, 126f), "Buy For");
				break;
			case SlotType.Player:
				drawGoldText(new Vector2(980f, 126f), "Sell For");
				break;
			}
		}
		drawWoodText(new Vector2(300f, 550f), "Buy");
		drawWoodText(new Vector2(980f, 550f), "Sell");
	}

	protected static int idAllPrice()
	{
		idTypes.Clear();
		int num = 0;
		for (int i = 0; i < DM.Player.Inventory.Length; i++)
		{
			Item item = DM.Player.Inventory[i];
			if (item == null || item.Identified)
			{
				continue;
			}
			if (item is Equipment)
			{
				num++;
				continue;
			}
			Type type = item.GetType();
			if (!idTypes.Contains(type))
			{
				idTypes.Add(type);
				num++;
			}
		}
		int result;
		switch (num)
		{
		case 0:
			return 0;
		default:
			result = (int)Math.Round((double)(IdentifyPrice * num) * 0.8999999761581421);
			break;
		case 1:
			result = IdentifyPrice;
			break;
		}
		return result;
	}

	private void drawPriceTag()
	{
		string s;
		int number;
		if (slotType == SlotType.ShopTop)
		{
			switch (selectedId)
			{
			case 0:
				s = "Identify All Items";
				number = idAllPrice();
				break;
			case 1:
				s = "Identify One Item";
				number = ((idAllPrice() != 0) ? IdentifyPrice : 0);
				break;
			default:
				s = selectedItem.Name;
				number = buyValue(selectedItem);
				break;
			}
		}
		else
		{
			if (selectedItem == null)
			{
				return;
			}
			if (state == State.IdentifyState)
			{
				if (selectedItem.Identified)
				{
					return;
				}
				s = selectedItem.Name;
				number = IdentifyPrice;
			}
			else if (slotType == SlotType.Shop)
			{
				s = selectedItem.Name;
				number = buyValue(selectedItem);
			}
			else
			{
				s = selectedItem.Name;
				number = sellValue(selectedItem);
			}
		}
		Text.Draw(base.spriteBatch, new Vector2(174f, 178f), s, Color.Black);
		Text.Draw(base.spriteBatch, new Vector2(1066 - Text.Width(number), 178f), number, Color.Black);
	}

	private void drawShopTop()
	{
		for (int i = 2; i < 4; i++)
		{
			drawItem(SlotType.ShopTop, shopTopItems[i], getPos(SlotType.ShopTop, i));
		}
	}

	private void drawShopInventory()
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				drawItem(SlotType.Shop, shop.Items[num], getPos(SlotType.Shop, num));
				num++;
			}
		}
	}

	private void drawPlayerInventory()
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				drawItem(SlotType.Player, DM.Player.Inventory[num], getPos(SlotType.Player, num));
				num++;
			}
		}
	}

	private void drawItem(SlotType type, Item item, Vector2 pos)
	{
		if (item != null)
		{
			item.Sprite.Draw(base.spriteBatch, pos);
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

	private void drawInventoryButton()
	{
		Text.Draw(base.spriteBatch, new Vector2(232f, 616f), "INVENTORY", inventoryTextShadowColor);
		Text.Draw(base.spriteBatch, new Vector2(230f, 614f), "INVENTORY", inventoryTextColor);
	}

	private void drawGP()
	{
		Vector2 v = new Vector2(1066 - Text.Width(DM.Player.Gold), 618f);
		Text.Draw(base.spriteBatch, ref v, DM.Player.Gold, Color.Black);
	}

	private static int buyValue(Item item)
	{
		return (item != null) ? ((int)(((DM.Player.Depth > 35) ? 1.5 : ((DM.Player.Depth <= 20) ? 1.0 : 1.25)) * (double)(item.Value - (int)((double)item.Value * (double)DM.Player.ShopBonus)))) : 0;
	}

	private static int sellValue(Item item)
	{
		if (item == null || !item.Identified || item.Cursed)
		{
			return 0;
		}
		int num = (int)(Math.Sqrt(item.Value) * 2.0);
		return num + (int)((double)num * (double)DM.Player.ShopBonus);
	}

	private static State sort()
	{
		DM.Player.SortInventory();
		shop.Sort();
		return State.InfoState;
	}

	private static State identifyAll()
	{
		int num = idAllPrice();
		if (num == 0)
		{
			Message.Display("All items are identified.");
			return State.InfoState;
		}
		if (DM.Player.Gold < num)
		{
			Message.Display("You can't afford it.");
			return State.InfoState;
		}
		for (int i = 0; i < DM.Player.Inventory.Length; i++)
		{
			Item item = DM.Player.Inventory[i];
			if (item != null && !item.Identified)
			{
				item.Identify();
			}
		}
		DM.Player.Gold -= num;
		return State.InfoState;
	}

	private static State identifyOne()
	{
		if (idAllPrice() == 0)
		{
			Message.Display("All items are identified.");
			return State.InfoState;
		}
		if (DM.Player.Gold < IdentifyPrice)
		{
			Message.Display("You can't afford it.");
			return State.InfoState;
		}
		slotType = SlotType.Player;
		selectedId = 0;
		selectedItem = DM.Player.Inventory[selectedId];
		selectedItemPos = getPos(SlotType.Player, selectedId);
		return State.IdentifyState;
	}

	private static State buyItem()
	{
		if (DM.Player.Gold < buyValue(selectedItem))
		{
			Message.Display("You can't afford it.");
			return State.InfoState;
		}
		if (slotType == SlotType.ShopTop)
		{
			Item item = selectedId switch
			{
				2 => new PotionHealth(), 
				3 => new PotionOil(), 
				_ => throw new Exception("Unknown ShopTop id: " + selectedId), 
			};
			if (DM.Player.AddItem(item))
			{
				PlaySound.PickUp();
				DM.Player.Gold -= buyValue(item);
			}
			else
			{
				Message.Display("Inventory full.");
			}
		}
		else
		{
			Item item2 = selectedItem;
			if (DM.Player.AddItem(item2))
			{
				PlaySound.PickUp();
				shop.Items[selectedId] = null;
				selectedItem = null;
				DM.Player.Gold -= buyValue(item2);
			}
			else
			{
				Message.Display("Inventory full.");
			}
		}
		return State.InfoState;
	}

	private static State sellItem()
	{
		if (!selectedItem.Cursed)
		{
			shop.AddItem(selectedItem);
			DM.Player.Gold += sellValue(selectedItem);
		}
		DM.Player.RemoveItem(selectedId);
		PlaySound.Coin();
		return State.InfoState;
	}

	private static bool isJunk(Item item)
	{
		if (!item.Identified)
		{
			return false;
		}
		if (!(item is Junk))
		{
			if (!(item is PotionDirePoison))
			{
				if (!(item is ScrollIllOmen))
				{
					if (item is Equipment equipment && equipment.IsJunk())
					{
						return true;
					}
					return false;
				}
				return true;
			}
			return true;
		}
		return true;
	}

	private static State quickSell()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < DM.Player.Inventory.Length; i++)
		{
			Item item = DM.Player.Inventory[i];
			if (item == null)
			{
				continue;
			}
			int num3 = sellValue(item);
			if (isJunk(item))
			{
				num++;
				if (!item.Cursed)
				{
					shop.AddItem(item);
					num2 += num3;
					DM.Player.Gold += num3;
				}
				DM.Player.RemoveItem(i);
			}
		}
		if (num == 0)
		{
			Message.Display("No items were sold.");
			return State.InfoState;
		}
		PlaySound.Coin();
		Message.Display("Sold " + num + " item" + ((num != 1) ? "s" : "") + " for " + num2 + " GP.");
		return State.InfoState;
	}
}
